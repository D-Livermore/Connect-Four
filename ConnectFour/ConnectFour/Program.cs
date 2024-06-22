using System;

namespace ConnectFour
{
    public class PrintInstructions //Class to print instructions for players before starting the game. 
{
    public void Print()
    {
        Console.WriteLine("Welcome to Connect Four!");
        Console.WriteLine("Here are the rules:");
        Console.WriteLine("- The game is played on a grid that's 6 cells high by 7 cells wide just like original connect four game.");
        Console.WriteLine("- Player 1 is 'R' (red), and Player 2 is 'Y' (yellow). Players take turns putting their discs in any of the grid's columns.");
        Console.WriteLine("- The first player to get 4 of their discs in a row (up, down, across, or diagonally) is the winner.");
        Console.WriteLine("- If all cells are filled and no player has 4 in a row, the game is a draw.");
        Console.WriteLine("- To make a move, enter a number from 1 to 7 to choose the column when it is your turn.");
        Console.WriteLine("- Good luck and may the best player win!");
        Console.WriteLine();
    }
}
    // Enum representing the different states of a cell in the game board
    public enum Disc { Empty, Red, Yellow }

    // Class representing the game board
    public class GameBoard
    {
        private const int Rows = 6;
        private const int Columns = 7;
        private Disc[,] board;        // 2D array representing the game board
        private int[] columnCount;    // Array to keep track of the number of discs in each column

        // Constructor to initialize the game board
        public GameBoard()
        {
            board = new Disc[Rows, Columns];
            columnCount = new int[Columns];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    board[i, j] = Disc.Empty;
                }
            }
        }

        // Method to check if a column is valid for placing a disc
        public bool IsColumnValid(int column)
        {
            return column >= 0 && column < Columns && columnCount[column] < Rows;
        }

        // Method to place a disc in the specified column
        public bool PlaceDisc(int column, Disc disc)
        {
            if (!IsColumnValid(column)) return false;

            for (int row = Rows - 1; row >= 0; row--)
            {
                if (board[row, column] == Disc.Empty)
                {
                    board[row, column] = disc;
                    columnCount[column]++;
                    return true;
                }
            }
            return false;
        }

        // Method to check if the specified disc has won the game
        public bool CheckWin(Disc disc)
        {
            return CheckHorizontalWin(disc) || CheckVerticalWin(disc) || CheckDiagonalWin(disc);
        }

        // Method to check for a horizontal win
        private bool CheckHorizontalWin(Disc disc)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns - 3; col++)
                {
                    if (board[row, col] == disc && board[row, col + 1] == disc &&
                        board[row, col + 2] == disc && board[row, col + 3] == disc)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Method to check for a vertical win
        private bool CheckVerticalWin(Disc disc)
        {
            for (int col = 0; col < Columns; col++)
            {
                for (int row = 0; row < Rows - 3; row++)
                {
                    if (board[row, col] == disc && board[row + 1, col] == disc &&
                        board[row + 2, col] == disc && board[row + 3, col] == disc)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Method to check for a diagonal win
        private bool CheckDiagonalWin(Disc disc)
        {
            // Check for descending diagonal (\) win
            for (int row = 0; row < Rows - 3; row++)
            {
                for (int col = 0; col < Columns - 3; col++)
                {
                    if (board[row, col] == disc && board[row + 1, col + 1] == disc &&
                        board[row + 2, col + 2] == disc && board[row + 3, col + 3] == disc)
                    {
                        return true;
                    }
                }
            }

            // Check for ascending diagonal (/) win
            for (int row = 3; row < Rows; row++)
            {
                for (int col = 0; col < Columns - 3; col++)
                {
                    if (board[row, col] == disc && board[row - 1, col + 1] == disc &&
                        board[row - 2, col + 2] == disc && board[row - 3, col + 3] == disc)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Method to check if the game board is full
        public bool IsFull()
        {
            foreach (var count in columnCount)
            {
                if (count < Rows) return false;
            }
            return true;
        }

        // Method to print the game board
        public void PrintBoard()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    switch (board[row, col])
                    {
                        case Disc.Empty:
                            Console.Write(". ");
                            break;
                        case Disc.Red:
                            Console.Write("R ");
                            break;
                        case Disc.Yellow:
                            Console.Write("Y ");
                            break;
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }

    // Abstract class representing a player
    public abstract class Player
    {
        public string Name { get; set; }
        public Disc Disc { get; set; }

        protected Player(string name, Disc disc)
        {
            Name = name;
            Disc = disc;
        }

        // Abstract method to get the player's move
        public abstract int GetMove(GameBoard board);
    }

    // Class representing a human player
    public class HumanPlayer : Player
    {
        public HumanPlayer(string name, Disc disc) : base(name, disc) { }

        // Method to get the human player's move
        public override int GetMove(GameBoard board)
        {
            int column;
            bool validInput;
            do
            {
                Console.WriteLine($"{Name}, enter your move (1-7): ");
                validInput = int.TryParse(Console.ReadLine(), out column) && column >= 1 && column <= 7 && board.IsColumnValid(column - 1);
                if (!validInput)
                {
                    Console.WriteLine("Invalid move. Please try again.");
                }
            } while (!validInput);
            return column - 1;
        }
    }

    // Class representing the game
    public class Game
    {
        private GameBoard board;
        private Player player1;
        private Player player2;
        private Player currentPlayer;
        private PrintInstructions instructions;

        // Constructor to initialize the game
        public Game(Player player1, Player player2)
        {
            this.player1 = player1;
            this.player2 = player2;
            currentPlayer = player1;
            board = new GameBoard();
            instructions = new PrintInstructions();
        }

        // Method to play the game
        public void Play()
        {
            instructions.Print();
            bool gameWon = false;

            while (!gameWon)
            {
                board.PrintBoard();
                int column = currentPlayer.GetMove(board);
                board.PlaceDisc(column, currentPlayer.Disc);
                if (board.CheckWin(currentPlayer.Disc))
                {
                    board.PrintBoard();
                    Console.WriteLine($"{currentPlayer.Name} is the WINNER!");
                    gameWon = true;
                }
                else if (board.IsFull())
                {
                    board.PrintBoard();
                    Console.WriteLine("The game is a DRAW!");
                    gameWon = true;
                }
                else
                {
                    currentPlayer = currentPlayer == player1 ? player2 : player1;
                }
            }
        }
    }

    // Main program class
    class Program
    {
        static void Main(string[] args)
        {
            bool playAgain = true;
            while (playAgain)
            {
                Player player1 = new HumanPlayer("Player 1", Disc.Red);
                Player player2 = new HumanPlayer("Player 2", Disc.Yellow);

                Game game = new Game(player1, player2);
                game.Play();

                // Loop to handle replay prompt until a valid response is received
                bool validInput;
                do
                {
                    Console.WriteLine("Do you want to play again? (y/n): ");
                    string response = Console.ReadLine().ToLower();
                    if (response == "y")
                    {
                        playAgain = true;
                        validInput = true;
                    }
                    else if (response == "n")
                    {
                        playAgain = false;
                        validInput = true;
                    }
                    else
                    {
                        Console.WriteLine("Unknown command, why not try another game?");
                        validInput = false;
                    }
                } while (!validInput);
            }
        }
    }
}
