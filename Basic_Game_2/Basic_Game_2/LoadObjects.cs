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

    public partial class MainWindow : Window
    {

        // All Classes
        public void LoadClasses()
        {
            // maybe I should use a search thing instead of order thing...
            classMakerList.Add(new ClassMaker("Knight", "Knight", "This being is one made of metalic fury. Very basic class. Starts with a sword.", 10, -10, 10, -10, 0, 0, weaponList[1], 10, -10, -10, 10, 0, 0, 0));
            classMakerList.Add(new ClassMaker("Squire", "Squire", "One built from weakness and scared yet ambitious. High Speed, No Defence. Starts with a weak knife.", -20, -20, -20, -20, 1, 0, weaponList[0], 5, 0, 15, 5, 5, 4, 5));
            classMakerList.Add(new ClassMaker("Paladin", "Paladin", "One built from defence, yet low speed. Starts with a spear.", -20, -20, -20, -20, 1, 0, weaponList[2], 5, 0, 15, 5, 5, -2, 0));
            classMakerList.Add(new ClassMaker("Red Mage", "Red Mage", "One built from fury and destruction. High magic, low physical. Starts with Fire Book.", -20, -20, -20, -20, 1, 0, weaponList[4], 5, 0, 15, 5, 5, 2, 0));
            classMakerList.Add(new ClassMaker("Dark Mage", "Dark Mage", "They lurk in the dark yet are destroyed by it. Decent Magic and Decent phys, low defence. Starts with a strong Crimson Death.", -20, -20, -20, -20, 1, 0, weaponList[5], 5, 0, 15, 5, 5, 4, 0));
            classMakerList.Add(new ClassMaker("Gun Slinger", "Gunslinger", "The ones who stop the moving on. High gun, High phys, low magic, low magic defence. Starts with a .45 gun", -20, -20, -20, -20, 1, 0, weaponList[3], 5, 0, 15, 5, 5, 1, 0));
        }

        // All Weapons
        public void LoadWeapons()
        {
            weaponList.Add(new WeaponMaker("Knife", "knife", 20, "phys", 20, 20, 10, "melee", 20, 0, 0, "N/A", bulletList[0], 0));
            weaponList.Add(new WeaponMaker("sword", "sword", 30, "phys", 20, 40, 20, "melee", 20, 0, 0, "N/A", bulletList[0], 0));
            weaponList.Add(new WeaponMaker("spear", "spear", 15, "phys", 70, 20, 40, "melee", 0, 0, 0, "N/A", bulletList[0], 0));
            weaponList.Add(new WeaponMaker("gun45", ".45", 20, "gun", 10, 30, 10, "ranged", 20, 20, 0, "N/A", bulletList[1], 1)); // First Gun (1)
            weaponList.Add(new WeaponMaker("fireBook", "fire book", 80, "magic", 30, 20, 40, "melee", 0, 0, 60, "n/a", bulletList[0], 0));
            weaponList.Add(new WeaponMaker("crimsonDeath", "crimson death", 100, "magic", 30, 20, 40, "melee", 0, 0, 60, "n/a", bulletList[0], 0));
            weaponList.Add(new WeaponMaker("Ax", "ax", 80, "phys", 40, 40, 60, "melee", 0, 0, 0, "n/a", bulletList[0], 0));
            weaponList.Add(new WeaponMaker("Black Hole", "blackhole", 150, "magic", 60, 60, 0, "melee", 0, 0, 150, "n/a", bulletList[0], 0));
            weaponList.Add(new WeaponMaker("Ak-47", "ak", 30, "gun", 30, 20, 40, "ranged", 0, 2, 0, "n/a", bulletList[4], 3));
        }


        // All enemy weapons
        public void LoadEnemyWeapons()
        {
            enemyWeaponList.Add(new WeaponMaker("Swipe", "knife", 50, "phys", 20, 40, 20, "phys", 20, 0, 0, "n/a", bulletList[0], 0));
        }


        // All bullets
        public void LoadBullets()
        {
            bulletList.Add(new BulletMaker("None", "Nothing", "striaght", 0, 0, 0, 0, 0, 0));
            bulletList.Add(new BulletMaker("Normal", "A normal bullet", "striaght", 1, 20, 10, 10, 2, 5));
            bulletList.Add(new BulletMaker("Bouncy", "Highly Unaccurate", "striaght", 1, 20, 20, 1, 0, 30));
            bulletList.Add(new BulletMaker("Magical", "A normal bullet", "striaght", 1, 10, 10, 1, 10, 50));
            bulletList.Add(new BulletMaker("Pierce", "A normal bullet", "striaght", 5, 10, 10, 1, 10, 80));

        }

        // All Boons
        public void LoadBoons()
        {
            boonList.Add(new BoonMaker("boon", "Flight", "Icarus flew too high in the sun and fell to his death. Will this happen to you?", 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0));
            boonList.Add(new BoonMaker("boon", "Hatred", "The hatred of God flows through you.", 15, 10, 10, 10, 10, 0, 0, -5, -5, -5, 0, 0, 0, 0));
            boonList.Add(new BoonMaker("boon", "Benevloance", "Look Above Everyone Else", 0, 0, 0, 0, 0, 0, 0, 10, 10, 10, 10, 10, 0, 0));
            boonList.Add(new BoonMaker("boon", "Blood Tale", "Your tale is made of blood.", 0, -20, 0, -20, 0, 0, 0, 30, 30, 0, 0, 0, 0, 0));
            boonList.Add(new BoonMaker("boon", "Chaos Theory", "The world is chaos.", 0, 100, 0, 100, -100, -100, -20, 0, 0, 0, 0, 0, 0, 0));
            boonList.Add(new BoonMaker("boon", "Shrink", "Become small..", 0, 100, 0, -50, 0, 0, -15, 0, 0, 0, 0, 0, 3, 20));
            boonList.Add(new BoonMaker("boon", "Law", "Become law.", 0, 0, 0, 0, 0, 0, 0, 0, 0, 50, 50, 0, 0, 0));
            boonList.Add(new BoonMaker("boon", "Severed Head", "Lose your head. Become a god. May kill you.", 0, 100, 0, -80, 0, 0, 0, 50, 50, 50, 10, 10, 2, 0));
            boonList.Add(new BoonMaker("boon", "HEALTH", "Gaun Health. Lose nothing else... or really???", 0, 100, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, -20));
            boonList.Add(new BoonMaker("boon", "Demi-Fiend", "Become a Demon.", 0, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 0, 0));

            boonList.Add(new BoonMaker("curse", "Fear of Dark", "The crux against humanity.", 25, -5, -5, -5, -5, -5, -5, -5, -5, -5, -5, -5, 0, 0));
        }






        // Checking Collisions for line / rectangle
        public bool LineRectangleCollision(double x1, double x2, double y1, double y2, double x, double y, double width, double height)
        {


            double rx = x;
            double ry = y;

            double rh = height;
            double rw = width;



            // check if the line has hit any of the rectangle's sides
            // uses the Line/Line function below
            bool left = CheckLine(x1, y1, x2, y2, rx, ry, rx, ry + rh);
            bool right = CheckLine(x1, y1, x2, y2, rx + rw, ry, rx + rw, ry + rh);
            bool top = CheckLine(x1, y1, x2, y2, rx, ry, rx + rw, ry);
            bool bottom = CheckLine(x1, y1, x2, y2, rx, ry + rh, rx + rw, ry + rh);

            // if ANY of the above are true, the line
            // has hit the rectangle
            if (left || right || top || bottom)
            {
                return true;
            }
            return false;

        }

        public bool CheckLine(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {

            double uA = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));

            double uB = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));

            if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
            {
                return true;
            }


            return false;
        }
    }


}