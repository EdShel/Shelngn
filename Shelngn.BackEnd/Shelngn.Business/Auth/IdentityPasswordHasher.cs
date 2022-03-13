using Microsoft.AspNetCore.Identity;
using Shelngn.Entities;
using Shelngn.Services.Auth;

namespace Shelngn.Business.Auth
{
    public class IdentityPasswordHasher : IPasswordHasher
    {
        private PasswordHasher<AppUser> passwordHasher;

        public IdentityPasswordHasher()
        {
            this.passwordHasher = new PasswordHasher<AppUser>();
        }

        public string HashPassword(string password)
        {
            return this.passwordHasher.HashPassword(null!, password);
        }

        public bool VerifyPassword(string hashedPassword, string password)
        {
            return this.passwordHasher.VerifyHashedPassword(null!, hashedPassword, password) == PasswordVerificationResult.Success;
        }
    }
}
