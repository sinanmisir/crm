using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CRM.Models;

public partial class CrmDbContext : DbContext
{
    public CrmDbContext()
    {
    }

    public CrmDbContext(DbContextOptions<CrmDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Etiketler> Etiketlers { get; set; }

    public virtual DbSet<Firsatlar> Firsatlars { get; set; }

    public virtual DbSet<Gorevler> Gorevlers { get; set; }

    public virtual DbSet<IslemLog> IslemLogs { get; set; }

    public virtual DbSet<Kisiler> Kisilers { get; set; }

    public virtual DbSet<Kullanicilar> Kullanicilars { get; set; }

    public virtual DbSet<MusteriEtiketler> MusteriEtiketlers { get; set; }

    public virtual DbSet<Musteriler> Musterilers { get; set; }

    public virtual DbSet<Notlar> Notlars { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-C0TUPMF\\SQLEXPRESS02;Database=crmDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Etiketler>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Etiketle__3214EC27ACE65F3C");

            entity.ToTable("Etiketler");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Ad).HasMaxLength(100);
        });

        modelBuilder.Entity<Firsatlar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Firsatla__3214EC2725318787");

            entity.ToTable("Firsatlar");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Baslik).HasMaxLength(100);
            entity.Property(e => e.MusteriId).HasColumnName("MusteriID");
            entity.Property(e => e.SilindiMi).HasDefaultValue(false);
            entity.Property(e => e.TahminiGelir).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Musteri).WithMany(p => p.Firsatlars)
                .HasForeignKey(d => d.MusteriId)
                .HasConstraintName("FK__Firsatlar__Muste__3C69FB99");
        });

        modelBuilder.Entity<Gorevler>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Gorevler__3214EC27491A0D1A");

            entity.ToTable("Gorevler");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Durum).HasMaxLength(20);
            entity.Property(e => e.KisiId).HasColumnName("KisiID");
            entity.Property(e => e.MusteriId).HasColumnName("MusteriID");
            entity.Property(e => e.SilindiMi).HasDefaultValue(false);
            entity.Property(e => e.SorumluKullaniciId).HasColumnName("SorumluKullaniciID");

            entity.HasOne(d => d.Kisi).WithMany(p => p.Gorevlers)
                .HasForeignKey(d => d.KisiId)
                .HasConstraintName("FK__Gorevler__KisiID__4222D4EF");

            entity.HasOne(d => d.Musteri).WithMany(p => p.Gorevlers)
                .HasForeignKey(d => d.MusteriId)
                .HasConstraintName("FK__Gorevler__Muster__412EB0B6");

            entity.HasOne(d => d.SorumluKullanici).WithMany(p => p.Gorevlers)
                .HasForeignKey(d => d.SorumluKullaniciId)
                .HasConstraintName("FK__Gorevler__Soruml__4316F928");
        });

        modelBuilder.Entity<IslemLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__IslemLog__3214EC271D6C1B87");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Islem).HasMaxLength(255);
            entity.Property(e => e.KullaniciAd).HasMaxLength(100);
            entity.Property(e => e.Tarih)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Musteri).WithMany(p => p.IslemLogs)
                .HasForeignKey(d => d.MusteriId)
                .HasConstraintName("FK_IslemLogs_Musteriler");
        });

        modelBuilder.Entity<Kisiler>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Kisiler__3214EC27AA6C9642");

            entity.ToTable("Kisiler");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AdSoyad).HasMaxLength(100);
            entity.Property(e => e.Eposta).HasMaxLength(100);
            entity.Property(e => e.Gorev).HasMaxLength(50);
            entity.Property(e => e.MusteriId).HasColumnName("MusteriID");
            entity.Property(e => e.SilindiMi).HasDefaultValue(false);
            entity.Property(e => e.Telefon).HasMaxLength(20);

            entity.HasOne(d => d.Musteri).WithMany(p => p.Kisilers)
                .HasForeignKey(d => d.MusteriId)
                .HasConstraintName("FK__Kisiler__Musteri__398D8EEE");
        });

        modelBuilder.Entity<Kullanicilar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Kullanic__3214EC276AAD56BD");

            entity.ToTable("Kullanicilar");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AdSoyad).HasMaxLength(100);
            entity.Property(e => e.Eposta).HasMaxLength(100);
            entity.Property(e => e.KullaniciAdi).HasMaxLength(50);
            entity.Property(e => e.Rol).HasMaxLength(20);
            entity.Property(e => e.SifreHash).HasMaxLength(255);
        });

        modelBuilder.Entity<MusteriEtiketler>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MusteriE__3214EC2701FB96B5");

            entity.ToTable("MusteriEtiketler");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.EtiketId).HasColumnName("EtiketID");
            entity.Property(e => e.MusteriId).HasColumnName("MusteriID");

            entity.HasOne(d => d.Etiket).WithMany(p => p.MusteriEtiketlers)
                .HasForeignKey(d => d.EtiketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MusteriEt__Etike__5FB337D6");

            entity.HasOne(d => d.Musteri).WithMany(p => p.MusteriEtiketlers)
                .HasForeignKey(d => d.MusteriId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MusteriEt__Muste__5EBF139D");
        });

        modelBuilder.Entity<Musteriler>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Musteril__3214EC27ADA11E3C");

            entity.ToTable("Musteriler");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AdSoyad).HasMaxLength(100);
            entity.Property(e => e.Adres).HasMaxLength(255);
            entity.Property(e => e.Eposta).HasMaxLength(100);
            entity.Property(e => e.KayitTarihi).HasColumnType("datetime");
            entity.Property(e => e.SilindiMi).HasDefaultValue(false);
            entity.Property(e => e.Telefon).HasMaxLength(20);
        });

        modelBuilder.Entity<Notlar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notlar__3214EC2765E88DE8");

            entity.ToTable("Notlar");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Baslik).HasMaxLength(100);
            entity.Property(e => e.KisiId).HasColumnName("KisiID");
            entity.Property(e => e.MusteriId).HasColumnName("MusteriID");
            entity.Property(e => e.SilindiMi).HasDefaultValue(false);
            entity.Property(e => e.Tarih).HasColumnType("datetime");

            entity.HasOne(d => d.Kisi).WithMany(p => p.Notlars)
                .HasForeignKey(d => d.KisiId)
                .HasConstraintName("FK__Notlar__KisiID__46E78A0C");

            entity.HasOne(d => d.Musteri).WithMany(p => p.Notlars)
                .HasForeignKey(d => d.MusteriId)
                .HasConstraintName("FK__Notlar__MusteriI__45F365D3");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
