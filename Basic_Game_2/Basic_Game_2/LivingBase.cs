﻿using System;
// For the console
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Collections.Generic;


namespace Basic_Game_2
{
    public class LivingBase
    {
        public string name = "";
        public string playerClass = "";
        public double health;
        public double healthMax;
        public bool PlayerOrEnemy = true;

        public string currentDirrection = "left";

        public double mp;
        public double mpMax;
        public double phys;
        public double magic;
        public double gun;
        public double physDef;
        public double magDef;
        public double speed;
        public double mpRegen;
        public double size;

        public double speedX = 0;
        public double speedY = 0;

        public WeaponMaker weapon;

        public bool[] ifTouchWall = { false, false, false, false };


        public int TakeDamage(double damage, string Attacker, int currentPlayer, TextBlock LogBox, Action UpdateUi, Canvas PlayerUiBox, ScrollViewer ScrollBar, ProgressBar CurrentProgressBar)
        {
            this.health -= damage;

            LogBox.Text += $"{Attacker} did {damage} damage to {name}!";

            if (health < 0)
            {
                LogBox.Text += $"{Attacker} killed the {name}! \n";
            }

            if (PlayerOrEnemy)
            {
                currentPlayer = CheckDeath(PlayerUiBox, LogBox, Attacker, currentPlayer);
            }
            else
            {
                CurrentProgressBar.Value = health;
            }

            UpdateUi();
            ScrollBar.ScrollToEnd();

            return currentPlayer;
        }

        public int CheckDeath(Canvas PlayerUiBox, TextBlock LogBox, string Attacker, int currentPlayer)
        {
            if (health <= 0)
            {
                foreach (Rectangle x in PlayerUiBox.Children.OfType<Rectangle>())
                {
                    if ((string)x.Tag == $"PlayerUiBackground-{currentPlayer}")
                    {
                        _ = new FillDraw("ui/backgroundDead", x);

                    }

                }

                LogBox.Text += $"{name} is dead! Killed by {Attacker}! \n";

            }

            return currentPlayer;
        }


        public void basic_movement(Rectangle Self, Rectangle Other, string type, Canvas PlayerSpace, Canvas ItemSpace)
        {

            if (type == "PLAYER")
            {

                if (Keyboard.IsKeyDown(Key.Left) && ifTouchWall[2] == false)
                {

                    speedX -= speed;

                    currentDirrection = "left";


                }
                else if (Keyboard.IsKeyDown(Key.Right) && ifTouchWall[3] == false)
                {
                    speedX += speed;

                    currentDirrection = "right";



                }
                else if (Keyboard.IsKeyDown(Key.Up) && ifTouchWall[0] == false)
                {
                    speedY -= speed;

                    currentDirrection = "up";



                }
                else if (Keyboard.IsKeyDown(Key.Down) && ifTouchWall[1] == false)
                {
                    speedY += speed;

                    currentDirrection = "down";


                }

                CheckIfWallCollide(Self, PlayerSpace);
                CheckIfWallCollide(Self, ItemSpace);

            }
            else // Enemy Movemenet
            {
                if (Canvas.GetLeft(Self) < Canvas.GetLeft(Other))
                {
                    speedX += speed;

                    currentDirrection = "right";
                }
                else if (Canvas.GetLeft(Self) > (Canvas.GetLeft(Other)))
                {
                    speedX -= speed;

                    currentDirrection = "left";
                }

                CheckIfWallCollide(Self, PlayerSpace);
                CheckIfWallCollide(Self, ItemSpace);

                if (Canvas.GetTop(Self) < Canvas.GetTop(Other))
                {
                    speedY += speed;

                    currentDirrection = "down";
                }
                else if (Canvas.GetTop(Self) > Canvas.GetTop(Other))
                {
                    speedX -= speed;

                    currentDirrection = "up";

                }

                CheckIfWallCollide(Self, PlayerSpace);
                CheckIfWallCollide(Self, ItemSpace);

                Enemy_movement(Self, Other, speedX, speedY, type);

            }

            double moveLeft = speedX;
            double moveUp = speedY;

            if (type == "PLAYER")
            {

                if (ifTouchWall[0] == false && currentDirrection == "up")
                {
                    Canvas.SetTop(Self, Canvas.GetTop(Self) + moveUp);

                }
                else if (ifTouchWall[1] == false && currentDirrection == "down")
                {
                    Canvas.SetTop(Self, Canvas.GetTop(Self) + moveUp);
                }
                else if (ifTouchWall[2] == false && currentDirrection == "left")
                {
                    Canvas.SetLeft(Self, Canvas.GetLeft(Self) + moveLeft);
                }
                else if (ifTouchWall[3] == false && currentDirrection == "right")
                {
                    Canvas.SetLeft(Self, Canvas.GetLeft(Self) + moveLeft);
                }
            }


            (ifTouchWall[0], ifTouchWall[1], ifTouchWall[2], ifTouchWall[3]) = (false, false, false, false);

            speedX = 0;
            speedY = 0;
        }

