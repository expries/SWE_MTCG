using System.Text;
using MTCG.Domain.Cards;

namespace MTCG.Domain
{
    public class GameLog
    {
        public User Winner { get; private set; }
        
        public Card WinningCard { get; private set; }
        
        public int Rounds { get; private set; }
        
        private StringBuilder LogBuilder { get; }

        public GameLog()
        {
            Winner = null;
            WinningCard = null;
            Rounds = 0;
            LogBuilder = new StringBuilder();
        }

        public override string ToString() => LogBuilder.ToString();

        public GameLog Append(string message)
        {
            LogBuilder.Append(message);
            return this;
        }

        public GameLog AddWinner(User winner)
        {
            Winner = winner;
            return this;
        }
        
        public GameLog AddWinner(Card winner)
        {
            WinningCard = winner;
            return this;
        }

        public void IncrementRounds()
        {
            Rounds++;
        }
    }
}