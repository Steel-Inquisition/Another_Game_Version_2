using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Shapes;


namespace Basic_Game_2
{


    // Main Probklem:
    // Exiting to a new room could be a bit more efficent. Since it is repettive and can probally just be a single function
    public partial class MainWindow : Window
    {



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
                            enemyStats[i].WeaponCreated = true;
                            enemyStats[i].weaponRectangle = weapon.CreateWeapon(enemyStats[i].currentDirrection, ItemSpace, $"enemymelee-{enemyStats[i].thisCount}", enemyStats[i].weapon);

                            // Set reaction back to 0
                            enemyStats[i].reaction = 0;

                        }

                    }

                    foreach (Rectangle x in ItemSpace.Children.OfType<Rectangle>())
                    {

                        // if the weapon is from an enemy
                        if ((string)x.Tag == $"enemymelee-{i}" || (string)x.Tag == $"enemyranged-{i}")
                        {

                            // Follow Enemy
                            weapon.FollowPlayer(x, enemyStats[i].self, enemyStats[i].WeaponCreated, enemyStats[i].currentDirrection);

                            // If the fire rate is complete then delete it
                            if (enemyStats[i].firerate > enemyStats[i].innerFrame)
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
                                    currentPlayer = enemyStats[i].calculateDamage(playerList[currentPlayer], x, oldDirrection, LogBox, UpdateUi, ScrollBar, healthBarList[i], PlayerSpace, ItemSpace, currentPlayer, PlayerUiBox, enemyStats, i, difficulty);

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
                currentPlayer = playerList[currentPlayer].damaged(enemyStats[enemyTarget], LogBox, UpdateUi, ScrollBar, PlayerUiBox, healthBarList[0], currentPlayer, difficulty);
                playerList[currentPlayer].KnockBack(PlayerSpace, ItemSpace, Player, enemyStats[enemyTarget].currentDirrection, enemyStats[enemyTarget].knockback);
                currentPlayer = playerList[currentPlayer].CheckIfDead(currentPlayer, PlayerUiBox, enemyStats[enemyTarget].name, LogBox, ScrollBar, UpdateUi);


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

        // if the health is above the max health, set health to max health
        public void SetHealthToMax(int select)
        {
            if (playerList[select].health > playerList[select].healthMax)
            {
                playerList[select].health = playerList[select].healthMax;
            }
        }

    }


    // Player Maker Class
    public class PlayerMaker : LivingBase
    {
        public int invisibiltyFrames;
        public List<BoonMaker> playerBoons = new();

        public PlayerMaker(string name, string playerClass, double health, int mp, double healthMax, double mpMax, double size, WeaponMaker weapon, double phys, double magic, double gun, double physDef, double magDef, double speed, double mpRegen, int invisibiltyFrames)
        {
            this.name = name;
            this.playerClass = playerClass;
            this.health = health;
            this.healthMax = healthMax;
            this.mp = mp;
            this.mpMax = mpMax;
            this.size = size;
            this.weapon = weapon;
            this.phys = phys;
            this.magic = magic;
            this.gun = gun;
            this.physDef = physDef;
            this.magDef = magDef;
            this.speed = speed;
            this.mpRegen = mpRegen;
            this.invisibiltyFrames = invisibiltyFrames;
        }


        // Change player based on ZXCV
        public int playerSwitcher(int currentPlayer, Rectangle Player, List<PlayerMaker> playerList, bool PlayerIsHit)
        {
            if (Keyboard.IsKeyDown(Key.Z) && playerList[0].health > 0 && PlayerIsHit == false)
            {
                currentPlayer = 0;

                PlayerImage(Player);

            }
            else if (Keyboard.IsKeyDown(Key.X) && playerList[1].health > 0 && PlayerIsHit == false)
            {
                currentPlayer = 1;

                PlayerImage(Player);

            }
            else if (Keyboard.IsKeyDown(Key.C) && playerList[2].health > 0 && PlayerIsHit == false)
            {
                currentPlayer = 2;

                PlayerImage(Player);

            }
            else if (Keyboard.IsKeyDown(Key.V) && playerList[3].health > 0 && PlayerIsHit == false)
            {
                currentPlayer = 3;

                PlayerImage(Player);

            }

            return currentPlayer;
        }

        // Change player image by filling
        public void PlayerImage(Rectangle Player)
        {
            _ = new FillDraw("classes/" + playerClass, Player);
        }


        // Change UI based on player selected
        public int PlayerUiSelect(int currentPlayer, Canvas PlayerSpace, List<PlayerMaker> playerList)
        {


            foreach (Rectangle x in PlayerSpace.Children.OfType<Rectangle>())
            {


                // All players that have enough health have a white background
                for (int i = 0; i < playerList.Count; i++)
                {
                    if ((string)x.Tag == $"PlayerUiBackground-{i}" && playerList[i].health > 0)
                    {
                        _ = new FillDraw("ui/background", x);
                    }
                }

                // find the current player selected and turn that green
                if ((string)x.Tag == $"PlayerUiBackground-{currentPlayer}" && playerList[currentPlayer].health > 0)
                {
                    _ = new FillDraw("ui/backgroundSelect", x);
                } // else if player swithcing to is dead, then find someone else
                else if (playerList[currentPlayer].health <= 0)
                {


                    // if all the players health are above 0
                    if (playerList[0].health > 0 || playerList[1].health > 0 || playerList[2].health > 0 || playerList[3].health > 0)
                    {

                        // If the current player you are controlling has less than 0 health
                        while (playerList[currentPlayer].health <= 0)
                        {

                            // if the current player is in the third slot, move to the 1st slot
                            if (currentPlayer == 3)
                            {
                                currentPlayer = 0;
                            }
                            else // else increase current player until you find a player that is alive
                            {
                                currentPlayer++;
                            }
                        }
                    }


                }

            }



            return currentPlayer;

        }
        

        // Deal Damage to Player
        public int damaged(EnemyMaker Enemy, TextBlock LogBox, Action UpdateUi, ScrollViewer ScrollBar, Canvas PlayerUiBox, ProgressBar CurrentProgressBar, int currentPlayer, int[] difficulty)
        {

            // Set Initial Damage
            double damage = difficulty[2];

            // Based on damage type add that type of damage
            if (Enemy.weapon.damageType == "phys")
            {
                damage += Enemy.phys;
                damage -= physDef;
            }
            else if (Enemy.weapon.damageType == "magic")
            {
                damage += (Enemy.magic);
                damage -= magDef;
            }

            // Let player take damage
            currentPlayer = TakeDamage(damage, Enemy.name, currentPlayer, LogBox, UpdateUi, PlayerUiBox, ScrollBar, CurrentProgressBar);


            return currentPlayer;
        }

        // Deal damage based on bomb
        public int BombDamaged(int damage, TextBlock LogBox, Action UpdateUi, ScrollViewer ScrollBar, Canvas PlayerUiBox, ProgressBar CurrentProgressBar, int currentPlayer)
        {
            return TakeDamage(damage, "bomb", currentPlayer, LogBox, UpdateUi, PlayerUiBox, ScrollBar, CurrentProgressBar);

        }

        // Check if the player is dead
        public int CheckIfDead(int currentPlayer, Canvas PlayerUiBox, string currentKiller, TextBlock LogBox, ScrollViewer ScrollBar, Action UpdateUi)
        {

            // if the current player has less than 0 health
            if (health <= 0)
            {

                // Find the player and turn them red
                foreach (Rectangle x in PlayerUiBox.Children.OfType<Rectangle>())
                {
                    if ((string)x.Tag == $"PlayerUiBackground-{currentPlayer}")
                    {
                        _ = new FillDraw("ui/backgroundDead", x);

                    }

                }

                // Show that the player was dead
                LogBox.Text += $"{name} is dead! Killed by {currentKiller}! \n";

                // Change the UI
                UpdateUi();
                ScrollBar.ScrollToEnd();
            }

            return currentPlayer;
        }



        // Move to next room when reaching border
        public void NextRoom(Rectangle Player, MapMaker Map, Action FillMap, Action transportNextRoom, Action FindRoomOnMap, Action RemovePlayerSymbol, TextBlock LogBox, ScrollViewer ScrollBar, List<EnemyMaker> enemyStats, List<ProgressBar> healthBarList, List<BulletMaker> bulletFired, List<BombMaker> bombList, List<ExplosionMaker> explosionList, Action MakeArmPusherBoss)
        {

            bool exitMap = false;

            // Going to the Left
            if (Canvas.GetLeft(Player) < 80)
            {

                // Change the player symbol
                RemovePlayerSymbol();

                // Change the current room the player is in
                Map.currentRoom -= 1;

                // Teleport player to give illusion of change
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) + 475);

                // Exit map is true
                exitMap = true;

                LogBox.Text += "Moved Left \n";

            }
            else if (Canvas.GetLeft(Player) > (580 - Player.Width)) // to the right
            {
                // Change the player symbol
                RemovePlayerSymbol();

                // Change the current room the player is in
                Map.currentRoom += 1;

                // Teleport player to give illusion of change
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) - 475);

                // Exit map is true
                exitMap = true;

                LogBox.Text += "Moved Right \n";

            }
            else if (Canvas.GetTop(Player) < 80) // to the top
            {

                // Change the player symbol
                RemovePlayerSymbol();

                // Change the current room the player is in
                Map.currentRoom -= 10;

                // Teleport player to give illusion of change
                Canvas.SetTop(Player, Canvas.GetTop(Player) + 475);

                // Exit map is true
                exitMap = true;

                LogBox.Text += "Moved North \n";
            }
            else if (Canvas.GetTop(Player) > (580 - Player.Height)) // to the south
            {

                // Change the player symbol
                RemovePlayerSymbol();

                // Change the current room the player is in
                Map.currentRoom += 10;

                // Teleport player to give illusion of change
                Canvas.SetTop(Player, Canvas.GetTop(Player) - 475);

                // Exit map is true
                exitMap = true;

                LogBox.Text += "Moved South \n";
            }


            // if exiitng the map
            // clear everything
            // then fill the map to the new room
            if (exitMap)
            {
                healthBarList.Clear();

                FindRoomOnMap();
                transportNextRoom();

                enemyStats.Clear();
                bulletFired.Clear();

                bombList.Clear();
                explosionList.Clear();

                FillMap();

                // Activate Special Room
                Map.totalMap[Map.currentRoom].ActivateRoom(LogBox, ScrollBar, Player, MakeArmPusherBoss);




            }

        }

        // This is the sound made when walking
        public void walkingSound()
        {
            // https://www.w3schools.com/cs/cs_interface.php

        }
    }

    public class BoonMaker
    {
        public string boonName = "";
        public string boonDescription = "";
        public string boonType = "";
        public int cost;
        public double healthCost;
        public int healthMaxCost;
        public int invisibiltyFrames;

        public int mpCost;
        public int mpMaxCost;
        public int physCost;
        public int magicCost;
        public int gunCost;
        public int physDefCost;
        public int magDefCost;
        public int speedCost;
        public int mpRegenCost;
        public int sizeCost;

        public BoonMaker(string boonType, string boonName, string boonDescription, int cost, double healthCost, int mpCost, int healthMaxCost, int mpMaxCost, int physCost, int magicCost, int gunCost, int physDefCost, int magDefCost, int speedCost, int mpRegenCost, int sizeCost, int invisibiltyFrames)
        {
            this.boonType = boonType;
            this.boonName = boonName;
            this.boonDescription = boonDescription;
            this.cost = cost;
            this.healthCost = healthCost;
            this.mpCost = mpCost;
            this.healthMaxCost = healthMaxCost;
            this.mpMaxCost = mpMaxCost;
            this.physCost = physCost;
            this.magicCost = magicCost;
            this.gunCost = gunCost;
            this.physDefCost = physDefCost;
            this.magDefCost = magDefCost;
            this.speedCost = speedCost;
            this.mpRegenCost = mpRegenCost;
            this.sizeCost = sizeCost;
            this.invisibiltyFrames = invisibiltyFrames;
        }

    }



    public class ClassMaker
    {

        public string classDescription = "";
        public string className = "";
        public string imageClassName = "";
        public string statsDisplay = "";

        public double classHealth;
        public int classHealthMax;
        public int classMp;
        public int classMpMax;
        public int classPhys;
        public int classMagic;
        public int classGun;
        public int classPhysDef;
        public int classMagDef;
        public int classSpeed;
        public int classMpRegen;
        public int classSize;
        public int invisibiltyFrames;

        public WeaponMaker classWeapon;

        public ClassMaker(string imageClassName, string className, string classDescription, double classHealth, int classMp, int classHealthMax, int classMpMax, int classMpRegen, int classSize, WeaponMaker classWeapon, int classPhys, int classMagic, int classGun, int classPhysDef, int classMagDef, int classSpeed, int invisibiltyFrames)
        {
            this.className = className;
            this.imageClassName = imageClassName;
            this.classDescription = classDescription;

            this.classHealth = classHealth;
            this.classMp = classMp;
            this.classHealthMax = classHealthMax;
            this.classMpMax = classMpMax;
            this.classSize = classSize;
            this.classWeapon = classWeapon;
            this.classPhys = classPhys;
            this.classMagic = classMagic;
            this.classGun = classGun;
            this.classPhysDef = classPhysDef;
            this.classMagDef = classMagDef;
            this.classSpeed = classSpeed;
            this.classMpRegen = classMpRegen;
            this.invisibiltyFrames = invisibiltyFrames;

        }
    }


}