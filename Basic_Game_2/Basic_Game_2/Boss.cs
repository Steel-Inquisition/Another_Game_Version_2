using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;


// If attacked, the boss spawns frogs in the middle. Maybe gets smaller as well.


namespace Basic_Game_2
{

    public partial class MainWindow : Window
    {
        // Heads
        public List<HeadMaker> HeadMakerList = new();
        // Hand
        public List<HandMaker> HandMakerList = new();


        public void MakeArmPusherBoss()
        {
            // Heads
            HeadMakerList.Add(new HeadMaker(2, 50, ItemSpace));

            // Health Bar
            _ = new DrawBossHealthBar(1200, healthBarList, ItemSpace);

            // Hands
            HandMakerList.Add(new HandMaker(1, 20, 105, 80, "BossHands1", ItemSpace));
            HandMakerList.Add(new HandMaker(1, 20, 505, 80, "BossHand2", ItemSpace));
        }

        public void ArmPusherBossLogic(Rect PlayerHitbox)
        {
            // Move Hand
            foreach (HandMaker handMaker in HandMakerList)
            {
                handMaker.MoveHand(ItemSpace, Player);
            }

            HeadMakerList[0].MoveHead(ItemSpace, Player);

            // Laser Fire
            if (!HeadMakerList[0].fired)
            {
                HeadMakerList[0].LaserTimer++;

                if (HeadMakerList[0].LaserTimer > 200)
                {
                    HeadMakerList[0].LaserFire(ItemSpace, BulletCanvas, HandMakerList);
                    HeadMakerList[0].fired = true;
                    HeadMakerList[0].LaserTimer = 0;
                }
            }


            if (HeadMakerList[0].fired)
            {
                // Laser Follow Player
                HeadMakerList[0].FollowLaser(BulletCanvas, ItemSpace);
                HeadMakerList[0].LaserTimer++;

                if (HeadMakerList[0].LaserTimer > 100)
                {
                    foreach (Rectangle z in BulletCanvas.Children.OfType<Rectangle>())
                    {
                        if ((string)z.Tag == "laser")
                        {
                            itemstoremove.Add(z);
                            HeadMakerList[0].fired = false;
                            HeadMakerList[0].LaserTimer = 0;
                        }
                    }
                }

            }






            // Check Damage

            HeadMakerList[0].CheckForDamage(ItemSpace, playerList[currentPlayer].weapon, UpdateBossHealthBar, enemyStats, bulletList, difficulty, healthBarList, enemyWeaponList, PlayerHitbox, playerList, currentPlayer, PlayerIsHit);
        }


        public void UpdateBossHealthBar()
        {

            foreach (ProgressBar z in ItemSpace.Children.OfType<ProgressBar>())
            {
                if ((string)z.Tag == "BossHealth")
                {
                    z.Value = HeadMakerList[0].health;

                    break;
                }

            }
        }

    }


    public class ArmBoss
    {
        public double health = 1200;
        public string damageType = "phys";
        public int LaserTimer = 0;
    }

    public class HeadMaker : ArmBoss
    {
        public double speed;
        public double damage;
        public bool fired = false;
        public int FireTime = 0;

        public HeadMaker(double speed, double damage, Canvas ItemSpace)
        {
            this.speed = speed;
            this.damage = damage;

            _ = new Draw("BossHead", 160, 160, 250, 150, "frog", "Boss", ItemSpace);
        }

        public void MoveHead(Canvas ItemSpace, Rectangle Player)
        {
            foreach (Rectangle x in ItemSpace.Children.OfType<Rectangle>())
            {


                if ((string)x.Tag == "BossHead")
                {
                    if (Canvas.GetTop(x) + (x.Height / 2) < Canvas.GetTop(Player) + (Player.Height / 2) && Canvas.GetTop(x) > 135) // if the hand is above the player
                    {

                        Canvas.SetTop(x, (Canvas.GetTop(x) - speed));

                    }
                    else if (Canvas.GetTop(x) + (x.Width / 2) > Canvas.GetTop(Player) + (Player.Width / 2) && Canvas.GetTop(x) < 345) // if the hand is bellow the player
                    {

                        Canvas.SetTop(x, (Canvas.GetTop(x) + speed));
                    }
                }


            }

        }


