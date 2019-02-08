using System;
using System.Collections.Generic;
using System.Text;
using GoogleApi;

namespace Entities
{
    public class Point : IPoint
    {
        public int PointId { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public string Address { get; set; }
    }
}
