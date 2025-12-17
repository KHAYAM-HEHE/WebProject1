using MICGame.Models;

namespace MICGame.Services;

public interface IGameService
{
    GameState InitializeGame();
    GameState GetCurrentGameState();
    void SaveGameState(GameState state);
    OperationResult ExecuteCovertAction(CovertAction action);
    void ProcessAITurn();
    void ProcessYearEnd();
    bool PerformReview();
    List<CovertAction> GetAvailableActions();
    int CalculateActionCost(CovertActionType actionType, int spyNetworkSize);
    int CalculateSuccessRate(CovertAction action, int spyNetworkSize, int targetCounterIntelligence);
    
    // New functionalities
    string GetIntelligenceReport();
    List<string> GetStrategicRecommendations();
    Dictionary<string, int> GetThreatAnalysis();
    bool CheckRandomEvent();
    string ProcessRandomEvent();
    int CalculateReviewScore(); // New: Let player see review score
}

public class GameService : IGameService
{
    private GameState _currentState;
    private readonly Random _random = new();
    private int _turnsSinceLastEvent = 0;
    private int _aggressiveAICounter = 0; // Track AI aggression level

    public GameService()
    {
        _currentState = InitializeGame();
    }

    public GameState InitializeGame()
    {
        var state = new GameState
        {
            CurrentYear = 1,
            CurrentTurn = 1,
            YearsUntilReview = 5,
            PlayerBudget = 100000,
            AnnualBudget = 100000,
            SpyNetworkSize = 10,
            PlayerReputation = 50,
            DeepStateInfluence = 0
        };

        // Initialize countries with more balanced stats
        state.Countries = new Dictionary<CountryType, Country>
        {
            [CountryType.NotReal] = new Country
            {
                Type = CountryType.NotReal,
                Name = "NotReal",
                Alignment = CountryAlignment.Neutral,
                Stability = 100,
                Economy = 100,
                MilitaryStrength = 80,
                PublicSupport = 70
            },
            [CountryType.PureLand] = new Country
            {
                Type = CountryType.PureLand,
                Name = "PureLand",
                Alignment = CountryAlignment.Hostile,
                Stability = 85, // Reduced from 90
                Economy = 80, // Reduced from 85
                MilitaryStrength = 70, // Reduced from 75
                PublicSupport = 75, // Reduced from 80
                CounterIntelligenceLevel = 40, // Reduced from 50
                BorderDefense = 40, // Reduced from 50
                InternalSecurity = 45 // Reduced from 50
            },
            [CountryType.IWalk] = new Country
            {
                Type = CountryType.IWalk,
                Name = "I-Walk",
                Alignment = CountryAlignment.AllyOfPureLand,
                Stability = 65, // Reduced from 70
                Economy = 55, // Reduced from 60
                MilitaryStrength = 45, // Reduced from 50
                PublicSupport = 60 // Reduced from 65
            },
            [CountryType.OutDia] = new Country
            {
                Type = CountryType.OutDia,
                Name = "OutDia",
                Alignment = CountryAlignment.AllyOfNotReal,
                Stability = 85,
                Economy = 80,
                MilitaryStrength = 70,
                PublicSupport = 75
            },
            [CountryType.ChutkiBan] = new Country
            {
                Type = CountryType.ChutkiBan,
                Name = "Chutki-Ban",
                Alignment = CountryAlignment.Neutral,
                Stability = 55, // Reduced from 60
                Economy = 50, // Reduced from 55
                MilitaryStrength = 40, // Reduced from 45
                PublicSupport = 45, // Reduced from 50
                CanBeBribed = true,
                CorruptionLevel = 45 // Increased from 40
            },
            [CountryType.ChiHan] = new Country
            {
                Type = CountryType.ChiHan,
                Name = "Chi-Han",
                Alignment = CountryAlignment.AllyOfPureLand,
                Stability = 90, // Reduced from 95
                Economy = 90, // Reduced from 95
                MilitaryStrength = 85, // Reduced from 90
                PublicSupport = 80 // Reduced from 85
            }
        };

        // Initialize relations
        InitializeRelations(state);

        _currentState = state;
        _aggressiveAICounter = 0;
        return state;
    }

    private void InitializeRelations(GameState state)
    {
        // PureLand relations
        state.Countries[CountryType.PureLand].Relations = new()
        {
            [CountryType.NotReal] = -80,
            [CountryType.IWalk] = 70,
            [CountryType.OutDia] = -50,
            [CountryType.ChutkiBan] = 20,
            [CountryType.ChiHan] = 80
        };

        // I-Walk relations
        state.Countries[CountryType.IWalk].Relations = new()
        {
            [CountryType.NotReal] = -70,
            [CountryType.PureLand] = 70,
            [CountryType.OutDia] = -40,
            [CountryType.ChutkiBan] = 30,
            [CountryType.ChiHan] = 50
        };

        // OutDia relations
        state.Countries[CountryType.OutDia].Relations = new()
        {
            [CountryType.NotReal] = 80,
            [CountryType.PureLand] = -50,
            [CountryType.IWalk] = -40,
            [CountryType.ChutkiBan] = 10,
            [CountryType.ChiHan] = -30
        };

        // ChutkiBan relations
        state.Countries[CountryType.ChutkiBan].Relations = new()
        {
            [CountryType.NotReal] = 0,
            [CountryType.PureLand] = 20,
            [CountryType.IWalk] = 30,
            [CountryType.OutDia] = 10,
            [CountryType.ChiHan] = 15
        };

        // ChiHan relations
        state.Countries[CountryType.ChiHan].Relations = new()
        {
            [CountryType.NotReal] = -60,
            [CountryType.PureLand] = 80,
            [CountryType.IWalk] = 50,
            [CountryType.OutDia] = -30,
            [CountryType.ChutkiBan] = 15
        };
    }

