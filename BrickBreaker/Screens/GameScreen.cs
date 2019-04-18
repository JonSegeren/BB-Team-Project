﻿/*  Created by: Steven HL
 *  Project: Brick Breaker
 *  Date: Tuesday, April 4th
 */ 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace BrickBreaker
{
    public partial class GameScreen : UserControl
    {
        #region global values

        //player1 button control keys - DO NOT CHANGE
        Boolean leftArrowDown, rightArrowDown, ADown, DDown;
        bool numPlayers = true;

        // Game values
        int lives, p2lives;

        // Paddle and Ball objects
        Paddle paddle, newPaddle;
        Ball ball, ball2;

        // list of all blocks for current level
        List<Block> blocks = new List<Block>();

        // Brushes
        SolidBrush paddleBrush = new SolidBrush(Color.White);
        SolidBrush ballBrush = new SolidBrush(Color.White);
        SolidBrush blockBrush = new SolidBrush(Color.Red);

        // pause menu variables
        bool paused = false; // false - show game screen true - show pause menu
        //asdf
        #endregion

        public GameScreen()
        {
            InitializeComponent();
            if (numPlayers == true)
            {
                OnStart();
            }
            else if(numPlayers == false)
            {
                NickMethod();
            }

            OnStart();
        }

        List<Ball> ballList = new List<Ball>();
        public void OnStart()
        {
            //set life counter
            lives = 3;

            //set all button presses to false.
            leftArrowDown = rightArrowDown = false;

            // setup starting paddle values and create paddle object
            int paddleWidth = 80;
            int paddleHeight = 20;
            int paddleX = ((this.Width / 2) - (paddleWidth / 2));
            int paddleY = (this.Height - paddleHeight);
            int paddleSpeed = 8;
            paddle = new Paddle(paddleX, paddleY, paddleWidth, paddleHeight, paddleSpeed, Color.White);

            // Creates a new ball
            int xSpeed = 6;
            int ySpeed = 6;
            int ballSize = 20;

            // setup starting ball values
            int ballX = ((paddle.x - ballSize) + (paddle.width / 2));
            int ballY = this.Height - paddle.height - paddle.y;

            ballList.Add(ball = new Ball(ballX, ballY, xSpeed, ySpeed, ballSize, 1, 1));


            #region Creates blocks for generic level. Need to replace with code that loads levels.

            blocks.Clear();
            int x = 10;

            while (blocks.Count < 12)
            {
                x += 57;
                Block b1 = new Block(x, 10, 1, Color.White);
                blocks.Add(b1);
            }

            #endregion

            // start the game engine loop
            gameTimer.Enabled = true;
        }

        private void GameScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //player 1 button presses
            switch (e.KeyCode)
            {
                case Keys.A:
                    ADown = true;
                    break;
                case Keys.D:
                    DDown = true;
                    break;
                case Keys.Left:
                    leftArrowDown = true;
                    break;
                case Keys.Right:
                    rightArrowDown = true;
                    break;
                case Keys.Escape:
                    // check if paused
                    if (paused)
                    {
                        // stop game loop
                        paused = false;
                        gameTimer.Enabled = true;
                    }
                    else 
                    {
                        paused = true;
                    }

                    // Carter change screen
                    break;
                default:
                    break;
            }
        }

        private void GameScreen_KeyUp(object sender, KeyEventArgs e)
        {
            //player 1 button releases
            switch (e.KeyCode)
            {
                case Keys.A:
                    ADown = false;
                    break;
                case Keys.D:
                    DDown = false;
                    break;
                case Keys.Left:
                    leftArrowDown = false;
                    break;
                case Keys.Right:
                    rightArrowDown = false;
                    break;
                default:
                    break;
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (numPlayers == false)
            {
                if (paused)
                {
                    gameTimer.Enabled = false;
                }
                else if (!paused)
                {
                    //pauseScreen ps = new pauseScreen();
                }

                // Move the paddle
                if (leftArrowDown && paddle.x > 0)
                {
                    paddle.Move("left");
                }
                if (rightArrowDown && paddle.x < (this.Width/2) - paddle.width)
                {
                    paddle.Move("right");
                }
                if (ADown && newPaddle.x > (this.Width / 2))
                {
                    newPaddle.Move("left");
                }
                if (DDown && newPaddle.x < (this.Width - newPaddle.width))
                {
                    newPaddle.Move("right");
                }

                // Move ball
                foreach (Ball b in ballList)
                {
                    // Move ball
                    b.Move();

                    // Check for collision with top and side walls
                    b.WallCollision(this);

                    // Check for ball hitting bottom of screen
                    if (b.BottomCollision(this, paddle))
                    {
                        if (b.x > this.Width / 2 - b.size)
                        {
                            lives--;
                        }
                        else if (b.x < this.Width / 2 - b.size)
                        {
                            p2lives--;
                        }

                        // Moves the ball back to origin
                        b.x = ((paddle.x - (b.size / 2)) + (paddle.width / 2));
                        b.y = 30;

                        if (lives == 0 || p2lives ==0)
                        {
                            gameTimer.Enabled = false;
                            OnEnd();
                        }
                    }
                    // Check for collision of ball with paddle, (incl. paddle movement)
                    b.PaddleCollision(paddle, leftArrowDown, rightArrowDown);
                    b.PaddleCollision(newPaddle, ADown, DDown);
                }

                // Check if ball has collided with any blocks
                foreach (Ball ba in ballList)
                {
                    foreach (Block b in blocks)
                    {
                        if (ba.BlockCollision(b))
                        {
                            blocks.Remove(b);

                            if (blocks.Count == 0)
                            {
                                gameTimer.Enabled = false;
                                OnEnd();
                            }

                            break;
                        }
                    }
                }
                //redraw the screen
                Refresh();
            }


            if (numPlayers == true)
            {
                if (paused)
                {
                    gameTimer.Enabled = false;
                }
                else if (!paused)
                {
                    //pauseScreen ps = new pauseScreen();
                }

                // Move the paddle
                if (leftArrowDown && paddle.x > 0)
                {
                    paddle.Move("left");
                }
                if (rightArrowDown && paddle.x < (this.Width) - paddle.width)
                {
                    paddle.Move("right");
                }

                // Move ball
                foreach (Ball b in ballList)
                {
                    // Move ball
                    b.Move();

                    // Check for collision with top and side walls
                    b.WallCollision(this);

                    // Check for ball hitting bottom of screen
                    if (b.BottomCollision(this, paddle))
                    {
                        lives--;

                        // Moves the ball back to origin
                        b.x = ((paddle.x - (b.size / 2)) + (paddle.width / 2));
                        b.y = 30;

                        if (lives == 0)
                        {
                            gameTimer.Enabled = false;
                            OnEnd();
                        }
                    }
                    // Check for collision of ball with paddle, (incl. paddle movement)
                    b.PaddleCollision(paddle, leftArrowDown, rightArrowDown);
               
                }

                // Check if ball has collided with any blocks
                foreach (Ball ba in ballList)
                {
                    foreach (Block b in blocks)
                    {
                        if (ba.BlockCollision(b))
                        {
                            blocks.Remove(b);

                            if (blocks.Count == 0)
                            {
                                gameTimer.Enabled = false;
                                OnEnd();
                            }

                            break;
                        }
                    }
                }
                //redraw the screen
                Refresh();
            }
        }

        public void OnEnd()
        {
            // Goes to the game over screen
            Form form = this.FindForm();
            MenuScreen ps = new MenuScreen();
            
            ps.Location = new Point((form.Width - ps.Width) / 2, (form.Height - ps.Height) / 2);

            form.Controls.Add(ps);
            form.Controls.Remove(this);
        }

        public void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            // Draws paddle
            if (numPlayers == true)
            {
                paddleBrush.Color = paddle.colour;
                e.Graphics.FillRectangle(paddleBrush, paddle.x, paddle.y, paddle.width, paddle.height);
            }
            
            if (numPlayers == false)
            {
                paddleBrush.Color = Color.Firebrick;
                e.Graphics.FillRectangle(paddleBrush, paddle.x, paddle.y, paddle.width, paddle.height);
                paddleBrush.Color = newPaddle.colour;
                e.Graphics.FillRectangle(paddleBrush, newPaddle.x, newPaddle.y, newPaddle.width, newPaddle.height);
            }
            // Draws blocks
            foreach (Block b in blocks)
            {
                e.Graphics.FillRectangle(blockBrush, b.x, b.y, b.width, b.height);
            }

            // Draws ball
            foreach(Ball b in ballList)
            {
                e.Graphics.FillEllipse(ballBrush, Convert.ToSingle(b.x), Convert.ToInt32(b.y), b.size, b.size);
            }

        }

        public void NickMethod()
        {
            //set life counter
            lives = p2lives = 3;

            //set all button presses to false.
            leftArrowDown = rightArrowDown = ADown = DDown = false;

            // setup starting paddle values and create paddle object
            int paddleWidth = 80;
            int paddleHeight = 20;
            int paddleX = ((this.Width / 2) - this.Width/2/2 - paddleWidth);
            int newPaddleX = ((this.Width / 2) - (paddleWidth / 2)) + ((this.Width / 2) / 2);
            int paddleY = (this.Height - paddleHeight);
            int paddleSpeed = 8;
            paddle = new Paddle(paddleX, paddleY, paddleWidth, paddleHeight, paddleSpeed, Color.Firebrick);
            newPaddle = new Paddle(newPaddleX, paddleY, paddleWidth, paddleHeight, paddleSpeed, Color.RoyalBlue);

            // setup starting ball values
            int ballX = (this.Width / 2 - 10) - ((this.Width / 2) / 2);
            int ballX2 = (this.Width / 2 - 10) + ((this.Width / 2) / 2);
            int ballY = this.Height - paddle.height - 80;

            // Creates a new ball
            int xSpeed = 6;
            int ySpeed = 6;
            int ballSize = 20;
            ball2 = new Ball(ballX2, ballY, xSpeed, ySpeed, ballSize, 0, 0);
            ball = new Ball(ballX, ballY, xSpeed, ySpeed, ballSize, 0, 0);

            #region Creates blocks for generic level. Need to replace with code that loads levels.

            blocks.Clear();
            int x = 10;

            while (blocks.Count < 12)
            {
                x += 57;
                Block b1 = new Block(x, 10, 1, Color.White);
                blocks.Add(b1);
            }

            #endregion

            // start the game engine loop
            gameTimer.Enabled = true;

        }
    }
}
