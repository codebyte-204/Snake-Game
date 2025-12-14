🐍 Snake Game (C# WinForms):

A classic Snake Game built using C# Windows Forms with modern visuals, sound effects, difficulty levels, obstacles, bonus food, and persistent high scores.

This project is beginner-friendly and well-structured, making it suitable for learning game loops, event handling, and graphics rendering in WinForms.



> 🎮 Features

* 🐍 Classic snake movement using arrow keys
* 🍎 Normal food to grow the snake
* ⭐ Bonus food (gold) appears every few foods
* ⏱️ Bonus timer bar for limited-time bonus food
* 🧱 Obstacles and maze generation
* 🎚️ Difficulty levels: Easy, Normal, Hard
* 🔊 Sound effects (eat, bonus, click, game over)
* 🏆 High score saved to local file
* 🎨 Neon-style grid and colors



🧠 Game Logic Overview

> Game States

1. Start → Waiting for player to start
2. Running  → Snake is moving and game is active

> Movement

* Controlled using Arrow Keys
* Direction changes are validated to prevent reverse movement

> Difficulty Modes

| Difficulty | Speed  | Obstacles     |
| ---------- | ------ | ------------- |
| Easy       | Slow   | None          |
| Normal     | Medium | Random blocks |
| Hard       | Fast   | Maze pattern  |



 🍎 Food & Bonus System

* Normal Food (Red)

  * Increases score by 1
  * Increases snake length

* Bonus Food (Gold)

  * Appears after every 3 foods eaten
  * Gives +5 score
  * Disappears after a short time
  * Has a visual timer bar


🧱 Obstacles

* Normal Mode: Random obstacles
* Hard Mode: Maze-style obstacles
* Collision with obstacles ends the game



🖼️ Graphics & Rendering

* Drawn using `Graphics` in `Paint` event
* Neon grid and borders
* Snake head highlighted differently
* Double-buffering enabled to prevent flickering



🔊 Sound Effects

The game uses `SoundPlayer` with embedded `.wav` resources:

* `eat.wav` → Eating food
* `bonus.wav` → Bonus food
* `click.wav` → Button click
* `gameover.wav` → Game over

> Sounds are stored in Properties → Resources


 💾 High Score Storage

High score is saved locally in:

  ```
  %AppData%\MySnakeGame\highscore.txt
  ```
Automatically loaded when the game starts


 🎮 Controls

| Key   | Action     |
| ----- | ---------- |
| ↑     | Move Up    |
| ↓     | Move Down  |
| ←     | Move Left  |
| →     | Move Right |
| Enter | Start Game |



🛠️ Required UI Controls (Designer)

Make sure these controls exist in Form Designer:

* `gamePanel` (Panel)
* `btnStart` (Button)
* `btnRestart` (Button)
* `comboSpeed` (ComboBox)
* `lblScore` (Label)
* `lblHighScore` (Label)
* `startPanel` (Panel)
* `gameOverPanel` (Panel)
* `tmrGameTimer` (Timer)



🚀 How to Run

1. Open the project in Visual Studio
2. Ensure sound files are added to Resources
3. Build the solution
4. Press Start or hit Enter
5. Use arrow keys to play 🎉


📚 Learning Outcomes

This project helps you understand:

* WinForms event-driven programming
* Game loops using `Timer`
* Collision detection
* Basic game state management
* Drawing graphics manually
* File handling in C#

🔮 Possible Future Enhancements

* Energy booster power-ups
* Level progression system
* Smooth animation interpolation
* Settings menu
* Mobile or WPF version

👩‍💻 Author

Created as a learning project using C# WinForms.

Happy Coding & Gaming! 🐍✨
