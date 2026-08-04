using Steward.Server.Data.Entities;

namespace Steward.Server.Models;

public class WardDto
{
    public string Id { get; set; } = "";

    public string Name { get; set; } = "";

    public List<string> Tags { get; set; } = [];

    public List<string> UserIds { get; set; } = [];

    public Dictionary<string, WardAgentSelectionDto> AgentSelections { get; set; } = [];

    public class WardAgentSelectionDto
    {
        public List<string> DeviceIds { get; set; } = [];

        public List<string> ResourceIds { get; set; } = [];
    }

    public static WardDto FromEntity(WardEntity ward) => new()
    {
        Id = ward.Id,

        Name = ward.Name,

        Tags = [.. ward.Tags],

        UserIds = [.. ward.Users.Select(x => x.UserId)],

        AgentSelections = ward.Devices
            .GroupBy(x => x.Device.AgentId)
            .ToDictionary(
                group => group.Key,
                group => new WardAgentSelectionDto
                {
                    DeviceIds = [.. group.Select(x => x.DeviceId)],

                    ResourceIds = [.. ward.Resources
                        .Where(resource =>
                            resource.Resource.AgentId == group.Key)
                        .Select(resource =>
                            resource.ResourceId)]
                })
    };
}