    public GameState GetCurrentGameState() => _currentState;

    public void SaveGameState(GameState state)
    {
        _currentState = state;
    }

    // NEW: Calculate review score so player can see it
    public int CalculateReviewScore()
    {
        int score = _currentState.PlayerReputation;
        
        // Success/failure ratio (max +25, min -25)
        if (_currentState.SuccessfulOperations + _currentState.FailedOperations > 0)
        {
            double successRate = (double)_currentState.SuccessfulOperations / 
                                (_currentState.SuccessfulOperations + _currentState.FailedOperations);
            score += (int)(successRate * 50) - 25;
        }

        // Detection penalty (-2 per detected operation)
        score -= _currentState.DetectedOperations * 2;

        // Deep state protection bonus
        score += _currentState.DeepStateInfluence / 3;

        // Impact on PureLand (main objective)
        var pureLand = _currentState.Countries[CountryType.PureLand];
        int pureLandImpact = (100 - pureLand.Stability) + pureLand.InsurgencyLevel + (pureLand.CorruptionLevel / 2);
        score += pureLandImpact / 3;

        // Efficiency bonus (achieving results with less detection)
        if (_currentState.SuccessfulOperations > 5 && _currentState.DetectedOperations < 3)
        {
            score += 10;
        }

        return Math.Clamp(score, 0, 100);
    }

    // NEW: Intelligence Report
    public string GetIntelligenceReport()
    {
        var pureLand = _currentState.Countries[CountryType.PureLand];
        var report = new System.Text.StringBuilder();

        report.AppendLine("?? CLASSIFIED INTELLIGENCE BRIEFING");
        report.AppendLine($"?? Year {_currentState.CurrentYear}, Turn {_currentState.CurrentTurn}");
        report.AppendLine();
        
        // Threat Level Assessment
        int threatLevel = CalculateThreatLevel(pureLand);
        string threatColor = threatLevel > 60 ? "HIGH" : threatLevel > 30 ? "MODERATE" : "LOW";
        report.AppendLine($"?? PureLand Threat Level: {threatColor} ({threatLevel}/100)");
        
        // Review score preview
        int reviewScore = CalculateReviewScore();
        string reviewStatus = reviewScore >= 60 ? "PASSING" : "FAILING";
        report.AppendLine($"?? Current Review Score: {reviewScore}/100 ({reviewStatus})");
        
        // Vulnerability Analysis
        if (pureLand.Stability < 50)
            report.AppendLine("? OPPORTUNITY: PureLand stability is weakening");
        if (pureLand.InsurgencyLevel > 40)
            report.AppendLine("? SUCCESS: Insurgency operations showing results");
        if (pureLand.CorruptionLevel > 50)
            report.AppendLine("?? ADVANTAGE: Government corruption is widespread");
        if (pureLand.CounterIntelligenceLevel > 70)
            report.AppendLine("?? WARNING: Enemy counter-intelligence is highly effective");
        
        // Budget Warning
        if (_currentState.PlayerBudget < 50000)
            report.AppendLine("?? CRITICAL: Budget running low - reduce operations");
        
        // Review Warning
        if (_currentState.YearsUntilReview <= 2)
            report.AppendLine($"? URGENT: Performance review in {_currentState.YearsUntilReview} year(s)");
        
        return report.ToString();
    }

