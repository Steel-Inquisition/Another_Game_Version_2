using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;


namespace Basic_Game_2
{

    public partial class MainWindow : Window
    {

        // Press Bomb to drop it
        public void PlaceBomb()
        {

            // When placed bomb and have enoguh bombs
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && bomb > 0)
            {

                // Add a bomb to the list
                bombList.Add(new("bomb", "A bomb that goes boom", bombList.Count, "bomb", 25, 25, 20, Canvas.GetLeft(Player), Canvas.GetTop(Player), ItemSpace, new($"explosion-{bombList.Count}", "explosion", 60, 60, 50, 10, 40, Canvas.GetLeft(Player), Canvas.GetTop(Player) - 10, ItemSpace)));

                // Subtract bomb amount
                bomb -= 1;

                // show new amount of bombs
                TotalPartyInv.Text = $" Coin: {coin} \n Ammo: {ammo} \n Holy Cross: {holyCross} \n Key: {key} \n Bomb:{bomb}";

                // Show that a bomb was dropped!
                LogBox.Text += $"{playerList[currentPlayer].name} dropped a bomb! \n";

                // Update UI to show change
                UpdateUi();
                ScrollBar.ScrollToEnd();
            }




            for (int i = 0; i < bombList.Count; i++)
            {

                // Increase bomb fuse
                bombList[i].fuse++;

                // if the bomb fuse is above the fuse max
                if (bombList[i].fuse > bombList[i].fuseMax)
                {
                    // bomb now exists
                    bool bombExists = true;

                    foreach (Rectangle bomb in ItemSpace.Children.OfType<Rectangle>())
                    {
                        if ((string)bomb.Tag == $"{bombList[i].tag}")
                        {
                            // add the explosions
                            explosionList.Add(bombList[i].explosion);

                            // remove bomb
                            itemstoremove.Add(bomb);

                            // bomb no longer exists
                            bombExists = false;
                        }
                    }

                    // if bomb no longer exists
                    if (bombExists == false)
                    {
                        // draw the explosion
                        bombList[i].explosion.ExplosionGoBoom(ItemSpace);
                    }


                }


            }


        }

        // explosions
        public void Explosion()
        {
            foreach (Rectangle explosion in ItemSpace.Children.OfType<Rectangle>())
            {
                for (int i = 0; i < explosionList.Count; i++)
                {
                    if ((string)explosion.Tag == $"explosion-{i}")
                    {

                        // increase explosion fuse
                        explosionList[i].fuse++;

                        // if the fuse runs out
                        if (explosionList[i].fuse > explosionList[i].fuseMax)
                        {

                            // bomb goes boom
                            LogBox.Text += $"Bomb exploaded! \n";

                            // update UI
                            UpdateUi();
                            ScrollBar.ScrollToEnd();

                            // destroy explosion
                            itemstoremove.Add(explosion);
                        }
                    }

                }

            }

        }

        // when the bomb exploads
        public void BombExpload(Rect PlayerHtBox)
        {
            foreach (Rectangle y in ItemSpace.Children.OfType<Rectangle>())
            {
                for (int w = 0; w < explosionList.Count; w++)
                {
                    if ((string)y.Tag == $"explosion-{w}")
                    {
                        var Explosion = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);


                        // Check Out Enemy
                        foreach (Rectangle x in ItemSpace.Children.OfType<Rectangle>())
                        {
                            for (int i = 0; i < enemyStats.Count; i++)
                            {
                                if ((string)x.Tag == $"enemy-{i}")
                                {

                                    // get enemy hit box
                                    var Enemy = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);


                                    // if the enemy hits the explpsion
                                    if (Enemy.IntersectsWith(Explosion))
                                    {
                                        // dead damage
                                        currentPlayer = enemyStats[i].calculateBombDamage(explosionList[w], LogBox, UpdateUi, ScrollBar, healthBarList[i], currentPlayer, PlayerUiBox);

                                        // check if enemy is dead
                                        enemyStats[i].checkIfDead(itemstoremove, progressstoremove, x, healthBarList[i], PlayerSpace, i);

                                    }
                                }
                            }

                            // search for broken walls
                            for (int i = 0; i < itemStats.Count; i++)
                            {
                                if ((string)x.Tag == $"item-{i}")
                                {
                                    Rect item = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                                    if (Explosion.IntersectsWith(item))
                                    {

                                        // destroy wall
                                        itemStats[i].takeDamage(100);

                                        // save this change
                                        SaveMap(x, "0");

                                        // destroy the wall from the canvas
                                        itemstoremove.Add(x);
                                    }

                                }

                            }

                        }







                        // inivisbility frames for player
                        if (invisibilityFrame > playerList[currentPlayer].invisibiltyFrames)
                        {
                            invisibilityFrame = 0;
                            PlayerIsHit = false;
                        }

                        // Check out Player
                        if (PlayerHtBox.IntersectsWith(Explosion) && PlayerIsHit == false)
                        {
                            // player is hit
                            PlayerIsHit = true;

                            // player is delt damage
                            currentPlayer = playerList[currentPlayer].BombDamaged(explosionList[w].damage, LogBox, UpdateUi, ScrollBar, PlayerUiBox, healthBarList[0], currentPlayer);

                            // player is dead
                            currentPlayer = playerList[currentPlayer].CheckIfDead(currentPlayer, PlayerUiBox, "BOMB", LogBox, ScrollBar, UpdateUi);
                        }


                    }

                }
            }



        }

    }




    // explosion class
    public class ExplosionMaker
    {
        public int width;
        public int height;

        public string tag;
        public string imageName;

        public int damage;
        public int knockback;

        public double x;
        public double y;

        public int fuse = 0;
        public int fuseMax;

        public ExplosionMaker(string tag, string imageName, int width, int height, int damage, int knockback, int fuseMax, double x, double y, Canvas PlayerSpace)
        {
            this.tag = tag;
            this.width = width;
            this.imageName = imageName;
            this.height = height;
            this.damage = damage;
            this.knockback = knockback;
            this.fuseMax = fuseMax;
            this.x = x;
            this.y = y;
        }


        // create explosion
        public void ExplosionGoBoom(Canvas ThisCanvas)
        {
            _ = new Draw(tag, Convert.ToInt16(height), Convert.ToInt16(width), Convert.ToInt16(x), Convert.ToInt16(y), imageName, "explosion", ThisCanvas);
        }
    }

    // bomb class
    public class BombMaker
    {
        public string name;
        public string description;
        public string tag;
        public string imageName;

        public int width;
        public int height;

        public int xBomb;
        public int yBomb;


        public int fuse = 0;
        public int fuseMax;

        public bool bombExploaded = false;


        public ExplosionMaker explosion;


        public BombMaker(string name, string description, int count, string imageName, int width, int height, int fuseMax, double xBomb, double yBomb, Canvas ThisCanvas, ExplosionMaker explosion)
        {
            this.name = name;
            this.description = description;
            tag = $"bomb-{count}";
            this.imageName = imageName;

            this.explosion = explosion;

            this.width = width;
            this.height = height;

            this.fuseMax = fuseMax;

            this.xBomb = Convert.ToInt16(xBomb);
            this.yBomb = Convert.ToInt16(yBomb);


            // create bombs
            _ = new Draw(tag, Convert.ToInt16(height), Convert.ToInt16(width), this.xBomb, this.yBomb, imageName, name, ThisCanvas);
        }
    }


}