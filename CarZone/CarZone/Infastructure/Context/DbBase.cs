using Kawerk.Domain;
using Microsoft.EntityFrameworkCore;

namespace Kawerk.Infastructure.Context
{
    public class DbBase : DbContext
    {
        public DbBase(DbContextOptions options) : base(options) { }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Branches> Branches { get; set; }
        public DbSet<Salesman> Salesman { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<RefreshTokens> RefreshTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Customer>();


            //For the vehicle class, the vehicle could be sold by a Manufacturer or a normal everyday guy. To accomodate that,
            //the seller could be a manufacturer or a normal customer meaning we have to store both in the database on the 
            //condition that one field will have an ID and the other will be null. So if we have a vehicle sold by a manufacturer,
            //the manufacturerID Field will have the manufacturer ID and the SellerID field will be null, and vice verse. 

            //Vehicle ManufacturerID foreign key
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Manufacturer)
                .WithMany(m => m.Vehicles)
                .HasForeignKey(v => v.ManufacturerID)
                .OnDelete(DeleteBehavior.NoAction);

            //Vehicle SellerID foreign Key
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Seller)
                .WithMany(s => s.VehiclesSold)
                .HasForeignKey(v => v.SellerID)
                .OnDelete(DeleteBehavior.NoAction);

            //Vehicle BuyerID foreign Key
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Buyer)
                .WithMany(b => b.VehiclesBought)
                .HasForeignKey(v => v.BuyerID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Manufacturer>()
                .HasOne(m => m.Manager)
                .WithOne(c => c.ManagedManufacturer)
                .OnDelete(DeleteBehavior.NoAction)
                .HasForeignKey<Manufacturer>(m => m.ManagerID);

            modelBuilder.Entity<Branches>()
                .HasMany(b => b.Salesmen)
                .WithOne(s => s.Branch)
                .HasForeignKey(s => s.BranchID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Branches>()
                .HasOne(b => b.BranchManager)
                .WithMany(c => c.ManagedBranches)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Salesman>();

            //Transaction Buyer Foreign Key
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Buyer)
                .WithMany(c => c.Purchases)
                .HasForeignKey(t => t.BuyerID);

            //Transaction Vehicle Foreign Key
            modelBuilder.Entity<Transaction>()
                .HasOne(t=>t.Vehicle)
                .WithOne(v=>v.Transaction)
                .HasForeignKey<Transaction>(t=>t.VehicleID);

            //Transaction SellerCustomer Foreign Key
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.SellerCustomer)
                .WithMany(c => c.Sells)
                .HasForeignKey(t => t.SellerCustomerID);

            //Transaction SellerManufacturer Foreign Key
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.SellerManufacturer)
                .WithMany(m => m.Transactions)
                .HasForeignKey(t => t.SellerManufacturerID);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Customer)
                .WithMany(c => c.Notifications)
                .HasForeignKey(n => n.CustomerID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.RefreshToken)
                .WithOne(r => r.Customer)
                .HasForeignKey<RefreshTokens>(r => r.CustomerID);
        }

    }
}
