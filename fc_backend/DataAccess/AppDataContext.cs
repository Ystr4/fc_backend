using System;
using System.Collections.Generic;
using fc_backend.DataAccess.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace fc_backend.DataAccess {
    public class AppDataContext : IdentityDbContext<UserEntity, UserRoleEntity, Guid> {
        public AppDataContext(DbContextOptions options) 
                : base(options) { }

//        public DbSet<HardwareReferenceEntity> HardwareReferences { get; set; }

        public DbSet<ReferenceEx> References { get; set; }

        public DbSet<DeviceDataByDay> DeviceData { get; set; }
        public DbSet<DeviceInfoEntity> DeviceInfo { get; set; }
    }
}