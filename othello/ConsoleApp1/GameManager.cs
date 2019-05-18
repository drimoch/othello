using System;
using System.Collections.Generic;
using System.Text;

namespace B19_Ex02_Othelo
{
    class GameManager
    {
        // Members
        private readonly int[] m_ValidBoardSizes = { 6, 8 };
        private readonly ConsoleUI m_ConsoleUI;
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
                eResponseCode availableCells = checkValidCellsForBothPlayers();
                if (availableCells == eResponseCode.NoValidCellsForBothPlayers)
                {
                    calculateWinner();
                    m_ConsoleUI.PrintErrorMessege(availableCells);
                    m_ConsoleUI.GameOverMessege(m_Winner.PlayerID, m_Player1.Score, m_Player2.Score);
                    finishGame();
                }
                else if (availableCells == eResponseCode.NoValidCellsForPlayer)
                {
                    m_ConsoleUI.PrintErrorMessege(availableCells);
                    changeCurrentPlayer();
                }
                else
                {
                    if (m_CurrentPlayer.PlayerID == Player.ePlayerID.Computer)
                    {
                        makeAiMove();
                    }
                    else
                    {
                        string inputLocation = m_ConsoleUI.GetMoveFromUser(m_CurrentPlayer.PlayerID);
                        Cell.Location location = parseLocation(inputLocation);
                        eResponseCode moveResponse = executeMove(location);
                        while (moveResponse != eResponseCode.OK)
                        {
                            m_ConsoleUI.PrintErrorMessege(moveResponse);
                            inputLocation = m_ConsoleUI.GetInputFromUser();
                            location = parseLocation(inputLocation);
                            moveResponse = executeMove(location);
                        }
                    }

                    m_ConsoleUI.PrintTheBoardAfterTurn();
                    changeCurrentPlayer();
                }
            }
        }

        private void makeComputerMove()
        {
            Random random = new Random();
            List<Cell> computerCells = getComputerValidCells();
            int randomIndex = random.Next(computerCells.Count);
            executeMove(computerCells[randomIndex].CellLocation);
        }


        //This function calculates all possible moves
        //and choose the move that will gain the player most points
        private void makeAiMove()
        {
            int bestScore = int.MinValue;
            List<Cell> computerCells = getComputerValidCells();
            Cell bestMove = computerCells[0];
            foreach (Cell move in computerCells)
            {
                Board childBoard =m_GameBoard;
                int nodeScore = calculateMoveOnChildBoard(move.CellLocation,childBoard);
                if (nodeScore > bestScore)
                {
                    bestScore = nodeScore;
                    bestMove = move;
                }
            }
            executeMove(bestMove.CellLocation);
        }

        private List<Cell> getComputerValidCells()
        {
            List<Cell> computerValidCells = new List<Cell>();
            List<Cell> tempList = new List<Cell>();
            foreach (Cell cell in m_GameBoard.Matrix)
            {
                if (cell.CellType == Cell.eType.Empty)
                {
                    tempList = findCellsToFlip(cell.CellLocation, Cell.eType.Player2);
                    if(tempList.Count > 0)
                    {
                        computerValidCells.Add(cell);
                    }
                }
            }

            return computerValidCells;
        }

        private eResponseCode executeMove(Cell.Location i_Location)
        {
            eResponseCode moveResult;
            if (m_GameBoard.Matrix[i_Location.X, i_Location.Y].CellType == Cell.eType.Empty)
            {
                Cell.eType cellType = m_CurrentPlayer.PlayerID == Player.ePlayerID.Player1 ? Cell.eType.Player1 : Cell.eType.Player2; // player2 can be also the computer
                List<Cell> cellsToFlip = findCellsToFlip(i_Location, cellType);
                if(cellsToFlip.Count == 0)
                {
                    moveResult = eResponseCode.InvalidMove;
                }
                else
                {
                    flipCells(cellsToFlip, cellType);
                    updateScore(m_Player1);
                    updateScore(m_Player2);
                    moveResult = eResponseCode.OK;
                }
            }
            else
            {
                moveResult = eResponseCode.NotEmpty;
            }

            return moveResult;
        }

        private int calculateMoveOnChildBoard(Cell.Location i_Location,Board i_ChildBoard)
        {
            if (i_ChildBoard.Matrix[i_Location.X, i_Location.Y].CellType == Cell.eType.Empty)
            {
                Cell.eType cellType = m_CurrentPlayer.PlayerID == Player.ePlayerID.Player1 ? Cell.eType.Player1 : Cell.eType.Player2; // player2 can be also the computer
                List<Cell> cellsToFlip = findCellsToFlip(i_Location, cellType,i_ChildBoard);
                return cellsToFlip.Count;

            }
            else
            {
                return 0;
            }

        }


        private void updateScore(Player i_Player)
        {
            int counter = 0;
            Cell.eType playerCells = i_Player.PlayerID == Player.ePlayerID.Player1 ? Cell.eType.Player1 : Cell.eType.Player2;
            foreach(Cell cell in m_GameBoard.Matrix)
            {
                if(cell.CellType == playerCells)
                {
                    counter++;
                }
            }

            i_Player.Score = counter;
        }

  
        private void calculateWinner()
        {
            m_Winner = m_Player1.Score > m_Player2.Score ? m_Player1 : m_Player2;
        }

        private void changeCurrentPlayer()
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

        private List<Cell> findCellsToFlip(Cell.Location i_Cell, Cell.eType i_CellType)
        {
            List<Cell> cellsToFlip = new List<Cell>();
            Cell.eType otherCellType = (i_CellType == Cell.eType.Player1 ? Cell.eType.Player2 : Cell.eType.Player1);
                int[,] directionArr = new int[8, 2] { {0,1}, {1, 1}, {1, 0}, {1, -1}, {0, -1}, {-1, -1}, {-1, 0}, {-1, 1} };
            for (int i = 0; i < directionArr.GetLength(0); i++)
            {
                int x = i_Cell.X;
                int y = i_Cell.Y;
                int xDirection = directionArr[i, 0];
                int yDirection = directionArr[i, 1];
                x += xDirection;
                y += yDirection;
                if (isOnMatrix(x, y) && m_GameBoard.Matrix[x, y].CellType == otherCellType)
                {
                    x += xDirection;
                    y += yDirection;
                    if (!isOnMatrix(x, y))
                    {
                        continue;
                    }

                    while (m_GameBoard.Matrix[x, y].CellType == otherCellType)
                    {
                        x += xDirection;
                        y += yDirection;
                        if (!isOnMatrix(x, y))
                        {
                            break;
                        }
                    }

                    if (!isOnMatrix(x, y))
                    {
                        continue;
                    }

                    if (m_GameBoard.Matrix[x, y].CellType == i_CellType)
                    { // there are cells to flip - go in reverse to find them
                        while (x != i_Cell.X || y != i_Cell.Y)
                        {
                            x -= xDirection;
                            y -= yDirection;
                            addToList<Cell>(ref cellsToFlip, m_GameBoard.Matrix[x, y]);
                        }
                    }

                }
            }

            return cellsToFlip;
        }

        private List<Cell> findCellsToFlip(Cell.Location i_Cell, Cell.eType i_CellType,Board i_ChildBoard)
        {
            List<Cell> cellsToFlip = new List<Cell>();
            Cell.eType otherCellType = (i_CellType == Cell.eType.Player1 ? Cell.eType.Player2 : Cell.eType.Player1);
            int[,] directionArr = new int[8, 2] { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 }, { -1, 1 } };
            for (int i = 0; i < directionArr.GetLength(0); i++)
            {
                int x = i_Cell.X;
                int y = i_Cell.Y;
                int xDirection = directionArr[i, 0];
                int yDirection = directionArr[i, 1];
                x += xDirection;
                y += yDirection;
                if (isOnMatrix(x, y) && i_ChildBoard.Matrix[x, y].CellType == otherCellType)
                {
                    x += xDirection;
                    y += yDirection;
                    if (!isOnMatrix(x, y))
                    {
                        continue;
                    }

                    while (i_ChildBoard.Matrix[x, y].CellType == otherCellType)
                    {
                        x += xDirection;
                        y += yDirection;
                        if (!isOnMatrix(x, y))
                        {
                            break;
                        }
                    }

                    if (!isOnMatrix(x, y))
                    {
                        continue;
                    }

                    if (i_ChildBoard.Matrix[x, y].CellType == i_CellType)
                    { // there are cells to flip - go in reverse to find them
                        while (x != i_Cell.X || y != i_Cell.Y)
                        {
                            x -= xDirection;
                            y -= yDirection;
                            addToList<Cell>(ref cellsToFlip, i_ChildBoard.Matrix[x, y]);
                        }
                    }

                }
            }

            return cellsToFlip;
        }

        private void addToList<T>(ref List<T> i_List, T i_Item)
        {
            i_List.Add(i_Item);
        }

        private void flipCells(List<Cell> i_ListOfCells, Cell.eType i_NewType)
        {
            foreach(Cell cell in i_ListOfCells)
            {
                cell.CellType = i_NewType;
            }
        }


        private bool isOnMatrix(int i_X, int i_Y)
        {
            bool isOnMatrix;
            if(i_X >= 0 && i_X < m_GameBoard.Matrix.GetLength(0) && i_Y >= 0 && i_Y < m_GameBoard.Matrix.GetLength(1))
            {
                isOnMatrix = true;
            }
            else
            {
                isOnMatrix = false;
            }

            return isOnMatrix;
        }

        private eResponseCode checkValidCellsForPlayer(Cell.eType i_Type)
        {
            List<Cell> availableCells = new List<Cell>();
            eResponseCode response = eResponseCode.NoValidCellsForPlayer;
            foreach(Cell cell in m_GameBoard.Matrix)
            {
                if(cell.CellType == Cell.eType.Empty)
                {
                    availableCells = findCellsToFlip(cell.CellLocation, i_Type);
                    if(availableCells.Count > 0)
                    {
                        response = eResponseCode.OK;
                        break;
                    }
                }
            }

            return response;
        }

        private eResponseCode checkValidCellsForBothPlayers()
        {
            eResponseCode response;
            Cell.eType currentType = m_CurrentPlayer.PlayerID == Player.ePlayerID.Player1 ? Cell.eType.Player1 : Cell.eType.Player2;
            if(checkValidCellsForPlayer(Cell.eType.Player1) == eResponseCode.NoValidCellsForPlayer && checkValidCellsForPlayer(Cell.eType.Player2) == eResponseCode.NoValidCellsForPlayer)
            {
                response = eResponseCode.NoValidCellsForBothPlayers;
            }
            else if(checkValidCellsForPlayer(currentType) == eResponseCode.NoValidCellsForPlayer)
            {
                response = eResponseCode.NoValidCellsForPlayer;
            }
            else
            {
                response = eResponseCode.OK;
            }

            return response;
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
