using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSiteCore.DAL.Models
{
    public class Forum
    {
        public Int64 Id { get; set; }

        [Column(TypeName = "citext")]
        public String Name { get; set; }
        public String Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public Boolean Inactive { get; set; }
        public Int64 UserId { get; set; }
        public Int64 Saves { get; set; }

        public List<ForumSave> ForumSaves { get; set; }
        public List<Post> Posts { get; set; }
        public ApplicationUser User { get; set; }
    }
}
