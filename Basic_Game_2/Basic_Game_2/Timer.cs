using System.Windows;
using System.Windows.Media;


namespace Basic_Game_2
{

    public partial class MainWindow : Window
    {
        public void CheckTimer()
        {
            TimerBar.Value = timer;

            if (timer > maxTimer)
            {
                difficulty[0]++;
                ChangeDifficultyValues(difficulty[0], maxTimer);

            }
        }

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

            DifficultyBlock.Text = "Difficulty: " + change;

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


            TimerBar.Maximum = maximum;

            timer = 0;

        }
    }


}