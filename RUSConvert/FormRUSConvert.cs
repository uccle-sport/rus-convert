﻿using RUSConvert.CODAPmt;
using RUSConvert.UBL;
using System.Configuration;

namespace RUSConvert
{
    public partial class FormRUSConvert : Form
    {
        private readonly FormSettings frmSettings = new();
        public FormRUSConvert()
        {
            InitializeComponent();
        }

        private void ButtonLoadInvoices_Click(object sender, EventArgs e)
        {
            openFileDialog.InitialDirectory = Properties.Settings.Default.InvoicesSourceFolder;
            openFileDialog.Filter = "Fichiers Excel (*.xlsx)|*.xlsx";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                labelInvoices.Text = openFileDialog.FileName;
            }
        }

        private void ButtonConvertInvoices_Click(object sender, EventArgs e)
        {
            var result = Twizzit2UBL.Convert(labelInvoices.Text);
            labelStatusInvoices.Text = result.Messages[0];
        }


        private void ButtonLoadPayments_Click(object sender, EventArgs e)
        {
            openFileDialog.InitialDirectory = Properties.Settings.Default.PaymentsSourceFolder;
            openFileDialog.Filter = "Fichiers Excel (*.xlsx)|*.xlsx";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                labelPayments.Text = openFileDialog.FileName;
            }
        }

        private void ButtonConvertPayments_Click(object sender, EventArgs e)
        {
            var result = Twizzit2CODAPmt.Convert(labelPayments.Text, DateTimeEnvelop.Value.Date, textBoxCommunication.Text);
            labelStatusPayments.Text = result.Messages[0];
        }

        private void FormRUSConvert_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
            }
        }
        private void NotifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }
        private void FormRUSConvert_Load(object sender, EventArgs e)
        {
            FormClosing += new FormClosingEventHandler(FormRUSConvert_FormClosing!);
            Binding bndLocation = new("Location", frmSettings,
                "FormLocation", true, DataSourceUpdateMode.OnPropertyChanged);
            DataBindings.Add(bndLocation);
            Size = frmSettings.FormSize;
        }
        private void FormRUSConvert_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmSettings.FormSize = this.Size;
            frmSettings.Save();
            notifyIcon.Visible = false;
        }
    }

    //Application settings wrapper class
    sealed class FormSettings : ApplicationSettingsBase
    {
        [UserScopedSetting()]
        [DefaultSettingValue("0, 0")]
        public Point FormLocation
        {
            get { return (Point)(this[nameof(FormLocation)]); }
            set { this[nameof(FormLocation)] = value; }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("225, 200")]
        public Size FormSize
        {
            get { return (Size)this[nameof(FormSize)]; }
            set { this[nameof(FormSize)] = value; }
        }
    }
}
