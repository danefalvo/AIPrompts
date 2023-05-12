using System;
using System.Collections.Generic;
using System.Linq;
public class Player
{
    public string Name { get; set; }
    public int Cash { get; set; }
    public Dictionary<string, Property> Properties { get; set; }
    public int TotalValue { get; set; }
}

public class Property
{
    public string Name { get; set; }
    public List<string> Squares { get; set; }
}

public class GameBoard
{
    public int Width { get; set; }
    public int Height { get; set; }
    public Dictionary<string, string> Grid { get; set; }
}

public class FunkyRealEstateRoyaleGame
{
    private List<Player> players;
    private GameBoard gameBoard;

    // The rest of the methods
    public void InitializePlayers()
    {
        players = new List<Player>
        {
            new Player { Name = "Alice", Cash = 20, Properties = new Dictionary<string, Property>() },
            new Player { Name = "Bob", Cash = 20, Properties = new Dictionary<string, Property>() },
            new Player { Name = "Cathy", Cash = 20, Properties = new Dictionary<string, Property>() },
            new Player { Name = "Darren", Cash = 20, Properties = new Dictionary<string, Property>() },
            new Player { Name = "Ernest", Cash = 20, Properties = new Dictionary<string, Property>() },
            new Player { Name = "Frank", Cash = 20, Properties = new Dictionary<string, Property>() },
            new Player { Name = "Garry", Cash = 20, Properties = new Dictionary<string, Property>() },
            new Player { Name = "Harry", Cash = 20, Properties = new Dictionary<string, Property>() },
            new Player { Name = "Indie", Cash = 20, Properties = new Dictionary<string, Property>() },
            // Add more players with unique names as needed
        };
    }

    public void InitializeGame(int playerCount)
    {
        if (playerCount < 8 || playerCount > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(playerCount), "Player count must be between 8 and 12.");
        }

        List<Player> players = new List<Player>();
        for (int i = 0; i < playerCount; i++)
        {
            Console.Write("Enter the name for player {i + 1}: ");
            string playerName = Console.ReadLine();
            bool isUnique = true;
            foreach (Player player in players)
            {
                if (player.Name[0] == playerName[0])
                {
                    isUnique = false;
                    break;
                }
            }

            if (isUnique)
            {
                players.Add(new Player { Name = playerName, Cash = 20, Properties = new Dictionary<string, Property>() });
            }
            else
            {
                Console.WriteLine("Player names must have unique starting letters. Please enter a different name.");
                i--;
            }
        }
    }

public void RandomizeGameBoard()
{
    Random random = new Random();
    int width = random.Next(15, 26);
    int height = random.Next(10, 16);

    gameBoard = new GameBoard
    {
        Width = width,
        Height = height,
        Grid = new Dictionary<string, string>()
    };

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            string coordinates = GetSquareCoordinates(x, y);
            gameBoard.Grid[coordinates] = ".";
        }
    }
}

    public void StartGame()
    {
        const int totalRounds = 30;
        PrintGameBoard();
        for (int round = 1; round <= totalRounds; round++)
        {
            Console.WriteLine($"Round {round} of {totalRounds}");
            BuyingRound();
            ClearScreen();
            UpdatePlayerPropertyValues();
            PrintPlayerInformation();
            PrintBarGraph();
            PrintGameBoard();
            IncreasePlayerCash(); // Add this line to increase player cash at the end of each round
            System.Threading.Thread.Sleep(2000);
        }
    }

    public void PrintGameBoard()
    {
        for (int y = 0; y < gameBoard.Height; y++)
        {
            for (int x = 0; x < gameBoard.Width; x++)
            {
                string coordinates = GetSquareCoordinates(x, y);
                Console.Write(gameBoard.Grid[coordinates] + " ");
            }
            Console.WriteLine();
        }
    }