    // NEW: Strategic Recommendations
    public List<string> GetStrategicRecommendations()
    {
        var recommendations = new List<string>();
        var pureLand = _currentState.Countries[CountryType.PureLand];
        var chutkiBan = _currentState.Countries[CountryType.ChutkiBan];

        // Review-based recommendations
        int reviewScore = CalculateReviewScore();
        if (reviewScore < 60 && _currentState.YearsUntilReview <= 3)
        {
            recommendations.Add("?? CRITICAL: Review score below passing - focus on successful operations!");
        }

        // Spy Network Recommendations
        if (_currentState.SpyNetworkSize < 15)
            recommendations.Add("Consider expanding spy network for better success rates");
        else if (_currentState.SpyNetworkSize > 25 && _currentState.PlayerBudget < 80000)
            recommendations.Add("Spy network may be too large - consider reducing maintenance costs");

        // Target-specific recommendations
        if (pureLand.Stability > 70 && pureLand.InsurgencyLevel < 20)
            recommendations.Add("PureLand is stable - increase insurgency operations");
        
        if (pureLand.InsurgencyLevel > 50 && pureLand.Stability > 40)
            recommendations.Add("High insurgency but stability holding - target economy or engineer crisis");
        
        if (pureLand.CorruptionLevel < 30)
            recommendations.Add("Low corruption - bribe politicians to weaken governance");
        
        if (pureLand.PublicSupport > 70)
            recommendations.Add("High public support for government - engineer crisis to reduce support");

        // Diplomatic recommendations
        if (chutkiBan.BribeLevel < 50 && _currentState.PlayerBudget > 50000)
            recommendations.Add("Chutki-Ban can still be influenced - continue bribery operations");
        
        if (pureLand.Relations[CountryType.ChiHan] > 70)
            recommendations.Add("PureLand-ChiHan alliance is strong - sabotage their relations");

        // Deep State recommendations
        if (_currentState.DeepStateInfluence < 30 && _currentState.YearsUntilReview < 3)
            recommendations.Add("Build deep state connections before next review");

        // Budget recommendations
        if (_currentState.PlayerBudget > 150000)
            recommendations.Add("Surplus budget available - execute high-impact operations");
        
        if (_currentState.PlayerBudget < 30000)
            recommendations.Add("CRITICAL: Low budget - focus on cheaper operations or reduce spy network");

        // Win condition proximity
        if (pureLand.Stability < 40)
            recommendations.Add("?? PRIORITY: PureLand stability weakening - push harder for victory!");
        
        if (pureLand.InsurgencyLevel > 60)
            recommendations.Add("?? PRIORITY: Insurgency levels high - victory is near!");

        return recommendations.Count > 0 ? recommendations : new List<string> { "Continue current strategy - operations are progressing well" };
    }

    // NEW: Threat Analysis
    public Dictionary<string, int> GetThreatAnalysis()
    {
        var pureLand = _currentState.Countries[CountryType.PureLand];
        
        return new Dictionary<string, int>
        {
            ["PureLand Military Threat"] = pureLand.MilitaryStrength,
            ["Counter-Intelligence Risk"] = pureLand.CounterIntelligenceLevel,
            ["Economic Resilience"] = pureLand.Economy,
            ["Government Stability"] = pureLand.Stability,
            ["Popular Support"] = pureLand.PublicSupport,
            ["ChiHan Alliance Strength"] = _currentState.Countries[CountryType.ChiHan].Relations[CountryType.PureLand]
        };
    }

    public bool CheckRandomEvent()
    {
        _turnsSinceLastEvent++;
        
        // 20% chance each turn after 2 turns (increased from 15% after 3)
        if (_turnsSinceLastEvent >= 2 && _random.Next(100) < 20)
        {
            _turnsSinceLastEvent = 0;
            return true;
        }
        return false;
    }

    public string ProcessRandomEvent()
    {
        var events = new List<(string description, System.Action effect)>
        {
            ("?? BUDGET WINDFALL: Additional funding approved - gained $50,000", () =>
            {
                _currentState.PlayerBudget += 50000;
            }),
            
            ("?? MEDIA LEAK: One of your operations was leaked to press - reputation -10", () =>
            {
                _currentState.PlayerReputation = Math.Max(0, _currentState.PlayerReputation - 10);
            }),
            
            ("??? INTELLIGENCE COUP: Recruited high-value asset - spy network +3 agents", () =>
            {
                _currentState.SpyNetworkSize += 3;
                _currentState.SpyNetworkMaintenanceCost = _currentState.SpyNetworkSize * 500;
            }),
            
            ("?? CYBER ATTACK: Your systems were compromised - lost $20,000", () =>
            {
                _currentState.PlayerBudget = Math.Max(0, _currentState.PlayerBudget - 20000);
            }),
            
            ("?? POLITICAL FAVOR: Deep state allies protected you - influence +15", () =>
            {
                _currentState.DeepStateInfluence = Math.Min(100, _currentState.DeepStateInfluence + 15);
            }),
            
            ("?? DIPLOMATIC CRISIS: International incident damaged reputation - reputation -5", () =>
            {
                _currentState.PlayerReputation = Math.Max(0, _currentState.PlayerReputation - 5);
            }),
            
            ("? INTELLIGENCE SUCCESS: Uncovered PureLand weakness - their stability -10", () =>
            {
                var pureLand = _currentState.Countries[CountryType.PureLand];
                pureLand.Stability = Math.Max(0, pureLand.Stability - 10);
            }),
            
            ("? COUNTER-OP DISCOVERED: Enemy found and eliminated your agents - spy network -2", () =>
            {
                _currentState.SpyNetworkSize = Math.Max(5, _currentState.SpyNetworkSize - 2);
                _currentState.SpyNetworkMaintenanceCost = _currentState.SpyNetworkSize * 500;
            }),
            
            ("? PERFORMANCE BONUS: Your work impressed leadership - reputation +10", () =>
            {
                _currentState.PlayerReputation = Math.Min(100, _currentState.PlayerReputation + 10);
            }),
            
            ("?? ALLY SUPPORT: OutDia provided intelligence assistance - gained $30,000", () =>
            {
                _currentState.PlayerBudget += 30000;
            }),
            
            ("??? UNEXPECTED CRISIS in PureLand: Natural disaster weakened their economy - economy -15", () =>
            {
                var pureLand = _currentState.Countries[CountryType.PureLand];
                pureLand.Economy = Math.Max(0, pureLand.Economy - 15);
                pureLand.PublicSupport = Math.Max(0, pureLand.PublicSupport - 10);
            }),
            
            ("?? REGIME CHANGE ATTEMPT in Chutki-Ban: Political instability - relations with all countries -10", () =>
            {
                var chutkiBan = _currentState.Countries[CountryType.ChutkiBan];
                chutkiBan.Stability = Math.Max(0, chutkiBan.Stability - 20);
                foreach (var key in chutkiBan.Relations.Keys.ToList())
                {
                    chutkiBan.Relations[key] = Math.Max(-100, chutkiBan.Relations[key] - 10);
                }
            }),
            
            ("?? TRAINING PROGRAM: Your agents completed advanced training - success rates improved", () =>
            {
                _currentState.PlayerReputation += 5;
            }),
            
            ("?? BUDGET AUDIT: Accounting oversight reduced available funds - lost $25,000", () =>
            {
                _currentState.PlayerBudget = Math.Max(0, _currentState.PlayerBudget - 25000);
            }),
            
            ("?? POLITICAL SHIFT in PureLand: New hardline faction gained power - counter-intel +10", () =>
            {
                var pureLand = _currentState.Countries[CountryType.PureLand];
                pureLand.CounterIntelligenceLevel = Math.Min(100, pureLand.CounterIntelligenceLevel + 10);
                pureLand.InternalSecurity = Math.Min(100, pureLand.InternalSecurity + 5);
            })
        };

        var selectedEvent = events[_random.Next(events.Count)];
        selectedEvent.effect();
        
        return selectedEvent.description;
    }

