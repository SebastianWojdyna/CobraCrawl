﻿using Microsoft.EntityFrameworkCore;
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

        // Dictionary which maps directions to head rotations
        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            // For the up direction there is no rotation
            { Direction.Up, 0 },
            // For the right direction there is 90 degrees rotation
            { Direction.Right, 90 },
            // For down there is 180 degrees rotation
            { Direction.Down, 180 },
            // For left there is 270 degrees rotation
            { Direction.Left, 270 },
        };


        // Variables for number of rows and columns
        private readonly int rows = 20, cols = 20;
        // Image array for access the image for giver position in the grid
        private readonly Image[,] gridImages;
        // Game state object
        private GameState gameState;
        // Game running object which is false by default
        private bool gameRunning;
        // Variable which check if the game is paused
        private bool isPaused = false;


        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            // Initializing Game State object
            gameState = new GameState(rows, cols);
        }

        private async Task RunGame()
        {
            Draw();
            await ShowCountDown();
            // Hide the overlay
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            // Create fresh game state for the next game
            gameState = new GameState(rows, cols);
        }

        // Method for change pause state
        private void TogglePauseGame()
        {
            isPaused = !isPaused; // Change value on opposite
            Overlay.Visibility = isPaused ? Visibility.Visible : Visibility.Collapsed;
            OverlayText.Text = isPaused ? "Gra zapauzowana. Naciśnij dowolny klawisz, aby wznowić." : "Wciśnij dowolny klawisz, aby rozpocząć.";
        }


        // When the user presses a key then the Window_PreviewKeyDown is called
        // And after that Window_KeyDown is also called
        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // If the overlay is visible
            if (Overlay.Visibility == Visibility.Visible && !gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
                e.Handled = true;
            }
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
            else if (e.Key == Key.P && !isPaused)
            {
                TogglePauseGame();
            }
            else if (isPaused)
            {
                TogglePauseGame();
            }
            else
            {
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
        }

        // Method to move in regular intervals so need to be Async
        private async Task GameLoop()
        {
            // The loop will run untill the game is over
            while (!gameState.GameOver)
            {
                if (isPaused)
                {
                    await Task.Delay(100);
                    continue;
                }

                // After delay I call move method
                gameState.Move();
                // And then draw the new game state
                Draw();
                // Small delay - 100 ms to make the game slower
                await Task.Delay(100);
            }
        }

        private Image[,] SetupGrid()
        {
            // First create 2d array
            Image[,] images = new Image[rows, cols];
            // Next set the number of cols and rows in the game grid
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols / (double)rows);

            // Loop over all grid positions
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    // Create a new image for each of them
                    Image image = new Image()
                    {
                        // Initially source is a empty image asset
                        Source = Images.Empty,
                        // For images to rotate around center point
                        RenderTransformOrigin = new Point(0.5, 0.5)
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
            DrawSnakeHead();
            // Set the ScoreText to SCORE followed by actual score stored in the gameState
            // So the score will be update when the snake eats
            ScoreText.Text = $"WYNIK {gameState.Score}";
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
                    // This ensures that the only rotating image is the one, which is snake's head
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        // Method which draw head of snake and use current snake direction for the correct head roration
        private void DrawSnakeHead()
        {
            // First I get the position of the snake's head
            Position headPos = gameState.HeadPosition();
            // And get a grid image for that position
            Image image = gridImages[headPos.Row, headPos.Col];
            // Next set the source to the head image
            image.Source = Images.Head;
            // Apply the rotation to the image
            // First get number of degrees from dictionary
            int rotation = dirToRotation[gameState.Dir];
            // Then, rotate the image by that amount
            image.RenderTransform = new RotateTransform(rotation);
        }

        // Method which draw the snake when user failed and the snake is dead
        private async Task DrawDeadSnake()
        {
            // Create a list containg all the snake positions
            List<Position> positions = new List<Position>(gameState.SnakePositions());
            // Make iterations through these positions
            for (int i = 0; i < positions.Count; i++)
            {
                // Grep the position with index i 
                Position pos = positions[i];
                // Decide the image source from that position
                // If i == 0 I need the image called DeadHead otherwise
                // Otherwise, I need the DeadBody image
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                // Set the source of the images in current positions
                gridImages[pos.Row, pos.Col].Source = source;
                // And add the small delay
                await Task.Delay(50);

            }
        }

        // Simple cound down 
        private async Task ShowCountDown()
        {
            // Count from 3 down to 1
            for (int i = 3; i >= 1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        // Helper method for debugging
        private void ExceptionHandler(DbUpdateException dbEx)
        {
            var builder = new StringBuilder("Wystąpił błąd podczas zapisu do bazy danych.\n");
            try
            {
                builder.AppendLine(dbEx.InnerException.Message);
            }
            catch
            {
                builder.AppendLine("Nie można uzyskać szczegółów wewnętrznego wyjątku.");
            }

            MessageBox.Show(builder.ToString());
        }

        // Method which save the score to the database
        private void SaveHighScore(string playerName, int score)
        {
            using (var db = new SnakeGameContext())
            {
                var highScore = new HighScore
                {
                    PlayerName = playerName,
                    Score = score,
                    Date = DateTime.Now
                };

                try
                {
                    db.HighScores.Add(highScore);
                    db.SaveChanges();
                }
                catch (DbUpdateException dbEx)
                {
                    // Exception related to db update
                    ExceptionHandler(dbEx);
                }
                catch (Exception ex)
                {
                    // Other exceptions
                    MessageBox.Show($"Wystąpił błąd: {ex.Message}");
                }

            }
        }

        // Game over overlay
        private async Task ShowGameOver()
        {
            await DrawDeadSnake();
            // It starts with 1s delay
            await Task.Delay(1000);
            // And then make the overlay visible again
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "WCIŚNIJ DOWOLNY PRZYCISK BY ZACZĄĆ";

            // Logic for getting user neame
            var playerNameDialog = new PlayerNameDialog();
            if (playerNameDialog.ShowDialog() == true)
            {
                string playerName = playerNameDialog.PlayerName;
                if (!string.IsNullOrEmpty(playerName))
                {
                    SaveHighScore(playerName, gameState.Score); // Save score with user's name
                    ShowHighScores(); // Show best scores
                }
            }

            // Reset the game state to be able start form the beggining
            gameState = new GameState(rows, cols);
            gameRunning = false;
        }

        // Method which takes bet scores from database and show them to user
        // The method is called when the user paste his name 
        private void ShowHighScores()
        {
            using (var db = new SnakeGameContext())
            {
                var highScores = db.HighScores
                    .OrderByDescending(h => h.Score)
                    .Take(15) // Show best 15 scores
                    .ToList();


                var highScoresText = new StringBuilder();
                highScores.ForEach(hs =>
                    highScoresText.AppendLine($"{hs.PlayerName} - {hs.Score}"));

                var dialog = new CustomDialog(highScoresText.ToString());
                dialog.Title = "Najlepsze wyniki";
                dialog.ShowDialog();
            }
        }
    }
}
