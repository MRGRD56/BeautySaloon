using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using BeautySaloon.Model.DbModels;

namespace BeautySaloon.Context
{
    public partial class AppContext : DbContext
    {
        public AppContext()
            : base(@"data source=localhost\SQLEXPRESS;initial catalog=Beauty;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework")
        {
        }

        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<ClientService> ClientServices { get; set; }
        public virtual DbSet<DocumentByService> DocumentByServices { get; set; }
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<Manufacturer> Manufacturers { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductPhoto> ProductPhotoes { get; set; }
        public virtual DbSet<ProductSale> ProductSales { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServicePhoto> ServicePhotoes { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                .Property(e => e.GenderCode)
                .IsFixedLength();

            modelBuilder.Entity<Client>()
                .HasMany(e => e.ClientServices)
                .WithRequired(e => e.Client)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.Tags)
                .WithMany(e => e.Clients)
                .Map(m => m.ToTable("TagOfClient").MapLeftKey("ClientID").MapRightKey("TagID"));

            modelBuilder.Entity<ClientService>()
                .HasMany(e => e.DocumentByServices)
                .WithRequired(e => e.ClientService)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Gender>()
                .Property(e => e.Code)
                .IsFixedLength();

            modelBuilder.Entity<Gender>()
                .HasMany(e => e.Clients)
                .WithRequired(e => e.Gender)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.Cost)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ProductPhotoes)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ProductSales)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Product1)
                .WithMany(e => e.Products)
                .Map(m => m.ToTable("AttachedProduct").MapLeftKey("MainProductID").MapRightKey("AttachedProductID"));

            modelBuilder.Entity<Service>()
                .Property(e => e.Cost)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Service>()
                .HasMany(e => e.ClientServices)
                .WithRequired(e => e.Service)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Service>()
                .HasMany(e => e.ServicePhotos)
                .WithRequired(e => e.Service)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Tag>()
                .Property(e => e.Color)
                .IsFixedLength();
        }
    }
}
