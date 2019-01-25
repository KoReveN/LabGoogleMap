using GoogleApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.RequestModels
{
    public class MapSimpleRouteResponce
    {
        public IEnumerable<ILeg> Legs { get; set; }
        public int LegsFromDbCount { get; set; }

        public MapSimpleRouteResponce()
        {
            Legs = new List<ILeg>();
        }
    }
}
