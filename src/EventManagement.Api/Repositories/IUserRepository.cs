using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;
using EventManagement.Api.Common.Identity;

namespace EventManagement.Api.Repositories;

    public interface IUserRepository
    {
        Task<bool> UserExistsAsync(string email);
        Task<string> GetUserIdByEmailAsync(string email);
        Task<bool> ValidateUserAsync(string email, string password);
        Task<bool> CreateUserAsync(string email, string password);
        Task<bool> UpdatePasswordHashAsync(string userId, string newPasswordHash);
    }
