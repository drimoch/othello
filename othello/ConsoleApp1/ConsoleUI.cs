﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B19_Ex02_Othelo
{
    public class ConsoleUI
    {
        // Members
        private Board m_GameBoard = null; // will be sent by reference from GameManager
        private string m_FirstPlayerName;
        private string m_SecondPlayerName;
        private GameManager m_GameManager;
        // Methods
        public ConsoleUI()
        {
            m_GameManager = new GameManager(new Action(Exit), new Action<eResponseCode>(PrintErrorMessege), new GetUserInput(GetInputFromUser));
        }


        public void StartGame()
        {
            int[] gameProperties = PrintStartGameMenu(); //Return array [numOfPlayers, boardSize]

            m_GameBoard = m_GameManager.InitiateGame(gameProperties[0], gameProperties[1]);
            PrintTheBoardAfterTurn();
            playTurns();
        }
        private void playTurns()
        {
            while (m_GameManager.Winner == null)
            {
                eResponseCode availableCells = m_GameManager.CheckValidCellsForBothPlayers();
                if (availableCells == eResponseCode.NoValidCellsForBothPlayers)
                {
                    Player looser = m_GameManager.CalculateWinnerAndLooser();
                    PrintErrorMessege(availableCells);
                    GameOverMessege(m_GameManager.Winner, looser, m_GameManager.Player1.Score, m_GameManager.Player2.Score);
                    FinishGame();
                }
                else if (availableCells == eResponseCode.NoValidCellsForPlayer)
                {
                    PrintErrorMessege(availableCells);
                }
                else
                {
                    if (m_GameManager.CurrentPlayer.PlayerID == Player.ePlayerID.Computer)
                    {
                        m_GameManager.MakeAiMove();
                    }
                    else
                    {
                        string inputLocation = GetMoveFromUser(m_GameManager.CurrentPlayer.PlayerID);

                        Cell.Location location = m_GameManager.ParseLocation(inputLocation);
                        eResponseCode moveResponse = m_GameManager.ExecuteMove(location);
                        while (moveResponse != eResponseCode.OK)
                        {
                            PrintErrorMessege(moveResponse);
                            inputLocation = GetInputFromUser();

                            location = m_GameManager.ParseLocation(inputLocation);
                            moveResponse = m_GameManager.ExecuteMove(location);
                        }
                    }

                    PrintTheBoardAfterTurn();
                }

                m_GameManager.ChangeCurrentPlayer();
            }
        }

        public int[] PrintStartGameMenu()
        {
            int[] gameProperties = new int[2];

            string messege = string.Format("Welcome to Othelo! Please enter first player's name:");
            Console.WriteLine(messege);
            m_FirstPlayerName = Console.ReadLine();
            messege = string.Format("How do you want to play?{0}1. One player against the computer{1}2. 2 Players", Environment.NewLine, Environment.NewLine);
            Console.WriteLine(messege);
            string numOfPlayersStr = Console.ReadLine();
            while (!menuInputValidation(numOfPlayersStr, "1", "2"))
            {
                Console.WriteLine("Invalid input. Please type 1 or 2:");
                numOfPlayersStr = Console.ReadLine();
            }

            gameProperties[0] = int.Parse(numOfPlayersStr);
            if (gameProperties[0] == 2)
            {
                Console.WriteLine("Please enter second player's name:");
                m_SecondPlayerName = Console.ReadLine();
            }

            messege = string.Format("Please choose the board size:{0}1. 6X6{1}2. 8X8", Environment.NewLine, Environment.NewLine);
            Console.WriteLine(messege);
            string boardSize = Console.ReadLine();
            while (!menuInputValidation(boardSize, "1", "2"))
            {
                Console.WriteLine("Invalid input. Please type 1 or 2:");
                boardSize = Console.ReadLine();
            }

            gameProperties[1] = (boardSize == "1" ? 6 : 8);

            return gameProperties;
        }

        private bool menuInputValidation(string i_Input, string i_ValidInput1, string i_ValidInput2)
        {
            bool isValid = i_Input == i_ValidInput1 || i_Input == i_ValidInput2;

            return isValid;
        }

        public string GetMoveFromUser(Player.ePlayerID i_CurrentPlayer)
        {
            string playerName = getPlayerNameByPlayerID(i_CurrentPlayer);

            Console.WriteLine("{0}: Please choose a cell (e.g. A1)", playerName);
            string userInput = GetInputFromUser();

            return userInput;
        }

        private string getPlayerNameByPlayerID(Player.ePlayerID i_CurrentPlayer)
        {
            string playerName;

            switch (i_CurrentPlayer)
            {
                case Player.ePlayerID.Player1:
                    playerName = m_FirstPlayerName;
                    break;
                case Player.ePlayerID.Player2:
                    playerName = m_SecondPlayerName;
                    break;
                case Player.ePlayerID.Computer:
                    playerName = "the computer";
                    break;
                default:
                    playerName = m_FirstPlayerName;
                    break;
            }

            return playerName;
        }

        public void PrintErrorMessege(eResponseCode i_Error)
        {
            switch (i_Error)
            {
                case eResponseCode.CellIsInvalid:
                    Console.WriteLine("The chosen cell is invalid, please choose another cell:");
                    break;
                case eResponseCode.NoValidCellsForPlayer:
                    Console.WriteLine("You have no valid cells. The turn will be moved to the second player");
                    break;
                case eResponseCode.NoValidCellsForBothPlayers:
                    Console.WriteLine("Both players have no valid cells");
                    break;
                case eResponseCode.InvalidMove:
                    Console.WriteLine("The cell you chose doesn't block the competitor's coins, please try again:");
                    break;
                case eResponseCode.NotEmpty:
                    Console.WriteLine("The chosen cell is not empty, please try again:");
                    break;
                case eResponseCode.OutOfRange:
                    Console.WriteLine("The chosen cell is out of range, please try again:");
                    break;
                default:
                    break;
            }
        }

        public void PrintTheBoardAfterTurn()
        {
            cleanScreen();
            printBoard();
        }

        private void cleanScreen()
        {
            Console.Clear();
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

            switch (i_Type)
            {
                case Cell.eType.Player1:
                    type = 'O';
                    break;
                case Cell.eType.Player2:
                    type = 'X';
                    break;
                default:
                    type = ' ';
                    break;
            }

            return type;
        }

        public void GameOverMessege(Player i_Winner, Player i_Looser, int i_Score1, int i_Score2)
        {
            string winner = getPlayerNameByPlayerID(i_Winner.PlayerID);
            string looser = getPlayerNameByPlayerID(i_Looser.PlayerID);
            string endGameMsg = string.Format
                (@"Game Over! The winner is {0}!
                 Scores Summary:
                 {1} - {2}
                 {3} - {4}", winner, winner, i_Winner.Score, looser, i_Looser.Score);

            Console.WriteLine(endGameMsg);
        }

        public void FinishGame()
        {

            if (StartNewGame())
            {
               m_GameManager.InitiateGame(m_GameManager.NumOfPlayers, m_GameBoard.Matrix.GetLength(0));
            }
            else
            {
                Exit();
            }
        }

        public bool StartNewGame()
        {
            bool startNewGame;
            string answer;

            Console.WriteLine("Would you like to start a new game? yes / no");
            answer = Console.ReadLine();
            while (!menuInputValidation(answer, "yes", "no"))
            {
                Console.WriteLine("Invalid answer, please type: yes / no");
                answer = Console.ReadLine();
            }

            startNewGame = answer == "yes";

            return startNewGame;
        }

        public void GoodbyeMessege()
        {
            Console.WriteLine("It was a pleasure! Goodbye!");
        }
        private void Exit()
        {
            GoodbyeMessege();
            System.Environment.Exit(1);
        }
    }
}
