using System;

namespace DAS.CodeGenerator
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnGenModel = new System.Windows.Forms.Button();
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.btnClearOutput = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.label8 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtDisplayname = new System.Windows.Forms.TextBox();
            this.cbIsModel = new System.Windows.Forms.CheckBox();
            this.cbIsService = new System.Windows.Forms.CheckBox();
            this.cbIsController = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtGroup = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.rtbInput = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtRepos = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnCopySqlServerScript = new System.Windows.Forms.Button();
            this.btnCopyOracleScript = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnGenModel
            // 
            this.btnGenModel.Location = new System.Drawing.Point(504, 299);
            this.btnGenModel.Name = "btnGenModel";
            this.btnGenModel.Size = new System.Drawing.Size(147, 34);
            this.btnGenModel.TabIndex = 3;
            this.btnGenModel.Text = "Generate code";
            this.btnGenModel.UseVisualStyleBackColor = true;
            this.btnGenModel.Click += new System.EventHandler(this.btnGenModel_Click);
            // 
            // rtbOutput
            // 
            this.rtbOutput.Location = new System.Drawing.Point(504, 348);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.ReadOnly = true;
            this.rtbOutput.Size = new System.Drawing.Size(933, 379);
            this.rtbOutput.TabIndex = 10;
            this.rtbOutput.Text = "";
            // 
            // btnClearOutput
            // 
            this.btnClearOutput.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnClearOutput.Location = new System.Drawing.Point(1336, 299);
            this.btnClearOutput.Name = "btnClearOutput";
            this.btnClearOutput.Size = new System.Drawing.Size(101, 34);
            this.btnClearOutput.TabIndex = 9;
            this.btnClearOutput.Text = "Xóa log";
            this.btnClearOutput.UseVisualStyleBackColor = true;
            this.btnClearOutput.Click += new System.EventHandler(this.btnClearOutput_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1451, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(488, 41);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(111, 20);
            this.label8.TabIndex = 2;
            this.label8.Text = "Thư mục dự án:";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(1350, 38);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(87, 27);
            this.btnBrowse.TabIndex = 13;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Location = new System.Drawing.Point(605, 38);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(739, 27);
            this.txtOutput.TabIndex = 14;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(605, 79);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(381, 27);
            this.txtName.TabIndex = 0;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(526, 79);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(73, 20);
            this.label9.TabIndex = 2;
            this.label9.Text = "Tên bảng:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(511, 125);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(88, 20);
            this.label11.TabIndex = 2;
            this.label11.Text = "Tên hiển thị:";
            // 
            // txtDisplayname
            // 
            this.txtDisplayname.Location = new System.Drawing.Point(605, 125);
            this.txtDisplayname.Name = "txtDisplayname";
            this.txtDisplayname.Size = new System.Drawing.Size(381, 27);
            this.txtDisplayname.TabIndex = 1;
            // 
            // cbIsModel
            // 
            this.cbIsModel.AutoSize = true;
            this.cbIsModel.Checked = true;
            this.cbIsModel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIsModel.Location = new System.Drawing.Point(1210, 84);
            this.cbIsModel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbIsModel.Name = "cbIsModel";
            this.cbIsModel.Size = new System.Drawing.Size(163, 24);
            this.cbIsModel.TabIndex = 16;
            this.cbIsModel.Text = "Model + Repository";
            this.cbIsModel.UseVisualStyleBackColor = true;
            // 
            // cbIsService
            // 
            this.cbIsService.AutoSize = true;
            this.cbIsService.Checked = true;
            this.cbIsService.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIsService.Location = new System.Drawing.Point(1210, 118);
            this.cbIsService.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbIsService.Name = "cbIsService";
            this.cbIsService.Size = new System.Drawing.Size(78, 24);
            this.cbIsService.TabIndex = 16;
            this.cbIsService.Text = "Service";
            this.cbIsService.UseVisualStyleBackColor = true;
            // 
            // cbIsController
            // 
            this.cbIsController.AutoSize = true;
            this.cbIsController.Checked = true;
            this.cbIsController.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIsController.Location = new System.Drawing.Point(1210, 152);
            this.cbIsController.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbIsController.Name = "cbIsController";
            this.cbIsController.Size = new System.Drawing.Size(176, 24);
            this.cbIsController.TabIndex = 16;
            this.cbIsController.Text = "Controller + View + Js";
            this.cbIsController.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(1109, 79);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(68, 20);
            this.label13.TabIndex = 17;
            this.label13.Text = "Tùy chọn";
            // 
            // txtGroup
            // 
            this.txtGroup.Location = new System.Drawing.Point(606, 175);
            this.txtGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtGroup.Name = "txtGroup";
            this.txtGroup.Size = new System.Drawing.Size(380, 27);
            this.txtGroup.TabIndex = 18;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(546, 175);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(53, 20);
            this.label14.TabIndex = 2;
            this.label14.Text = "Nhóm:";
            // 
            // rtbInput
            // 
            this.rtbInput.AcceptsTab = true;
            this.rtbInput.Location = new System.Drawing.Point(13, 125);
            this.rtbInput.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rtbInput.Name = "rtbInput";
            this.rtbInput.Size = new System.Drawing.Size(466, 603);
            this.rtbInput.TabIndex = 2;
            this.rtbInput.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 20);
            this.label1.TabIndex = 20;
            this.label1.Text = "Thông tin bảng dữ liệu";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label2.Location = new System.Drawing.Point(12, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(391, 20);
            this.label2.TabIndex = 21;
            this.label2.Text = "(Tên cột       Tiêu đề cột     Kiểu dữ liệu      Cho phép rỗng)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(522, 223);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Tên Repos:";
            // 
            // txtRepos
            // 
            this.txtRepos.Location = new System.Drawing.Point(609, 223);
            this.txtRepos.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtRepos.Name = "txtRepos";
            this.txtRepos.Size = new System.Drawing.Size(377, 27);
            this.txtRepos.TabIndex = 18;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(155, 20);
            this.label4.TabIndex = 22;
            this.label4.Text = "Copy Script gen bảng:";
            // 
            // btnCopySqlServerScript
            // 
            this.btnCopySqlServerScript.Location = new System.Drawing.Point(13, 47);
            this.btnCopySqlServerScript.Name = "btnCopySqlServerScript";
            this.btnCopySqlServerScript.Size = new System.Drawing.Size(96, 29);
            this.btnCopySqlServerScript.TabIndex = 23;
            this.btnCopySqlServerScript.Text = "SQL Server";
            this.btnCopySqlServerScript.UseVisualStyleBackColor = true;
            this.btnCopySqlServerScript.Click += new System.EventHandler(this.btnCopySqlServerScript_Click);
            // 
            // btnCopyOracleScript
            // 
            this.btnCopyOracleScript.Location = new System.Drawing.Point(115, 47);
            this.btnCopyOracleScript.Name = "btnCopyOracleScript";
            this.btnCopyOracleScript.Size = new System.Drawing.Size(68, 29);
            this.btnCopyOracleScript.TabIndex = 23;
            this.btnCopyOracleScript.Text = "Oracle";
            this.btnCopyOracleScript.UseVisualStyleBackColor = true;
            this.btnCopyOracleScript.Click += new System.EventHandler(this.btnCopyOracleScript_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1451, 739);
            this.Controls.Add(this.btnCopyOracleScript);
            this.Controls.Add(this.btnCopySqlServerScript);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rtbInput);
            this.Controls.Add(this.txtRepos);
            this.Controls.Add(this.txtGroup);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.cbIsController);
            this.Controls.Add(this.cbIsService);
            this.Controls.Add(this.cbIsModel);
            this.Controls.Add(this.txtDisplayname);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnClearOutput);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rtbOutput);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnGenModel);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DAS.CodeGenerator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnGenModel;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.Windows.Forms.Button btnClearOutput;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtDisplayname;
        private System.Windows.Forms.CheckBox cbIsModel;
        private System.Windows.Forms.CheckBox cbIsService;
        private System.Windows.Forms.CheckBox cbIsController;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtGroup;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.RichTextBox rtbInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtRepos;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCopySqlServerScript;
        private System.Windows.Forms.Button btnCopyOracleScript;
    }
}

