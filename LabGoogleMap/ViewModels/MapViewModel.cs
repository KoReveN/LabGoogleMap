using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabGoogleMap.Models
{
    public class MapViewModel
    {
        public IEnumerable<MarkerIcon> MarkerIcons { get; set; }
        public IEnumerable<Marker> Markers { get; set; }

        public MapViewModel()
        {
            MarkerIcons = new List<MarkerIcon>();
            Markers = new List<Marker>();
        }
    }


}
