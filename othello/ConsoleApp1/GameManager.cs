using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class GameManager
    {
        private int[] m_validBoardSizes = { 6, 8 };
        private ConsoleUI m_consoleUI = new ConsoleUI();
        public Board gameBoard;
        public void initiateGame(int[] i_InitInfo)
        {
            if (isBoardSizeInputValid(i_InitInfo[(int)GameInitInfo.boardSize]))
            { 
                gameBoard = new Board(i_InitInfo[(int)GameInitInfo.boardSize]);
                m_consoleUI.initBoard(ref gameBoard);
            }
            else
            {
               // m_consoleUI.sayToUserBoardSizeIsNotValid();
            }
            //init players with i_InitInfo[GameInitInfo.numOfPlayers]
            //  m_consoleUI.printBoard();
        }
        public bool isBoardSizeInputValid(int i_BoardSizeInput)
        {
            //im not sure if its valid by guy ronen's standard :/
            bool isValidSize = false;
            for (int i = 0; i < m_validBoardSizes.Length; i++)
            {
                if (i_BoardSizeInput == m_validBoardSizes[i])
                {
                    isValidSize = true;
                    return isValidSize;
                }
            }
            return isValidSize;
        }
        public void startGame()
        {
            int[] gameInitInfo = m_consoleUI.startGame();



        }
    }
}
