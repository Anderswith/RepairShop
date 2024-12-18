using RepairShop.BE;
using RepairShop.BLL.interfaces;
using RepairShop.DAL.Repositories.Interfaces;
using RepairShop.Helpers.interfaces;

namespace RepairShop.BLL;

public class UserLogic : IUserLogic
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordEncrypter _passwordEncrypter;
    private readonly IJwtToken _jwtToken;

    public UserLogic(IUserRepository userRepository, IPasswordEncrypter passwordEncrypter, IJwtToken jwtToken)
    {
        _userRepository = userRepository;
        _passwordEncrypter = passwordEncrypter;
        _jwtToken = jwtToken;
    }

    public User RegisterUser(string username, string password, string role, Guid userId)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new ArgumentException("Username cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("Password cannot be null or empty.");
        }

        var existingUser = _userRepository.GetUserByUserName(username);
        if (existingUser != null)
        {
            throw new ArgumentException("Username is already taken.");
        }
        
        var (hash, salt) = _passwordEncrypter.EncryptPassword(password);
        
        var user = new User
        {
            UserId = userId,
            Hash = hash,
            Salt = salt,
            Username = username,
            Role = role
        };
        
        _userRepository.RegisterUser(user);
        

        return user;
    }



    public (User, string token) LoginUser(string username, string password)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new ArgumentException("Username cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("Password cannot be null or empty.");
        }

        var user = _userRepository.GetUserByUserName(username);
        if (user == null)
        {
            throw new ArgumentException("Invalid username or password.");
        }

        var hashWithStoredSalt = _passwordEncrypter.EncryptPasswordWithUsersSalt(password, user.Salt);

        if (hashWithStoredSalt != user.Hash)
        {
            throw new ArgumentException("Invalid username or password.");
        }

        var token = _jwtToken.GenerateJwtToken(username, user.Role);
        return (user, token);
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _userRepository.GetUsers();
    }

    public User GetUserByName(string username)
    {
        return _userRepository.GetUserByUserName(username);
    }
}