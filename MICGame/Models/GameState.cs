namespace MICGame.Models;

public class GameState
{
    public int CurrentYear { get; set; } = 1;
    public int CurrentTurn { get; set; } = 1;
    public int YearsUntilReview { get; set; } = 5;
    
    // Player resources
    public int PlayerBudget { get; set; } = 100000;
    public int AnnualBudget { get; set; } = 100000;
    public int SpyNetworkSize { get; set; } = 10;
    public int SpyNetworkMaintenanceCost { get; set; } = 5000;
    public int PlayerReputation { get; set; } = 50; // 0-100, affects review
    public int SuccessfulOperations { get; set; } = 0;
    public int FailedOperations { get; set; } = 0;
    public int DetectedOperations { get; set; } = 0;
    
    // Countries
    public Dictionary<CountryType, Country> Countries { get; set; } = new();
    
    // Game status
    public bool GameOver { get; set; } = false;
    public string? GameOverReason { get; set; }
    public bool PlayerWon { get; set; } = false;
    
    // Turn history
    public List<TurnSummary> TurnHistory { get; set; } = new();
    
    // Deep state connections
    public int DeepStateInfluence { get; set; } = 0; // 0-100, harder to remove player
}

public class TurnSummary
{
    public int Turn { get; set; }
    public int Year { get; set; }
    public List<string> PlayerActions { get; set; } = new();
    public List<string> AIActions { get; set; } = new();
    public Dictionary<CountryType, int> StabilityChanges { get; set; } = new();
    public string Summary { get; set; } = string.Empty;
}
