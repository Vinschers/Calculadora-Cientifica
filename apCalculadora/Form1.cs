using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace apCalculadora
{
    public partial class frmCalculadora : Form
    {
        string operacaoInfixa;
        public frmCalculadora()
        {
            InitializeComponent();
            lblPosFixa.AutoSize = true;
        }

        private void frmCalculadora_Resize(object sender, EventArgs e)
        {
            lblPosFixa.MaximumSize = new Size(Width, 0);
        }

        private void btnIgual_Click(object sender, EventArgs e) //evento para calcular
        {
            //
        }

        private void btnSqrt_Click(object sender, EventArgs e) //evento das operacoes
        {
            operacaoInfixa += " " + ((Button)sender).Text.Substring(((Button)sender).Text.Length - 1);
            foreach (Control c in Controls)
                if (c is Button)
                {
                    if ((c as Button).Text.Length != 6)
                        (c as Button).Enabled = false;
                    else
                        (c as Button).Enabled = true;
                }
        }

        private void btn0_Click(object sender, EventArgs e) //evento pra click em numeros
        {
            //
        }

        private void btnCE_Click(object sender, EventArgs e)
        {
            txtVisor.Text = "";
        }

        private void btnC_Click(object sender, EventArgs e)
        {

        }

        private void btnApagar_Click(object sender, EventArgs e)
        {

        }
    }
}
