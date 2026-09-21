using Application.Common.Interfaces;
using Domain.Features.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.WorkOrders.EventHandlers
{
    public class SendWorkOrderCompletedNotificationsHandler : INotificationHandler<WorkOrderCompleted>
    {
        private readonly IAppDbContext context;
        private readonly ILogger<SendWorkOrderCompletedNotificationsHandler> logger;
        private readonly INotificationService notificationService;

        public SendWorkOrderCompletedNotificationsHandler(IAppDbContext context, ILogger<SendWorkOrderCompletedNotificationsHandler> logger, INotificationService notificationService)
        {
            this.context = context;
            this.logger = logger;
            this.notificationService = notificationService;
        }
        public async Task Handle(WorkOrderCompleted notification, CancellationToken cancellationToken)
        {
            var workOrder = await context.WorkOrders
                       .Include(w => w.Vehicle!).ThenInclude(v => v.Customer)
                       .AsNoTracking()
                       .FirstOrDefaultAsync(w => w.Id == notification.WorkOrderId, cancellationToken);


            if (workOrder is null)
            {
                logger.LogError("WorkOrder with id: {workOrderId} does not exist.", notification.WorkOrderId);
                return;
            }
            await notificationService.SendEmailAsync(workOrder.Vehicle?.Customer?.Email!, cancellationToken);
            await notificationService.SendSmsAsync(workOrder.Vehicle?.Customer?.PhoneNumber!, cancellationToken);
        }
    }
}