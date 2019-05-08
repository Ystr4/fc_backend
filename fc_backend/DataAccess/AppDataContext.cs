using System;
using System.Collections.Generic;
using Stienen.Backend.DataAccess.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Stienen.Backend.DataAccess {
    public class AppDataContext : DbContext { //IdentityDbContext<UserEntity, UserRoleEntity, Guid> {
        public AppDataContext(DbContextOptions options) 
                : base(options) { }






    }
}