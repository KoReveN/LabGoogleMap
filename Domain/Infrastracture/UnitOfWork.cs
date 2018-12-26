//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Domain.Infrastracture
//{
//    public class UnitOfWork : IUnitOfWork
//    {
//        private readonly IDbFactory dbFactory;
//        private LabContext dbContext;

//        public UnitOfWork(IDbFactory dbFactory)
//        {
//            this.dbFactory = dbFactory;
//        }

//        public LabContext DbContext
//        {
//            get { return dbContext ?? (dbContext = dbFactory.Init()); }
//        }

//        public void Commit()
//        {
//            DbContext.Commit();
//        }
//    }
//}
