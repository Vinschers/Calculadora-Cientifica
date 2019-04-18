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
            operacaoInfixa = "";
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
            Button btn = (sender as Button);
            if (btn.Text.Contains("log") || btn.Text.Contains("√"))
            {
                if (operacaoInfixa.Length > 0 && operacaoInfixa[operacaoInfixa.Length - 1] != '(')
                    operacaoInfixa += " " + btn.Text + "(";
                else
                    operacaoInfixa += btn.Text + "(";
            }
            else if (!btn.Text.Contains("!") && !btn.Text.Contains("^") && !btn.Text.Contains("/"))
                operacaoInfixa += " " + btn.Text.Substring(((Button)sender).Text.Length - 1);
            else
                operacaoInfixa += btn.Text.Substring(((Button)sender).Text.Length - 1);
            HabilitarBotoes();
            AtualizarVisor();
        }

        private void btn0_Click(object sender, EventArgs e) //evento pra click em numeros
        {
            if (operacaoInfixa.Length > 0)
            {
                string ultimoCaractere = operacaoInfixa.Substring(operacaoInfixa.Length - 1);
                if (int.TryParse(ultimoCaractere, out int n) ||
                    ultimoCaractere.Equals("(") || ultimoCaractere.Equals(",") || ultimoCaractere.Equals("^") || ultimoCaractere.Equals("/"))
                    operacaoInfixa += (sender as Button).Text;
                else
                    operacaoInfixa += " " + (sender as Button).Text;
            }
            else
            {
                if ((sender as Button).Text.Equals(","))
                    operacaoInfixa += "0" + (sender as Button).Text;
                else
                    operacaoInfixa += (sender as Button).Text;
            }
            HabilitarBotoes();
            AtualizarVisor();
        }

        private void btnCE_Click(object sender, EventArgs e)
        {
            operacaoInfixa = "";
            HabilitarBotoes();
            AtualizarVisor();
        }

        private void btnC_Click(object sender, EventArgs e)
        {

        }

        private void btnApagar_Click(object sender, EventArgs e)
        {
            string ultimoCaractere = "";
            do
            {
                operacaoInfixa = operacaoInfixa.Substring(0, operacaoInfixa.Length - 1);
                if (operacaoInfixa.Length > 0)
                    ultimoCaractere = operacaoInfixa.Substring(operacaoInfixa.Length - 1);
                else
                    break;
            } while (ultimoCaractere.Equals(" "));
            AtualizarVisor();
        }
        private void AtualizarVisor()
        {
            if (operacaoInfixa.Length > 0)
                btnApagar.Enabled = true;
            else
                btnApagar.Enabled = false;
            txtVisor.Text = operacaoInfixa;
        }
        private void HabilitarBotoes()
        {
            string[] coisas = operacaoInfixa.Split(' ');
            string ultimaCoisa = coisas[coisas.Length - 1];
            if (!ultimaCoisa.Equals("") && IsNumeric(ultimaCoisa[ultimaCoisa.Length-1].ToString()))
            {
                if (ultimaCoisa.Contains(","))
                    btnDecimal.Enabled = false;
                foreach (Control c in Controls)
                    if (c is Button)
                    {
                        Button btn = (c as Button);
                        if (!(btn.Text.Contains("√") || btn.Text.Contains("log") || btn.Text.Contains("(")))
                            btn.Enabled = true;
                        else
                            btn.Enabled = false;
                    }
            }
            else
            {
                btnDecimal.Enabled = false;
                if (ultimaCoisa.Contains("log") || ultimaCoisa.Contains("√"))
                {
                    btnAbreParenteses.Enabled = true;
                    btnDividir.Enabled = btnMultiplicar.Enabled = btnElevado.Enabled = btnFatorial.Enabled = btnMais.Enabled = btnMenos.Enabled = false;
                    btnSqrt.Enabled = btnLog.Enabled = true;
                }
                else if (ultimaCoisa.Contains(","))
                {
                    btnAbreParenteses.Enabled = btnSqrt.Enabled = btnLog.Enabled = btnDividir.Enabled = btnMultiplicar.Enabled = btnElevado.Enabled = btnFatorial.Enabled = btnMais.Enabled = btnMenos.Enabled = false;
                }
                else if (!ultimaCoisa.Contains("!") && !ultimaCoisa.Contains("^") && !ultimaCoisa.Contains("/"))
                {
                    btnAbreParenteses.Enabled = true;
                    btnSqrt.Enabled = btnLog.Enabled = true;
                    btnDividir.Enabled = btnMultiplicar.Enabled = btnElevado.Enabled = btnFatorial.Enabled = btnMais.Enabled = btnMenos.Enabled = false;
                }
            }
        }

        private void btnAbreParenteses_Click(object sender, EventArgs e)
        {
            Button btn = (sender as Button);
            if (operacaoInfixa.Length > 0 && btn.Text.Equals("("))
                operacaoInfixa += " " + btn.Text;
            else
                operacaoInfixa += btn.Text;
            AtualizarVisor();
        }
        private bool IsNumeric(string str)
        {
            return int.TryParse(str, out int n);
        }
    }
}
