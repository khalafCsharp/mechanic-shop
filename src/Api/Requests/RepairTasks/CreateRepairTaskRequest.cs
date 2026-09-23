using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.RepairTasks;

namespace Api.Requests.RepairTasks
{
    public class CreateRepairTaskRequest
    {

        [Required(ErrorMessage = "Task name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Labor cost is required.")]
        [Range(1, 10000, ErrorMessage = "Labor cost must be between 1 and 10,000.")]
        public decimal LaborCost { get; set; }

        [Required(ErrorMessage = "Estimated duration is required.")]
        public RepairTaskDurationInMinutes? EstimatedDurationInMins { get; set; }

        [MinLength(1, ErrorMessage = "At least one part is required.")]
        public List<CreatePartRequest> Parts { get; set; } = [];
    }
}