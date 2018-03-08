using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.DAL
{
    public class SimpleAmbientTransaction : IDisposable
    {
        public IDbContextTransaction Transaction { get; set; }
        public Boolean TopLevel { get; set; }
        public SimpleAmbientTransaction(Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade database)
        {
            TopLevel = database.CurrentTransaction == null;
            Transaction = TopLevel ? database.BeginTransaction() : database.CurrentTransaction;
        }

        public void Commit()
        {
            if (TopLevel)
            {
                Transaction.Commit();
            }            
        }

        public void Rollback()
        {
            if (TopLevel)
            {
                Transaction.Rollback();
            }
        }

        public void Dispose()
        {
            if (TopLevel)
            {
                Transaction.Dispose();
            }
        }
    }
}
