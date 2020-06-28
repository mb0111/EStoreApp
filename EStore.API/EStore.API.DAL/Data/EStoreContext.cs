using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EStore.API.DAL.Data
{
    public partial class EStoreContext : DbContext
    {
        public EStoreContext()
        {
        }

        public EStoreContext(DbContextOptions<EStoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Purchase> Purchase { get; set; }
        public virtual DbSet<PurchaseStatus> PurchaseStatus { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserProduct> UserProduct { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.IdCategory).HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.IdProduct)
                    .HasName("PK_Products");

                entity.HasIndex(e => e.IdCategory);

                entity.Property(e => e.IdProduct).HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.IdCategoryNavigation)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.IdCategory)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Category");
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasIndex(e => e.IdPurchaseStatus);

                entity.HasIndex(e => e.IdUserProduct);

                entity.Property(e => e.IdPurchase).HasDefaultValueSql("(newid())");

                entity.Property(e => e.IdPurchaseStatus).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.IdPurchaseStatusNavigation)
                    .WithMany(p => p.Purchase)
                    .HasForeignKey(d => d.IdPurchaseStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Purchase_PurchaseStatus");

                entity.HasOne(d => d.IdUserProductNavigation)
                    .WithMany(p => p.Purchase)
                    .HasForeignKey(d => d.IdUserProduct)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Purchase_UserProduct");
            });

            modelBuilder.Entity<PurchaseStatus>(entity =>
            {
                entity.Property(e => e.IdPurchaseStatus).ValueGeneratedNever();
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.IdRole)
                    .HasName("PK_Roles");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.IdTransaction)
                    .HasName("PK_Transactions");

                entity.Property(e => e.IdTransaction).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.IdPurchaseNavigation)
                    .WithMany(p => p.Transaction)
                    .HasForeignKey(d => d.IdPurchase)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_Purchase");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.IdUser)
                    .HasName("PK_Users");

                entity.Property(e => e.IdUser).HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<UserProduct>(entity =>
            {
                entity.HasKey(e => e.IdUserProduct)
                    .HasName("PK_UserProducts");

                entity.HasIndex(e => e.IdProduct);

                entity.HasIndex(e => e.IdUser);

                entity.Property(e => e.IdUserProduct).HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.IdProductNavigation)
                    .WithMany(p => p.UserProduct)
                    .HasForeignKey(d => d.IdProduct)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserProduct_Product");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.UserProduct)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserProduct_User");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.IdUserRole)
                    .HasName("PK_UserRoles");

                entity.HasIndex(e => e.IdRole);

                entity.HasIndex(e => e.IdUser);

                entity.Property(e => e.IdUserRole).HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.IdRoleNavigation)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.IdRole)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRole_Role");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRole_User");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
