using System.Collections.Generic;
using System.Linq;
using MTCG.Domain;
using MTCG.Entities;

namespace MTCG.Mappers
{
    public static class UserEntityMapper
    {
        public static List<User> Map(IEnumerable<UserEntity> entities)
        {
            return entities.Select(Map).ToList();
        }
        
        public static User Map(UserEntity obj)
        {
            if (obj is null)
            {
                return null;
            }

            var creation = User.Create(obj.Username, obj.Password, obj.Id);
            var user = creation.Value;
            user.AddCoins(obj.Coins);
            return user;
        }
    }
}