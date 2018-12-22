using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LabGoogleMap.Models;
using System.Data.SqlClient;
using Domain.Entities;
using Domain.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LabGoogleMap.Controllers
{

    public class HomeController : Controller
    {
        private readonly MapService mapService;

        public HomeController(MapService mapService)
        {
            this.mapService = mapService;
        }


        public IActionResult Index()
        {
            var customerID = 1;
            var model = new MapViewModel()
            {
                MarkerIcons = mapService.GetMarkerIcons(),
                Markers = mapService.GetMarkers(customerID)
            };

            return View(model);
        }

        public class Data
        {
            public string Lat { get; set; }
            public string Lng { get; set; }
            public string MarkerIconId { get; set; }
        }


        [HttpPost]
        //public JsonNetResult TeamMapMarkerAdd([FromBody]Data markerData)
        public JsonNetResult TeamMapMarkerAdd1(string Lat)
        {
            var s1 = Request.Form["Lat"];

            var s2 = RouteData.Values["Lat"];

            var s3 = Request.Query["Lat"];

            return new JsonNetResult(new
            {
                Lat = Lat,
                success = true
            });
        }

        [HttpPost]
        //public JsonNetResult TeamMapMarkerAdd([FromBody]Data markerData)
        public JsonNetResult TeamMapMarkerAdd2(string Lat, string Lng)
        {
            return new JsonNetResult(new
            {
                Lat = Lat,
                success = true
            });
        }


        [HttpPost]
        //public JsonNetResult TeamMapMarkerAdd([FromBody]Data markerData)
        public JsonNetResult TeamMapMarkerAdd(string Lat, string Lng, string MarkerIconId)
        {
            try
            {
                //this.Request.Body.Position = 0;
                var result = new System.IO.StreamReader(this.Request.Body).ReadToEnd();

                //var controller = RouteData.Values["controller"].ToString();
                //var action = RouteData.Values["action"].ToString();
                //return Content($"controller: {controller} | action: {action}");

                var customerID = 1;

                //var newMarker = new Marker()
                //{
                //    Lat = lat,
                //    Lng = lng,
                //    MarkerIconId = markerIconId,
                //    CustomerID = customerID
                //};

                //mapService.AddMarker(newMarker);

                return new JsonNetResult(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                return new JsonNetResult(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public JsonNetResult TeamMapMarkersRemove()
        {
            try
            {
                var customerID = 1;

                mapService.RemoveMarkers(customerID);

                return new JsonNetResult(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                return new JsonNetResult(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }







        ////public string connectionString = @"data source=(LocalDB)\MSSQLLocalDB;Database=study_google_map;integrated security=True;";

        ////public IActionResult Index()
        ////{
        ////    return View();
        ////}

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}



    }



    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
       public  IActionResult Test ()
        {
            return new JsonNetResult(new
            {
                success = false
            });
        }
    }




}
