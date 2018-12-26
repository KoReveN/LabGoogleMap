using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Infrastracture
{
    public interface IDbFactory : IDisposable
    {
        LabContext Init();
    }
}
