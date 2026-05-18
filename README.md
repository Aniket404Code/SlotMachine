# Game Overview

This project is a basic Slot Machine Game developed in Unity as part of the assignment.
The game includes smooth reel spinning animations, randomized outcomes using RNG, symbol matching win logic,
and payout simulation based on winning combinations.

# Instructions to Run WebGL Build
- Open the project folder in Visual Studio Code.
- Navigate to the WebGL Build folder.
- Locate the index.html file.
- Right-click on the index.html file.
- Select “Open with Live Server”.
- The game will automatically open in your browser.
- Set the browser zoom/aspect view to approximately 33% for complete game view


# Thought Process / Approach

The main goal while developing this slot machine game was to create a simple but polished gameplay experience with clean architecture and smooth user interaction.

# Development Approach
## 1. Reel System Design

The reel system was designed to simulate a real slot machine experience.
Each reel spins independently with smooth animation timing and stops sequentially to improve visual feedback and game feel.

## 2. Randomization Logic (RNG)

Random outcomes are generated using Unity’s randomization system to ensure fair and unpredictable results for every spin.
Each reel selects symbols independently, making every game round unique.

## 3. Winning Logic

The winning condition is based on all reels showing the same symbol after stopping.
Once all reels stop spinning:

Symbols are compared
Winning combinations are checked
Payout logic is triggered if conditions are met

## 4. Animation & User Experience

Special attention was given to:

Smooth reel movement
Symbol alignment
Consistent UI feedback
Responsive spin interactions

The goal was to keep the gameplay visually clean and easy to understand.

## 5. Code Structure

The project was organized into separate responsibilities such as:

Reel control
Game management
RNG handling
UI updates
Win checking logic

This helped maintain cleaner and more manageable code.
