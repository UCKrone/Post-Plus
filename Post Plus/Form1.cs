using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Post_Plus
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private const int cGrip = 16;      // Grip size
        private const int cCaption = 32;   // Caption bar height;

        int windowWidth = 0;
        int windowHeight = 0;

        int windowTop = 0;
        int windowLeft = 0;

        private void btnExpand_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Normal;

                this.Height = windowHeight;
                this.Width = windowWidth;

                this.Top = windowTop;
                this.Left = windowLeft;

                btnExpand.BackgroundImage = Properties.Resources.Expandd;
            }
            else
            {
                windowHeight = this.Height;
                windowWidth = this.Width;

                windowTop = this.Top;
                windowLeft = this.Left;

                var workingArea = Screen.FromHandle(Handle).WorkingArea;
                MaximizedBounds = new Rectangle(0, 0, workingArea.Width, workingArea.Height);
                WindowState = FormWindowState.Maximized;

                this.Bounds = Screen.PrimaryScreen.Bounds;

                btnExpand.BackgroundImage = Properties.Resources.Retractt;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            //Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
            //ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rc);
            //rc = new Rectangle(0, 0, this.ClientSize.Width, cCaption);
            //e.Graphics.FillRectangle(Brushes.DarkBlue, rc);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                btnExpand.BackgroundImage = Properties.Resources.Retractt;
            }
            else
            {
                btnExpand.BackgroundImage = Properties.Resources.Expandd;
            }
            if (m.Msg == 0x84)
            {  // Trap WM_NCHITTEST
                //Point pos = new Point(m.LParam.ToInt32());
                //pos = this.PointToClient(pos);
                //if (pos.Y < cCaption)
                //{
                //    m.Result = (IntPtr)2;  // HTCAPTION
                //    return;
                //}
                //if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                //{
                //    m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                //    return;
                //}
            }
            base.WndProc(ref m);
        }

        private static readonly HttpClient client = new HttpClient();

        private async void btnPost_Click(object sender, EventArgs e)
        {
            try
            {
                var values = new Dictionary<string, string> { };

                if (postType1.Text != "" && postValue1.Text != "")
                    values.Add(postType1.Text, postValue1.Text);
                if (postType2.Text != "" && postValue2.Text != "")
                    values.Add(postType2.Text, postValue2.Text);
                if (postType3.Text != "" && postValue3.Text != "")
                    values.Add(postType3.Text, postValue3.Text);
                if (postType4.Text != "" && postValue4.Text != "")
                    values.Add(postType4.Text, postValue4.Text);
                if (postType5.Text != "" && postValue5.Text != "")
                    values.Add(postType5.Text, postValue5.Text);
                if (postType6.Text != "" && postValue6.Text != "")
                    values.Add(postType6.Text, postValue6.Text);

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync(tbURL.Text, content);

                var responseString = await response.Content.ReadAsStringAsync();

                rtbReturn.Text = responseString;
            }
            catch(Exception ex)
            {
                rtbReturn.Text = ex.Message;
            }
        }
    }
}
