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
        public void PlaceBomb()
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && bomb > 0)
            {
                bombList.Add(new("bomb", "A bomb that goes boom", bombList.Count, "bomb", 25, 25, 20, Canvas.GetLeft(Player), Canvas.GetTop(Player), ItemSpace, new($"explosion-{bombList.Count}", "explosion", 60, 60, 50, 10, 40, Canvas.GetLeft(Player), Canvas.GetTop(Player) - 10, ItemSpace)));

                bomb -= 1;

                TotalPartyInv.Text = $" Coin: {coin} \n Ammo: {ammo} \n Holy Cross: {holyCross} \n Key: {key} \n Bomb:{bomb}";

                LogBox.Text += $"{playerList[currentPlayer].name} dropped a bomb! \n";

                UpdateUi();
                ScrollBar.ScrollToEnd();
            }




            for (int i = 0; i < bombList.Count; i++)
            {

                bombList[i].fuse++;

                if (bombList[i].fuse > bombList[i].fuseMax)
                {

                    bool bombExists = true;

                    foreach (Rectangle bomb in ItemSpace.Children.OfType<Rectangle>())
                    {
                        if ((string)bomb.Tag == $"{bombList[i].tag}")
                        {
                            explosionList.Add(bombList[i].explosion);
                            itemstoremove.Add(bomb);

                            bombExists = false;
                        }
                    }

                    if (bombExists == false)
                    {
                        bombList[i].explosion.ExplosionGoBoom(ItemSpace);
                    }


                }


            }


        }


        public void Explosion()
        {
            foreach (Rectangle explosion in ItemSpace.Children.OfType<Rectangle>())
            {
                for (int i = 0; i < explosionList.Count; i++)
                {
                    if ((string)explosion.Tag == $"explosion-{i}")
                    {
                        explosionList[i].fuse++;

                        if (explosionList[i].fuse > explosionList[i].fuseMax)
                        {
                            LogBox.Text += $"Bomb exploaded! \n";

                            UpdateUi();
                            ScrollBar.ScrollToEnd();

                            itemstoremove.Add(explosion);
                        }
                    }

                }

            }

        }

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
                                    var Enemy = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);



                                    if (Enemy.IntersectsWith(Explosion))
                                    {

                                        currentPlayer = enemyStats[i].calculateBombDamage(explosionList[w], LogBox, UpdateUi, ScrollBar, healthBarList[i], currentPlayer, PlayerUiBox);
                                        enemyStats[i].checkIfDead(itemstoremove, progressstoremove, x, healthBarList[i], PlayerSpace, i);

                                    }
                                }
                            }


                            for (int i = 0; i < itemStats.Count; i++)
                            {
                                if ((string)x.Tag == $"item-{i}")
                                {
                                    Rect item = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                                    if (Explosion.IntersectsWith(item))
                                    {
                                        itemStats[i].takeDamage(100);
                                        SaveMap(x, "0");
                                        itemstoremove.Add(x);
                                    }

                                }

                            }

                        }








                        if (invisibilityFrame > playerList[currentPlayer].invisibiltyFrames)
                        {
                            invisibilityFrame = 0;
                            PlayerIsHit = false;
                        }

                        // Check out Player
                        if (PlayerHtBox.IntersectsWith(Explosion) && PlayerIsHit == false)
                        {

                            PlayerIsHit = true;
                            currentPlayer = playerList[currentPlayer].BombDamaged(explosionList[w].damage, LogBox, UpdateUi, ScrollBar, PlayerUiBox, healthBarList[0], currentPlayer);

                            currentPlayer = playerList[currentPlayer].CheckIfDead(currentPlayer, PlayerUiBox, "BOMB", LogBox, ScrollBar, UpdateUi);
                        }


                    }

                }
            }



        }

    }





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

        public void ExplosionGoBoom(Canvas ThisCanvas)
        {
            _ = new Draw(tag, Convert.ToInt16(height), Convert.ToInt16(width), Convert.ToInt16(x), Convert.ToInt16(y), imageName, "explosion", ThisCanvas);
        }
    }

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


            _ = new Draw(tag, Convert.ToInt16(height), Convert.ToInt16(width), this.xBomb, this.yBomb, imageName, name, ThisCanvas);
        }
    }


}