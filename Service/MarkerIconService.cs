
using DAL;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public interface IMarkerIconService
    {
        IEnumerable<MarkerIcon> GetMarkerIcons();
    }


    public class MarkerIconService : IMarkerIconService
    {
        private readonly LabContext db;
        private readonly DbSet<MarkerIcon> dbSet;

        public MarkerIconService (LabContext db_)
        {
            this.db = db_;
            this.dbSet = db_.Set<MarkerIcon>();
        }

        public IEnumerable<MarkerIcon> GetMarkerIcons()
        {
            return dbSet.ToList();
        }
    }
}
