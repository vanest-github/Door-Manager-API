namespace DoorManager.Entity.Configurations;

public class JWTConfiguration
{
    public string SecretKey { get; set; }

    public string Issuer { get; set; }

    public string Audience { get; set; }

    public int TokenExpirationDays { get; set; }
}
