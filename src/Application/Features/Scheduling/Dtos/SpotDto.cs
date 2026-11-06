using Domain.Features.WorkOrders.Enums;

namespace Application.Features.Scheduling.Dtos
{
    public class SpotDto
    {
        public Spot Spot { get; set; }
        public List<SlotDto> Slots { get; set; } =[];
    }
}
