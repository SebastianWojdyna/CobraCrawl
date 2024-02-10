using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CobraCrawl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Dictionary which connect grid values to image sources
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            // If a grid position is empty then will be displayed an empty image asset
            { GridValue.Empty, Images.Empty },
            // If a position contains part of the snake then will be shown the body image
            { GridValue.Snake, Images.Body },
            // The same with food
            { GridValue.Food, Images.Food }
        };
        
        
        // Variables for number of rows and columns
        private readonly int rows = 15, cols = 15;
        // Image array for access the image for giver position in the grid
        private readonly Image[,] gridImages;
        // Game state object
        private GameState gameState;

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            // Initializing Game State object
            gameState = new GameState(rows, cols);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Draw();
            await GameLoop();
        }

        // Method which is called when User presses a Key
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // If the game is over
            if (gameState.GameOver)
            {
                // Then pressing a Key should not do anything
                return;
            }

            // Otherwise I check which Key is pressed
            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    break;
            }
        }

        // Method to move in regular intervals so need to be Async
        private async Task GameLoop()
        {
            // The loop will run untill the game is over
            while (!gameState.GameOver)
            {
                // Small delay - 100 ms to make the game slower
                await Task.Delay(100);
                // After delay I call move method
                gameState.Move();
                // And then draw the new game state
                Draw();
            }
        }

        private Image[,] SetupGrid()
        {
            // First create 2d array
            Image[,] images = new Image[rows, cols];
            // Next set the number of cols and rows in the game grid
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;

            // Loop over all grid positions
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    // Create a new image for each of them
                    Image image = new Image()
                    {
                        // Initially source is a empty image asset
                        Source = Images.Empty
                    };

                    // Store this images in 2d array
                    images[r, c] = image;
                    // Add it as a child of the game grid
                    GameGrid.Children.Add(image);
                }
            }

            // Outside the loop image array is returned
            return images;
        }

        // More general Draw method, where will be called DrawGrid()
        private void Draw()
        {
            DrawGrid();
            // Set the ScoreText to SCORE followed by actual score stored in the gameState
            // So the score will be update when the snake eats
            ScoreText.Text = $"SCORE {gameState.Score}";
        }

        // Method which will look at the grid array in GameState and update the grid images
        private void DrawGrid()
        {
            // It looks through every grid position
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    // Inside the loop I get grid value in current position
                    GridValue gridVal = gameState.Grid[r, c];
                    // Set the source for correspondent image using Dictionary
                    gridImages[r, c].Source = gridValToImage[gridVal];
                }
            }
        }
    }
}
