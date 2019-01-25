using System;
using System.Linq;
using Domain.Entities;
using LabGoogleMap.Models;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.RequestModels;
using Microsoft.Extensions.Configuration;

namespace LabGoogleMap.Controllers
{

    public class HomeController : Controller {
        private readonly IMarkerService markerService;
        private readonly IMarkerIconService markerIconService;
        private readonly IRouteLegService routeLegService;
        private readonly IPointService pointService;

        const int CustomerID = 1;

        public HomeController (IMarkerService markerService, IMarkerIconService markerIconService, IRouteLegService routeLegService,
            IPointService pointService) {
            this.markerService = markerService;
            this.markerIconService = markerIconService;
            this.routeLegService = routeLegService;
            this.pointService = pointService;
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
        public IActionResult MapMarkerAdd ([FromBody] Marker marker) {
            try {

                if (!markerService.CanAddMarker(CustomerID)) {
                    return Json (new {
                        success = false,
                        message = "You can't add more then 10 markers"
                    });
                }

                marker.Point.Address = pointService.GetPointAddress(marker.Point);
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
        public IActionResult GetGoogleSimpleRoute([FromBody] Marker[] markers_)
        {
            if (markers_.Count() > 1)
            {
                var markers = markerService.MarkersBeforeRouteWork(markers_).ToArray();
                markerService.UpdateMarkers(markers);

                var responce = routeLegService.GetRouteLegs(markers);

                return Json(new { success = true, legs = responce.Legs, markers, responce.LegsFromDbCount });
            }
            return Json(new
            {
                success = false,
                msg = "To build a route must be at least 2 points"
            });
        }

        [HttpPost]
        public IActionResult GetGoogleOptimalRoute([FromBody] MapOtimizedRouteRequest optimalRouteRequest)
        {
            if (optimalRouteRequest.Markers.Count() > 1)
            {
                if (optimalRouteRequest.Markers.Count() < 11)
                {
                     var markers = markerService.MarkersBeforeRouteWork(optimalRouteRequest.Markers);
                    optimalRouteRequest.Markers = markers;
                    var response = routeLegService.GetGoogleOptimalRoute(optimalRouteRequest);
                    markerService.UpdateMarkers(response.Markers);
                    return Json(new { success = true, otimizedRoute = response });

                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        msg = "Maximum number of route points - 10. You can not build a route."
                    });
                }
            }
            else
            {
                return Json(new
                {
                    success = false,
                    msg = "To build a route must be at least 2 points"
                });
            }
        }




        [HttpPost]
        public IActionResult MapMarkerRemove([FromBody] int markerId)
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
        public JsonNetResult MapMarkersRemove() {
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
        public IActionResult MapMarkersUpdate ([FromBody] Marker[] markers) {

            var response = markerService.UpdateMarkers(markers);

            return Json (new {
                success = true,
                markers = response
            });
        }



    }
}












//public string GetGooglGeocodeAddress(Marker marker)
//{
//    string key = AppConfiguration["googleApi:Key"];
//    string url = AppConfiguration["googleApi:DirectionsUrl"];
//    string geocodeUrl = AppConfiguration["googleApi:GeocodeUrl"];
//    var googleApi = new GoogleApi.Direction(key, url, geocodeUrl);

//    return googleApi.GetAddress(marker.Point);

//    //string key = AppConfiguration["googleApi:Key"];// @"AIzaSyA3YhAyyckDAMFGuVR7yRI-fG_NATvL8Yk";
//    //string lat = marker.Point.Lat.ToString();
//    //string lng = marker.Point.Lng.ToString();
//    //string url = AppConfiguration["googleApi:GeocodeUrl"]  //@"https://maps.googleapis.com/maps/api/geocode/json?latlng=" 
//    //+ lat + "," + lng + "&key=" + key;

//    //WebRequest request = WebRequest.Create(url);
//    //request.ContentType = "application/json; charset=utf-8";
//    //request.Method = WebRequestMethods.Http.Get;

//    //WebResponse response = request.GetResponse();
//    //Stream data = response.GetResponseStream();
//    //StreamReader reader = new StreamReader(data);
//    //// json-formatted string from maps api
//    //string responseFromServer = reader.ReadToEnd();
//    //reader.Close();
//    //data.Close();
//    //response.Close();

//    //JObject jObj = JObject.Parse(responseFromServer);

//    //string address = jObj["results"].FirstOrDefault().SelectToken("formatted_address").ToString();

//    //return address;
//}





//[HttpPost]
//public IActionResult GetGoogleRoute([FromBody] Marker[] markers)
//{
//    string origin = "", destination = "", wayPoints = "";
//    //List<string> wayPoints = new List<string>();

//    foreach (Marker marker in markers)
//    {
//        switch (marker.MarkerType)
//        {
//            case MarkerType.WayPoint:
//                if (string.IsNullOrEmpty(wayPoints))
//                {
//                    wayPoints += $"waypoints=via:{marker.Lat},{marker.Lng}";
//                }
//                else
//                {
//                    wayPoints += $"|via:{marker.Lat},{marker.Lng}";
//                }
//                break;
//            case MarkerType.StartPoint:
//                origin = $"origin={marker.Lat},{marker.Lng}";
//                break;
//            case MarkerType.EndPoint:
//                destination = $"destination={marker.Lat},{marker.Lng}";
//                break;
//        }
//    }

//    string key = @"AIzaSyA3YhAyyckDAMFGuVR7yRI-fG_NATvL8Yk";
//    string url = @"https://maps.googleapis.com/maps/api/directions/json?" + origin +
//        "&" + destination +
//        "&" + wayPoints +
//        "&key=" + key;

//    WebRequest request = WebRequest.Create(url);
//    request.ContentType = "application/json; charset=utf-8";
//    request.Method = WebRequestMethods.Http.Get;

//    WebResponse response = request.GetResponse();
//    Stream data = response.GetResponseStream();
//    StreamReader reader = new StreamReader(data);
//    // json-formatted string from maps api
//    string responseFromServer = reader.ReadToEnd();
//    reader.Close();
//    data.Close();
//    response.Close();

//    JObject jObj = JObject.Parse(responseFromServer);

//    JToken jRoute = jObj["routes"].FirstOrDefault();

//    string polyline = jRoute.SelectToken("overview_polyline").SelectToken("points").ToString();

//    //string address = jObj["results"].FirstOrDefault().SelectToken("formatted_address").ToString();

//    return Json(url);
//}