private void BuyingRound()
{
    const int buyingSteps = 5;

    for (int step = 0; step < buyingSteps; step++)
    {
        foreach (Player player in players)
        {
            Console.WriteLine($"{player.Name}, it's your turn to buy a property.");
            string input = GetComputerPlayerInput(player);

            if (input == "SKIP")
            {
                Console.WriteLine($"{player.Name} decided to skip.");
                continue;
            }

            if (gameBoard.Grid.ContainsKey(input) && gameBoard.Grid[input] == ".")
            {
                var (x, y) = ParseSquareCoordinates(input); // Parse coordinates here

                int cost = CalculateSquareCost(x, y);

                if (player.Cash >= cost)
                {
                    player.Cash -= cost;
                    gameBoard.Grid[input] = player.Name[0].ToString();
                    Console.WriteLine($"{player.Name} bought the empty square at {input}");
                }
                else
                {
                    Console.WriteLine($"{player.Name} doesn't have enough cash to buy the property at {input}.");
                }
            }
            else
            {
                Console.WriteLine($"{player.Name} entered invalid coordinates or the property is already owned.");
            }
        }
    }
}


private string GetComputerPlayerInput(Player player)
{
    List<string> validCoordinates = new List<string>();
    List<string> validAdjacentCoordinates = new List<string>();

    for (int y = 0; y < gameBoard.Height; y++)
    {
        for (int x = 0; x < gameBoard.Width; x++)
        {
            string coordinates = GetSquareCoordinates(x, y);
            if (gameBoard.Grid[coordinates] == ".")
            {
                validCoordinates.Add(coordinates);
                if (CountAdjacentPlayerProperties(player, coordinates) > 0)
                {
                    validAdjacentCoordinates.Add(coordinates);
                }
            }
        }
    }

    string bestCoordinate = "";
    int lowestCost = int.MaxValue;

    // Prioritize buying squares that expand their properties
    foreach (string coordinate in validAdjacentCoordinates)
    {
        var (x, y) = ParseSquareCoordinates(coordinate); // Parse coordinates here
        int cost = CalculateSquareCost(x, y);
        if (player.Cash >= cost && cost < lowestCost)
        {
            lowestCost = cost;
            bestCoordinate = coordinate;
        }
    }

    // If no adjacent square is affordable, look for any affordable square on the board
    if (bestCoordinate == "")
    {
        foreach (string coordinate in validCoordinates)
        {
            var (x, y) = ParseSquareCoordinates(coordinate); // Parse coordinates here
            int cost = CalculateSquareCost(x, y);
            if (player.Cash >= cost && cost < lowestCost)
            {
                lowestCost = cost;
                bestCoordinate = coordinate;
            }
        }
    }

    if (bestCoordinate == "")
    {
        return "SKIP";
    }

    return bestCoordinate;
}

private int CountAdjacentPlayerProperties(Player player, string square)
{
    int x = int.Parse(square.Substring(0, square.Length - 1)) - 1;
    int y = square[square.Length - 1] - 'A';
    int[] dx = { 0, 1, 0, -1 };
    int[] dy = { 1, 0, -1, 0 };
    int count = 0;

    for (int i = 0; i < 4; i++)
    {
        int newX = x + dx[i];
        int newY = y + dy[i];

        if (newX >= 0 && newX < gameBoard.Width && newY >= 0 && newY < gameBoard.Height)
        {
            string coordinates = GetSquareCoordinates(newX, newY);
            if (gameBoard.Grid[coordinates] == player.Name[0].ToString())
            {
                count++;
            }
        }
    }

    return count;
}

    // ...


    private void ClearScreen()
    {
        Console.Clear();
    }
