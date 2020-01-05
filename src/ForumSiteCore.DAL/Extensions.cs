using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.DAL
{
    public static class Extensions
    {
        public static SimpleAmbientTransaction BeginSimpleAmbientTransaction(this Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade database)
        {
            return new SimpleAmbientTransaction(database);
        }
    }
}
