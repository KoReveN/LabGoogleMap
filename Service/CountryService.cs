using DAL;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public interface ICountryService
    {
        void ImportCountries(IEnumerable<Country> counries);
    }

    public class CountryService : ICountryService
    {

        private readonly LabContext db;
        private readonly DbSet<Country> dbSet;

        public CountryService(LabContext db_)
        {
            this.db = db_;
            this.dbSet = db_.Set<Country>();
        }

        public void ImportCountries(IEnumerable<Country> counries)
        {
            dbSet.AddRange(counries);
            db.SaveChanges();
        }

    }
}
