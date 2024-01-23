using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using static System.Console;

namespace ASCII_CLI_IdleRPG
{
    internal class Program
    {
        static bool isRunning = true;
        static bool isMenu = true;
        static bool isPlay = false;
        static bool isRules = false;

        static short choice;

        // character info
        //static string name = "hero";

        public class CharacterStats
        {
            public string Name { get; set; }
            public int HP { get; set; } = 50;
            public int HPMAX { get; private set; } = 100;
            public int ATK { get; set; } = 3;
            public int POTION { get; set; } = 1;
            public int ELIXIR { get; set; } = 0;
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

                        Write(Name); Write("> ");
                        ReadLine();

                        // FIXME: Implement quit to menu, 1. (read rules) > (enter) see # (and 0. QUIT) when typed 0 immediately go to main menu
                        drawLine();
                        WriteLine("0. QUIT TO MENU! ");
                        drawLine();

                        Write(Name); Write("# ");
                        string dest = Convert.ToString(ReadLine());

                        //Write(Name); Write("> ");
                        //ReadLine();

                        //Write(Name); Write("# ");
                        //string dest = Convert.ToString(ReadLine());

                        if (dest == "0")
                        {
                            isPlay = false;
                            isMenu = true;
                            isRules = false;
                            break;
                        }
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

                                    //if (load_list.Length == 9)
                                    //{
                                        Name = load_list[0].TrimEnd();
                                        HP = int.Parse(load_list[1].TrimEnd());
                                        ATK = int.Parse(load_list[2].TrimEnd());
                                        POTION = int.Parse(load_list[3].TrimEnd());
                                        ELIXIR = int.Parse(load_list[4].TrimEnd());
                                        GOLD = int.Parse(load_list[5].TrimEnd());
                                        CoordinateX = int.Parse(load_list[6].TrimEnd());
                                        CoordinateY = int.Parse(load_list[6].TrimEnd());
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
                    drawLine();
                    WriteLine($"LOCATION: {NameOfTile}");
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
                    WriteLine("0. SAVE AND QUIT! ");
                    if ( CoordinateY > 0)
                    {
                        WriteLine("1. NORTH");
                    }
                    if ( CoordinateX < LengthX)
                    {
                        WriteLine("2. EAST");
                    }
                    if (CoordinateY < LengthY)
                    {
                        WriteLine("3. SOUTH");
                    }
                    if (CoordinateX > 0)
                    {
                        WriteLine("4. WEST");
                    }
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
                        }
                    }
                    else if (Destination == "3")
                    {
                        if (CoordinateY < LengthY)
                        {
                            CoordinateY += 1;
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
                        }
                    }
                }
            }
        }
    }
}
