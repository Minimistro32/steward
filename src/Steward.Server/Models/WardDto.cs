using Steward.Server.Data.Entities;

namespace Steward.Server.Models;

public class WardDto
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public List<string> Tags { get; set; } = [];

    public List<int> UserIds { get; set; } = [];

    public List<int> DeviceIds { get; set; } = [];

    public List<int> ResourceIds { get; set; } = [];

    public static WardDto FromEntity(WardEntity ward) => new()
    {
        Id = ward.Id,

        Name = ward.Name,

        Tags = [.. ward.Tags],

        UserIds = [.. ward.Users.Select(x => x.UserId)],

        DeviceIds = [.. ward.Devices.Select(x => x.DeviceId)],

        ResourceIds = [.. ward.Resources.Select(x => x.ResourceId)],
    };
}