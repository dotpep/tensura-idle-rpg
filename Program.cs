using System;
using System.IO;
using static System.Console;

namespace ASCII_CLI_IdleRPG
{
    internal class Program
    {
        static bool run = true;
        static bool menu = true;
        static bool play = false;
        static bool rules = false;

        static short choice;

        static string name = "hero";

        static void startPageInfo()
        {
            WriteLine("1. NEW GAME");
            WriteLine("2. LOAD GAME");
            WriteLine("3. RULES");
            WriteLine("4. QUIT GAME");
        }

        enum CharacterStats
        {
            HP = 50,
            ATK = 3,
        }

        static void save()
        {
            string[] list = new string[]
            {
                name,
                ((int)CharacterStats.HP).ToString(),
                ((int)CharacterStats.ATK).ToString(),
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

            while (run)
            {
                while (menu)
                {
                    if (rules)
                    {
                        Clear();
                        WriteLine("There is some rules, there's nothing , there's nothing...");
                        rules = false;

                        Write(name); Write("> ");
                        ReadLine();

                        menu = false;
                        play = true;
                    }
                    else
                    {
                        Write(name); Write("# ");
                        string input = ReadLine();
                        bool valid = Int16.TryParse(input, out choice);
                        while (!valid)
                        {
                            Clear();
                            startPageInfo();
                            Write(name); Write("# ");
                            input = ReadLine();
                            valid = Int16.TryParse(input, out choice);
                        }
                    }

                    switch (choice)
                    {
                        case 1:
                            Clear();
                            Write("# What's your name, hero? ");
                            name = Convert.ToString(ReadLine());

                            menu = false;
                            play = true;
                            break;
                        case 2:
                            using (StreamReader file = new StreamReader("load.txt"))
                            {
                                string[] load_list = file.ReadToEnd().Split('\n');
                                string name = load_list[0].TrimEnd();
                                int HP = int.Parse(load_list[1].TrimEnd());
                                int ATK = int.Parse(load_list[2].TrimEnd());

                                WriteLine(name + " " + HP + " " + ATK);
                            }

                            Clear();
                            WriteLine("Welcome back, " + name + "!");

                            Write(name); Write("> "); ReadLine();

                            menu = false;
                            play = true;
                            break;
                        case 3:
                            rules = true;
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
                while (play)
                {
                    save();  // autosave

                    Clear();
                    WriteLine("0. EXIT ");
                    Write(name); Write("# ");

                    string dest = Convert.ToString(ReadLine());

                    if (dest == "0")
                    {
                        play = false;
                        menu = true;
                        rules = false;
                        save();
                        Clear();
                        startPageInfo();
                    }
                }
            }
        }
    }
}
