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
    }

    public interface IMarkerRepository : IRepository<Marker>
    {

    }
}



