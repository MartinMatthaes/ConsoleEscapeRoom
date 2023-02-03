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
        Console.Title = "Escape Room"; // Set the console title
        Console.CursorVisible = false; // Hide the cursor for better UI
        EscapeRoom(); // Run the main game logic
    }


    // Main logic
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
                // Determines the location of the door neext to one of the four walls of the map
                whichSideDoor = rnd.Next(1, 5); //1=leftWall; 2=rightWall; 3=topWall; 4=bottomWall;
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
                // Determines the location of the player and the key
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
                } while (
                    (yPositionPlayer == yPositionKey && xPositionPlayer == xPositionKey) || 
                    (yPositionPlayer == yPositionDoor && xPositionPlayer == xPositionDoor) || 
                    (xPositionKey == xPositionDoor && yPositionKey == yPositionDoor));

            // Draw the map to the console
            DrawMap(MapArray, xPositionKey, yPositionKey, keyFound, xPositionDoor, yPositionDoor);
                // Movement logic
                do
                {
                    // Check if console is big enough
                    if (Console.WindowHeight <= mapHeight || Console.WindowWidth <= mapWidth)
                    {
                        consoleSizeError = true;
                        gameOver = true;
                        break;
                    }

                    // Movement logic using switch-statement with arrow-keys and WASD
                    // To exit Escape or Enter
                    consoleKey = Console.ReadKey(true);
                    switch (consoleKey.Key)
                    {
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W:
                            yPositionPlayer--;
                            break;

                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S:
                            yPositionPlayer++;
                            break;

                        case ConsoleKey.LeftArrow:
                        case ConsoleKey.A:
                            xPositionPlayer--;
                            break;

                        case ConsoleKey.RightArrow:
                        case ConsoleKey.D:
                            xPositionPlayer++;
                            break;

                        case ConsoleKey.Escape:
                        case ConsoleKey.Enter:
                            gameOver = true;
                            quit = true;
                            break;
                    }

                    // Check if the player runs into the border
                    if (yPositionPlayer < 1) yPositionPlayer = 1;
                    if (xPositionPlayer < 1) xPositionPlayer = 1;
                    if (yPositionPlayer >= mapWidth - 1) yPositionPlayer = mapWidth - 2;
                    if (xPositionPlayer >= mapHeight - 1) xPositionPlayer = mapHeight - 2;

                    // Move the player
                    MapArray = MovePlayer(MapArray, xPositionPlayer, yPositionPlayer);

                    // If key is found set keyFound to true
                    if (xPositionKey == xPositionPlayer && yPositionKey == yPositionPlayer)
                    {
                        keyFound = true;
                        Console.Beep(440, 250);
                    }

                    // If key is found and player is on the door end the game
                    if (keyFound && xPositionDoor == xPositionPlayer && yPositionDoor == yPositionPlayer) gameOver = true;


                    // DrawMap with updated positions after 1 cycle
                    DrawMap(MapArray, xPositionKey, yPositionKey, keyFound, xPositionDoor, yPositionDoor);

                    // Continue loop until gameOver = true;
                } while (!gameOver);

                // Check for Console-Size-Error
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

                // If player did not quit
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

                // Changing the map width
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
                    // Do loop until the width is acceptable
                } while (!widthCompatible);

                // Changing the map height
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
                    // Do loop until the height is acceptable
                } while (!heightCompatible);

            }
            // Print the tutorial
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
            // Quit the game with Exitcode 0
            else if (option == 4)
            {
                Console.Clear();
                Environment.Exit(0);
            }
            else { option = 0; }
            // Do loop as long as option is not 4 (quit program)
        } while (option != 4);
    }
    
    // Prints the title
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
    
    // Prints the options
    public static void PrintOptions()
    {
        Console.WriteLine("Please choose one of the following options...\n");
        Console.WriteLine("1. Play and try to escape");
        Console.WriteLine("2. Adjust EscapeRoom size");
        Console.WriteLine("3. Tutorial");
        Console.WriteLine("4. Exit\n");
    }


    private static int prevX = -1, prevY = -1;

    // Moves the player by updating the position and removing the previous position
    private static char[,] MovePlayer(char[,] MapArray, int xPosition, int yPosition)
    {
        // Check if previous position of the player is valid
        if (prevX > -1 && prevY > -1)
        {
            // Remove the player from the previous position
            MapArray[prevY, prevX] = ' ';
        }
        // Update the player's new position
        MapArray[yPosition, xPosition] = player;

        // Store the new position as the previous position for the next move
        prevX = xPosition;
        prevY = yPosition;

        return MapArray;
    }

    // Draws the map on the console
    private static void DrawMap(char[,] MapArray, int keyX, int keyY, bool keyFound, int doorX, int doorY)
    {
        // Loop through the entire map and draw each element on the console
        for (var y = 0; y < MapArray.GetLength(0); y++)
        {
            for (var x = 0; x < MapArray.GetLength(1); x++)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(MapArray[y, x]);
            }
        }

        // Draw the key if it hasn't been found yet
        if (!keyFound)
        {
            Console.SetCursorPosition(keyX, keyY);
            Console.Write(key);
        }

        // Draw the door
        Console.SetCursorPosition(doorX, doorY);
        Console.Write(door);
    }


    private static char[,] InitializeMap(int mapWidth, int mapHeight)
    {
        // Create array with width and height
        char[,] MapArray = new char[mapWidth, mapHeight];

        // Create top and bottom of map
        for (int i = 0; i < mapWidth; i++)
        {
            MapArray[i, 0] = border;
            MapArray[i, mapHeight - 1] = border;
        }

        // Create left and right of map
        for (int i = 0; i < mapHeight; i++)
        {
            MapArray[0, i] = border;
            MapArray[mapWidth - 1, i] = border;
        }

        return MapArray;
    }
}

    