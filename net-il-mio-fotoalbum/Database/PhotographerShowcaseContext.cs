﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Database
{
    public class PhotographerShowcaseContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Photo> Photos { get; set; }

        public DbSet<Category> Categories { get; set; }
    }
}