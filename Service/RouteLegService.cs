using Domain.Entities;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Service.RequestModels;
using Microsoft.Extensions.Configuration;

namespace Service
{

    public interface IRouteLegService
    {
        MapSimpleRouteResponce GetRouteLegs(Marker[] markers);
        MapOtimizedRouteResponce GetGoogleOptimalRoute(MapOtimizedRouteRequest optimalRouteRequest);
    }

    public class RouteLegService : IRouteLegService
    {
        private readonly IRouteLegRepository routeLegRepository;
        public IConfiguration AppConfiguration { get; set; }

        GoogleApi.Direction googleApi;

        public RouteLegService(IRouteLegRepository routeLegRepository, IConfiguration config)
        {
            this.routeLegRepository = routeLegRepository;
            AppConfiguration = config;

            string key = AppConfiguration["googleApi:Key"];
            string url = AppConfiguration["googleApi:DirectionsUrl"];
            googleApi = new GoogleApi.Direction(key, url);
        }


        public MapSimpleRouteResponce GetRouteLegs(Marker[] markers)
        {
            List<RouteLeg> legsRequest = new List<RouteLeg>();
            List<RouteLeg> legsRepos, legsResponce;

            for (int i = 0; i < markers.Count() -1; i++)
            {
                RouteLeg leg = new RouteLeg() {
                    StartPoint = markers[i].PointId,
                    EndPoint = markers[i+1].PointId
                };

                legsRequest.Add(leg);
            }

            // Get All legs by this route from local DB
            legsResponce = routeLegRepository
                .GetMany(repLeg =>
                    legsRequest.Any(r =>
                        r.StartPoint == repLeg.StartPoint && r.EndPoint == repLeg.EndPoint
                    )).ToList();

            var legsFromDbCount = legsResponce.Count();

            legsRequest = legsRequest.Where(req => !legsResponce.Any(resp =>
                        resp.StartPoint == req.StartPoint && resp.EndPoint == req.EndPoint
                    )).ToList();

            foreach (var leg in legsRequest)
            {
                RouteLeg newLeg = GetLegByGoogleApi(markers.FirstOrDefault(m => m.PointId == leg.StartPoint),
                                    markers.FirstOrDefault(m => m.PointId == leg.EndPoint));
                legsResponce.Add(newLeg);
                routeLegRepository.Add(newLeg);
            }

            routeLegRepository.Save();

            //return legsResponce;
            return new MapSimpleRouteResponce()
            {
                Legs = legsResponce,
                LegsFromDbCount = legsFromDbCount
            };
        }



        private RouteLeg GetLegByGoogleApi(Marker startPoint, Marker endPoint)
        {
            RouteLeg leg = googleApi.GetLegByGoogleApi(startPoint.Point, endPoint.Point);
            leg.StartPoint = startPoint.PointId;
            leg.EndPoint = endPoint.PointId;

            return leg;
        }




        public MapOtimizedRouteResponce GetGoogleOptimalRoute(MapOtimizedRouteRequest optimalRouteRequest)
        {
            var startPoint = optimalRouteRequest.Markers.FirstOrDefault(marker => marker.MarkerType == MarkerType.StartPoint)?.Point;
            var endPoint = optimalRouteRequest.Markers.FirstOrDefault(marker => marker.MarkerType == MarkerType.EndPoint)?.Point;
            var wayPoints = optimalRouteRequest.Markers.Where(marker => marker.MarkerType == MarkerType.WayPoint).Select(x => x.Point);

            string options = "&departure_time=now";
            options += "&traffic_model=" + optimalRouteRequest.OptimalRouteOption.ToString();


            var apiResponce = googleApi.GetGoogleOptimalRoute(startPoint, endPoint, wayPoints, options);


            var markers = MarkerService.MarkersWaypointsReOrder(optimalRouteRequest.Markers, apiResponce.WaypointOrder.ToList()).OrderBy(x => x.Index).ToArray();
            var legs = apiResponce.Legs;
            for (int i = 0; i < markers.Count() - 1; i++)
            {
                legs[i].StartPoint = markers[i].PointId;
                legs[i].EndPoint = markers[i + 1].PointId;
            }

            return new MapOtimizedRouteResponce()
            {
                Legs = legs,
                Markers = markers,
                Polyline = apiResponce.Polyline
            };


        }
        
    }

}
