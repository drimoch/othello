using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B19_Ex02_Othelo
{
    public class Cell
    {
        // Members
        private Location m_CellLocation;
        public Location CellLocation
        {
            get
            {
                return m_CellLocation;
            }
            set
            {
                m_CellLocation = value;
            }
        }

        public enum eType
        {
            Empty, // empty cell
            Player1, // X
            Player2 // O
        }

        private eType m_CellType;

        public eType CellType
        {
            get
            {
                return m_CellType;
            }
            set
            {
                m_CellType = value;
            }
        }

        public struct Location
        {
            private int x;
            private int y;

            public int X
            {
                get
                {
                    return x;
                }
                set
                {
                    x = value;
                }
            }
            public int Y
            {
                get
                {
                    return y;
                }
                set
                {
                    y = value;
                }
            }
        }

        // Methods
        public Cell(int i_X, int i_Y, eType i_Type = eType.Empty)
        { // constructor
            m_CellLocation = new Location();
            m_CellType = i_Type;
            m_CellLocation.X = i_X;
            m_CellLocation.Y = i_Y;
        }

        public bool IsCellEmpty()
        {
            bool isEmpty = (m_CellType == eType.Empty); // if empty - return true

            return isEmpty;
        }
    }
}
