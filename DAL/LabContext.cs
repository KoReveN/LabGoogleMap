using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL
{

    public class LabContext : DbContext
    {

        public DbSet<Marker> Markers { get; set; }
        public DbSet<MarkerIcon> MarkerIcons { get; set; }
        public DbSet<RouteLeg> RouteLegs { get; set; }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Place> Places { get; set; }



        public LabContext(DbContextOptions<LabContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            //https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet
            Database.EnsureCreated();
            DatabaseSeed();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(LocalDB)\MSSQLLocalDB;Database=study_google_map;integrated security=True;");
        }


        private void DatabaseSeed()
        {
            if (!this.MarkerIcons.Any())
            {
                SeedDb.MarketIconsSeed(this);
            }

            if (!this.Countries.Any())
            {
                SeedDb.CountriesSeed(this);
            }
        }

    }
}
