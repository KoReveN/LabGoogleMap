using Domain.Entities;
using Domain.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Domain.Repositories
{
    public class PointRepository : RepositoryBase<Point>, IPointRepository
    {
        public PointRepository(IDbFactory dbFactory) : base(dbFactory)     {   }

    }

    public interface IPointRepository : IRepository<Point>
    {
    }
}


