using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Entities
{
    public class Marker
    {
        public int MarkerId { get; set; }
        public int CustomerID { get; set; }
        public int PointId { get; set; }
        public int MarkerIconId { get; set; }
        public MarkerType MarkerType { get; set; }
        public int Index { get; set; }


        public Point Point{ get; set; }
        public MarkerIcon MarkerIcon { get; set; }



    }


    public enum MarkerType
    {
        [Description("Way Point")]
        WayPoint ,
        [Description("Start Point")]
        StartPoint ,
        [Description("End Point")]
        EndPoint 
    }
}
