using System;
using Microsoft.AspNetCore.Identity;

namespace Stienen.Backend.DataAccess.Models {
    public class UserEntity : IdentityUser<Guid>{
        public string Name { get; set; }
    }
}