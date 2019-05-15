using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class Board
    {
        enum BOARD_SIZE
        {
            SIZE6, // 6X6
            SIZE8 // 8X8
        }

        // Memebers
        private Cell[][] m_MatrixBoard; // matrix for game's board
        private BOARD_SIZE m_boardSize;

        // Methods
        public Board(int i_boardSize)
        {
            m_boardSize = (BOARD_SIZE)i_boardSize;
        } // constructor
        private void initializeBoard() { } // set board matrix at the begining of the game
        public void PrintBoard()
        { }
        private void cleanBoard() { }
        private void updateBoard() { }

    }
}
