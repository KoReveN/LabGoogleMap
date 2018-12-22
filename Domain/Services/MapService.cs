using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Services
{
    public class MapService
    {
        private LabContext db;

        public MapService(LabContext labContext)
        {
            this.db = labContext;
        }

        //public MapService()
        //{
        //    this.db = new LabContext();
        //}


        public IEnumerable<Marker> GetMarkers (int customerID)
        {
            return db.Markers.Where(x => x.CustomerID == customerID);
        }

        public IEnumerable<MarkerIcon> GetMarkerIcons()
        {
            return db.MarkerIcons;
        }

        public void AddMarker(Marker marker)
        {
            db.Markers.Add(marker);
            db.SaveChanges();
        }

        public void RemoveMarkers(int customerID)
        {
            db.Markers.RemoveRange(db.Markers.Where(x => x.CustomerID == customerID));
            db.SaveChanges();
        }

    }
}



