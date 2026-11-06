using Domain.Common.Results;
using Domain.Features.RepairTasks.Parts;
using MechanicShop.Test.Common.RepairTasks;

namespace MechanicShop.Domain.UnitTests.RepairTasks
{
    public class PartTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            // Given

            var result = PartFactory.CreatePart();

            Assert.True(result.IsSuccess);
            // When
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void Create_WithInvalidName_ShouldFail(string? invalidName)
        {
            var result = PartFactory.CreatePart(Name: invalidName);

            Assert.True(result.IsError);
            Assert.Equal(PartErrors.NameRequired.Code, result.TopError.Code);

        }
        [Theory]
        [InlineData(1000000)]
        [InlineData(-29)]
        public void Create_WithInvalidCost_ShouldFail(decimal invalidCost)
        {
            var result = PartFactory.CreatePart(Cost: invalidCost);

            Assert.True(result.IsError);
            Assert.Equal(PartErrors.CostInvalid.Code, result.TopError.Code);

        }
        [Theory]
        [InlineData(100)]
        [InlineData(-2)]
        public void Create_WithInvalidQuantity_ShouldFail(int invalidQuantity)
        {
            var result = PartFactory.CreatePart(Quantity: invalidQuantity);

            Assert.True(result.IsError);
            Assert.Equal(PartErrors.QuantityInvalid.Code, result.TopError.Code);

        }




        [Fact]
        public void Update_WithValidData_ShouldSucceed()
        {
            // Given
            Guid id = Guid.NewGuid();
            string name = "back-light";
            decimal cost = 199m;
            int quantity = 2;

            var oldPart = PartFactory.CreatePart(Id: id).Value;

            var result = oldPart.Update(name: name, cost: cost, quantity: quantity);

            Assert.True(result.IsSuccess);
            Assert.Equal(Result.Updated, result.Value);

            Assert.Equal(oldPart.Name, name);
            Assert.Equal(oldPart.Cost, cost);
            Assert.Equal(oldPart.Quantity, quantity);
            Assert.Equal(oldPart.Id, id);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void Update_WithInvalidName_ShouldFail(string? invalidName)
        {
            var part = PartFactory.CreatePart().Value;

            var result = part.Update(invalidName, part.Cost, part.Quantity);

            Assert.True(result.IsError);
            Assert.Equal(PartErrors.NameRequired.Code, result.TopError.Code);

        }
        [Theory]
        [InlineData(1000000)]
        [InlineData(-29)]
        public void Update_WithInvalidCost_ShouldFail(decimal invalidCost)
        {
            var originalPart = PartFactory.CreatePart().Value;

            var result = originalPart.Update("Front-light", invalidCost, 2);

            Assert.True(result.IsError);
            Assert.Equal(PartErrors.CostInvalid.Code, result.TopError.Code);

        }
        [Theory]
        [InlineData(100)]
        [InlineData(-2)]
        public void Update_WithInvalidQuantity_ShouldFail(int invalidQuantity)
        {
            var originalPart = PartFactory.CreatePart().Value;

            var result = originalPart.Update(originalPart.Name, originalPart.Cost, invalidQuantity);

            Assert.True(result.IsError);
            Assert.Equal(PartErrors.QuantityInvalid.Code, result.TopError.Code);

        }
    }
}