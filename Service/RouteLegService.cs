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

        public RouteLegService(IRouteLegRepository routeLegRepository)
        {
            this.routeLegRepository = routeLegRepository;
        }


        public IEnumerable<RouteLeg> GetRouteLegs(Marker[] markers)
        {
            List<RouteLeg> legsRequest = new List<RouteLeg>();
            List<RouteLeg> legsRepos = new List<RouteLeg>();
            List<RouteLeg> legsResponce = new List<RouteLeg>();


            for (int i = 0; i < markers.Count() -1; i++)
            {
                RouteLeg leg = new RouteLeg() {
                    StartPoint = markers[i].MarkerId,
                    EndPoint = markers[i+1].MarkerId
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
                    RouteLeg newLeg = GetLegByGoogleApi(markers.FirstOrDefault(m => m.MarkerId == leg.StartPoint),
                                                        markers.FirstOrDefault(m => m.MarkerId == leg.EndPoint));
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

            string origin = $"origin={startPoint.Lat},{startPoint.Lng}";
            string destination = $"destination={endPoint.Lat},{endPoint.Lng}";

            string key = @"AIzaSyA3YhAyyckDAMFGuVR7yRI-fG_NATvL8Yk";

            string url = @"https://maps.googleapis.com/maps/api/directions/json?" + 
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



            leg.StartPoint = startPoint.MarkerId;
            leg.EndPoint = endPoint.MarkerId;
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
                            wayPoints += $"&waypoints=optimize:true|{marker.Lat},{marker.Lng}";
                        }
                        else
                        {
                            wayPoints += $"|{marker.Lat},{marker.Lng}";
                        }
                        break;
                    case MarkerType.StartPoint:
                        origin = $"origin={marker.Lat},{marker.Lng}";
                        break;
                    case MarkerType.EndPoint:
                        destination = $"&destination={marker.Lat},{marker.Lng}";
                        break;
                }
            }

            string key = @"AIzaSyA3YhAyyckDAMFGuVR7yRI-fG_NATvL8Yk";
            string url = @"https://maps.googleapis.com/maps/api/directions/json?" + origin +
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
            JToken jLeg = jRoute.SelectToken("legs").FirstOrDefault();

            RouteLeg leg = new RouteLeg();
            leg.Polyline = jRoute.SelectToken("overview_polyline").SelectToken("points").ToString();

            //leg.StartPoint = startPoint.MarkerId;
            //leg.EndPoint = endPoint.MarkerId;
            leg.Distance = (int)jLeg.SelectToken("distance").SelectToken("value");
            leg.Duration = (int)jLeg.SelectToken("duration").SelectToken("value");

            //var waypoint_order = new List<int>();
            //foreach (var item in jRoute.SelectTokens("waypoint_order"))
            //{
            //    waypoint_order.Add((int)item);
            //}
            var waypoint_order = jRoute.SelectToken("waypoint_order").Select(x => (int)x).ToList();
            var markers = MarkerService.MarkersWaypointsReOrder(optimalRouteRequest.Markers, waypoint_order);

           return new MapOtimizedRouteResponce() {
                Leg = leg,
                Markers = markers
            };

        }


    }

}
