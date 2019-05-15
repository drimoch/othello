using System;
using System.Collections.Generic;
using System.Text;

namespace B19_Ex02_Othelo
{
    class GameManager
    {
        // Members
        private int[] m_ValidBoardSizes = { 6, 8 };
        private ConsoleUI m_ConsoleUI;
        private Board m_GameBoard;
        private int m_NumOfPlayers;
        private Player m_Player1;
        private Player m_Player2;
        private Player m_Winner;
        private Player m_CurrentPlayer;
        public enum eResponseCode
        {
            CellIsInvalid,
            NoValidCellsForPlayer,
            NoValidCellsForBothPlayers,
            OutOfRange,
            WrongInput,
            OK // no error
        }

        // Methods
        public GameManager()
        {
            m_ConsoleUI = new ConsoleUI();
        }

        public void StartGame()
        {
            int[] gameInitInfo = m_ConsoleUI.StartGame();
            initiateGame(gameInitInfo[0], gameInitInfo[1]);
        }

        private void initiateGame(int i_NumOfPlayers, int i_BoardSize)
        {
            m_GameBoard = new Board(i_BoardSize);
            m_Winner = null;
            m_ConsoleUI.InitBoard(ref m_GameBoard);
            m_NumOfPlayers = i_NumOfPlayers;
            initPlayers();
            m_GameBoard.InitializeBoard();
            printBoard();
            playTurns();
        }

        private void printBoard()
        {
            m_ConsoleUI.PrintTheBoardAfterTurn();
        }

        private void playTurns()
        {
            while (m_Winner == null)
            {
                eResponseCode availableCells = checkAvailableValidCells();
                if (availableCells == eResponseCode.NoValidCellsForBothPlayers)
                {
                    calculateWinner();
                    m_ConsoleUI.PrintErrorMessege(availableCells);
                    m_ConsoleUI.GameOverMessege(m_Winner.PlayerID, m_Player1.Score, m_Player2.Score);
                    finishGame();
                }

                if (availableCells == eResponseCode.NoValidCellsForPlayer)
                {
                    m_ConsoleUI.PrintErrorMessege(availableCells);
                    changeCurrentPlayer();
                }

                string inputLocation = m_ConsoleUI.GetMoveFromUser(m_CurrentPlayer.PlayerID);
                parseLocation(inputLocation);
                m_ConsoleUI.PrintTheBoardAfterTurn();
                changeCurrentPlayer();
            }
        }

        private void changeCurrentPlayer()
        {
            if (m_NumOfPlayers == 2)
            {
                if (m_CurrentPlayer.PlayerID == Player.ePlayerID.Player1)
                {
                    m_CurrentPlayer = m_Player2;
                }
                else
                {
                    m_CurrentPlayer = m_Player1;
                }
            }
        }

        private eResponseCode checkAvailableValidCells()
        {// TODO
            return eResponseCode.OK;
        }

        private void calculateWinner()
        {
            //TODO
        }

        private void initPlayers()
        {
            m_Player1 = new Player(Player.ePlayerID.Player1);
            m_CurrentPlayer = m_Player1;
            if (m_NumOfPlayers == 2)
            {
                m_Player2 = new Player(Player.ePlayerID.Player2);
            }
            else
            {
                m_Player2 = new Player(Player.ePlayerID.Computer);
            }
        }

        private void parseLocation(string i_InputLocation)
        {
            Cell.Location chosenLocation = new Cell.Location();
            if (i_InputLocation == "Q") // Quit game
            {
                Exit();
            }

            eResponseCode response = locationInputValidation(i_InputLocation);
            while (response != eResponseCode.OK)
            {
                m_ConsoleUI.PrintErrorMessege(response);
                i_InputLocation = m_ConsoleUI.GetInputFromUser();
                response = locationInputValidation(i_InputLocation);
            }

            chosenLocation.X = i_InputLocation[0] - 64; // convert letter to number
            chosenLocation.Y = int.Parse(i_InputLocation[1].ToString());
        }

        private eResponseCode locationInputValidation(string i_InputLocation)
        {
            eResponseCode sendResponse = eResponseCode.OK;
            int boardSize = m_GameBoard.Matrix.GetLength(0);
            if (i_InputLocation.Length != 2)
            {
                sendResponse = eResponseCode.CellIsInvalid;
            }
            else if (i_InputLocation[0] < 'A' || i_InputLocation[0] > ('A' + boardSize) || i_InputLocation[1] < '1' || i_InputLocation[1] > '1' + boardSize)
            {
                sendResponse = eResponseCode.CellIsInvalid;
            }

            return sendResponse;
        }

        private void finishGame()
        {
            if (m_ConsoleUI.StartNewGame())
            {
                initiateGame(m_NumOfPlayers, m_GameBoard.Matrix.GetLength(0));
            }
            else
            {
                Exit();
            }
        }

        private void Exit()
        {
            m_ConsoleUI.GoodbyeMessege();
            System.Environment.Exit(1);
        }
    }
}
