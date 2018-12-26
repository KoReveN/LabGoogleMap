using Domain.Entities;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public interface IMarkerService
    {
        IEnumerable<Marker> GetMarkers(int customerID);
        void AddMarker(Marker marker);
        void RemoveMarkers(int customerID);
    }


    public class MarkerService : IMarkerService
    {
        //private LabContext db;
        private readonly IMarkerRepository markerRepository;

        public MarkerService(IMarkerRepository markerRepository)
        {
            this.markerRepository = markerRepository;
        }


        public IEnumerable<Marker> GetMarkers (int customerID)
        {
            return markerRepository.GetMany(x => x.CustomerID == customerID);
            //return db.Markers.Where(x => x.CustomerID == customerID);
        }



        public void AddMarker(Marker marker)
        {
            markerRepository.Add(marker);
            markerRepository.Save();
            //db.Markers.Add(marker);
            //db.SaveChanges();
        }

        public void RemoveMarkers(int customerID)
        {
            markerRepository.Delete(x => x.CustomerID == customerID);
            markerRepository.Save();

            //db.Markers.RemoveRange(db.Markers.Where(x => x.CustomerID == customerID));
            //db.SaveChanges();
        }

    }
}



