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
        public string? SearchTerm { get; set; }
        public string SortColumn { get; set; } = "createdAt";
        public string SortDirection { get; set; } = "desc";
        public WorkOrderState? State { get; set; }
        public Guid? VehicleId { get; set; }
        public Guid? LaborId { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public Spot? Spot { get; set; }
    }
}