using Steward.Server.Data.Entities;

namespace Steward.Server.Api.Models;

public sealed class SaveUserDto
{
    public string Name { get; set; } = "";
    public UserType Type { get; set; } = UserType.Member;
    public string? Email { get; set; }
    // Null or empty preserves the PIN on edit. Clearing is explicit.
    public string? Pin { get; set; }
    public bool ClearPin { get; set; }
}
