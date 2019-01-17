using Domain.Entities;
using Domain.Infrastracture;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Repositories
{
    public class RouteLegRepository : RepositoryBase<RouteLeg>, IRouteLegRepository
    {
        public RouteLegRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }
    }

    public interface IRouteLegRepository : IRepository<RouteLeg>
    {
    }
}
