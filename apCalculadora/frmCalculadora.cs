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
    public partial class FrmCalculadora : Form
    {
        const string posfixaDefault = "Operação em sequência pós-fixa: ";
        string infixaMostrada = "";
        Button[,] botoes = new Button[5, 5];
        FrmHistorico historico;

        Calculadora calculadora;
        public FrmCalculadora()
        {
            InitializeComponent();

            lblPosFixa.AutoSize = true;

            calculadora = new Calculadora();
            calculadora.IniciarNovaConta();

            lblPosFixa.Text = posfixaDefault;

            // Guarda o histórico de contas
            historico = new FrmHistorico();

            // Facilita a responsividade
            ColocarBotoesNaMatriz();

            // Tira o foco de qualquer botão
            txtVisor.Focus();
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
            try
            {
                string result = calculadora.CalcularExpressao().ToString();
                lblPosFixa.Text = posfixaDefault + calculadora.Posfixa;

                historico.Atualizar(infixaMostrada + " = " + result);
                infixaMostrada = result;
                txtVisor.Text = result;

                calculadora.IniciarNovaConta();
                //calculadora.Infixa = result;
                calculadora.AdicionarAoVetor(result);

                // Tira o foco do botão
                txtVisor.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Serve para colocar funções após os operadores na notação interna à calculadora. Ex: log(2 + 3) -> (2 + 3)log
        private Pilha<String> operacoesAColocar = new Pilha<string>();
        // Quando o número chega a 0, a operação pode ser colocada. Isso permite expressões como log(2 + 3 * (5 -2))
        private Pilha<int> quandoColocarOperacao = new Pilha<int>();

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

            if (btn.Text.Contains('!'))
                //calculadora.Infixa += " " + toAdd;
                calculadora.AdicionarAoVetor(toAdd);
            else if (btn.Text.Contains("log") || btn.Text.Contains("√"))
            {
                //if (calculadora.Infixa.Length > 0)
                //    calculadora.Infixa += " (";
                //else
                //    calculadora.Infixa += "(";
                calculadora.AdicionarAoVetor("(");
                operacoesAColocar.Empilhar(btn.Text);
                quandoColocarOperacao.Empilhar(1);
            }
            else
                //calculadora.Infixa += toAdd;
                calculadora.AdicionarAoVetor(toAdd);
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
            //if (calculadora.Infixa.Length > 0 && calculadora.Infixa[calculadora.Infixa.Length - 1] == '(')
            //    calculadora.Infixa += " ";
            //calculadora.Infixa += toAdd;
            calculadora.AdicionarAoVetor(toAdd);
            HabilitarBotoes();
            AtualizarVisor();

            txtVisor.Focus();
        }

        private void btnC_Click(object sender, EventArgs e)
        {
            infixaMostrada = "";
            //calculadora.Infixa = "";
            calculadora.ExcluirVetor();
            HabilitarBotoes();
            AtualizarVisor();
        }

        private void btnApagar_Click(object sender, EventArgs e)
        {
            char ultimoCaractere = ' ';
            do
            {
                infixaMostrada = infixaMostrada.Substring(0, infixaMostrada.Length - 1);
                if (infixaMostrada.Length > 0)
                    ultimoCaractere = infixaMostrada[infixaMostrada.Length - 1];
                else
                    break;
            } while (ultimoCaractere.Equals(' '));

            if (ultimoCaractere == '√')
                infixaMostrada = infixaMostrada.Substring(0, infixaMostrada.Length - 1);
            else if (ultimoCaractere == 'g' && infixaMostrada.Length > 2 && infixaMostrada.Substring(infixaMostrada.Length - 3) == "log")
                infixaMostrada = infixaMostrada.Substring(0, infixaMostrada.Length - 3);

            char qualApagou = ' ';
            do
            {
                qualApagou = calculadora.Infixa[calculadora.Infixa.Length - 1];

                if (qualApagou == '√')
                {
                    operacoesAColocar.Empilhar("√");
                    quandoColocarOperacao.Empilhar(1);

                    calculadora.Infixa = calculadora.Infixa.Substring(0, calculadora.Infixa.Length - 1);
                }
                else if (qualApagou == 'g' && calculadora.Infixa.Length > 2 && calculadora.Infixa.Substring(calculadora.Infixa.Length - 3) == "log")
                {
                    operacoesAColocar.Empilhar("log");
                    quandoColocarOperacao.Empilhar(1);

                    calculadora.Infixa = calculadora.Infixa.Substring(0, calculadora.Infixa.Length - 3);
                }

                calculadora.Infixa = calculadora.Infixa.Substring(0, calculadora.Infixa.Length - 1);

                if (calculadora.Infixa.Length > 0)
                    ultimoCaractere = calculadora.Infixa[calculadora.Infixa.Length - 1];
                else
                    break;
            } while (ultimoCaractere.Equals(' '));

            if (qualApagou == '(' && !operacoesAColocar.EstaVazia())
            {
                quandoColocarOperacao.Empilhar(quandoColocarOperacao.Pop() - 1); // Significa que a operação está mais próxima de ser colocada
                if (quandoColocarOperacao.Topo == 0) // Quer dizer que o parênteses que deu origem à operação - Ex: log( - foi apagado
                {
                    operacoesAColocar.Pop();
                    quandoColocarOperacao.Pop();
                }
            }
            else if (qualApagou == ')' && !operacoesAColocar.EstaVazia()) // Afasta a operação de poder ser colocada
                quandoColocarOperacao.Empilhar(quandoColocarOperacao.Pop() + 1);

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

            // Tira o foco dos botões
            txtVisor.Focus();
        }
        private void HabilitarBotoes()
        {
            string[] coisas = infixaMostrada.Split(' ');
            string ultimaCoisa = coisas[coisas.Length - 1];
            if (ultimaCoisa.Length > 1)
                ultimaCoisa = ultimaCoisa.Substring(ultimaCoisa.Length - 1);
            if (!ultimaCoisa.Equals("") && IsNumeric(ultimaCoisa[ultimaCoisa.Length - 1].ToString()))
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
                        btnLog.Enabled = btnSqrt.Enabled = false;
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
                //calculadora.Infixa += " " + btn.Text + " ";
                calculadora.AdicionarAoVetor(btn.Text);

                if (!quandoColocarOperacao.EstaVazia()) // Afasta a colocação da operação
                    quandoColocarOperacao.Empilhar(quandoColocarOperacao.Pop() + 1);
            }
            else if (btn.Text.Equals("("))
            {
                infixaMostrada += btn.Text;
                //calculadora.Infixa += btn.Text + " ";
                calculadora.AdicionarAoVetor(btn.Text);
            }
            else if (btn.Text.Equals(")"))
            {
                infixaMostrada += btn.Text;
                //calculadora.Infixa += " " + btn.Text;
                calculadora.AdicionarAoVetor(btn.Text);

                if (!quandoColocarOperacao.EstaVazia()) // Aproxima a colocação da operação
                {
                    quandoColocarOperacao.Empilhar(quandoColocarOperacao.Pop() - 1);
                    if (quandoColocarOperacao.Topo == 0) // Quando chega a 0, a operação deve ser colocada, e suas informações podem ser retiradas das pilhas
                    {
                        //calculadora.Infixa += " " + operacoesAColocar.Pop();
                        calculadora.AdicionarAoVetor(operacoesAColocar.Pop());
                        quandoColocarOperacao.Pop();
                    }
                }
            }
            HabilitarBotoes();
            AtualizarVisor();
        }
        private bool IsNumeric(string str)
        {
            return double.TryParse(str, out double n);
        }

        private void FrmCalculadora_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
                btnApagar.PerformClick();
            else if (e.KeyCode == Keys.Enter)
                btnIgual.PerformClick();
        }

        private void FrmCalculadora_KeyPress(object sender, KeyPressEventArgs e)
        {
            char caracter = e.KeyChar;
            if (IsNumeric(caracter.ToString()))
                (Controls.Find("btn" + caracter, false)[0] as Button).PerformClick();
            else if (caracter == '=')
                btnIgual.PerformClick();
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

        private void BtnCE_Click(object sender, EventArgs e)
        {
            ZerarUltimoNumeroInfixaMostrada();
            ZerarUltimoNumeroInfixaOculta();
            
            HabilitarBotoes();
            AtualizarVisor();
        }
        private void ZerarUltimoNumeroInfixaMostrada()
        {
            // Quebra as partes da infixa exibida no visor através do espaço
            var partesInfixaMostrada = infixaMostrada.Split(' ');

            // Percorre as partes do final até o começo. Quando o último elemento numérico é zerado, o loop para
            for (int i = partesInfixaMostrada.Length - 1; i >= 0; i--)
            {
                var parte = "";
                var jaColocouZero = false;

                // Aquela parte pode não ser inteiramente número ou operador. Pode existir "*(26", por exemplo
                // Por isso, cada caracter da parte é percorrido, sendo colocado numa nova string (parte) se for um operador
                // Ou, se indicar um número e o 0 ainda não tiver sido colocado
                for (int j = 0; j < partesInfixaMostrada[i].Length; j++)
                    if (IsNumeric(partesInfixaMostrada[i][j].ToString()) || partesInfixaMostrada[i][j] == ',') // É número ou quebra um número
                    {
                        if (!jaColocouZero)
                        {
                            parte += "0";
                            jaColocouZero = true;
                        }
                    }
                    else // É operador, pode ser colocado onde estava na nova string
                        parte += partesInfixaMostrada[i][j];

                partesInfixaMostrada[i] = parte; // A parte é substituída por uma nova string. Com o exemplo anterior, seria "*(0"

                if (jaColocouZero) // Nesse ponto, indica que o último elemento numérico já virou 0
                    break;
            }
            infixaMostrada = string.Join(" ", partesInfixaMostrada); // Junta as partes novamente na infixa mostrada
        }
        private void ZerarUltimoNumeroInfixaOculta()
        {
            // Quebra as partes da infixa interna através do espaço
            var partesInfixa = calculadora.Infixa;

            // Ao contrário da infixa mostrada no visor, a infixa interna nunca tem mais de um elemento não separado por espaço
            // Por isso, cada parte será apenas ou operador ou número
            for (int i = partesInfixa.Length - 1; i >= 0; i--)
                if (IsNumeric(partesInfixa[i])) // Percorre do final ao começo até achar um número, que é substituído por 0
                {
                    partesInfixa[i] = "0";
                    break;
                }

            calculadora.Infixa = partesInfixa; // Junta as partes novamente na infixa interna
        }

        private void BtnHistorico_Click(object sender, EventArgs e)
        {
            historico.Show();

            txtVisor.Focus();
            historico.Focus();
        }
    }
}