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
        const string posfixaDefault = "Operação em sequência pós-fixa: ";
        Button[,] botoes = new Button[5, 5];

        Calculadora calculadora;
        public frmCalculadora()
        {
            InitializeComponent();
            lblPosFixa.AutoSize = true;
            calculadora = new Calculadora();
            lblPosFixa.Text = posfixaDefault;
            // Facilita a responsividade
            ColocarBotoesNaMatriz();
            
        }
        private void ColocarBotoesNaMatriz()
        {
            int linha = 4, coluna = 4;

            // Percorre os controles em busca dos botões, que estão em ordem do canto inferior direito até o superior esquerdo
            foreach (Control ctrl in Controls)
                if (ctrl.Name.Substring(0, 3) == "btn")
                {
                    botoes[linha, coluna] = (Button)ctrl;

                    if (--coluna == -1) // Ao achar um botão, retrocede uma coluna
                    { // Se acabarem as colunas, retrocede uma linha e volta a coluna até o canto direito
                        linha--;
                        coluna = 4;

                        if (linha == -1)
                            break;
                    }
                }
        }

        private void frmCalculadora_Resize(object sender, EventArgs e)
        {
            lblPosFixa.MaximumSize = new Size(Width, 0); //permite quebra de linha no label da sequencia posfixa
            Reposicionar();       
        }
        private void Reposicionar()
        {
            // Margem interna (entre os botões)
            int margem = 3;

            // Cada botão terá 1/5 do tamanho total, desconsiderando espaços externos e margens internas
            int widthBotoes = (Width - 39 - 8 * margem) / 5; 
            int heightBotoes = (Height - 183 - 8 * margem) / 5;

            float fontSize;

            // Tamanho da fonte baseado na menor dimensão
            if (widthBotoes < heightBotoes)
                fontSize = widthBotoes / 5;
            else
                fontSize = heightBotoes / 5;

            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                {
                    botoes[j, i].Width = widthBotoes;
                    botoes[j, i].Height = heightBotoes;

                    // A localização deve considerar a posição do primeiro e as margens entre os botões
                    botoes[j, i].Left = 12 + (widthBotoes + 2 * margem) * i;
                    botoes[j, i].Top = 136 + (heightBotoes + 2 * margem) * j;

                    // btnApagar tem um caracter especial, que deve estar com uma fonte maior
                    if (botoes[j, i] != btnApagar)
                        botoes[j, i].Font = new Font(botoes[j, i].Font.FontFamily, fontSize);
                    else
                        botoes[j, i].Font = new Font(botoes[j, i].Font.FontFamily, fontSize * 5 / 4);
                }
        }

        private void btnIgual_Click(object sender, EventArgs e) //evento para calcular
        {
            lblPosFixa.Text = posfixaDefault + calculadora.Posfixa;
            double result = calculadora.CalcularExpressao();
            txtVisor.Text = result.ToString();
        }

        private void btnSqrt_Click(object sender, EventArgs e) //evento das operacoes
        {
            Button btn = (sender as Button);
            if (btn.Text.Contains("log") || btn.Text.Contains("√"))
            {
                if (calculadora.Infixa.Length > 0 && calculadora.Infixa[calculadora.Infixa.Length - 1] != '(')
                    calculadora.Infixa += " " + btn.Text + "(";
                else
                    calculadora.Infixa += btn.Text + "(";
            }
            else if (!btn.Text.Contains("!") && !btn.Text.Contains("^") && !btn.Text.Contains("/"))
                calculadora.Infixa += " " + btn.Text.Substring(((Button)sender).Text.Length - 1);
            else
                calculadora.Infixa += btn.Text.Substring(((Button)sender).Text.Length - 1);
            HabilitarBotoes();
            AtualizarVisor();
        }

        private void btn0_Click(object sender, EventArgs e) //evento pra click em numeros
        {
            if (calculadora.Infixa.Length > 0)
            {
                string ultimoCaractere = calculadora.Infixa.Substring(calculadora.Infixa.Length - 1);
                if (int.TryParse(ultimoCaractere, out int n) ||
                    ultimoCaractere.Equals("(") || ultimoCaractere.Equals(",") || ultimoCaractere.Equals("^") || ultimoCaractere.Equals("/"))
                    calculadora.Infixa += (sender as Button).Text;
                else
                    calculadora.Infixa += " " + (sender as Button).Text;
            }
            else
            {
                if ((sender as Button).Text.Equals(","))
                    calculadora.Infixa += "0" + (sender as Button).Text;
                else
                    calculadora.Infixa += (sender as Button).Text;
            }
            HabilitarBotoes();
            AtualizarVisor();
        }

        private void btnCE_Click(object sender, EventArgs e)
        {
            calculadora.Infixa = "";
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
                calculadora.Infixa = calculadora.Infixa.Substring(0, calculadora.Infixa.Length - 1);
                if (calculadora.Infixa.Length > 0)
                    ultimoCaractere = calculadora.Infixa.Substring(calculadora.Infixa.Length - 1);
                else
                    break;
            } while (ultimoCaractere.Equals(" "));
            AtualizarVisor();
        }
        private void AtualizarVisor()
        {
            if (calculadora.Infixa.Length > 0)
                btnApagar.Enabled = true;
            else
                btnApagar.Enabled = false;
            txtVisor.Text = calculadora.Infixa;
        }
        private void HabilitarBotoes()
        {
            string[] coisas = calculadora.Infixa.Split(' ');
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
            if (calculadora.Infixa.Length > 0 && btn.Text.Equals("("))
                calculadora.Infixa += " " + btn.Text;
            else
                calculadora.Infixa += btn.Text;
            AtualizarVisor();
        }
        private bool IsNumeric(string str)
        {
            return int.TryParse(str, out int n);
        }
    }
}