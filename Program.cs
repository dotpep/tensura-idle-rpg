using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography;
using static System.Console;

namespace ASCII_CLI_IdleRPG
{
    internal class Program
    {
        // Basic game logic
        static bool isRunning = true;
        static bool isMenu = true;
        static bool isPlay = false;
        static bool isRules = false;

        //static short choice;
        static int choice;
        //static char choice;
        //static string choice;

        // Battle logic
        static bool Fight = false;
        static bool Standing = true;

        static Random random = new Random();

        // Advanture logic
        static bool shopBuy = false;
        static bool speakMayor = false;
        static bool bossEnemy = false;

        public class CharacterStats
        {
            public string Name { get; set; }
            public int HP { get; set; } = 50;
            public int HPMAX { get; private set; } = 50;
            public int ATK { get; set; } = 3;
            public int POTION { get; set; } = 1;
            public int ELIXIR { get; set; } = 0;
            public int GOLD { get; set; } = 7;
            public int CoordinateX { get; set; } = 0;
            public int CoordinateY { get; set; } = 0;
            public bool IsKey { get; set; } = false;
        }

        static CharacterStats characterStats = new CharacterStats();

        static public string Name = characterStats.Name;
        static public int HP = characterStats.HP;
        static public int HPMAX = characterStats.HPMAX;
        static public int ATK = characterStats.ATK;
        static public int POTION = characterStats.POTION;
        static public int ELIXIR = characterStats.ELIXIR;
        static public int GOLD = characterStats.GOLD;
        static public int CoordinateX = characterStats.CoordinateX;
        static public int CoordinateY = characterStats.CoordinateY;
        static public bool IsKey = characterStats.IsKey;


        // Maps 2d array or matix
        static string[,] Map = //columns (X)
        {   // x = 0    // x = 1    // x = 2   // x = 3   // x = 4   // x = 5     // x = 6         // rows (Y)
            { "plains",  "plains",  "plains",  "plains",  "forest",  "mountain",  "cave" },        // y = 0
            { "forest",  "forest",  "forest",  "forest",  "forest",  "hills",     "mountain" },    // y = 1
            { "forest",  "fields",  "bridge",  "plains",  "hills",   "forest",    "hills" },       // y = 2
            { "plains",  "shop",    "town",    "mayor",   "plains",  "hills",     "mountain" },    // y = 3
            { "plains",  "fields",  "fields",  "plains",  "hills",   "mountain",  "mountain" }     // y = 4
        };

        static int LengthY = Map.GetLength(0) - 1;
        static int LengthX = Map.GetLength(1) - 1;

        // Dict of Biom enemy spawn
        static Dictionary<string, Dictionary<string, object>> Biom = new Dictionary<string, Dictionary<string, object>>()
        {
            {"plains",   new Dictionary<string, object> { {"text", "PLAINS"},       {"enemy", true}  }},
            {"forest",   new Dictionary<string, object> { {"text", "WOODS"},        {"enemy", true}  }},
            {"fields",   new Dictionary<string, object> { {"text", "FIELDS"},       {"enemy", false} }},
            {"bridge",   new Dictionary<string, object> { {"text", "BRIDGE"},       {"enemy", true}  }},
            {"town",     new Dictionary<string, object> { {"text", "TOWN CENTRE"},  {"enemy", false} }},
            {"shop",     new Dictionary<string, object> { {"text", "SHOP"},         {"enemy", false} }},
            {"mayor",    new Dictionary<string, object> { {"text", "MAYOR"},        {"enemy", false} }},
            {"cave",     new Dictionary<string, object> { {"text", "CAVE"},         {"enemy", false} }},
            {"mountain", new Dictionary<string, object> { {"text", "MOUNTAIN"},     {"enemy", true}  }},
            {"hills",    new Dictionary<string, object> { {"text", "HILLS"},        {"enemy", true}  }},
        };


        static List<string> EnemyList = new List<string>() { "Goblin", "Orc", "Slime" };

