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
        // Variables for number of rows and columns
        private readonly int rows = 15, cols = 15;
        // Image array for access the image for giver position in the grid
        private readonly Image[,] gridImages;

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
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
    }
}
