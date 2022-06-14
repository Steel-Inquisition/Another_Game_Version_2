using System.Windows;
using System.Windows.Media;


namespace Basic_Game_2
{

    public partial class MainWindow : Window
    {

        // Chdck timer
        public void CheckTimer()
        {

            // Change the current timer bar to show the timer stat
            TimerBar.Value = timer;

            //If the timer is above the max timer
            if (timer > maxTimer)
            {

                // Increase difficulty
                difficulty[0]++;

                // Change the difficulty
                ChangeDifficultyValues(difficulty[0], maxTimer);

            }
        }

        // Only used once when the timer runs out
        public void ChangeDifficultyValues(int change, int maximum)
        {
            // Enemy Health
            difficulty[1] = change * 100;
            // Enemy Damage
            difficulty[2] = change * 10;
            // Enemy Speed
            difficulty[3] = change / 2;

            // Weapon Damage
            difficulty[4] = change * 20;

            // Show the difficulty text
            DifficultyBlock.Text = "Difficulty: " + change;

            // If the difficulty is 1,2,3, change max timer and background of the timer
            if (difficulty[0] == 1)
            {
                maxTimer = 5000;

                TimerBar.Foreground = Brushes.GreenYellow;
            } else if (difficulty[0] == 2)
            {
                maxTimer = 8000;

                TimerBar.Foreground = Brushes.Yellow;

            }
            else if (difficulty[0] == 3)
            {
                maxTimer = 10000;

                TimerBar.Foreground = Brushes.Orange;

            }

            // Change max timer
            TimerBar.Maximum = maximum;

            // Timer is now 0
            timer = 0;

        }
    }


}