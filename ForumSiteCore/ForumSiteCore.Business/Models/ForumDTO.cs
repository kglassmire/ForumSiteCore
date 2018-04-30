using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Models
{
    public class ForumDto
    {
        public Int64 Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public Boolean Inactive { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public Int64 Saves { get; set; }
    }
}
