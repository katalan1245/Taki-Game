using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TakiClient
{
    public partial class FrmOpen : Form
    {
        public FrmOpen()
        {
            InitializeComponent();
        }

        private void InstructionBtn_Click(object sender, EventArgs e)
        {
            Form instructionForm = new FrmInstructions();
            instructionForm.ShowDialog();
        }

        private void playBtn_Click(object sender, EventArgs e)
        {
            Form playForm = new FrmUser(this);
            playForm.Show();
            this.Hide();
        }
    }
}
