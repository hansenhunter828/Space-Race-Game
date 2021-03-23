using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace Space_Race_Game
{
    public partial class Form1 : Form
    {
        #region Variables
        #region Player Variables
        int shipHeight = 20;
        int shipWidth = 10;
        int shipSpeed = 6;

        int ship1Score = 0;
        int ship2Score = 0;

        int ship1Y = 480; int ship1X = 300;
        int ship2Y = 480; int ship2X = 450;
        int ship1XDefault = 300; int ship1YDefault = 480;
        int ship2XDefault = 480; int ship2YDefault = 480;

        #endregion
        #region Player Movement
        //Player 1
        bool wDown = false;
        bool sDown = false;

        //Player 2
        bool upArrowDown = false;
        bool downArrowDown = false;
        #endregion
        #region Obstacle Variables
        List<int> obstacle1XList = new List<int>();
        List<int> obstacle1YList = new List<int>();
        List<int> obstacle2XList = new List<int>();
        List<int> obstacle2YList = new List<int>();

        int obstacleHeight = 5;
        int obstacleWidth = 20;
        int obstacleSpeed = 6;
        #endregion
        #region Brushes
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        #endregion
        #region Sound Libary
        SoundPlayer gameOver = new SoundPlayer(Properties.Resources.gameOver);
        SoundPlayer point = new SoundPlayer(Properties.Resources.point);
        SoundPlayer rocketSound = new SoundPlayer(Properties.Resources.engineNoise);
        SoundPlayer failSound = new SoundPlayer(Properties.Resources.failNoise);
        
        #endregion
        string gameState = "waiting";

        bool soundplaying = false;
        Random randGen = new Random();

        int spawnCounter = 0;
        int timer = 500;
        #endregion
        public Form1()
        {
            InitializeComponent();
        }

        public void GameInitialize()
        {
            titleLabel.Text = "";
            subTitleLabel.Text = "";
            ship1ScoreLabel.Text = "0";
            ship2ScoreLabel.Text = "0";

            gameTimer.Enabled = true;
            gameState = "running";

            ship1Reset();
            ship2Reset();

            obstacle1XList.Clear();
            obstacle1YList.Clear();
            obstacle2XList.Clear();
            obstacle2YList.Clear();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                //Player 1
                case Keys.W:
                    wDown = true;
                    if(soundplaying == false)
                    {
                        rocketSound.Play();
                        soundplaying = true;
                    }
                    break;
                case Keys.S:
                    sDown = true;
                    if (soundplaying == false)
                    {
                        rocketSound.Play();
                        soundplaying = true;
                    }
                    break;
                //Player 2
                case Keys.Up:
                    upArrowDown = true;
                    if (soundplaying == false)
                    {
                        rocketSound.Play();
                        soundplaying = true;
                    }
                    break;
                case Keys.Down:
                    downArrowDown = true;
                    if (soundplaying == false)
                    {
                        rocketSound.Play();
                        soundplaying = true;
                    }
                    break;
                case Keys.Space:
                    if(gameState == "waiting" || gameState == "gameOver")
                    {
                        GameInitialize();
                    }
                    break;
                case Keys.Escape:
                    if(gameState == "waiting" || gameState == "gameOver")
                    {
                        Application.Exit();
                    }
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                //Player 1
                case Keys.W:
                    wDown = false;
                    soundplaying = false;
                    rocketSound.Stop();
                    break;
                case Keys.S:
                    sDown = false;
                    soundplaying = false;
                    rocketSound.Stop();
                    break;
                //Player 2
                case Keys.Up:
                    upArrowDown = false;
                    soundplaying = false;
                    rocketSound.Stop();
                    break;
                case Keys.Down:
                    downArrowDown = false;
                    soundplaying = false;
                    rocketSound.Stop();
                    break;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            #region Game Scenes
            if (gameState == "waiting")
            {
                titleLabel.Text = "Space Race";
                subTitleLabel.Text = "Press Space To Start Or Escape To Exit";
            }
            else if(gameState == "running")
            {
                titleLabel.Text = "";
                subTitleLabel.Text = "";

                ship1ScoreLabel.Text = $"{ship1Score}";
                ship2ScoreLabel.Text = $"{ship2Score}";

                #region Players
                //Draw Players
                e.Graphics.FillRectangle(whiteBrush, ship1X, ship1Y, shipWidth, shipHeight);
                e.Graphics.FillRectangle(whiteBrush, ship2X, ship2Y, shipWidth, shipHeight);
                #endregion
                #region Obstacles
                for (int i = 0; i < obstacle1XList.Count; i++)
                {
                    e.Graphics.FillRectangle(whiteBrush, obstacle1XList[i], obstacle1YList[i], obstacleWidth, obstacleHeight);
                }
                for (int i = 0; i < obstacle2XList.Count; i++)
                {
                    e.Graphics.FillRectangle(whiteBrush, obstacle2XList[i], obstacle2YList[i], obstacleWidth, obstacleHeight);
                }
                #endregion
                //Timer
                for(int i = 0; i < timer; i++)
                {
                    e.Graphics.FillRectangle(whiteBrush, 373, 0, 10, 500 - timer) ;
                }
            }
            else if(gameState == "gameOver")
            {
                if(ship1Score > ship2Score)
                {
                    titleLabel.Text = "Game Over";
                    subTitleLabel.Text = $"Player 1 Won with {ship1Score} points";
                    subTitleLabel.Text += $"\nPress Space To Replay Or Escape To Exit";
                }

                if(ship2Score > ship1Score)
                {
                    titleLabel.Text = "Game Over";
                    subTitleLabel.Text = $"Player 2 Won with {ship2Score} points";
                    subTitleLabel.Text += $"\nPress Space To Replay Or Escape To Exit";
                }

                if(ship1Score == ship2Score)
                {
                    titleLabel.Text = "Game Over";
                    subTitleLabel.Text = $"Its a Tie!";
                    subTitleLabel.Text += $"\nPress Space To Replay Or Escape To Exit";
                }
            }
            #endregion
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            timer--;
            spawnCounter++;
            #region Player Movement
            //Player 1 Movement
            if (wDown == true && ship1Y > 0)
            {
                ship1Y -= shipSpeed;              
            }
            if (sDown == true && ship1Y < this.Height - shipHeight)
            {
                ship1Y += shipSpeed;  
            }
            //Player 2 Movement
            if (upArrowDown == true && ship2Y > 0)
            {
                ship2Y -= shipSpeed;
            }
            if (downArrowDown == true && ship2Y < this.Height - shipHeight)
            {
                ship2Y += shipSpeed;
            }
            #endregion
            //Spawn Obstacles
            if (spawnCounter == 10)
            {
                //Left Obstacles
                obstacle1YList.Add(randGen.Next(10, this.Height - 50));
                obstacle1XList.Add(10);
                //Right Obstacles
                obstacle2YList.Add(randGen.Next(10, this.Height - 50));
                obstacle2XList.Add(750);

                spawnCounter = 0;
            }
            #region Move Obstacles
            for(int i = 0; i < obstacle1XList.Count; i++)
            {
                obstacle1XList[i] += obstacleSpeed;
            }
            for (int i = 0; i < obstacle2XList.Count; i++)
            {
                obstacle2XList[i] -= obstacleSpeed;
            }
            #endregion
            #region Delete Obstacles
            for(int i = 0; i < obstacle1XList.Count; i++)
            {
                if(obstacle1XList[i] > this.Width)
                {
                    obstacle1XList.RemoveAt(i);
                    obstacle1YList.RemoveAt(i);
                }
            }
            for (int i = 0; i < obstacle2XList.Count; i++)
            {
                if (obstacle2XList[i] < 0)
                {
                    obstacle2XList.RemoveAt(i);
                    obstacle2YList.RemoveAt(i);
                }
            }
            #endregion
            #region Player Obstacle Collisions
            Rectangle ship1Rec = new Rectangle(ship1X, ship1Y, shipWidth, shipHeight);
            Rectangle ship2Rec = new Rectangle(ship2X, ship2Y, shipWidth, shipHeight);

            for(int i = 0; i < obstacle1XList.Count; i++)
            {
                Rectangle obstacle1Rec = new Rectangle(obstacle1XList[i], obstacle1YList[i], obstacleWidth, obstacleHeight);
                if (ship1Rec.IntersectsWith(obstacle1Rec))
                {
                    ship1Reset();
                }
                if (ship2Rec.IntersectsWith(obstacle1Rec))
                {
                    ship2Reset();
                }
            }
            for (int i = 0; i < obstacle2XList.Count; i++)
            {
                Rectangle obstacle2Rec = new Rectangle(obstacle2XList[i], obstacle2YList[i], obstacleWidth, obstacleHeight);
                if (ship1Rec.IntersectsWith(obstacle2Rec))
                {
                    ship1Reset();
                }

                if (ship2Rec.IntersectsWith(obstacle2Rec))
                {
                    ship2Reset();
                }
            }
            #endregion
            #region Scoring
            if(ship1Y <= 0)
            {
                ship1Score++;
                ship1ScoreLabel.Text = $"{ship1Score}";
                point.Play();
                ship1Reset();
            }

            if (ship2Y <= 0)
            {
                ship2Score++;
                ship2ScoreLabel.Text = $"{ship2Score}";
                point.Play();
                ship2Reset();
            }
            #endregion
            if(timer == 0)
            {
                gameOver.Play();
                gameState = "gameOver";
                gameTimer.Enabled = false;
            }

            Refresh();
        }

        public void ship1Reset()
        {
            failSound.Play();
            ship1X = ship1XDefault;
            ship1Y = ship1YDefault;
        }

        public void ship2Reset()
        {
            failSound.Play();
            ship2X = ship2XDefault;
            ship2Y = ship2YDefault;
        }
    }
}
