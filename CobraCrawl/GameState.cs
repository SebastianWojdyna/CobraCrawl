using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace CobraCrawl
{
    public class GameState
    {
        public int Rows { get; } // Number of rows in the grid
        public int Cols { get; } // Number of columns in the grid
        public GridValue[,] Grid { get; } // Grid itself which is rectangle array of grid values
        public Direction Dir { get; private set; } // Define for snake where to move next
        public int Score { get; private set; } // Score property
        public bool GameOver { get; private set; } // Game over boolean 

        // Linked list is used because it allow to add and delete both from beginning and end of the list
        // First element is the head of the snake and the last element is the tail 
        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();// List which contains positions which are currenly occupied by the snake
        // This random will be used to figure out where the food should appear
        private readonly Random random = new Random();

        // Constructor which takes the number of rows and cols as parameters
        public GameState(int rows, int cols) 
        {
            Rows = rows; 
            Cols = cols;
            // Initialize grid array with the size
            Grid = new GridValue[rows, cols];
            // When the game start snake'd direction will be right
            Dir = Direction.Right;

            AddSnake();
            AddFood();
        }

        // Method which add the snake to the grid
        private void AddSnake()
        {
            // Snake will appear in middle row in columns 1,2,3

            // Create variable for the middle row
            int r = Rows / 2;

            // Loop over the columns from 1 to 3
            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                // Add this position to the snake positions list
                snakePositions.AddFirst(new Position(r, c));
            }
        }

        // Method which returns all empty grid positions
        private IEnumerable<Position> EmptyPositions()
        {
            // Look through all rows and columns
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    // Check if the grid at r,c is empty
                    if (Grid[r, c] == GridValue.Empty)
                    {
                        // If is empty then yield return this position
                        yield return new Position(r, c);
                    }
                }
            }
        }

        // Method for adding food
        private void AddFood()
        {
            // Create a list of empty positions
            List<Position> empty = new List<Position>(EmptyPositions());

            if (empty.Count == 0)
            {
                return;
            }

            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = GridValue.Food;
        }

        // -----=== Snake related methods ===-----

        // Method which returns the positions of the snake's head
        public Position HeadPosition()
        {
            // Get this position from beggining of the list
            return snakePositions.First.Value;
        }

        // Method which returns the position of the snake's tail
        public Position TailPosition()
        {
            // Get this position from the other end of the list
            return snakePositions.Last.Value;
        }

        // Method which returns all the snake's positions as a IEnumerable
        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }

        // Method for making snake's new head by adding position
        private void AddHead(Position pos)
        {
            // Add giving possitions to the snake and making it the new head
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.Snake;
        }

        // Method for removing the tail
        private void RemoveTail()
        {
            // Getting the current tail position
            Position tail = snakePositions.Last.Value;
            // Making that position empty in the grid
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            // Removing it from the linked list
            snakePositions.RemoveLast();
        }

        // -----=== Methods for modifying the game state ===-----

        // Method for change the direction
        public void ChangeDirection(Direction dir) 
        {
            Dir = dir;
        }

        // Method which checks if given possition is outside the grid or not
        private bool OutsideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;
        }

        // Method which returns what the snake will hit if he moves there
        private GridValue WillHit(Position newHeadPos)
        {
            // Case if the new head position will be outside of the grid
            if (OutsideGrid(newHeadPos))
            {
                return GridValue.Outside;
            }
            
            // Case if the new head position is the same as current tail position
            if (newHeadPos == TailPosition())
            {
                return GridValue.Empty;
            }

            // General case
            return Grid[newHeadPos.Row, newHeadPos.Col];
        }

        // Method to move the snake 1 step in the current direction
        public void Move()
        {
            // Getting new head position
            Position newHeadPos = HeadPosition().Translate(Dir);
            // Check what the head will hit
            GridValue hit = WillHit(newHeadPos);

            if (hit == GridValue.Outside || hit == GridValue.Snake)
            {
                GameOver = true;
            }
            else if (hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if (hit == GridValue.Food)
            {
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }
        }
    }
}
