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

        // How this Function Works Part 1
        // MapMaker is used to grab what text file is needed to make the map.
        // It then splits it based on whenever there is a new line in the textfile
        // This then creates a new object called 'room' (using RoomMaker) and places this text into that 'room' object
        // This is then placed in the Dictionary that holds all the 'room' objects

        // How this Function Works Part 2
        // MakeMap will the draw this room based on what room the player is in
        // this will then split the text got by the 'room' object by each ',' letter
        // It will then push the words into a room List
        // It will then go row by row to create a tile based on the room

        // How this Function Works Part 3
        // FillMap is only for the tiles and going to anothe room.
        // When going to another room, instead of creating new tiles, it fills the current tiles with a new image, using similar logic to MakeMap()


        // Main Problem: This was before I learned about JSON saving and therefore it loading weirdlt
        // Plus then idea that it has to be split before then turning into the List... is a bit inefficent

        public void MakeMap()
        {
            // SET UP DATA 

            // Get the totalMap and the current room that the player is in. From that current room, get the room tiles within.
            String dictionary = Map.totalMap[Map.currentRoom].roomTiles;
            String[] dictionaryWords = Regex.Split(dictionary, @",+");

            // Create map. This will be used as the basis for the place where the player moves
            List<string> room = new List<string>();

            for (int i = 0; i < Map.totalMap[Map.currentRoom].size; i += 2)
            {
                room.Add(dictionaryWords[i]);

            }

            // SET UP ROOM

            int col = 0;
            int row = 0;
            int section = 20;
            int generateRoom = 0;

            int wallSize = 25;


            // Idea: Already create the rectangle floor, just set the FILL style of the floor instead of creating a new rectangle each time

            while (generateRoom < room.Count)
            {

                if (generateRoom < section) // If this part of the array is bellow the length of the map (20)
                {
                    if (room[generateRoom] == "1") // If the given part of the map has a 1
                    {
                        // Increase the row
                        row++;

                        wallStats.Add(new WallMaker(100, wallSize, (row * wallSize) + 55, (col * wallSize) + 80, PlayerSpace, wallStats.Count, "wall", "wall", "wall"));

                    }
                    else if (room[generateRoom] == "2")
                    {
                        // Increase the row
                        row++;

                        wallStats.Add(new WallMaker(100, wallSize, (row * wallSize) + 55, (col * wallSize) + 80, PlayerSpace, wallStats.Count, "dirt", "dirt", "dirt"));
                    }
                    else if (room[generateRoom] == "3")
                    {
                        // Increase the row
                        row++;

                        wallStats.Add(new WallMaker(100, wallSize, (row * wallSize) + 55, (col * wallSize) + 80, PlayerSpace, wallStats.Count, "wood", "wood", "wood"));
                    }
                    else if (room[generateRoom] == "WATER")
                    {
                        row++;

                        wallStats.Add(new WallMaker(100, wallSize, (row * wallSize) + 55, (col * wallSize) + 80, PlayerSpace, wallStats.Count, "water", "water", "water"));
                    }
                    else
                    {
                        row++; // If this part of the array is 0

                        wallStats.Add(new WallMaker(100, wallSize, (row * wallSize) + 55, (col * wallSize) + 80, PlayerSpace, wallStats.Count, "grass", "grass", "grass"));
                    }

                    generateRoom++; // Get to the next part of the array

                }
                else
                {
                    // When reading the array goes farther than the length of the map (10) switch to a new row
                    row = 0;
                    col++;
                    section += 20;
                }

            }

            MakeEnemyMap(dictionaryWords);

        }

        // Creates objects for the map for the player to interact with
        // Not just enemeis but every object as well
        public void MakeEnemyMap(String[] dictionaryWords)
        {
            List<string> room = new();
            List<string> wallRoom = new();

            for (int i = 1; i < Map.totalMap[Map.currentRoom].size; i += 2)
            {
                room.Add(dictionaryWords[i]);
            }

            for (int i = 0; i < Map.totalMap[Map.currentRoom].size; i += 2)
            {
                wallRoom.Add(dictionaryWords[i]);
            }

            int col = 0;
            int row = 0;
            int section = 20;
            int generateRoom = 0;

            int wallSize = 25;

            while (generateRoom < room.Count)
            {

                if (generateRoom < section) // If this part of the array is bellow the length of the map (20)
                {
                    if (room[generateRoom] == "ENEMY") // If the given part of the map has a 1
                    {
                        // Increase the row

                        row++;

                        double[] visionCone = {300, 300};

                        //_ = new Draw($"enemy-{enemyStats.Count}", Convert.ToInt16(wallSize), Convert.ToInt16(wallSize), (row * wallSize) + 55, (col * wallSize) + 80, "zombie", "zombie", ItemSpace);

                        enemyStats.Add(new EnemyMaker("zombie", "zombie", $"enemy-{enemyStats.Count - 1}", "zombie", "ZOMBIE", 100, 10, 100, 10, 1, 25, enemyWeaponList[0], 10, 10, 10, 10, 10, 2, 0, 20, 20, visionCone, false, row, col, enemyStats.Count, healthBarList, wallSize, new(10, 0, 0, 0, 0, 0, 20, 0, 0), bulletList[0], ItemSpace));


                    }
                    else if (room[generateRoom] == "SHOOTER")
                    {
                        // Increase the row

                        row++;

                        double[] visionCone = { 300, 300 };

                        enemyStats.Add(new EnemyMaker("ArmoredCore", "bot", $"enemy-{enemyStats.Count - 1}", "zombie", "SHOOTER", 200, 0, 200, 0, 0, 50, enemyWeaponList[0], 20, 0, 20, 50, 0, 3, 20, 20, 10, visionCone, true, row, col, enemyStats.Count, healthBarList, wallSize, new(30, 0, 0, 0, 0, 9, 20, 0, 0), bulletList[1], ItemSpace));
                    }
                    else if (room[generateRoom] == "FROG")
                    {
                        // Increase the row

                        row++;

                        double[] visionCone = { 500, 500 };


                        enemyStats.Add(new EnemyMaker("Frog", "frog", $"enemy-{enemyStats.Count - 1}", "frog", "ZOMBIE", 50, 0, 50, 0, 0, 25, enemyWeaponList[0], 30, 0, 0, 10, 0, 3, 0, 20, 30, visionCone, false, row, col, enemyStats.Count, healthBarList, wallSize, new(10, 0, 0, 0, 0, 0, 0, 0, 0), bulletList[0], ItemSpace));


                    }
                    else if (room[generateRoom] == "FIREBALL")
                    {
                        // Increase the row

                        row++;

                        double[] visionCone = { 200, 200 };

                        enemyStats.Add(new EnemyMaker("fireball", "fireball", $"enemy-{enemyStats.Count - 1}", "fireball", "ZOMBIE", 1000, 0, 1000, 0, 0, 50, enemyWeaponList[0], 50, 50, 50, 20, 20, 4, 0, 40, 30, visionCone, false, row, col, enemyStats.Count, healthBarList, wallSize, new(150, 0, 0, 0, 0, 0, 0, 0, 0), bulletList[0], ItemSpace));




                    }
                    else if (room[generateRoom] == "EYEBEAST")
                    {
                        // Increase the row

                        row++;

                        double[] visionCone = { 300, 300 };


                        enemyStats.Add(new EnemyMaker("Eye", "Eye", $"enemy-{enemyStats.Count - 1}", "Eye", "ZOMBIE", 1200, 0, 1200, 0, 0, 50, enemyWeaponList[0], 20, 0, 0, 0, 0, 4, 0, 0, 30, visionCone, false, row, col, enemyStats.Count, healthBarList, wallSize, new(200, 0, 0, 0, 0, 0, 0, 0, 0), bulletList[0], ItemSpace));


                    }
                    else if (room[generateRoom] == "SOLDIER")
                    {
                        // Increase the row

                        row++;

                        double[] visionCone = { 300, 300 };

                        enemyStats.Add(new EnemyMaker("soldier", "soldier", $"enemy-{enemyStats.Count - 1}", "soldier", "ZOMBIE", 200, 0, 20, 0, 20, 25, enemyWeaponList[0], 20, 20, 20, 20, 20, 3, 0, 40, 20, visionCone, true, row, col, enemyStats.Count, healthBarList, wallSize, new(10, 0, 0, 0, 0, 0, 0, 0, 0), bulletList[0], ItemSpace));



                    }
                    else if (room[generateRoom] == "SLIME")
                    {
                        // Increase the row

                        row++;

                        double[] visionCone = { 500, 500 };

                        enemyStats.Add(new EnemyMaker("slime", "slime", $"enemy-{enemyStats.Count - 1}", "slime", "ZOMBIE", 75, 0, 75, 0, 0, 25, enemyWeaponList[0], 30, 0, 0, 50, 0, 4, 0, 40, 30, visionCone, false, row, col, enemyStats.Count, healthBarList, wallSize, new(10, 0, 0, 0, 0, 0, 0, 0, 0), bulletList[0], ItemSpace));



                    }
                    else if (room[generateRoom] == "2")
                    {
                        // Increase the row
                        row++;

                        itemStats.Add(new ItemMaker(1, 25, 25, (row * wallSize) + 55, (col * wallSize) + 80, ItemSpace, itemStats.Count, "items/coin", "coin", "none", new(1, 0, 0, 0, 0, 0, 0, -1, -1)));
                    }
                    else if (room[generateRoom] == "3")
                    {
                        // Increase the row
                        row++;

                        itemStats.Add(new ItemMaker(1, 25, 25, (row * wallSize) + 55, (col * wallSize) + 80, ItemSpace, itemStats.Count, "items/ammo", "ammo", "none", new(0, 1, 0, 0, 0, 0, 0, -1, -1)));
                    }
                    else if (room[generateRoom] == "4")
                    {
                        // Increase the row
                        row++;

                        itemStats.Add(new ItemMaker(1, 25, 25, (row * wallSize) + 55, (col * wallSize) + 80, ItemSpace, itemStats.Count, "items/holy_cross", "holy_cross", "none", new(0, 0, 1, 0, 0, 0, 0, -1, -1)));
                    }
                    else if (room[generateRoom] == "5")
                    {
                        // Increase the row
                        row++;

                        itemStats.Add(new ItemMaker(1, 25, 25, (row * wallSize) + 55, (col * wallSize) + 80, ItemSpace, itemStats.Count, "items/key", "key", "none", new(0, 0, 0, 1, 0, 0, 0, -1, -1)));
                    }
                    else if (room[generateRoom] == "6")
                    {
                        // Increase the row
                        row++;

                        itemStats.Add(new ItemMaker(1, 25, 25, (row * wallSize) + 55, (col * wallSize) + 80, ItemSpace, itemStats.Count, "items/bomb", "bomb", "none", new(0, 0, 0, 0, 1, 0, 0, -1, -1)));
                    }
                    else if (room[generateRoom] == "W") // weapon crate
                    {
                        // Increase the row
                        row++;

                        Random rand = new Random();

                        int randomWeapon = rand.Next(0, weaponList.Count);

                        // Chnage this to a crate later on
                        itemStats.Add(new ItemMaker(1, 25, 25, (row * wallSize) + 55, (col * wallSize) + 80, ItemSpace, itemStats.Count, "items/crate", "crate", "crate", new(-1, 0, 0, 0, 0, 0, 0, randomWeapon, -1)));
                    }
                    else if (room[generateRoom] == "B") // boon crate
                    {
                        row++;

                        Random rand = new Random();

                        int randomBoon = rand.Next(0, boonList.Count);

                        // Chnage this to a crate later on
                        itemStats.Add(new ItemMaker(1, 25, 25, (row * wallSize) + 55, (col * wallSize) + 80, ItemSpace, itemStats.Count, "items/crate2", "crate", "crate2", new(-1, 0, 0, 0, 0, 0, 0, -1, randomBoon)));
                    }
                    else if (room[generateRoom] == "H") // heal crate
                    {
                        row++;

                        Random rand = new Random();

                        int randomHealth = rand.Next(50, 100);

                        // Chnage this to a crate later on
                        itemStats.Add(new ItemMaker(1, 25, 25, (row * wallSize) + 55, (col * wallSize) + 80, ItemSpace, itemStats.Count, "items/crate3", "crate", "crate3", new(-1, 0, 0, 0, 0, randomHealth, 0, -1, -1)));
                    }
                    else if (room[generateRoom] == "WALL")
                    {
                        row++;

                        itemStats.Add(new ItemMaker(1, 25, 25, (row * wallSize) + 55, (col * wallSize) + 80, ItemSpace, itemStats.Count, "wall", "fakewall", "wall", new(0, 0, 0, 0, 0, 0, 0, -1, -1)));
                    }
                    else if (room[generateRoom] == "DOORUP")
                    {
                        row++;

                        itemStats.Add(new ItemMaker(1, 50, 25, (row * wallSize) + 55, (col * wallSize) + 80, ItemSpace, itemStats.Count, "items/door", "door", "door", new(0, 0, 0, 0, 0, 0, 0, -1, -1)));
                    }
                    else if (room[generateRoom] == "DOORSIDE")
                    {
                        row++;

                        itemStats.Add(new ItemMaker(1, 25, 50, (row * wallSize) + 55, (col * wallSize) + 80, ItemSpace, itemStats.Count, "items/door", "door", "door", new(0, 0, 0, 0, 0, 0, 0, -1, -1)));
                    }
                    else if (room[generateRoom] == "STATUE")
                    {
                        row++;

                        itemStats.Add(new ItemMaker(1, 50, 25, (row * wallSize) + 55, (col * wallSize) + 80, ItemSpace, itemStats.Count, "items/statue", "statue", "statue", new(0, 0, 0, 0, 0, 0, 0, -1, -1)));
                    }
                    else if (room[generateRoom] == "TABLE")
                    {
                        row++;

                        itemStats.Add(new ItemMaker(1, 25, 25, (row * wallSize) + 55, (col * wallSize) + 80, ItemSpace, itemStats.Count, "items/table", "table", "table", new(0, 0, 0, 0, 0, 0, 0, -1, -1)));
                    }
                    else if (room[generateRoom] == "CHAIR")
                    {
                        row++;

                        itemStats.Add(new ItemMaker(1, 25, 25, (row * wallSize) + 55, (col * wallSize) + 80, ItemSpace, itemStats.Count, "items/chair", "chair", "chair", new(0, 0, 0, 0, 0, 0, 0, -1, -1)));
                    }
                    else if (room[generateRoom] == "EXIT")
                    {
                        row++;

                        itemStats.Add(new ItemMaker(1, 25, 50, (row * wallSize) + 55, (col * wallSize) + 80, ItemSpace, itemStats.Count, "items/exit", "exit", "exit", new(0, 0, 0, 0, 0, 0, 0, -1, -1)));
                    }
                    else if (room[generateRoom] == "8")
                    {
                        // Increase the row
                        row++;

                        Random rand = new Random();

                        int randomNumber = rand.Next(0, 100);

                        if (randomNumber < 50)
                        {
                            itemStats.Add(new ItemMaker(1, 25, 25, (row * wallSize) + 55, (col * wallSize) + 80, ItemSpace, itemStats.Count, "items/coin", "coin", "none", new(1, 0, 0, 0, 0, 0, 0, -1, -1)));

                            room[generateRoom] = "2";
                        }
                        else if (randomNumber < 95)
                        {
                            itemStats.Add(new ItemMaker(1, 25, 25, (row * wallSize) + 55, (col * wallSize) + 80, ItemSpace, itemStats.Count, "items/ammo", "ammo", "none", new(0, 1, 0, 0, 0, 0, 0, -1, -1)));

                            room[generateRoom] = "3";
                        }
                        else if (randomNumber < 98)
                        {
                            itemStats.Add(new ItemMaker(1, 25, 25, (row * wallSize) + 55, (col * wallSize) + 80, ItemSpace, itemStats.Count, "items/bomb", "bomb", "none", new(0, 0, 0, 0, 0, 1, 0, -1, -1)));

                            room[generateRoom] = "6";
                        }
                        else if (randomNumber < 100)
                        {
                            itemStats.Add(new ItemMaker(1, 25, 25, (row * wallSize) + 55, (col * wallSize) + 80, ItemSpace, itemStats.Count, "items/holy_cross", "holy_cross", "none", new(0, 0, 1, 0, 0, 0, 0, -1, -1)));

                            room[generateRoom] = "4";
                        }

                    }
                    else
                    {
                        row++; // If this part of the array is 0
                    }

                    generateRoom++; // Get to the next part of the array

                }
                else
                {
                    // When reading the array goes farther than the length of the map (10) switch to a new row
                    row = 0;
                    col++;
                    section += 20;
                }

            }

            string totalRoom = "";

            for (int i = 0; i < Map.totalMap[Map.currentRoom].size / 2; i += 1)
            {
                totalRoom += wallRoom[i] + ",";
                totalRoom += room[i] + ",";
            }


            Map.totalMap[Map.currentRoom].roomTiles = totalRoom;
        }


        // Find a room on the mini map
        public void FindRoomOnMap()
        {
            foreach (Rectangle x in MiniMapCanvas.Children.OfType<Rectangle>())
            {

                if (x.Name == $"map{Map.currentRoom}")
                {
                    _ = new FillDraw("grass", x);
                }

                if (x.Name == $"token{Map.currentRoom}")
                {
                    _ = new FillDraw("sword", x);
                }

            }

        }

        // Move the player across the minimap
        public void RemovePlayerSymbol()
        {
            foreach (Rectangle x in MiniMapCanvas.Children.OfType<Rectangle>())
            {
                if (x.Name == $"token{Map.currentRoom}")
                {
                    _ = new FillDraw("blank", x);
                }
            }

        }


        // Create the mini map
        public void CreateMiniMap()
        {


            List<int> room = new List<int>();


            for (int i = 0; i < 100; i++)
            {
                room.Add(i);
            }


            int col = 0;
            int row = 0;
            int section = 10;
            int generateRoom = 0;

            int wallSize = 20;



            while (generateRoom < room.Count)
            {

                if (generateRoom < section) // If this part of the array is bellow the length of the map (20)
                {
                    row++;

                    // Fog of War
                    listStats.Add(new WallMaker(100, wallSize, (row * wallSize) - wallSize, (col * wallSize), MiniMapCanvas, listStats.Count, "ruby", $"map{listStats.Count}", "wall"));

                    // Hidden Stats
                    hiddenStats.Add(new WallMaker(100, wallSize, (row * wallSize) - wallSize, (col * wallSize), MiniMapCanvas, hiddenStats.Count, "blank", $"token{hiddenStats.Count}", "wall"));

                    generateRoom++; // Get to the next part of the array

                }
                else
                {
                    // When reading the array goes farther than the length of the map (10) switch to a new row
                    row = 0;
                    col++;
                    section += 10;
                }

            }
        }


        // Fill the map
        public void FillMap()
        {
            // SET UP DATA 

            // Get the totalMap and the current room that the player is in. From that current room, get the room tiles within.
            String dictionary = Map.totalMap[Map.currentRoom].roomTiles;
            String[] dictionaryWords = Regex.Split(dictionary, @",+");

            // Create map. This will be used as the basis for the place where the player moves
            List<string> room = new();

            // Set this up for the Foreach loop
            int y = 0;

            // Add these tiles to the current room
            for (int i = 0; i < Map.totalMap[Map.currentRoom].size; i += 2)
            {
                room.Add(dictionaryWords[i]);
            }

            // Change object to fill
            FillingAllSections(PlayerSpace, room, y);

            // Create the enemy map
            MakeEnemyMap(dictionaryWords);


        }

        // FIllinf the entire map
        public void FillingAllSections(Canvas ThisCanvas, List<string> room, int y)
        {

            foreach (Rectangle x in ThisCanvas.Children.OfType<Rectangle>())
            {

                // Well.. placing the object here somehow fixes everything????
                ImageBrush image = new();

                // Check for only rectangles that are walls
                if ((string)x.Tag == $"wall" || (string)x.Tag == $"grass" || (string)x.Tag == $"dirt" || (string)x.Tag == $"wood" || (string)x.Tag == $"water")
                {
                    string fileName = "";

                    if (room[y] == "1")
                    {
                        fileName = $"data-files/images/wall.png";
                        x.Tag = "wall";
                    }
                    else if (room[y] == "2")
                    {
                        fileName = $"data-files/images/dirt.png";
                        x.Tag = "dirt";

                    }
                    else if (room[y] == "0")
                    {
                        fileName = $"data-files/images/grass.png";
                        x.Tag = "grass";

                    }
                    else if (room[y] == "3")
                    {
                        fileName = $"data-files/images/wood.png";
                        x.Tag = "wood";

                    }
                    else if (room[y] == "WATER")
                    {
                        fileName = $"data-files/images/water.png";
                        x.Tag = "water";

                    }

                    string fullPath = System.IO.Path.GetFullPath(fileName);
                    image.ImageSource = new BitmapImage(new Uri(fullPath));
                    x.Fill = image;

                    y++;

                }




            }

        }





        // Delete all enemies when using holy cross
        // Honestly there are so many enemies that it's going to be  a pain to include everyone
        public void DeleteALlEnemies()
        {

            string replace = "ENEMY";

            Map.totalMap[Map.currentRoom].roomTiles = Map.totalMap[Map.currentRoom].roomTiles.Replace(replace, "0");

            string newReplace = "SHOOTER";

            Map.totalMap[Map.currentRoom].roomTiles = Map.totalMap[Map.currentRoom].roomTiles.Replace(newReplace, "0");


        }


        // Save the map
        public void SaveMap(Rectangle c, string selection)
        {
            // Get the totalMap and the current room that the player is in. From that current room, get the room tiles within.
            String dictionary = Map.totalMap[Map.currentRoom].roomTiles;
            String[] dictionaryWords = Regex.Split(dictionary, @",+");

            // Create map. This will be used as the basis for the place where the player moves
            List<string> room = new();
            List<string> wallRoom = new();

            for (int i = 1; i < Map.totalMap[Map.currentRoom].size; i += 2)
            {
                room.Add(dictionaryWords[i]);
            }

            for (int i = 0; i < Map.totalMap[Map.currentRoom].size; i += 2)
            {
                wallRoom.Add(dictionaryWords[i]);
            }

            // extra split

            string totalRoom = "";


            // How This works: it gets the top of the object and the left ot it. Subtracting both by 80 (since that is how far the screen is from the border) to measure it based on the monitor thing. Afterwards, it will then turn this into what tile it needs to find. The top and left of the object is dividied by 25 to convert this to the actual positon on the grid (to get x and y). The y is times by twenty to repersent it looking down (since twenty is how long a row is) and x decides what collum it will select. This is how the object is found.


            //MessageBox.Show($"Top:{Canvas.GetTop(c) - 80} Left:{Canvas.GetLeft(c) - 80} + Top Edited: {((Canvas.GetTop(c) - 80) / 25)} Left Edited: {((Canvas.GetLeft(c) - 80) / 25)}");

            //MessageBox.Show(((((Canvas.GetTop(c) - 80) / 25) * 20) + ((Canvas.GetLeft(c) - 80) / 25)).ToString());


            room[Convert.ToInt16((((Canvas.GetTop(c) - 80) / 25) * 20) + ((Canvas.GetLeft(c) - 80) / 25))] = selection;




            for (int i = 0; i < Map.totalMap[Map.currentRoom].size / 2; i += 1)
            {
                totalRoom += wallRoom[i] + ",";
                totalRoom += room[i] + ",";
            }


            Map.totalMap[Map.currentRoom].roomTiles = totalRoom;


        }


    }

    // Map Maker Class
    public class MapMaker
    {
        public String? totalMapText = null;
        public String? totalRoomText = null;
        public String[]? totalMapSplit = null;
        public String[]? totalRoomSplit = null;


        public int currentRoom;
        public int mapLength, mapType;
        public Dictionary<int, RoomMaker> totalMap = new();

        public MapMaker(int mapLength, int mapType)
        {

            this.mapLength = mapLength;
            this.mapType = mapType;

            if (mapType == 1)
            {
                this.currentRoom = 24;
                
                totalMapText = System.IO.File.ReadAllText(@"data-files/map/tutorial.txt");
                totalRoomText = System.IO.File.ReadAllText(@"data-files/map/tutorialSettings.txt");
            }
            else if (mapType == 0)
            {
                this.currentRoom = 44;
                totalMapText = System.IO.File.ReadAllText(@"data-files/map/map1.txt");
                totalRoomText = System.IO.File.ReadAllText(@"data-files/map/map1Settings.txt");
            }

            totalMapSplit = Regex.Split(totalMapText, @"\r\n+");
            totalRoomSplit = Regex.Split(totalRoomText, @"\r\n+");


            for (int i = 0; i < mapLength; i++)
            {
                var RoomMaker = new RoomMaker(800, totalRoomSplit[i], totalMapSplit[i]);
                totalMap.Add(i, RoomMaker);
            }
        }
    }

    // Room Maker Class
    public class RoomMaker
    {
        public int size = 0;
        public string roomType = "";
        public string roomTiles;

        public RoomMaker(int size, string roomType, string roomTiles)
        {
            this.size = size;
            this.roomType = roomType;
            this.roomTiles = roomTiles;
        }


        // Activate the room if it is special
        public void ActivateRoom(TextBlock LogBox, ScrollViewer ScrollBar, Rectangle Player, Action MakeArmPusherBoss)
        {
            if (roomType == "none") // basic room type
            {
                // Does Nothing

            } else if (roomType == "[trap]") // trap
            {
                roomType = "none";

                LogBox.Text += $"It's a trap!!! \n";

                ScrollBar.ScrollToEnd();

                Canvas.SetLeft(Player, Canvas.GetLeft(Player) + 200);
            } else if (roomType == "[exit1]") // get to the exit
            {
                LogBox.Text += $"Exit Tutorial! \n";

                ScrollBar.ScrollToEnd();
            }
            else if (roomType == "[boss]") // final boss
            {
                LogBox.Text += $"The Final Boss! \n";

                ScrollBar.ScrollToEnd();

                Canvas.SetTop(Player, Canvas.GetTop(Player) - 100);


                MakeArmPusherBoss();
            }
            else // Used to write special text
            {
                LogBox.Text += $"{roomType} \n";

                ScrollBar.ScrollToEnd();
            }
        }

    }


}