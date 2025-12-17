namespace MICGame.Models;

/// <summary>
/// Game configuration and balance settings
/// </summary>
public static class GameConfig
{
    // Starting Resources
    public const int StartingBudget = 100000;
    public const int AnnualBudget = 100000;
    public const int StartingSpyNetwork = 10;
    public const int StartingReputation = 50;
    
    // Spy Network
    public const int MinSpyNetwork = 5;
    public const int MaxSpyNetwork = 50;
    public const int SpyMaintenanceCostPerAgent = 500;
    public const int SpyNetworkExpandCost = 15000;
    public const int SpyNetworkExpandAmount = 5;
    
    // Reviews
    public const int YearsBetweenReviews = 5;
    public const int ReviewPassingScore = 60;
    
    // Win/Loss Conditions
    public const int WinStabilityThreshold = 30;
    public const int WinInsurgencyThreshold = 70;
    
    // Reputation Changes
    public const int ReputationGainOnSuccess = 2;
    public const int ReputationLossOnFailure = 1;
    public const int ReputationLossOnDetection = 5;
    
    // AI Threat Thresholds
    public const int HighThreatLevel = 60;
    public const int MediumThreatLevel = 30;
    
    // AI Review Pass Threshold
    public const int AIReviewPassScore = 100;
    
    // Deep State
    public const int DeepStateConnectionsCost = 25000;
    public const int DeepStateInfluenceGain = 15;
    
    // Success Rate Modifiers
    public const int SpyBonusPerFiveAgents = 5;
    public const int CounterIntelligencePenaltyDivisor = 2;
    public const int MinSuccessRate = 10;
    public const int MaxSuccessRate = 95;
    
    // Detection
    public const int MinDetectionChance = 5;
    
    // Operation Base Costs
    public const int FundInsurgencyCost = 20000;
    public const int BribePoliticiansCost = 25000;
    public const int TriggerBorderSkirmishCost = 15000;
    public const int EngineerCrisisCost = 30000;
    public const int EconomicSabotageCost = 18000;
    public const int BribeCountryCost = 35000;
    public const int SabotageRelationsCost = 20000;
    
    // Operation Impacts
    public static class ImpactValues
    {
        // Fund Insurgency
        public const int InsurgencyStabilityImpact = -15;
        public const int InsurgencyLevelIncrease = 20;
        
        // Bribe Politicians
        public const int BriberyCorruptionIncrease = 15;
        public const int BriberyPublicSupportDecrease = -10;
        
        // Border Skirmish
        public const int BorderSkirmishStabilityImpact = -10;
        public const int BorderSkirmishRelationsImpact = -20;
        
        // Engineer Crisis
        public const int CrisisStabilityImpact = -20;
        public const int CrisisEconomyImpact = -15;
        public const int CrisisPublicSupportImpact = -15;
        
        // Economic Sabotage
        public const int EconomicSabotageEconomyImpact = -12;
        public const int EconomicSabotagePublicSupportImpact = -8;
        
        // Bribe Country
        public const int BribeCountryInfluenceIncrease = 20;
        public const int BribeCountryRelationsImprovement = 15;
        public const int BribeCountryHostileRelationsDecrease = -10;
        
        // Sabotage Relations
        public const int SabotageRelationsImpact = -25;
    }
    
    // AI Response
    public static class AIResponse
    {
        // Security increases based on threat
        public const int HighThreatCounterIntelBonus = 5;
        public const int HighThreatInternalSecurityBonus = 4;
        public const int HighThreatMilitaryBonus = 3;
        
        public const int MediumThreatCounterIntelBonus = 3;
        public const int MediumThreatEconomyBonus = 2;
        
        public const int LowThreatEconomyBonus = 4;
        public const int LowThreatPublicSupportBonus = 3;
        
        // Insurgency suppression
        public const int InsurgencySuppressionEfficiency = 2; // Divided into insurgency level
        public const int MaxInsurgencySuppression = 15;
        public const int SuppressionPublicSupportCost = -5;
        
        // Counter-operation
        public const int CounterOpDetectionThreshold = 2;
        public const int CounterOpChance = 40;
        public const int CounterOpBudgetDamage = 10000;
        public const int CounterOpSpyNetworkDamage = 2;
        public const int CounterOpReputationDamage = 3;
        
        // Natural recovery
        public const int StabilityRecoveryThreshold = 80;
        public const int StabilityRecoveryInsurgencyMax = 20;
        public const int StabilityRecoveryAmount = 3;
    }
    
    // Country Initial Stats
    public static class InitialStats
    {
        // PureLand
        public const int PureLandStability = 90;
        public const int PureLandEconomy = 85;
        public const int PureLandMilitary = 75;
        public const int PureLandPublicSupport = 80;
        public const int PureLandCounterIntel = 50;
        public const int PureLandBorderDefense = 50;
        public const int PureLandInternalSecurity = 50;
        
        // Other countries can be configured here as needed
    }
}
