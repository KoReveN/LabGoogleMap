using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Point
    {
        public int PointId { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public string Address { get; set; }
    }
}
