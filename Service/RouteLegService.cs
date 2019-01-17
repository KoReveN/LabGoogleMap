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

namespace Service
{

    public interface IRouteLegService
    {
        IEnumerable<RouteLeg> GetRouteLegs(Marker[] markers);
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



        public RouteLeg GetLegByGoogleApi(Marker startPoint, Marker endPoint)
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

    }

}
