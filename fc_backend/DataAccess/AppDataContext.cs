using System;
using System.Collections.Generic;
using Stienen.Backend.DataAccess.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Stienen.Backend.DataAccess.Models.Frontend;

namespace Stienen.Backend.DataAccess {
    public class AppDataContext : IdentityDbContext<UserEntity, UserRoleEntity, Guid> {
        public AppDataContext(DbContextOptions options) 
                : base(options) { }

        DbSet<RoundEntity> Rounds { get; set; }

//        DbSet<GeneralSettingsEntity> GeneralSettings { get; set; }
//        DbSet<FilterSettingsEntity> FilterSettings { get; set; }
    }
}