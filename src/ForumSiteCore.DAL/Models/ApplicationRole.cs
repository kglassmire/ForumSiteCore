using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSiteCore.DAL.Models
{
    public class ApplicationRole : IdentityRole<long>
    {
        public ApplicationRole()
        { }

        public ApplicationRole(string roleName)
        {
            Name = roleName;
        }
    }
}
