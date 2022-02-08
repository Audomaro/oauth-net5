using BO;
using DAO;
using System.Collections.Generic;

namespace SVC
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<UserEntity> GetAll()
        {
            return _context.Users;
        }

        public UserEntity GetById(int id)
        {
            UserEntity user = _context.Users.Find(id);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            return user;
        }
    }
}
