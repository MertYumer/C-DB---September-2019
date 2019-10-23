namespace P03_SalesDatabase.Data
{
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class SalesContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Sale> Sales { get; set; }

        public DbSet<Store> Stores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureCustomerEntity(modelBuilder);

            ConfigureProductEntity(modelBuilder);

            ConfigureSaleEntity(modelBuilder);

            ConfigureStoreEntity(modelBuilder);
        }

        private void ConfigureCustomerEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(customer =>
            {
                customer.HasKey(c => c.CustomerId);

                customer
                .HasMany(c => c.Sales)
                .WithOne(s => s.Customer);

                customer.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired()
                .IsUnicode();

                customer
                .Property(c => c.Email)
                .HasMaxLength(80)
                .IsRequired();
            });
        }

        private void ConfigureProductEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(product =>
            {
                product.HasKey(p => p.ProductId);

                product
                .HasMany(p => p.Sales)
                .WithOne(s => s.Product);

                product.Property(p => p.Name)
                .HasMaxLength(50)
                .IsRequired()
                .IsUnicode();

                product.Property(p => p.Description)
                .HasMaxLength(250)
                .HasDefaultValue("No desription");
            });
        }

        private void ConfigureSaleEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sale>(sale =>
            {
                sale.HasKey(s => s.SaleId);

                sale.Property(s => s.Date).HasDefaultValueSql("GETDATE()");
            });
        }

        private void ConfigureStoreEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Store>(store =>
            {
                store.HasKey(s => s.StoreId);

                store
                .HasMany(st => st.Sales)
                .WithOne(sa => sa.Store);

                store.Property(p => p.Name)
                .HasMaxLength(80)
                .IsRequired()
                .IsUnicode();
            });
        }
    }
}
