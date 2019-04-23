using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

//This is a stud

//Carter Leitch Branch

// Malcolm Branch

namespace BrickBreaker
{
    public partial class Form1 : Form
    {
        public static GameScreen currentGame;
        public Form1()
        {
            InitializeComponent();
            Directory.SetCurrentDirectory(Program.FilePath);//Set the program to put files in the created directory
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Start the program centred on the Menu Screen
            MenuScreen ms = new MenuScreen();
            this.Controls.Add(ms);
            //Center the control
            ms.Location = new Point((this.Width - ms.Width) / 2, (this.Height - ms.Height) / 2);
            //ms.Size = new Size(this.Width, this.Height);
        }
    }
}