    public List<CovertAction> GetAvailableActions()
    {
        var actions = new List<CovertAction>
        {
            new CovertAction
            {
                Type = CovertActionType.FundInsurgency,
                Name = "Fund Insurgency in PureLand",
                Description = "Provide weapons and funding to rebel groups in PureLand",
                Cost = CalculateActionCost(CovertActionType.FundInsurgency, _currentState.SpyNetworkSize),
                BaseSuccessRate = 60,
                DetectionRisk = 40,
                TargetCountry = CountryType.PureLand,
                StabilityImpact = -15,
                InsurgencyImpact = 20
            },
            new CovertAction
            {
                Type = CovertActionType.BribePoliticians,
                Name = "Bribe PureLand Politicians",
                Description = "Corrupt key government officials in PureLand",
                Cost = CalculateActionCost(CovertActionType.BribePoliticians, _currentState.SpyNetworkSize),
                BaseSuccessRate = 55,
                DetectionRisk = 50,
                TargetCountry = CountryType.PureLand,
                CorruptionImpact = 15,
                PublicSupportImpact = -10
            },
            new CovertAction
            {
                Type = CovertActionType.TriggerBorderSkirmish,
                Name = "Trigger Border Skirmish (OutDia-PureLand)",
                Description = "Use OutDia to create border tensions with PureLand",
                Cost = CalculateActionCost(CovertActionType.TriggerBorderSkirmish, _currentState.SpyNetworkSize),
                BaseSuccessRate = 70,
                DetectionRisk = 30,
                TargetCountry = CountryType.PureLand,
                SecondaryTarget = CountryType.OutDia,
                StabilityImpact = -10,
                RelationsImpact = -20
            },
            new CovertAction
            {
                Type = CovertActionType.EngineerCrisis,
                Name = "Engineer Infrastructure Crisis",
                Description = "Sabotage critical infrastructure (dam, power grid, etc.)",
                Cost = CalculateActionCost(CovertActionType.EngineerCrisis, _currentState.SpyNetworkSize),
                BaseSuccessRate = 50,
                DetectionRisk = 60,
                TargetCountry = CountryType.PureLand,
                StabilityImpact = -20,
                EconomyImpact = -15,
                PublicSupportImpact = -15
            },
            new CovertAction
            {
                Type = CovertActionType.EconomicSabotage,
                Name = "Economic Sabotage",
                Description = "Disrupt trade routes and economic activities",
                Cost = CalculateActionCost(CovertActionType.EconomicSabotage, _currentState.SpyNetworkSize),
                BaseSuccessRate = 65,
                DetectionRisk = 35,
                TargetCountry = CountryType.PureLand,
                EconomyImpact = -12,
                PublicSupportImpact = -8
            },
            new CovertAction
            {
                Type = CovertActionType.ExpandSpyNetwork,
                Name = "Expand Spy Network",
                Description = "Recruit more spies (increases operational capacity but also costs)",
                Cost = 15000,
                BaseSuccessRate = 90,
                DetectionRisk = 10,
                TargetCountry = CountryType.PureLand,
                SpyNetworkChange = 5
            },
            new CovertAction
            {
                Type = CovertActionType.ReduceSpyNetwork,
                Name = "Reduce Spy Network",
                Description = "Reduce spy network to lower costs and detection risk",
                Cost = 0,
                BaseSuccessRate = 100,
                DetectionRisk = 0,
                TargetCountry = CountryType.PureLand,
                SpyNetworkChange = -5
            },
            new CovertAction
            {
                Type = CovertActionType.BribeCountry,
                Name = "Bribe Chutki-Ban",
                Description = "Pay Chutki-Ban to shift alignment away from PureLand",
                Cost = CalculateActionCost(CovertActionType.BribeCountry, _currentState.SpyNetworkSize),
                BaseSuccessRate = 75,
                DetectionRisk = 20,
                TargetCountry = CountryType.ChutkiBan,
                RelationsImpact = 15
            },
            new CovertAction
            {
                Type = CovertActionType.SabotageRelations,
                Name = "Sabotage PureLand-ChiHan Relations",
                Description = "Create diplomatic incidents between PureLand and Chi-Han",
                Cost = CalculateActionCost(CovertActionType.SabotageRelations, _currentState.SpyNetworkSize),
                BaseSuccessRate = 55,
                DetectionRisk = 45,
                TargetCountry = CountryType.PureLand,
                SecondaryTarget = CountryType.ChiHan,
                RelationsImpact = -25
            },
            new CovertAction
            {
                Type = CovertActionType.FormDeepStateConnections,
                Name = "Form Deep State Connections",
                Description = "Build connections in NotReal's government to protect your position",
                Cost = 25000,
                BaseSuccessRate = 70,
                DetectionRisk = 15,
                TargetCountry = CountryType.NotReal
            }
        };

        return actions;
    }

