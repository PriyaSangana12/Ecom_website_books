﻿﻿using BookCart.Dto;
using BookCart.Helpers;
using BookCart.Interfaces;
using BookCart.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BookCart.DataAccess
{
    public class UserDataAccessLayer(BookDBContext dbContext) : IUserService
    {
        readonly BookDBContext _dbContext = dbContext;

        public AuthenticatedUser AuthenticateUser(UserLogin loginCredentials)
        {
            AuthenticatedUser authenticatedUser = new();

            var userDetails = _dbContext.UserMaster.FirstOrDefault(
                u => u.Username == loginCredentials.Username);

            if (userDetails == null || !PasswordHelper.VerifyPassword(loginCredentials.Password, userDetails.Password))
            {
                return new AuthenticatedUser();
            }

            authenticatedUser = new AuthenticatedUser
            {
                Username = userDetails.Username,
                UserId = userDetails.UserId,
                UserTypeName = userDetails.UserTypeId == 1 ? "Admin" : "User"
            };
            return authenticatedUser;
        }

        public async Task<bool> RegisterUser(UserMaster userData)
        {
            if (!CheckUserNameAvailabity(userData.Username))
            {
                return false;
            }

            try
            {
                Console.WriteLine($"Starting registration for user: {userData.Username}");
                userData.Password = PasswordHelper.HashPassword(userData.Password);
                Console.WriteLine("Password hashed successfully");
                
                await _dbContext.UserMaster.AddAsync(userData);
                Console.WriteLine("User added to context");
                
                int changes = await _dbContext.SaveChangesAsync();
                Console.WriteLine($"SaveChanges completed with {changes} records affected");
                
                return changes > 0;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                throw new Exception("Database update failed", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw new Exception("Registration failed", ex);
            }
        }

        public bool CheckUserNameAvailabity(string userName)
        {
            UserMaster user = _dbContext.UserMaster.FirstOrDefault(x => x.Username == userName);

            return user == null;
        }

        public async Task<bool> isUserExists(int userId)
        {
            UserMaster user = await _dbContext.UserMaster.FirstOrDefaultAsync(x => x.UserId == userId);

            return user != null;
        }
    }
}
