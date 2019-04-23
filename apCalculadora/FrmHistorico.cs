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
    public partial class FrmHistorico : Form
    {
        public FrmHistorico()
        {
            InitializeComponent();
        }
        public void Atualizar(string conta)
        {
            lsbHistorico.Items.Insert(0, conta);
        }
        private void LsbHistorico_Resize(object sender, EventArgs e)
        {
            // Centraliza o botão
            btnApagarHistorico.Left = lsbHistorico.Left + (lsbHistorico.Width - btnApagarHistorico.Width) / 2;
        }

        private void BtnApagarHistorico_Click(object sender, EventArgs e)
        {
            lsbHistorico.Items.Clear();
        }
    }
}
