using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GFL_TZ.Models;

public partial class GflDbContext : DbContext
{
    public GflDbContext()
    {
    }

    public GflDbContext(DbContextOptions<GflDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Catalogy> Catalogies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-CFOLTDF\\SQLEXPRESS;Database=GFL_DB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Catalogy>(entity =>
        {
            entity.ToTable("Catalogy");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
