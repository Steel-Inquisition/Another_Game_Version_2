using System;
using System.Collections.Generic;
// For the console
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;


namespace Basic_Game_2
{

    public partial class MainWindow : Window
    {

        // Once the boss is beat, end the game
        // Or once you are dead, end the game
        public void EndGame()
        {
            // Increase points based on stats
            points += (coin * 100);
            points += (Convert.ToInt32(ammo) * 500);
            points += (holyCross * 1000);
            points += (bomb * 500);
            points += (key * 1500);



            // if everyone is dead, then don't add bonus
            if (playerList[0].health <= 0 && playerList[1].health <= 0 && playerList[2].health <= 0 && playerList[3].health <= 0)
            {
                points += 0;
            } else
            {


                // Increase points based on player health
                for (int i = 0; i < playerList.Count; i++)
                {
                    points += (Convert.ToInt32(playerList[i].health) * 500);
                }

                // Increase points based on difficulty
                if (difficulty[0] < 5)
                {
                    points += 20000;
                }
                else if (difficulty[0] < 7)
                {
                    points += 10000;

                }
                else if (difficulty[0] < 10)
                {
                    points += 5000;

                }
            }

            // Show that the game has ended
            MessageBox.Show("GAME OVER!");
            MessageBox.Show($"You scored: {points} points!");

        }

    }


}