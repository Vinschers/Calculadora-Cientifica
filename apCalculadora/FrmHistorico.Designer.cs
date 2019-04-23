namespace apCalculadora
{
    partial class FrmHistorico
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmHistorico));
            this.lsbHistorico = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnApagarHistorico = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lsbHistorico
            // 
            this.lsbHistorico.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsbHistorico.FormattingEnabled = true;
            this.lsbHistorico.ItemHeight = 22;
            this.lsbHistorico.Location = new System.Drawing.Point(14, 46);
            this.lsbHistorico.Name = "lsbHistorico";
            this.lsbHistorico.Size = new System.Drawing.Size(352, 378);
            this.lsbHistorico.TabIndex = 0;
            this.lsbHistorico.Resize += new System.EventHandler(this.LsbHistorico_Resize);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(352, 29);
            this.label1.TabIndex = 1;
            this.label1.Text = "Histórico";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnApagarHistorico
            // 
            this.btnApagarHistorico.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnApagarHistorico.Location = new System.Drawing.Point(132, 432);
            this.btnApagarHistorico.Name = "btnApagarHistorico";
            this.btnApagarHistorico.Size = new System.Drawing.Size(116, 38);
            this.btnApagarHistorico.TabIndex = 2;
            this.btnApagarHistorico.Text = "Apagar";
            this.btnApagarHistorico.UseVisualStyleBackColor = true;
            this.btnApagarHistorico.Click += new System.EventHandler(this.BtnApagarHistorico_Click);
            // 
            // FrmHistorico
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(378, 478);
            this.Controls.Add(this.btnApagarHistorico);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lsbHistorico);
            this.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmHistorico";
            this.Text = "Histórico da Calculadora";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lsbHistorico;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnApagarHistorico;
    }
}