    public int CalculateActionCost(CovertActionType actionType, int spyNetworkSize)
    {
        int baseCost = actionType switch
        {
            CovertActionType.FundInsurgency => 20000,
            CovertActionType.BribePoliticians => 25000,
            CovertActionType.TriggerBorderSkirmish => 15000,
            CovertActionType.EngineerCrisis => 30000,
            CovertActionType.EconomicSabotage => 18000,
            CovertActionType.BribeCountry => 35000,
            CovertActionType.SabotageRelations => 20000,
            _ => 10000
        };

        // Larger spy network reduces costs (economy of scale)
        double multiplier = Math.Max(0.5, 1.0 - (spyNetworkSize - 10) * 0.02);
        return (int)(baseCost * multiplier);
    }

    public int CalculateSuccessRate(CovertAction action, int spyNetworkSize, int targetCounterIntelligence)
    {
        int baseRate = action.BaseSuccessRate;
        
        // Spy network size bonus (each 5 spies adds 5% success)
        int spyBonus = (spyNetworkSize - 10) / 5 * 5;
        
        // Counter-intelligence penalty
        int ciPenalty = (targetCounterIntelligence - 40) / 3; // Less harsh penalty
        
        int finalRate = baseRate + spyBonus - ciPenalty;
        return Math.Clamp(finalRate, 15, 90); // Increased min from 10 to 15
    }

    public OperationResult ExecuteCovertAction(CovertAction action)
    {
        var result = new OperationResult
        {
            CostIncurred = action.Cost
        };

        // Check if player has enough budget
        if (_currentState.PlayerBudget < action.Cost)
        {
            result.Success = false;
            result.Message = "Insufficient budget for this operation!";
            return result;
        }

        // Deduct cost
        _currentState.PlayerBudget -= action.Cost;

        var target = _currentState.Countries[action.TargetCountry];
        
        // Calculate success
        int successRate = CalculateSuccessRate(action, _currentState.SpyNetworkSize, 
            action.TargetCountry == CountryType.PureLand ? target.CounterIntelligenceLevel : 30);
        
        result.Success = _random.Next(100) < successRate;

        // Calculate detection
        int detectionChance = action.DetectionRisk - (_currentState.SpyNetworkSize - 10);
        result.Detected = _random.Next(100) < Math.Max(5, detectionChance);

        if (result.Success)
        {
            ApplyActionEffects(action, result);
            _currentState.SuccessfulOperations++;
            _currentState.PlayerReputation += 2;
        }
        else
        {
            result.Message = "Operation failed! No significant impact achieved.";
            _currentState.FailedOperations++;
            _currentState.PlayerReputation = Math.Max(0, _currentState.PlayerReputation - 1);
        }

        if (result.Detected)
        {
            HandleDetection(action, result);
            _currentState.DetectedOperations++;
            _currentState.PlayerReputation = Math.Max(0, _currentState.PlayerReputation - 5);
            
            // AI becomes more aggressive after detection
            _aggressiveAICounter += 2;
        }

        return result;
    }

