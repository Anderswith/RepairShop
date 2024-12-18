using RepairShop.BE;
using RepairShop.BLL.interfaces;
using RepairShop.DAL.Repositories.Interfaces;

namespace RepairShop.BLL;

public class UserDataLogic: IUserDataLogic
{
    private readonly IUserDataRepository _UserDataRepository;

    public UserDataLogic(IUserDataRepository userDataRepository)
    {
        _UserDataRepository = userDataRepository;
    }
    
    public void CreateUserData(Guid userId, string email, int phoneNumber, string firstName, string lastName)
    {
        if (string.IsNullOrEmpty(email) || !email.Contains("@"))
        {
            throw new ArgumentException("Please enter a valid email");
        }

        if (string.IsNullOrEmpty(firstName))
        {
            throw new ArgumentException("First name cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(lastName))
        {
            throw new ArgumentException("Last name cannot be null or empty.");
        }

        if (phoneNumber == null)
        {
            throw new ArgumentException("Phone number cannot be null or empty.");
        }

        if (phoneNumber.ToString().Length != 8)
        {
            throw new ArgumentException("Invalid phone number");
        }

        var userData = new UserData
        {
            UserDataId = Guid.NewGuid(),
            UserId = userId,
            PhoneNumber = phoneNumber,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
        };
        _UserDataRepository.CreateUserData(userData);
        
    }

    public UserData GetUserData(Guid userId)
    {
        return _UserDataRepository.GetUserData(userId);
    }

    public void UpdateUserData(Guid loggedInUserId, string email, int phoneNumber, string firstName, string lastName)
    {
        if (loggedInUserId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.");
        }

        // Validate input values
        if (string.IsNullOrEmpty(email) || !email.Contains("@"))
        {
            throw new ArgumentException("Please enter a valid email.");
        }

        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
        {
            throw new ArgumentException("First name and last name cannot be empty.");
        }

        if (phoneNumber.ToString().Length != 8)
        {
            throw new ArgumentException("Invalid phone number.");
        }

        // Fetch existing user data
        var existingUserData = _UserDataRepository.GetUserData(loggedInUserId);

        if (existingUserData == null)
        {
            // Create new user data if none exists
            existingUserData = new UserData
            {
                UserDataId = Guid.NewGuid(),
                UserId = loggedInUserId,
                Email = email,
                PhoneNumber = phoneNumber,
                FirstName = firstName,
                LastName = lastName,
            };

            _UserDataRepository.CreateUserData(existingUserData);
        }
        else
        {
            // Update existing user data
            existingUserData.Email = email;
            existingUserData.PhoneNumber = phoneNumber;
            existingUserData.FirstName = firstName;
            existingUserData.LastName = lastName;

            _UserDataRepository.UpdateUserData(existingUserData);
        }
    }

    
}