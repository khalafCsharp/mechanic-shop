using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common.Results;
using Domain.Features.WorkOrders.Billing;
using MechanicShop.Test.Common;
using MechanicShop.Test.Common.Invoices;
using Microsoft.AspNetCore.Http;

namespace MechanicShop.Domain.UnitTests.Invoices
{
    public class InvoiceTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var result = InvoiceFactory.CreateInvoice();

            Assert.True(result.IsSuccess);
        }
        [Fact]
        public void Create_WithEmptyWorkOrderId_ShouldFail()
        {
            var result = InvoiceFactory.CreateInvoice(WorkOrderId: Guid.Empty);

            Assert.True(result.IsError);
            Assert.Equal(InvoiceErrors.WorkOrderIdInvalid, result.TopError);
        }
        [Theory]
        [MemberData(nameof(GetLineItems))]
        public void Create_WithEmptyOrNullInvoiceLineItems_ShouldFail(List<InvoiceLineItem>? items)
        {
            var result = InvoiceFactory.CreateInvoice(Items: items);

            Assert.True(result.IsError);
            Assert.Equal(InvoiceErrors.LineItemsEmpty, result.TopError);
        }

        [Fact]
        public void ApplyDiscount_WithValidData_ShouldSucceed()
        {
            // Given
            var originalInvoice = InvoiceFactory.CreateInvoice().Value;

            // When
            var result = originalInvoice.ApplyDiscount(1m);

            // Then
            Assert.True(result.IsSuccess);
            Assert.Equal(Result.Updated, result.Value);
        }
        [Fact]
        public void ApplyDiscount_WithPaidState_ShouldFail()
        {
            // Given

            var invoice = InvoiceFactory.CreateInvoice().Value;

            var pay = invoice.MarkAsPaid(timeProvider: TimeProvider.System);
            Assert.Equal(Result.Updated, pay.Value);
            Assert.True(pay.IsSuccess);

            var applyDiscountResult = invoice.ApplyDiscount(10m);

            Assert.True(applyDiscountResult.IsError);
            Assert.Equal(InvoiceErrors.InvoiceLocked, applyDiscountResult.TopError);
        }

        [Fact]
        public void MarkAsPaid_ShouldSucceed()
        {
            // Given
            var invoice = InvoiceFactory.CreateInvoice().Value;

            var time = new FakeTimeProvider();
            time.SetUtcNow(DateTimeOffset.Parse("2022-10-10T00:00:00z"));
            // When
            var result = invoice.MarkAsPaid(time);
            // Then
            Assert.True(result.IsSuccess);
        }



        public static IEnumerable<object[]?> GetLineItems()
        {
            yield return null;
            yield return [];
        }
        // Guid id,
        //    Guid workOrderId,
        //     List<InvoiceLineItem> items,
        //     decimal discountAmount,
        //     decimal taxAmount,
        //     TimeProvider datetime)
    }
}