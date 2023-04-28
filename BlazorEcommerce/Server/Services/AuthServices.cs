using System.Security.Cryptography;

namespace BlazorEcommerce.Server.Services
{
    public interface IAuthServices
    {
        Task<ServiceResponse<int>> Register(User user, string password);
        Task<bool> UserExists(string email);
    }

    public class AuthServices : IAuthServices
    {
        private readonly DataContext _context;

        public AuthServices(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            if (await UserExists(user.Email))
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "User Already Exists."
                };
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new ServiceResponse<int> { Data = user.Id, Message = "Register Success" };
        }

        public async Task<bool> UserExists(string email)
        {
            var result = await _context.Users.Where(q => q.Email.ToLower() == email.ToLower()).ToListAsync();

            if (result.Count > 0)
            {
                return true;
            }

            return false;

        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
