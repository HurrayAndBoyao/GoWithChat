using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GoWithChat
{
    public partial class Note : Form
    {
        public Note(String msg)
        {
            InitializeComponent();
            tb_Note.Text = msg;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void Note_Load(object sender, EventArgs e)
        {

        }

    }
}
