using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Basic_Game_2
{

    public partial class MainWindow : Window
    {
     
        // Use a holy cross when press button
        public void UseHolyCross()
        {

            // use a holy cross when pressing B and have enough
            if (Keyboard.IsKeyDown(Key.B) && holyCross > 0)
            {

                // Use the holy cross
                holyCross--;

                // broken for now
                MessageBox.Show("This is Broken. Fix it later");


                // Show that the holy cross is used
                LogBox.Text += $"Holy Cross was used! \n";

                // Use the holy cross function
                HolyCrossUse();

                // Show that the INV changed
                TotalPartyInv.Text = $" Coin: {coin} \n Ammo: {ammo} \n Holy Cross: {holyCross} \n Key: {key} \n Bomb:{bomb}";


                // Show thatthe Ui has changed
                UpdateUi();
                ScrollBar.ScrollToEnd();
            }

        }


        // Usage of Holy Cross
        public void HolyCrossUse()
        {


            foreach (Rectangle x in PlayerSpace.Children.OfType<Rectangle>())
            {
                for (int i = 0; i < enemyStats.Count; i++)
                {
                    // Find enemy
                    if ((string)x.Tag == $"enemy-{i}")
                    {

                        // set all enemies health bellow 0
                        enemyStats[i].health = -1;

                        // show that they were killed by the holy cross
                        LogBox.Text += $"{enemyStats[i].name} was killed forever by Holy Cross! \n";

                        // delete them from the map permenetly (not canvas)
                        DeleteALlEnemies();

                        // Update the map
                        UpdateUi();
                        ScrollBar.ScrollToEnd();

                        // kill the enemy on the canvas
                        enemyStats[i].checkIfDead(itemstoremove, progressstoremove, x, healthBarList[i], PlayerSpace, i);
                    }
                }


            }

        }
    }


}