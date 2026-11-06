using Domain.Common.Results;
using Domain.Features.WorkOrders.Billing;
using Infrastructure.src.Infrastructure.Data;
using MechanicShop.Test.Common.Invoices;

namespace MechanicShop.Domain.UnitTests.Invoices
{
    public class InvoiceLineItemTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            // Given
            Guid id = Guid.NewGuid();
            string description = "first line in the issued invoice";
            int quantity = 1;
            decimal unitPrice = 50m;

            var result = InvoiceLineItemFactory.CreateInvoiceLineItem(id, 1, description, quantity, unitPrice);

            Assert.True(result.IsSuccess);
            Assert.Equal(id, result.Value.InvoiceId);
            Assert.Equal(1, result.Value.LineNumber);
            Assert.Equal(description, result.Value.Description);
            Assert.Equal(quantity, result.Value.Quantity);
            Assert.Equal(unitPrice, result.Value.UnitPrice);
        }

        [Fact]
        public void Create_WithEmptyInvoiceId_ShouldFail()
        {
            // Given

            // When
            var result = InvoiceLineItemFactory.CreateInvoiceLineItem(Guid.Empty, 1, "description", 1, 10m);
            // Then
            Assert.True(result.IsError);
            Assert.Equal(InvoiceLineItemErrors.InvoiceIdRequired, result.TopError);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_WithInvalidLineNumber_ShouldFail(int lineNumber)
        {
            // Given

            // When
            var result = InvoiceLineItemFactory.CreateInvoiceLineItem(Guid.NewGuid(), lineNumber, "description", 1, 10m);
            // Then
            Assert.False(result.IsSuccess);
            Assert.Equal(InvoiceLineItemErrors.LineNumberInvalid, result.TopError);
        }
        //          if (string.IsNullOrWhiteSpace(description))
        //             {
        //                 return InvoiceLineItemErrors.DescriptionRequired;
        //             }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void Create_WithInvalidDescription_ShouldFail(string? desc)
        {
            // Given
            var result = InvoiceLineItemFactory.CreateInvoiceLineItem(Guid.NewGuid(), 1, desc, 1, 10m);
            // When

            // Then
            Assert.True(result.IsError);
            Assert.Equal(InvoiceLineItemErrors.DescriptionRequired,result.TopError);
        }

        //             if (quantity <= 0)
        //             {
        //                 return InvoiceLineItemErrors.QuantityInvalid;
        //             }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_WithInvalidQuantity_ShouldFail(int quantity)
        {
            // Given

            // When
            var result = InvoiceLineItemFactory.CreateInvoiceLineItem(Guid.NewGuid(), 3, "description", quantity, 10m);
            // Then
            Assert.False(result.IsSuccess);
            Assert.Equal(InvoiceLineItemErrors.QuantityInvalid, result.TopError);
        }

        // if (unitPrice <= 0)
        // {
        //     return InvoiceLineItemErrors.UnitPriceInvalid;
        // }
        [Fact]
        public void Create_WithInvalidUnitPrice_ShouldFail()
        {
            // Given

            // When
            var result = InvoiceLineItemFactory.CreateInvoiceLineItem(Guid.NewGuid(), 3, "description", 2, 0m);
            // Then
            Assert.False(result.IsSuccess);
            Assert.Equal(InvoiceLineItemErrors.UnitPriceInvalid, result.TopError);
        }

    }
}