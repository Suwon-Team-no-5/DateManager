namespace DateManager
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox1 = new PictureBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            panel1 = new GroupBox(); // Panel -> GroupBox 변경
            panel2 = new GroupBox();
            button14 = new Button();
            button6 = new Button();
            button5 = new Button();
            button4 = new Button();
            button3 = new Button();
            button2 = new Button();
            trackBar1 = new TrackBar();
            button1 = new Button();
            panel3 = new GroupBox();
            listBox1 = new ListBox();
            panel4 = new GroupBox();
            button9 = new Button();
            button8 = new Button();
            button7 = new Button();
            panel5 = new GroupBox();
            label7 = new Label();
            button11 = new Button();
            button13 = new Button();
            button12 = new Button();
            button10 = new Button();
            panel7 = new GroupBox();
            checkBox3 = new CheckBox();
            checkBox2 = new CheckBox();
            checkBox1 = new CheckBox();
            panel6 = new GroupBox();
            richTextBox1 = new RichTextBox();
            label6 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            panel5.SuspendLayout();
            panel7.SuspendLayout();
            panel6.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.Location = new Point(6, 50);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(645, 341);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label1.ForeColor = Color.White;
            label1.Location = new Point(488, 356);
            label1.Name = "label1";
            label1.Size = new Size(116, 21);
            label1.TabIndex = 1;
            label1.Text = "Throttle: +0.0";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("맑은 고딕", 9F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label2.ForeColor = Color.White;
            label2.Location = new Point(13, 23);
            label2.Name = "label2";
            label2.Size = new Size(99, 15);
            label2.TabIndex = 2;
            label2.Text = "Frame Index 0/0";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("맑은 고딕", 9F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label3.ForeColor = Color.White;
            label3.Location = new Point(205, 23);
            label3.Name = "label3";
            label3.Size = new Size(71, 15);
            label3.TabIndex = 3;
            label3.Text = "Angle: +0.0";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("맑은 고딕", 9F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label4.ForeColor = Color.White;
            label4.Location = new Point(355, 23);
            label4.Name = "label4";
            label4.Size = new Size(84, 15);
            label4.TabIndex = 4;
            label4.Text = "Throttle: +0.0"; // 오타 수정 (Thorttle -> Throttle)
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("맑은 고딕", 9F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label5.ForeColor = Color.White;
            label5.Location = new Point(505, 23);
            label5.Name = "label5";
            label5.Size = new Size(70, 15);
            label5.TabIndex = 5;
            label5.Text = "Timestamp";
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(label1);
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(label3);
            panel1.ForeColor = Color.White;
            panel1.Location = new Point(24, 53);
            panel1.Name = "panel1";
            panel1.Size = new Size(657, 397);
            panel1.TabIndex = 6;
            panel1.TabStop = false;
            panel1.Text = "CAM IMAGE VIEW"; // 그룹박스 제목 추가
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel2.Controls.Add(button14);
            panel2.Controls.Add(button6);
            panel2.Controls.Add(button5);
            panel2.Controls.Add(button4);
            panel2.Controls.Add(button3);
            panel2.Controls.Add(button2);
            panel2.Controls.Add(trackBar1);
            panel2.Controls.Add(button1);
            panel2.ForeColor = Color.White;
            panel2.Location = new Point(24, 456);
            panel2.Name = "panel2";
            panel2.Size = new Size(657, 125);
            panel2.TabIndex = 7;
            panel2.TabStop = false;
            panel2.Text = "DATAFRAME NAVIGATION";
            // 
            // button14
            // 
            button14.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button14.BackColor = Color.Silver;
            button14.ForeColor = Color.Black;
            button14.Location = new Point(355, 82);
            button14.Name = "button14";
            button14.Size = new Size(94, 29);
            button14.TabIndex = 7;
            button14.Text = "1.0";
            button14.UseVisualStyleBackColor = false;
            // 
            // button6
            // 
            button6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button6.BackColor = Color.Silver;
            button6.ForeColor = Color.Black;
            button6.Location = new Point(557, 82);
            button6.Name = "button6";
            button6.Size = new Size(94, 29);
            button6.TabIndex = 6;
            button6.Text = ">>>";
            button6.UseVisualStyleBackColor = false;
            // 
            // button5
            // 
            button5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button5.BackColor = Color.Silver;
            button5.ForeColor = Color.Black;
            button5.Location = new Point(457, 82);
            button5.Name = "button5";
            button5.Size = new Size(94, 29);
            button5.TabIndex = 5;
            button5.Text = "<<<";
            button5.UseVisualStyleBackColor = false;
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button4.BackColor = Color.Silver;
            button4.ForeColor = Color.Black;
            button4.Location = new Point(557, 47);
            button4.Name = "button4";
            button4.Size = new Size(94, 29);
            button4.TabIndex = 4;
            button4.Text = ">";
            button4.UseVisualStyleBackColor = false;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button3.BackColor = Color.Silver;
            button3.ForeColor = Color.Black;
            button3.Location = new Point(457, 47);
            button3.Name = "button3";
            button3.Size = new Size(94, 29);
            button3.TabIndex = 3;
            button3.Text = "<";
            button3.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.BackColor = Color.Silver;
            button2.ForeColor = Color.Black;
            button2.Location = new Point(457, 12);
            button2.Name = "button2";
            button2.Size = new Size(94, 29);
            button2.TabIndex = 2;
            button2.Text = "Play";
            button2.UseVisualStyleBackColor = false;
            // 
            // trackBar1
            // 
            trackBar1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBar1.Location = new Point(3, 23);
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(446, 45);
            trackBar1.TabIndex = 1;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.BackColor = Color.Silver;
            button1.ForeColor = Color.Black;
            button1.Location = new Point(557, 12);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 0;
            button1.Text = "Stop";
            button1.UseVisualStyleBackColor = false;
            // 
            // panel3
            // 
            panel3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            panel3.Controls.Add(listBox1);
            panel3.ForeColor = Color.White;
            panel3.Location = new Point(687, 53);
            panel3.Name = "panel3";
            panel3.Size = new Size(147, 236);
            panel3.TabIndex = 8;
            panel3.TabStop = false;
            panel3.Text = "FRAME LIST";
            // 
            // listBox1
            // 
            listBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(6, 22);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(135, 199);
            listBox1.TabIndex = 0;
            // 
            // panel4
            // 
            panel4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panel4.Controls.Add(button9);
            panel4.Controls.Add(button8);
            panel4.Controls.Add(button7);
            panel4.ForeColor = Color.White;
            panel4.Location = new Point(840, 53);
            panel4.Name = "panel4";
            panel4.Size = new Size(155, 236);
            panel4.TabIndex = 9;
            panel4.TabStop = false;
            panel4.Text = "SYSTEM OPERATIONS";
            // 
            // button9
            // 
            button9.BackColor = Color.MediumSeaGreen;
            button9.Font = new Font("맑은 고딕", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 129);
            button9.ForeColor = Color.Yellow;
            button9.Location = new Point(6, 116);
            button9.Name = "button9";
            button9.Size = new Size(143, 111);
            button9.TabIndex = 2;
            button9.Text = "Start AI Model\r\nTraining"; // 오타 수정 (Traning -> Training) 및 줄바꿈
            button9.UseVisualStyleBackColor = false;
            // 
            // button8
            // 
            button8.BackColor = Color.CornflowerBlue;
            button8.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            button8.ForeColor = Color.White;
            button8.Location = new Point(6, 62);
            button8.Name = "button8";
            button8.Size = new Size(143, 48);
            button8.TabIndex = 1;
            button8.Text = "Load tub";
            button8.UseVisualStyleBackColor = false;
            // 
            // button7
            // 
            button7.BackColor = Color.CornflowerBlue;
            button7.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            button7.ForeColor = Color.White;
            button7.Location = new Point(6, 22);
            button7.Name = "button7";
            button7.Size = new Size(143, 34);
            button7.TabIndex = 0;
            button7.Text = "Load config";
            button7.UseVisualStyleBackColor = false;
            // 
            // panel5
            // 
            panel5.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            panel5.Controls.Add(label7);
            panel5.Controls.Add(button11);
            panel5.Controls.Add(button13);
            panel5.Controls.Add(button12);
            panel5.Controls.Add(button10);
            panel5.Controls.Add(panel7);
            panel5.ForeColor = Color.White;
            panel5.Location = new Point(687, 295);
            panel5.Name = "panel5";
            panel5.Size = new Size(308, 155);
            panel5.TabIndex = 10;
            panel5.TabStop = false;
            panel5.Text = "DATA MANAGEMENT && FILTERING";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point, 129);
            label7.ForeColor = Color.White;
            label7.Location = new Point(192, 47);
            label7.Name = "label7";
            label7.Size = new Size(46, 21);
            label7.TabIndex = 8;
            label7.Text = "(0, 0)";
            // 
            // button11
            // 
            button11.BackColor = Color.Silver;
            button11.ForeColor = Color.Black;
            button11.Location = new Point(153, 79);
            button11.Name = "button11";
            button11.Size = new Size(67, 29);
            button11.TabIndex = 7;
            button11.Text = "Set L";
            button11.UseVisualStyleBackColor = false;
            // 
            // button13
            // 
            button13.BackColor = Color.Red;
            button13.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            button13.ForeColor = Color.White;
            button13.Location = new Point(153, 114);
            button13.Name = "button13";
            button13.Size = new Size(138, 29);
            button13.TabIndex = 6;
            button13.Text = "Delete";
            button13.UseVisualStyleBackColor = false;
            // 
            // button12
            // 
            button12.BackColor = Color.Silver;
            button12.ForeColor = Color.Black;
            button12.Location = new Point(224, 79);
            button12.Name = "button12";
            button12.Size = new Size(67, 29);
            button12.TabIndex = 5;
            button12.Text = "Set R";
            button12.UseVisualStyleBackColor = false;
            // 
            // button10
            // 
            button10.BackColor = Color.CornflowerBlue;
            button10.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            button10.ForeColor = Color.White;
            button10.Location = new Point(153, 15);
            button10.Name = "button10";
            button10.Size = new Size(138, 29);
            button10.TabIndex = 3;
            button10.Text = "Apply Filter";
            button10.UseVisualStyleBackColor = false;
            // 
            // panel7
            // 
            panel7.Controls.Add(checkBox3);
            panel7.Controls.Add(checkBox2);
            panel7.Controls.Add(checkBox1);
            panel7.ForeColor = Color.White;
            panel7.Location = new Point(6, 22);
            panel7.Name = "panel7";
            panel7.Size = new Size(141, 121);
            panel7.TabIndex = 0;
            panel7.TabStop = false;
            panel7.Text = "Filter Options";
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            checkBox3.ForeColor = Color.White;
            checkBox3.Location = new Point(6, 79);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(90, 19);
            checkBox3.TabIndex = 2;
            checkBox3.Text = "Large Angle";
            checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            checkBox2.ForeColor = Color.White;
            checkBox2.Location = new Point(6, 54);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(82, 19);
            checkBox2.TabIndex = 1;
            checkBox2.Text = "Angle == 0"; // 오타 수정 (Angel -> Angle)
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            checkBox1.ForeColor = Color.White;
            checkBox1.Location = new Point(6, 29);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(61, 19);
            checkBox1.TabIndex = 0;
            checkBox1.Text = "Thr > 0"; // Thr > 1 에서 기획서대로 0으로 변경
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // panel6
            // 
            panel6.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            panel6.Controls.Add(richTextBox1);
            panel6.ForeColor = Color.White;
            panel6.Location = new Point(687, 456);
            panel6.Name = "panel6";
            panel6.Size = new Size(308, 125);
            panel6.TabIndex = 11;
            panel6.TabStop = false;
            panel6.Text = "TRAINING LOG";
            // 
            // richTextBox1
            // 
            richTextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            richTextBox1.Location = new Point(6, 22);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(296, 97);
            richTextBox1.TabIndex = 1;
            richTextBox1.Text = "";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe Script", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.ForeColor = Color.Red;
            label6.Location = new Point(24, 9);
            label6.Name = "label6";
            label6.Size = new Size(244, 37);
            label6.TabIndex = 12;
            label6.Text = "Team 05 Donkey UI";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            ClientSize = new Size(1007, 604);
            Controls.Add(label6);
            Controls.Add(panel6);
            Controls.Add(panel5);
            Controls.Add(panel4);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            ForeColor = SystemColors.ControlText;
            MinimumSize = new Size(1023, 643); // 폼 최소 크기 지정 (컨트롤 겹침 방지)
            Name = "Form1";
            Text = "MoveArt Donkeycar Data Manager"; // 폼 제목 설정
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            panel3.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            panel7.ResumeLayout(false);
            panel7.PerformLayout();
            panel6.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private GroupBox panel1; // 변수 타입들 모두 GroupBox로 변경
        private GroupBox panel2;
        private TrackBar trackBar1;
        private Button button1;
        private GroupBox panel3;
        private ListBox listBox1;
        private GroupBox panel4;
        private GroupBox panel5;
        private GroupBox panel7;
        private GroupBox panel6;
        private Label label6;
        private Button button6;
        private Button button5;
        private Button button4;
        private Button button3;
        private Button button2;
        private Button button7;
        private Button button9;
        private Button button8;
        private CheckBox checkBox3;
        private CheckBox checkBox2;
        private CheckBox checkBox1;
        private Button button10;
        private Button button11;
        private Button button13;
        private Button button12;
        private Button button14;
        private Label label7;
        private RichTextBox richTextBox1;
    }
}