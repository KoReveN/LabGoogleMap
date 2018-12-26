using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Infrastracture
{
    public class DbFactory : Disposable, IDbFactory
    {
        LabContext dbContext;

        public LabContext Init()
        {
            return dbContext ?? (dbContext = new LabContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}
