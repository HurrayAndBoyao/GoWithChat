namespace GoWithChat
{
    partial class Landed
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lb_username = new System.Windows.Forms.Label();
            this.lb_passwd = new System.Windows.Forms.Label();
            this.tb_username = new System.Windows.Forms.TextBox();
            this.tb_passwd = new System.Windows.Forms.TextBox();
            this.bt_server = new System.Windows.Forms.Button();
            this.bt_landed = new System.Windows.Forms.Button();
            this.bt_reg = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lb_username
            // 
            this.lb_username.AutoSize = true;
            this.lb_username.Location = new System.Drawing.Point(61, 78);
            this.lb_username.Name = "lb_username";
            this.lb_username.Size = new System.Drawing.Size(41, 12);
            this.lb_username.TabIndex = 0;
            this.lb_username.Text = "用户名";
            // 
            // lb_passwd
            // 
            this.lb_passwd.AutoSize = true;
            this.lb_passwd.Location = new System.Drawing.Point(61, 151);
            this.lb_passwd.Name = "lb_passwd";
            this.lb_passwd.Size = new System.Drawing.Size(29, 12);
            this.lb_passwd.TabIndex = 1;
            this.lb_passwd.Text = "密码";
            // 
            // tb_username
            // 
            this.tb_username.Location = new System.Drawing.Point(167, 78);
            this.tb_username.Name = "tb_username";
            this.tb_username.Size = new System.Drawing.Size(142, 21);
            this.tb_username.TabIndex = 2;
            // 
            // tb_passwd
            // 
            this.tb_passwd.Location = new System.Drawing.Point(167, 151);
            this.tb_passwd.Name = "tb_passwd";
            this.tb_passwd.Size = new System.Drawing.Size(142, 21);
            this.tb_passwd.TabIndex = 3;
            // 
            // bt_server
            // 
            this.bt_server.Location = new System.Drawing.Point(63, 248);
            this.bt_server.Name = "bt_server";
            this.bt_server.Size = new System.Drawing.Size(75, 23);
            this.bt_server.TabIndex = 4;
            this.bt_server.Text = "服务器";
            this.bt_server.UseVisualStyleBackColor = true;
            // 
            // bt_landed
            // 
            this.bt_landed.Location = new System.Drawing.Point(274, 248);
            this.bt_landed.Name = "bt_landed";
            this.bt_landed.Size = new System.Drawing.Size(75, 23);
            this.bt_landed.TabIndex = 5;
            this.bt_landed.Text = "登录";
            this.bt_landed.UseVisualStyleBackColor = true;
            this.bt_landed.Click += new System.EventHandler(this.bt_landed_Click);
            // 
            // bt_reg
            // 
            this.bt_reg.Location = new System.Drawing.Point(169, 248);
            this.bt_reg.Name = "bt_reg";
            this.bt_reg.Size = new System.Drawing.Size(75, 23);
            this.bt_reg.TabIndex = 6;
            this.bt_reg.Text = "注册";
            this.bt_reg.UseVisualStyleBackColor = true;
            // 
            // Landed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 302);
            this.Controls.Add(this.bt_reg);
            this.Controls.Add(this.bt_landed);
            this.Controls.Add(this.bt_server);
            this.Controls.Add(this.tb_passwd);
            this.Controls.Add(this.tb_username);
            this.Controls.Add(this.lb_passwd);
            this.Controls.Add(this.lb_username);
            this.Name = "Landed";
            this.Text = "登录界面";
            this.Load += new System.EventHandler(this.Landed_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_username;
        private System.Windows.Forms.Label lb_passwd;
        private System.Windows.Forms.TextBox tb_username;
        private System.Windows.Forms.TextBox tb_passwd;
        private System.Windows.Forms.Button bt_server;
        private System.Windows.Forms.Button bt_landed;
        private System.Windows.Forms.Button bt_reg;
    }
}