        static Dictionary<string, Dictionary<string, int>> Mobs = new Dictionary<string, Dictionary<string, int>>()
        {
            {"Goblin",  new Dictionary<string, int>()   { { "hp", 15 },     { "atk", 3 },   { "gold", 8   }  } },
            {"Orc",     new Dictionary<string, int>()   { { "hp", 35 },     { "atk", 5 },   { "gold", 18  }  } },
            {"Slime",   new Dictionary<string, int>()   { { "hp", 30 },     { "atk", 2 },   { "gold", 12  }  } },
            {"Dragon",  new Dictionary<string, int>()   { { "hp", 100 },    { "atk", 8 },   { "gold", 100 }  } }
        };

        static void startPageInfo()
        {
            WriteLine(@"
▄█▄    █    ▄█     ▄█ ██▄   █     ▄███▄                   
█▀ ▀▄  █    ██     ██ █  █  █     █▀   ▀                  
█   ▀  █    ██     ██ █   █ █     ██▄▄                    
█▄  ▄▀ ███▄ ▐█     ▐█ █  █  ███▄  █▄   ▄▀                 
▀███▀      ▀ ▐      ▐ ███▀      ▀ ▀███▀                   
▄█    ▄▄▄▄▄   ▄███▄   █  █▀ ██   ▄█     █▄▄▄▄ █ ▄▄    ▄▀  
██   █     ▀▄ █▀   ▀  █▄█   █ █  ██     █  ▄▀ █   █ ▄▀    
██ ▄  ▀▀▀▀▄   ██▄▄    █▀▄   █▄▄█ ██     █▀▀▌  █▀▀▀  █ ▀▄  
▐█  ▀▄▄▄▄▀    █▄   ▄▀ █  █  █  █ ▐█     █  █  █     █   █ 
 ▐            ▀███▀     █      █  ▐       █    █     ███  
                       ▀      █          ▀      ▀         
                             ▀                            
            ");
            drawLine();
            WriteLine("1. NEW GAME");
            WriteLine("2. LOAD GAME");
            WriteLine("3. RULES");
            WriteLine("4. QUIT GAME");
            drawLine();
        }

        static void drawLine()
        {
            WriteLine("xX--------------------xX");
        }

        static void Save()
        {
            string[] list = new string[]
            {
                Name,
                HP.ToString(),
                ATK.ToString(),
                POTION.ToString(),
                ELIXIR.ToString(),
                GOLD.ToString(),
                CoordinateX.ToString(),
                CoordinateY.ToString(),
                IsKey.ToString()
            };

            // TODO: Save this file in current project directory not in bin/Debug/load.txt
            // NOTE: there may be use interface and save it another data formats like plain text, json, database like sqlite for executable game
            using (StreamWriter file = new StreamWriter("load.txt"))
            {
                foreach (string item in list)
                {
                    file.WriteLine(item);
                }
            }

        }

        static void Heal(int amount)
        {
            if (HP + amount < HPMAX) { HP += amount; }
            else { HP = HPMAX; }
            WriteLine($"{Name}'s HP refilled to {HP}!");
        }

        static void Battle()
        {
            string enemyName;

            if (!bossEnemy)
            {
                int randomEnemyIndex = random.Next(EnemyList.Count);
                enemyName = EnemyList[randomEnemyIndex];
            }
            else { enemyName = "Dragon"; }

            int enemyHP = Mobs[enemyName]["hp"];
            int enemyHPMax = enemyHP;
            int enemyATK = Mobs[enemyName]["atk"];
            int enemyGold = Mobs[enemyName]["gold"];

            while (Fight)
            {
                Clear();
                drawLine();
                WriteLine("Defeat the " + enemyName + "!");
                drawLine();
                WriteLine($"{enemyName}'s HP: {enemyHP}/{enemyHPMax}");
                WriteLine($"{Name}'s HP: {HP}/{HPMAX}");
                WriteLine($"POTIONS: {POTION}");
                WriteLine($"ELIXIR: {ELIXIR}");
                drawLine();

                WriteLine("1 - ATTACK");
                if (POTION > 0) { WriteLine("2 - USE POTION (30HP)"); }
                if (ELIXIR > 0) { WriteLine("3 - USE ELIXIR (50HP)"); }
                drawLine();

                Write($"{Name}# ");
                string choice = ReadLine();

                if (choice == "1")
                {
                    enemyHP -= ATK;
                    WriteLine($"{Name} dealt {ATK} damage to the {enemyName}.");
                    if (enemyHP > 0)
                    {
                        HP -= enemyATK;
                        WriteLine($"{enemyName} dealt {enemyATK} damage to the {Name}.");

                    }

                    Write($"{Name}> ");
                    ReadLine();

                }
                else if (choice == "2")
                {
                    if (POTION > 0)
                    {
                        POTION--;
                        Heal(amount: 30);
                        
                        // Skip one move if player in Fight with enemy
                        HP -= enemyATK;
                        WriteLine($"{enemyName} dealt {enemyATK} damage to the {Name}.");
                    }
                    else { WriteLine("No potions!"); }

                    Write($"{Name}> ");
                    ReadLine();
                }
                else if (choice == "3")
                {
                    if (ELIXIR > 0)
                    {
                        ELIXIR--;
                        Heal(amount: 50);

                        // Skip one move if player in Fight with enemy
                        HP -= enemyATK;
                        WriteLine($"{enemyName} dealt {enemyATK} damage to the {Name}.");
                    }
                    else { WriteLine("No elixirs!"); }

                    Write($"{Name}> ");
                    ReadLine();
                }

                if (HP <= 0)
                {
                    WriteLine($"{enemyName} defeated {Name}...");
                    drawLine();
                    Fight = false;
                    isPlay = false;
                    isRunning = false;
                    WriteLine("GAME OVER");

                    Write($"{Name}> ");
                    ReadLine();
                }

                if (enemyHP <= 0)
                {
                    WriteLine($"{Name} defeated the {enemyName}!");
                    drawLine();
                    Fight = false;

                    GOLD += enemyGold;
                    WriteLine($"You've found {enemyGold} gold!");

                    if (random.Next(0, 100) <= 30)
                    {
                        POTION++;
                        WriteLine($"You've found a potion!");
                    }
                    else if (random.Next(0, 100) <= 20)
                    {
                        ELIXIR++;
                        WriteLine($"You've found a elixir!");
                    }

                    if (enemyName == "Dragon")
                    {
                        WriteLine("Congratulations, you've finished the game!");
                        bossEnemy = false;
                        isPlay = false;
                        isRunning = false;
                    }


                    Write($"{Name}> ");
                    ReadLine();
                    Clear();
                }


            }
        }

        static void Shop()
        {
            while (shopBuy)
            {
                Clear();
                drawLine();
                WriteLine("Welcome to the shop!");
                drawLine();
                WriteLine($"GOLD: {GOLD}");
                WriteLine($"POTIONS: {POTION}");
                WriteLine($"ELIXIRS: {ELIXIR}");
                WriteLine($"ATK: {ATK}");
                drawLine();
                WriteLine("1 - BUY POTION (30 HP) - 5 GOLD");
                WriteLine("2 - BUY ELIXIR (MAXHP) - 8 GOLD");
                WriteLine("3 - UPGRADE WEAPON (+2ATK) - 10 GOLD");
                WriteLine("4 - LEAVE");
                drawLine();

                Write($"{Name}# ");
                string choice = ReadLine();

                if (choice == "1")
                {
                    if (GOLD >= 5)
                    {
                        POTION++;
                        GOLD -= 5;
                        WriteLine("You've bought a potion!");
                    }
                    else { WriteLine("Not enough gold!"); }

                    Write($"{Name}> ");
                    ReadLine();
                }

                else if (choice == "2")
                {
                    if (GOLD >= 8)
                    {
                        ELIXIR++;
                        GOLD -= 8;
                        WriteLine("You've bought a elixir!");
                    }
                    else { WriteLine("Not enough gold!"); }

                    Write($"{Name}> ");
                    ReadLine();
                }
                else if (choice == "3")
                {
                    if (GOLD >= 10)
                    {
                        ATK += 2;
                        GOLD -= 10;
                        WriteLine("You've upgraded your weapon!");
                    }
                    else { WriteLine("Not enough gold!"); }

                    Write($"{Name}> ");
                    ReadLine();
                }

                else if (choice == "4") { shopBuy = false; }


            }
        }

        static void Mayor()
        {
            while (speakMayor)
            {
                Clear();
                drawLine();
                WriteLine($"Hello there, {Name}!");
                if (ATK < 10)
                {
                    WriteLine("You're not strong enough to face the dragon yet! Keep practicing and come back later!");
                    IsKey = false;
                }
                else
                {
                    WriteLine("You might want to take on the dragon now! Take this key but be careful with the beast!");
                    IsKey = true;
                }

                drawLine();
                WriteLine("1 - LEAVE");
                drawLine();

                Write($"{Name}# ");
                string choice = ReadLine();

                if (choice == "1")
                {
                    speakMayor = false;
                }

            }
        }

        static void Cave()
        {
            while (bossEnemy)
            {
                Clear();
                drawLine();
                WriteLine("Here lies the cave of the dragon. What will you do?");
                drawLine();

                if (IsKey)
                {
                    WriteLine("1 - USE KEY");
                }
                WriteLine("2 - TURN BACK");
                drawLine();

                Write($"{Name}# ");
                string choice = ReadLine();

                if (choice == "1")
                {
                    if (IsKey)
                    {
                        Fight = true;
                        Battle();
                    }
                }
                else if (choice == "2")
                {
                    bossEnemy = false;
                }

            }
        }


        static void Main(string[] args)
        {
            MainGameManager();
        }

        static void MainGameManager()
        {
            Clear();
            startPageInfo();

            while (isRunning)
            {
                while (isMenu)
                {
                    if (isRules)
                    {
                        Clear();
                        WriteLine("There is some rules, there's nothing , there's nothing yet...");
                        isRules = false;

                        Write($"{Name}> ");
                        ReadLine();

                        continue;
                    }
                    else
                    {
                        Write($"{Name}# ");
                        string userInput = ReadLine();

                        if (int.TryParse(userInput, out choice)) { }
                        else
                        {
                            Clear();
                            startPageInfo();
                            continue;
                        }
                    }

                    switch (choice)
                    {
                        case 1:
                            Clear();
                            Write("# What's your name, hero? ");
                            Name = Convert.ToString(ReadLine());

                            isMenu = false;
                            isPlay = true;
                            break;
                        case 2:
                            try
                            {
                                // TODO: try to read and load this file in current project layer not in bin/Debug/load.txt
                                using (StreamReader file = new StreamReader("load.txt"))
                                {
                                    string[] load_list = file.ReadToEnd().Split('\n');
                                    // TODO: Implement reading Corrupt save file exception!
                                    //if (load_list.Length == 9)
                                    //{
                                        Name = load_list[0].TrimEnd();
                                        HP = int.Parse(load_list[1].TrimEnd());
                                        ATK = int.Parse(load_list[2].TrimEnd());
                                        POTION = int.Parse(load_list[3].TrimEnd());
                                        ELIXIR = int.Parse(load_list[4].TrimEnd());
                                        GOLD = int.Parse(load_list[5].TrimEnd());
                                        CoordinateX = int.Parse(load_list[6].TrimEnd());
                                        CoordinateY = int.Parse(load_list[7].TrimEnd());
                                        IsKey = bool.Parse(load_list[8].TrimEnd());

                                        Clear();
                                        WriteLine("Welcome back, " + Name + "!");

                                        Write($"{Name}> ");

                                        isMenu = false;
                                        isPlay = true;
                                    //}
                                    //else
                                    //{
                                    //    WriteLine("Corrupt save file!");
                                    //    Write("> "); ReadLine();
                                    //}
                                }
                            }
                            catch (IOException)
                            {
                                WriteLine("No loadable save file!");
                                Write($"{Name}> ");
                            }


                            break;
                        case 3:
                            isRules = true;
                            break;
                        case 4:
                            return;

                        default:
                            Clear();
                            WriteLine("That Choice Doesn't Exist. Try others!");
                            startPageInfo();
                            break;

                    }
                }
                while (isPlay)
                {
                    Save();  // autosave
                    Clear();

                    bool enemySpawn = (bool)Biom[Map[CoordinateY,CoordinateX]]["enemy"];
                    string mapLocation = (string)Biom[Map[CoordinateY,CoordinateX]]["text"];

                    if (!Standing)
                    {
                        if (enemySpawn)
                        {
                            if (random.Next(0, 100) <= 30)
                            {
                                Fight = true;
                                Battle();
                            }
                        }
                    }

                    if (isPlay)
                    {                    
                        // Player Information
                        drawLine();
                        WriteLine($"LOCATION: {mapLocation}");
                        drawLine();
                        WriteLine(
                            $"NAME:     {Name}\n" +
                            $"HP:       {HP}/{HPMAX}\n" +
                            $"ATK:      {ATK} \n" +
                            $"POTIONS:  {POTION} \n" +
                            $"ELIXIRS:  {ELIXIR} \n" +
                            $"GOLD:     {GOLD} "
                        );
                        drawLine();
                        WriteLine($"COORDINATES: {CoordinateX}, {CoordinateY}");
                        drawLine();

                        // Action button for player
                        WriteLine("0 - SAVE AND QUIT! ");
                        if ( CoordinateY > 0 ) { WriteLine("1 - NORTH"); }
                        if ( CoordinateX < LengthX ) { WriteLine("2 - EAST"); }
                        if ( CoordinateY < LengthY ){ WriteLine("3 - SOUTH"); }
                        if ( CoordinateX > 0 ) { WriteLine("4 - WEST"); }
                        if ( POTION > 0 ) { WriteLine("5 - USE POTION (30HP)"); }
                        if ( ELIXIR > 0 ) { WriteLine("6 - USE ELIXIR (50HP)"); }

                        if (Map[CoordinateY, CoordinateX] == "shop" ||
                            Map[CoordinateY, CoordinateX] == "mayor" ||
                            Map[CoordinateY, CoordinateX] == "cave" ) { WriteLine("7 - ENTER"); }

                        drawLine();


                        Write($"{Name}# ");
                        string Destination = Convert.ToString(ReadLine());
                        if (Destination == "0")
                        {
                            isPlay = false;
                            isMenu = true;
                            isRules = false;
                            Save();
                            Clear();
                            startPageInfo();
                        }
                        else if (Destination == "1")
                        {
                            if (CoordinateY > 0)
                            {
                                CoordinateY--;
                                Standing = false;
                            }
                        }
                        else if (Destination == "2")
                        {
                            if (CoordinateX < LengthX)
                            {
                                CoordinateX++;
                                Standing = false;
                            }
                        }
                        else if (Destination == "3")
                        {
                            if (CoordinateY < LengthY)
                            {
                                CoordinateY++;
                                Standing = false;
                            }
                        }
                        else if (Destination == "4")
                        {
                            if (CoordinateX > 0)
                            {
                                CoordinateX--;
                                Standing = false;
                            }
                        }
                        else if (Destination == "5")
                        {
                            if (POTION > 0)
                            {
                                POTION--;
                                Heal(amount: 30);
                            }
                            else { WriteLine("No potions!"); }

                            Write($"{Name}> ");
                            ReadLine();

                            Standing = true;
                        }
                        else if (Destination == "6")
                        {
                            if (ELIXIR > 0)
                            {
                                ELIXIR--;
                                Heal(amount: 50);
                            }
                            else { WriteLine("No elixirs!"); }

                            Write($"{Name}> ");
                            ReadLine();

                            Standing = true;
                        }
                        else if (Destination == "7")
                        {
                            if (Map[CoordinateY, CoordinateX] == "shop")
                            {
                                shopBuy = true;
                                Shop();
                            }
                            else if (Map[CoordinateY, CoordinateX] == "mayor")
                            {
                                speakMayor = true;
                                Mayor();
                            }
                            else if (Map[CoordinateY, CoordinateX] == "cave")
                            {
                                bossEnemy = true;
                                Cave();
                            }
                        }
                        else
                        {
                            Standing = true;
                        }

                    }
                }
            }
        }
    }
}
