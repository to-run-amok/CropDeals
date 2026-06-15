using CropDeals.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CropDeals.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Farmer> Farmers { get; set; }
        public DbSet<Dealer> Dealers { get; set; }
        public DbSet<Crop> Crops { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }


        //Fluent api - for explicity mapping tables 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Crop>()
                .HasOne(c => c.Farmer)
                .WithMany(f => f.Crops)
                .HasForeignKey(c => c.FarmerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Crop>()
                .HasOne(c => c.PurchasedByDealer)
                .WithMany()
                .HasForeignKey(c => c.PurchasedByDealerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Receipt>()
                .HasOne(r => r.Farmer)
                .WithMany()
                .HasForeignKey(r => r.FarmerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Receipt>()
                .HasOne(r => r.Crop)
                .WithMany()
                .HasForeignKey(r => r.CropId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Dealer)
                .WithMany()
                .HasForeignKey(i => i.DealerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Crop)
                .WithMany()
                .HasForeignKey(i => i.CropId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BankAccount>()
                .HasOne(b => b.Farmer)
                .WithOne(f => f.BankAccount)
                .HasForeignKey<BankAccount>(b => b.FarmerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BankAccount>()
                .HasOne(b => b.Dealer)
                .WithOne(d => d.BankAccount)
                .HasForeignKey<BankAccount>(b => b.DealerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Dealer)
                .WithMany()
                .HasForeignKey(p => p.DealerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Farmer)
                .WithMany()
                .HasForeignKey(p => p.FarmerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Crop)
                .WithMany()
                .HasForeignKey(p => p.CropId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Dealer)
                .WithMany()
                .HasForeignKey(r => r.DealerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Farmer)
                .WithMany()
                .HasForeignKey(r => r.FarmerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Crop)
                .WithMany()
                .HasForeignKey(r => r.CropId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Dealer)
                .WithMany()
                .HasForeignKey(s => s.DealerId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Crop>()
                .Property(c => c.Quantity)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Crop>()
                .Property(c => c.AgreedPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Receipt>()
                .Property(r => r.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Receipt>()
                .Property(r => r.Quantity)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Invoice>()
                .Property(i => i.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Invoice>()
                .Property(i => i.Quantity)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);
        }
    }
}