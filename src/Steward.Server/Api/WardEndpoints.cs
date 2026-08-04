using Microsoft.EntityFrameworkCore;
using Steward.Server.Data;
using Steward.Server.Models;
using Steward.Server.Data.Entities;

namespace Steward.Server.Api;

public static class WardEndpoints
{
    public static void MapWardEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/wards");

        // GET
        group.MapGet("/", async (StewardDbContext db) =>
        {
            var wards = await db.Wards
                .IncludeDetails()
                .ToListAsync();


            return Results.Ok(
                wards.Select(WardDto.FromEntity)
            );
        });

        // GET {ID}
        group.MapGet("/{id}", async (
            string id,
            StewardDbContext db) =>
        {
            var ward = await db.Wards
                .IncludeDetails()
                .FirstOrDefaultAsync(w => w.Id.ToString() == id);

            if (ward is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(
                WardDto.FromEntity(ward)
            );
        });

        // POST
        group.MapPost("/", async (
            WardDto dto,
            StewardDbContext db) =>
        {
            var ward = new WardEntity
            {
                Id = dto.Id,
                Name = dto.Name,
                Tags = dto.Tags
            };

            AddSelections(ward, dto);

            db.Wards.Add(ward);
            await db.SaveChangesAsync();

            return Results.Created(
                $"/api/wards/{ward.Id}",
                dto
            );
        });


        // PUT
        group.MapPut("/{id}", async (
            string id,
            WardDto dto,
            StewardDbContext db) =>
        {
            var ward = await db.Wards
                .IncludeDetails()
                .FirstOrDefaultAsync(w => w.Id.ToString() == id);


            if (ward is null)
            {
                return Results.NotFound();
            }

            ward.Name = dto.Name;
            ward.Tags = dto.Tags;

            ward.Users.Clear();
            ward.Devices.Clear();
            ward.Resources.Clear();

            AddSelections(ward, dto);

            await db.SaveChangesAsync();


            return Results.Ok(dto);
        });

        // DELETE
        group.MapDelete("/{id}", async (
            string id,
            StewardDbContext db) =>
        {
            var ward = await db.Wards
                .FirstOrDefaultAsync(w => w.Id.ToString() == id);


            if (ward is null)
            {
                return Results.NotFound();
            }


            db.Wards.Remove(ward);

            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }

    private static void AddSelections(
        WardEntity ward,
        WardDto dto)
    {
        foreach (var userId in dto.UserIds)
        {
            ward.Users.Add(new WardUserEntity
            {
                WardId = ward.Id,
                UserId = userId
            });
        }

        foreach (var deviceId in dto.DeviceIds)
        {
            ward.Devices.Add(new WardDeviceEntity
            {
                WardId = ward.Id,
                DeviceId = deviceId
            });
        }


        foreach (var resourceId in dto.ResourceIds)
        {
            ward.Resources.Add(new WardResourceEntity
            {
                WardId = ward.Id,
                ResourceId = resourceId
            });
        }
    }

    public static IQueryable<WardEntity> IncludeDetails(
        this IQueryable<WardEntity> query)
    {
        return query
            .AsSplitQuery()
            .Include(w => w.Users)
            .Include(w => w.Devices)
                .ThenInclude(wd => wd.Device)
            .Include(w => w.Resources)
                .ThenInclude(wr => wr.Resource);
    }
}