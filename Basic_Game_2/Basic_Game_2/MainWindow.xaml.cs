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
        public MapMaker Map = new(100, 1);

        // Points
        public int points = 0;
        public int coin = 5;
        public double ammo = 20;
        public int holyCross = 1;
        public int bomb = 10;
        public int key = 1;

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