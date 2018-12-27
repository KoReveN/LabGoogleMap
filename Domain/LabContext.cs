using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain
{

    public class LabContext : DbContext
    {

        public DbSet<Marker> Markers { get; set; }
        public DbSet<MarkerIcon> MarkerIcons { get; set; }




        public LabContext() 
        {
            //Database.EnsureDeleted();
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
                MarkerIcons.AddRange(
                        new List<MarkerIcon>() {
                            new MarkerIcon("Blue",   "http://maps.google.com/mapfiles/kml/paddle/blu-blank.png"),
                            new MarkerIcon("Red",    "http://maps.google.com/mapfiles/kml/paddle/red-blank.png"),
                            new MarkerIcon("Green",  "http://maps.google.com/mapfiles/kml/paddle/grn-blank.png"),
                            new MarkerIcon("Purple", "http://maps.google.com/mapfiles/kml/paddle/purple-blank.png"),
                            new MarkerIcon("White",  "http://maps.google.com/mapfiles/kml/paddle/wht-blank.png"),
                        }
                    );
                SaveChanges();
            }
        }

    }
}
