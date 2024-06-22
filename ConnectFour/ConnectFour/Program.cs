using System;

namespace ConnectFour
{
    public enum Disc { Empty, Red, Yellow }

    public class GameBoard
    {
        private const int Rows = 6;
        private const int Columns = 7;
        private Disc[,] board;
        private int[] columnCount;

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

        public bool IsColumnValid(int column)
        {
            return column >= 0 && column < Columns && columnCount[column] < Rows;
        }

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

        public bool CheckWin(Disc disc)
        {
            return CheckHorizontalWin(disc) || CheckVerticalWin(disc) || CheckDiagonalWin(disc);
        }

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

        private bool CheckDiagonalWin(Disc disc)
        {
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

        public bool IsFull()
        {
            foreach (var count in columnCount)
            {
                if (count < Rows) return false;
            }
            return true;
        }

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

    public abstract class Player
    {
        public string Name { get; set; }
        public Disc Disc { get; set; }

        protected Player(string name, Disc disc)
        {
            Name = name;
            Disc = disc;
        }

        public abstract int GetMove(GameBoard board);
    }

    public class HumanPlayer : Player
    {
        public HumanPlayer(string name, Disc disc) : base(name, disc) { }

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
            board = new GameBoard();
        }

        public void Play()
        {
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
