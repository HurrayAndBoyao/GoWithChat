using System.Drawing;

namespace GoWithChat
{
    partial class Board
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            int i, j;
            for (i = 0; i < 19; i++)
            {
                for (j = 0; j < 19; j++)
                {
                    this.pictureBox[i, j] = new System.Windows.Forms.PictureBox();
                    ((System.ComponentModel.ISupportInitialize)(this.pictureBox[i, j])).BeginInit();
                }
            }
            this.SuspendLayout();
            // 
            // pictureBox[i,j]
            // 
            for (i = 0; i < 19; i++)
            {
                for (j = 0; j < 19; j++)
                {
                    this.pictureBox[i, j].Location = new System.Drawing.Point(22 + i * 23, 22 + j * 23);
                    this.pictureBox[i, j].Name = "pictureBox[" + (i + 1) + "][" + (j + 1) + "]";
                    this.pictureBox[i, j].Size = new System.Drawing.Size(19, 19);
                    this.pictureBox[i, j].TabIndex = 0;
                    this.pictureBox[i, j].BackColor = Color.Transparent;
                    this.pictureBox[i, j].TabStop = false;
                }
            }
            // 
            // Board
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 467);
            for (i = 0; i < 19; i++)
            {
                for (j = 0; j < 19; j++)
                {
                    this.Controls.Add(this.pictureBox[i, j]);
                }
            }
            this.Name = "Board";
            this.Text = "GoWithChat";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PaintHandler);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MouseUpHandler);
            for (i = 0; i < 19; i++)
            {
                for (j = 0; j < 19; j++)
                {
                    ((System.ComponentModel.ISupportInitialize)(this.pictureBox[i, j])).EndInit();
                    this.pictureBox[i, j].Hide();
                }
            }
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox[,] pictureBox = new System.Windows.Forms.PictureBox[19, 19];

    }
}