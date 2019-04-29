using System;
using Microsoft.AspNetCore.Identity;

namespace fc_backend.DataAccess.Models {
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