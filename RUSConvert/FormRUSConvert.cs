using RUSConvert.CODAPmt;
using RUSConvert.Shared;
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
            textBoxCommunication.Text = Properties.Settings.Default.DefaultCommunication;
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

        private async void ButtonConvertInvoices_Click(object sender, EventArgs e)
        {
            await RunTwizzit2UBL();
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

        private async void ButtonConvertPayments_Click(object sender, EventArgs e)
        {
            await RunTwizzit2CODAPmt();
        }

        private async Task RunTwizzit2UBL()
        {
            var progress = new Progress<JobProgress>();
            progress.ProgressChanged += (s, message) =>
            {
                if (!progressBarUBL.Visible) progressBarUBL.Show();
                if (progressBarUBL.Maximum != message.Max)
                {
                    progressBarUBL.Maximum = message.Max;
                }
                progressBarUBL.Value = message.Value;
                labelStatusInvoices.Text = message.Text;
            };
            var job = new Twizzit2UBL(progress);
            var result = await Task.Run(() => job.Convert(labelInvoices.Text, DateOnly.FromDateTime(DateTimeInvoices.Value.Date)));
            labelStatusInvoices.Text = result.Messages[0] ?? "";
        }

        private async Task RunTwizzit2CODAPmt()
        {
            var progress = new Progress<JobProgress>();
            progress.ProgressChanged += (s, message) =>
            {
                if (!progressBarCODA.Visible) progressBarCODA.Show();
                if (progressBarCODA.Maximum != message.Max)
                {
                    progressBarCODA.Maximum = message.Max;
                }
                progressBarCODA.Value = message.Value;
                labelStatusPayments.Text = message.Text;
            };
            var job = new Twizzit2CODAPmt(progress);
            var result = await Task.Run(() => job.Convert(labelPayments.Text, DateTimeEnvelop.Value.Date, textBoxCommunication.Text));
            labelStatusPayments.Text = result.Messages[0] ?? "";
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
    internal sealed class FormSettings : ApplicationSettingsBase
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