        public void Enemy_movement(Rectangle Self, Rectangle Other, double moveLeft, double moveUp, string type)
        {
            if (type == "ENEMY_ZOMBIE")
            {

                if (Canvas.GetLeft(Self) < Canvas.GetLeft(Other) && ifTouchWall[3] == false) // if the enemy is to the left of the player 
                {
                    Canvas.SetLeft(Self, (Canvas.GetLeft(Self) + moveLeft));

                    currentDirrection = "right";

                }
                else if (Canvas.GetLeft(Self) > (Canvas.GetLeft(Other)) && ifTouchWall[2] == false) // if the enemy is to the right of the player 
                {
                    Canvas.SetLeft(Self, (Canvas.GetLeft(Self) + moveLeft));

                    currentDirrection = "left";

                }
                if (Canvas.GetTop(Self) < Canvas.GetTop(Other) && ifTouchWall[0] == false) // if the enemy is above the player
                {
                    Canvas.SetTop(Self, (Canvas.GetTop(Self) + moveUp));

                    currentDirrection = "down";

                }
                else if (Canvas.GetTop(Self) > Canvas.GetTop(Other) && ifTouchWall[1] == false) // if the enemy is bellow the player
                {
                    Canvas.SetTop(Self, (Canvas.GetTop(Self) + moveUp));

                    currentDirrection = "up";

                }

            }
            else if (type == "ENEMY_SHOOTER")
            {

                if (Canvas.GetLeft(Self) < Canvas.GetLeft(Other) && ifTouchWall[3] == false) // if the enemy is to the left of the player 
                {
                    Canvas.SetLeft(Self, (Canvas.GetLeft(Self) + moveLeft));

                    currentDirrection = "right";

                }
                else if (Canvas.GetLeft(Self) > (Canvas.GetLeft(Other) + Other.Width) && ifTouchWall[2] == false) // if the enemy is to the right of the player 
                {
                    Canvas.SetLeft(Self, (Canvas.GetLeft(Self) + moveLeft));

                    currentDirrection = "left";

                }
                else if (Canvas.GetTop(Self) < Canvas.GetTop(Other) && ifTouchWall[0] == false) // if the enemy is above the player
                {
                    Canvas.SetTop(Self, (Canvas.GetTop(Self) + moveUp));

                    currentDirrection = "down";

                }
                else if (Canvas.GetTop(Self) > Canvas.GetTop(Other) && ifTouchWall[1] == false) // if the enemy is bellow the player
                {
                    Canvas.SetTop(Self, (Canvas.GetTop(Self) + moveUp));

                    currentDirrection = "up";
                }

            }
        }



        public void CheckIfWallCollide(Rectangle Self, Canvas ThisOne)
        {
            foreach (Rectangle x in ThisOne.Children.OfType<Rectangle>())
            {


                if ((string)x.Tag == "wall" || (string)x.Tag == "water" || x.Name == "crate" || x.Name == "crate2" || x.Name == "crate3" || x.Name == "door" || x.Name == "fakewall" || x.Name == "statue" || x.Name == "table" || x.Name == "chair")
                {
                    var Wall = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);


                    if (currentDirrection == "up")
                    {


                        // Check if it hits the above box and stop player from entering it by speed and momentum of player
                        Rect upBox = new Rect(Canvas.GetLeft(Self), Canvas.GetTop(Self) - Math.Abs(speedY), Self.Width, Self.Height);


                        if ((upBox).IntersectsWith(Wall))
                        {
                            ifTouchWall[0] = true;

                            break;

                        }


                    }
                    else if (currentDirrection == "down")
                    {
                        Rect downBox = new Rect(Canvas.GetLeft(Self), Canvas.GetTop(Self) + Math.Abs(speedY), Self.Width, Self.Height);


                        if ((downBox).IntersectsWith(Wall))
                        {
                            ifTouchWall[1] = true;

                            break;
                        }


                    }
                    else if (currentDirrection == "left")
                    {
                        Rect leftBox = new Rect(Canvas.GetLeft(Self) - Math.Abs(speedX), Canvas.GetTop(Self), Self.Width, Self.Height);

                        if ((leftBox).IntersectsWith(Wall))
                        {
                            ifTouchWall[2] = true;

                            break;


                        }


                    }
                    else if (currentDirrection == "right")
                    {
                        Rect rightBox = new Rect(Canvas.GetLeft(Self) + Math.Abs(speedX), Canvas.GetTop(Self), Self.Width, Self.Height);


                        if ((rightBox).IntersectsWith(Wall))
                        {
                            ifTouchWall[3] = true;

                            break;
                        }

                    }
                }


            }
        }


