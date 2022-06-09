using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;


namespace Basic_Game_2
{
    public partial class MainWindow : Window
    {

        public void StartGame()
        {
            // Initialize player size
            Player.Width = playerList[0].size;
            Player.Height = playerList[0].size;

            currentPlayer = 0;

            // Change player fill to current image
            playerList[currentPlayer].PlayerImage(Player);

            // Select current player on the canvas
            playerList[currentPlayer].PlayerUiSelect(currentPlayer, PlayerUiBox, playerList);

            // PlayerUiBox
            PlayerUiBox.Visibility = Visibility.Visible;

            // Set up Current Room
            MakeMap();

            // Set Up Map
            CreateMiniMap();

            // Set up clear fog of war on map
            FindRoomOnMap();

            // Activate Special Room features
            Map.totalMap[Map.currentRoom].ActivateRoom(LogBox, ScrollBar, Player, MakeArmPusherBoss);

            // Change Menu to Total Inv
            TotalPartyInv.Text = $" Coin: {coin} \n Ammo: {ammo} \n Holy Cross: {holyCross} \n Key: {key} \n Bomb:{bomb}";

            // Add Timer for the game -> FrameRate
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1);
            dispatcherTimer.Tick += new EventHandler(GameTimerEvent); // linking the timer event
            dispatcherTimer.Start(); // starting the timer

            PlayerSpace.Focus(); // this is what will be mainly focused on for the program
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (playerList.Count < 4)
            {

                // For somereason I couldn't getthe content from a ComboBoxItem
                string className = classSelect.SelectedValue.ToString();

                // Search for name
                for (int i = 0; i < classMakerList.Count; i++)
                {
                    if (className == $"System.Windows.Controls.ComboBoxItem: " + classMakerList[i].className)
                    {
                        playerList.Add(new PlayerMaker(GetName.Text, classMakerList[i].className, 100 + classMakerList[i].classHealth, 100 + classMakerList[i].classMp, 100 + classMakerList[i].classHealthMax, 100 + classMakerList[i].classMpMax, 25 + classMakerList[i].classSize, classMakerList[i].classWeapon, 0 + classMakerList[i].classPhys, 0 + classMakerList[i].classMagic, 0 + classMakerList[i].classMagic, 10 + classMakerList[i].classPhysDef, 10 + classMakerList[i].classMagDef, 4 + classMakerList[i].classSpeed, 1 + classMakerList[i].classMpRegen, 20 + classMakerList[i].invisibiltyFrames));
                    }
                }

                string description = $"\n Name: {playerList[currentPlayer].name} the {playerList[currentPlayer].playerClass} \n Health:{playerList[currentPlayer].health}/{playerList[currentPlayer].healthMax} \n Mp:{playerList[currentPlayer].mp}/{playerList[currentPlayer].mpMax} \n Phys:{playerList[currentPlayer].phys} \n Magic: {playerList[currentPlayer].magic} \n Gun: {playerList[currentPlayer].gun} \n Phys Def: {playerList[currentPlayer].phys} \n Magic Def: {playerList[currentPlayer].magDef} \n Speed: {playerList[currentPlayer].speed} \n Mp Regen: {playerList[currentPlayer].mpRegen} \n Size: {playerList[currentPlayer].size} \n Weapon: {playerList[currentPlayer].weapon.name} \n Boons: ";

                foreach (BoonMaker x in playerList[currentPlayer].playerBoons.OfType<BoonMaker>())
                {
                    description += x.boonName;
                }

                
                foreach (TextBlock x in TitleSpace.Children.OfType<TextBlock>())
                {
                    if (x.Name == $"CurrentPlayersBlock{playerList.Count}")
                    {
                        x.Text = description;
                    }
                }

                uiList.Add(new PlayerUi(description, currentPlayer, PlayerUiBox, playerList));

                currentPlayer++;
            }
            else
            {
                MessageBox.Show("You Can't add any more players!");
            }

        }

        private void classSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string className = classSelect.SelectedValue.ToString();

            // Search for name
            for (int i = 0; i < classMakerList.Count; i++)
            {
                if (className == $"System.Windows.Controls.ComboBoxItem: " + classMakerList[i].className)
                {
                    ClassDisplay.Text = classMakerList[i].classDescription;
                }
            }
        }

        private void StartingGame(object sender, RoutedEventArgs e)
        {

            if (playerList.Count == 4)
            {
                TitleSpace.Children.Clear();
                StartGame();
            }
            else
            {
                MessageBox.Show($"Not Enough Players! You need to create 4 players but you only have {playerList.Count}!");
            }

        }
    }

}