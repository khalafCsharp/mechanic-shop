using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.WorkOrders.Enums;

namespace Api.Requests.WorkOrders
{
    public class RelocateWorkOrderRequest
    {
        public DateTimeOffset NewStartAtUtc { get; set; }
        public Spot NewSpot { get; set; }
    }
}