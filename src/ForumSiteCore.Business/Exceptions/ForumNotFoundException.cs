using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Exceptions
{
    public class ForumNotFoundException : Exception
    {
        public ForumNotFoundException(string message) : base(message)
        {
        }

        public ForumNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
