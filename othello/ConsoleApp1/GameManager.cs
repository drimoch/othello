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
            WrongInput,
            InvalidMove,
            NotEmpty,
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
                Cell.Location location = parseLocation(inputLocation);
                eResponseCode moveResponse = calculateMove(location);
                while(moveResponse == eResponseCode.InvalidMove)
                {
                    m_ConsoleUI.PrintErrorMessege(eResponseCode.InvalidMove);
                    inputLocation = m_ConsoleUI.GetInputFromUser();
                    location = parseLocation(inputLocation);
                    moveResponse = calculateMove(location);
                }

                m_ConsoleUI.PrintTheBoardAfterTurn();
                changeCurrentPlayer();
            }
        }

        private eResponseCode calculateMove(Cell.Location i_Location)
        {
            eResponseCode moveResult = eResponseCode.OK;
            if (m_GameBoard.Matrix[i_Location.X, i_Location.Y].CellType == Cell.eType.Empty)
            {
                if (!calculateMovesDown(i_Location))
                {
                    if (!calculateMovesUp(i_Location))
                    {
                        if (!calculateMovesRight(i_Location))
                        {
                            if (!calculateMovesLeft(i_Location))
                            {
                                moveResult = eResponseCode.InvalidMove;
                            }
                        }
                    }
                }
            }
            else
            {
                moveResult = eResponseCode.NotEmpty;
            }

            return moveResult;
        }

        private int executeMove(Cell.Location i_Location, int i_EndRow, int i_EndCol, Cell.eType i_NewType)
        {
            int numOfChangedCells = 0;
            if (i_Location.X < i_EndRow)
            {
                for (int i = i_Location.X; i < i_EndRow; i++)
                {
                    m_GameBoard.Matrix[i, i_Location.Y].CellType = i_NewType;
                    numOfChangedCells++;
                }
            }
            else if (i_Location.Y < i_EndCol)
            {
                for (int i = i_Location.Y; i < i_EndCol; i++)
                {
                    m_GameBoard.Matrix[i_Location.X, i].CellType = i_NewType;
                    numOfChangedCells++;
                }
            }
            else if (i_Location.Y > i_EndCol)
            {
                for (int i = i_Location.Y; i > i_EndCol; i--)
                {
                    m_GameBoard.Matrix[i_Location.X, i].CellType = i_NewType;
                    numOfChangedCells++;
                }
            }
            else if (i_Location.X > i_EndRow)
            {
                for (int i = i_Location.X; i > i_EndRow; i--)
                {
                    m_GameBoard.Matrix[i, i_Location.Y].CellType = i_NewType;
                    numOfChangedCells++;
                }
            }

            return numOfChangedCells;
        }

        private void calculateScore(int i_NumOfCellsEarned)
        {
            m_CurrentPlayer.Score += i_NumOfCellsEarned;
        }

        private bool calculateMovesRight(Cell.Location i_Location)
        {
            int row = i_Location.X;
            int column = i_Location.Y;
            bool isValid = false;
            if (column + 1 < m_GameBoard.Matrix.GetLength(0))
            {
                if (m_GameBoard.Matrix[row, column + 1].CellType.Equals(Cell.eType.Player1)
                    && (m_CurrentPlayer.PlayerID == Player.ePlayerID.Player2))
                {
                    for (int i = column + 2; i < m_GameBoard.Matrix.GetLength(0); i++)
                    {
                        if (m_GameBoard.Matrix[row, i].CellType == Cell.eType.Empty)
                        {
                            isValid = false;
                            break;
                        }
                        if (m_GameBoard.Matrix[row, i].CellType == Cell.eType.Player2)
                        {
                            isValid = true;
                            int coins = executeMove(i_Location, row, i, Cell.eType.Player2);
                            calculateScore(coins);
                            break;
                        }
                    }
                }
                else if (m_GameBoard.Matrix[row, column + 1].CellType.Equals(Cell.eType.Player2)
                    && (m_CurrentPlayer.PlayerID == Player.ePlayerID.Player1))
                {
                    for (int i = column + 2; i < m_GameBoard.Matrix.GetLength(0); i++)
                    {
                        if (m_GameBoard.Matrix[row, i].CellType == Cell.eType.Empty)
                        {
                            isValid = false;
                            break;
                        }
                        if (m_GameBoard.Matrix[row, i].CellType == Cell.eType.Player1)
                        {
                            int coins = executeMove(i_Location, row, i, Cell.eType.Player1);
                            calculateScore(coins);
                            isValid = true;
                            break;
                        }
                    }
                }
            }

            return isValid;
        }

        private bool calculateMovesLeft(Cell.Location i_Location)
        {
            int row = i_Location.X;
            int column = i_Location.Y;
            bool isValid = false;
            if (column - 1 >= 0)
            {
                if (m_GameBoard.Matrix[row, column - 1].CellType.Equals(Cell.eType.Player1)
                    && (m_CurrentPlayer.PlayerID == Player.ePlayerID.Player2))
                {
                    for (int i = column - 2; i >= 0; i--)
                    {
                        if (m_GameBoard.Matrix[row, i].CellType == Cell.eType.Empty)
                        {
                            isValid = false;
                            break;
                        }
                        if (m_GameBoard.Matrix[row, i].CellType == Cell.eType.Player2)
                        {
                            int coins = executeMove(i_Location, row, i, Cell.eType.Player2);
                            calculateScore(coins);
                            isValid = true;
                            break;
                        }
                    }
                }
                else if (m_GameBoard.Matrix[row, column - 1].CellType.Equals(Cell.eType.Player2)
                    && (m_CurrentPlayer.PlayerID == Player.ePlayerID.Player1))
                {
                    for (int i = column - 2; i >= 0; i--)
                    {
                        if (m_GameBoard.Matrix[row, i].CellType == Cell.eType.Empty)
                        {
                            isValid = false;
                            break;
                        }
                        if (m_GameBoard.Matrix[row, i].CellType == Cell.eType.Player1)
                        {
                            int coins = executeMove(i_Location, row, i, Cell.eType.Player1);
                            calculateScore(coins);
                            isValid = true;
                            break;
                        }
                    }
                }
            }

            return isValid;
        }

        private bool calculateMovesUp(Cell.Location i_Location)
        {
            int row = i_Location.X;
            int column = i_Location.Y;
            bool isValid = false;
            if (row + 1 < m_GameBoard.Matrix.GetLength(0))
            {
                if (m_GameBoard.Matrix[row + 1, column].CellType.Equals(Cell.eType.Player1)
                    && (m_CurrentPlayer.PlayerID == Player.ePlayerID.Player2))
                {
                    for (int i = row + 2; i < m_GameBoard.Matrix.GetLength(0); i++)
                    {
                        if (m_GameBoard.Matrix[i, column].CellType == Cell.eType.Empty)
                        {
                            isValid = false;
                            break;
                        }
                        if (m_GameBoard.Matrix[i, column].CellType == Cell.eType.Player2)
                        {
                            int coins = executeMove(i_Location, i, column, Cell.eType.Player2);
                            calculateScore(coins);
                            isValid = true;
                            break;
                        }
                    }
                }
                else if (m_GameBoard.Matrix[row + 1, column].CellType.Equals(Cell.eType.Player2)
                    && (m_CurrentPlayer.PlayerID == Player.ePlayerID.Player1))
                {
                    for (int i = row + 2; i < m_GameBoard.Matrix.GetLength(0); i++)
                    {
                        if (m_GameBoard.Matrix[i, column].CellType == Cell.eType.Empty)
                        {
                            isValid = false;
                            break;
                        }
                        if (m_GameBoard.Matrix[i, column].CellType == Cell.eType.Player1)
                        {
                            int coins = executeMove(i_Location, i, column, Cell.eType.Player1);
                            calculateScore(coins);
                            isValid = true;
                            break;
                        }
                    }
                }
            }

            return isValid;
        }

        private bool calculateMovesDown(Cell.Location i_Location)
        {
            int row = i_Location.X;
            int column = i_Location.Y;
            bool isValid = false;
            if (row - 1 >= 0)
            {
                if (m_GameBoard.Matrix[row - 1, column].CellType.Equals(Cell.eType.Player1)
                    && (m_CurrentPlayer.PlayerID == Player.ePlayerID.Player2))
                {
                    for (int i = row - 2; i >= 0; i--)
                    {
                        if (m_GameBoard.Matrix[i, column].CellType == Cell.eType.Empty)
                        {
                            isValid = false;
                            break;
                        }
                        if (m_GameBoard.Matrix[i, column].CellType == Cell.eType.Player2)
                        {
                            int coins = executeMove(i_Location, i, column, Cell.eType.Player2);
                            calculateScore(coins);
                            isValid = true;
                            break;
                        }
                    }
                }
                else if (m_GameBoard.Matrix[row - 1, column].CellType.Equals(Cell.eType.Player2)
                    && (m_CurrentPlayer.PlayerID == Player.ePlayerID.Player1))
                {
                    for (int i = row - 2; i >= 0; i--)
                    {
                        if (m_GameBoard.Matrix[i, column].CellType == Cell.eType.Empty)
                        {
                            isValid = false;
                            break;
                        }
                        if (m_GameBoard.Matrix[i, column].CellType == Cell.eType.Player1)
                        {
                            int coins = executeMove(i_Location, i, column, Cell.eType.Player1);
                            calculateScore(coins);
                            isValid = true;
                            break;
                        }
                    }
                }
            }

            return isValid;
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
        {
            // TODO
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

        private Cell.Location parseLocation(string i_InputLocation)
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

            chosenLocation.Y = i_InputLocation[0] - 65; // convert letter to number
            chosenLocation.X = int.Parse((i_InputLocation[1]).ToString()) - 1;

            return chosenLocation;
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
