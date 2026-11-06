using Domain.Common.Results;
using Domain.Features.RepairTasks;
using Domain.Features.RepairTasks.Parts;
using MechanicShop.Test.Common.RepairTasks;

namespace MechanicShop.Domain.UnitTests.RepairTasks
{
    public class RepairTaskTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            // Given
            Guid id = Guid.NewGuid();
            string name = "a repair task";
            decimal laborCost = 50m;
            RepairTaskDurationInMinutes taskDuration = RepairTaskDurationInMinutes.Min30;
            List<Part> parts = [PartFactory.CreatePart().Value];

            var result = RepairTaskFactory.CreateRepairTask(id, name, laborCost, taskDuration, parts);

            Assert.True(result.IsSuccess);

            Assert.Equal(id, result.Value.Id);
            Assert.Equal(name, result.Value.Name);
            Assert.Equal(laborCost, result.Value.LaborCost);
            Assert.Equal(taskDuration, result.Value.EstimatedDurationInMins);
            Assert.Equal(parts, result.Value.Parts);
        }
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void Create_WithInvalidName_ShouldFail(string? invalidName)
        {
            // Given

            // When
            var result = RepairTaskFactory.CreateRepairTask(Name: invalidName);
            // Then
            Assert.True(result.IsError);
            Assert.Equal(RepairTaskErrors.NameRequired.Code, result.TopError.Code);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-20)]
        public void Create_WithInvalidLaborCost_ShouldFail(int invalidLaborCost)
        {
            // Given

            // When
            var result = RepairTaskFactory.CreateRepairTask(LaborCost: invalidLaborCost);
            // Then
            Assert.True(result.IsError);
            Assert.Equal(RepairTaskErrors.LaborCostInvalid.Code, result.TopError.Code);
        }
        [Theory]
        [InlineData(60)]
        public void Create_WithInvalidTaskDuration_ShouldFail(int invalidTaskDuration)
        {
            // Given

            // When
            var result = RepairTaskFactory.CreateRepairTask(DurationInMinutes: (RepairTaskDurationInMinutes)invalidTaskDuration);
            // Then
            Assert.True(result.IsError);
            Assert.Equal(RepairTaskErrors.DurationInvalid.Code, result.TopError.Code);
        }
        [Theory]
        [InlineData(null)]
        [MemberData(nameof(GetPartData))]
        public void Create_WithInvalidParts_ShouldFail(List<Part>? parts)
        {
            // Given

            // When
            var result = RepairTaskFactory.CreateRepairTask(Parts: parts);
            // Then
            Assert.True(result.IsError);
            Assert.Equal(RepairTaskErrors.PartsRequired.Code, result.TopError.Code);
        }

        //update

        [Fact]
        public void Update_WithValidData_ShouldSucceed()
        {
            // Given
            List<Part> parts = [PartFactory.CreatePart().Value];

            var originalTask = RepairTaskFactory.CreateRepairTask(Guid.NewGuid(), "original repair task", 20m, RepairTaskDurationInMinutes.Min60, parts).Value;


            // When
            var result = originalTask.Update("updated repair task", 50m, RepairTaskDurationInMinutes.Min120);
            // Then
            Assert.True(result.IsSuccess);
            Assert.Equal(Result.Updated, result.Value);
        }
        [Theory]
        [MemberData(nameof(GetInvalidDataForUpdate))]
        public void Update_ShouldReturnError_ForInvalidNameOrCost(string name, decimal cost, RepairTaskDurationInMinutes duration, bool expected)
        {
            var task = RepairTaskFactory.CreateRepairTask().Value;

            var result = task.Update(name, cost, duration);

            Assert.Equal(expected, result.IsSuccess);
        }

        [Fact]
        public void Update_WithInvalidTaskDuration_ShouldFail()
        {
            // Given
            var task = RepairTaskFactory.CreateRepairTask().Value;
            // When
            var result = task.Update("updated", 20m, (RepairTaskDurationInMinutes)70);
            // Then
            Assert.True(result.IsError);
            Assert.Equal(RepairTaskErrors.DurationInvalid, result.TopError);
        }

        [Fact]
        public void UpdateParts_RemovePartNotInIncomingParts_ShouldSucceed()
        {
            // Given
            var oldPart = PartFactory.CreatePart(Name: "old part").Value;
            var newPart = PartFactory.CreatePart(Name: "new part").Value;

            var oldTask = RepairTaskFactory.CreateRepairTask(Parts: [oldPart]).Value;
            // When
            var result = oldTask.UpdateRepairTaskParts([newPart]);
            // Then
            Assert.True(result.IsSuccess);
            Assert.Equal(oldTask.Parts.Single(), newPart);
        }

        [Fact]
        public void UpdateParts_WhenExistingUpdated_ShouldSucceed()
        {
            // Given
            var p1 = PartFactory.CreatePart(Name: "p1").Value;


            var task = RepairTaskFactory.CreateRepairTask(Parts: [p1]).Value;

            var incoming = PartFactory.CreatePart(Name: "updated").Value;

            // When

            var result = task.UpdateRepairTaskParts([incoming]);

            // Then
            Assert.True(result.IsSuccess);
            Assert.Equal(Result.Updated, result.Value);
            Assert.Equal("updated", task.Name);
        }

        public static IEnumerable<object[]>? GetInvalidDataForUpdate()
        {
            yield return ["", 20m, RepairTaskDurationInMinutes.Min105, false];
            yield return ["  ", 20m, RepairTaskDurationInMinutes.Min105, false];
            yield return ["abc", 0m, RepairTaskDurationInMinutes.Min105, false];
            yield return ["abc", 200000m, RepairTaskDurationInMinutes.Min105, false];

        }
        public static IEnumerable<object[]>? GetPartData()
        {
            yield return [new List<Part>()];
        }

        // public static Result<RepairTask> Create(Guid id, string name, decimal laborCost, RepairTaskDurationInMinutes estimatedDurationInMins, List<Part> parts)
    }
}