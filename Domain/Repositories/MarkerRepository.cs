using Domain.Entities;
using Domain.Infrastracture;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Domain.Repositories
{
    public class MarkerRepository : RepositoryBase<Marker>, IMarkerRepository
    {
        public MarkerRepository(IDbFactory dbFactory) : base(dbFactory) {  }

        public int GetLastPointIndex(int customerId)
        {
            try
            {
                return this.DbContext.Markers
                    .Where(m => m.CustomerID == customerId && m.MarkerType == MarkerType.WayPoint).Max(m => m.Index);
            }
            catch (System.Exception)
            {
               return 0;
            }
        }


        public override IEnumerable<Marker> GetMany(Expression<Func<Marker, bool>> where)
        {
            return this.DbContext.Markers.Include(x => x.Point).Where(where).ToList();
        }

    }

    public interface IMarkerRepository : IRepository<Marker>
    {
        int GetLastPointIndex (int customerId);
    }
}



