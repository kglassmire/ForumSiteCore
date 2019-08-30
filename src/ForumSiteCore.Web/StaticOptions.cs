using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSiteCore.Web
{
    public static class StaticOptions
    {
        public static bool IsDebug
        {
            get
            {
                bool isDebug = false;
#if DEBUG == true
                isDebug = true;
#endif
                return isDebug;            
            }
        }
    }
}
