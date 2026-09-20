using Application.Common.Interfaces;
using Application.Features.Labors.Mappers;
using Application.Features.RepairTasks.Mappers;
using Application.Features.Scheduling.Dtos;
using Domain.Common.Results;
using Domain.Features.Customers.Vehicles;
using Domain.Features.WorkOrders;
using Domain.Features.WorkOrders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Scheduling.Queries.GetDailyScheduleQuery
{
    public class GetDailyScheduleQueryHandler : IRequestHandler<GetDailyScheduleQuery, Result<ScheduleDto>>
    {
        private readonly IAppDbContext context;
        private readonly TimeProvider dateTime;

        public GetDailyScheduleQueryHandler(IAppDbContext context, TimeProvider dateTime)
        {
            this.context = context;
            this.dateTime = dateTime;
        }

        private static string? FormatVehicleInfo(Vehicle vehicle) =>
       vehicle != null ? $"{vehicle.Make} | {vehicle.LicensePlate}" : null;
        public async Task<Result<ScheduleDto>> Handle(GetDailyScheduleQuery request, CancellationToken cancellationToken)
        {
            var localStart = request.ScheduleDate.ToDateTime(TimeOnly.MinValue);
            var localEnd = localStart.AddDays(1);

            var utcStart = TimeZoneInfo.ConvertTimeToUtc(localStart, request.TimeZone);
            var utcEnd = TimeZoneInfo.ConvertTimeToUtc(localEnd, request.TimeZone);

            var workOrders = await context.WorkOrders
           .Where(w =>
               w.StartAtUtc < utcEnd &&
               w.EndAtUtc > utcStart &&
               (request.LaborId == null || w.LaborId == request.LaborId))
           .Include(w => w.RepairTasks)
           .Include(w => w.Vehicle)
           .Include(w => w.Labor)
           .ToListAsync(cancellationToken);

            var localNow = TimeZoneInfo.ConvertTime(dateTime.GetUtcNow(), request.TimeZone);

            var result = new ScheduleDto
            {
                ScheduleDate = request.ScheduleDate,
                IsEndOfDay = localEnd < localNow,
                Spots = []
            };

            foreach (Spot spot in Enum.GetValues<Spot>())
            {
                var currentTime = localStart;
                var slots = new List<SlotDto>();


                var workOrdersBySpot = workOrders
                    .Where(w => w.Spot == spot)
                    .OrderBy(w => w.StartAtUtc)
                    .ToList();

                while (currentTime < localEnd)
                {
                    var next = currentTime.AddMinutes(15);
                    var startUtc = TimeZoneInfo.ConvertTimeToUtc(currentTime, request.TimeZone);
                    var endUtc = TimeZoneInfo.ConvertTimeToUtc(next, request.TimeZone);

                    WorkOrder? workOrder = workOrdersBySpot.FirstOrDefault(w => w.StartAtUtc < endUtc && w.EndAtUtc > startUtc);

                    if (workOrder != null)
                    {
                        if (!slots.Any(s => s.WorkOrderId == workOrder.Id))
                        {
                            slots.Add(new SlotDto
                            {
                                WorkOrderId = workOrder.Id,
                                Spot = spot,
                                StartAtUtc = workOrder.StartAtUtc,
                                EndAtUtc = workOrder.EndAtUtc,
                                Vehicle = FormatVehicleInfo(workOrder.Vehicle!),
                                Labor = workOrder.Labor!.ToDto(),
                                IsOccupied = true,
                                RepairTasks = [.. workOrder.RepairTasks.ToList().ConvertAll(rt => rt.ToDto())],
                                IsWorkOrderLocked = !workOrder.IsEditable,
                                State = workOrder.State,
                                IsAvailable = false
                            });
                        }
                    }
                    else
                    {
                        slots.Add(new SlotDto
                        {
                            Spot = spot,
                            StartAtUtc = startUtc,
                            EndAtUtc = endUtc,
                            IsWorkOrderLocked = false,
                            IsAvailable = currentTime >= localNow
                        });
                    }
                    currentTime = next;
                }
                result.Spots.Add(new SpotDto
                {
                    Spot = spot,
                    Slots = slots
                });
            }
            return result;
        }
    }
}