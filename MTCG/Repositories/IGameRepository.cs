using MTCG.Domain;

namespace MTCG.Repositories
{
    public interface IGameRepository
    {
        public Game FindForUser(User user);

        public Game Create(Game game);

        public void Delete(Game game);
    }
}