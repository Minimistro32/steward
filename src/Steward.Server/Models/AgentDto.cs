using Steward.Server.Data.Entities;

namespace Steward.Server.Models;

public class AgentDto
{
    public int Id { get; set; }

    public string AgentId { get; set; } = "";

    public string InstanceId { get; set; } = "";

    public string Name { get; set; } = "";

    public AgentStatus Status { get; set; }

    public DateTime? LastSeen { get; set; }

    public List<DeviceDto> Devices { get; set; } = [];

    public List<ResourceDto> Resources { get; set; } = [];


    public static AgentDto FromEntity(AgentEntity agent) => new()
    {
        Id = agent.Id,
        AgentId = agent.AgentId,
        InstanceId = agent.InstanceId,
        Name = agent.Name,
        Status = agent.Status,
        LastSeen = agent.LastSeen,

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