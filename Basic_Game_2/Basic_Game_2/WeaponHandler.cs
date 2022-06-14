using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace Basic_Game_2
{
    public partial class MainWindow : Window
    {

        // How this Function Works:
        // Checks if player presses keyboard and if a weapon is not created
        // if so, the dirrection the player is facing is now the dirrection the sword is aiming
        // the weapon is created where the sword is aiming at and changes height and width based on this dirrection. This is why there are two different images to allow this to happen.
        // if the weapon is created, then add to the 'frame' level. If the frame passes the weaponTimer, then the sword will disapear. This is basically how long the player attcks. 
        // Find the sword if it exists, and then follow the player based on the current position of the player.

        // At the very bottom is how the weapon is made




        // Main Problems:
        // The WeaponCreate shouldn't have been an object. I think Weapon Maker have being donated from the overall list, should be the one that has the object of itself stored within it



        // CONNECTED TO THE BASE
        public void SwordAttack(WeaponMaker CurrentWeapon, PlayerMaker CurrentPlayer)
        {

            // If the space bar is press and the weapon is not created
            if (Keyboard.IsKeyDown(Key.Space) && weaponCreated == false)
            {

                // Get the dirrection the player is looking at that and that will be the dirrection of the blade no matter the dirrection moving 
                oldDirrection = CurrentPlayer.currentDirrection;

                // Check the damage type
                // If it's magic it will use MP while if it's phys / gun it will just swing normally
                CheckAttackType(CurrentWeapon, CurrentPlayer);

                // If the weapon is ranged then it will shoot out a projectile
                CheckRanged(CurrentWeapon, CurrentPlayer);

            }


            // If the weapon is created
            if (weaponCreated)
            {
                frame++;
                weaponCreated = weapon.AddRemoveSword(weaponCreated, frame, Player, itemstoremove, oldDirrection);

            }
            else
            {
                frame = 0;
            }
        }


        // Check the damagetype of the weapon
        public void CheckAttackType(WeaponMaker CurrentWeapon, PlayerMaker CurrentPlayer)
        {

            // If the weapon is physical or gun
            if (CurrentWeapon.damageType == "phys" || CurrentWeapon.damageType == "gun")
            {

                // Create a weapon and then return "true" that a weapon is created
                weaponCreated = weapon.CreateWeapon(CurrentPlayer.currentDirrection, ItemSpace, CurrentWeapon.type, CurrentWeapon);

            } // or the weapon is magical
            else if (CurrentWeapon.damageType == "magic")
            {

                // Check Magic
                CheckMagic(CurrentWeapon, CurrentPlayer);

                UpdateUi();

            }

        }


        // If the weapon is magical
        public void CheckMagic(WeaponMaker CurrentWeapon, PlayerMaker CurrentPlayer)
        {
            // If Mp usage above current weapon mpusage
            if (CurrentPlayer.mp - CurrentWeapon.mpUsage > 0)
            {
                // Subtract the current player mp bythe current weapon mp usage
                CurrentPlayer.mp -= CurrentWeapon.mpUsage;

                // 
                weaponCreated = weapon.CreateWeapon(CurrentPlayer.currentDirrection, ItemSpace, CurrentWeapon.type, CurrentWeapon);

            }
        }

        // If the weapon is ranged
        public void CheckRanged(WeaponMaker CurrentWeapon, PlayerMaker CurrentPlayer)
        {
            if (CurrentWeapon.type == "ranged")
            {

                // If there is enough ammo
                if (ammo - CurrentWeapon.bulletUsage >= 0)
                {

                    // Subtract the amount of ammo by the bullet usage of the weapon
                    ammo -= CurrentWeapon.bulletUsage;

                    // Fire the projectile
                    Fire(CurrentWeapon, CurrentPlayer.currentDirrection);

                    // Change UI to show this change
                    UpdateItemCount();
                }

            }
        }

        // Move each bullet in the array
        public void MoveBullet()
        {
            Random rand = new Random();

            for (int i = 0; i < bulletFired.Count; i++)
            {
                bulletFired[i].Move(rand, BulletAccuracyX, BulletAccuracyY, itemstoremove);
            }

        }




        // Randomly change the bullet movement in the x axis
        public void BulletAccuracyX(Rectangle x, Random rand, int accuracy)
        {
            // Random
            Canvas.SetLeft(x, (Canvas.GetLeft(x) - rand.Next(0, accuracy)));
            Canvas.SetLeft(x, (Canvas.GetLeft(x) + rand.Next(0, accuracy)));
        }

        // Randony chnage the bullet movement in the y axis
        public void BulletAccuracyY(Rectangle x, Random rand, int accuracy)
        {
            // Random
            Canvas.SetTop(x, (Canvas.GetTop(x) + rand.Next(0, accuracy)));
            Canvas.SetTop(x, (Canvas.GetTop(x) - rand.Next(0, accuracy)));
        }

        // The creation of the bullet
        public void Fire(WeaponMaker CurrentWeapon, string currentDirrection)
        {
            // Get Current Bullet
            // And create a duplicate of it
            BulletMaker CurrentBullet = new(CurrentWeapon.bulletType.name, CurrentWeapon.bulletType.description, CurrentWeapon.bulletType.bulletType, CurrentWeapon.bulletType.bulletUsage, CurrentWeapon.bulletType.bulletDamage, CurrentWeapon.bulletType.bulletWidth, CurrentWeapon.bulletType.bulletHeight, CurrentWeapon.bulletType.bulletSpeed, CurrentWeapon.bulletType.bulletAccuracy);

            // Get the current bullet and add the damage of the weapon onto it
            CurrentBullet.bulletDamage += CurrentWeapon.damage;

            // Get the dirrection of the bullet fired
            CurrentBullet.dirrection = currentDirrection;

            // Chnage the tag to the current bullet
            CurrentBullet.tag = $"bullet-{bulletFired.Count}-{oldDirrection}";

            // Get the (x,y) position
            double x = Canvas.GetLeft(Player);
            double y = Canvas.GetTop(Player);

            if (currentDirrection == "up" || currentDirrection == "down") // If current Dirrection up or down
            {

                // Create the bullet onto the canvas
                var newRect = new Draw($"bullet-{bulletFired.Count}-{oldDirrection}", CurrentBullet.bulletWidth, CurrentBullet.bulletHeight, Convert.ToInt16(x), Convert.ToInt16(y), $"weapons/10", CurrentBullet.name, BulletCanvas);

                // Add the current self onto the bullet
                CurrentBullet.self = newRect.Rect;


            }
            else if (currentDirrection == "left" || currentDirrection == "right")
            {

                // Create the bullet onto the canvas
                var newRect = new Draw($"bullet-{bulletFired.Count}-{oldDirrection}", CurrentBullet.bulletHeight, CurrentBullet.bulletWidth, Convert.ToInt16(x), Convert.ToInt16(y), $"weapons/10", CurrentBullet.name, BulletCanvas);

                // Add the current self onto the bullet
                CurrentBullet.self = newRect.Rect;

            }

            // Add it to the List so it can be accessed
            bulletFired.Add(CurrentBullet);


        }

    }

    // Class that Creates Bullets
    public class BulletMaker
    {
        public string name;
        public string description;
        public int bulletUsage;
        public string bulletType;

        public string tag = "";
        public string dirrection = "";

        public double bulletDamage;

        public int bulletWidth;
        public int bulletHeight;
        public int bulletSpeed;
        public int bulletAccuracy;

        public Rectangle self = new();

        public BulletMaker(string name, string description, string bulletType, int bulletUsage, double bulletDamage, int bulletWidth, int bulletHeight, int bulletSpeed, int bulletAccuracy)
        {
            this.name = name;
            this.description = description;
            this.bulletType = bulletType;
            this.bulletUsage = bulletUsage;

            this.bulletDamage = bulletDamage;

            this.bulletWidth = bulletWidth;
            this.bulletHeight = bulletHeight;
            this.bulletSpeed = bulletSpeed;
            this.bulletAccuracy = bulletAccuracy;
        }



        // If this Bullet is Off Boundry then delete it
        public void CheckIfOffBoundry(Rectangle x, List<Rectangle> itemstoremove)
        {
            if (Canvas.GetLeft(x) < 80)
            {
                itemstoremove.Add(x);
            }
            else if (Canvas.GetLeft(x) > (580 - x.Width))
            {
                itemstoremove.Add(x);
            }
            else if (Canvas.GetTop(x) < 80)
            {
                itemstoremove.Add(x);
            }
            else if (Canvas.GetTop(x) > (580 - x.Height))
            {
                itemstoremove.Add(x);
            }
        }

        // Moving the Bullet
        public void Move(Random rand, Action<Rectangle, Random, int> BulletAccuracyX, Action<Rectangle, Random, int> BulletAccuracyY, List<Rectangle> itemstoremove)
        {

            if (dirrection == "up")
            {
                Canvas.SetTop(self, (Canvas.GetTop(self) - bulletSpeed));

                BulletAccuracyX(self, rand, bulletAccuracy);

                CheckIfOffBoundry(self, itemstoremove);
            }
            else if (dirrection == "down")
            {
                Canvas.SetTop(self, (Canvas.GetTop(self) + bulletSpeed));

                BulletAccuracyX(self, rand, bulletAccuracy);

                CheckIfOffBoundry(self, itemstoremove);
            }
            else if (dirrection == "left")
            {
                Canvas.SetLeft(self, (Canvas.GetLeft(self) - bulletSpeed));

                BulletAccuracyY(self, rand, bulletAccuracy);

                CheckIfOffBoundry(self, itemstoremove);
            }
            else if (dirrection == "right")
            {
                Canvas.SetLeft(self, (Canvas.GetLeft(self) + bulletSpeed));

                BulletAccuracyY(self, rand, bulletAccuracy);

                CheckIfOffBoundry(self, itemstoremove);
            }


        }
    }


    // Class that is made to create weapons from Weapon Maker
    // I don't know why this is an object, since most of the time it is used, it is treated like a normal function
    public class WeaponCreate
    {

        public double weaponHeight;
        public double weaponWidth;
        public double weaponTimer;

        public Rectangle self = new();


        // Create weapon based on dirrection
        public bool CreateWeapon(string currentDirrection, Canvas PlayerSpace, string tagName, WeaponMaker CurrentWeapon)
        {
            if (currentDirrection == "up" || currentDirrection == "down") // If current Dirrection up or down
            {
                weaponWidth = CurrentWeapon.height;
                weaponHeight = CurrentWeapon.width;

                weaponTimer = CurrentWeapon.timeLength;
                
                // Create the weapon
                var newRect = new Draw(tagName, Convert.ToInt16(weaponWidth), Convert.ToInt16(weaponHeight), 0, 0, $"weapons/{CurrentWeapon.imageName}-1", CurrentWeapon.name, PlayerSpace);

                // Set the self to the self
                self = newRect.Rect;

            }
            else if (currentDirrection == "left" || currentDirrection == "right")
            {
                weaponWidth = CurrentWeapon.width;
                weaponHeight = CurrentWeapon.height;

                weaponTimer = CurrentWeapon.timeLength;

                // Create the weapon
                var newRect = new Draw(tagName, Convert.ToInt16(weaponWidth), Convert.ToInt16(weaponHeight), 0, 0, $"weapons/{CurrentWeapon.imageName}-1.5", CurrentWeapon.name, PlayerSpace);

                // Set the self to the self
                self = newRect.Rect;
            }

            return true;

        }

        // Adding or Removing the sword based on timer
        public bool AddRemoveSword(bool weaponCreated, int frame, Rectangle Player, List<Rectangle> itemstoremove, string oldDirrection)
        {

            // if the weapon is melee or ranged, then follow playr and if the weapon timer runs out, delete itself
            if ((string)self.Tag == $"melee" || (string)self.Tag == $"ranged")
            {
                weaponCreated = FollowPlayer(self, Player, weaponCreated, oldDirrection);

                if (frame > weaponTimer)
                {
                    itemstoremove.Add(self);

                    weaponCreated = false;
                }

            }

            return weaponCreated;
        }




        // This makes no sense to use as an object
        public bool FollowPlayer(Rectangle z, Rectangle PlayerCharacter, bool weaponCreated, string oldDirrection)
        {
            // Follow the Player Perfectly

            var rt = new RotateTransform();

            if (oldDirrection == "up") //up
            {

                rt.Angle = 0;

                z.RenderTransform = rt;
                z.RenderTransformOrigin = new Point(0.5, 0.5);

                Canvas.SetTop(z, (Canvas.GetTop(PlayerCharacter) - z.Height));

                Canvas.SetLeft(z, (Canvas.GetLeft(PlayerCharacter) + PlayerCharacter.Width / 2) - z.Width / 2);

            }
            else if (oldDirrection == "down") //down
            {

                rt.Angle = 180;

                z.RenderTransform = rt;
                z.RenderTransformOrigin = new Point(0.5, 0.5);

                Canvas.SetTop(z, (Canvas.GetTop(PlayerCharacter) + PlayerCharacter.Height));

                Canvas.SetLeft(z, (Canvas.GetLeft(PlayerCharacter) + PlayerCharacter.Width / 2) - z.Width / 2);



            }

            if (oldDirrection == "left") // left
            {

                rt.Angle = 0;

                z.RenderTransform = rt;
                z.RenderTransformOrigin = new Point(0.5, 0.5);

                Canvas.SetTop(z, (Canvas.GetTop(PlayerCharacter) + PlayerCharacter.Height / 4));

                Canvas.SetLeft(z, (Canvas.GetLeft(PlayerCharacter) - z.Width));


            }
            else if (oldDirrection == "right") // left
            {
                rt.Angle = 180;


                z.RenderTransform = rt;
                z.RenderTransformOrigin = new Point(0.5, 0.5);

                Canvas.SetTop(z, (Canvas.GetTop(PlayerCharacter) + PlayerCharacter.Width / 4));

                Canvas.SetLeft(z, (Canvas.GetLeft(PlayerCharacter) + PlayerCharacter.Width));

            }



            return weaponCreated;
        }
    }

    public class WeaponMaker
    {

        // Base Weapon
        public string name;
        public string imageName;
        public double damage;
        public double height;
        public double width;
        public double knockBack;
        public string damageType;
        public string type;
        public double timeLength;

        // if ranged
        public double range;

        // If Magic
        public double mpUsage;
        public string magicType;

        // If Gun
        public double bulletUsage;
        public BulletMaker bulletType = new("", "", "", 0, 0, 0, 0, 0, 0);



        public WeaponMaker(string name, string imageName, double damage, string damageType, double width, double height, double knockBack, string type, double timeLength, double range, double mpUsage, string magicType, BulletMaker bulletType, double bulletUsage)
        {
            this.name = name;
            this.imageName = imageName;
            this.damage = damage;
            this.damageType = damageType;
            this.width = width;
            this.height = height;
            this.knockBack = knockBack;
            this.type = type;

            this.timeLength = timeLength;

            // Ranged
            this.range = range;

            // Magic
            this.mpUsage = mpUsage;
            this.magicType = magicType;

            // Gun
            this.bulletType = bulletType;
            this.bulletUsage = bulletUsage;
        }
    }



    /*
     public class EnemyMaker
    {

    }


    public class AllPlayers
    {
        public int coin;
        public int ammo;
        public List<HolyCross> holyCross;
        public List<Bomb> bomb;
        public List<Key> key;

        public AllPlayers(int getCoin, int getAmmo, HolyCross getHolyCross, Bomb getBomb, Key getKey)
        {
            this.coin = getCoin;
            this.ammo = getAmmo;
            holyCross.Add(getHolyCross);
            bomb.Add(getBomb);
            key.Add(getKey);
        }
    }


    public class HolyCross
    {

        public int amount;

        public HolyCross(int getAmount)
        {
            this.amount = getAmount;
        }


        // Destroy all Enemies
        public void DestroyAll(List<EnemyMaker> enemyList, List<Rectangle> itemstoremove, Canvas PlayerSpace)
        {

            // If there is enough holy crosses
            if (amount > 0)
            {
                // Testing all the rectangles
                foreach (Rectangle x in PlayerSpace.Children.OfType<Rectangle>())
                {
                    for (int i = 0; i < enemyList.Count; i++)
                    {
                        if ((string)x.Tag == $"enemy-{i}")
                        {
                            itemstoremove.Add(x);
                        }
                    }
                }
            } else
            {
                MessageBox.Show("Not Enough Crosses");
            }

        }
        
    }

    public class Bomb
    {

    }

    public class Key
    {

    }

     * */

}