namespace Application.Features.Dashboard.Dtos
{
    public record DailyWorkOrderStatsDto
    {
        public DateOnly StatsDate { get; init; }
        public int TotalWorkOrders { get; init; }
        public int ScheduledWorkOrders { get; init; }
        public int InProgressWorkOrders { get; init; }
        public int CompletedWorkOrders { get; init; }
        public int CancelledWorkOrders { get; init; }
        public decimal TotalRevenue { get; init; }
        public decimal TotalPartsCost { get; init; }
        public decimal TotalLaborCost { get; init; }
        public int UniqueVehicles { get; init; }
        public int UniqueCustomers { get; init; }
        public decimal NetProfit { get; init; }
        public decimal ProfitMargin { get; init; }
        public decimal CompletionRate { get; init; }
        public decimal AverageRevenuePerOrder { get; init; }
        public decimal OrdersPerVehicle { get; init; }
        public decimal PartsCostRatio { get; init; }
        public decimal LaborCostRatio { get; init; }
        public decimal CancellationRate { get; init; }
    }
}