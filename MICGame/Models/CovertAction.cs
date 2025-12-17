namespace MICGame.Models;

public enum CovertActionType
{
    FundInsurgency,
    BribePoliticians,
    TriggerBorderSkirmish,
    EngineerCrisis,
    ExpandSpyNetwork,
    ReduceSpyNetwork,
    SabotageRelations,
    BribeCountry,
    FormDeepStateConnections,
    EconomicSabotage
}

public class CovertAction
{
    public CovertActionType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Cost { get; set; }
    public int BaseSuccessRate { get; set; } // 0-100
    public int DetectionRisk { get; set; } // 0-100
    public CountryType TargetCountry { get; set; }
    public CountryType? SecondaryTarget { get; set; } // For sabotage relations
    
    // Impact if successful
    public int StabilityImpact { get; set; } = 0;
    public int EconomyImpact { get; set; } = 0;
    public int InsurgencyImpact { get; set; } = 0;
    public int CorruptionImpact { get; set; } = 0;
    public int RelationsImpact { get; set; } = 0;
    public int PublicSupportImpact { get; set; } = 0;
    public int SpyNetworkChange { get; set; } = 0;
}

public class OperationResult
{
    public bool Success { get; set; }
    public bool Detected { get; set; }
    public string Message { get; set; } = string.Empty;
    public int CostIncurred { get; set; }
    public Dictionary<string, int> Impacts { get; set; } = new();
}
