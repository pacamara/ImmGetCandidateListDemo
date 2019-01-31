namespace ImmGetCandidateListDemo
{
    partial class ImmForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.immButton = new System.Windows.Forms.Button();
            this.immLabel = new System.Windows.Forms.Label();
            this.immTextBox = new System.Windows.Forms.RichTextBox();
            this.immCandidatesBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // immButton
            // 
            this.immButton.Location = new System.Drawing.Point(188, 140);
            this.immButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.immButton.Name = "immButton";
            this.immButton.Size = new System.Drawing.Size(130, 34);
            this.immButton.TabIndex = 2;
            this.immButton.Text = "Invoke IME";
            this.immButton.UseVisualStyleBackColor = true;
            this.immButton.Click += new System.EventHandler(this.immButton_Click);
            // 
            // immLabel
            // 
            this.immLabel.AutoSize = true;
            this.immLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.immLabel.Location = new System.Drawing.Point(29, 29);
            this.immLabel.Name = "immLabel";
            this.immLabel.Size = new System.Drawing.Size(432, 31);
            this.immLabel.TabIndex = 3;
            this.immLabel.Text = "ImmGetCandidateList demo for C#";
            // 
            // immTextBox
            // 
            this.immTextBox.Font = new System.Drawing.Font("Microsoft YaHei", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.immTextBox.ImeMode = System.Windows.Forms.ImeMode.On;
            this.immTextBox.Location = new System.Drawing.Point(35, 83);
            this.immTextBox.Name = "immTextBox";
            this.immTextBox.Size = new System.Drawing.Size(426, 32);
            this.immTextBox.TabIndex = 4;
            this.immTextBox.Text = "";
            // 
            // immCandidatesBox
            // 
            this.immCandidatesBox.Location = new System.Drawing.Point(35, 199);
            this.immCandidatesBox.Name = "immCandidatesBox";
            this.immCandidatesBox.Size = new System.Drawing.Size(426, 328);
            this.immCandidatesBox.TabIndex = 5;
            this.immCandidatesBox.Text = "";
            // 
            // ImmForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 565);
            this.Controls.Add(this.immCandidatesBox);
            this.Controls.Add(this.immTextBox);
            this.Controls.Add(this.immLabel);
            this.Controls.Add(this.immButton);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ImmForm";
            this.Text = "ImmForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button immButton;
        private System.Windows.Forms.Label immLabel;
        private System.Windows.Forms.RichTextBox immTextBox;
        private System.Windows.Forms.RichTextBox immCandidatesBox;
    }
}

