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
        string infixaMostrada = "";
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

            if (Width >= MinimumSize.Width && Height >= MinimumSize.Height) // Evita que haja tentativa de responsividade enquanto a tela volta de um estado minimizado
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
            double result = calculadora.CalcularExpressao();
            lblPosFixa.Text = posfixaDefault + calculadora.Posfixa;
            txtVisor.Text = result.ToString();
        }

        private void btnSqrt_Click(object sender, EventArgs e) //evento das operacoes
        {
            Button btn = (sender as Button);
            string toAdd = "";
            if (btn.Text.Contains("log") || btn.Text.Contains("√"))
            {
                //if (infixaMostrada.Length > 0 && infixaMostrada[infixaMostrada.Length - 1] != '(')
                //    toAdd = " " + btn.Text + "(";
                //else
                toAdd += btn.Text + "(";
            }
            else if (btn.Text.Equals("-"))
            {
                if (infixaMostrada.Length == 0)
                    toAdd = btn.Text;
                else if (infixaMostrada.Length > 0 && infixaMostrada[infixaMostrada.Length - 1] == '(')
                    toAdd = btn.Text;
                else
                    toAdd = " " + btn.Text;
            }
            else if (!btn.Text.Contains("!") && !btn.Text.Contains("-"))
                toAdd += " " + btn.Text.Substring(btn.Text.Length - 1);
            else
                toAdd += btn.Text.Substring(btn.Text.Length - 1);
            infixaMostrada += toAdd;
            calculadora.Infixa += toAdd;
            HabilitarBotoes();
            AtualizarVisor();
        }

        private void btn0_Click(object sender, EventArgs e) //evento pra click em numeros
        {
            string toAdd = "";
            if (infixaMostrada.Length > 0)
            {
                if (infixaMostrada.Length == 1)
                    toAdd = (sender as Button).Text;
                else
                {
                    string ultimoCaractere = infixaMostrada.Substring(infixaMostrada.Length - 1);
                    if (ultimoCaractere.Equals("-"))
                    {
                        string antesDoUltimoCaractere = infixaMostrada.Substring(infixaMostrada.Length - 2, 1);
                        if (antesDoUltimoCaractere.Equals("("))
                        {
                            toAdd = (sender as Button).Text;
                        }
                        else
                            toAdd = " " + (sender as Button).Text;
                    }
                    else if (int.TryParse(ultimoCaractere, out int n) ||
                        ultimoCaractere.Equals("(") || ultimoCaractere.Equals(","))
                        toAdd = (sender as Button).Text;
                    else
                        toAdd = " " + (sender as Button).Text;
                }
            }
            else
            {
                if ((sender as Button).Text.Equals(","))
                    toAdd = "0" + (sender as Button).Text;
                else
                    toAdd = (sender as Button).Text;
            }
            infixaMostrada += toAdd;
            calculadora.Infixa += toAdd;
            HabilitarBotoes();
            AtualizarVisor();
        }

        private void btnCE_Click(object sender, EventArgs e)
        {
            infixaMostrada = "";
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
                infixaMostrada = infixaMostrada.Substring(0, infixaMostrada.Length - 1);
                if (infixaMostrada.Length > 0)
                    ultimoCaractere = infixaMostrada.Substring(infixaMostrada.Length - 1);
                else
                    break;
            } while (ultimoCaractere.Equals(" "));
            HabilitarBotoes();
            AtualizarVisor();
        }
        private void AtualizarVisor()
        {
            if (infixaMostrada.Length > 0)
                btnApagar.Enabled = true;
            else
                btnApagar.Enabled = false;
            txtVisor.Text = infixaMostrada;
        }
        private void HabilitarBotoes()
        {
            string[] coisas = infixaMostrada.Split(' ');
            string ultimaCoisa = coisas[coisas.Length - 1];
            if (ultimaCoisa.Length > 1)
                ultimaCoisa = ultimaCoisa.Substring(ultimaCoisa.Length-1);
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
                    if (ultimaCoisa.Equals("("))
                        btnMenos.Enabled = true;
                    else
                        btnMenos.Enabled = false;
                    if (!ultimaCoisa.Equals(")"))
                        btnDividir.Enabled = btnMultiplicar.Enabled = btnElevado.Enabled = btnFatorial.Enabled = btnMais.Enabled = false;
                    else
                    {
                        btnDividir.Enabled = btnMultiplicar.Enabled = btnElevado.Enabled = btnFatorial.Enabled = btnMais.Enabled = btnMenos.Enabled = true;
                        btnFatorial.Enabled = btnLog.Enabled = btnSqrt.Enabled = false;
                    }
                }
                else
                {
                    btnAbreParenteses.Enabled = true;
                }
            }
            if (infixaMostrada.Equals(""))
            {
                btnDecimal.Enabled = btnMenos.Enabled = true;
                btnFechaParenteses.Enabled = false;
            }
        }

        private void btnAbreParenteses_Click(object sender, EventArgs e)
        {
            Button btn = (sender as Button);
            if (infixaMostrada.Length > 0 && btn.Text.Equals("("))
            {
                infixaMostrada += " " + btn.Text;
                calculadora.Infixa += " " + btn.Text + " ";
            }
            else if (btn.Text.Equals(")"))
            {
                infixaMostrada += btn.Text;
                calculadora.Infixa += " " + btn.Text;
            }
            HabilitarBotoes();
            AtualizarVisor();
        }
        private bool IsNumeric(string str)
        {
            for (int i = 0; i < str.Length; i++)
                if (str[i] < 48 || str[i] > 57)
                    return false;
            return true;
        }

        private void FrmCalculadora_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back && btnApagar.Enabled)
                btnApagar.PerformClick();
            else if (e.KeyCode == Keys.Enter && btnIgual.Enabled)
                btnIgual.PerformClick();
        }

        private void FrmCalculadora_KeyPress(object sender, KeyPressEventArgs e)
        {
            char caracter = e.KeyChar;
            if (IsNumeric(caracter.ToString()))
                (Controls.Find("btn" + caracter, false)[0] as Button).PerformClick();
            else if (caracter == '(')
                btnAbreParenteses.PerformClick();
            else if (caracter == ')')
                btnFechaParenteses.PerformClick();
            else if (caracter == '.' || caracter == ',')
                btnDecimal.PerformClick();
            else if (caracter == '/')
                btnDividir.PerformClick();
            else if (caracter == '*')
                btnMultiplicar.PerformClick();
            else if (caracter == '-')
                btnMenos.PerformClick();
            else if (caracter == '+')
                btnMais.PerformClick();
            else if (caracter == '^')
                btnElevado.PerformClick();
            else if (caracter == '!')
                btnFatorial.PerformClick();
            else if (caracter.ToString().ToLower() == "q")
                btnSqrt.PerformClick();
            else if (caracter.ToString().ToLower() == "l")
                btnLog.PerformClick();
        }
    }
}