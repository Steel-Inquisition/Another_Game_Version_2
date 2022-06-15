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

        // Interact with Items
        public void itemInteract(Rect PlayerHitBox, WeaponMaker CurrentWeapon, List<BoonMaker> CurrentBoon)
        {
            bool exit = false;

            foreach (Rectangle x in ItemSpace.Children.OfType<Rectangle>())
            {

                for (int i = 0; i < itemStats.Count; i++)
                {
                    if ((string)x.Tag == $"item-{i}")
                    {


                        // increases player hit box so that the player doesn't actually have to touch the item and might go in it
                        Rect item = new Rect(Canvas.GetLeft(x) - 10, Canvas.GetTop(x) - 10, x.Width + 20, x.Height + 20);


                        // If the newly expanded hitbox interacts with items
                        if (PlayerHitBox.IntersectsWith(item))
                        {


                            // If this is a chest
                            if (itemStats[i].isChest == "none") // No
                            {

                                // Delete Item
                                itemStats[i].takeDamage(100);

                                // Save this change
                                SaveMap(x, "0");

                                // Give item to player
                                GiveItems(i, "item");
                            }
                            else if (itemStats[i].isChest == "door" && Keyboard.IsKeyDown(Key.Enter)) // Is Door
                            {

                                // if there is enough keys
                                if (key > 0)
                                {

                                    // Subtract keys
                                    key -= 1;

                                    // Delete Item
                                    itemStats[i].takeDamage(100);

                                    // Save this change
                                    SaveMap(x, "0");

                                    // Give items
                                    GiveItems(i, "item");

                                }

                            }
                            else if (itemStats[i].isChest == "crate" && Keyboard.IsKeyDown(Key.Enter)) // is a weapon crate
                            {
                                WeaponChestOpen(i, x, CurrentWeapon);
                            }
                            else if (itemStats[i].isChest == "crate2" && Keyboard.IsKeyDown(Key.Enter)) // is a boon crate 
                            {
                                BoonChestOpen(i, x, CurrentBoon);
                            }
                            else if (itemStats[i].isChest == "crate3" && Keyboard.IsKeyDown(Key.Enter)) // is a health crate
                            {
                                HealthChestOpen(i, x);
                            }
                            else if (itemStats[i].isChest == "exit" && Keyboard.IsKeyDown(Key.Enter)) // is an exit
                            {
                                exit = true;
                            }


                            // Check if the item was used
                            itemStats[i].checkIfDead(itemstoremove, x);

                        }

                    }
                }
            }

            // If the exit is used, exit the map (not room, but to an entirely new map)
            if (exit)
            {
                ExitTheMap();
            }

        }

        // If the health chest is opem
        public void HealthChestOpen(int i, Rectangle x)
        {
            if (MessageBox.Show($"Do you really want to open the health box? Only the current player you're selecting will get it and that player will get a random set of health.", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {

                // Increase Health
                playerList[currentPlayer].health += itemStats[i].dropItem.healthAmount;

                // Give these items
                GiveItems(i, "item");

                // Delete this item
                itemStats[i].takeDamage(100);

                // Save this
                SaveMap(x, "0");

                // Show that health increased
                LogBox.Text += $"{playerList[currentPlayer].name} got {itemStats[i].dropItem.healthAmount} health! \n";

                // Update the UI to show this
                UpdateUi();
                ScrollBar.ScrollToEnd();


            }
        }


        // if boon chest is opened
        public void BoonChestOpen(int i, Rectangle x, List<BoonMaker> CurrentBoon)
        {
            if (MessageBox.Show($"Do you really want to open the boon box? Only the current player you're selecting will get it and you will only have once chance to accept or get rid of it! A boon cannot be removed. You will also have to pay {-itemStats[i].dropItem.coinAmount + difficulty[1]} coins and you have {coin} coins!", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {

                // Get a new boon
                BoonMaker NewBoon = boonList[itemStats[i].dropItem.thisBoon];

                if (coin >= -(itemStats[i].dropItem.coinAmount - difficulty[1]))
                {
                    // This looses coins
                    GiveItems(i, "cost");

                    // Delete chest
                    itemStats[i].takeDamage(100);

                    // Show this change
                    SaveMap(x, "0");


                    if (MessageBox.Show($"Do want this boon? \n {NewBoon.boonName} \n {NewBoon.boonType} \n {NewBoon.boonDescription}", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {

                        // Add to current boon
                        CurrentBoon.Add(NewBoon);

                        // Apply Boon Stats
                        ApplyBoon(NewBoon);

                        // Change Text
                        LogBox.Text += $"{playerList[currentPlayer].name} got a {NewBoon.boonName} \n";

                        // Change UI
                        UpdateUi();
                        ScrollBar.ScrollToEnd();

                    }
                }
                else
                {
                    MessageBox.Show($"You don't have enough coins! You only have {coin} and the chest requires {itemStats[i].dropItem.coinAmount}!");
                }

            }
        }

        // if the weapon chest is opened
        public void WeaponChestOpen(int i, Rectangle x, WeaponMaker CurrentWeapon)
        {
            if (MessageBox.Show($"Do you really want to open the weapon box? Only the current player you're selecting will get it and you will only have once chance to accept or get rid of it! You will also have to pay {-(itemStats[i].dropItem.coinAmount - difficulty[1])} coins and you have {coin} coins!", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (coin >= -(itemStats[i].dropItem.coinAmount - difficulty[1])) // if player has enough coins
                {

                    // This looses coins
                    GiveItems(i, "cost");

                    itemStats[i].takeDamage(100);
                    SaveMap(x, "0");

                    // get weapon based on dropped item
                    WeaponMaker NewWeapon = new(weaponList[itemStats[i].dropItem.weapon].name, weaponList[itemStats[i].dropItem.weapon].imageName, weaponList[itemStats[i].dropItem.weapon].damage, weaponList[itemStats[i].dropItem.weapon].damageType, weaponList[itemStats[i].dropItem.weapon].height, weaponList[itemStats[i].dropItem.weapon].width, weaponList[itemStats[i].dropItem.weapon].knockBack, weaponList[itemStats[i].dropItem.weapon].type, weaponList[itemStats[i].dropItem.weapon].range, weaponList[itemStats[i].dropItem.weapon].mpUsage, weaponList[itemStats[i].dropItem.weapon].mpUsage, weaponList[itemStats[i].dropItem.weapon].magicType, weaponList[itemStats[i].dropItem.weapon].bulletType, weaponList[itemStats[i].dropItem.weapon].bulletUsage);

                    Random rand = new();

                    // Random Stats
                    NewWeapon.damage += rand.Next(0, 50 + difficulty[4]);
                    NewWeapon.width += rand.Next(0, 25);
                    NewWeapon.height += rand.Next(0, 25);


                    if (MessageBox.Show($"Do you Want this Weapon? \n Current Weapon:{CurrentWeapon.name} \n Damage:{CurrentWeapon.damage} \n Type:{CurrentWeapon.damageType} and {CurrentWeapon.type} \n Height: {CurrentWeapon.height} \n Width: {CurrentWeapon.width} \n Knockback: {CurrentWeapon.knockBack} \n" + " vs " + $"\n New Weapon:{NewWeapon.name} \n Damage:{NewWeapon.damage} \n Type:{NewWeapon.damageType} and {NewWeapon.type} \n Height: {NewWeapon.height} \n Width: {NewWeapon.width} \n Knockback: {NewWeapon.knockBack} \n", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {

                        // Player get new weapon
                        playerList[currentPlayer].weapon = NewWeapon;

                        // Change text
                        LogBox.Text += $"{playerList[currentPlayer].name} got a {playerList[currentPlayer].weapon.name}! \n";

                        // Show UI change
                        UpdateUi();
                        ScrollBar.ScrollToEnd();
                    };

                }
                else
                {
                    MessageBox.Show($"You don't have enough coins! You only have {coin} and the chest requires {itemStats[i].dropItem.coinAmount}!");
                }


            }
        }


        // Give Items
        public void GiveItems(int i, string typeOfGive)
        {
            coin += itemStats[i].dropItem.coinAmount;
            ammo += itemStats[i].dropItem.ammoAmount;
            holyCross += itemStats[i].dropItem.holy_crossAmount;
            key += itemStats[i].dropItem.keyAmount;
            bomb += itemStats[i].dropItem.bombAmount;

            if (typeOfGive == "cost")
            {
                coin -= difficulty[1];
            }

            UpdateItemCount();
        }

        // Show this change
        public void UpdateItemCount()
        {
            TotalPartyInv.Text = $" Coin: {coin} \n Ammo: {ammo} \n Holy Cross: {holyCross} \n Key: {key} \n Bomb:{bomb}";
        }


        // Apply boon to player
        public void ApplyBoon(BoonMaker NewBoon)
        {

            // fix this later
            var x = NewBoon;

            // Apply stat changes
            playerList[currentPlayer].health += x.healthCost;
            playerList[currentPlayer].mp += x.mpCost;
            playerList[currentPlayer].healthMax += x.healthMaxCost;
            playerList[currentPlayer].mpMax += x.mpMaxCost;

            playerList[currentPlayer].phys += x.physCost;
            playerList[currentPlayer].magic += x.magicCost;
            playerList[currentPlayer].gun += x.physCost;
            playerList[currentPlayer].physDef += x.physDefCost;
            playerList[currentPlayer].magDef += x.magDefCost;
            playerList[currentPlayer].speed += x.speedCost;
            playerList[currentPlayer].mpRegen += x.mpRegenCost;
            playerList[currentPlayer].size += x.sizeCost;
            playerList[currentPlayer].invisibiltyFrames += x.invisibiltyFrames;


        }
    }


}