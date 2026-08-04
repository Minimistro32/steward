using Steward.Server.Data.Entities;

namespace Steward.Server.Models;

public class UserDto
{
    public string Id { get; set; } = "";

    public string Name { get; set; } = "";

    public Dictionary<string, UserAgentSelectionDto> AgentSelections { get; set; } = [];

    public class UserAgentSelectionDto
    {
        public List<string> DeviceIds { get; set; } = [];
    }

    public static UserDto FromEntity(UserEntity user) => new()
    {
        Id = user.Id,

        Name = user.Name,

        AgentSelections = user.UserDevices
            .GroupBy(x => x.Device.AgentId)
            .ToDictionary(
                group => group.Key,
                group => new UserAgentSelectionDto
                {
                    DeviceIds = [.. group.Select(x => x.DeviceId)]
                })
    };
}