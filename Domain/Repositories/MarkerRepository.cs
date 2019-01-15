using Domain.Entities;
using Domain.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Repositories
{
    public class MarkerRepository : RepositoryBase<Marker>, IMarkerRepository
    {
        public MarkerRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }

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

    }

    public interface IMarkerRepository : IRepository<Marker>
    {
        int GetLastPointIndex (int customerId);
    }
}



