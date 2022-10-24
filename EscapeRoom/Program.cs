internal class Program
{
    // ESCAPE ROOM CHARACTERS
    public const char player = '×';
    public const char door = '#';
    public const char key = '⌐';
    public const char border = '█';

    // MAP OPTIONS
    public static int mapWidth = 15;
    public static int mapHeight = 15;
    public const int maxMapWidth = 30;
    public const int maxMapHeight = 30;
    public const int minMapWidth = 10;
    public const int minMapHeight = 10;

    // MAP CUSTOMISATION
    public static int consoleInputWidth;
    public static int consoleInputHeight;
    public static bool widthCompatible = false;
    public static bool heightCompatible = false;
    public static bool successWidth = false;
    public static bool successHeight = false;
    // PROGRAM SETTINGS
    public static int option;
    public static bool gameOver = false;
    public static bool quit = false;
    public static bool keyFound = false;
    public static bool consoleSizeError = false;

    // ESCAPE ROOM VARIABLES
    public static Random rnd = new Random();
    public static int xPositionPlayer;
    public static int yPositionPlayer;
    public static int xPositionKey;
    public static int yPositionKey;
    public static int xPositionDoor;
    public static int yPositionDoor;
    public static int whichSideDoor;

    public static void Main(string[] args)
    {
        Console.Title = "Escape Room";
        Console.CursorVisible = false;
        EscapeRoom(); // RUN GAME
    }

    public static void EscapeRoom()
    {
        Console.Clear();
        PrintTitle();
        Console.WriteLine("Press any key to continue..\n");
        Console.ReadKey();
        Console.Clear();
        do
        {
            Console.Clear();
            PrintTitle();
            PrintOptions();
            int.TryParse(Console.ReadLine(), out option);

            if (option == 1)
            {
                whichSideDoor = rnd.Next(1, 2); //1=leftWall; 2=rightWall; 3=topWall; 4=bottomWall;
                switch (whichSideDoor)
                {
                    case 1:
                        xPositionDoor = 1;
                        yPositionDoor = rnd.Next(1, mapHeight - 1);
                        break;
                    case 2:
                        xPositionDoor = mapWidth - 2;
                        yPositionDoor = rnd.Next(1, mapHeight - 1);
                        break;
                    case 3:
                        xPositionDoor = rnd.Next(1, mapWidth - 1);
                        yPositionDoor = 1;
                        break;
                    case 4:
                        xPositionDoor = rnd.Next(1, mapWidth - 1);
                        yPositionDoor = mapHeight - 2;
                        break;
                    default:
                        xPositionDoor = 1;
                        yPositionDoor = rnd.Next(1, mapHeight - 1);
                        break;
                }
                yPositionPlayer = rnd.Next(1, mapWidth - 1);
                xPositionPlayer = rnd.Next(1, mapHeight - 1);
                xPositionKey = rnd.Next(1, mapWidth - 1);
                yPositionKey = rnd.Next(1, mapHeight - 1);


                ConsoleKeyInfo consoleKey;
                char[,] MapArray = InitializeMap(mapWidth, mapHeight);

                MapArray = MovePlayer(MapArray, xPositionPlayer, yPositionPlayer);
                Console.Clear();

                //if player/key/door spawn in eachother -> reposition player/key;
                do
                {
                    if (xPositionPlayer == xPositionKey && yPositionPlayer == yPositionKey) // PLAYER = KEY
                    {
                        xPositionPlayer = rnd.Next(1, mapWidth - 1);
                        yPositionPlayer = rnd.Next(1, mapHeight - 1);
                    }

                    if (xPositionPlayer == xPositionDoor && yPositionPlayer == yPositionDoor) // PLAYER = DOOR
                    {
                        xPositionPlayer = rnd.Next(1, mapWidth - 1);
                        yPositionPlayer = rnd.Next(1, mapHeight - 1);
                    }

                    if (xPositionKey == xPositionDoor && yPositionKey == yPositionDoor) // KEY == DOOR
                    {
                        xPositionKey = rnd.Next(1, mapWidth - 1);
                        yPositionKey = rnd.Next(1, mapHeight - 1);
                    }
                } while ((yPositionPlayer == xPositionKey && xPositionPlayer == yPositionKey) || (yPositionPlayer == xPositionDoor && xPositionPlayer == yPositionDoor) || (xPositionKey == xPositionDoor && yPositionKey == yPositionDoor));


                DrawMap(MapArray, xPositionKey, yPositionKey, keyFound, xPositionDoor, yPositionDoor);

                do
                {
                    if (Console.WindowHeight < mapHeight || Console.WindowWidth < mapWidth)
                    {
                        consoleSizeError = true;
                        gameOver = true;
                        break;
                    }

                    consoleKey = Console.ReadKey(true);

                    switch (consoleKey.Key)
                    {
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W:
                            xPositionPlayer--;
                            break;

                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S:
                            xPositionPlayer++;
                            break;

                        case ConsoleKey.LeftArrow:
                        case ConsoleKey.A:
                            yPositionPlayer--;
                            break;

                        case ConsoleKey.RightArrow:
                        case ConsoleKey.D:
                            yPositionPlayer++;
                            break;

                        case ConsoleKey.Escape:
                        case ConsoleKey.Enter:
                            gameOver = true;
                            quit = true;
                            break;
                    }

                    // CHECK IF PLAYER RUNS INTO BORDER
                    if (yPositionPlayer < 1) yPositionPlayer = 1;
                    if (xPositionPlayer < 1) xPositionPlayer = 1;
                    if (yPositionPlayer >= mapWidth - 1) yPositionPlayer = mapWidth - 2;
                    if (xPositionPlayer >= mapHeight - 1) xPositionPlayer = mapHeight - 2;

                    MapArray = MovePlayer(MapArray, xPositionPlayer, yPositionPlayer);

                    if (xPositionKey == yPositionPlayer && yPositionKey == xPositionPlayer)
                    {
                        keyFound = true;
                        Console.Beep(440, 250);
                    }

                    if (keyFound && xPositionDoor == yPositionPlayer && yPositionDoor == xPositionPlayer) gameOver = true;

                    DrawMap(MapArray, xPositionKey, yPositionKey, keyFound, xPositionDoor, yPositionDoor);
                } while (!gameOver);

                if (consoleSizeError)
                {
                    Console.WriteLine("Console window is too small.");
                    Console.WriteLine($"Minimum size is {maxMapWidth} width x {maxMapHeight} height.");
                    Console.WriteLine("Increase the size of the console window.");

                    Console.WriteLine("EscapeRoom was closed.");
                    option = 4;
                    quit = true;
                }
                consoleSizeError = false;
                gameOver = false;
                keyFound = false;

                if (!quit)
                {
                    Console.Clear();
                    PrintTitle();
                    Console.WriteLine("Congratulations. You successfully escaped this escape room!\n");
                    Console.WriteLine("To return to the main menu press any key..\n");
                    Console.ReadKey();
                }
                quit = false;
            }
            else if (option == 2)
            {
                Console.Clear();
                PrintTitle();

                // CHANGING MAP WIDHT
                do
                {
                    Console.WriteLine($"To change the width choose a number between {minMapWidth} and {maxMapWidth}. Currently set to {mapWidth}");
                    successWidth = int.TryParse(Console.ReadLine(), out consoleInputWidth);

                    if (!successWidth) { widthCompatible = false; }
                    if (consoleInputWidth < minMapWidth || consoleInputWidth > maxMapWidth) { widthCompatible = false; }
                    else
                    {
                        widthCompatible = true;
                        mapWidth = consoleInputWidth;
                    }
                } while (!widthCompatible);

                // CHANGING MAP HEIGHT
                do
                {
                    Console.WriteLine($"To change the height choose a number between {minMapHeight} and {maxMapHeight}. Currently set to {mapHeight}");
                    successHeight = int.TryParse(Console.ReadLine(), out consoleInputHeight);

                    if (!successHeight) { heightCompatible = false; }
                    if (consoleInputHeight < minMapHeight || consoleInputHeight > maxMapHeight) { heightCompatible = false; }
                    else
                    {
                        heightCompatible = true;
                        mapHeight = consoleInputHeight;
                    }
                } while (!heightCompatible);

            }
            else if (option == 3)
            {
                Console.Clear();
                PrintTitle();

                Console.WriteLine("Tutorial for the game:\n\n");
                Console.WriteLine($"To move your player ({player}) use W, A, S, D or the arrow keys on your keyboard.\n");
                Console.WriteLine($"To beat the EscapeRoom you need to escape and walk through a door: ({door})\n");
                Console.WriteLine($"To unlock the door you first need to collect a key: ({key})\n");
                Console.WriteLine("To quit the game press ESCAPE or the ENTER key.\n");
                Console.WriteLine("Good luck trying to escape without a key!\n\n\n");
                Console.WriteLine("Press any key to return to the menu..\n");
                Console.ReadKey();
            }
            else if (option == 4)
            {
                Console.Clear();
                Environment.Exit(0);
            }
            else { option = 0; }
        } while (option != 4);
    }
    public static void PrintTitle()
    {
        Console.WriteLine("\n\n");
        Console.WriteLine("███████╗░██████╗░█████╗░░█████╗░██████╗░███████╗  ██████╗░░█████╗░░█████╗░███╗░░░███╗");
        Console.WriteLine("██╔════╝██╔════╝██╔══██╗██╔══██╗██╔══██╗██╔════╝  ██╔══██╗██╔══██╗██╔══██╗████╗░████║");
        Console.WriteLine("█████╗░░╚█████╗░██║░░╚═╝███████║██████╔╝█████╗░░  ██████╔╝██║░░██║██║░░██║██╔████╔██║");
        Console.WriteLine("██╔══╝░░░╚═══██╗██║░░██╗██╔══██║██╔═══╝░██╔══╝░░  ██╔══██╗██║░░██║██║░░██║██║╚██╔╝██║");
        Console.WriteLine("███████╗██████╔╝╚█████╔╝██║░░██║██║░░░░░███████╗  ██║░░██║╚█████╔╝╚█████╔╝██║░╚═╝░██║");
        Console.WriteLine("╚══════╝╚═════╝░░╚════╝░╚═╝░░╚═╝╚═╝░░░░░╚══════╝  ╚═╝░░╚═╝░╚════╝░░╚════╝░╚═╝░░░░░╚═╝");
        Console.WriteLine("\n\n");
    }
    public static void PrintOptions()
    {
        Console.WriteLine("Please choose one of the following options...\n");
        Console.WriteLine("1. Play and try to escape");
        Console.WriteLine("2. Adjust EscapeRoom size");
        Console.WriteLine("3. Tutorial");
        Console.WriteLine("4. Exit\n");
    }


    private static int prevX = -1, prevY = -1;

    private static char[,] MovePlayer(char[,] MapArray, int xPosition, int yPosition)
    {
        if (prevY > -1 && prevX > -1)
        {
            MapArray[prevY, prevX] = ' ';
        }
        MapArray[yPosition, xPosition] = player;

        prevY = yPosition;
        prevX = xPosition;

        return MapArray;
    }

    private static void DrawMap(char[,] MapArray, int keyX, int keyY, bool keyFound, int doorX, int doorY)
    {
        for (var x = 0; x < MapArray.GetLength(0); x++)
        {
            for (var y = 0; y < MapArray.GetLength(1); y++)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(MapArray[x, y]);
            }
        }

        if (!keyFound)
        {
            Console.SetCursorPosition(keyX, keyY);
            Console.Write(key);
        }

        Console.SetCursorPosition(doorX, doorY);
        Console.Write(door);
    }

    private static char[,] InitializeMap(int mapWidth, int mapHeight)
    {
        char[,] MapArray = new char[mapWidth, mapHeight];

        for (int i = 0; i < mapWidth; i++)
        {
            MapArray[i, 0] = border;
        }

        for (int i = 0; i < mapWidth; i++)
        {
            MapArray[i, mapHeight - 1] = border;
        }

        for (int i = 0; i < mapHeight; i++)
        {
            MapArray[0, i] = border;
        }

        for (int i = 0; i < mapHeight; i++)
        {
            MapArray[mapWidth - 1, i] = border;
        }

        return MapArray;
    }
}

