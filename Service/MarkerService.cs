using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Entities;
using Domain.Repositories;

namespace Service {
    public interface IMarkerService {
        IEnumerable<Marker> GetMarkers (int customerID);
        void AddMarker (Marker marker);
        void RemoveMarkers (int customerID);
        bool CanAddMarker(int customerID);
        IEnumerable<Marker> AddMarkerWithIndexUpdate (Marker marker);
    }

    public class MarkerService : IMarkerService {
        //private LabContext db;
        private readonly IMarkerRepository markerRepository;

        public MarkerService (IMarkerRepository markerRepository) {
            this.markerRepository = markerRepository;
        }

        public IEnumerable<Marker> GetMarkers (int customerID) {
            return markerRepository.GetMany (x => x.CustomerID == customerID);
            //return db.Markers.Where(x => x.CustomerID == customerID);
        }

        public void AddMarker (Marker marker) {
            markerRepository.Add (marker);
            markerRepository.Save ();
            //db.Markers.Add(marker);
            //db.SaveChanges();
        }

        public void RemoveMarkers (int customerID) {
            markerRepository.Delete (x => x.CustomerID == customerID);
            markerRepository.Save ();

            //db.Markers.RemoveRange(db.Markers.Where(x => x.CustomerID == customerID));
            //db.SaveChanges();
        }

        public bool CanAddMarker(int customerID) {
           return markerRepository.Count(x => x.CustomerID == customerID) <= 10;
        }

        public IEnumerable<Marker> AddMarkerWithIndexUpdate (Marker marker) {
            var markerWithSameIndex = markerRepository.Get (x => x.CustomerID == marker.CustomerID && x.Index == marker.Index);

            if (markerWithSameIndex != null) {
                // add new with update issue markers
                if (marker.MarkerType != MarkerType.EndPoint) {
                    var markersToUpdate = markerRepository.GetMany (x => x.CustomerID == marker.CustomerID && x.Index >= marker.Index);
                    foreach (var markerToUpdate in markersToUpdate) {
                        if (markerToUpdate.MarkerType == MarkerType.StartPoint)
                            markerToUpdate.MarkerType = MarkerType.WayPoint;

                        markerToUpdate.Index++;
                        markerRepository.Update(markerToUpdate);

                    }
                } else {
                    var markerToUpdate = markerRepository.Get(x => x.CustomerID == marker.CustomerID && x.MarkerType == MarkerType.EndPoint);
                    markerToUpdate.Index = markerRepository.GetLastPointIndex(marker.CustomerID) + 1;
                    markerToUpdate.MarkerType = MarkerType.WayPoint;
                    markerRepository.Update(markerToUpdate);
                }

                markerRepository.Add (marker);
                markerRepository.Save ();

                return markerRepository.GetMany(x => x.CustomerID == marker.CustomerID);

            } else {
                markerRepository.Add (marker);
                markerRepository.Save ();

                return new List<Marker> () { marker };
            }

        }
    }
}