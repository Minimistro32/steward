using Steward.Server.Data.Entities;

namespace Steward.Server.Api.Models;

public class UserDto
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public List<int> DeviceIds { get; set; } = [];

    public static UserDto FromEntity(UserEntity user) => new()
    {
        Id = user.Id,

        Name = user.Name,

        DeviceIds = [.. user.UserDevices.Select(x => x.DeviceId)]
    };
}