using MayinTarlasi.DataBase;
using MayinTarlasi.DataBase.Contract;
using MayinTarlasi.Models;
using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace MayinTarlasi
{
    public partial class FormView : Form
    {
        int mineRow = 10, mineCol = 10;
        Button[,] buttons;
        int score = 0;
        bool gameOver = false;
        Player player;
        IDataBase accessDataBase;
        int[] mines;
        public FormView()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            dateTimeText.Text = DateTime.Now.ToString();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            flowPanel.Width = 470;
            flowPanel.Height = 470;
            progressBar.Maximum = 0;
            accessDataBase = new AccessDataBase();
            buttons = new Button[mineRow, mineCol];
            player = new Player();
            DataGridLoad();
            timer1.Interval = 1000;
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Start();
        }
        private void GameStarted(int mine)
        {
            int forCount = 0;
            Clear();
            mineText.Text = "Mayın :" + mine.ToString();
            mines = new int[mine];
            Random r = new Random();
            for (int i = 0; i < mines.Length; i++)
            {
                int randomValue = r.Next(0, 100);
                if (mines.Contains(randomValue) is true)
                    i--;
                else
                    mines[i] = randomValue;
            }
            for (int i = 0; i < mineRow; i++)
            {
                for (int j = 0; j < mineCol; j++)
                {
                    var button = ButtonGenerator(40,40,forCount);
                    buttons[i, j] = button;
                    flowPanel.Controls.Add(button);
                    forCount++;
                }
            }
        }

        Button ButtonGenerator(int width,int height,int count)
        {
            Button button = new Button
            {
                Width = width,
                Height = height,
                BackColor = Color.Blue,
                Margin = new Padding(5, 0, 0, 0),
                Tag = mines.Contains(count)
            };
            button.Click += button_click;
            return button;
        }

        void Clear()
        {
            gameOver = false;
            flowPanel.Controls.Clear();
            player.Score = 0;
            progressBar.Value = 0;
        }

        private void button_click(object sender, EventArgs e)
        {
            int count = 0;
            if (!gameOver)
            {
                Button btnClick = sender as Button;
                int row = -1;
                int col = -1;
                for (int i = 0; i < mineRow; i++)
                {
                    for (int j = 0; j < mineCol; j++)
                    {
                        if (buttons[i, j] == btnClick)
                        {
                            row = i;
                            col = j;
                            break;
                        }
                    }
                    if (row != -1 && col != -1)
                        break;
                }

                if ((bool)btnClick.Tag is true)
                {
                    foreach (Control item in flowPanel.Controls)
                    {
                        var btn = item as Button;
                        if ((bool)btn.Tag is true)
                            btn.Image = Image.FromFile(@"..\..\Image\mine.jpg");
                    }
                    GameOver();
                }
                else
                {
                    btnClick.BackColor = Color.Green;
                    player.Score += score;
                    progressBar.Value += 1;
                    scoreText.Text = "Puan : " + player.Score;
                    GameComplate();
                }
                for (int i = row - 1; i <= row + 1; i++)
                {
                    for (int j = col - 1; j <= col + 1; j++)
                    {
                        if (i >= 0 && i < 10 && j >= 0 && j < 10 
                            && !(i == row && j == col))
                        {
                            if (buttons[i, j].Tag is true)
                                count++;
                        }
                    }
                }
                buttons[row, col].Text = count.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            player.Name = txtAd.Text;
            if (string.IsNullOrEmpty(player.Name))
            {
                MessageBox.Show("Adınızı giriniz!");
                return;
            }
            switch (true)
            {
                case var _ when easy.Checked:
                    GameStarted(10);
                    progressBar.Maximum = 80;
                    score = 1;
                    break;
                case var _ when mid.Checked:
                    GameStarted(25);
                    progressBar.Maximum = 75;
                    score = 3;
                    break;
                case var _ when hard.Checked:
                    GameStarted(40);
                    progressBar.Maximum = 60;
                    score = 5;
                    break;
            }

        }
        void GameComplate()
        {
            if (progressBar.Value == progressBar.Maximum)
            {
                gameOver = true;
                MessageBox.Show(player.Name+" Win : " + player.Score.ToString());
                accessDataBase.DataInsert(player);
                DataGridLoad();
            }
        }
        void GameOver()
        {
            MessageBox.Show("Game Over \nScore : " + player.Score.ToString());
            gameOver = true;
            accessDataBase.DataInsert(player);
            DataGridLoad();
        }

        void DataGridLoad()
        {
            dataGridView1.DataSource = accessDataBase.DataLoad();
        }



    }
}

