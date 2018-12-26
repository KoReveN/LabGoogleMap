using Domain.Entities;
using Domain.Infrastracture;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Repositories
{
    public class MarkerIconRepository : RepositoryBase<MarkerIcon>, IMarkerIconRepository
    {
        public MarkerIconRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }

    public interface IMarkerIconRepository : IRepository<MarkerIcon>
    {

    }


}
