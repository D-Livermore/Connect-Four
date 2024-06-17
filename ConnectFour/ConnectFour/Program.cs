using System;

namespace ConnectFour
{
    // Enum for the disc types, empty means no disc
    public enum Disc { Empty, Red, Yellow }

    public class GameBoard
    {
        private const int Rows = 6; // Number of rows in the board
        private const int Columns = 7; // Number of columns in the board
        private Disc[,] board; // 2D array to hold the discs

        public GameBoard()
        {
            board = new Disc[Rows, Columns]; // Initialize the board
            for (int i = 0; i < Rows; i++) // Loop through rows
            {
                for (int j = 0; j < Columns; j++) // Loop through columns
                {
                    board[i, j] = Disc.Empty; // Set each cell to empty
                }
            }
        }

        public bool PlaceDisc(int column, Disc disc)
        {
            if (column < 0 || column >= Columns) return false; // Check if column is valid

            for (int row = Rows - 1; row >= 0; row--) // Start from the bottom row
            {
                if (board[row, column] == Disc.Empty) // Find the first empty cell
                {
                    board[row, column] = disc; // Place the disc there
                    return true; // Successfully placed the disc
                }
            }
            return false; // Column is full
        }

        public bool CheckWin(Disc disc)
        {
            // Check horizontal, vertical, and diagonal win conditions
            return CheckHorizontalWin(disc) || CheckVerticalWin(disc) || CheckDiagonalWin(disc);
        }

        private bool CheckHorizontalWin(Disc disc)
        {
            for (int row = 0; row < Rows; row++) // Loop through rows
            {
                for (int col = 0; col < Columns - 3; col++) // Loop through columns, stopping 3 before the end
                {
                    if (board[row, col] == disc && board[row, col + 1] == disc &&
                        board[row, col + 2] == disc && board[row, col + 3] == disc)
                    {
                        return true; // Found 4 in a row horizontally
                    }
                }
            }
            return false; // No horizontal win found
        }

        private bool CheckVerticalWin(Disc disc)
        {
            for (int col = 0; col < Columns; col++) // Loop through columns
            {
                for (int row = 0; row < Rows - 3; row++) // Loop through rows, stopping 3 before the end
                {
                    if (board[row, col] == disc && board[row + 1, col] == disc &&
                        board[row + 2, col] == disc && board[row + 3, col] == disc)
                    {
                        return true; // Found 4 in a row vertically
                    }
                }
            }
            return false; // No vertical win found
        }

        private bool CheckDiagonalWin(Disc disc)
        {
            // Check for both descending and ascending diagonals
            for (int row = 0; row < Rows - 3; row++) // Loop through rows
            {
                for (int col = 0; col < Columns - 3; col++) // Loop through columns
                {
                    if (board[row, col] == disc && board[row + 1, col + 1] == disc &&
                        board[row + 2, col + 2] == disc && board[row + 3, col + 3] == disc)
                    {
                        return true; // Found 4 in a row diagonally (descending)
                    }
                }
            }
            for (int row = 3; row < Rows; row++) // Loop through rows starting from the 4th row
            {
                for (int col = 0; col < Columns - 3; col++) // Loop through columns
                {
                    if (board[row, col] == disc && board[row - 1, col + 1] == disc &&
                        board[row - 2, col + 2] == disc && board[row - 3, col + 3] == disc)
                    {
                        return true; // Found 4 in a row diagonally (ascending)
                    }
                }
            }
            return false; // No diagonal win found
        }

        public void PrintBoard()
        {
            for (int row = 0; row < Rows; row++) // Loop through rows
            {
                for (int col = 0; col < Columns; col++) // Loop through columns
                {
                    switch (board[row, col]) // Check the disc type
                    {
                        case Disc.Empty:
                            Console.Write(". "); // Print dot for empty cell
                            break;
                        case Disc.Red:
                            Console.Write("R "); // Print R for red disc
                            break;
                        case Disc.Yellow:
                            Console.Write("Y "); // Print Y for yellow disc
                            break;
                    }
                }
                Console.WriteLine(); // New line after each row
            }
            Console.WriteLine(); // Extra new line for spacing
        }
    }

    public abstract class Player
    {
        public string Name { get; set; } // Player's name
        public Disc Disc { get; set; } // Disc type (Red or Yellow)

        protected Player(string name, Disc disc)
        {
            Name = name;
            Disc = disc;
        }

        public abstract int GetMove(GameBoard board); // Abstract method to get the move
    }

    public class HumanPlayer : Player
    {
        public HumanPlayer(string name, Disc disc) : base(name, disc) { }

        public override int GetMove(GameBoard board)
        {
            Console.WriteLine($"{Name}, enter your move (1-7): "); // Prompt for move
            int column;
            while (!int.TryParse(Console.ReadLine(), out column) || column < 1 || column > 7) // Validate input
            {
                Console.WriteLine("No Available Move. Enter a number between 1 and 7: "); // Error message
            }
            return column - 1; // Adjust for 0-based index
        }
    }

    public class Game
    {
        private GameBoard board;
        private Player player1;
        private Player player2;
        private Player currentPlayer;

        public Game(Player player1, Player player2)
        {
            this.player1 = player1;
            this.player2 = player2;
            currentPlayer = player1;
            board = new GameBoard(); // Initialize the game board
        }

        public void Play()
        {
            bool gameWon = false; // Flag to check if the game is won

            while (!gameWon) // Keep playing until the game is won
            {
                board.PrintBoard(); // Print the current state of the board
                int column = currentPlayer.GetMove(board); // Get the move from the current player
                if (board.PlaceDisc(column, currentPlayer.Disc)) // Try to place the disc
                {
                    if (board.CheckWin(currentPlayer.Disc)) // Check if the current player has won
                    {
                        board.PrintBoard(); // Print the board one last time
                        Console.WriteLine($"{currentPlayer.Name} is WINNER!"); // Print win message
                        gameWon = true; // Set the flag to true
                    }
                    else
                    {
                        currentPlayer = currentPlayer == player1 ? player2 : player1; // Switch turns
                    }
                }
                else
                {
                    Console.WriteLine("No Room! Try a different column."); // Error message if column is full
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Player player1 = new HumanPlayer("Player 1", Disc.Red); // Create player 1
            Player player2 = new HumanPlayer("Player 2", Disc.Yellow); // Create player 2

            Game game = new Game(player1, player2); // Create the game
            game.Play(); // Start the game
        }
    }
}
