namespace MICGame.Models;

public enum CountryType
{
    NotReal,      // Player's country
    PureLand,     // Target nation (AI-controlled)
    IWalk,        // Hostile theocracy aligned with PureLand
    OutDia,       // Strategic ally of NotReal
    ChutkiBan,    // Unstable theocracy (can be bribed)
    ChiHan        // Powerful socialist ally of PureLand
}

public enum CountryAlignment
{
    Neutral,
    AllyOfNotReal,
    AllyOfPureLand,
    Hostile
}

public class Country
{
    public CountryType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public CountryAlignment Alignment { get; set; }
    public int Stability { get; set; } = 100; // 0-100
    public int Economy { get; set; } = 100; // 0-100
    public int MilitaryStrength { get; set; } = 100; // 0-100
    public int PublicSupport { get; set; } = 100; // 0-100
    public int InsurgencyLevel { get; set; } = 0; // 0-100
    public int CorruptionLevel { get; set; } = 0; // 0-100
    public bool CanBeBribed { get; set; } = false;
    public int BribeLevel { get; set; } = 0; // 0-100, how much they've been bribed
    
    // Relations with other countries (-100 to 100)
    public Dictionary<CountryType, int> Relations { get; set; } = new();
    
    // AI-specific properties (for PureLand)
    public int CounterIntelligenceLevel { get; set; } = 50; // 0-100
    public int BorderDefense { get; set; } = 50; // 0-100
    public int InternalSecurity { get; set; } = 50; // 0-100
}
