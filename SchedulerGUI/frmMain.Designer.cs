namespace SchedulerGUI
{
  partial class frmMain
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
      this.lstEvents = new System.Windows.Forms.ListBox();
      this.button1 = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // lstEvents
      // 
      this.lstEvents.FormattingEnabled = true;
      this.lstEvents.Location = new System.Drawing.Point(12, 77);
      this.lstEvents.Name = "lstEvents";
      this.lstEvents.Size = new System.Drawing.Size(641, 290);
      this.lstEvents.TabIndex = 0;
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(545, 31);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(108, 40);
      this.button1.TabIndex = 1;
      this.button1.Text = "button1";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // frmMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(665, 382);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.lstEvents);
      this.Name = "frmMain";
      this.Text = "Scheduler GUI";
      this.Load += new System.EventHandler(this.frmMain_Load);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListBox lstEvents;
    private System.Windows.Forms.Button button1;
  }
}

