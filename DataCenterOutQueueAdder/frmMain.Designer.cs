namespace DataCenterOutQueueAdder
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
      this.btnAddPriceRequest = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btnAddPriceRequest
      // 
      this.btnAddPriceRequest.Location = new System.Drawing.Point(28, 34);
      this.btnAddPriceRequest.Name = "btnAddPriceRequest";
      this.btnAddPriceRequest.Size = new System.Drawing.Size(168, 53);
      this.btnAddPriceRequest.TabIndex = 0;
      this.btnAddPriceRequest.Text = "Add PriceRequest";
      this.btnAddPriceRequest.UseVisualStyleBackColor = true;
      this.btnAddPriceRequest.Click += new System.EventHandler(this.btnAddPriceRequest_Click);
      // 
      // frmMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(540, 343);
      this.Controls.Add(this.btnAddPriceRequest);
      this.Name = "frmMain";
      this.Text = "OutQueue Adder";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnAddPriceRequest;
  }
}

