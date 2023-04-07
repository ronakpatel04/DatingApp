using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class UserLike
    {
        public AppUser SourceUser { get; set; }


        public int SourceId { get; set; }

        public AppUser TargetUser { get; set; }

        public int  TargetUserId { get; set; }
    
    }
}