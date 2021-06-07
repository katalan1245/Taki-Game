using System;
using System.Windows.Forms;

namespace TakiClient
{
    public partial class FrmConnect : Form
    {
        private FrmUser parentForm;
        public FrmConnect(FrmUser parentForm)
        {
            this.parentForm = parentForm;
            InitializeComponent();
        }

        private void frmConnect_Load(object sender, EventArgs e)
        {
            Random rnd = new Random();
            txtIP.Text = "127.0.0.1";
            txtPort.Text = "8002";
            txtName.Text = $"user{rnd.Next(10)}";
            parentForm.ShouldConnect = false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (txtIP.Text != "" && txtPort.Text != "" && txtName.Text != "")
            {
                parentForm.IpAddress = txtIP.Text;
                parentForm.Port = Convert.ToInt32(txtPort.Text);
                parentForm.UserName = txtName.Text;
                parentForm.ShouldConnect = true;
                this.Close();
            }
            else
                MessageBox.Show("עלייך למלא את כל הנתונים", "שגיאה", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void FrmConnect_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (parentForm.ShouldConnect)
                return;
            parentForm.ShouldConnect = false;
        }
    }
}
