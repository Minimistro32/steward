using Microsoft.EntityFrameworkCore;
using Steward.Server.Data.Entities;

namespace Steward.Server.Data;

public class StewardDbContext(DbContextOptions<StewardDbContext> options) : DbContext(options)
{
    // Agent
    public DbSet<AgentEntity> Agents => Set<AgentEntity>();
    public DbSet<DeviceEntity> Devices => Set<DeviceEntity>();
    public DbSet<ResourceEntity> Resources => Set<ResourceEntity>();

    // User
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<UserDeviceEntity> UserDevices => Set<UserDeviceEntity>();

    // Ward
    public DbSet<WardEntity> Wards => Set<WardEntity>();
    public DbSet<WardUserEntity> WardUsers => Set<WardUserEntity>();
    public DbSet<WardDeviceEntity> WardDevices => Set<WardDeviceEntity>();
    public DbSet<WardResourceEntity> WardResources => Set<WardResourceEntity>();

    // Policy
    public DbSet<PolicyEntity> Policies => Set<PolicyEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AgentEntity>()
            .HasKey(x => x.AgentId);

        modelBuilder.Entity<AgentEntity>()
            .Property(a => a.Status)
            .HasConversion<string>();

        modelBuilder.Entity<UserDeviceEntity>()
            .HasKey(x => new
            {
                x.UserId,
                x.DeviceId
            });

        modelBuilder.Entity<WardUserEntity>()
            .HasKey(x => new
            {
                x.WardId,
                x.UserId
            });

        modelBuilder.Entity<WardDeviceEntity>()
            .HasKey(x => new
            {
                x.WardId,
                x.DeviceId
            });


        modelBuilder.Entity<WardResourceEntity>()
            .HasKey(x => new
            {
                x.WardId,
                x.ResourceId
            });

        modelBuilder.Entity<PolicyEntity>()
            .OwnsOne(x => x.Schedule);

        modelBuilder.Entity<PolicyEntity>()
            .OwnsOne(x => x.Access);

        modelBuilder.Entity<PolicyEntity>()
            .OwnsOne(x => x.Override, overrideBuilder =>
            {
                overrideBuilder.OwnsOne(x => x.Allowance);
            });
    }
}