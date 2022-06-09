using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;


namespace Basic_Game_2
{

    public partial class MainWindow : Window
    {
        public void EnemyMovement(Rect PlayerHitBox)
        {
            bool DidEnemyFired = false;
            int saveThisNumber = 0;

            double saveX = 0;
            double saveY = 0;

            foreach (Rectangle x in ItemSpace.Children.OfType<Rectangle>())
            {
                for (int i = 0; i < enemyStats.Count; i++)
                {
                    if ((string)x.Tag == $"enemy-{i}")
                    {
                        var Enemy = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);


                        var EnemySenseor = new Rect(Canvas.GetLeft(x) - (enemyStats[i].visionBox[0] / 2), Canvas.GetTop(x) - (enemyStats[i].visionBox[1] / 2), (enemyStats[i].visionBox[0]), (enemyStats[i].visionBox[1]));

                        var WeaponAttack = new Rect(Canvas.GetLeft(x) - (enemyStats[i].weapon.width / 2), Canvas.GetTop(x) - (enemyStats[i].weapon.height / 2), (enemyStats[i].weapon.width), (enemyStats[i].weapon.height));


                        if (PlayerHitBox.IntersectsWith(EnemySenseor))
                        {

                            //
                            enemyStats[i].attracted = LineOfSight(i, x);
                            //

                        }


                        if (PlayerHitBox.IntersectsWith(EnemySenseor) && enemyStats[i].attracted && enemyStats[i].WeaponCreated == false && enemyStats[i].AllowWeapon == false)
                        {

                            enemyStats[i].AllowWeapon = true;


                        }

                        if (enemyStats[i].attracted)
                        {
                            enemyStats[i].basic_movement(x, Player, $"ENEMY_{enemyStats[i].type}", PlayerSpace, ItemSpace);
                        }


                        //BulletAccuracyX(x, rand, Convert.ToInt16(enemyStats[i].accuracy));
                        //BulletAccuracyY(x, rand, Convert.ToInt16(enemyStats[i].accuracy));

                        Canvas.SetTop(healthBarList[i], Enemy.Top - 10);
                        Canvas.SetLeft(healthBarList[i], Enemy.Left - (Enemy.Width / 2));

                        if (enemyStats[i].ranged == true && enemyStats[i].attracted == true)
                        {
                            enemyStats[i].shootRate++;

                            if (enemyStats[i].shootRate > enemyStats[i].firerate)
                            {
                                saveX = Canvas.GetLeft(x);
                                saveY = Canvas.GetTop(x);

                                DidEnemyFired = true;
                                saveThisNumber = i;

                                enemyStats[i].shootRate = 0;
                            }

                        }


                    }
                }
            }

            if (DidEnemyFired)
            {
                enemyStats[saveThisNumber].EnemyFire(bulletFired, Player, enemyStats[saveThisNumber].currentDirrection, BulletCanvas, saveX, saveY, bulletNum);
                bulletNum++;
            }




        }