    private void ApplyActionEffects(CovertAction action, OperationResult result)
    {
        var target = _currentState.Countries[action.TargetCountry];

        switch (action.Type)
        {
            case CovertActionType.FundInsurgency:
                target.Stability = Math.Max(0, target.Stability + action.StabilityImpact);
                target.InsurgencyLevel = Math.Min(100, target.InsurgencyLevel + action.InsurgencyImpact);
                result.Message = $"Successfully funded insurgency! PureLand stability decreased by {-action.StabilityImpact}%. ";
                result.Impacts["Stability"] = action.StabilityImpact;
                result.Impacts["Insurgency"] = action.InsurgencyImpact;
                break;

            case CovertActionType.BribePoliticians:
                target.CorruptionLevel = Math.Min(100, target.CorruptionLevel + action.CorruptionImpact);
                target.PublicSupport = Math.Max(0, target.PublicSupport + action.PublicSupportImpact);
                result.Message = $"Politicians bribed successfully! Corruption increased.";
                result.Impacts["Corruption"] = action.CorruptionImpact;
                break;

            case CovertActionType.TriggerBorderSkirmish:
                target.Stability = Math.Max(0, target.Stability + action.StabilityImpact);
                if (action.SecondaryTarget.HasValue && target.Relations.ContainsKey(action.SecondaryTarget.Value))
                {
                    target.Relations[action.SecondaryTarget.Value] = 
                        Math.Max(-100, target.Relations[action.SecondaryTarget.Value] + action.RelationsImpact);
                }
                result.Message = "Border skirmish successfully triggered! Relations deteriorated.";
                result.Impacts["Stability"] = action.StabilityImpact;
                break;

            case CovertActionType.EngineerCrisis:
                target.Stability = Math.Max(0, target.Stability + action.StabilityImpact);
                target.Economy = Math.Max(0, target.Economy + action.EconomyImpact);
                target.PublicSupport = Math.Max(0, target.PublicSupport + action.PublicSupportImpact);
                result.Message = "Infrastructure crisis engineered! Major damage to economy and stability.";
                result.Impacts["Stability"] = action.StabilityImpact;
                result.Impacts["Economy"] = action.EconomyImpact;
                break;

            case CovertActionType.EconomicSabotage:
                target.Economy = Math.Max(0, target.Economy + action.EconomyImpact);
                target.PublicSupport = Math.Max(0, target.PublicSupport + action.PublicSupportImpact);
                result.Message = "Economic sabotage successful! Trade disrupted.";
                result.Impacts["Economy"] = action.EconomyImpact;
                break;

            case CovertActionType.ExpandSpyNetwork:
                _currentState.SpyNetworkSize += action.SpyNetworkChange;
                _currentState.SpyNetworkMaintenanceCost = _currentState.SpyNetworkSize * 500;
                result.Message = $"Spy network expanded! Now operating {_currentState.SpyNetworkSize} agents.";
                break;

            case CovertActionType.ReduceSpyNetwork:
                _currentState.SpyNetworkSize = Math.Max(5, _currentState.SpyNetworkSize + action.SpyNetworkChange);
                _currentState.SpyNetworkMaintenanceCost = _currentState.SpyNetworkSize * 500;
                result.Message = $"Spy network reduced to {_currentState.SpyNetworkSize} agents. Costs lowered.";
                break;

            case CovertActionType.BribeCountry:
                var chutkiBan = _currentState.Countries[CountryType.ChutkiBan];
                chutkiBan.BribeLevel = Math.Min(100, chutkiBan.BribeLevel + 20);
                chutkiBan.Relations[CountryType.NotReal] = Math.Min(100, chutkiBan.Relations[CountryType.NotReal] + 15);
                chutkiBan.Relations[CountryType.PureLand] = Math.Max(-100, chutkiBan.Relations[CountryType.PureLand] - 10);
                result.Message = "Chutki-Ban successfully bribed! Relations improved.";
                break;

            case CovertActionType.SabotageRelations:
                if (action.SecondaryTarget.HasValue && target.Relations.ContainsKey(action.SecondaryTarget.Value))
                {
                    target.Relations[action.SecondaryTarget.Value] = 
                        Math.Max(-100, target.Relations[action.SecondaryTarget.Value] + action.RelationsImpact);
                    result.Message = "Successfully sabotaged diplomatic relations!";
                }
                break;

            case CovertActionType.FormDeepStateConnections:
                _currentState.DeepStateInfluence = Math.Min(100, _currentState.DeepStateInfluence + 15);
                result.Message = "Deep state connections established. Your position is more secure.";
                break;
        }
    }

    private void HandleDetection(CovertAction action, OperationResult result)
    {
        result.Message += " WARNING: Operation detected by enemy intelligence!";
        
        var target = _currentState.Countries[action.TargetCountry];
        
        // PureLand responds more aggressively to detection
        if (action.TargetCountry == CountryType.PureLand)
        {
            target.CounterIntelligenceLevel = Math.Min(100, target.CounterIntelligenceLevel + 8); // Increased from 5
            target.InternalSecurity = Math.Min(100, target.InternalSecurity + 5); // Increased from 3
            
            // Relations worsen more significantly
            target.Relations[CountryType.NotReal] = Math.Max(-100, target.Relations[CountryType.NotReal] - 15); // Increased from 10
        }
    }

