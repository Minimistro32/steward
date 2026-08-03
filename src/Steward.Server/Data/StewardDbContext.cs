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

    // Ward
    public DbSet<WardEntity> Wards => Set<WardEntity>();
    public DbSet<WardUserEntity> WardUsers => Set<WardUserEntity>();
    public DbSet<WardDeviceEntity> WardDevices => Set<WardDeviceEntity>();
    public DbSet<WardResourceEntity> WardResources => Set<WardResourceEntity>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AgentEntity>()
            .HasIndex(x => new { x.AgentId, x.InstanceId })
            .IsUnique();


        modelBuilder.Entity<UserDeviceEntity>()
            .HasKey(x => new
            {
                x.UserId,
                x.DeviceId
            });

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
    }
}