        public bool LineOfSight(int i, Rectangle x)
        {
            foreach (Rectangle y in PlayerSpace.Children.OfType<Rectangle>())
            {
                if ((string)y.Tag == "wall" || (string)y.Tag == "water")
                {


                    if (LineRectangleCollision(Canvas.GetLeft(Player), Canvas.GetLeft(x), Canvas.GetTop(Player), Canvas.GetTop(x), Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height))
                    {

                        return false;

                    }


                }
            }

            foreach (Rectangle y in ItemSpace.Children.OfType<Rectangle>())
            {
                if (y.Name == "crate" || y.Name == "crate2" || y.Name == "crate3" || y.Name == "door" || y.Name == "fakewall" || y.Name == "statue" || y.Name == "table" || y.Name == "chair")
                {
                    if (LineRectangleCollision(Canvas.GetLeft(Player), Canvas.GetLeft(x), Canvas.GetTop(Player), Canvas.GetTop(x), Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height))
                    {

                        return false;

                    }
                }
            }


            return true;

        }
    }

    public class EnemyMaker : LivingBase
    {
        public string imageName = "";
        public string tagname = "";

        public double width;
        public double height;

        public double[] visionBox;

        public bool attracted = false;

        public int shootRate = 0;

        public bool AllowWeapon = false;

        public bool ranged;

        public string type = "";


        public bool WeaponCreated = false;

        public int thisCount = 0;


        // What The Enemy Drops
        public DropMaker drop;


        // For The Weapon
        public int innerFrame = 0;
        public int firerate;

        public int reaction = 0;
        public int reactionTime;

        public bool AllowCreateWeapon = true;


        // Bullets And Weapons Usage
        public BulletMaker bullet;

        // Diificulty
        public int[] difficulty = {};

        public double knockback;


        public EnemyMaker(string name, string imageName, string tagname, string playerClass, string type, double health, int mp, double healthMax, double mpMax, double mpRegen, double size, WeaponMaker weapon, double phys, double magic, double gun, double physDef, double magDef, double speed, int firerate, int reactionTime, int knockback, double[] visionBox, bool ranged, int row, int col, int count, List<ProgressBar> healthBarList, int wallSize, DropMaker drop, BulletMaker bullet, Canvas OnThisCanvas)
        {

            this.name = name;
            this.imageName = imageName;
            this.tagname = tagname;
            this.playerClass = playerClass;
            this.type = type;
            this.health = health;
            this.mp = mp;
            this.healthMax = healthMax;
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
            this.firerate = firerate;
            this.reactionTime = reactionTime;
            this.knockback = knockback;
            this.visionBox = visionBox;
            this.ranged = ranged;

            this.bullet = bullet;
            this.drop = drop;

            PlayerOrEnemy = false;

            _ = new Draw($"enemy-{count}", Convert.ToInt16(wallSize), Convert.ToInt16(wallSize), (row * wallSize) + 55, (col * wallSize) + 80, imageName, name, OnThisCanvas);

            _ = new DrawHealthBar(count - 1, health, width, healthBarList, (row * wallSize) + 55, (col * wallSize) + 70, OnThisCanvas);
        }


        public int calculateBombDamage(ExplosionMaker explosion, TextBlock LogBox, Action UpdateUi, ScrollViewer ScrollBar, ProgressBar CurrentProgressBar, int currentPlayer, Canvas PlayerUiBox)
        {
            currentPlayer = TakeDamage(explosion.damage, "bomb", currentPlayer, LogBox, UpdateUi, PlayerUiBox, ScrollBar, CurrentProgressBar);

            return currentPlayer;
        }


        public int calculateDamage(PlayerMaker CurrentPlayer, Rectangle Enemy, string dirrection, TextBlock LogBox, Action UpdateUi, ScrollViewer ScrollBar, ProgressBar CurrentProgressBar, Canvas PlayerSpace, Canvas ItemSpace, int currentPlayer, Canvas PlayerUiBox, List<EnemyMaker> enemyStats, int enemyTarget)
        {
            double damage = 0;

            damage += CurrentPlayer.weapon.damage;

            if (CurrentPlayer.weapon.damageType == "phys")
            {
                damage += ((CurrentPlayer.phys));
                damage -= physDef;
            }
            else if (CurrentPlayer.weapon.damageType == "magic")
            {
                damage += (CurrentPlayer.magic);
                damage -= magDef;
            }
            else if (CurrentPlayer.weapon.damageType == "gun")
            {
                damage += (CurrentPlayer.gun);
            }

            // Add knockback here
            KnockBack(PlayerSpace, ItemSpace, Enemy, CurrentPlayer.currentDirrection, CurrentPlayer.weapon.knockBack);


            currentPlayer = TakeDamage(damage, CurrentPlayer.name, currentPlayer, LogBox, UpdateUi, PlayerUiBox, ScrollBar, CurrentProgressBar);

            return currentPlayer;
        }



        public void EnemyFire(List<BulletMaker> bulletFired, Rectangle Player, string currentDirrection, Canvas BulletCanvas, double getX, double getY, int bulletNum)
        {
            // Get Current Bullet
            BulletMaker CurrentBullet = bullet;

            CurrentBullet.bulletDamage += gun;

            double x = getX;
            double y = getY;

            CurrentBullet.dirrection = currentDirrection;

            bulletFired.Add(CurrentBullet);

            CurrentBullet.tag = $"enemybullet-{bulletFired.Count - 1}-{currentDirrection}";

            _ = new Draw($"enemybullet-{bulletFired.Count - 1}-{currentDirrection}", CurrentBullet.bulletWidth, CurrentBullet.bulletHeight, Convert.ToInt16(x), Convert.ToInt16(y), $"weapons/10", CurrentBullet.name, BulletCanvas);

        }


        public void checkIfDead(List<Rectangle> itemstoremove, List<ProgressBar> progressstoremove, Rectangle x, ProgressBar y, Canvas PlayerSpace, int select)
        {

            if (this.health <= 0)
            {
                itemstoremove.Add(x);
                progressstoremove.Add(y);
            }
        }
    }


}