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
        IEnumerable<RouteLeg> GetRouteLegs(Marker[] markers);
        MapOtimizedRouteResponce GetGoogleOptimalRoute(MapOptimalRouteRequest optimalRouteRequest);
    }

    public class RouteLegService : IRouteLegService
    {
        private readonly IRouteLegRepository routeLegRepository;
        public IConfiguration AppConfiguration { get; set; }

        public RouteLegService(IRouteLegRepository routeLegRepository, IConfiguration config)
        {
            this.routeLegRepository = routeLegRepository;
            AppConfiguration = config;
        }


        public IEnumerable<RouteLeg> GetRouteLegs(Marker[] markers)
        {
            List<RouteLeg> legsRequest = new List<RouteLeg>();
            List<RouteLeg> legsRepos = new List<RouteLeg>();
            List<RouteLeg> legsResponce = new List<RouteLeg>();


            for (int i = 0; i < markers.Count() -1; i++)
            {
                RouteLeg leg = new RouteLeg() {
                    StartPoint = markers[i].PointId,
                    EndPoint = markers[i+1].PointId
                };

                legsRequest.Add(leg);
            }

            // Get All legs by this route from local DB
            legsRepos = routeLegRepository
                .GetMany(repLeg =>
                    legsRequest.Any(r =>
                        r.StartPoint == repLeg.StartPoint && r.EndPoint == repLeg.EndPoint
                    )).ToList();



            foreach (var leg in legsRequest)
            {
               var legFill = legsRepos.FirstOrDefault(x => x.StartPoint == leg.StartPoint && x.EndPoint == leg.EndPoint);

                if (legFill == null)
                {
                    // Google api request for new leg
                    RouteLeg newLeg = GetLegByGoogleApi(markers.FirstOrDefault(m => m.PointId == leg.StartPoint),
                                                        markers.FirstOrDefault(m => m.PointId == leg.EndPoint));
                    legsResponce.Add(newLeg);
                    routeLegRepository.Add(newLeg);
                    
                } else
                {
                    legsResponce.Add(legFill);
                }
            }

            routeLegRepository.Save();

            return legsResponce;
        }



        private RouteLeg GetLegByGoogleApi(Marker startPoint, Marker endPoint)
        {
            RouteLeg leg = new RouteLeg();

            string origin = $"origin={startPoint.Point.Lat},{startPoint.Point.Lng}";
            string destination = $"destination={endPoint.Point.Lat},{endPoint.Point.Lng}";

            string key = AppConfiguration["googleApi.Key"];  //@"AIzaSyA3YhAyyckDAMFGuVR7yRI-fG_NATvL8Yk";

            string url = AppConfiguration["googleApi.DirectionsUrl"] + //@"https://maps.googleapis.com/maps/api/directions/json?" + 
                origin + "&" + destination + "&key=" + key;

            WebRequest request = WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            request.Method = WebRequestMethods.Http.Get;

            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();
            StreamReader reader = new StreamReader(data);

            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            data.Close();
            response.Close();

            JObject jObj = JObject.Parse(responseFromServer);

            JToken jRoute = jObj["routes"].FirstOrDefault();
            JToken jLeg = jRoute.SelectToken("legs").FirstOrDefault();

            leg.Polyline = jRoute.SelectToken("overview_polyline").SelectToken("points").ToString();



            leg.StartPoint = startPoint.PointId;
            leg.EndPoint = endPoint.PointId;
            leg.Distance = (int)jLeg.SelectToken("distance").SelectToken("value");
            leg.Duration = (int)jLeg.SelectToken("duration").SelectToken("value");
            //string address = jObj["results"].FirstOrDefault().SelectToken("formatted_address").ToString();



            return leg;
        }



        
        public MapOtimizedRouteResponce GetGoogleOptimalRoute(MapOptimalRouteRequest optimalRouteRequest)
        {
            string origin = "", destination = "", wayPoints = "";
            //List<string> wayPoints = new List<string>();

            //var routeOptionsDataSource = ["Pissimistic","Best guess","Optimistic"];


            // traffic_model : best_guess  / pessimistic  / optimistic 
            //best_guess: he value returned in the duration_in_traffic field in the response, 
            //which contains the predicted time in traffic based on historical averages

            // MUST BE:
            //  travel mode is driving       => &mode=driving (default)
            // and where the request includes a departure_time  => &departure_time=now


            string options = "&departure_time=now";
            options += "&traffic_model=" + optimalRouteRequest.OptimalRouteOption.ToString();


            foreach (Marker marker in optimalRouteRequest.Markers)
            {
                switch (marker.MarkerType)
                {
                    case MarkerType.WayPoint:
                        if (string.IsNullOrEmpty(wayPoints))
                        {
                            wayPoints += $"&waypoints=optimize:true|{marker.Point.Lat},{marker.Point.Lng}";
                        }
                        else
                        {
                            wayPoints += $"|{marker.Point.Lat},{marker.Point.Lng}";
                        }
                        break;
                    case MarkerType.StartPoint:
                        origin = $"origin={marker.Point.Lat},{marker.Point.Lng}";
                        break;
                    case MarkerType.EndPoint:
                        destination = $"&destination={marker.Point.Lat},{marker.Point.Lng}";
                        break;
                }
            }

            string key = AppConfiguration["googleApi.Key"];//@"AIzaSyA3YhAyyckDAMFGuVR7yRI-fG_NATvL8Yk";
            string url = AppConfiguration["googleApi.DirectionsUrl"] + // @"https://maps.googleapis.com/maps/api/directions/json?" 
                 origin +
                 destination +
                 wayPoints + options +
                "&key=" + key;

            WebRequest request = WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            request.Method = WebRequestMethods.Http.Get;

            WebResponse webResponse = request.GetResponse();
            Stream data = webResponse.GetResponseStream();
            StreamReader reader = new StreamReader(data);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            data.Close();
            webResponse.Close();


            JObject jObj = JObject.Parse(responseFromServer);
            JToken jRoute = jObj["routes"].FirstOrDefault();

           string polyline = jRoute.SelectToken("overview_polyline").SelectToken("points").ToString();
           var jLegs = jRoute.SelectToken("legs");

            var legs = new List<RouteLeg>();

            foreach (var jLeg in jLegs)
            {
                RouteLeg leg = new RouteLeg();

                //float sLat = (float)jLeg.SelectToken("start_location").SelectToken("lat");
                //float sLng = (float)jLeg.SelectToken("start_location").SelectToken("lng");
                //float eLat = (float)jLeg.SelectToken("end_location").SelectToken("lat");
                //float eLng = (float)jLeg.SelectToken("end_location").SelectToken("lng");

                //leg.StartPoint = optimalRouteRequest.Markers.FirstOrDefault(x => x.Point.Lat == sLat && x.Point.Lng == sLng)?.PointId ?? 0;
                //leg.EndPoint   = optimalRouteRequest.Markers.FirstOrDefault(x => x.Point.Lat == eLat && x.Point.Lng == eLng)?.PointId ?? 0;

                leg.Distance = (int)jLeg.SelectToken("distance").SelectToken("value");
                leg.Duration = (int)jLeg.SelectToken("duration").SelectToken("value");

                legs.Add(leg);
            }

            legs[0].StartPoint = optimalRouteRequest.Markers.FirstOrDefault(x => x.MarkerType == MarkerType.StartPoint)?.PointId ?? 0;
            foreach (var leg in legs)
            {

            }

            var waypoint_order = jRoute.SelectToken("waypoint_order").Select(x => (int)x).ToList();
            var markers = MarkerService.MarkersWaypointsReOrder(optimalRouteRequest.Markers, waypoint_order).OrderBy(x => x.Index).ToArray();


            for (int i = 0; i < markers.Count() - 1; i++)
            {
                legs[i].StartPoint = markers[i].PointId;
                legs[i].EndPoint = markers[i+1].PointId;
            }

            return new MapOtimizedRouteResponce() {
                Legs = legs,
                Markers = markers,
                Polyline = polyline
            };

        }


    }

}
