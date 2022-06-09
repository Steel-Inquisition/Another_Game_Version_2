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
        public void UseHolyCross()
        {
            if (Keyboard.IsKeyDown(Key.B) && holyCross > 0)
            {
                holyCross--;

                MessageBox.Show("This is Broken. Fix it later");

                LogBox.Text += $"Holy Cross was used! \n";

                HolyCrossUse();

                TotalPartyInv.Text = $" Coin: {coin} \n Ammo: {ammo} \n Holy Cross: {holyCross} \n Key: {key} \n Bomb:{bomb}";

                UpdateUi();
                ScrollBar.ScrollToEnd();
            }

        }



        public void HolyCrossUse()
        {


            foreach (Rectangle x in PlayerSpace.Children.OfType<Rectangle>())
            {
                for (int i = 0; i < enemyStats.Count; i++)
                {

                    if ((string)x.Tag == $"enemy-{i}")
                    {
                        enemyStats[i].health = -1;

                        LogBox.Text += $"{enemyStats[i].name} was killed forever by Holy Cross! \n";

                        DeleteALlEnemies();

                        UpdateUi();
                        ScrollBar.ScrollToEnd();
                        enemyStats[i].checkIfDead(itemstoremove, progressstoremove, x, healthBarList[i], PlayerSpace, i);
                    }
                }


            }

        }
    }


}