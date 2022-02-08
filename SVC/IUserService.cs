using BO;
using System.Collections.Generic;

namespace SVC
{
    public interface IUserService
    {
        IEnumerable<UserEntity> GetAll();
        UserEntity GetById(int id);
    }
}
