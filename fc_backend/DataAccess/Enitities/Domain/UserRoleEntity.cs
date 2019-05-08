using System;
using Microsoft.AspNetCore.Identity;

namespace Stienen.Backend.DataAccess.Models {
    public class UserRoleEntity : IdentityRole<Guid>{
        public UserRoleEntity()
                : base()
        {
        }

        public UserRoleEntity(string roleName)
                : base(roleName)
        {
        }
    }
}