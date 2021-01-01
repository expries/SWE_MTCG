using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MTCG.Domain.Cards
{
    public class BattleData
    {
        public User Winner { get; private set; }
        public Card WinningCard { get; private set; }
        private StringBuilder LogBuilder { get; }
        
        public BattleData()
        {
            Winner = null;
            LogBuilder = new StringBuilder();
        }

        public override string ToString() => LogBuilder.ToString();

        public BattleData Log(string message)
        {
            LogBuilder.Append(message);
            return this;
        }

        public BattleData AddWinner(User winner)
        {
            Winner = winner;
            return this;
        }

        public BattleData AddWinner(Card winner)
        {
            WinningCard = winner;
            return this;
        }
    }
}