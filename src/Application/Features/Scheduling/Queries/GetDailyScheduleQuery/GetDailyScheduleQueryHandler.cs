namespace MechanicShop.Application.Features.Scheduling.Queries.GetDailyScheduleQuery
{
    using Domain.Common.Results;
    using Domain.Features.Customers.Vehicles;
    using Domain.Features.WorkOrders.Enums;
    using global::Application.Common.Interfaces;
    using global::Application.Features.Labors.Mappers;
    using global::Application.Features.RepairTasks.Mappers;
    using global::Application.Features.Scheduling.Dtos;
    using MediatR;
    using Microsoft.EntityFrameworkCore;

    public class GetDailyScheduleQueryHandler(
        IAppDbContext context,
        TimeProvider datetime)
        : IRequestHandler<GetDailyScheduleQuery, Result<ScheduleDto>>
    {
        private readonly IAppDbContext context = context;
        private readonly TimeProvider datetime = datetime;

        public async Task<Result<ScheduleDto>> Handle(GetDailyScheduleQuery query, CancellationToken ct)
        {
            var localStart = query.ScheduleDate.ToDateTime(TimeOnly.MinValue);
            var localEnd = localStart.AddDays(1);

            var utcStart = TimeZoneInfo.ConvertTimeToUtc(localStart, query.TimeZone);
            var utcEnd = TimeZoneInfo.ConvertTimeToUtc(localEnd, query.TimeZone);

            var workOrders = await this.context.WorkOrders
                .Where(w =>
                    w.StartAtUtc < utcEnd &&
                    w.EndAtUtc > utcStart &&
                    (query.LaborId == null || w.LaborId == query.LaborId))
                .Include(w => w.RepairTasks)
                .Include(w => w.Vehicle)
                .Include(w => w.Labor)
                .ToListAsync(ct);

            var now = TimeZoneInfo.ConvertTime(this.datetime.GetUtcNow(), query.TimeZone);

            var result = new ScheduleDto
            {
                ScheduleDate = query.ScheduleDate,
                IsEndOfDay = localEnd < now,
                Spots = [],
            };

            foreach (var spot in Enum.GetValues<Spot>())
            {
                var current = localStart;
                var slots = new List<SlotDto>();

                var woBySpot = workOrders
                    .Where(w => w.Spot == spot)

                    .OrderBy(w => w.StartAtUtc)
                    .ToList();

                while (current < localEnd)
                {
                    var next = current.AddMinutes(15);
                    var startUtc = TimeZoneInfo.ConvertTimeToUtc(current, query.TimeZone);
                    var endUtc = TimeZoneInfo.ConvertTimeToUtc(next, query.TimeZone);

                    var wo = woBySpot.FirstOrDefault(w =>
                        w.StartAtUtc < endUtc && w.EndAtUtc > startUtc);

                    if (wo != null)
                    {
                        if (!slots.Any(s => s.WorkOrderId == wo.Id))
                        {
                            slots.Add(new SlotDto
                            {
                                WorkOrderId = wo.Id,
                                Spot = spot,
                                StartAtUtc = wo.StartAtUtc,
                                EndAtUtc = wo.EndAtUtc,
                                Vehicle = FormatVehicleInfo(wo.Vehicle!),
                                Labor = wo.Labor!.ToDto(),
                                IsOccupied = true,
                                RepairTasks = [.. wo.RepairTasks.ToList().ConvertAll(rt => rt.ToDto())],
                                IsWorkOrderLocked = !wo.IsEditable,
                                State = wo.State,
                                IsAvailable = false,
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
                            IsAvailable = current >= now,
                        });
                    }

                    current = next;
                }

                result.Spots.Add(new SpotDto
                {
                    Spot = spot,
                    Slots = slots,
                });
            }

            return result;
        }

        private static string? FormatVehicleInfo(Vehicle vehicle) =>
            vehicle != null ? $"{vehicle.Make} | {vehicle.LicensePlate}" : null;
    }
}
