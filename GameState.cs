
using System;
using System.Collections.Generic;
namespace SnakeGameWPF
{
    public class GameState
    {

        public int Rows { get; }
        public int Cols { get; }
        public GridValue[,] Grid;
        public bool GameOver { get; private set; }
        public int Score { get; private set; }
        public Direction Dir { get; private set; }
        private readonly Random random = new Random();
        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();
        private readonly LinkedList<Direction> dirchange = new LinkedList<Direction>();


        public GameState(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridValue[Rows, Cols];
            Dir = Direction.Right;
            AddSnake();
            AddFood();
        }
        private void AddSnake()
        {
            int r = Rows / 2;
            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridValue.snake;
                snakePositions.AddFirst(new Position(r, c));
            }
        }
        private IEnumerable<Position> EmptyPositions()
        {
            for (int r = 0; r < Rows; r++)

            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c] == GridValue.empty)
                        yield return new Position(r, c);
                }


            }
        }
        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());
            if (empty.Count == 0)
                return;
            Position pos = empty[random.Next(empty.Count)];

            Grid[pos.Row, pos.Col] = GridValue.food;
        }
        public Position HeadPosition()
        {
            return snakePositions.First.Value;
        }
        public Position TailPosition()
        {
            return snakePositions.Last.Value;
        }
        private void AddHead(Position pos)
        {
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.snake;

        }
        private void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;

            Grid[tail.Row, tail.Col] = GridValue.empty;
            snakePositions.RemoveLast();

        }
        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }
        private bool OutSideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;
        }
        private GridValue WillHit(Position newHeadPosition)
        {
            if (OutSideGrid(newHeadPosition))
            {
                return GridValue.outside;
            }
            if (newHeadPosition == TailPosition())
            {
                return GridValue.empty;
            }
            return Grid[newHeadPosition.Row, newHeadPosition.Col];
        }
         private Direction getLastDirection()
        {
            if (dirchange.Count == 0)
                return Dir;
            return dirchange.Last.Value; 
        }
        private bool CanChangeDirection(Direction newDir)
        {

            if (dirchange.Count == 2)
                return false;
            Direction lastDir = getLastDirection();
            return lastDir != newDir && lastDir.Opposite() != newDir;
        }
        public void ChangeDirection(Direction dir)
        {
            if (CanChangeDirection(dir))
                dirchange.AddLast(dir);
        }
        public void Move()
        {
            if (dirchange.Count > 0)
            {
                Dir  = dirchange.First.Value;
                dirchange.RemoveFirst();
            }
            Position newHeadPos = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);
            if (hit == GridValue.outside || hit == GridValue.snake)
            {
                GameOver = true; 

            }else if(hit == GridValue.empty)
            {
                RemoveTail();
                AddHead(newHeadPos);

            }
            else if(hit == GridValue.food)
            {
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }

        }
    }
}
