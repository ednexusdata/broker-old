namespace OregonNexus.Broker.Connector.Authentication;

public class AuthenticatedUser
{
    public string Email { get; set; } = default!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Name { get; set; }
    public string? UserId { get; set; }
}