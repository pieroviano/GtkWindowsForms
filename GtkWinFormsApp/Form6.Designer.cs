namespace GtkWinFormsApp
{
    partial class Form6
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        internal System.ComponentModel.IContainer components = null;

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
        internal void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            addressBox = new System.Windows.Forms.TextBox();
            goButton = new System.Windows.Forms.Button();
            backButton = new System.Windows.Forms.Button();
            forwardButton = new System.Windows.Forms.Button();
            refreshButton = new System.Windows.Forms.Button();
            stopButton = new System.Windows.Forms.Button();
            statusLabel = new System.Windows.Forms.Label();
            webBrowser1 = new System.Windows.Forms.WebBrowser();
            SuspendLayout();
            //
            // backButton
            //
            backButton.Location = new System.Drawing.Point(12, 12);
            backButton.Name = "backButton";
            backButton.Size = new System.Drawing.Size(40, 27);
            backButton.TabIndex = 0;
            backButton.Text = "<";
            backButton.UseVisualStyleBackColor = true;
            backButton.Click += backButton_Click;
            //
            // forwardButton
            //
            forwardButton.Location = new System.Drawing.Point(58, 12);
            forwardButton.Name = "forwardButton";
            forwardButton.Size = new System.Drawing.Size(40, 27);
            forwardButton.TabIndex = 1;
            forwardButton.Text = ">";
            forwardButton.UseVisualStyleBackColor = true;
            forwardButton.Click += forwardButton_Click;
            //
            // refreshButton
            //
            refreshButton.Location = new System.Drawing.Point(104, 12);
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new System.Drawing.Size(70, 27);
            refreshButton.TabIndex = 2;
            refreshButton.Text = "Refresh";
            refreshButton.UseVisualStyleBackColor = true;
            refreshButton.Click += refreshButton_Click;
            //
            // stopButton
            //
            stopButton.Location = new System.Drawing.Point(180, 12);
            stopButton.Name = "stopButton";
            stopButton.Size = new System.Drawing.Size(60, 27);
            stopButton.TabIndex = 3;
            stopButton.Text = "Stop";
            stopButton.UseVisualStyleBackColor = true;
            stopButton.Click += stopButton_Click;
            //
            // addressBox
            //
            addressBox.Location = new System.Drawing.Point(246, 14);
            addressBox.Name = "addressBox";
            addressBox.Size = new System.Drawing.Size(430, 23);
            addressBox.TabIndex = 4;
            addressBox.Text = "https://example.com/";
            //
            // goButton
            //
            goButton.Location = new System.Drawing.Point(682, 12);
            goButton.Name = "goButton";
            goButton.Size = new System.Drawing.Size(60, 27);
            goButton.TabIndex = 5;
            goButton.Text = "Go";
            goButton.UseVisualStyleBackColor = true;
            goButton.Click += goButton_Click;
            //
            // webBrowser1
            //
            webBrowser1.Location = new System.Drawing.Point(12, 45);
            webBrowser1.Name = "webBrowser1";
            webBrowser1.Size = new System.Drawing.Size(730, 460);
            webBrowser1.TabIndex = 6;
            //
            // statusLabel
            //
            statusLabel.AutoSize = true;
            statusLabel.Location = new System.Drawing.Point(12, 513);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new System.Drawing.Size(0, 15);
            statusLabel.TabIndex = 7;
            //
            // Form6
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(754, 541);
            Controls.Add(statusLabel);
            Controls.Add(webBrowser1);
            Controls.Add(goButton);
            Controls.Add(addressBox);
            Controls.Add(stopButton);
            Controls.Add(refreshButton);
            Controls.Add(forwardButton);
            Controls.Add(backButton);
            Name = "Form6";
            Text = "WebBrowser";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        internal System.Windows.Forms.TextBox addressBox;
        internal System.Windows.Forms.Button goButton;
        internal System.Windows.Forms.Button backButton;
        internal System.Windows.Forms.Button forwardButton;
        internal System.Windows.Forms.Button refreshButton;
        internal System.Windows.Forms.Button stopButton;
        internal System.Windows.Forms.Label statusLabel;
        internal System.Windows.Forms.WebBrowser webBrowser1;
    }
}
