using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ProjectB.sakila
{
    public partial class withprimarykeyContext : DbContext
    {
        public withprimarykeyContext()
        {
        }

        public withprimarykeyContext(DbContextOptions<withprimarykeyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Sale> Sales { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySQL("server=localhost;user=root;password=;database=withprimarykey;SslMode=none");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => e.Product)
                    .HasName("PRIMARY");

                entity.ToTable("sales");

                entity.Property(e => e.Product)
                    .HasMaxLength(250)
                    .HasColumnName("product");

                entity.Property(e => e.Price)
                    .HasColumnType("int(11)")
                    .HasColumnName("price");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
