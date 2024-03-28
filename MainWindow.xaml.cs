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
//using static System.Net.Mime.MediaTypeNames;

namespace SnakeGameWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridvalToImage = new()
        {
            {GridValue.empty , Images.Empty },
            {GridValue.snake , Images.Body },
            {GridValue.food , Images.Food }

        };
        private readonly int rows = 15, cols = 15;
        private readonly Image[,] gridImages;
        private GameState gamestate;
        private bool gameRunning;
        public MainWindow()
        {
             InitializeComponent();

            gridImages = SetupGrid();
            gamestate = new GameState(rows, cols);
        }
        private async Task RunGame()
        {
            Draw();
            await ShowCountDown();
            OverLay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            gamestate = new GameState(rows, cols);


        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(OverLay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }
            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }



        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gamestate.GameOver)
            {
                return; 
            }
            switch (e.Key)
            {
                case Key.Left:
                    gamestate.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gamestate.ChangeDirection(Direction.Right);
                    break;
                case Key.Up:
                    gamestate.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    gamestate.ChangeDirection(Direction.Down);
                    break;
            }

        }
        private async Task GameLoop()
        {
            while (!gamestate.GameOver)
            {
                await Task.Delay(100);
                gamestate.Move();
                Draw();
            }
        }

        public Image[,] SetupGrid()
        {
            Image[,]images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty

                    };
                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }
            return images;
        }
        void Draw()
        {
            DrawGrid();
            ScoreText.Text = $"Score {gamestate.Score}";
        }


        void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GridValue gridvalue = gamestate.Grid[r, c];
                    gridImages[r, c].Source = gridvalToImage[gridvalue];

                }
            }

        }
        private async Task ShowCountDown()
        {
            for (int i= 3; i >= 1; i--)
            {
                OverLayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }
        private async Task ShowGameOver()
        {
            await Task.Delay(1000);
            OverLay.Visibility = Visibility.Visible;
            OverLayText.Text = "Press any key to Start";


        }
    }
}
