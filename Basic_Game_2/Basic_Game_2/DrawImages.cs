using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Windows.Threading;


namespace Basic_Game_2
{

    // Draw all Objects
    public class Draw
    {

        public Rectangle Rect;

        public Draw(string tag, int height, int width, int x, int y, string imageName, string name, Canvas ThisCanvas)
        {

            ImageBrush image = new ImageBrush();

            string fileName = $"data-files/images/{imageName}.png";
            string fullPath = System.IO.Path.GetFullPath(fileName);

            image.ImageSource = new BitmapImage(new Uri(fullPath));

            Rectangle newRect = new Rectangle
            {
                Name = name,
                Tag = tag,
                Height = height,
                Width = width,
                Fill = image
            };

            Rect = newRect;

            Canvas.SetLeft(newRect, x);
            Canvas.SetTop(newRect, y);

            ThisCanvas.Children.Add(newRect);

            // Collect Garbage
            GC.Collect(); // collect any unused resources for this game
        }

    }

    // Draw Health Bar
    public class DrawHealthBar
    {

        public ProgressBar Self;

        public DrawHealthBar(int TotalEnemy, double health, double enemyWidth, List<ProgressBar> healthBarList, int x, int y, Canvas PlayerSpace)
        {

            ProgressBar newHealthbar = new ProgressBar
            {
                Name = "healthbar",
                Tag = $"enemy-{TotalEnemy - 1}-bar",
                Height = 10,
                Width = 50,
                Maximum = health,
                Value = health
            };

            Self = newHealthbar;

            Canvas.SetLeft(newHealthbar, x - (enemyWidth / 2));
            Canvas.SetTop(newHealthbar, y);

            PlayerSpace.Children.Add(newHealthbar);

            // Collect Garbage
            GC.Collect(); // collect any unused resources for this game
        }
    }

    // Draw Boss Health
    public class DrawBossHealthBar
    {
        public DrawBossHealthBar(double health, List<ProgressBar> healthBarList, Canvas PlayerSpace)
        {

            ProgressBar newHealthbar = new ProgressBar
            {
                Name = "healthbar",
                Tag = $"BossHealth",
                Height = 10,
                Width = 500,
                Maximum = health,
                Value = health
            };

            Canvas.SetLeft(newHealthbar, 80);
            Canvas.SetTop(newHealthbar, 80);

            PlayerSpace.Children.Add(newHealthbar);

            // Collect Garbage
            GC.Collect(); // collect any unused resources for this game
        }
    }



    // Fill Object
    public class FillDraw
    {
        public FillDraw(string imageName, Rectangle ThisObject)
        {
            ImageBrush image = new ImageBrush();

            string fileName = $"data-files/images/{imageName}.png";
            string fullPath = System.IO.Path.GetFullPath(fileName);

            image.ImageSource = new BitmapImage(new Uri(fullPath));

            ThisObject.Fill = image;
        }
    }

    // Draw Textblock
    public class DrawTextBlock
    {
        public DrawTextBlock(string tagName, int height, int width, int x, int y, int size, string text, Canvas PlayerSpace)
        {
            TextBlock textBlock = new TextBlock
            {
                Tag = tagName,
                Height = height,
                Width = width,
                Text = text,
                FontSize = size
            };

            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);

            PlayerSpace.Children.Add(textBlock);

            // Collect Garbage
            GC.Collect(); // collect any unused resources for this game
        }
    }

}