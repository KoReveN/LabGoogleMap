using DAL;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class CountryService
    {

        private readonly LabContext db;
        private readonly DbSet<Marker> dbSet;

        public CountryService(LabContext db_)
        {
            this.db = db_;
            this.dbSet = db_.Set<Marker>();
        }

        public void ImportCountries(IEnumerable<Country> counries)
        {
            //db.
        }

    }
}
