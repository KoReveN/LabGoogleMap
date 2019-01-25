using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;

namespace GoogleApi
{
    public class Geocode
    {

        string googleApiKey, googleGeocodeUrl;

        public Geocode(string googleApiKey_, string googleGeocodeUrl_)
        {
            googleApiKey = googleApiKey_;
            googleGeocodeUrl = googleGeocodeUrl_;
        }


        public string GetAddress(IPoint point)
        {
            string lat = point.Lat.ToString();
            string lng = point.Lng.ToString();
            string url = googleGeocodeUrl + lat + "," + lng + "&key=" + googleApiKey;

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

            string address = jObj["results"].FirstOrDefault().SelectToken("formatted_address").ToString();

            return address;
        }
    }

}
