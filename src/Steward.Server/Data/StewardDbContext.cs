using Microsoft.EntityFrameworkCore;
using Steward.Server.Data.Entities;

namespace Steward.Server.Data;

public class StewardDbContext(DbContextOptions<StewardDbContext> options) : DbContext(options)
{
    // Agent
    public DbSet<AgentEntity> Agents => Set<AgentEntity>();
    public DbSet<AgentStatusEntity> AgentStatuses => Set<AgentStatusEntity>();
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
    public DbSet<PolicyAccessEntity> PolicyAccess => Set<PolicyAccessEntity>();

    // Request
    public DbSet<OverrideRequestEntity> OverrideRequests => Set<OverrideRequestEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AgentEntity>()
            .HasIndex(a => a.InstanceId)
            .IsUnique();

        modelBuilder.Entity<AgentEntity>()
            .HasOne(a => a.Status)
            .WithOne(s => s.Agent)
            .HasForeignKey<AgentStatusEntity>(s => s.AgentId);

        modelBuilder.Entity<AgentStatusEntity>()
            .HasKey(s => s.AgentId);

        modelBuilder.Entity<AgentStatusEntity>()
            .Property(x => x.State)
            .HasConversion<string>();

        modelBuilder.Entity<UserEntity>()
            .Property(x => x.Type)
            .HasConversion<string>()
            .HasDefaultValue(UserType.Member);

        modelBuilder.Entity<UserEntity>()
            .ToTable("Users", table =>
            {
                table.HasCheckConstraint("CK_Users_Type",
                    "Type IN ('Member', 'Admin')");
                table.HasCheckConstraint("CK_Users_AdminEmail",
                    "Type <> 'Admin' OR (Email IS NOT NULL AND length(trim(Email)) > 0)");
                table.HasCheckConstraint("CK_Users_AdminPinHash",
                    "Type <> 'Admin' OR (PinHash IS NOT NULL AND length(trim(PinHash)) > 0)");
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

        modelBuilder.Entity<PolicyEntity>()
            .OwnsOne(x => x.Schedule);

        modelBuilder.Entity<PolicyEntity>()
            .OwnsOne(x => x.Access);

        modelBuilder.Entity<PolicyEntity>()
            .OwnsOne(x => x.Override, overrideBuilder =>
            {
                overrideBuilder.OwnsOne(x => x.Allowance);
            });

        modelBuilder.Entity<PolicyAccessEntity>()
            .HasKey(x => new
            {
                x.PolicyId,
                x.UserId
            });

        modelBuilder.Entity<OverrideRequestEntity>()
            .HasIndex(x => new
            {
                x.UserId,
                x.PolicyId
            })
            .IsUnique()
            .HasFilter($"Status = {(int)OverrideRequestStatus.Pending}");
    }
}