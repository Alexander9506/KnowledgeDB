using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace KnowledgeDB.Areas.Identity.Data
{
    public class KnowledgeDBUser : IdentityUser
    {
        public string ProfilePicturePath { get; set; }
    }
}
