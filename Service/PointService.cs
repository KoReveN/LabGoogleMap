using DAL;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{

    public interface IPointService
    {
        string GetPointAddress(Point point);
    }

    public class PointService : IPointService
    {
        //private readonly LabContext db;
        //private readonly DbSet<Point> dbSet;
        public IConfiguration AppConfiguration { get; set; }

        public PointService(LabContext db_, IConfiguration config)
        {
            //this.db = db_;
            //this.dbSet = db_.Set<Point>();
            AppConfiguration = config;
        }


        public string GetPointAddress(Point point)
        {
            string key = AppConfiguration["googleApi:Key"];
            string geocodeUrl = AppConfiguration["googleApi:GeocodeUrl"];
            var googleApi = new GoogleApi.Geocode(key, geocodeUrl);

            return googleApi.GetAddress(point);
        }



    }
}
