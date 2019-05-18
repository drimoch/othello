using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B19_Ex02_Othelo
{
    public class Player
    {
        // Members
        private ePlayerID m_playerID;
        private int m_Score;

        public enum ePlayerID
        {
            Computer,
            Player1,
            Player2           
        }

        public int Score
        {
            get
            {
                return m_Score;
            }

            set
            {
                m_Score = value;
            }
        }

        public ePlayerID PlayerID
        {
            get
            {
                return m_playerID;
            }

            set
            {
                m_playerID = value;
            }
        }

        // Methods
        public Player(ePlayerID i_PlayerID)
        {
            m_playerID = i_PlayerID;
            m_Score = 2; // 2 coins for each player when game starts
        }
    }
}
