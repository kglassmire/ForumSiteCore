using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSiteCore.DAL.Models
{
    public class Forum
    {
        public long Id { get; set; }

        [Column(TypeName = "citext")]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public bool Inactive { get; set; }
        public long UserId { get; set; }
        public long Saves { get; set; }

        public List<ForumSave> ForumSaves { get; set; }
        public List<Post> Posts { get; set; }
        public ApplicationUser User { get; set; }
    }
}