        public void KnockBack(Canvas PlayerSpace, Canvas ItemSpace, Rectangle Self, string knockback_dirrection, double knockback)
        {

            EnemyKnockBackChcker(PlayerSpace, Self, knockback_dirrection, knockback, ifTouchWall);
            EnemyKnockBackChcker(ItemSpace, Self, knockback_dirrection, knockback, ifTouchWall);


            if (knockback_dirrection == "up" && ifTouchWall[1] == false)
            {
                Canvas.SetTop(Self, Canvas.GetTop(Self) - knockback);
            }
            else if (knockback_dirrection == "down" && ifTouchWall[0] == false)
            {
                Canvas.SetTop(Self, Canvas.GetTop(Self) + knockback);
            }
            else if (knockback_dirrection == "left" && ifTouchWall[2] == false)
            {
                Canvas.SetLeft(Self, Canvas.GetLeft(Self) - knockback);
            }
            else if (knockback_dirrection == "right" && ifTouchWall[3] == false)
            {
                Canvas.SetLeft(Self, Canvas.GetLeft(Self) + knockback);
            }

            (ifTouchWall[0], ifTouchWall[1], ifTouchWall[2], ifTouchWall[3]) = (false, false, false, false);
        }



        public bool[] EnemyKnockBackChcker(Canvas ThisSpace, Rectangle EnemyPosition, string dirrection, double changeBy, bool[] ifTouchWall)
        {


            foreach (Rectangle x in ThisSpace.Children.OfType<Rectangle>())
            {


                if ((string)x.Tag == "wall" || (string)x.Tag == "water" || x.Name == "crate" || x.Name == "crate2" || x.Name == "crate3" || x.Name == "door" || x.Name == "fakewall" || x.Name == "statue" || x.Name == "table" || x.Name == "chair")
                {
                    var Wall = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);


                    if (dirrection == "up")
                    {


                        // Check if it hits the above box and stop player from entering it by speed and momentum of player
                        Rect upBox = new Rect(Canvas.GetLeft(EnemyPosition), Canvas.GetTop(EnemyPosition) - Math.Abs(changeBy), EnemyPosition.Width, EnemyPosition.Height);


                        if ((upBox).IntersectsWith(Wall))
                        {

                            ifTouchWall[1] = true;
                            Canvas.SetTop(EnemyPosition, Canvas.GetTop(x) + size);


                        }


                    }
                    else if (dirrection == "down")
                    {
                        Rect downBox = new Rect(Canvas.GetLeft(EnemyPosition), Canvas.GetTop(EnemyPosition) + Math.Abs(changeBy), EnemyPosition.Width, EnemyPosition.Height);


                        if ((downBox).IntersectsWith(Wall))
                        {
                            ifTouchWall[0] = true;
                            Canvas.SetTop(EnemyPosition, Canvas.GetTop(x) - size);
                        }


                    }
                    else if (dirrection == "left")
                    {
                        Rect leftBox = new Rect(Canvas.GetLeft(EnemyPosition) - Math.Abs(changeBy), Canvas.GetTop(EnemyPosition), EnemyPosition.Width, EnemyPosition.Height);

                        if ((leftBox).IntersectsWith(Wall))
                        {
                            ifTouchWall[2] = true;
                            Canvas.SetLeft(EnemyPosition, Canvas.GetLeft(x) + size);

                        }


                    }
                    else if (dirrection == "right")
                    {
                        Rect rightBox = new Rect(Canvas.GetLeft(EnemyPosition) + Math.Abs(changeBy), Canvas.GetTop(EnemyPosition), EnemyPosition.Width, EnemyPosition.Height);


                        if ((rightBox).IntersectsWith(Wall))
                        {
                            ifTouchWall[3] = true;

                            Canvas.SetLeft(EnemyPosition, Canvas.GetLeft(x) - EnemyPosition.Width);

                        }

                    }
                }


            }

            return ifTouchWall;
        }
    }



}