    public void ProcessAITurn()
    {
        var pureLand = _currentState.Countries[CountryType.PureLand];
        var turnSummary = new TurnSummary
        {
            Turn = _currentState.CurrentTurn,
            Year = _currentState.CurrentYear,
            AIActions = new List<string>()
        };

        // AI assesses threats with improved logic
        int threatLevel = CalculateThreatLevel(pureLand);

        // Decay aggressive counter (over time AI calms down)
        if (_aggressiveAICounter > 0)
        {
            _aggressiveAICounter--;
        }

        // AI STRATEGIC RESPONSE - Much more dynamic
        if (threatLevel > 70 || _aggressiveAICounter > 5)
        {
            // CRISIS MODE: Maximum response
            pureLand.CounterIntelligenceLevel = Math.Min(100, pureLand.CounterIntelligenceLevel + 8);
            pureLand.InternalSecurity = Math.Min(100, pureLand.InternalSecurity + 7);
            pureLand.MilitaryStrength = Math.Min(100, pureLand.MilitaryStrength + 5);
            pureLand.BorderDefense = Math.Min(100, pureLand.BorderDefense + 6);
            turnSummary.AIActions.Add("?? PureLand declared STATE OF EMERGENCY - massive security buildup!");
            
            // Aggressive counter-operation
            if (_random.Next(100) < 60 && _currentState.DetectedOperations > 1)
            {
                int budgetLoss = _random.Next(15000, 30000);
                _currentState.PlayerBudget = Math.Max(0, _currentState.PlayerBudget - budgetLoss);
                int spyLoss = _random.Next(2, 5);
                _currentState.SpyNetworkSize = Math.Max(5, _currentState.SpyNetworkSize - spyLoss);
                _currentState.SpyNetworkMaintenanceCost = _currentState.SpyNetworkSize * 500;
                turnSummary.AIActions.Add($"?? PureLand conducted MAJOR counter-op: Lost {spyLoss} agents and ${budgetLoss}!");
                _currentState.PlayerReputation = Math.Max(0, _currentState.PlayerReputation - 8);
                _aggressiveAICounter = Math.Max(0, _aggressiveAICounter - 3); // Counter-op released pressure
            }
        }
        else if (threatLevel > 50 || _aggressiveAICounter > 2)
        {
            // HIGH ALERT: Strong response
            pureLand.CounterIntelligenceLevel = Math.Min(100, pureLand.CounterIntelligenceLevel + 6);
            pureLand.InternalSecurity = Math.Min(100, pureLand.InternalSecurity + 5);
            pureLand.MilitaryStrength = Math.Min(100, pureLand.MilitaryStrength + 3);
            turnSummary.AIActions.Add("?? PureLand significantly increased security measures");
            
            // Moderate counter-operation chance
            if (_random.Next(100) < 40 && _currentState.DetectedOperations > 0)
            {
                int budgetLoss = _random.Next(8000, 15000);
                _currentState.PlayerBudget = Math.Max(0, _currentState.PlayerBudget - budgetLoss);
                int spyLoss = _random.Next(1, 3);
                _currentState.SpyNetworkSize = Math.Max(5, _currentState.SpyNetworkSize - spyLoss);
                _currentState.SpyNetworkMaintenanceCost = _currentState.SpyNetworkSize * 500;
                turnSummary.AIActions.Add($"?? PureLand counter-intelligence strike: Lost {spyLoss} agents and ${budgetLoss}");
                _currentState.PlayerReputation = Math.Max(0, _currentState.PlayerReputation - 5);
            }
        }
        else if (threatLevel > 30)
        {
            // MODERATE CONCERN: Balanced approach
            pureLand.CounterIntelligenceLevel = Math.Min(100, pureLand.CounterIntelligenceLevel + 4);
            pureLand.InternalSecurity = Math.Min(100, pureLand.InternalSecurity + 3);
            pureLand.Economy = Math.Min(100, pureLand.Economy + 3);
            turnSummary.AIActions.Add("?? PureLand balanced security and economic development");
            
            // Small counter-op chance
            if (_random.Next(100) < 20 && _currentState.DetectedOperations > 2)
            {
                int budgetLoss = _random.Next(5000, 10000);
                _currentState.PlayerBudget = Math.Max(0, _currentState.PlayerBudget - budgetLoss);
                turnSummary.AIActions.Add($"??? PureLand minor counter-operation: Lost ${budgetLoss}");
                _currentState.PlayerReputation = Math.Max(0, _currentState.PlayerReputation - 2);
            }
        }
        else
        {
            // LOW THREAT: Focus on development
            pureLand.Economy = Math.Min(100, pureLand.Economy + 5);
            pureLand.PublicSupport = Math.Min(100, pureLand.PublicSupport + 4);
            pureLand.Stability = Math.Min(100, pureLand.Stability + 3);
            turnSummary.AIActions.Add("?? PureLand focused on economic growth and public welfare");
        }

        // Handle insurgency with improved logic
        if (pureLand.InsurgencyLevel > 50)
        {
            // Major crackdown
            int suppressionEffort = Math.Min(pureLand.InsurgencyLevel / 2, 20);
            pureLand.InsurgencyLevel = Math.Max(0, pureLand.InsurgencyLevel - suppressionEffort);
            pureLand.PublicSupport = Math.Max(0, pureLand.PublicSupport - 8); // Harsh suppression is unpopular
            pureLand.MilitaryStrength = Math.Max(0, pureLand.MilitaryStrength - 3); // Military weakened by combat
            turnSummary.AIActions.Add($"?? PureLand launched MAJOR counter-insurgency campaign (reduced by {suppressionEffort}%)");
        }
        else if (pureLand.InsurgencyLevel > 30)
        {
            // Moderate suppression
            int suppressionEffort = Math.Min(pureLand.InsurgencyLevel / 3, 12);
            pureLand.InsurgencyLevel = Math.Max(0, pureLand.InsurgencyLevel - suppressionEffort);
            pureLand.PublicSupport = Math.Max(0, pureLand.PublicSupport - 4);
            turnSummary.AIActions.Add($"??? PureLand suppressed insurgency (reduced by {suppressionEffort}%)");
        }
        else if (pureLand.InsurgencyLevel > 10)
        {
            // Light suppression
            int suppressionEffort = 5;
            pureLand.InsurgencyLevel = Math.Max(0, pureLand.InsurgencyLevel - suppressionEffort);
            turnSummary.AIActions.Add($"?? PureLand police operations reduced insurgency by {suppressionEffort}%");
        }

        // Natural stability recovery (slower if insurgency is high)
        if (pureLand.InsurgencyLevel < 20)
        {
            if (pureLand.Stability < 70)
            {
                int recoveryAmount = pureLand.Economy > 60 ? 5 : 3;
                pureLand.Stability = Math.Min(100, pureLand.Stability + recoveryAmount);
                if (recoveryAmount > 3)
                {
                    turnSummary.AIActions.Add("? Strong economy helping PureLand stability recover");
                }
            }
        }

        // Economic recovery/decline based on stability
        if (pureLand.Stability > 60 && pureLand.Economy < 90)
        {
            pureLand.Economy = Math.Min(100, pureLand.Economy + 2);
        }
        else if (pureLand.Stability < 40 && pureLand.Economy > 20)
        {
            pureLand.Economy = Math.Max(0, pureLand.Economy - 3);
            turnSummary.AIActions.Add("?? Low stability damaging PureLand's economy");
        }

        // Public support dynamics
        if (pureLand.CorruptionLevel > 60)
        {
            pureLand.PublicSupport = Math.Max(0, pureLand.PublicSupport - 3);
            turnSummary.AIActions.Add("?? High corruption eroding public support in PureLand");
        }
        
        if (pureLand.Stability < 40 || pureLand.Economy < 40)
        {
            pureLand.PublicSupport = Math.Max(0, pureLand.PublicSupport - 2);
            turnSummary.AIActions.Add("?? Poor conditions reducing PureLand government support");
        }

        // Chi-Han assistance if relations are strong
        if (pureLand.Relations[CountryType.ChiHan] > 60 && threatLevel > 50)
        {
            pureLand.Economy = Math.Min(100, pureLand.Economy + 5);
            pureLand.MilitaryStrength = Math.Min(100, pureLand.MilitaryStrength + 3);
            turnSummary.AIActions.Add("?? Chi-Han provided economic and military aid to PureLand");
        }

        // I-Walk assistance if relations are strong
        if (pureLand.Relations[CountryType.IWalk] > 50 && pureLand.InsurgencyLevel > 40)
        {
            pureLand.InsurgencyLevel = Math.Max(0, pureLand.InsurgencyLevel - 5);
            turnSummary.AIActions.Add("?? I-Walk provided intelligence assistance to combat insurgency");
        }

        _currentState.TurnHistory.Add(turnSummary);
    }

