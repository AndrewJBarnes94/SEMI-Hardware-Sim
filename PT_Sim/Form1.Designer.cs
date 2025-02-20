using System.Windows.Forms;

namespace PT_Sim
{
    partial class Form1
    {
        private System.Windows.Forms.Panel panel1;

        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();

            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(this.ClientSize.Width / 2, 0); // Start at midpoint
            this.panel1.Size = new System.Drawing.Size(this.ClientSize.Width / 2, this.ClientSize.Height); // Take right half
            this.panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right; // Stretch dynamically
            this.Controls.Add(this.panel1);

            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "Form1";
            this.Text = "OpenTK WinForms App";
            this.Resize += new System.EventHandler(this.Form1_Resize); // Handle resizing
            this.ResumeLayout(false);
        }
    }
}
