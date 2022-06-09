using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.IO;
using System.Text.Json;

namespace Basic_Game_2
{
    // fix minimap... soon

    public partial class MainWindow : Window
    {
        public void SaveGame()
        {

            if (Keyboard.IsKeyDown(Key.S))
            {
                // Save File
                MessageBox.Show("The Game is Saved!");

                // Seralzie the data
                var options = new JsonSerializerOptions { IncludeFields = true };
                string playerString = JsonSerializer.Serialize(boonList[1], options);
                string mapString = JsonSerializer.Serialize(Map, options);

                int[] data = { points, coin, Convert.ToInt16(ammo), holyCross, bomb, key, currentPlayer, timer, maxTimer, difficulty[0], difficulty[1], difficulty[2], difficulty[3], difficulty[4], Convert.ToInt32(Canvas.GetLeft(Player)), Convert.ToInt32(Canvas.GetTop(Player)) };

                string basic_data = JsonSerializer.Serialize(data, options);

                // The data as a JSON string may be easily saved to a file
                File.WriteAllText(@$"data-files/save_files/player.txt", playerString);
                File.WriteAllText(@$"data-files/save_files/player_map.txt", mapString);
                File.WriteAllText(@$"data-files/save_files/basic_data.txt", basic_data);
            }

        }

        // Load all the data from the JSON / Text file
        public void Load_User_Data(object? sender, EventArgs e)
        {
            // Seralzie
            var options = new JsonSerializerOptions { IncludeFields = true };
            var playerOptions = new JsonSerializerOptions { IncludeFields = true };
            string playerStringFromFile = File.ReadAllText(@"data-files/save_files/player.txt");
            string mapStringFromFile = File.ReadAllText(@"data-files/save_files/player_map.txt");
            string basicItemFromFile = File.ReadAllText(@$"data-files/save_files/basic_data.txt");

            MapMaker? tempMap = JsonSerializer.Deserialize<MapMaker>(mapStringFromFile, options);
            int[]? basic_data = JsonSerializer.Deserialize<int[]>(basicItemFromFile, options);

            //List<PlayerMaker>? temp_player_list = JsonSerializer.Deserialize<List<PlayerMaker>>(playerStringFromFile, playerOptions);

            if (basic_data != null && tempMap != null)
            {
                Map = tempMap;


                points = basic_data[0];
                coin = basic_data[1];
                ammo = basic_data[2];
                holyCross = basic_data[3];
                bomb = basic_data[4];
                key = basic_data[5];
                currentPlayer = basic_data[6];
                timer = basic_data[7];
                maxTimer = basic_data[8];
                difficulty[0] = basic_data[9];
                difficulty[1] = basic_data[10];
                difficulty[2] = basic_data[11];
                difficulty[3] = basic_data[12];
                difficulty[4] = basic_data[13];

                Canvas.SetLeft(Player, basic_data[14]);
                Canvas.SetTop(Player, basic_data[15]);
            }

            TitleSpace.Children.Clear();
            StartGame();
        }
    }


}