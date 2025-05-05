using System.Threading.Tasks;
using EventManagement.Api.Common.Identity;
using EventManagement.Api.Repositories;

namespace EventManagement.Api.Features.Users.Login;

public class LoginHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly JwtTokenService _jwtTokenService;

        public LoginHandler(
            IUserRepository userRepository, 
            IRoleRepository roleRepository,
            JwtTokenService jwtTokenService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        }

        public async Task<LoginResponse> HandleAsync(LoginRequest request)
        {
            // Create the response object with the return URL (if provided)
            var response = new LoginResponse
            {
                ReturnUrl = request.ReturnUrl
            };

            // Validate credentials (this will also update the password hash if needed)
            var isValid = await _userRepository.ValidateUserAsync(request.Email, request.Password);
            if (!isValid)
            {
                response.Success = false;
                response.ErrorMessage = "Invalid email or password.";
                return response;
            }

            // Get user ID
            var userId = await _userRepository.GetUserIdByEmailAsync(request.Email);
            if (string.IsNullOrEmpty(userId))
            {
                response.Success = false;
                response.ErrorMessage = "User not found.";
                return response;
            }

            // Get user roles
            var roles = await _roleRepository.GetUserRolesAsync(userId);

            // Set response properties
            response.Success = true;
            response.UserId = userId;
            response.Email = request.Email;
            response.Roles.AddRange(roles);
            
            // Generate JWT token
            response.Token = _jwtTokenService.GenerateToken(userId, request.Email, roles);

            return response;
        }
    }

// public class LoginHandler
// {
//     private readonly IUserRepository _userRepository;
//     private readonly IRoleRepository _roleRepository;

//     public LoginHandler(IUserRepository userRepository, IRoleRepository roleRepository)
//     {
//         _userRepository = userRepository;
//         _roleRepository = roleRepository;
//     }

//     public async Task<LoginResponse> HandleAsync(LoginRequest request)
//     {
//         var response = new LoginResponse
//         {
//             ReturnUrl = request.ReturnUrl
//         };

//         // Validate credentials
//         var isValid = await _userRepository.ValidateUserAsync(request.Email, request.Password);
//         if (!isValid)
//         {
//             response.Success = false;
//             response.ErrorMessage = "Invalid email or password.";
//             return response;
//         }

//         // Get user ID
//         var userId = await _userRepository.GetUserIdByEmailAsync(request.Email);
//         if (string.IsNullOrEmpty(userId))
//         {
//             response.Success = false;
//             response.ErrorMessage = "User not found.";
//             return response;
//         }

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
