using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class MarkerIcon
    {
        public int MarkerIconId { get; set; }
        public string Color { get; set; }
        public string IconUrl { get; set; }


        public MarkerIcon(string color, string iconUrl)
        {
            Color = color;
            IconUrl = iconUrl;
        }

    }
}
