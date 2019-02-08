using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Service {
    public interface IMarkerService {
        IEnumerable<Marker> GetMarkers (int customerID);
        void AddMarker (Marker marker);
        void RemoveMarker(int markerId);
        void RemoveMarkers (int customerID);
        bool CanAddMarker(int customerID);
        IEnumerable<Marker> AddMarkerWithIndexUpdate (Marker marker);
        IEnumerable<Marker> UpdateMarkers(IEnumerable<Marker> markers);
        IEnumerable<Marker> MarkersBeforeRouteWork(IEnumerable<Marker> markers);
    }

    public class MarkerService : IMarkerService
    {
        private readonly LabContext db;
        private readonly DbSet<Marker> dbSet;

        public MarkerService (LabContext db_) {
            this.db = db_;
            this.dbSet = db_.Set<Marker>();
        }

        public IEnumerable<Marker> GetMarkers (int customerID) {
            return dbSet.Include(x => x.Point).Where(x => x.CustomerID == customerID);
        }

        public void AddMarker (Marker marker) {
            dbSet.Add(marker);
            db.SaveChanges();
        }

        public void RemoveMarker(int markerId)
        {
            IEnumerable<Marker> objects = dbSet.Where(x => x.MarkerId == markerId).AsEnumerable();
            foreach (Marker obj in objects)
                dbSet.Remove(obj);
 
            db.SaveChanges();
        }

        public void RemoveMarkers (int customerID) {
            IEnumerable<Marker> objects = dbSet.Where(x => x.CustomerID == customerID).AsEnumerable();
            foreach (Marker obj in objects)
                dbSet.Remove(obj);

            db.SaveChanges();
        }

        public bool CanAddMarker(int customerID) {
           return dbSet.Count(x => x.CustomerID == customerID) <= 10;
        }

        public IEnumerable<Marker> UpdateMarkers(IEnumerable<Marker> markers)
        {
            foreach (var marker in markers)
            {
                dbSet.Update(marker);
            }
            db.SaveChanges();
            return markers;
        }

        public IEnumerable<Marker> AddMarkerWithIndexUpdate (Marker marker) {
            var markerWithSameIndex = dbSet.FirstOrDefault(x => x.CustomerID == marker.CustomerID && x.Index == marker.Index);

            if (markerWithSameIndex != null) {
                // add new with update issue markers
                if (marker.MarkerType != MarkerType.EndPoint) {
                    var markersToUpdate = dbSet.Where(x => x.CustomerID == marker.CustomerID && x.Index >= marker.Index);
                    foreach (var markerToUpdate in markersToUpdate) {
                        if (markerToUpdate.MarkerType == MarkerType.StartPoint)
                        {
                            markerToUpdate.MarkerType = MarkerType.WayPoint;
                            markerToUpdate.MarkerIconId = 1;
                        }

                        markerToUpdate.Index++;
                        dbSet.Update(markerToUpdate);

                    }
                } else {
                    var markersToUpdate = dbSet.Where(x => x.CustomerID == marker.CustomerID && x.MarkerType == MarkerType.EndPoint);
                    foreach (var markerToUpdate in markersToUpdate)
                    {
                        markerToUpdate.Index = GetLastPointIndex(marker.CustomerID) + 1;
                        markerToUpdate.MarkerType = MarkerType.WayPoint;
                        markerToUpdate.MarkerIconId = 1;
                        dbSet.Update(markerToUpdate);
                    }
                }

                dbSet.Add (marker);
                db.SaveChanges();

                return dbSet.Include(x => x.Point).Where(x => x.CustomerID == marker.CustomerID);

            } else {
                dbSet.Add (marker);
                db.SaveChanges();

                return new List<Marker> () { marker };
            }

        }


        public int GetLastPointIndex(int customerId)
        {
            try
            {
                return dbSet.Where(m => m.CustomerID == customerId && m.MarkerType == MarkerType.WayPoint)
                    .Max(m => m.Index);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }


        public IEnumerable<Marker> MarkersBeforeRouteWork(IEnumerable<Marker> markers)
        {
            if (markers.Count() > 1)
            {
                if (!markers.Any(x => x.MarkerType == MarkerType.StartPoint))
                    SetStartPointMarker(markers);
                if (!markers.Any(x => x.MarkerType == MarkerType.EndPoint))
                    SetEndPointMarker(markers);
            }
            if (markers.Count() > 2)
                SetWayPointCorrectIndexes(markers);

                return markers.OrderBy(x => x.Index);
        }

        private IEnumerable<Marker> SetStartPointMarker(IEnumerable<Marker> markers)
        {
            Marker startPoint = markers.Where(m => m.Index == markers.Min(x => x.Index)).FirstOrDefault();
            startPoint.Index = 0;
            startPoint.MarkerType = MarkerType.StartPoint;
            startPoint.MarkerIconId = 5;
            return markers;
        }

        private IEnumerable<Marker> SetEndPointMarker(IEnumerable<Marker> markers)
        {
            Marker startPoint = markers.Where(m => m.Index == markers.Max(x => x.Index)).FirstOrDefault();
            startPoint.Index = 10;
            startPoint.MarkerType = MarkerType.EndPoint;
            startPoint.MarkerIconId = 5;
            return markers;
        }

        private IEnumerable<Marker> SetWayPointCorrectIndexes(IEnumerable<Marker> markers)
        {
            int currIndex = 1;

            foreach (var item in markers.Where(x => x.MarkerType == MarkerType.WayPoint).OrderBy(x => x.Index))
            {
                item.Index = currIndex;
                currIndex++;
            }

            foreach (var item in markers.Where(x => x.MarkerType == MarkerType.WayPoint && x.MarkerIconId == 5))
            {
                item.MarkerIconId = 1;
            }

            return markers;
        }


        public static IEnumerable<Marker> MarkersWaypointsReOrder(IEnumerable<Marker> markers, List<int> newWaypointOrder)
        {

            var result = new List<Marker>();
            result.Add(markers.FirstOrDefault(x => x.MarkerType == MarkerType.StartPoint));

            var wayPointsMarkers = markers.Where(x => x.MarkerType == MarkerType.WayPoint).ToList();

            if (wayPointsMarkers.Count() == newWaypointOrder.Count())
            {
                for (int i = 0; i < wayPointsMarkers.Count(); i++)
                {
                    wayPointsMarkers[i].Index = 1 + newWaypointOrder[i];
                }
                result.AddRange(wayPointsMarkers.OrderBy(x => x.Index));
            }
            else
                throw new Exception("Error reordering way-points.");

            result.Add(markers.FirstOrDefault(x => x.MarkerType == MarkerType.EndPoint));
            return result;

        }

    }
}