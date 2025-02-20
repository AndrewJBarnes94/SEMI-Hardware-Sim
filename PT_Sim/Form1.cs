using System;
using System.Windows.Forms;

namespace PT_Sim
{
    public partial class Form1 : Form
    {
        private GLRenderPanel glPanel;

        public Form1()
        {
            InitializeComponent();

            this.BackColor = System.Drawing.Color.FromArgb(100, 100, 100); // Dark Grey Backgrounf

            this.WindowState = FormWindowState.Maximized;

            InitializeOpenGL();
        }

        private void InitializeOpenGL()
        {
            glPanel = new GLRenderPanel();
            glPanel.Dock = DockStyle.Fill;
            panel1.Controls.Add(glPanel);

            // Force an initial repaint after loading
            this.Shown += (sender, e) => glPanel.Invalidate();
        }


        // Ensures panel1 is always on the right half of the window
        private void Form1_Resize(object sender, EventArgs e)
        {
            panel1.Location = new System.Drawing.Point(this.ClientSize.Width / 2, 0); // Set right-side position
            panel1.Size = new System.Drawing.Size(this.ClientSize.Width / 2, this.ClientSize.Height); // Adjust size
        }
    }
}
