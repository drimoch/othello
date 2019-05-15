using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class Cell
    {
        struct Location
        {
            int x;
            int y;
        }
        enum Type
        {
            EMPTY = 0, // empty cell
            BLACK = 1, // X
            WHITE = 2 // O
        }

        // Members
        private Location m_CellLocation;
        private Type m_CellType;

        // Methods
        private bool isCellEmpty()
        {
            bool isEmpty = (m_CellType == Type.EMPTY); // if empty return true
            return isEmpty;
        }

        // Also add getters and setters
    }
}
