using System.Threading.Tasks;
using EventManagement.Api.Repositories;

namespace EventManagement.Api.Features.Users.Register;

public class RegisterHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public RegisterHandler(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
    }

    public async Task<RegisterResponse> HandleAsync(RegisterRequest request)
    {
        var response = new RegisterResponse();

        // Validate request
        if (request.Password != request.ConfirmPassword)
        {
            response.Success = false;
            response.Errors.Add("The password and confirmation password do not match.");
            return response;
        }

        // Check if user already exists
        var userExists = await _userRepository.UserExistsAsync(request.Email);
        if (userExists)
        {
            response.Success = false;
            response.Errors.Add("User with this email already exists.");
            return response;
        }

        // Create user with hashed password (handled in the repository)
        var success = await _userRepository.CreateUserAsync(request.Email, request.Password);
        if (!success)
        {
            response.Success = false;
            response.Errors.Add("Failed to create user.");
            return response;
        }

        // Get user ID
        var userId = await _userRepository.GetUserIdByEmailAsync(request.Email);
        if (string.IsNullOrEmpty(userId))
        {
            response.Success = false;
            response.Errors.Add("Failed to retrieve user ID.");
            return response;
        }

        // Add user to default role
        await _roleRepository.AddUserToRoleAsync(userId, "User");

        // Get user roles
        var roles = await _roleRepository.GetUserRolesAsync(userId);

        // Set response properties
        response.Success = true;
        response.UserId = userId;
        response.Email = request.Email;
        response.Roles.AddRange(roles);

        return response;
    }
}

// public class RegisterHandler
// {
//     private readonly IUserRepository _userRepository;
//     private readonly IRoleRepository _roleRepository;

//     public RegisterHandler(IUserRepository userRepository, IRoleRepository roleRepository)
//     {
//         _userRepository = userRepository;
//         _roleRepository = roleRepository;
//     }

//     public async Task<RegisterResponse> HandleAsync(RegisterRequest request)
//     {
//         var response = new RegisterResponse();

//         // Validate request
//         if (request.Password != request.ConfirmPassword)
//         {
//             response.Success = false;
//             response.Errors.Add("The password and confirmation password do not match.");
//             return response;
//         }

//         // Check if user already exists
//         var userExists = await _userRepository.UserExistsAsync(request.Email);
//         if (userExists)
//         {
//             response.Success = false;
//             response.Errors.Add("User with this email already exists.");
//             return response;
//         }

//         // Create user
//         var success = await _userRepository.CreateUserAsync(request.Email, request.Password);
//         if (!success)
//         {
//             response.Success = false;
//             response.Errors.Add("Failed to create user.");
//             return response;
//         }

//         // Get user ID
//         var userId = await _userRepository.GetUserIdByEmailAsync(request.Email);
//         if (string.IsNullOrEmpty(userId))
//         {
//             response.Success = false;
//             response.Errors.Add("Failed to retrieve user ID.");
//             return response;
//         }

//         // Add user to default role
//         await _roleRepository.AddUserToRoleAsync(userId, "User");

//         // Get user roles
//         var roles = await _roleRepository.GetUserRolesAsync(userId);

//         // Set response properties
//         response.Success = true;
//         response.UserId = userId;
//         response.Email = request.Email;
//         response.Roles = new System.Collections.Generic.List<string>(roles);

//         return response;
//     }
// }
