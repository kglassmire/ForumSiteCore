using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSiteCore.DAL.Models
{
    public class ForumSave
    {
        public Int64 Id { get; set; }
        public Int64 UserId { get; set; }
        public Int64 ForumId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public Boolean Saved { get; set; }

        public Forum Forum { get; set; }
        public ApplicationUser User { get; set; }
    }
}
