using CanonPoint.App.Models;
using Microsoft.EntityFrameworkCore;

namespace CanonPoint.App.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Status> Statuses { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Point> Points { get; set; }
    public DbSet<Shot> Shots { get; set; }
    public DbSet<Move> Moves { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration de Status
        modelBuilder.Entity<Status>()
            .ToTable("statuses")
            .HasKey(s => s.Id);

        modelBuilder.Entity<Status>()
            .Property(s => s.Name)
            .HasMaxLength(50)
            .IsRequired();

        // Configuration de Game
        modelBuilder.Entity<Game>()
            .ToTable("games")
            .HasKey(g => g.Id);

        modelBuilder.Entity<Game>()
            .Property(g => g.Name)
            .HasMaxLength(100);

        modelBuilder.Entity<Game>()
            .HasOne(g => g.Status)
            .WithMany(s => s.Games)
            .HasForeignKey(g => g.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuration de Player
        modelBuilder.Entity<Player>()
            .ToTable("players")
            .HasKey(p => p.Id);

        modelBuilder.Entity<Player>()
            .Property(p => p.Name)
            .HasMaxLength(100);

        modelBuilder.Entity<Player>()
            .Property(p => p.ColorHex)
            .HasMaxLength(7);

        // Configuration de Point
        modelBuilder.Entity<Point>()
            .ToTable("points")
            .HasKey(p => p.Id);

        modelBuilder.Entity<Point>()
            .HasOne(p => p.Game)
            .WithMany(g => g.Points)
            .HasForeignKey(p => p.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Point>()
            .HasOne(p => p.Owner)
            .WithMany(pl => pl.Points)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuration de Shot
        modelBuilder.Entity<Shot>()
            .ToTable("shots")
            .HasKey(s => s.Id);

        modelBuilder.Entity<Shot>()
            .HasOne(s => s.Game)
            .WithMany(g => g.Shots)
            .HasForeignKey(s => s.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Shot>()
            .HasOne(s => s.Player)
            .WithMany(p => p.Shots)
            .HasForeignKey(s => s.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Shot>()
            .ToTable(t => t.HasCheckConstraint("CK_shots_power", "\"Power\" BETWEEN 1 AND 9"));

        // Configuration de Move
        modelBuilder.Entity<Move>()
            .ToTable("moves")
            .HasKey(m => m.Id);

        modelBuilder.Entity<Move>()
            .HasOne(m => m.Game)
            .WithMany(g => g.Moves)
            .HasForeignKey(m => m.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Move>()
            .HasOne(m => m.Player)
            .WithMany(p => p.Moves)
            .HasForeignKey(m => m.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Move>()
            .HasOne(m => m.Point)
            .WithMany(p => p.Moves)
            .HasForeignKey(m => m.PointId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Move>()
            .HasOne(m => m.Shot)
            .WithMany(s => s.Moves)
            .HasForeignKey(m => m.ShotId)
            .OnDelete(DeleteBehavior.SetNull);

        // Index unique sur (GameId, SequenceNumber)
        modelBuilder.Entity<Move>()
            .HasIndex(m => new { m.GameId, m.SequenceNumber })
            .IsUnique();
    }
}
