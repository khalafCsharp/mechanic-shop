using Application.Features.WorkOrders.Mappers;
using Domain.Features.WorkOrders;
using MechanicShop.Test.Common.Customers;
using MechanicShop.Test.Common.Employees;
using MechanicShop.Test.Common.Invoices;
using MechanicShop.Test.Common.RepairTasks;
using MechanicShop.Test.Common.WorkOrders;

namespace MechanicShop.Application.UnitTests.Mappers
{
    public class WorkOrderMapperTests
    {
        [Fact]
        public void ToDto_ShouldMapCorrectly()
        {
            var customer = CustomerFactory.CreateCustomer().Value;
            var labor = EmployeeFactory.CreateLabor().Value;
            var vehicle = customer.Vehicles.First();
            var part = PartFactory.CreatePart(Cost: 100, Quantity: 2).Value;

            var repairTask = RepairTaskFactory.CreateRepairTask(
                LaborCost: 150m,
                Parts: [part]).Value;

            var totalPartsCost = part.Cost * part.Quantity;
            var totalLaborCost = repairTask.LaborCost;
            var totalCost = totalPartsCost + totalLaborCost;
            var duration = (int)repairTask.EstimatedDurationInMins;

            var invoiceLine = InvoiceLineItemFactory.CreateInvoiceLineItem(
                LineNumber: 1,
                Description: "some description",
                Quantity: 1,
                UnitPrice: totalCost).Value;

            var invoice = InvoiceFactory.CreateInvoice(
                WorkOrderId: Guid.NewGuid(),
                Items: [invoiceLine]).Value;

            var workOrder = WorkOrderFactory.CreateWorkOrder(
                VehicleId: vehicle.Id,
                LaborId: labor.Id,
                RepairTasks: [repairTask]).Value;

            workOrder.Vehicle = vehicle;
            workOrder.Labor = labor;
            workOrder.Invoice = invoice;

            var dto = workOrder.ToDto();

            Assert.Equal(workOrder.Id, dto.WorkOrderId);
            Assert.Equal(workOrder.Spot, dto.Spot);
            Assert.Equal(workOrder.StartAtUtc, dto.StartAtUtc);
            Assert.Equal(workOrder.EndAtUtc, dto.EndAtUtc);
            Assert.Equal(workOrder.State, dto.State);
            Assert.Equal(workOrder.CreatedAtUtc, dto.CreatedAt);

            Assert.NotNull(dto.Labor);
            Assert.Equal(workOrder.LaborId, dto.Labor!.LaborId);
            Assert.Equal($"{labor.FirstName} {labor.LastName}", dto.Labor.Name);

            Assert.NotNull(dto.Vehicle);
            Assert.Equal(vehicle.Id, dto.Vehicle!.VehicleId);
            Assert.Equal(vehicle.Make, dto.Vehicle.Make);
            Assert.Equal(vehicle.Model, dto.Vehicle.Model);
            Assert.Equal(vehicle.Year, dto.Vehicle.Year);
            Assert.Equal(vehicle.LicensePlate, dto.Vehicle.LicensePlate);

            Assert.Single(dto.RepairTasks);
            Assert.Equal(totalPartsCost, dto.TotalPartCost);
            Assert.Equal(totalLaborCost, dto.TotalLaborCost);
            Assert.Equal(totalCost, dto.TotalCost);
            Assert.Equal(duration, dto.TotalDurationInMins);
            Assert.Equal(invoice.Id, dto.InvoiceId);
        }

        [Fact]
        public void ToDtos_ShouldMapListCorrectly()
        {
            // Arrange
            var customer = CustomerFactory.CreateCustomer().Value;
            var labor = EmployeeFactory.CreateLabor().Value;
            var vehicle = customer.Vehicles.First();

            var repairTask = RepairTaskFactory.CreateRepairTask(
                LaborCost: 100m,
                Parts: [PartFactory.CreatePart(Cost: 50, Quantity: 1).Value]).Value;

            var workOrder = WorkOrderFactory.CreateWorkOrder(VehicleId: vehicle.Id, LaborId: labor.Id, RepairTasks: [repairTask]).Value;

            workOrder.Vehicle = vehicle;
            workOrder.Labor = labor;

            var workOrders = new List<WorkOrder> { workOrder };

            // Act
            var dtos = workOrders.ToDtos();

            // Assert
            Assert.Single(dtos);
            var dto = dtos[0];

            Assert.Equal(workOrder.Id, dto.WorkOrderId);
            Assert.Equal(workOrder.Spot, dto.Spot);
            Assert.Equal(workOrder.StartAtUtc, dto.StartAtUtc);
            Assert.Equal(workOrder.EndAtUtc, dto.EndAtUtc);
            Assert.NotNull(dto.Labor);
            Assert.Equal($"{labor.FirstName} {labor.LastName}", dto.Labor!.Name);
            Assert.Equal(labor.Id, dto.Labor.LaborId);
            Assert.NotNull(dto.Vehicle);
            Assert.Single(dto.RepairTasks);
            Assert.Equal(workOrder.State, dto.State);
        }

       
    }
}