namespace RepairShop.Helpers.interfaces;

public interface IJwtToken
{
    String GenerateJwtToken(String username, string role);
}