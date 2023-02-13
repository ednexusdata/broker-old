// Copyright: 2023 Education Nexus Oregon
// Author: Makoa Jacobsen, makoa@makoajacobsen.com

namespace OregonNexus.Broker.Data;

using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OregonNexus.Broker.Domain;

public class BrokerDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
    //private readonly IMediator _mediator;
    
    public BrokerDbContext(DbContextOptions<BrokerDbContext> options)  // , IMediator mediator
        : base(options)
    {
        //_mediator = mediator;
    }

    public DbSet<EducationOrganization>? EducationOrganizations { get; set; }
    public DbSet<User>? ApplicationUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
