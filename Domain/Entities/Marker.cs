using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Marker
    {
        public int MarkerId { get; set; }
        public int CustomerID { get; set; }
        public int Index { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public int MarkerIconId { get; set; }
        public MarkerIcon MarkerIcon { get; set; }

        public MarkerType MarkerType { get; set; }


    }

    public enum MarkerType
    {
        WayPoint ,
        StartPoint ,
        EndPoint 
    }
}
