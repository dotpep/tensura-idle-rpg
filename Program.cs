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

        static short choice;

        // Battle logic
        static bool Fight = false;
        static bool Standing = true;

        static Random random = new Random();

        // character info
        //static string name = "hero";

        public class CharacterStats
        {
            public string Name { get; set; }
            public int HP { get; set; } = 50;
            public int HPMAX { get; private set; } = 500;
            public int ATK { get; set; } = 3;
            public int POTION { get; set; } = 10;
            public int ELIXIR { get; set; } = 3;
            public int GOLD { get; set; } = 0;
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
        static string[,] Map =
        {   // columns (X)   // x = 0    // x = 1    // x = 2   // x = 3   // x = 4   // x = 5     // x = 6        // rows (Y)
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

        static string CurrentTile = Map[CoordinateY,CoordinateX];
        static object NameOfTile = Biom[CurrentTile]["text"];
        static object EnemyTile = Biom[CurrentTile]["enemy"];

        // dict object string
        static string CurrentBiomText = (string)Biom[Map[CoordinateY, CoordinateX]]["text"];
        // dict object boolean
        static bool CurrentBiomSpawnEnemy = (bool)Biom[Map[CoordinateY, CoordinateX]]["enemy"];


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

        static void save()
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

            WriteLine("Fight happening!");

            int randomEnemyIndex = random.Next(EnemyList.Count);
            string enemyName = EnemyList[randomEnemyIndex];

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

                Write(Name); Write("# "); 
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

                    Write(Name); Write("> ");
                    ReadLine();

                }
                else if (choice == "2")
                {
                    if (POTION > 0)
                    {
                        POTION--;
                        Heal(amount: 30);
                        
                        // Skip one move if player in Fight with enemy
                        HP -= ATK;
                        WriteLine($"{enemyName} dealt {enemyATK} damage to the {Name}.");
                    }
                    else { WriteLine("No potions!"); }

                    Write(Name); Write("> ");
                    ReadLine();
                }
                else if (choice == "3")
                {
                    if (ELIXIR > 0)
                    {
                        ELIXIR--;
                        Heal(amount: 50);

                        // Skip one move if player in Fight with enemy
                        HP -= ATK;
                        WriteLine($"{enemyName} dealt {enemyATK} damage to the {Name}.");
                    }
                    else { WriteLine("No elixirs!"); }

                    Write(Name); Write("> ");
                    ReadLine();
                }
                else
                {

                }

                if (HP <= 0)
                {
                    WriteLine($"{enemyName} defeated {Name}...");
                    drawLine();
                    Fight = false;
                    isPlay = false;
                    isRunning = false;
                    WriteLine("GAME OVER");

                    Write(Name); Write("> ");
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


                    Write(Name); Write("> ");
                    ReadLine();
                    Clear();
                }


            }


            //Write(Name); Write("# ");
            //ReadLine();
        }


        static void Main(string[] args)
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
                        WriteLine("There is some rules, there's nothing , there's nothing...");
                        isRules = false;

                        //Write(Name); Write("> ");
                        //ReadLine();

                        //isMenu = false;
                        //isPlay = true;


                        // FIXME: Implement quit to menu, 1. (read rules) > (enter) see # (and 0. QUIT) when typed 0 immediately go to main menu

                        Write(Name); Write("> ");
                        ReadLine();

                        drawLine();
                        WriteLine("0. QUIT TO MENU! ");
                        drawLine();

                        Write(Name); Write("# ");
                        string Destination = Convert.ToString(ReadLine());
                        if (Destination == "0")
                        {
                            isPlay = false;
                            isMenu = true;
                            isRules = false;
                            Clear();
                            startPageInfo();
                        }

                        //Write(Name); Write("# ");
                        //string dest = Convert.ToString(ReadLine());

                        //Write(Name); Write("> ");
                        //ReadLine();

                        //Write(Name); Write("# ");
                        //string dest = Convert.ToString(ReadLine());

                        //if (dest == "0")
                        //{
                        //    isPlay = false;
                        //    isMenu = true;
                        //    isRules = false;
                        //    break;
                        //}
                    }
                    else
                    {
                        Write(Name); Write("# ");
                        string input = ReadLine();
                        bool valid = Int16.TryParse(input, out choice);
                        while (!valid)
                        {
                            Clear();
                            startPageInfo();
                            Write(Name); Write("# ");
                            input = ReadLine();
                            valid = Int16.TryParse(input, out choice);
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

                                        Write(Name); Write("> "); ReadLine();

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
                                Write("> "); ReadLine();
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
                    // FIXME: When isRules and isLoaded game returns this isPlay loop by displaying Stats of Player
                    save();  // autosave
                    Clear();

                    // Spawn enemyes Logic
                    //if (!Standing)
                    //{
                    //    var currentTile = (dynamic)Biom[Map[CoordinateY, CoordinateX]];
                    //    if (currentTile.enemy && random.Next(0, 100) < 30)
                    //    {
                    //        Fight = true;
                    //        Battle();
                    //    }
                    //}

                    bool enemySpawn = (bool)Biom[Map[CoordinateY, CoordinateX]]["enemy"];

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
                        WriteLine($"LOCATION: {Biom[Map[CoordinateY, CoordinateX]]["text"]}");
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
                        drawLine();


                        Write(Name); Write("# ");
                        string Destination = Convert.ToString(ReadLine());
                        if (Destination == "0")
                        {
                            isPlay = false;
                            isMenu = true;
                            isRules = false;
                            save();
                            Clear();
                            startPageInfo();
                        }
                        else if (Destination == "1")
                        {
                            if (CoordinateY > 0)
                            {
                                CoordinateY -= 1;
                                Standing = false;
                            }

                            //if (CoordinateY > 0)
                            //{
                            //    CoordinateY -= 1;
                            //}
                            //else
                            //{
                            //    CoordinateY = LengthY;
                            //}
                        }
                        else if (Destination == "2")
                        {
                            if (CoordinateX < LengthX)
                            {
                                CoordinateX += 1;
                                Standing = false;
                            }
                        }
                        else if (Destination == "3")
                        {
                            if (CoordinateY < LengthY)
                            {
                                CoordinateY += 1;
                                Standing = false;
                            }
                            //if (CoordinateY < LengthY)
                            //{
                            //    CoordinateY += 1;
                            //}
                            //else
                            //{
                            //    CoordinateY = 0;
                            //}
                        }
                        else if (Destination == "4")
                        {
                            if (CoordinateX > 0)
                            {
                                CoordinateX -= 1;
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

                            Write(Name); Write("> ");
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

                            Write(Name); Write("> ");
                            ReadLine();

                            Standing = true;
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
