using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Infrastracture
{
    public class DbFactory : Disposable, IDbFactory
    {
        LabContext dbContext;

        public DbFactory(LabContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public LabContext Init()
        {
            return dbContext;// ?? (dbContext = new LabContext(connectionString));
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}
