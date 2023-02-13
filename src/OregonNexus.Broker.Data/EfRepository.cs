// Copyright: 2023 Education Nexus Oregon
// Author: Makoa Jacobsen, makoa@makoajacobsen.com

using Ardalis.Specification.EntityFrameworkCore;
using OregonNexus.Broker.SharedKernel;

namespace OregonNexus.Broker.Data;  
  
// We are using the EfRepository from Ardalis.Specification
// https://github.com/ardalis/Specification/blob/v5/ArdalisSpecificationEF/src/Ardalis.Specification.EF/RepositoryBaseOfT.cs
public class EfRepository<T> : RepositoryBase<T>, IRepository<T> where T : class, IAggregateRoot
{
    public EfRepository(BrokerDbContext dbContext) : base(dbContext)
    {
    }
}