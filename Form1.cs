using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace WindowsFormsApp1
{
        public partial class Form1 : Form
        {
        // ===== GRID =====
        private const int gridWidth = 25;
        private const int gridHeight = 20;
        private const int cellSize = 20;

        // ===== GAME STATE =====
        private enum GameState { Start, Running }
        private GameState gameState = GameState.Start;

        private enum Direction { Up, Down, Left, Right }
        private Direction currentDirection = Direction.Right;
        private Direction nextDirection = Direction.Right;

        private enum Difficulty { Easy, Normal, Hard }
        private Difficulty currentDifficulty = Difficulty.Easy;

        // ===== GAME DATA =====
        private List<Point> snake = new List<Point>();
        private List<Point> obstacles = new List<Point>();
        private Point food;
        private Point bonusFood;

        private bool hasBonus = false;
        private int bonusTimer = 0;
        private const int BONUS_LIFETIME = 50;

        private int foodEatenCount = 0;
        private int score = 0;
        private int highScore = 0;

        private Random rnd = new Random();

        // ===== FILE =====
        private readonly string scoreFilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MySnakeGame", "highscore.txt");

        // ===== SOUND =====
        SoundPlayer sndEat = new SoundPlayer(Properties.Resources.eat);
        SoundPlayer sndBonus = new SoundPlayer(Properties.Resources.bonus);
        SoundPlayer sndGameOver = new SoundPlayer(Properties.Resources.gameover);
        SoundPlayer sndClick = new SoundPlayer(Properties.Resources.click);

        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;

            // Game panel size
            gamePanel.Width = gridWidth * cellSize;
            gamePanel.Height = gridHeight * cellSize;

            typeof(Panel).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(gamePanel, true, null);

            // Timer
            tmrGameTimer.Interval = 150;
            tmrGameTimer.Tick += GameTimer_Tick;

            // Events
            gamePanel.Paint += GamePanel_Paint;
            KeyDown += Form1_KeyDown;
            btnStart.Click += BtnStart_Click;
            btnRestart.Click += BtnRestart_Click;

            LoadHighScore();

            startPanel.Visible = true;
            gameOverPanel.Visible = false;
        }

        // ===== START GAME =====
        private void StartGame()
        {
            sndClick.Play();

            snake.Clear();
            obstacles.Clear();

            snake.Add(new Point(gridWidth / 2, gridHeight / 2));
            currentDirection = nextDirection = Direction.Right;

            score = 0;
            foodEatenCount = 0;
            hasBonus = false;

            DetectDifficulty();
            ApplyDifficulty();
            GenerateFood();

            UpdateLabels();

            startPanel.Visible = false;
            gameOverPanel.Visible = false;

            gameState = GameState.Running;
            tmrGameTimer.Start();
        }

        private void BtnStart_Click(object sender, EventArgs e) => StartGame();
        private void BtnRestart_Click(object sender, EventArgs e) => StartGame();

        // ===== KEY CONTROL =====
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState == GameState.Start && e.KeyCode == Keys.Enter)
            {
                StartGame();
                return;
            }

            if (gameState != GameState.Running) return;

            switch (e.KeyCode)
            {
                case Keys.Up: if (currentDirection != Direction.Down) nextDirection = Direction.Up; break;
                case Keys.Down: if (currentDirection != Direction.Up) nextDirection = Direction.Down; break;
                case Keys.Left: if (currentDirection != Direction.Right) nextDirection = Direction.Left; break;
                case Keys.Right: if (currentDirection != Direction.Left) nextDirection = Direction.Right; break;
            }
        }

        // ===== DIFFICULTY =====
        private void DetectDifficulty()
        {
            switch (comboSpeed.SelectedItem?.ToString())
            {
                case "Normal": currentDifficulty = Difficulty.Normal; break;
                case "Hard": currentDifficulty = Difficulty.Hard; break;
                default: currentDifficulty = Difficulty.Easy; break;
            }
        }

        private void ApplyDifficulty()
        {
            obstacles.Clear();

            switch (currentDifficulty)
            {
                case Difficulty.Easy:
                    tmrGameTimer.Interval = 220;
                    break;

                case Difficulty.Normal:
                    tmrGameTimer.Interval = 150;
                    AddRandomObstacles(6);
                    break;

                case Difficulty.Hard:
                    tmrGameTimer.Interval = 100;
                    GenerateMaze();
                    break;
            }
        }

        private void AddRandomObstacles(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Point p;
                do
                {
                    p = new Point(rnd.Next(1, gridWidth - 1), rnd.Next(1, gridHeight - 1));
                }
                while (snake.Contains(p) || obstacles.Contains(p));

                obstacles.Add(p);
            }
        }

        private void GenerateMaze()
        {
            for (int y = 2; y < gridHeight - 2; y += 3)
                for (int x = 2; x < gridWidth - 2; x++)
                    if (x % 4 == 0)
                        obstacles.Add(new Point(x, y));
        }

        // ===== GAME LOOP =====
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (gameState != GameState.Running) return;
            MoveSnake();
            gamePanel.Invalidate();
        }

        private void MoveSnake()
        {
            currentDirection = nextDirection;
            Point head = snake[0];
            Point newHead = head;

            if (currentDirection == Direction.Up) newHead.Y--;
            if (currentDirection == Direction.Down) newHead.Y++;
            if (currentDirection == Direction.Left) newHead.X--;
            if (currentDirection == Direction.Right) newHead.X++;

            if (newHead.X < 0 || newHead.X >= gridWidth ||
                newHead.Y < 0 || newHead.Y >= gridHeight ||
                snake.Contains(newHead) || obstacles.Contains(newHead))
            {
                EndGame();
                return;
            }

            snake.Insert(0, newHead);

            if (newHead == food)
            {
                sndEat.Play();
                score++;
                foodEatenCount++;

                if (foodEatenCount % 3 == 0)
                    SpawnBonus();

                GenerateFood();
            }
            else if (hasBonus && newHead == bonusFood)
            {
                sndBonus.Play();
                score += 5;
                hasBonus = false;
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
            }

            if (hasBonus && --bonusTimer <= 0)
                hasBonus = false;

            UpdateLabels();
        }

        // ===== FOOD =====
        private void GenerateFood()
        {
            Point p;
            do
            {
                p = new Point(rnd.Next(0, gridWidth), rnd.Next(0, gridHeight));
            }
            while (snake.Contains(p) || obstacles.Contains(p));

            food = p;
        }

        private void SpawnBonus()
        {
            Point p;
            do
            {
                p = new Point(rnd.Next(0, gridWidth), rnd.Next(0, gridHeight));
            }
            while (snake.Contains(p) || obstacles.Contains(p));

            bonusFood = p;
            hasBonus = true;
            bonusTimer = BONUS_LIFETIME;
        }

        // ===== DRAW =====
        private void GamePanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.FromArgb(20, 20, 30));

            Pen gridPen = new Pen(Color.FromArgb(40, 0, 255, 255));
            for (int x = 0; x <= gridWidth; x++)
                g.DrawLine(gridPen, x * cellSize, 0, x * cellSize, gridHeight * cellSize);
            for (int y = 0; y <= gridHeight; y++)
                g.DrawLine(gridPen, 0, y * cellSize, gridWidth * cellSize, y * cellSize);

            g.DrawRectangle(Pens.Cyan, 0, 0, gridWidth * cellSize - 1, gridHeight * cellSize - 1);

            foreach (var o in obstacles)
                g.FillRectangle(Brushes.DarkRed, o.X * cellSize, o.Y * cellSize, cellSize, cellSize);

            g.FillEllipse(Brushes.Red,
                food.X * cellSize + 2, food.Y * cellSize + 2,
                cellSize - 4, cellSize - 4);

            if (hasBonus)
                g.FillEllipse(Brushes.Gold,
                    bonusFood.X * cellSize + 2, bonusFood.Y * cellSize + 2,
                    cellSize - 4, cellSize - 4);

            for (int i = 0; i < snake.Count; i++)
            {
                Brush b = i == 0 ? Brushes.Lime : Brushes.Green;
                g.FillRectangle(b,
                    snake[i].X * cellSize,
                    snake[i].Y * cellSize,
                    cellSize, cellSize);
            }

            if (hasBonus)
            {
                int w = (int)((bonusTimer / (float)BONUS_LIFETIME) * gamePanel.Width);
                g.FillRectangle(Brushes.Gold, 0, gamePanel.Height - 5, w, 5);
            }
        }

        // ===== GAME OVER =====
        private void EndGame()
        {
            sndGameOver.Play();
            tmrGameTimer.Stop();
            gameState = GameState.Start;

            if (score > highScore)
            {
                highScore = score;
                SaveHighScore();
            }

            lblHighScore.Text = $"High Score: {highScore}";
            MessageBox.Show($"Game Over!\nYour Score: {score}",
                "Snake Game", MessageBoxButtons.OK, MessageBoxIcon.Information);

            startPanel.Visible = true;
        }

        // ===== SCORE =====
        private void UpdateLabels()
        {
            lblScore.Text = $"Score: {score}";
            lblHighScore.Text = $"High Score: {highScore}";
        }

        private void LoadHighScore()
        {
            if (File.Exists(scoreFilePath))
                int.TryParse(File.ReadAllText(scoreFilePath), out highScore);
        }

        private void SaveHighScore()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(scoreFilePath));
            File.WriteAllText(scoreFilePath, highScore.ToString());
        }

   
    }
}


