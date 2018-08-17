using MemoryChallenge_Adapter.Game;
using MemoryChallenge_Client.Common;
using MemoryChallenge_Client.Controllers;
using MemoryChallenge_Client.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace MemoryChallenge_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClientController clientController;
        private GameColors correctColor;
        private GameZone currentGameZone = null;

        private const int MAX_LEVEL = 20;
        private const int PREVIEW_TIME = 3;

        private int countCorrectAnswers = 0;
        private int level = 1;

        public MainWindow(string UserName)
        {
            InitializeComponent();

            clientController = new ClientController(UserName);
            clientController.ConnectionEstablishedEvent += IdChanged;
            clientController.ChangeGameZoneEvent += GameZoneChanged;
            clientController.ErrorOccuredEvent += ErrorOccured;

            lbUserName.Content = UserName;
        }

        private void ErrorOccured(object data)
        {
            MessageBox.Show((string)data);
        }

        private void btnStartGame_Click(object sender, RoutedEventArgs e)
        {
            btnStartGame.IsEnabled = false;

            lbLevel.Content = level;
            clientController.StartGame();
        }

        private void IdChanged(object connectionID)
        {
            //lbID.Content = connectionID.ToString();
        }

        private void GameZoneChanged(object data)
        {
            var gameZone = (GameZone)data;
            setGameZone(gameZone);
        }

        private void setGameZone(GameZone gameZone)
        {
            countCorrectAnswers = 0;
            currentGameZone = gameZone;

            GameZonePanel.Children.Clear();

            correctColor = gameZone.CorrectColor;
            correctColorControl.Fill = getColorByName(correctColor.ToString());

            foreach (var tile in gameZone.GameTiles)
            {
                GameZonePanel.Children.Add(createTile(tile.Color));
            }

            GameTimer.ExecuteWithDelay(new Action(flipTiles), TimeSpan.FromSeconds(PREVIEW_TIME));
        }

        private void flipTiles()
        {
            var tiles = GameZonePanel.Children;

            foreach (Button tile in tiles)
            {
                tile.Background = new SolidColorBrush(Colors.OldLace);
                tile.Click += tileClicked;
            }
        }

        private Control createTile(GameColors color)
        {
            var tile = new Button();
            tile.Width = 100;
            tile.Height = 100;

            tile.Tag = color;
            tile.Background = getColorByName(color.ToString());

            return tile;
        }

        private void tileClicked(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            GameColors tileColor = (GameColors)button.Tag;

            if(tileColor != correctColor)
            {
                playerLoose();
            }
            else
            {
                correctAnswer(button);
            }
        }

        private void correctAnswer(Control tile)
        {
            tile.IsEnabled = false;

            countCorrectAnswers++;

            if(countCorrectAnswers == currentGameZone.CountCorrectTiles)
            {
                playerCompleteLevel();
            }
        }

        private void playerCompleteLevel()
        {
            increaseLevel();

            if (level == MAX_LEVEL)
            {
                finishGame();
            }
            else
            {
                clientController.ContinueGame(level);
            }
        }

        private void increaseLevel()
        {
            level++;
            lbLevel.Content = level;
        }

        private void finishGame()
        {
            resetGame();
            MessageBox.Show("Congrats, u win!");
        }

        private void playerLoose()
        {
            resetGame();
            MessageBox.Show("You loose, try again");
        }

        private void resetGame()
        {
            level = 1;
            GameZonePanel.Children.Clear();
            btnStartGame.IsEnabled = true;
        }

        private SolidColorBrush getColorByName(string name)
        {
            return new BrushConverter().ConvertFromString(name) as SolidColorBrush;
        }
    }
}
