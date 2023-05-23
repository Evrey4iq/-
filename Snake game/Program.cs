using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SnakeGame
{
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum FruitType
    {
        Apple,
        Cherry,
        Orange,
        Watermelon
    }

    public class Fruit
    {
        public Position Position { get; set; }
        public FruitType Type { get; set; }

        public Fruit(int x, int y, FruitType type)
        {
            Position = new Position(x, y);
            Type = type;
        }
    }

    public class SnakeGame
    {
        private int width;
        private int height;
        private Position fruit;
        private List<Position> snake;
        private int score;
        private Direction direction;
        private bool isGameOver;
        private List<Fruit> fruits;

        public SnakeGame(int width, int height)
        {
            this.width = width;
            this.height = height;
            fruit = new Position(width / 2, height / 2);
            snake = new List<Position>
            {
                new Position(width / 2, height / 2)
            };
            score = 0;
            direction = Direction.Right;
            isGameOver = false;
            fruits = new List<Fruit>();
            InitializeFruits();
        }

        private void InitializeFruits()
        {
            fruits.Add(new Fruit(width / 2, height / 2, FruitType.Apple));
            fruits.Add(new Fruit(width / 3, height / 3, FruitType.Cherry));
            fruits.Add(new Fruit(width / 4, height / 4, FruitType.Orange));
            fruits.Add(new Fruit(width / 5, height / 5, FruitType.Watermelon));
        }

        public void StartGame()
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(width + 2, height + 2);
            Console.SetBufferSize(width + 2, height + 2);

            DrawBorder();
            DrawFruit();
            DrawFruits();
            DrawSnake();

            while (!isGameOver)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;

                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                            if (direction != Direction.Down)
                                direction = Direction.Up;
                            break;
                        case ConsoleKey.DownArrow:
                            if (direction != Direction.Up)
                                direction = Direction.Down;
                            break;
                        case ConsoleKey.LeftArrow:
                            if (direction != Direction.Right)
                                direction = Direction.Left;
                            break;
                        case ConsoleKey.RightArrow:
                            if (direction != Direction.Left)
                                direction = Direction.Right;
                            break;
                    }
                }

                MoveSnake();
                CheckCollision();
                Thread.Sleep(100);
            }

            Console.SetCursorPosition(width / 2 - 4, height / 2);
            Console.WriteLine("Game Over!");
            Console.SetCursorPosition(width / 2 - 5, height / 2 + 1);
            Console.WriteLine("Your Score: " + score);
        }

        private void DrawBorder()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write("+");
            for (int i = 0; i < width; i++)
            {
                Console.Write("-");
            }
            Console.Write("+");

            for (int i = 0; i < height; i++)
            {
                Console.SetCursorPosition(0, i + 1);
                Console.Write("|");
                Console.SetCursorPosition(width + 1, i + 1);
                Console.Write("|");
            }

            Console.SetCursorPosition(0, height + 1);
            Console.Write("+");
            for (int i = 0; i < width; i++)
            {
                Console.Write("-");
            }
            Console.Write("+");
        }

        private void DrawSnake()
        {
            foreach (var position in snake)
            {
                Console.SetCursorPosition(position.X + 1, position.Y + 1);
                Console.Write("*");
            }
        }

        private void ClearSnakeTail()
        {
            var tail = snake.Last();
            Console.SetCursorPosition(tail.X + 1, tail.Y + 1);
            Console.Write(" ");
        }

        private void DrawFruit()
        {
            Console.SetCursorPosition(fruit.X + 1, fruit.Y + 1);
            Console.Write("@");
        }

        private void DrawFruits()
        {
            foreach (var fruit in fruits)
            {
                Console.SetCursorPosition(fruit.Position.X + 1, fruit.Position.Y + 1);
                Console.Write(GetFruitSymbol(fruit.Type));
            }
        }

        private char GetFruitSymbol(FruitType type)
        {
            switch (type)
            {
                case FruitType.Apple:
                    return 'A';
                case FruitType.Cherry:
                    return 'C';
                case FruitType.Orange:
                    return 'O';
                case FruitType.Watermelon:
                    return 'W';
                default:
                    return '@';
            }
        }

        private void MoveSnake()
        {
            var head = snake.First();
            var newHead = new Position(head.X, head.Y);

            switch (direction)
            {
                case Direction.Up:
                    newHead.Y--;
                    break;
                case Direction.Down:
                    newHead.Y++;
                    break;
                case Direction.Left:
                    newHead.X--;
                    break;
                case Direction.Right:
                    newHead.X++;
                    break;
            }

            snake.Insert(0, newHead);
            ClearSnakeTail();

            if (newHead.X == fruit.X && newHead.Y == fruit.Y)
            {
                score++;
                GenerateNewFruit();
                DrawFruit();
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
            }

            DrawSnake();
        }

        private void GenerateNewFruit()
        {
            var random = new Random();
            int x, y;
            FruitType type;
            do
            {
                x = random.Next(0, width);
                y = random.Next(0, height);
                type = (FruitType)random.Next(0, Enum.GetValues(typeof(FruitType)).Length);
            } while (snake.Any(p => p.X == x && p.Y == y) || fruits.Any(f => f.Position.X == x && f.Position.Y == y));

            fruits.Add(new Fruit(x, y, type));
        }

        private void CheckCollision()
        {
            var head = snake.First();

            if (head.X < 0 || head.X >= width || head.Y < 0 || head.Y >= height || snake.Skip(1).Any(p => p.X == head.X && p.Y == head.Y))
            {
                isGameOver = true;
            }

            if (fruits.Any(f => f.Position.X == head.X && f.Position.Y == head.Y))
            {
                var fruit = fruits.First(f => f.Position.X == head.X && f.Position.Y == head.Y);
                fruits.Remove(fruit);
                score++;
                GenerateNewFruit();
                DrawFruits();
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            int width = 40;
            int height = 20;

            SnakeGame game = new SnakeGame(width, height);
            game.StartGame();
        }
    }
}
