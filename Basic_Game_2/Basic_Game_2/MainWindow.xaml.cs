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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// 
    /// 
    /// The Tag System doonmed me
    /// At first the major thing is searching through the canvas for a rectnagle with the same tag and then pair that with the object in the array
    /// However this is inefficent and I found that can push the rectangle onto the object in the array
    /// However this is not a full solution since many of the functions require searching through the canvas to find and object. There is jank between remvoing object on canvas vs list
    /// Therefore, unless I want to rewrite everything (AGAIN) I will just stick with this most of the time
    /// 
    /// </summary>
    public partial class MainWindow : Window
    {

        // Create Player Stats
        public List<PlayerMaker> playerList = new();

        // Create List of all Weapon
        public List<WeaponMaker> weaponList = new();
        public List<WeaponMaker> enemyWeaponList = new();

        // Create A Map, this is really important
        // it gets the amount of rooms in the map and what type of map it is
        // based on the type of map, it will select which textfile to draw the map from
        // it then creates each room based on each line of the textfile
        // makes a room object out of the text from the textfile
        public MapMaker Map = new(100, 0);

        // Points
        public int points = 0;
        public int coin = 5;
        public double ammo = 20;
        public int holyCross = 1;
        public int bomb = 10;
        public int key = 0;

        // Bullet ID
        public int bulletNum = 0;

        // TImer
        public int timer = 0;
        public int maxTimer = 2000;
        public int[] difficulty = { 0, 0, 0, 0, 0 };

        // List of Boons and Curses
        public List<BoonMaker> boonList = new();

        // A list that stores all the current walls
        public List<WallMaker> wallStats = new();

        public List<ProgressBar> healthBarList = new();

        // List of MiniMap
        public List<WallMaker> listStats = new();
        public List<WallMaker> hiddenStats = new();

        // List of enemies
        public List<EnemyMaker> enemyList = new();

        // A list that stores all the current enemies
        public List<ItemMaker> itemStats = new();

        // Making the list of rectangles to be stored and then removed
        public List<Rectangle> itemstoremove = new();
        public List<ProgressBar> progressstoremove = new();
        public List<BulletMaker> bulletstoreremove = new();

        // Global Dispatcher Timer
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        // Public Bomb List
        public List<BombMaker> bombList = new();

        // Explosion List
        public List<ExplosionMaker> explosionList = new();

        // All the classes
        public List<ClassMaker> classMakerList = new();

        // Making the Weapon Class
        public WeaponCreate weapon = new();

        // Making list of bullets
        public List<BulletMaker> bulletList = new();
        // List of bullet fired
        public List<BulletMaker> bulletFired = new();

        // List of enemies on map
        public List<EnemyMaker> enemyStats = new();


        // Making the UI class
        public List<PlayerUi> uiList = new();

        // Player Selector
        public int currentPlayer = 0;

        // If a weapon is created or not. As well as setting up the frames of how long the sword will last
        public bool weaponCreated = false;
        public int frame = 0;

        public int invisibilityFrame = 0;
        public bool PlayerIsHit = false;

        // The Stats that involve the current player dirrection and the 'old' dirrection of where the player faced before activating something
        public string oldDirrection = "none";


        // When the Game Starts
        public MainWindow()
        {

            InitializeComponent();

            classSelect.SelectedIndex = 0;

            LoadBullets();
            LoadWeapons();
            LoadClasses();
            LoadEnemyWeapons();
            LoadBoons();
        }




        // This is mainly used to run the game code
        private void GameTimerEvent(object? sender, EventArgs e)
        {
            // Increase Dificulty Timer
            timer++;

            // Check Timer
            CheckTimer();

            // Create the hitbox around the player
            // This is after switch player so that the size of the player can change and the hitbox can change
            var PlayerHitbox = new Rect(Canvas.GetLeft(Player), Canvas.GetTop(Player), Player.Width, Player.Height);

            // Switch Player
            currentPlayer = playerList[currentPlayer].playerSwitcher(currentPlayer, Player, playerList, PlayerIsHit);

            // Move the Player And get the current dirrection of the Player
            playerList[currentPlayer].basic_movement(Player, Player, $"PLAYER", PlayerSpace, ItemSpace);


            // Attack Using Weapon
            // Activating the Function SwordAttack() -> Create the Weapon and set weapon create to true CreateWeapon() -> sword follows player until it runs out of time AddRemoveSword
            SwordAttack(playerList[currentPlayer].weapon, playerList[currentPlayer]);

            // Drop Bomb
            PlaceBomb();

            // Use Holycross
            UseHolyCross();

            // item interact
            if (itemStats.Count > 0)
            {
                itemInteract(PlayerHitbox, playerList[currentPlayer].weapon, playerList[currentPlayer].playerBoons);

            }

            // Explode all bombs
            if (explosionList.Count > 0)
            {
                Explosion();
                BombExpload(PlayerHitbox);

            }

            // If a boss exist
            if (HandMakerList.Count > 0)
            {
                ArmPusherBossLogic(PlayerHitbox);
            }

            // move enemies
            EnemyMoverAndAttacker(PlayerHitbox);


            // If player is hit run the invisibility timer
            if (PlayerIsHit)
            {
                invisibilityFrame++;
            }

            // Move bullet
            MoveBullet();
            AmmoInteract(PlayerHitbox);


            // move to next room
            playerList[currentPlayer].NextRoom(Player, Map, FillMap, transportNextRoom, FindRoomOnMap, RemovePlayerSymbol, LogBox, ScrollBar, enemyStats, healthBarList, bulletFired, bombList, explosionList, MakeArmPusherBoss);


            currentPlayer = playerList[currentPlayer].PlayerUiSelect(currentPlayer, PlayerUiBox, playerList);


            // Remove all rectangles that are trashed
            foreach (Rectangle x in itemstoremove)
            {
                // remove them permanently from the canvas
                PlayerSpace.Children.Remove(x);
                BulletCanvas.Children.Remove(x);
                ItemSpace.Children.Remove(x);

            }

            foreach (BulletMaker x in bulletstoreremove)
            {
                bulletFired.Remove(x);
            }


            foreach (ProgressBar x in progressstoremove)
            {
                // remove them permanently from the canvas
                ItemSpace.Children.Remove(x);

            }


            // Regenerate Mp if bellow Max
            MpGain();

            // Check the Dead
            CheckIfAlldead();

            // Delete Weapon if dead
            // This really doesn't need to run every frame
            DeleteWeaponIfDead();

            // Save the game
            SaveGame();
        }






        // Delete weapon of enemy if they are dead
        // This really doesn't need to be run every frame
        public void DeleteWeaponIfDead()
        {

            for (int i = 0; i < enemyStats.Count; i++)
            {

                if (enemyStats[i].health <= 0)
                {
                    foreach (Rectangle x in ItemSpace.Children.OfType<Rectangle>())
                    {
                        if ((string)x.Tag == $"enemymelee-{i}" || (string)x.Tag == $"enemyranged-{i}")
                        {
                            itemstoremove.Add(x);
                        }
                    }

                }

            }

        }

        // Move Enemy
        public void EnemyMoverAndAttacker(Rect PlayerHitbox)
        {
            if (enemyStats.Count > 0)
            {

                // Move Enemy
                EnemyMovement(PlayerHitbox);

                // Attack Enemy / Player
                PlayerAttackEnemy(PlayerHitbox);


                // Enemy Create Weapon or Not
                for (int i = 0; i < enemyStats.Count; i++)
                {

                    // Enemy reaction time to attack
                    // If the enemy can attack and weapon is not created and is not ranged
                    if (enemyStats[i].AllowWeapon == true && enemyStats[i].WeaponCreated == false && !enemyStats[i].ranged)
                    {
                        // Reaction Time for Enemy
                        enemyStats[i].reaction++;

                        // If the reaction time is complete
                        if (enemyStats[i].reaction > enemyStats[i].reactionTime)
                        {

                            // Complete Enemy Weapon
                            enemyStats[i].WeaponCreated = weapon.CreateWeapon(enemyStats[i].currentDirrection, ItemSpace, $"enemymelee-{enemyStats[i].thisCount}", enemyStats[i].weapon);

                            // Set reaction back to 0
                            enemyStats[i].reaction = 0;

                        }

                    }

                    foreach (Rectangle x in ItemSpace.Children.OfType<Rectangle>())
                    {

                        // if the weapon is from an enemy
                        if ((string)x.Tag == $"enemymelee-{i}" || (string)x.Tag == $"enemyranged-{i}")
                        {

                            // Get Enemy
                            if ((string)enemyStats[i].self.Tag == $"enemy-{i}")
                            {

                                // Follow Enemy
                                weapon.FollowPlayer(x, enemyStats[i].self, enemyStats[i].WeaponCreated, enemyStats[i].currentDirrection);

                                // If the fire rate is complete then delete it
                                if (enemyStats[i].firerate < enemyStats[i].innerFrame)
                                {
                                    enemyStats[i].innerFrame = 0;
                                    enemyStats[i].AllowWeapon = false;
                                    itemstoremove.Add(x);
                                    enemyStats[i].WeaponCreated = false;
                                }
                                else
                                {
                                    enemyStats[i].innerFrame++;
                                }
                            }



                        }
                    }








                }
            }

        }


        // Attack Eachother
        public void PlayerAttackEnemy(Rect PlayerHitbox)
        {


            foreach (Rectangle x in ItemSpace.Children.OfType<Rectangle>())
            {
                for (int i = 0; i < enemyStats.Count; i++)
                {
                    if ((string)x.Tag == $"enemy-{i}")
                    {
                        var Enemy = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                        // Check if player takes damage
                        PlayerTakeDamage(Enemy, PlayerHitbox, i, x);


                        // Check if the enemy weapon hits player
                        foreach (Rectangle y in ItemSpace.Children.OfType<Rectangle>())
                        {
                            if ((string)y.Tag == $"enemymelee-{i}" || (string)y.Tag == $"enemyranged-{i}")
                            {
                                var Weapon = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);


                                PlayerTakeDamage(Weapon, PlayerHitbox, i, x);


                            }


                            // Check the player weapon
                            if ((string)y.Tag == "melee" || (string)y.Tag == "ranged")
                            {
                                var Weapon = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                                // if the weapon hits the enemy
                                if (Weapon.IntersectsWith(Enemy))
                                {

                                    // Calculate damage delt to enemy
                                    currentPlayer = enemyStats[i].calculateDamage(playerList[currentPlayer], x, oldDirrection, LogBox, UpdateUi, ScrollBar, healthBarList[i], PlayerSpace, ItemSpace, currentPlayer, PlayerUiBox, enemyStats, i);

                                    // Check if enemy is dead
                                    enemyStats[i].checkIfDead(itemstoremove, progressstoremove, x, healthBarList[i], ItemSpace, i);
                                }

                            }

                        }
                    }
                }

            }
        }


        // If the enemy touches the player and invisibility frames are over
        public void PlayerTakeDamage(Rect ThisAttack, Rect PlayerHitbox, int enemyTarget, Rectangle MoveRectangle)
        {

            if (invisibilityFrame > playerList[currentPlayer].invisibiltyFrames)
            {
                invisibilityFrame = 0;
                PlayerIsHit = false;
            }
            if (ThisAttack.IntersectsWith(PlayerHitbox) && PlayerIsHit == false)
            {
                PlayerIsHit = true;
                currentPlayer = playerList[currentPlayer].damaged(enemyStats[enemyTarget], LogBox, UpdateUi, ScrollBar, PlayerUiBox, healthBarList[0], currentPlayer);
                playerList[currentPlayer].KnockBack(PlayerSpace, ItemSpace, Player, enemyStats[enemyTarget].currentDirrection, enemyStats[enemyTarget].knockback);
                currentPlayer = playerList[currentPlayer].CheckIfDead(currentPlayer, PlayerUiBox, enemyStats[enemyTarget].name, LogBox, ScrollBar, UpdateUi);


            }
        }




        // thisis it!
        public void AmmoInteract(Rect PlayerHitbox)
        {



            // Check for enemy and check if bullet deals damage to them
            foreach (Rectangle x in ItemSpace.Children.OfType<Rectangle>())
            {
                for (int i = 0; i < enemyStats.Count; i++)
                {
                    if ((string)x.Tag == $"enemy-{i}")
                    {
                        var Enemy = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                        foreach (BulletMaker y in bulletFired)
                        {
                            foreach (Rectangle w in BulletCanvas.Children.OfType<Rectangle>())
                            {
                                if ((y.tag == (string)w.Tag))
                                {
                                    var Weapon = new Rect(Canvas.GetLeft(w), Canvas.GetTop(w), w.Width, w.Height);

                                    if (Weapon.IntersectsWith(PlayerHitbox))
                                    {
                                        itemstoremove.Add(w);
                                        PlayerTakeDamage(Weapon, PlayerHitbox, i, x);
                                    }
                                }
                            }


                        }
                    }

                }

                // if the bullet hits a wall
                if ((string)x.Tag == "wall" || (string)x.Tag == "water" || x.Name == "crate" || x.Name == "crate2" || x.Name == "crate3" || x.Name == "door" || x.Name == "fakewall" || x.Name == "statue" || x.Name == "table" || x.Name == "chair")
                {
                    var Wall = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    foreach (Rectangle y in PlayerSpace.Children.OfType<Rectangle>())
                    {
                        for (int w = 0; w < bulletFired.Count; w++)
                        {
                            if ((string)y.Tag == $"enemybullet-{w}-up" || (string)y.Tag == $"enemybullet-{w}-down" || (string)y.Tag == $"enemybullet-{w}-left" || (string)y.Tag == $"enemybullet-{w}-right" || (string)y.Tag == $"bullet-{w}-up" || (string)y.Tag == $"bullet-{w}-down" || (string)y.Tag == $"bullet-{w}-left" || (string)y.Tag == $"bullet-{w}-right")
                            {

                                var Bullet = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                                if ((Bullet).IntersectsWith(Wall))
                                {
                                    itemstoremove.Add(y);
                                }


                            }

                        }
                    }

                }




            }

        }


        // Check if all players are dead
        public void CheckIfAlldead()
        {

            if (playerList[0].health <= 0 && playerList[1].health <= 0 && playerList[2].health <= 0 && playerList[3].health <= 0)
            {
                MessageBox.Show("GAME OVER!");
                MessageBox.Show($"{playerList[0].health} {playerList[1].health} {playerList[2].health} {playerList[3].health}");
                dispatcherTimer.Stop();

                EndGame();
            }
        }

        // Transport to next room
        public void transportNextRoom()
        {

            BulletCanvas.Children.Clear();
            ItemSpace.Children.Clear();

            foreach (ProgressBar x in ItemSpace.Children.OfType<ProgressBar>())
            {
                progressstoremove.Add(x);
            }

        }


        // gain mp based on mp gained
        public void MpGain()
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i].mp < playerList[i].mpMax)
                {
                    playerList[i].mp += playerList[i].mpRegen;

                    UpdateUi();
                }
            }

        }



        // Apply boon to player
        public void ApplyBoon(BoonMaker NewBoon)
        {

            // fix this later
            var x = NewBoon;

            // Apply stat changes
            playerList[currentPlayer].health += x.healthCost;
            playerList[currentPlayer].mp += x.mpCost;
            playerList[currentPlayer].healthMax += x.healthMaxCost;
            playerList[currentPlayer].mpMax += x.mpMaxCost;

            playerList[currentPlayer].phys += x.physCost;
            playerList[currentPlayer].magic += x.magicCost;
            playerList[currentPlayer].gun += x.physCost;
            playerList[currentPlayer].physDef += x.physDefCost;
            playerList[currentPlayer].magDef += x.magDefCost;
            playerList[currentPlayer].speed += x.speedCost;
            playerList[currentPlayer].mpRegen += x.mpRegenCost;
            playerList[currentPlayer].size += x.sizeCost;
            playerList[currentPlayer].invisibiltyFrames += x.invisibiltyFrames;


        }


        // if the health is above the max health, set health to max health
        public void SetHealthToMax(int select)
        {
            if (playerList[select].health > playerList[select].healthMax)
            {
                playerList[select].health = playerList[select].healthMax;
            }
        }



        // Interact with Items
        public void itemInteract(Rect PlayerHitBox, WeaponMaker CurrentWeapon, List<BoonMaker> CurrentBoon)
        {
            bool exit = false;

            foreach (Rectangle x in ItemSpace.Children.OfType<Rectangle>())
            {

                for (int i = 0; i < itemStats.Count; i++)
                {
                    if ((string)x.Tag == $"item-{i}")
                    {


                        // increases player hit box so that the player doesn't actually have to touch the item and might go in it
                        Rect item = new Rect(Canvas.GetLeft(x) - 10, Canvas.GetTop(x) - 10, x.Width + 20, x.Height + 20);


                        // If the newly expanded hitbox interacts with items
                        if (PlayerHitBox.IntersectsWith(item))
                        {


                            // If this is a chest
                            if (itemStats[i].isChest == "none") // No
                            {

                                // Delete Item
                                itemStats[i].takeDamage(100);

                                // Save this change
                                SaveMap(x, "0");

                                // Give item to player
                                GiveItems(i);
                            }
                            else if (itemStats[i].isChest == "door" && Keyboard.IsKeyDown(Key.Enter)) // Is Door
                            {

                                // if there is enough keys
                                if (key > 0)
                                {

                                    // Subtract keys
                                    key -= 1;

                                    // Delete Item
                                    itemStats[i].takeDamage(100);

                                    // Save this change
                                    SaveMap(x, "0");

                                    // Give items
                                    GiveItems(i);

                                }

                            }
                            else if (itemStats[i].isChest == "crate" && Keyboard.IsKeyDown(Key.Enter)) // is a weapon crate
                            {
                                WeaponChestOpen(i, x, CurrentWeapon);
                            }
                            else if (itemStats[i].isChest == "crate2" && Keyboard.IsKeyDown(Key.Enter)) // is a boon crate 
                            {
                                BoonChestOpen(i, x, CurrentBoon);
                            }
                            else if (itemStats[i].isChest == "crate3" && Keyboard.IsKeyDown(Key.Enter)) // is a health crate
                            {
                                HealthChestOpen(i, x);
                            }
                            else if (itemStats[i].isChest == "exit" && Keyboard.IsKeyDown(Key.Enter)) // is an exit
                            {
                                exit = true;
                            }


                            // Check if the item was used
                            itemStats[i].checkIfDead(itemstoremove, x);

                        }

                    }
                }
            }

            // If the exit is used, exit the map (not room, but to an entirely new map)
            if (exit)
            {
                ExitTheMap();
            }

        }

        // Exit to a new map
        public void ExitTheMap()
        {
            // Clear All Canvases
            ItemSpace.Children.Clear();
            BulletCanvas.Children.Clear();
            MiniMapCanvas.Children.Clear();

            listStats.Clear();
            hiddenStats.Clear();



            if (Map.totalMap[Map.currentRoom].roomType == "[exit1]")
            {
                // Create New Map
                Map = new(100, 1);
            }



            // Fill Said Map
            FillMap();

            // Set Up Map
            CreateMiniMap();

            // Set up clear fog of war on map
            FindRoomOnMap();



            // Activate Special Room features
            Map.totalMap[Map.currentRoom].ActivateRoom(LogBox, ScrollBar, Player, MakeArmPusherBoss);
        }



        // If the health chest is opem
        public void HealthChestOpen(int i, Rectangle x)
        {
            if (MessageBox.Show($"Do you really want to open the health box? Only the current player you're selecting will get it and that player will get a random set of health.", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {

                // Increase Health
                playerList[currentPlayer].health += itemStats[i].dropItem.healthAmount;

                // Give these items
                GiveItems(i);

                // Delete this item
                itemStats[i].takeDamage(100);

                // Save this
                SaveMap(x, "0");

                // Show that health increased
                LogBox.Text += $"{playerList[currentPlayer].name} got {itemStats[i].dropItem.healthAmount} health! \n";

                // Update the UI to show this
                UpdateUi();
                ScrollBar.ScrollToEnd();


            }
        }


        // if boon chest is opened
        public void BoonChestOpen(int i, Rectangle x, List<BoonMaker> CurrentBoon)
        {
            if (MessageBox.Show($"Do you really want to open the boon box? Only the current player you're selecting will get it and you will only have once chance to accept or get rid of it! A boon cannot be removed. You will also have to pay {itemStats[i].dropItem.coinAmount * -1} coins and you have {coin} coins!", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {

                // Get a new boon
                BoonMaker NewBoon = boonList[itemStats[i].dropItem.thisBoon];

                if (coin >= itemStats[i].dropItem.coinAmount * -1)
                {
                    // This looses coins
                    GiveItems(i);

                    // Delete chest
                    itemStats[i].takeDamage(100);

                    // Show this change
                    SaveMap(x, "0");


                    if (MessageBox.Show($"Do want this boon? \n {NewBoon.boonName} \n {NewBoon.boonType} \n {NewBoon.boonDescription}", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {

                        // Add to current boon
                        CurrentBoon.Add(NewBoon);

                        // Apply Boon Stats
                        ApplyBoon(NewBoon);

                        // Change Text
                        LogBox.Text += $"{playerList[currentPlayer].name} got a {NewBoon.boonName} \n";

                        // Change UI
                        UpdateUi();
                        ScrollBar.ScrollToEnd();

                    }
                }
                else
                {
                    MessageBox.Show($"You don't have enough coins! You only have {coin} and the chest requires {itemStats[i].dropItem.coinAmount}!");
                }

            }
        }

        // if the weapon chest is opened
        public void WeaponChestOpen(int i, Rectangle x, WeaponMaker CurrentWeapon)
        {
            if (MessageBox.Show($"Do you really want to open the weapon box? Only the current player you're selecting will get it and you will only have once chance to accept or get rid of it! You will also have to pay {itemStats[i].dropItem.coinAmount * -1} coins and you have {coin} coins!", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (coin >= itemStats[i].dropItem.coinAmount * -1) // if player has enough coins
                {

                    // This looses coins
                    GiveItems(i);

                    itemStats[i].takeDamage(100);
                    SaveMap(x, "0");

                    // get weapon based on dropped item
                    WeaponMaker NewWeapon = new(weaponList[itemStats[i].dropItem.weapon].name, weaponList[itemStats[i].dropItem.weapon].imageName, weaponList[itemStats[i].dropItem.weapon].damage, weaponList[itemStats[i].dropItem.weapon].damageType, weaponList[itemStats[i].dropItem.weapon].height, weaponList[itemStats[i].dropItem.weapon].width, weaponList[itemStats[i].dropItem.weapon].knockBack, weaponList[itemStats[i].dropItem.weapon].type, weaponList[itemStats[i].dropItem.weapon].range, weaponList[itemStats[i].dropItem.weapon].mpUsage, weaponList[itemStats[i].dropItem.weapon].mpUsage, weaponList[itemStats[i].dropItem.weapon].magicType, weaponList[itemStats[i].dropItem.weapon].bulletType, weaponList[itemStats[i].dropItem.weapon].bulletUsage);

                    Random rand = new();

                    // Random Stats
                    NewWeapon.damage += rand.Next(0, 50 + difficulty[4]);
                    NewWeapon.width += rand.Next(0, 25);
                    NewWeapon.height += rand.Next(0, 25);


                    if (MessageBox.Show($"Do you Want this Weapon? \n Current Weapon:{CurrentWeapon.name} \n Damage:{CurrentWeapon.damage} \n Type:{CurrentWeapon.damageType} and {CurrentWeapon.type} \n Height: {CurrentWeapon.height} \n Width: {CurrentWeapon.width} \n Knockback: {CurrentWeapon.knockBack} \n" + " vs " + $"\n New Weapon:{NewWeapon.name} \n Damage:{NewWeapon.damage} \n Type:{NewWeapon.damageType} and {NewWeapon.type} \n Height: {NewWeapon.height} \n Width: {NewWeapon.width} \n Knockback: {NewWeapon.knockBack} \n", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {

                        // Player get new weapon
                        playerList[currentPlayer].weapon = NewWeapon;

                        // Change text
                        LogBox.Text += $"{playerList[currentPlayer].name} got a {playerList[currentPlayer].weapon.name}! \n";

                        // Show UI change
                        UpdateUi();
                        ScrollBar.ScrollToEnd();
                    };

                }
                else
                {
                    MessageBox.Show($"You don't have enough coins! You only have {coin} and the chest requires {itemStats[i].dropItem.coinAmount}!");
                }


            }
        }


        // Give Items
        public void GiveItems(int i)
        {
            coin += itemStats[i].dropItem.coinAmount;
            ammo += itemStats[i].dropItem.ammoAmount;
            holyCross += itemStats[i].dropItem.holy_crossAmount;
            key += itemStats[i].dropItem.keyAmount;
            bomb += itemStats[i].dropItem.bombAmount;

            UpdateItemCount();
        }

        // Show this change
        public void UpdateItemCount()
        {
            TotalPartyInv.Text = $" Coin: {coin} \n Ammo: {ammo} \n Holy Cross: {holyCross} \n Key: {key} \n Bomb:{bomb}";
        }

        // Update UI
        public void UpdateUi()
        {

            for (int i = 0; i < playerList.Count; i++)
            {

                // Set health to max
                SetHealthToMax(i);

                // Show the current stats of the players
                string description = $"\n Name: {playerList[i].name} the {playerList[i].playerClass} \n Health:{playerList[i].health}/{playerList[i].healthMax} \n Mp:{playerList[i].mp}/{playerList[i].mpMax} \n Phys:{playerList[i].phys} \n Magic: {playerList[i].magic} \n Gun: {playerList[i].gun} \n Phys Def: {playerList[i].phys} \n Magic Def: {playerList[i].magDef} \n Speed: {playerList[i].speed} \n Mp Regen: {playerList[i].mpRegen} \n Size: {playerList[i].size} \n Weapon: {playerList[i].weapon.name} \n Boon: ";

                // Show the boons
                foreach (BoonMaker x in playerList[i].playerBoons.OfType<BoonMaker>())
                {
                    description += x.boonName;
                }

                // Show the text and description
                foreach (TextBlock x in PlayerUiBox.Children.OfType<TextBlock>())
                {
                    if ((string)x.Tag == "Text-" + i)
                    {
                        x.Text = description;
                    }
                }

            }
        }





    }



    // Player Ui Class
    public class PlayerUi
    {
        public string description;
        public int targetPlayer;

        public PlayerUi(string description, int targetPlayer, Canvas PlayerUiBox, List<PlayerMaker> playerList)
        {
            this.description = description;
            this.targetPlayer = targetPlayer;

            _ = new Draw("PlayerUiBackground-" + targetPlayer, 250, 125, -125 + (125 * playerList.Count), 0, $"ui/background", "background" + playerList.Count, PlayerUiBox);
            _ = new DrawTextBlock("Text-" + targetPlayer, 250, 125, -125 + (125 * playerList.Count), 0, 10, description, PlayerUiBox);

        }


    }

 
    // Wall Class
    public class WallMaker
    {
        public double health;
        public int totalWall;
        public string imageName;
        public string name;

        public WallMaker(double health, int size, int x, int y, Canvas PlayerSpace, int totalWall, string imageName, string name, string getTagName)
        {
            this.health = health;
            this.imageName = imageName;
            this.name = name;

            var _ = new Draw(getTagName, size, size, x, y, this.imageName, this.name, PlayerSpace);
        }

        public void takeDamage(double damage)
        {
            this.health -= damage;
        }

        public void checkIfDead(List<Rectangle> itemstoremove, Rectangle x)
        {
            if (this.health <= 0)
            {
                itemstoremove.Add(x);
            }
        }

    }

    // item class
    public class ItemMaker
    {
        private double health;
        public string nameImage;
        public string name;
        public string isChest;
        public DropMaker dropItem;


        public ItemMaker(double health, int height, int width, int x, int y, Canvas PlayerSpace, int totalItem, string nameImage, string name, string isChest, DropMaker dropItem)
        {
            this.health = health;
            this.nameImage = nameImage;
            this.name = name;
            this.isChest = isChest;
            this.dropItem = dropItem;

            var _ = new Draw($"item-{totalItem}", height, width, x, y, nameImage, name, PlayerSpace);
        }

        public void takeDamage(double damage)
        {
            this.health -= damage;
        }

        public void checkIfDead(List<Rectangle> itemstoremove, Rectangle x)
        {
            if (this.health <= 0)
            {
                itemstoremove.Add(x);
            }
        }

    }

    public class DropMaker
    {

        public int coinAmount;
        public int ammoAmount;
        public int holy_crossAmount;
        public int keyAmount;
        public int bombAmount;
        public int pointsAmount;
        public int thisBoon;
        public int weapon;
        public double healthAmount;

        public DropMaker(int coinAmount, int ammoAmount, int holy_crossAmount, int keyAmount, int bombAmount, double healthAmount, int pointsAmount, int weapon, int thisBoon)
        {
            this.coinAmount = coinAmount;
            this.ammoAmount = ammoAmount;
            this.holy_crossAmount = holy_crossAmount;
            this.keyAmount = keyAmount;
            this.bombAmount = bombAmount;
            this.healthAmount = healthAmount;
            this.pointsAmount = pointsAmount;
            this.weapon = weapon;
            this.thisBoon = thisBoon;
        }
    }
}