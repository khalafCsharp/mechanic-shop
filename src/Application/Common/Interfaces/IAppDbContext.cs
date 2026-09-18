using Domain.Features.Customers;
using Domain.Features.Customers.Vehicles;
using Domain.Features.Employees;
using Domain.Features.Identity;
using Domain.Features.RepairTasks;
using Domain.Features.RepairTasks.Parts;
using Domain.Features.WorkOrders;
using Domain.Features.WorkOrders.Billing;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces
{
    public interface IAppDbContext
    {
     DbSet<Customer> Customers { get; }
     DbSet<Part> Parts { get; }
     DbSet<RepairTask> RepairTasks { get; }
     DbSet<Vehicle> Vehicles { get; }
     DbSet<WorkOrder> WorkOrders { get; }
     DbSet<Employee> Employees { get; }
     DbSet<Invoice> Invoices { get; }
     DbSet<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}