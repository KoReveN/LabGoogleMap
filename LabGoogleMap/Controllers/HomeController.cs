using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Repositories;
using LabGoogleMap.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Service;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace LabGoogleMap.Controllers {

    public class HomeController : Controller {
        //private readonly MapService mapService;
        private readonly IMarkerService markerService;
        private readonly IMarkerIconService markerIconService;

        const int CustomerID = 1;

        public HomeController (IMarkerService markerService, IMarkerIconService markerIconService) {
            this.markerService = markerService;
            this.markerIconService = markerIconService;
        }

        public IActionResult Index () {
            var customerID = CustomerID;
            var model = new MapViewModel () {
                MarkerIcons = markerIconService.GetMarkerIcons (),
                Markers = markerService.GetMarkers (customerID)
            };

            return View (model);
        }

        [HttpPost]
        public IActionResult TeamMapMarkerAdd ([FromBody] Marker marker) {
            try {

                if (!markerService.CanAddMarker(CustomerID)) {
                    return Json (new {
                        success = false,
                        message = "You can't add more then 10 markers"
                    });
                }

                marker.Address = GetGooglGeocodeAddress(marker);
                marker.CustomerID = CustomerID;

                var markers = markerService.AddMarkerWithIndexUpdate (marker);
                //markerService.AddMarker (marker);

                return Json (new {
                    success = true,
                        markers
                });

            } catch (Exception ex) {
                return new JsonNetResult (new {
                    success = false,
                        message = ex.Message
                });
            }
        }


        [HttpPost]
        public IActionResult GetGoogleRoute([FromBody] Marker[] markers)
        {
            string origin = "", destination = "", wayPoints = "";
            //List<string> wayPoints = new List<string>();

            foreach (Marker marker in markers)
            {
                switch (marker.MarkerType)
                {
                    case MarkerType.WayPoint:
                        if (string.IsNullOrEmpty(wayPoints))
                        {
                            wayPoints += $"waypoints=via:{marker.Lat},{marker.Lng}";
                        } else
                        {
                            wayPoints += $"|via:{marker.Lat},{marker.Lng}";
                        }
                        break;
                    case MarkerType.StartPoint:
                        origin = $"origin={marker.Lat},{marker.Lng}";
                        break;
                    case MarkerType.EndPoint:
                        destination = $"destination={marker.Lat},{marker.Lng}";
                        break;
                }
            }

            string key = @"AIzaSyA3YhAyyckDAMFGuVR7yRI-fG_NATvL8Yk";
            string url = @"https://maps.googleapis.com/maps/api/directions/json?" + origin + 
                "&" + destination +
                "&" + wayPoints +
                "&key=" + key;

            WebRequest request = WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            request.Method = WebRequestMethods.Http.Get;

            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();
            StreamReader reader = new StreamReader(data);
            // json-formatted string from maps api
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            data.Close();
            response.Close();

            JObject jObj = JObject.Parse(responseFromServer);

            JToken jRoute = jObj["routes"].FirstOrDefault();

            string polyline = jRoute.SelectToken("overview_polyline").SelectToken("points").ToString();

            //string address = jObj["results"].FirstOrDefault().SelectToken("formatted_address").ToString();

            return Json(url);
        }


        public string GetGooglGeocodeAddress(Marker marker)
        {
            string key = @"AIzaSyA3YhAyyckDAMFGuVR7yRI-fG_NATvL8Yk";
            string lat = marker.Lat.ToString();
            string lng = marker.Lng.ToString();
            string url = @"https://maps.googleapis.com/maps/api/geocode/json?latlng=" + lat + "," + lng + "&key=" + key;

            WebRequest request = WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            request.Method = WebRequestMethods.Http.Get;

            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();
            StreamReader reader = new StreamReader(data);
            // json-formatted string from maps api
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            data.Close();
            response.Close();

            JObject jObj = JObject.Parse(responseFromServer);

            string address = jObj["results"].FirstOrDefault().SelectToken("formatted_address").ToString();

            return address;
        }

        [HttpPost]
        public IActionResult TeamMapMarkerRemove([FromBody] int markerId)
        {
            try
            {
                markerService.RemoveMarker(markerId);

                return Json(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }


        }

        [HttpPost]
        public JsonNetResult TeamMapMarkersRemove () {
            try {
                var customerID = CustomerID;

                markerService.RemoveMarkers (customerID);

                return new JsonNetResult (new {
                    success = true
                });
            } catch (Exception ex) {
                return new JsonNetResult (new {
                    success = false,
                        message = ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult TeamMapMarkersUpdate ([FromBody] Marker[] markers) {

            // foreach (var item in markers)
            // {
            //     item.Index = 5;
            // }

            return Json (new {
                success = true,
                    markers
            });
        }



    }
}