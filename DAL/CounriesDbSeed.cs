using Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DAL
{
    public static class SeedDb
    {

        public static void MarketIconsSeed(LabContext db)
        {
            db.MarkerIcons.AddRange(
                new List<MarkerIcon>() {
                                    new MarkerIcon("Blue",   "https://maps.google.com/mapfiles/kml/paddle/blu-blank.png"),
                                    new MarkerIcon("Red",    "https://maps.google.com/mapfiles/kml/paddle/red-blank.png"),
                                    new MarkerIcon("Green",  "https://maps.google.com/mapfiles/kml/paddle/grn-blank.png"),
                                    new MarkerIcon("Purple", "https://maps.google.com/mapfiles/kml/paddle/purple-blank.png"),
                                    new MarkerIcon("White",  "https://maps.google.com/mapfiles/kml/paddle/wht-blank.png"),
                }
            );
            db.SaveChanges();
        }

        public static void CountriesSeed(LabContext db)
        {
            var countries = new List<Country>();

            string jsonData = System.IO.File.ReadAllText(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), @"countries.json"));

            var jobjects = JArray.Parse(jsonData);

            foreach (var jobj in jobjects)
            {
                var country = new Country();

                country.Name = jobj["name"].ToString();
                country.ISO3166_2_Code = jobj["alpha2"].ToString();
                country.Description = jobj["currencies"]?.FirstOrDefault()?.ToString();
                country.Telephone_Code = jobj["countryCallingCodes"]?.FirstOrDefault()?.ToString();

                countries.Add(country);
            }

            db.Countries.AddRange(countries);
            db.SaveChanges();

            //JsonSerializerSettings settings = new JsonSerializerSettings
            //{
            //    ContractResolver = new PrivateSetterContractResolver()
            //};

            //List<WeatherEvent> events =
            // JsonConvert.DeserializeObject<List<WeatherEvent>>(
            //   jsonData, settings);





        }

    }




}
