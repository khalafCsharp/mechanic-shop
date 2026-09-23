using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.WorkOrders;

namespace Api.Requests.WorkOrders
{
    public class UpdateWorkOrderStateRequest
    {
        public WorkOrderState State { get; set; }

    }
}