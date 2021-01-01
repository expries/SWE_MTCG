using MTCG.Domain;

namespace MTCG.Repositories
{
    public interface IBattleRepository
    {
        public Battle FindForUser(User user);

        public Battle Create(Battle battle);

        public void Delete(Battle battle);
    }
}