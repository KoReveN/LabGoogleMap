using Domain.Entities;
using Domain.Repositories;
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
        private readonly IPointRepository pointRepository;
        public IConfiguration AppConfiguration { get; set; }

        public PointService(IPointRepository pointRepository, IConfiguration config)
        {
            this.pointRepository = pointRepository;
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
