namespace RepairShop.Helpers.interfaces;

public interface IPasswordEncrypter
{
    (string Hash, string Salt) EncryptPassword(string password);
    string EncryptPasswordWithUsersSalt(string password, string saltHex);
}