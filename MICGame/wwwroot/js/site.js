// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// MIC Game Interactive Features

document.addEventListener('DOMContentLoaded', function() {
    // Add confirmation for critical actions
    const endTurnButton = document.querySelector('button[asp-page-handler="EndTurn"]');
    if (endTurnButton) {
        endTurnButton.addEventListener('click', function(e) {
            if (!confirm('End turn and process AI response? This cannot be undone.')) {
                e.preventDefault();
            }
        });
    }

    // Add hover effects for action cards
    const actionCards = document.querySelectorAll('.action-card');
    actionCards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            const radio = this.querySelector('input[type="radio"]');
            if (radio && !radio.disabled) {
                this.style.borderColor = '#007bff';
                this.style.backgroundColor = '#f0f8ff';
            }
        });
        
        card.addEventListener('mouseleave', function() {
            const radio = this.querySelector('input[type="radio"]');
            if (radio && !radio.checked) {
                this.style.borderColor = '#ccc';
                this.style.backgroundColor = 'white';
            }
        });
    });

    // Highlight selected action
    const radioButtons = document.querySelectorAll('input[type="radio"][name="actionIndex"]');
    radioButtons.forEach(radio => {
        radio.addEventListener('change', function() {
            // Remove highlight from all cards
            actionCards.forEach(card => {
                card.style.borderColor = '#ccc';
                card.style.backgroundColor = 'white';
            });
            
            // Highlight selected card
            const selectedCard = this.closest('.action-card');
            if (selectedCard) {
                selectedCard.style.borderColor = '#28a745';
                selectedCard.style.backgroundColor = '#f0fff4';
            }
        });
    });

    // Add tooltips for stats
    const statBars = document.querySelectorAll('.stat-bar');
    statBars.forEach(bar => {
        const fill = bar.querySelector('.stat-fill');
        if (fill) {
            const percentage = fill.style.width;
            bar.title = `Current value: ${percentage}`;
        }
    });

    // Auto-dismiss alerts after 5 seconds
    const alerts = document.querySelectorAll('.alert-custom:not(.alert-info)');
    alerts.forEach(alert => {
        if (!alert.textContent.includes('GAME OVER') && !alert.textContent.includes('VICTORY')) {
            setTimeout(() => {
                alert.style.opacity = '0';
                setTimeout(() => {
                    alert.style.display = 'none';
                }, 500);
            }, 5000);
        }
    });

    // Add loading state to buttons
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function() {
            const submitButton = this.querySelector('button[type="submit"]');
            if (submitButton) {
                submitButton.disabled = true;
                submitButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Processing...';
            }
        });
    });

    // Animate country cards on load
    const countryCards = document.querySelectorAll('.country-card');
    countryCards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        setTimeout(() => {
            card.style.transition = 'all 0.5s ease';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, index * 100);
    });

    // Add pulse effect to critical stats
    setInterval(() => {
        const pureLandCard = document.querySelector('.country-hostile');
        if (pureLandCard) {
            const stabilityBar = pureLandCard.querySelector('.stat-fill.stat-stability');
            if (stabilityBar) {
                const stability = parseInt(stabilityBar.style.width);
                if (stability < 40) {
                    stabilityBar.style.animation = 'pulse 1s ease-in-out';
                    setTimeout(() => {
                        stabilityBar.style.animation = '';
                    }, 1000);
                }
            }
        }
    }, 3000);

    // Budget warning
    const budgetElement = document.querySelector('.resource-panel h3');
    if (budgetElement) {
        const budgetText = budgetElement.textContent.replace(/[^0-9]/g, '');
        const budget = parseInt(budgetText);
        if (budget < 30000) {
            budgetElement.style.color = '#dc3545';
            budgetElement.style.animation = 'pulse 2s infinite';
        }
    }

    // Add keyboard shortcuts
    document.addEventListener('keydown', function(e) {
        // Press 'E' to end turn
        if (e.key === 'e' || e.key === 'E') {
            const endTurnBtn = document.querySelector('button[asp-page-handler="EndTurn"]');
            if (endTurnBtn && !e.ctrlKey && !e.altKey) {
                endTurnBtn.click();
            }
        }
        
        // Press number keys 1-9 to select actions
        if (e.key >= '1' && e.key <= '9') {
            const index = parseInt(e.key) - 1;
            const radio = document.querySelector(`input[type="radio"][name="actionIndex"][value="${index}"]`);
            if (radio && !radio.disabled) {
                radio.checked = true;
                radio.dispatchEvent(new Event('change'));
            }
        }
    });

    // Add game tips tooltip
    console.log('%c🎯 MIC Game Tips:', 'color: #007bff; font-size: 16px; font-weight: bold;');
    console.log('%c• Press 1-9 to quickly select operations', 'color: #28a745;');
    console.log('%c• Press E to end turn', 'color: #28a745;');
    console.log('%c• Build deep state connections early for review protection', 'color: #ffc107;');
    console.log('%c• Larger spy networks = higher success rates', 'color: #ffc107;');
    console.log('%c• Watch your budget! Maintenance costs add up', 'color: #dc3545;');
});

// Smooth scroll to top on page load
window.scrollTo({ top: 0, behavior: 'smooth' });
