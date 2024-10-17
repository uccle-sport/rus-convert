namespace RUSConvert
{
    partial class FormRUSConvert
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRUSConvert));
            notifyIcon = new NotifyIcon(components);
            openFileDialog = new OpenFileDialog();
            tabControl = new TabControl();
            tabPageUBL = new TabPage();
            progressBarUBL = new ProgressBar();
            labelStatusInvoices = new Label();
            labelInvoices = new Label();
            ButtonLoadInvoices = new Button();
            ButtonConvertInvoices = new Button();
            tabPageCODA = new TabPage();
            progressBarCODA = new ProgressBar();
            DateTimeEnvelop = new DateTimePicker();
            labelCommunication = new Label();
            textBoxCommunication = new TextBox();
            labelStatusPayments = new Label();
            labelPayments = new Label();
            ButtonLoadPayments = new Button();
            ButtonConvertPayments = new Button();
            tabControl.SuspendLayout();
            tabPageUBL.SuspendLayout();
            tabPageCODA.SuspendLayout();
            SuspendLayout();
            // 
            // notifyIcon
            // 
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.BalloonTipText = "Convert";
            notifyIcon.BalloonTipTitle = "RUS";
            notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
            notifyIcon.Text = "RUS Convert";
            notifyIcon.DoubleClick += NotifyIcon1_DoubleClick;
            // 
            // openFileDialog
            // 
            openFileDialog.FileName = "openFileDialog";
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabPageUBL);
            tabControl.Controls.Add(tabPageCODA);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 0);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(704, 170);
            tabControl.TabIndex = 45;
            // 
            // tabPageUBL
            // 
            tabPageUBL.Controls.Add(progressBarUBL);
            tabPageUBL.Controls.Add(labelStatusInvoices);
            tabPageUBL.Controls.Add(labelInvoices);
            tabPageUBL.Controls.Add(ButtonLoadInvoices);
            tabPageUBL.Controls.Add(ButtonConvertInvoices);
            tabPageUBL.Location = new Point(4, 24);
            tabPageUBL.Name = "tabPageUBL";
            tabPageUBL.Padding = new Padding(3);
            tabPageUBL.Size = new Size(696, 142);
            tabPageUBL.TabIndex = 0;
            tabPageUBL.Text = "UBL (Factures)";
            tabPageUBL.UseVisualStyleBackColor = true;
            // 
            // progressBarUBL
            // 
            progressBarUBL.Location = new Point(12, 109);
            progressBarUBL.Name = "progressBarUBL";
            progressBarUBL.Size = new Size(669, 19);
            progressBarUBL.TabIndex = 49;
            // 
            // labelStatusInvoices
            // 
            labelStatusInvoices.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelStatusInvoices.Location = new Point(220, 82);
            labelStatusInvoices.Name = "labelStatusInvoices";
            labelStatusInvoices.Size = new Size(461, 19);
            labelStatusInvoices.TabIndex = 48;
            // 
            // labelInvoices
            // 
            labelInvoices.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelInvoices.Location = new Point(236, 17);
            labelInvoices.Name = "labelInvoices";
            labelInvoices.Size = new Size(445, 19);
            labelInvoices.TabIndex = 47;
            // 
            // ButtonLoadInvoices
            // 
            ButtonLoadInvoices.Location = new Point(12, 15);
            ButtonLoadInvoices.Name = "ButtonLoadInvoices";
            ButtonLoadInvoices.Size = new Size(202, 23);
            ButtonLoadInvoices.TabIndex = 46;
            ButtonLoadInvoices.Text = "Télécharger fichier XLSX";
            ButtonLoadInvoices.UseVisualStyleBackColor = true;
            ButtonLoadInvoices.Click += ButtonLoadInvoices_Click;
            // 
            // ButtonConvertInvoices
            // 
            ButtonConvertInvoices.Location = new Point(12, 80);
            ButtonConvertInvoices.Name = "ButtonConvertInvoices";
            ButtonConvertInvoices.Size = new Size(202, 23);
            ButtonConvertInvoices.TabIndex = 45;
            ButtonConvertInvoices.Text = "Lancer conversion";
            ButtonConvertInvoices.UseVisualStyleBackColor = true;
            ButtonConvertInvoices.Click += ButtonConvertInvoices_Click;
            // 
            // tabPageCODA
            // 
            tabPageCODA.Controls.Add(progressBarCODA);
            tabPageCODA.Controls.Add(DateTimeEnvelop);
            tabPageCODA.Controls.Add(labelCommunication);
            tabPageCODA.Controls.Add(textBoxCommunication);
            tabPageCODA.Controls.Add(labelStatusPayments);
            tabPageCODA.Controls.Add(labelPayments);
            tabPageCODA.Controls.Add(ButtonLoadPayments);
            tabPageCODA.Controls.Add(ButtonConvertPayments);
            tabPageCODA.Location = new Point(4, 24);
            tabPageCODA.Name = "tabPageCODA";
            tabPageCODA.Padding = new Padding(3);
            tabPageCODA.Size = new Size(696, 142);
            tabPageCODA.TabIndex = 1;
            tabPageCODA.Text = "CODA (Enveloppe)";
            tabPageCODA.UseVisualStyleBackColor = true;
            // 
            // progressBarCODA
            // 
            progressBarCODA.Location = new Point(12, 109);
            progressBarCODA.Name = "progressBarCODA";
            progressBarCODA.Size = new Size(669, 19);
            progressBarCODA.TabIndex = 57;
            // 
            // DateTimeEnvelop
            // 
            DateTimeEnvelop.Format = DateTimePickerFormat.Short;
            DateTimeEnvelop.Location = new Point(236, 50);
            DateTimeEnvelop.Name = "DateTimeEnvelop";
            DateTimeEnvelop.Size = new Size(97, 23);
            DateTimeEnvelop.TabIndex = 56;
            // 
            // labelCommunication
            // 
            labelCommunication.AutoSize = true;
            labelCommunication.Location = new Point(350, 54);
            labelCommunication.Name = "labelCommunication";
            labelCommunication.Size = new Size(94, 15);
            labelCommunication.TabIndex = 54;
            labelCommunication.Text = "Communication";
            // 
            // textBoxCommunication
            // 
            textBoxCommunication.Location = new Point(450, 50);
            textBoxCommunication.Name = "textBoxCommunication";
            textBoxCommunication.Size = new Size(231, 23);
            textBoxCommunication.TabIndex = 53;
            // 
            // labelStatusPayments
            // 
            labelStatusPayments.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelStatusPayments.Location = new Point(220, 82);
            labelStatusPayments.Name = "labelStatusPayments";
            labelStatusPayments.Size = new Size(461, 19);
            labelStatusPayments.TabIndex = 52;
            // 
            // labelPayments
            // 
            labelPayments.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelPayments.Location = new Point(236, 17);
            labelPayments.Name = "labelPayments";
            labelPayments.Size = new Size(445, 19);
            labelPayments.TabIndex = 51;
            // 
            // ButtonLoadPayments
            // 
            ButtonLoadPayments.Location = new Point(12, 15);
            ButtonLoadPayments.Name = "ButtonLoadPayments";
            ButtonLoadPayments.Size = new Size(202, 23);
            ButtonLoadPayments.TabIndex = 50;
            ButtonLoadPayments.Text = "Télécharger fichier XLSX";
            ButtonLoadPayments.UseVisualStyleBackColor = true;
            ButtonLoadPayments.Click += ButtonLoadPayments_Click;
            // 
            // ButtonConvertPayments
            // 
            ButtonConvertPayments.Location = new Point(12, 80);
            ButtonConvertPayments.Name = "ButtonConvertPayments";
            ButtonConvertPayments.Size = new Size(202, 23);
            ButtonConvertPayments.TabIndex = 49;
            ButtonConvertPayments.Text = "Lancer conversion";
            ButtonConvertPayments.UseVisualStyleBackColor = true;
            ButtonConvertPayments.Click += ButtonConvertPayments_Click;
            // 
            // FormRUSConvert
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(704, 170);
            Controls.Add(tabControl);
            Name = "FormRUSConvert";
            Text = "RUS Convert";
            Load += FormRUSConvert_Load;
            Resize += FormRUSConvert_Resize;
            tabControl.ResumeLayout(false);
            tabPageUBL.ResumeLayout(false);
            tabPageCODA.ResumeLayout(false);
            tabPageCODA.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private NotifyIcon notifyIcon;
        private OpenFileDialog openFileDialog;
        private TabControl tabControl;
        private TabPage tabPageUBL;
        private Label labelStatusInvoices;
        private Label labelInvoices;
        private Button ButtonLoadInvoices;
        private Button ButtonConvertInvoices;
        private TabPage tabPageCODA;
        private Label labelStatusPayments;
        private Label labelPayments;
        private Button ButtonLoadPayments;
        private Button ButtonConvertPayments;
        private TextBox textBoxCommunication;
        private Label labelCommunication;
        private DateTimePicker DateTimeEnvelop;
        private ProgressBar progressBarCODA;
        private ProgressBar progressBarUBL;
    }
}