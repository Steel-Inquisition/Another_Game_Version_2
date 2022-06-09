using System;
using System.Collections.Generic;
using System.Linq;
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


        /// <summary>
        ///  use classMakerList[i].classWeapon for weapon selector eventually
        /// </summary>


        public void SwordAttack(WeaponMaker CurrentWeapon, PlayerMaker CurrentPlayer)
        {


            if (Keyboard.IsKeyDown(Key.Space) && weaponCreated == false)
            {

                oldDirrection = CurrentPlayer.currentDirrection;

                CheckAttackType(CurrentWeapon, CurrentPlayer);
                CheckRanged(CurrentWeapon, CurrentPlayer);

            }

            if (weaponCreated)
            {
                frame++;
                weaponCreated = weapon.AddRemoveSword(ItemSpace, weaponCreated, frame, Player, itemstoremove, oldDirrection);

            }
            else
            {
                frame = 0;
            }
        }

        public void EnemySwordAttack(WeaponMaker CurrentWeapon, EnemyMaker Enemy, Rectangle EnemyRectangle, Rectangle Weapon, int enemyNum)
        {
            weapon.EnemyAddSword(PlayerSpace, Enemy.WeaponCreated, Enemy.innerFrame, EnemyRectangle, itemstoremove, Enemy.currentDirrection, enemyStats, Weapon, enemyNum);

        }


        public void CheckAttackType(WeaponMaker CurrentWeapon, PlayerMaker CurrentPlayer)
        {
            if (CurrentWeapon.damageType == "phys" || CurrentWeapon.damageType == "gun")
            {
                weaponCreated = weapon.CreateWeapon(CurrentPlayer.currentDirrection, ItemSpace, CurrentWeapon.type, CurrentWeapon);

            }
            else if (CurrentWeapon.damageType == "magic")
            {
                CheckMagic(CurrentWeapon, CurrentPlayer);

                UpdateUi();

            }

        }



        public void CheckMagic(WeaponMaker CurrentWeapon, PlayerMaker CurrentPlayer)
        {
            // If Mp usage above current weapon mpusage
            if (CurrentPlayer.mp - CurrentWeapon.mpUsage > 0)
            {
                CurrentPlayer.mp -= CurrentWeapon.mpUsage;

                weaponCreated = weapon.CreateWeapon(CurrentPlayer.currentDirrection, ItemSpace, CurrentWeapon.type, CurrentWeapon);

            }
        }

        public void CheckRanged(WeaponMaker CurrentWeapon, PlayerMaker CurrentPlayer)
        {
            if (CurrentWeapon.type == "ranged")
            {
                if (ammo - CurrentWeapon.bulletUsage > 0)
                {
                    ammo -= CurrentWeapon.bulletUsage;

                    Fire(CurrentWeapon, CurrentPlayer.currentDirrection);

                    UpdateItemCount();
                }

            }
        }

        public void MoveBullet()
        {
            Random rand = new Random();

            LogBox.Text = $"{bulletFired.Count}";

            foreach (Rectangle x in BulletCanvas.Children.OfType<Rectangle>())
            {
                foreach (BulletMaker y in bulletFired)
                {
                    if ((y.tag == (string)x.Tag))
                    {
                        y.Move(x, rand, BulletAccuracyX, BulletAccuracyY, itemstoremove);

                    }


                }

            }


        }





        public void BulletAccuracyX(Rectangle x, Random rand, int accuracy)
        {
            // Random
            Canvas.SetLeft(x, (Canvas.GetLeft(x) - rand.Next(0, accuracy)));
            Canvas.SetLeft(x, (Canvas.GetLeft(x) + rand.Next(0, accuracy)));
        }

        public void BulletAccuracyY(Rectangle x, Random rand, int accuracy)
        {
            // Random
            Canvas.SetTop(x, (Canvas.GetTop(x) + rand.Next(0, accuracy)));
            Canvas.SetTop(x, (Canvas.GetTop(x) - rand.Next(0, accuracy)));
        }

        public void Fire(WeaponMaker CurrentWeapon, string currentDirrection)
        {
            // Get Current Bullet
            BulletMaker CurrentBullet = CurrentWeapon.bulletType;

            CurrentBullet.bulletDamage += CurrentWeapon.damage;

            CurrentBullet.dirrection = currentDirrection;

            bulletFired.Add(CurrentBullet);

            CurrentBullet.tag = $"bullet-{bulletFired.Count - 1}-{oldDirrection}";

            double x = Canvas.GetLeft(Player);
            double y = Canvas.GetTop(Player);

            if (currentDirrection == "up" || currentDirrection == "down") // If current Dirrection up or down
            {
                _ = new Draw($"bullet-{bulletFired.Count - 1}-{oldDirrection}", CurrentBullet.bulletWidth, CurrentBullet.bulletHeight, Convert.ToInt16(x), Convert.ToInt16(y), $"weapons/10", CurrentBullet.name, BulletCanvas);


            }
            else if (currentDirrection == "left" || currentDirrection == "right")
            {
                _ = new Draw($"bullet-{bulletFired.Count - 1}-{oldDirrection}", CurrentBullet.bulletHeight, CurrentBullet.bulletWidth, Convert.ToInt16(x), Convert.ToInt16(y), $"weapons/10", CurrentBullet.name, BulletCanvas);


            }

        }

    }

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

        public BulletMaker(string name, string description, string bulletType, int bulletUsage, double bulletDamage,  int bulletWidth, int bulletHeight, int bulletSpeed, int bulletAccuracy)
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

        // Mocing the Bullet
        public void Move(Rectangle self, Random rand, Action<Rectangle, Random, int> BulletAccuracyX, Action<Rectangle, Random, int> BulletAccuracyY, List<Rectangle> itemstoremove)
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


    public class WeaponCreate
    {

        public double weaponHeight;
        public double weaponWidth;
        public double weaponTimer;

        public bool CreateWeapon(string currentDirrection, Canvas PlayerSpace, string tagName, WeaponMaker CurrentWeapon)
        {
            if (currentDirrection == "up" || currentDirrection == "down") // If current Dirrection up or down
            {
                weaponWidth = CurrentWeapon.height;
                weaponHeight = CurrentWeapon.width;

                weaponTimer = CurrentWeapon.timeLength;

                _ = new Draw(tagName, Convert.ToInt16(weaponWidth), Convert.ToInt16(weaponHeight), 0, 0, $"weapons/{CurrentWeapon.imageName}-1", CurrentWeapon.name, PlayerSpace);


            }
            else if (currentDirrection == "left" || currentDirrection == "right")
            {
                weaponWidth = CurrentWeapon.width;
                weaponHeight = CurrentWeapon.height;

                weaponTimer = CurrentWeapon.timeLength;

                _ = new Draw(tagName, Convert.ToInt16(weaponWidth), Convert.ToInt16(weaponHeight), 0, 0, $"weapons/{CurrentWeapon.imageName}-1.5", CurrentWeapon.name, PlayerSpace);
            }

            return true;

        }


        public void EnemyAddSword(Canvas PlayerSpace, bool weaponCreated, int frame, Rectangle EnemyRectangle, List<Rectangle> itemstoremove, string oldDirrection, List<EnemyMaker> enemyStats, Rectangle x, int y)
        {

            FollowPlayer(x, EnemyRectangle, frame, weaponCreated, itemstoremove, oldDirrection);
        }


        public bool AddRemoveSword(Canvas PlayerSpace, bool weaponCreated, int frame, Rectangle Player, List<Rectangle> itemstoremove, string oldDirrection)
        {
            foreach (Rectangle x in PlayerSpace.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == $"melee" || (string)x.Tag == $"ranged")
                {
                    weaponCreated = FollowPlayer(x, Player, frame, weaponCreated, itemstoremove, oldDirrection);

                    if (frame > weaponTimer)
                    {
                        itemstoremove.Add(x);

                        weaponCreated = false;
                    }

                    break;

                }

            }

            return weaponCreated;
        }

        public bool FollowPlayer(Rectangle z, Rectangle PlayerCharacter, int frame, bool weaponCreated, List<Rectangle> itemstoremove, string oldDirrection)
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
        public BulletMaker bulletType = new("", "", "", 0, 0, 0,0,0,0);



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