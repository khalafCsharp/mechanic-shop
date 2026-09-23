using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Requests.WorkOrders
{
    public class ModifyRepairTaskRequest
    {
        public Guid[] RepairTaskIds { get; set; } = [];
    }
}