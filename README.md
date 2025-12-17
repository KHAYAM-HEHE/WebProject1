# MIC: Man In Charge

## A Turn-Based AI-Driven Geopolitical Simulation Game

![.NET 9](https://img.shields.io/badge/.NET-9.0-blue)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Razor%20Pages-green)

### Overview

**MIC (Man-In-Charge)** is an advanced geopolitical simulation game where you play as the Director of the **Covert Influence & Destabilization Directorate (CIDD)**, a division of **DART (Directorate of Asymmetric Reconnaissance and Tactical Operations)** in the fictional nation of **NotReal**.

Your mission: Strategically destabilize the rival nation **PureLand** using covert operations, espionage, and asymmetric warfare tactics—all while managing your budget, spy network, and reputation.

### Game Features

#### ?? Strategic Gameplay
- **Turn-based strategy** with deep tactical decisions
- **10+ covert operations** including insurgency funding, political bribery, infrastructure sabotage, and more
- **Dynamic AI opponent** that adapts to your strategies
- **Resource management** balancing budget, spy network size, and operational costs
- **5-year performance reviews** that determine if you keep your position

#### ?? Fictional World
- **6 countries** with unique alignments and characteristics:
  - **NotReal** - Your home nation
  - **PureLand** - AI-controlled target nation
  - **I-Walk** - Hostile theocracy aligned with PureLand
  - **OutDia** - Your strategic ally
  - **Chutki-Ban** - Unstable theocracy (can be bribed)
  - **Chi-Han** - Powerful socialist ally of PureLand

#### ?? Advanced AI
- Assesses threats dynamically
- Allocates resources to counter your strategies
- Conducts counter-intelligence operations
- Faces its own internal reviews (failures cause instability)
- Adapts behavior based on threat levels

#### ?? Deep Metrics
Track multiple stats for each country:
- Stability (0-100)
- Economy (0-100)
- Military Strength (0-100)
- Public Support (0-100)
- Insurgency Level (0-100)
- Corruption Level (0-100)
- Counter-Intelligence (0-100)
- Diplomatic Relations

### How to Play

#### Objective
Destabilize PureLand by reducing its stability below 30% OR increasing insurgency above 70% while maintaining your position through 5-year performance reviews.

#### Win Conditions
- PureLand collapses (Stability < 30 OR Insurgency > 70)
- Pass performance reviews every 5 years (score 60+)

#### Lose Conditions
- Fail a performance review (score < 60)
- Run out of operational funds

#### Available Operations

1. **Fund Insurgency** - Arm and fund rebel groups (high impact on stability)
2. **Bribe Politicians** - Corrupt government officials (increases corruption)
3. **Trigger Border Skirmish** - Use allies to create border tensions
4. **Engineer Crisis** - Sabotage critical infrastructure (dam, power grid)
5. **Economic Sabotage** - Disrupt trade routes and economy
6. **Expand Spy Network** - Recruit more agents (better success rates, higher costs)
7. **Reduce Spy Network** - Cut costs by reducing agents
8. **Bribe Chutki-Ban** - Pay to shift their alignment
9. **Sabotage Relations** - Damage diplomatic ties between nations
10. **Form Deep State Connections** - Build protection within your own government

#### Strategy Tips

?? **Budget Management**
- Start with $100,000 and receive annual funding
- Spy network maintenance costs scale with size (500 per agent)
- Failed operations still cost money

?? **Risk vs. Reward**
- Higher-impact operations have lower success rates
- Detection damages your reputation significantly
- Building deep state connections protects you during reviews

?? **Optimal Spy Network**
- Larger networks: Higher success rates, lower operation costs, but higher maintenance
- Smaller networks: Lower costs but worse success rates
- Sweet spot: 15-20 agents for balanced gameplay

?? **AI Adaptation**
- The AI increases security spending when threatened
- Detected operations make future operations harder
- AI can conduct counter-operations against you

?? **Multi-Pronged Approach**
- Combine different operation types for maximum effect
- Use allies strategically (OutDia for border conflicts)
- Target corruption and insurgency together for cascading effects

### Technical Details

#### Technologies Used
- **.NET 9** - Latest .NET framework
- **ASP.NET Core Razor Pages** - Server-side web framework
- **Bootstrap 5** - Responsive UI framework
- **C# 13** - Modern C# features

#### Architecture
```
MICGame/
??? Models/
?   ??? Country.cs           - Country data model with stats
?   ??? CovertAction.cs      - Operations and results
?   ??? GameState.cs         - Overall game state
??? Services/
?   ??? GameService.cs       - Core game logic and AI
??? Pages/
?   ??? Index.cshtml         - Main game UI
?   ??? Index.cshtml.cs      - Page model with handlers
?   ??? About.cshtml         - Game documentation
??? wwwroot/
    ??? css/site.css         - Custom styling
```

#### Key Design Patterns
- **Singleton Pattern** - Game state persists across requests
- **Service Layer** - Separation of game logic from presentation
- **Model-View Pattern** - Clean separation of concerns

### Running the Game

#### Prerequisites
- .NET 9 SDK or later
- A modern web browser

#### Steps
1. Clone or download the repository
2. Navigate to the project directory
3. Run the following command:
   ```bash
   dotnet run
   ```
4. Open your browser to `https://localhost:5001` or `http://localhost:5000`
5. Start playing!

### Game Mechanics Deep Dive

#### Performance Review Scoring
Your review score is calculated based on:
- **Base Reputation**: Starting at 50
- **Success Rate**: +50 points for 100% success rate
- **Detection Penalty**: -2 points per detected operation
- **Deep State Bonus**: +1/3 of deep state influence
- **Impact Bonus**: Points for damage dealt to PureLand

**Passing Score**: 60/100

#### AI Decision Making
The AI evaluates threat level each turn:
- **Threat Level = (100 - Stability)/2 + Insurgency/2 + (100 - PublicSupport)/3 + Corruption/4**

Based on threat:
- **High (60+)**: Focuses on security and military
- **Medium (30-60)**: Balanced approach
- **Low (<30)**: Focuses on economy and public support

#### Operation Success Calculation
```
Success Rate = Base Rate + (Spy Network Bonus) - (Counter-Intel Penalty)
Spy Bonus = ((Network Size - 10) / 5) * 5
CI Penalty = (Counter Intel - 50) / 2
Final Rate = Clamp(Success Rate, 10, 95)
```

#### Detection Risk
```
Detection Chance = Base Risk - (Network Size - 10)
Minimum Detection: 5%
```

### Future Enhancements

Potential features for future versions:
- [ ] Save/Load game functionality
- [ ] Multiple difficulty levels
- [ ] Historical event log viewer
- [ ] More covert operation types
- [ ] Multiplayer mode (human vs. human)
- [ ] Enhanced AI with machine learning
- [ ] Economic warfare mechanics
- [ ] Nuclear/WMD systems
- [ ] Cyber warfare operations
- [ ] Diplomatic victory conditions

### Credits

Developed as an AI programming project demonstrating:
- Multi-agent decision making
- Strategic planning under uncertainty
- Adversarial AI behavior
- Dynamic resource allocation
- Political and social instability modeling
- Game theory and utility-based decision trees

### License

This is an educational project. Feel free to learn from, modify, and extend it.

---

**"In the shadows of geopolitics, every decision shapes the future of nations."**

?? Good luck, Director. NotReal is counting on you.
