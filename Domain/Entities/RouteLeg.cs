using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class RouteLeg
    {
        public int RouteLegId { get; set; }
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
        public int Distance { get; set; }
        public int Duration { get; set; }
        public string Polyline { get; set; } 
    }
}
