using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;

namespace GoogleApi
{
    public class Direction
    {
        string googleApiKey , googleDirectionUrl;

        public Direction(string googleApiKey_, string googleDirectionUrl_)
        {
            googleApiKey = googleApiKey_;
            googleDirectionUrl = googleDirectionUrl_;
        }


        public Leg GetLegByGoogleApi(IPoint startPoint, IPoint endPoint)
        {
            Leg leg = new Leg();

            string origin = $"origin={startPoint.Lat},{startPoint.Lng}";
            string destination = $"destination={endPoint.Lat},{endPoint.Lng}";

            string url = googleDirectionUrl + origin + "&" + destination + "&key=" + googleApiKey;

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

            leg.Distance = (int)jLeg.SelectToken("distance").SelectToken("value");
            leg.Duration = (int)jLeg.SelectToken("duration").SelectToken("value");

            return leg;
        }


        public IGoogleOptimalRouteResponce GetGoogleOptimalRoute(IPoint startPoint, IPoint endPoint, IEnumerable<IPoint> wayPoints, string options = "")
        {
            string origin = "", destination = "", wayPointsStr = "";

            origin = $"origin={startPoint.Lat},{startPoint.Lng}";
            destination = $"&destination={endPoint.Lat},{endPoint.Lng}";

            foreach (var point in wayPoints)
            {
                if (string.IsNullOrEmpty(wayPointsStr))
                {
                    wayPointsStr += $"&waypoints=optimize:true|{point.Lat},{point.Lng}";
                }
                else
                {
                    wayPointsStr += $"|{point.Lat},{point.Lng}";
                }
            }

            string url = googleDirectionUrl + 
                 origin +
                 destination +
                 wayPointsStr + options +
                "&key=" + googleApiKey;

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

            var legs = new List<Leg>();

            foreach (var jLeg in jLegs)
            {
                Leg leg = new Leg();

                leg.Distance = (int)jLeg.SelectToken("distance").SelectToken("value");
                leg.Duration = (int)jLeg.SelectToken("duration").SelectToken("value");

                legs.Add(leg);
            }

            var waypoint_order = jRoute.SelectToken("waypoint_order").Select(x => (int)x).ToList();

            return new GoogleOptimalRouteResponce() {
                Polyline = polyline,
                WaypointOrder = waypoint_order,
                Legs = legs
            };
        }

    }



}
