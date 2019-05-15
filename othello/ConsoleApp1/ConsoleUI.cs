using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class ConsoleUI
    {
        Board m_GameBoard;
        // Methods
        private void printMenu() { }
        private void sendDataToManager() { }
        public int[] startGame()
        {
            //add print menu
            //return array [numOfPlayers,boardSize(enum value)]
            return new int[] { 2, 2 };
        } // prints start menu 
        public void initBoard(ref Board i_board )
        {
            m_GameBoard = i_board;
        }
        private void finishGame() { }
        private bool inputValidation() { return true; }
    }
}