        public void CheckForDamage(Canvas ItemSpace, WeaponMaker PlayerWeapon, Action UpdateBossHealthBar, List<EnemyMaker> enemyStats, List<BulletMaker> bulletList, int[] difficulty, List<ProgressBar> healthBarList, List<WeaponMaker> enemyWeaponList, Rect PlayerHitbox, List<PlayerMaker> playerList, int currentPlayer, bool PlayerIsHit)
        {

            bool spawnEnemy = false;

            foreach (Rectangle x in ItemSpace.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "BossHead")
                {
                    var Enemy = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    foreach (Rectangle y in ItemSpace.Children.OfType<Rectangle>())
                    {

                        if ((string)y.Tag == "melee" || (string)y.Tag == "ranged")
                        {
                            var Weapon = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (Enemy.IntersectsWith(Weapon))
                            {
                                health -= PlayerWeapon.damage;

                                UpdateBossHealthBar();

                                Random rand = new();

                                int teleportY = rand.Next(135, 345);
                                int teleportX = rand.Next(105, 475);

                                Canvas.SetTop(x, teleportY);
                                Canvas.SetLeft(x, teleportX);

                                CheckIfDead();

                                spawnEnemy = true;
                            }

                        }

                    }
                }

            }

            if (spawnEnemy)
            {
                double[] visionCone = { 1000, 1000 };

                enemyStats.Add(new EnemyMaker("slime", "slime", $"enemy-{enemyStats.Count - 1}", "slime", "zombie", 75, 0, 75, 0, 0, 25, enemyWeaponList[0], 30, 0, 0, 50, 0, 4, 0, 40, 30, visionCone, false, 10, 10, enemyStats.Count, healthBarList, 25, new(10, 0, 0, 0, 0, 0, 0, 0, 0), bulletList[0], ItemSpace));
            }

        }


        public void CheckIfDead()
        {
            if (health <= 0)
            {

                MessageBox.Show("You Won The Game!!!");


                // EndGame();
            }
        }




        public void LaserFire(Canvas ItemSpace, Canvas BulletCanvas, List<HandMaker> HandMakerList)
        {

            int x = 0;
            int y = 0;


            foreach (Rectangle z in ItemSpace.Children.OfType<Rectangle>())
            {
                if ((string)z.Tag == "BossHands1")
                {
                    x = Convert.ToInt32(Canvas.GetLeft(z)) + 40;
                    y = Convert.ToInt32(Canvas.GetTop(z) + (z.Height / 2) - 20);

                    break;
                }

            }

            _ = new Draw("laser", 40, 360, x, y, "fireball", "fire", BulletCanvas);


        }

        public void FollowLaser(Canvas BulletCanvas, Canvas ItemSpace)
        {
            foreach (Rectangle z in BulletCanvas.Children.OfType<Rectangle>())
            {
                if ((string)z.Tag == "laser")
                {
                    foreach (Rectangle x in ItemSpace.Children.OfType<Rectangle>())
                    {
                        if ((string)x.Tag == "BossHands1")
                        {
                            Canvas.SetTop(z, Canvas.GetTop(x) + (x.Height / 2) - (z.Height / 2));
                        }
                    }

                }
            }

        }


    }



    public class HandMaker : ArmBoss
    {
        public double speed;
        public double damage;
        public string tag;
        public bool CanMove = true;

        public HandMaker(double speed, double damage, int x, int y, string tag, Canvas ItemSpace)
        {
            this.speed = speed;
            this.damage = damage;
            this.tag = tag;

            _ = new Draw(tag, 80, 50, x, y, "frog", "Boss", ItemSpace);
        }

        public void MoveHand(Canvas ItemSpace, Rectangle Player)
        {
            foreach (Rectangle x in ItemSpace.Children.OfType<Rectangle>())
            {


                if ((string)x.Tag == tag && CanMove)
                {
                    if (Canvas.GetTop(x) < Canvas.GetTop(Player)) // if the hand is above the player
                    {
                        Canvas.SetTop(x, (Canvas.GetTop(x) + speed));

                    }
                    else if (Canvas.GetTop(x) > Canvas.GetTop(Player)) // if the hand is bellow the player
                    {
                        Canvas.SetTop(x, (Canvas.GetTop(x) - speed));
                    }
                }


            }
        }


    }

}