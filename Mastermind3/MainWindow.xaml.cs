using Microsoft.VisualBasic;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Mastermind3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random rnd = new Random();
        private Button selectedButton; // Track the currently selected button

        string[] selectedColors = new string[4];
        string[] randomColorSelection = new string[4];
        SolidColorBrush[,] guessingHistoryFeedback = new SolidColorBrush[10, 4];
        string correctCodeString;
        int selectedColorPosition = 0;


        private DispatcherTimer timer = new DispatcherTimer();

        string userNameCurrentPlayer;
        string userNameNextPlayer;
        int currentPlayerIndex = 0;
        bool userNameEntered = false;
        int attempts = 0;
        int points = 100;

        int amountOfPlays = 10;

        List<string> ranking = new List<string>();
        List<string> players = new List<string>();


        public MainWindow()
        {
            InitializeComponent();

            StartGame();

        }

        public void StartGame()
        {
            bool addAnotherPlayer;

            do
            {
                string playerName = Interaction.InputBox("Welkom! Geef de naam van een speler.", "Welkom");

                while (string.IsNullOrWhiteSpace(playerName))
                {
                    MessageBox.Show("De naam mag niet leeg zijn. Voer een geldige naam in.", "Ongeldige Invoer", MessageBoxButton.OK, MessageBoxImage.Warning);
                    playerName = Interaction.InputBox("Welkom! Geef de naam van een speler.", "Welkom");
                }
                players.Add(playerName);

                MessageBoxResult result = MessageBox.Show("Wilt u nog een speler toevoegen?", "Speler Toevoegen", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(result == MessageBoxResult.Yes)
                {
                    addAnotherPlayer = true;
                }
                else
                {
                    addAnotherPlayer= false;
                }
            } while (addAnotherPlayer);


            StringBuilder playerList = new StringBuilder("De volgende spelers doen mee:\n");
            foreach (string player in players)
            {
                playerList.AppendLine(player);
            }
            MessageBox.Show(playerList.ToString(), "Spelerslijst", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void CurrentPlayer()
        {

            userNameCurrentPlayer = players[currentPlayerIndex];

            userNameNextPlayer = players[(currentPlayerIndex + 1) % players.Count];
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            currentPlayerNameLabel.Content = userNameCurrentPlayer.ToString();
        }

        public void CreateRandomColorCombination()
        {
            CurrentPlayer();

            introductionCanvas.Visibility = Visibility.Collapsed;
            gameCanvas.Visibility = Visibility.Visible;
            attempts++;

            string[] colorsName = { "Red", "Yellow", "Green", "Blue", "White", "Orange" };

            int[] randomColors = new int[4];

            for (int i = 0; i < randomColors.Length; i++)
            {
                randomColors[i] = rnd.Next(0, colorsName.Length);
            }

            // Display color names in the window title for debugging
            // RandomColors[n] gives a number, this number is entered in 'ColorsName[]', this gives the name of a color and displays it as title
            correctCodeString = $"The correct code was: {colorsName[randomColors[0]]}, {colorsName[randomColors[1]]}, {colorsName[randomColors[2]]}, {colorsName[randomColors[3]]}";

            attempts++;
            this.Title = $"Poging {attempts}";

            randomColorSelection[0] = colorsName[randomColors[0]];
            randomColorSelection[1] = colorsName[randomColors[1]];
            randomColorSelection[2] = colorsName[randomColors[2]];
            randomColorSelection[3] = colorsName[randomColors[3]];

            StartCountdown();
        }

        private void color1Button_Click(object sender, RoutedEventArgs e)
        {
            radioButtonsGroupBox.Visibility = Visibility.Visible;
            selectedButton = color1Button;
            selectedColorPosition = 0;
            UncheckRadioButtons();
        }
        private void color2Button_Click(object sender, RoutedEventArgs e)
        {
            radioButtonsGroupBox.Visibility = Visibility.Visible;
            selectedButton = color2Button;
            selectedColorPosition = 1;
            UncheckRadioButtons();
        }
        private void color3Button_Click(object sender, RoutedEventArgs e)
        {
            radioButtonsGroupBox.Visibility = Visibility.Visible;
            selectedButton = color3Button;
            selectedColorPosition = 2;
            UncheckRadioButtons();
        }
        private void color4Button_Click(object sender, RoutedEventArgs e)
        {
            radioButtonsGroupBox.Visibility = Visibility.Visible;
            selectedButton = color4Button;
            selectedColorPosition = 3;
            UncheckRadioButtons();
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton colorRadioButton)
            {
                switch (colorRadioButton.Content.ToString())
                {
                    case "Rood":
                        selectedButton.Background = Brushes.Red;
                        selectedColors[selectedColorPosition] = "Red";
                        break;
                    case "Geel":
                        selectedButton.Background = Brushes.Yellow;
                        selectedColors[selectedColorPosition] = "Yellow";
                        break;
                    case "Groen":
                        selectedButton.Background = Brushes.Green;
                        selectedColors[selectedColorPosition] = "Green";
                        break;
                    case "Blauw":
                        selectedButton.Background = Brushes.Blue;
                        selectedColors[selectedColorPosition] = "Blue";
                        break;
                    case "Wit":
                        selectedButton.Background = Brushes.White;
                        selectedColors[selectedColorPosition] = "White";
                        break;
                    case "Oranje":
                        selectedButton.Background = Brushes.Orange;
                        selectedColors[selectedColorPosition] = "Orange";
                        break;
                    default:
                        break;
                }
            }

        }
        private void UncheckRadioButtons()
        {
            redRadioButton.IsChecked = false;
            greenRadioButton.IsChecked = false;
            blueRadioButton.IsChecked = false;
            whiteRadioButton.IsChecked = false;
            orangeRadioButton.IsChecked = false;
            yellowRadioButton.IsChecked = false;
        }
        private void DisplayGuessOnCanvas(string[] guess, int attempt)
        {
            int totalRowWidth = 250;
            double canvasWidth = attemptCanvas.ActualWidth;
            double startingLeft = (canvasWidth - totalRowWidth) / 2;

            int topPosition = attempt * (30 + 10);

            for (int i = 0; i < guess.Length; i++)
            {
                Label label = new Label
                {
                    Width = 50,
                    Height = 30,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(guess[i])),
                    Content = "",
                    BorderThickness = new Thickness(1),
                    BorderBrush = Brushes.Black,
                };

                Canvas.SetLeft(label, startingLeft + i * (50 + 10));
                Canvas.SetTop(label, topPosition);

                attemptCanvas.Children.Add(label);
            }
        }
        private void validateColorCode_Click(object sender, RoutedEventArgs e)
        {
            Button[] buttons = { color1Button, color2Button, color3Button, color4Button };

            for (int i = 0; i < selectedColors.Length; i++)
            {
                if (selectedColors[i] == randomColorSelection[i])
                {
                    SetButtonStyle(buttons[i], new Thickness(2, 2, 2, 20), Colors.DarkRed);
                }
                else if (randomColorSelection.Contains(selectedColors[i]))
                {
                    SetButtonStyle(buttons[i], new Thickness(2, 2, 2, 20), Colors.Wheat);
                    points -= 1;
                    pointsLabel.Content = $"Score: {points.ToString()}";
                }
                else
                {
                    points -= 2;
                    pointsLabel.Content = $"Score: {points.ToString()}";
                }
            }

            // Display the guess on the canvas
            DisplayGuessOnCanvas(selectedColors, attempts);

            if (selectedColors[0] == randomColorSelection[0] && selectedColors[1] == randomColorSelection[1] && selectedColors[2] == randomColorSelection[2] && selectedColors[3] == randomColorSelection[3])
            {
                StopCountdown();
                MessageBox.Show($"Code is gekraakt in {attempts} pogingen, nu is speler {userNameNextPlayer} aan de beurt", userNameCurrentPlayer, MessageBoxButton.OK, MessageBoxImage.Information);
                StopGame();
            }


            attempts++;

            StopCountdown();
            StartCountdown();
            this.Title = $"Poging {attempts}";
        }
        private void ResetGame()
        {
            gameCanvas.Visibility = Visibility.Collapsed;
            // Reset alle variabelen
            attempts = 0;
            points = 100;
            selectedColors = new string[4];
            randomColorSelection = new string[4];
            guessingHistoryFeedback = new SolidColorBrush[10, 4];
            selectedColorPosition = 0;

            // Reset guessing history + points + timer
            attemptCanvas.Children.Clear();
            pointsLabel.Content = points.ToString();
            timerLabel.Content = "0";

            color1Button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDDDDDD"));
            SetButtonStyle(color1Button, new Thickness(1,1,1,1), Colors.Black);
            color2Button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDDDDDD"));
            SetButtonStyle(color2Button, new Thickness(1, 1, 1, 1), Colors.Black);
            color3Button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDDDDDD"));
            SetButtonStyle(color3Button, new Thickness(1, 1, 1, 1), Colors.Black);
            color4Button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDDDDDD"));
            SetButtonStyle(color4Button, new Thickness(1, 1, 1, 1), Colors.Black);

            // Genereer een nieuwe kleurcode
            CreateRandomColorCombination();

            // Update de titel
            this.Title = $"Poging {attempts}";
        }
        private void SetButtonStyle(Button button, Thickness thickness, Color color)
        {
            button.BorderThickness = thickness;
            button.BorderBrush = new SolidColorBrush(color);
        }



        DateTime startTime;
        TimeSpan elapsedTime;
        ///<summary>
        ///De 'StartCountdown' methode start een timer
        ///die iedere seconde omhoog gaat. Wanneer de timer 10 seconden bereikt zal deze resetten
        ///en de variable 'attempts' verhogen met 1
        ///</summary>
        private void StartCountdown()
        {
            if (attempts > amountOfPlays)
            {
                MessageBox.Show($"You have reached the maximum amount of guesses, {correctCodeString} \nNu is speler {userNameNextPlayer} aan de beurt", userNameCurrentPlayer);
                StopGame();
            }
            else
            {
                startTime = DateTime.Now;
                timer.Tick += Timer_Tick; //Event koppelen
                timer.Interval = new TimeSpan(0, 0, 1); //Elke seconde
                timer.Start(); //Timer starten
            }

        }
        private void StopGame()
        {
            gameCanvas.Visibility = Visibility.Hidden;
            introductionCanvas.Visibility = Visibility.Visible;

            

            ranking.Add($"{userNameCurrentPlayer} - {attempts} pogingen - {points}/100");

            // Reset Guesses Canvas
            attemptCanvas.Children.Clear();

            // Update de titel
            this.Title = $"Mastermind";
        }

        ///<summary>
        ///De 'StopCountdown' methode stopt de timer
        ///die eerder werd gestart via de 'StartCountdown' methode
        ///</summary>
        private void StopCountdown()
        {
            timer.Stop();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            elapsedTime = DateTime.Now - startTime;
            timerLabel.Content = elapsedTime.ToString("ss");

            if (elapsedTime.TotalSeconds > 10)
            {
                StopCountdown();
                attempts++;
                this.Title = $"Poging {attempts}";
                StartCountdown();
            }

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Weet je zeker dat je het spel wilt beëindigen?", "Afsluiten", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.No)
            {
                // Annuleer het afsluiten van de applicatie
                e.Cancel = true;
            }
        }
        private void newGame_Click(object sender, RoutedEventArgs e)
        {
            ResetGame();
        }
        private void closeApplication_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void amountOfGuesses_Click(object sender, RoutedEventArgs e)
        {
            int newAmountOfPlays;

            string result = Interaction.InputBox("Hoeveel pogingen wilt u krijgen? Kies een aantal tussen 3 en 20.", "Invoer", "10", 500);
            while (string.IsNullOrWhiteSpace(result) || !int.TryParse(result, out newAmountOfPlays) || newAmountOfPlays < 3 || newAmountOfPlays > 20)
            {
                MessageBox.Show("Voer een geldig getal in tussen 3 en 20!", "Foutieve invoer", MessageBoxButton.OK, MessageBoxImage.Warning);
                result = Interaction.InputBox("Hoeveel pogingen wilt u krijgen? Kies een aantal tussen 3 en 20.", "Invoer", "10", 500);
            }

            amountOfPlays = newAmountOfPlays;
            MessageBox.Show($"Aantal pogingen ingesteld op {amountOfPlays}.", "Ingesteld", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void highScores_Click(object sender, RoutedEventArgs e)
        {
            if (ranking.Count == 0)
            {
                MessageBox.Show("Er zijn nog geen high scores!", "High Scores", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                StringBuilder sb = new StringBuilder("High Scores:\n");
                foreach (string score in ranking)
                {
                    sb.AppendLine(score);
                }
                MessageBox.Show(sb.ToString(), "High Scores", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
