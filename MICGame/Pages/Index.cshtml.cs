using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MICGame.Models;
using MICGame.Services;

namespace MICGame.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IGameService _gameService;

    public GameState GameState { get; set; } = new();
    public List<CovertAction> AvailableActions { get; set; } = new();
    
    [BindProperty]
    public int SelectedActionIndex { get; set; }
    
    public OperationResult? LastActionResult { get; set; }
    public bool ShowReviewResults { get; set; }
    public int ReviewScore { get; set; }
    public int CurrentReviewScore { get; set; } // NEW: Show current score
    public string? RandomEvent { get; set; } // NEW: Random events
    public string? YearEndMessage { get; set; } // NEW: Year end feedback

    public IndexModel(ILogger<IndexModel> logger, IGameService gameService)
    {
        _logger = logger;
        _gameService = gameService;
    }

    public void OnGet()
    {
        LoadGameState();
    }

    public IActionResult OnPostExecuteAction(int actionIndex)
    {
        LoadGameState();
        
        if (GameState.GameOver)
        {
            return Page();
        }

        var actions = _gameService.GetAvailableActions();
        if (actionIndex >= 0 && actionIndex < actions.Count)
        {
            var action = actions[actionIndex];
            LastActionResult = _gameService.ExecuteCovertAction(action);
            
            // Store result in TempData to show after redirect
            TempData["LastActionMessage"] = LastActionResult.Message;
            TempData["ActionSuccess"] = LastActionResult.Success;
            TempData["ActionDetected"] = LastActionResult.Detected;
            
            // Process AI turn immediately after player action
            _gameService.ProcessAITurn();
            
            // Check for random event after action
            if (_gameService.CheckRandomEvent())
            {
                TempData["RandomEvent"] = _gameService.ProcessRandomEvent();
            }
        }

        return RedirectToPage();
    }

    public IActionResult OnPostEndTurn()
    {
        LoadGameState();
        
        if (GameState.GameOver)
        {
            return Page();
        }

        // Process AI turn
        _gameService.ProcessAITurn();
        
        // Check for random event
        if (_gameService.CheckRandomEvent())
        {
            TempData["RandomEvent"] = _gameService.ProcessRandomEvent();
        }
        
        // Process year end
        _gameService.ProcessYearEnd();
        
        TempData["YearEndMessage"] = $"Year {GameState.CurrentYear - 1} complete. New budget allocated: ${GameState.AnnualBudget:N0}";
        
        // Check for review (every 5 years)
        if (GameState.YearsUntilReview == 0)
        {
            bool passed = _gameService.PerformReview();
            int reviewScore = _gameService.CalculateReviewScore();
            TempData["ShowReview"] = true;
            TempData["ReviewPassed"] = passed;
            TempData["ReviewScore"] = reviewScore;
            
            // Reset review timer if passed
            if (passed && !GameState.GameOver)
            {
                GameState.YearsUntilReview = 5;
                _gameService.SaveGameState(GameState);
            }
        }

        return RedirectToPage();
    }

    public IActionResult OnPostNewGame()
    {
        _gameService.InitializeGame();
        TempData["GameMessage"] = "?? New game started! Good luck, Director. Your mission: Destabilize PureLand.";
        return RedirectToPage();
    }

    private void LoadGameState()
    {
        GameState = _gameService.GetCurrentGameState();
        AvailableActions = _gameService.GetAvailableActions();
        
        // Calculate current review score for display
        CurrentReviewScore = _gameService.CalculateReviewScore();
        
        // Load TempData messages if available
        if (TempData["LastActionMessage"] != null)
        {
            LastActionResult = new OperationResult
            {
                Message = TempData["LastActionMessage"]?.ToString() ?? "",
                Success = TempData["ActionSuccess"] as bool? ?? false,
                Detected = TempData["ActionDetected"] as bool? ?? false
            };
        }
        
        if (TempData["ShowReview"] as bool? ?? false)
        {
            ShowReviewResults = true;
            ReviewScore = TempData["ReviewScore"] as int? ?? 0;
        }
        
        if (TempData["RandomEvent"] != null)
        {
            RandomEvent = TempData["RandomEvent"]?.ToString();
        }
        
        if (TempData["YearEndMessage"] != null)
        {
            YearEndMessage = TempData["YearEndMessage"]?.ToString();
        }
    }
}
