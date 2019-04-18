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
        Button[,] botoes = new Button[5, 5];

        string operacaoInfixa;
        public frmCalculadora()
        {
            InitializeComponent();
            lblPosFixa.AutoSize = true;

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
            lblPosFixa.MaximumSize = new Size(Width, 0);
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
