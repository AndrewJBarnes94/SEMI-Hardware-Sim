using System;
using System.Windows.Forms;

namespace PT_Sim
{
    public partial class Form1 : Form
    {
        private HA600 glPanel;
        private TextBox angle1TextBox;
        private TextBox angle2TextBox;
        private TextBox angle3TextBox;
        private Button applyButton;

        public Form1()
        {
            InitializeComponent();

            this.BackColor = System.Drawing.Color.FromArgb(200, 200, 200); // Dark Grey Background

            this.WindowState = FormWindowState.Maximized;

            InitializeOpenGL();
            InitializeControls();
        }

        private void InitializeOpenGL()
        {
            glPanel = new HA600();
            glPanel.Dock = DockStyle.Fill;
            panel1.Controls.Add(glPanel);

            // Force an initial repaint after loading
            this.Shown += (sender, e) => glPanel.Invalidate();
        }

        private void InitializeControls()
        {
            // Initialize TextBoxes for angles
            angle1TextBox = new TextBox { Location = new System.Drawing.Point(10, 10), Width = 100 };
            angle2TextBox = new TextBox { Location = new System.Drawing.Point(10, 40), Width = 100 };
            angle3TextBox = new TextBox { Location = new System.Drawing.Point(10, 70), Width = 100 };

            // Initialize Button to apply changes
            applyButton = new Button { Location = new System.Drawing.Point(10, 100), Text = "Apply", Width = 100 };
            applyButton.Click += ApplyButton_Click;

            // Add controls to the form
            this.Controls.Add(angle1TextBox);
            this.Controls.Add(angle2TextBox);
            this.Controls.Add(angle3TextBox);
            this.Controls.Add(applyButton);
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            if (float.TryParse(angle1TextBox.Text, out float angle1) &&
                float.TryParse(angle2TextBox.Text, out float angle2) &&
                float.TryParse(angle3TextBox.Text, out float angle3))
            {
                glPanel.SetRobotAngles(angle1, angle2, angle3);
            }
            else
            {
                MessageBox.Show("Please enter valid angles.");
            }
        }

        // Ensures panel1 is always on the right half of the window
        private void Form1_Resize(object sender, EventArgs e)
        {
            panel1.Location = new System.Drawing.Point(this.ClientSize.Width / 2, 0); // Set right-side position
            panel1.Size = new System.Drawing.Size(this.ClientSize.Width / 2, this.ClientSize.Height); // Adjust size
        }
    }
}

