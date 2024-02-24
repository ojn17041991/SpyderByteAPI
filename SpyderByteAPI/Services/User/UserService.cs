using SpyderByteAPI.DataAccess;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.DataAccess.Abstract.Accessors;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Helpers.Authentication;
using SpyderByteAPI.Models.Authentication;
using SpyderByteAPI.Models.Users;
using SpyderByteAPI.Services.User.Abstract;

namespace SpyderByteAPI.Services.User
{
    public class UserService : IUsersService
    {
        private readonly IUsersAccessor usersAccessor;
        private readonly PasswordHasher passwordHasher;

        public UserService(IUsersAccessor usersAccessor, PasswordHasher passwordHasher)
        {
            this.usersAccessor = usersAccessor;
            this.passwordHasher = passwordHasher;
        }

        public async Task<IDataResponse<bool>> PostAsync(PostUser user)
        {
            // OJN: Check user exists here.

            // OJN: Don't allow any user to create new Admin or Utility user types.

            // OJN: Validation and logging.

            var hashData = passwordHasher.GenerateNewHash(user.Password);

            var hashedUser = new PostHashedUser
            {
                Id = user.Id,
                HashData = hashData,
                UserType = user.UserType
            };

            var response = await usersAccessor.PostAsync(hashedUser);
            return new DataResponse<bool>(response.Result == ModelResult.Created, response.Result);
        }

        public async Task<IDataResponse<bool>> DeleteAsync(string id)
        {
            // OJN: Implement with similar restrictions as above.

            throw new NotImplementedException();
        }
    }
}
