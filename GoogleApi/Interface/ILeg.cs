using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleApi
{
    public interface ILeg
    {
        int RouteLegId { get; set; }
        int StartPoint { get; set; }
        int EndPoint { get; set; }
        int Distance { get; set; }
        int Duration { get; set; }
        string Polyline { get; set; }
    }
}
