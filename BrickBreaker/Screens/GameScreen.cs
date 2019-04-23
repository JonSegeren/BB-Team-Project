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

        // Paddle and Ball objects
        Paddle paddle;
        Ball ball;

        // list of all blocks for current level
        List<Block> blocks = new List<Block>();

        // Brushes
        SolidBrush paddleBrush = new SolidBrush(Color.White);
        SolidBrush ballBrush = new SolidBrush(Color.White);
        SolidBrush blockBrush = new SolidBrush(Color.Red);

        // Lives
        public int player1Lives = 3;
        public int? player2Lives = null;
        #endregion

        // Creates a new ball
        int xSpeed = 6;
        int ySpeed = 6;
        int ballSize = 20;
        public GameScreen(bool multiplayer = false)
        {
            InitializeComponent();
            OnStart();
            if (multiplayer)
                player2Lives = 3;
        }
        // angle change buttons
        int angleposition = 3;
        bool start = false;

        bool Akeydown = false;
        bool Dkeydown = false;
        List<Ball> ballList = new List<Ball>();
        private int lives;

        public void OnStart()
        {
            //set all button presses to false.
            leftArrowDown = rightArrowDown = false;

            // setup starting paddle values and create paddle object
            int paddleWidth = 80;
            int paddleHeight = 20;
            int paddleX = ((this.Width / 2) - (paddleWidth / 2));
            int paddleY = (this.Height - paddleHeight);
            int paddleSpeed = 8;
            paddle = new Paddle(paddleX, paddleY, paddleWidth, paddleHeight, paddleSpeed, Color.White);

            // setup starting ball values
            int ballX = ((paddle.x - ballSize) + (paddle.width / 2));
            int ballY =  paddle.y - 21;
            ballList.Clear();
            ballList.Add(new Ball(ballX, ballY, xSpeed, ySpeed, ballSize, 1, -1));

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
                case Keys.Left:
                    leftArrowDown = true;
                    break;
                case Keys.Right:
                    rightArrowDown = true;
                    break;
                case Keys.Space:
                    start = true;
                    break;
                case Keys.Escape:
                    break;
                default:
                    break;
            }

            if (!start)
            {
                switch (e.KeyCode)
                {
                    case Keys.A:
                        if(angleposition > 1)
                        {
                            angleposition--;
                        }
                        break;
                    case Keys.D:
                        if (angleposition < 6)
                        {
                            angleposition++;
                        }
                        break;
                }
            }
        }

        private void anglechange()
        {
            // For the first ball, it works fine. For subsequent it breaks
            switch (angleposition)
            {
                case 1:
                    ballList[0].Xangle = 1;
                    ballList[0].Yangle = -0.5;
                    break;
                case 2:
                    ballList[0].Xangle = 1;
                    ballList[0].Yangle = -1;
                    break;
                case 3:
                    ballList[0].Xangle = 0.5;
                    ballList[0].Yangle = -1;
                    break;
                case 4:
                    ballList[0].Xangle = -1;
                    ballList[0].Yangle = -0.5;
                    break;
                case 5:
                    ballList[0].Xangle = -1;
                    ballList[0].Yangle = -1;
                    break;
                case 6:
                    ballList[0].Xangle = -0.5;
                    ballList[0].Yangle = -1;
                    break;
            }
        }

        private void GameScreen_KeyUp(object sender, KeyEventArgs e)
        {
            //player 1 button releases
            switch (e.KeyCode)
            {
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
            // Move the paddle
            if (leftArrowDown && paddle.x > 0)
            {
                paddle.Move("left");
            }
            if (rightArrowDown && paddle.x < (this.Width - paddle.width))
            {
                paddle.Move("right");
            }

            foreach(Ball b in ballList)
            {
                ballList[0].x = ((paddle.x - ballSize) + (paddle.width / 2));
                ballList[0].y = paddle.y - 21;
            }

            if (start)
            // Move ball
            foreach (Ball b in ballList)
            {
                anglechange();
                // Move ball
                b.Move();

                // Check for collision with top and side walls
                b.WallCollision(this);

                // Check for ball hitting bottom of screen
                if (b.BottomCollision(this, paddle))
                {
                    player1Lives--;

                    // Moves the ball back to origin
                    b.x = ((paddle.x - (ball.size / 2)) + (paddle.width / 2));
                    b.y = 30;

                    if (player1Lives == 0)
                    {
                        start = false;

                        ballList[0].x = ((paddle.x - ballSize) + (paddle.width / 2));
                        ballList[0].y = paddle.y - 40;
                        ballList[0].Yangle *= -1;
                        player1Lives--;


                        if (player2Lives == 0)
                        {
                            gameTimer.Enabled = false;
                            OnEnd();
                        }
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
            paddleBrush.Color = paddle.colour;
            e.Graphics.FillRectangle(paddleBrush, paddle.x, paddle.y, paddle.width, paddle.height);
            //paddleBrush.Color = newPaddle.colour;
            //e.Graphics.FillRectangle(paddleBrush, newPaddle.x, newPaddle.y, newPaddle.width, newPaddle.height);

            // Draws blocks
            foreach (Block b in blocks)
            {
                e.Graphics.FillRectangle(blockBrush, b.x, b.y, b.width, b.height);
            }

            // Draws ball
            foreach (Ball b in ballList)
            {
                e.Graphics.FillEllipse(ballBrush, Convert.ToSingle(b.x), Convert.ToInt32(b.y), b.size, b.size);
            }
        }

        public void NickMethod()
        {

            //set all button presses to false.
            leftArrowDown = rightArrowDown = ADown = DDown = false;

            // setup starting paddle values and create paddle object
            int paddleWidth = 80;
            int paddleHeight = 20;
            int paddleX = ((this.Width / 2) - (paddleWidth / 2)) - ((this.Width / 2) / 2);
            int newPaddleX = ((this.Width / 2) - (paddleWidth / 2)) + ((this.Width / 2) / 2);
            int paddleY = (this.Height - paddleHeight) - 60;
            int paddleSpeed = 8;
            paddle = new Paddle(paddleX, paddleY, paddleWidth, paddleHeight, paddleSpeed, Color.Firebrick);
            //newPaddle = new Paddle(newPaddleX, paddleY, paddleWidth, paddleHeight, paddleSpeed, Color.RoyalBlue);

            // setup starting ball values
            int ballX = (this.Width / 2 - 10) - ((this.Width / 2) / 2);
            int ballY = this.Height - paddle.height - 80;

            /// BallList[0] is P1
            /// BallList[1] is P2
            ballList.Clear();
            ballList.Add(ball = new Ball(ballX, ballY, 6, 6, 20, 1, 1));
            ballList.Add(ball = new Ball(ballX, this.Height - ballY, 6, 6, 20, 1, 1));
            // Creates a new ball

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