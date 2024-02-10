using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace n01629177_passion_project.Models {

  // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
  public class ApplicationUser : IdentityUser {
    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager) {

      // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
      var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);


      // Add custom user claims here
      return userIdentity;
    }


    //A User can have many PriceRecords; And a Price Record can have many users.
    //public virtual ICollection<BasePriceRecord> PriceRecords { get; set; }


    //A User can have attestations for many prices and a price can have attestations by many users.
    public virtual ICollection<Price> PriceAttestations { get; set; }
  }

  public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
    public ApplicationDbContext()
        : base("DefaultConnection", throwIfV1Schema: false) {
    }


    public DbSet<Shop> Shops { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<PromoType> PromoTypes { get; set; }
    public DbSet<PromoRecord> PromoRecordRecords { get; set; }
    //public DbSet<BasePriceRecord> BasePriceRecords { get; set; }
    public DbSet<Price> Prices { get; set; }


    protected override void OnModelCreating(DbModelBuilder modelBuilder) {
      base.OnModelCreating(modelBuilder);


      //modelBuilder.Entity<Item>().HasMany(e => e.ItemPriceRecords).WithRequired(e => e.Item).HasForeignKey(e => e.ItemId);
      //modelBuilder.Entity<Shop>().HasMany(e => e.PriceRecords).WithRequired(e => e.Shop).HasForeignKey(e => e.ShopId);
      //modelBuilder.Entity<ApplicationUser>().HasMany(e => e.PriceRecords).WithMany(e => e.Users);


      modelBuilder.Entity<Item>().HasMany(e => e.Prices).WithRequired(e => e.Item).HasForeignKey(e => e.ItemId);
      modelBuilder.Entity<Shop>().HasMany(e => e.Prices).WithRequired(e => e.Shop).HasForeignKey(e => e.ShopId);
      modelBuilder.Entity<ApplicationUser>().HasMany(e => e.PriceAttestations).WithMany(e => e.Users);
    }


    public static ApplicationDbContext Create() {
      return new ApplicationDbContext();
    }
  }
}