private (int, int) ParseSquareCoordinates(string coordinates)
{
    int x = int.Parse(coordinates.Substring(0, coordinates.Length - 1)) - 1;
    int y = coordinates[coordinates.Length - 1] - 'A';
    return (x, y);
}
private void UpdatePlayerPropertyValues()
{
    GroupProperties();

    foreach (Player player in players)
    {
        int totalValue = 0;

        foreach (Property property in player.Properties.Values)
        {
            int size = property.Squares.Count;
            int propertyValue = 2 + (5 * (size - 1));
            totalValue += propertyValue;
        }

        player.TotalValue = totalValue;
    }
}


    private
        void PrintPlayerInformation()
    {
        Console.WriteLine("Player Information:");
        Console.WriteLine("--------------------------------------------------------");
        Console.WriteLine("| Name         | Cash  | Total Property Value (TV)    |");
        Console.WriteLine("--------------------------------------------------------");

        foreach (Player player in players.OrderByDescending(p => p.TotalValue))
        {
            Console.WriteLine($"| {player.Name,-12} | ${player.Cash,-5} | ${player.TotalValue,-28} |");
        }

        Console.WriteLine("--------------------------------------------------------");
    }

    private
        void PrintBarGraph()
    {
        Console.WriteLine("Total Value (TV) Bar Graph:");
        Console.WriteLine("--------------------------------------------------------");

        foreach (Player player in players.OrderByDescending(p => p.TotalValue))
        {
            int barLength = player.TotalValue / 5;
            string bar = new string('#', barLength);
            Console.WriteLine($"{player.Name}: {bar}");
        }

        Console.WriteLine("--------------------------------------------------------");
    }

    private void GroupProperties()
{
    // Reset properties for all players
    foreach (Player player in players)
    {
        player.Properties.Clear();
    }

    // A set to keep track of visited squares
    HashSet<string> visitedSquares = new HashSet<string>();

    for (int y = 0; y < gameBoard.Height; y++)
    {
        for (int x = 0; x < gameBoard.Width; x++)
        {
            string coordinates = GetSquareCoordinates(x, y);

            if (visitedSquares.Contains(coordinates) || gameBoard.Grid[coordinates] == ".")
            {
                continue;
            }

            // Find the player who owns the current square
            Player owner = players.Find(p => p.Name[0] == gameBoard.Grid[coordinates][0]);

            List<string> propertySquares = new List<string>();
            GroupAdjacentSquares(x, y, owner, propertySquares, visitedSquares);

            Property property = new Property { Name = $"{owner.Name}'s Property", Squares = propertySquares };
            owner.Properties.Add(Guid.NewGuid().ToString(), property);
        }
    }
}

private void GroupAdjacentSquares(int x, int y, Player owner, List<string> propertySquares, HashSet<string> visitedSquares)
{
    string coordinates = GetSquareCoordinates(x, y);

    if (visitedSquares.Contains(coordinates) || gameBoard.Grid[coordinates] != owner.Name[0].ToString())
    {
        return;
    }

    visitedSquares.Add(coordinates);
    propertySquares.Add(coordinates);

    int[] dx = { 0, 1, 0, -1 };
    int[] dy = { 1, 0, -1, 0 };

    for (int i = 0; i < 4; i++)
    {
        int newX = x + dx[i];
        int newY = y + dy[i];

        if (newX >= 0 && newX < gameBoard.Width && newY >= 0 && newY < gameBoard.Height)
        {
            GroupAdjacentSquares(newX, newY, owner, propertySquares, visitedSquares);
        }
    }
}

public void AllocateRandomSquaresToPlayers()
{
    Random random = new Random();
    List<string> availableSquares = gameBoard.Grid.Where(kv => kv.Value == ".").Select(kv => kv.Key).ToList();

    foreach (Player player in players)
    {
        int randomIndex = random.Next(availableSquares.Count);
        string randomSquare = availableSquares[randomIndex];
        gameBoard.Grid[randomSquare] = player.Name[0].ToString();
        availableSquares.RemoveAt(randomIndex); // Remove the allocated square from the list of available squares
    }
}

    private int CalculateSquareCost(int x, int y)
    {
        int adjacentCost = 0;
        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { 1, 0, -1, 0 };

        for (int i = 0; i < 4; i++)
        {
            int newX = x + dx[i];
            int newY = y + dy[i];

            if (newX >= 0 && newX < gameBoard.Width && newY >= 0 && newY < gameBoard.Height)
            {
                string coordinates = GetSquareCoordinates(newX, newY);
                if (gameBoard.Grid[coordinates] != ".")
                {
                    adjacentCost += 5;
                }
            }
        }

        return 10 + adjacentCost;
    }

    private
        string GetSquareCoordinates(int x, int y)
    {
        char columnLetter = (char)('A' + y);
        return (x + 1).ToString() + columnLetter;
    }


private void IncreasePlayerCash()
{
    foreach (Player player in players)
    {
        player.Cash += 5;
    }
}

}



public class Program
{
    public static void Main(string[] args)
    {
        FunkyRealEstateRoyaleGame game = new FunkyRealEstateRoyaleGame();
        game.InitializePlayers();
        game.RandomizeGameBoard();
        game.AllocateRandomSquaresToPlayers();
        game.StartGame();
    }
}
