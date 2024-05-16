using Data.Entites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<UserAddress> UserAddresses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
