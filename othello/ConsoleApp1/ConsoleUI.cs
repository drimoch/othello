using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B19_Ex02_Othelo
{
    class ConsoleUI
    {
        // Members
        private Board m_GameBoard = null; // will be sent by reference from GameManager

        // Methods
        public void InitBoard(ref Board i_Board)
        {
            m_GameBoard = i_Board;
        }

        public int[] StartGame()
        {
            int[] gameProperties = PrintStartGameMenu(); //Return array [numOfPlayers, boardSize]
            return gameProperties;
        }

        public int[] PrintStartGameMenu()
        {
            int[] gameProperties = new int[2];
            string newLine = Environment.NewLine;
            string messege = string.Format("Welcome to Othelo!{0}" +
                                           "Please enter first player's name:",
                                           newLine);
            Console.WriteLine(messege);
            string firstPlayerName = Console.ReadLine();
            messege = string.Format("How do you want to play?{0}" +
                                     "1. One player against the computer{1}" +
                                     "2. 2 Players",
                                     newLine, newLine);
            Console.WriteLine(messege);
            string numOfPlayersStr = Console.ReadLine();
            while (!menuInputValidation(numOfPlayersStr))
            {
                Console.WriteLine("Invalid input. Please type 1 or 2: ");
                numOfPlayersStr = Console.ReadLine();
            }

            gameProperties[0] = int.Parse(numOfPlayersStr);
            if (gameProperties[0] == 2)
            {
                Console.WriteLine("Please enter the second player's name: ");
                string secondPlayerName = Console.ReadLine();
            }

            Console.WriteLine("Please choose the board size: {0}1. 6X6{1}2. 8X8", newLine, newLine);
            string boardSize = Console.ReadLine();
            while (!menuInputValidation(boardSize))
            {
                Console.WriteLine("Invalid input. Please type 1 or 2");
                boardSize = Console.ReadLine();
            }

            gameProperties[1] = boardSize == "1" ? 6 : 8;

            return gameProperties;
        }

        private bool menuInputValidation(string i_Input)
        {
            bool isValid;
            if (i_Input == "1" || i_Input == "2")
            {
                isValid = true;
            }
            else
            {
                isValid = false;
            }

            return isValid;
        }

        public string GetMoveFromUser(Player.ePlayerID i_CurrentPlayer)
        {
            Console.WriteLine("{0}: Please choose a cell (e.g A1)", i_CurrentPlayer);
            string userInput = GetInputFromUser();

            return userInput;
        }

        public Cell.Location PrintErrorMessege(GameManager.eResponseCode i_Error)
        {
            Cell.Location location = new Cell.Location();
            if (i_Error == GameManager.eResponseCode.CellIsInvalid)
            {
                Console.WriteLine("The chosen cell is invalid, please choose another cell:");
            }

            if (i_Error == GameManager.eResponseCode.NoValidCellsForPlayer)
            {
                Console.WriteLine("You have no valid cells. The turn will be moved to the second player");
            }

            if (i_Error == GameManager.eResponseCode.NoValidCellsForBothPlayers)
            {
                Console.WriteLine("Both players have no valid cells");
            }

            if(i_Error == GameManager.eResponseCode.InvalidMove)
            {
                Console.WriteLine("The cell you chose doesn't block the competitor's coins, please try again:");
            }

            if(i_Error == GameManager.eResponseCode.NotEmpty)
            {
                Console.WriteLine("The chosen cell is not empty, please try again:");
            }

            return location;
        }

        public void PrintTheBoardAfterTurn()
        {
            cleanScreen();
            printBoard();
        }

        private void cleanScreen()
        {
            Ex02.ConsoleUtils.Screen.Clear();
        }

        public string GetInputFromUser()
        {
            string userInput = Console.ReadLine();

            return userInput;
        }

        private void printBoard()
        {
            Cell[,] gameBoard = m_GameBoard.Matrix;
            int size = gameBoard.GetLength(0);
            StringBuilder matrix = new StringBuilder("    A ", size * size);
            for (char i = 'B'; i < 'A' + size; i++) // get columns letters
            {
                matrix.AppendFormat("  {0} ", i);
            }

            matrix.Append(Environment.NewLine);
            matrix.Append("  ");
            for (int i = 1; i < size; i++)
            {
                matrix.Append("=====");
            }

            for (int i = 0; i < size; i++) // print rows
            {
                matrix.Append(Environment.NewLine);
                matrix.AppendFormat("{0} |", i + 1);
                for (int j = 0; j < size; j++) // print each row
                {
                    char type = covertCellTypeToChar(gameBoard[i, j].CellType);
                    matrix.AppendFormat(" {0} |", type);
                }
                matrix.Append(Environment.NewLine);
                matrix.Append("  ");
                for (int j = 1; j < size; j++)
                {
                    matrix.Append("=====");
                }
            }

            Console.WriteLine(matrix);
        }

        private char covertCellTypeToChar(Cell.eType i_Type)
        {
            char type;
            if (i_Type == Cell.eType.Player1)
            {
                type = 'O';
            }
            else if (i_Type == Cell.eType.Player2)
            {
                type = 'X';
            }
            else
            {
                type = ' ';
            }

            return type;
        }

        public void GameOverMessege(Player.ePlayerID i_Winner, int i_Score1, int i_Score2)
        {
            string winner = (i_Winner == Player.ePlayerID.Player1 ? "player 1" : "player 2");
            if (i_Winner == Player.ePlayerID.Computer)
            {
                winner = "the computer";
            }

            string newLine = Environment.NewLine;
            string endGameMsg = string.Format("Game Over! The winner is {0}!" +
                "{1}Scores Summary:" +
                "{2}Player1 - {3}" +
                "{4}Player2 - {5}", winner, newLine, newLine, i_Score1, newLine, i_Score2);
            Console.WriteLine(endGameMsg);
        }

        public bool StartNewGame()
        {
            Console.WriteLine("Would you like to start a new game? yes / no");
            string answer = Console.ReadLine();
            bool startNewGame;
            while (!inputValidationStartNewGame(answer))
            {
                Console.WriteLine("Invalid answer, please type: yes / no");
                answer = Console.ReadLine();
            }

            startNewGame = answer == "yes";

            return startNewGame;
        }

        private bool inputValidationStartNewGame(string i_Input)
        {
            bool isValid;
            if (i_Input == "yes" || i_Input == "no")
            {
                isValid = true;
            }
            else
            {
                isValid = false;
            }

            return isValid;
        }

        public void GoodbyeMessege()
        {
            Console.WriteLine("It was a pleasure! Goodbye!");
        }
    }
}
