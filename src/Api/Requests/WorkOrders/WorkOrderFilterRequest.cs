using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.WorkOrders;
using Domain.Features.WorkOrders.Enums;

namespace Api.Requests.WorkOrders
{
    public class WorkOrderFilterRequest
    {
          public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;        public string? SearchTerm { get; set; }
        public string SortColumn { get; set; } = "createdAt";
        public string SortDirection { get; set; } = "desc";
        public WorkOrderState? State { get; set; }
        public Guid? VehicleId { get; set; }
        public Guid? LaborId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Spot? Spot { get; set; }
    }
}