    private int CalculateThreatLevel(Country pureLand)
    {
        int threat = 0;
        
        threat += (100 - pureLand.Stability) / 2; // Max 50
        threat += pureLand.InsurgencyLevel / 2; // Max 50
        threat += (100 - pureLand.PublicSupport) / 3; // Max ~33
        threat += pureLand.CorruptionLevel / 4; // Max 25
        threat -= pureLand.Economy / 5; // Strong economy reduces threat perception
        
        return Math.Clamp(threat, 0, 100);
    }

    public void ProcessYearEnd()
    {
        _currentState.CurrentYear++;
        _currentState.YearsUntilReview--;

        // Annual budget allocation
        _currentState.PlayerBudget += _currentState.AnnualBudget;
        
        // Deduct spy network maintenance
        _currentState.PlayerBudget -= _currentState.SpyNetworkMaintenanceCost;

        // Check if player is out of money
        if (_currentState.PlayerBudget < 0)
        {
            _currentState.GameOver = true;
            _currentState.GameOverReason = "?? Ran out of operational funds! You have been recalled from duty.";
            _currentState.PlayerWon = false;
            return;
        }

        // Check win condition
        var pureLand = _currentState.Countries[CountryType.PureLand];
        if (pureLand.Stability < 30 || pureLand.InsurgencyLevel > 70)
        {
            _currentState.GameOver = true;
            _currentState.GameOverReason = "?? VICTORY! PureLand has collapsed into chaos. Your mission is complete!";
            _currentState.PlayerWon = true;
            return;
        }
    }

    public bool PerformReview()
    {
        int score = CalculateReviewScore();

        // Pass threshold: 60
        bool passed = score >= 60;

        if (!passed)
        {
            _currentState.GameOver = true;
            _currentState.GameOverReason = $"? Failed performance review! Score: {score}/100. You have been recalled from duty.";
            _currentState.PlayerWon = false;
        }
        else
        {
            // Successful review - boost reputation
            _currentState.PlayerReputation = Math.Min(100, _currentState.PlayerReputation + 10);
            
            // Check win condition
            var pureLand = _currentState.Countries[CountryType.PureLand];
            if (pureLand.Stability < 30 || pureLand.InsurgencyLevel > 70)
            {
                _currentState.GameOver = true;
                _currentState.GameOverReason = $"?? VICTORY! PureLand has collapsed! Final Review Score: {score}/100";
                _currentState.PlayerWon = true;
            }
        }

        return passed;
    }
}
