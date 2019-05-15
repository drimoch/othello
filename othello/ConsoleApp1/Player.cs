using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B19_Ex02_Othelo
{
    public class Player
    {
        public enum ePlayerID
        {
            Computer = 0,
            Player1 = 1,
            Player2 = 2           
        }

        // Members
        private ePlayerID m_playerID;
        private int m_Score;

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
        { // constructor
            m_playerID = i_PlayerID;
            m_Score = 2; // 2 coins for each player when game starts
        }
    }
}
