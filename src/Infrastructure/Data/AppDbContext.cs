using Application.Common.Interfaces;
using Domain.Common.Entities;
using Domain.Features.Customers;
using Domain.Features.Customers.Vehicles;
using Domain.Features.Employees;
using Domain.Features.Identity;
using Domain.Features.RepairTasks;
using Domain.Features.RepairTasks.Parts;
using Domain.Features.WorkOrders;
using Domain.Features.WorkOrders.Billing;
using Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>, IAppDbContext
    {
        public DbSet<Customer> Customers => Set<Customer>();

        public DbSet<Part> Parts => Set<Part>();

        public DbSet<RepairTask> RepairTasks => Set<RepairTask>();

        public DbSet<Vehicle> Vehicles => Set<Vehicle>();

        public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();

        public DbSet<Employee> Employees => Set<Employee>();

        public DbSet<Invoice> Invoices => Set<Invoice>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await DispatchDomainEventsAsync(cancellationToken);
            return await base.SaveChangesAsync(cancellationToken);
        }

        private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
        {
            var domainEntitiesThatHasEvents = ChangeTracker.Entries()
           .Where(e => e.Entity is BaseEntity baseEntity && baseEntity.DomainEvents.Count != 0)
           .Select(e => (BaseEntity)e.Entity)
           .ToList();

            var domainEvents = domainEntitiesThatHasEvents
                .SelectMany(e => e.DomainEvents)
                .ToList();

            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent, cancellationToken);
            }

            foreach (var entity in domainEntitiesThatHasEvents)
            {
                entity.ClearDomainEvents();
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator) : base(options)
        {
            this.mediator = mediator;
        }

        private readonly IMediator mediator;
    }
}