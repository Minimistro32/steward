using Steward.Server.Data.Entities;

namespace Steward.Server.Models;

public class AgentDto
{
    public string Id { get; set; } = "";

    public string InstanceId { get; set; } = "";

    public string Name { get; set; } = "";

    public AgentStatus Status { get; set; }

    public DateTime? LastContact { get; set; }

    public List<DeviceDto> Devices { get; set; } = [];

    public List<ResourceDto> Resources { get; set; } = [];


    public static AgentDto FromEntity(AgentEntity agent) => new()
    {
        Id = agent.Id,
        InstanceId = agent.InstanceId,
        Name = agent.Name,
        Status = agent.Status?.State ?? AgentStatus.Offline,
        LastContact = agent.Status?.LastContact,

        Devices =
        [
            .. agent.Devices.Select(device => new DeviceDto
            {
                Id = device.Id,
                DeviceId = device.DeviceId,
                Name = device.Name
            })
        ],

        Resources =
        [
            .. agent.Resources.Select(resource => new ResourceDto
            {
                Id = resource.Id,
                ResourceId = resource.ResourceId,
                Name = resource.Name
            })
        ]
    };
}