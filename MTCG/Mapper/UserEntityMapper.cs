using MTCG.Database.Entity;
using MTCG.Resource;

namespace MTCG.Mapper
{
    public class UserEntityMapper : Mapper<UserEntity, User>
    {
        public override User Map(UserEntity obj)
        {
            if (obj is null)
            {
                return null;
            }
            
            var user =  new User(obj.Username, obj.Password, obj.Id);
            user.AddCoins(obj.Coins);
            return user;
        }
    }
}