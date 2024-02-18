using System;

namespace GemHunters
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
        }
    }

    class Game
    {
        public Board Board { get; }
        public Player Player1 { get; }
        public Player Player2 { get; }
        public Player CurrentTurn { get; private set; }
        public int TotalTurns { get; private set; }

        public Game()
        {
            Board = new Board();
            Player1 = new Player("P1", new Position(0, 0), this);
            Player2 = new Player("P2", new Position(Board.Size - 1, Board.Size - 1), this);
            CurrentTurn = Player1;
            TotalTurns = 0;
        }

        public void Start()
        {
            while (!IsGameOver())
            {
                Console.WriteLine($"Total turns: {TotalTurns}");
                Board.DisplayBoard(Player1, Player2);
                Console.WriteLine($"{CurrentTurn.Name}'s turn.");
                string direction = GetDirectionFromUser();
                CurrentTurn.Move(direction);
                SwitchTurn();
                TotalTurns++;
            }

            AnnounceWinner();
        }

        public void SwitchTurn()
        {
            CurrentTurn = (CurrentTurn == Player1) ? Player2 : Player1;
        }

        public bool IsGameOver()
        {
            return TotalTurns >= 15 || Player1.GemCount + Player2.GemCount == 3;
        }

        public void AnnounceWinner()
        {
            int p1Gems = Player1.GemCount;
            int p2Gems = Player2.GemCount;
            Console.WriteLine($"Game over. Player 1 collected {p1Gems} gems. Player 2 collected {p2Gems} gems.");

            if (p1Gems > p2Gems)
                Console.WriteLine("Player 1 wins!");
            else if (p2Gems > p1Gems)
                Console.WriteLine("Player 2 wins!");
            else
                Console.WriteLine("It's a tie!");

            Console.ReadLine();
        }

        private string GetDirectionFromUser()
        {
            Console.WriteLine("Enter direction (U for up, D for down, L for left, R for right): ");
            return Console.ReadLine().ToUpper();
        }
    }

    class Board
    {
        public int Size { get; } = 6;
        public Cell[,] cells;

        public Board()
        {
            cells = new Cell[Size, Size];

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    cells[i, j] = new Cell("-");
                }
            }

            Random random = new Random();
            for (int i = 0; i < 3; i++)
            {
                int gemRow = random.Next(0, Size);
                int gemCol = random.Next(0, Size);
                cells[gemRow, gemCol].Occupant = "G";
            }

            for (int i = 0; i < 4; i++)
            {
                int obstacleRow = random.Next(0, Size);
                int obstacleCol = random.Next(0, Size);
                if (cells[obstacleRow, obstacleCol].Occupant == "G")
                {
                    i--;
                    continue;
                }
                cells[obstacleRow, obstacleCol].Occupant = "O";
            }
        }

        public bool IsObstacle(Position position)
        {
            return cells[position.Y, position.X].Occupant == "O";
        }

        public void DisplayBoard(Player p1, Player p2)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (p1.Position.X == j && p1.Position.Y == i)
                        Console.Write("P1 ");
                    else if (p2.Position.X == j && p2.Position.Y == i)
                        Console.Write("P2 ");
                    else
                        Console.Write(cells[i, j].Occupant + " ");
                }
                Console.WriteLine();
            }
        }
    }


    class Cell
    {
        public string Occupant { get; set; }

        public Cell(string occupant)
        {
            Occupant = occupant;
        }
    }

    class Player
    {
        public string Name { get; }
        public Position Position { get; private set; }
        public int GemCount { get; private set; }
        private readonly Game game;

        public Player(string name, Position position, Game game)
        {
            Name = name;
            Position = position;
            GemCount = 0;
            this.game = game;
        }

        public void Move(string direction)
        {
            int newX = Position.X;
            int newY = Position.Y;

            switch (direction)
            {
                case "U":
                    newY = Math.Max(0, Position.Y - 1);
                    break;
                case "D":
                    newY = Math.Min(game.Board.Size - 1, Position.Y + 1);
                    break;
                case "L":
                    newX = Math.Max(0, Position.X - 1);
                    break;
                case "R":
                    newX = Math.Min(game.Board.Size - 1, Position.X + 1);
                    break;
                default:
                    break;
            }

            Position newPosition = new Position(newX, newY);
            if (IsValidMove(newPosition))
            {
                Position = newPosition;
                string currentOccupant = game.Board.cells[Position.Y, Position.X].Occupant;
                game.Board.cells[Position.Y, Position.X].Occupant = "-";
                if (currentOccupant == "G")
                {
                    GemCount++;
                }
            }
            else
            {
                Console.WriteLine("Invalid move. Player loses a turn.");
            }
        }

        private bool IsValidMove(Position newPosition)
        {
            if (newPosition.X < 0 || newPosition.X >= game.Board.Size || newPosition.Y < 0 || newPosition.Y >= game.Board.Size)
                return false; // Out of bounds

            string occupant = game.Board.cells[newPosition.Y, newPosition.X].Occupant;
            return occupant != "O"; // Check if it's not an obstacle
        }

    }


    class Position
    {
        public int X { get; }
        public int Y { get; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
