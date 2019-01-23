using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleApi
{
    public interface IPoint
    {
        float Lat { get; set; }
        float Lng { get; set; }
        string Address { get; set; }
    }
}
