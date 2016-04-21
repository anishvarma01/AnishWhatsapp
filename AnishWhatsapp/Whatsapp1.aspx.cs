using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WhatsAppApi;
using WhatsAppApi.Account;

namespace AnishWhatsapp
{
    public partial class Whatsapp1 : System.Web.UI.Page
    {
        Whatsapp WA;
        WhatsUser USER;
        private delegate void UpdateTextBox(TextBox textbox, string value);
        public void UpdateDataTextBox(TextBox textbox, string value)
        {
            textbox.Text += value;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox2.Text))
                return;
            if(WA != null)
            {
                if(WA.ConnectionStatus == ApiBase.CONNECTION_STATUS.LOGGEDIN)
                {
                    WA.SendMessage(TextBox1.Text, TextBox2.Text);
                    TextBox3.Text += string.Format("\r\n{0} :{1}", USER.Nickname, TextBox2.Text);
                    TextBox2.Clear();
                    TextBox2.Focus();
                }
            }

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            WhatsUserManager Manage = new WhatsUserManager();
            USER = Manage.CreateUser(TextBox5.Text, TextBox6.Text);
            var thread = new Thread(testc =>
            {
                UpdateTextBox textbox = UpdateDataTextBox;
                WA = new Whatsapp(TextBox5.Text, TextBox6.Text, TextBox4.Text, true);
                WA.OnConnectSuccess += () =>
                {
                    if (TextBox3.InvokeRequired)
                        Invoke(textbox, TextBox3, "Connected....");
                    WA.OnLoginSuccess += (phone, data) =>
                    {
                        if (TextBox3.InvokeRequired)
                            invoke(textbox, TextBox3, "\r\nLogin Success....");
                        while (WA != null)
                        {
                            WA = PollMessage();
                            Thread.Sleep(200);
                            continue;
                        }
                    };
                    WA.OnGetMessage += (node, from, id, name, message, receipt_send) =>
                    {
                        if (TextBox3.InvokeRequired)
                            invoke(textbox, TextBox3, string.Format("\r\n{0}:{1}", name, message));
                    };
                    WA.OnLoginFailed += (data) =>
                    {
                        if (TextBox3.InvokeRequired)
                            invoke(textbox, TextBox3, string.Format("\r\nConnect failed: {0}", data));
                    };
                    WA.Login();
                };
                WA.OnConnectFailed += (ex) =>
                {
                    if (TextBox3.InvokeRequired)
                        invoke(textbox, TextBox3, string.Format("\r\nConnection Failed {0}", ex.StactTrace));

                };
                           

               
                    WA.Connect();
                }) { IsBackground = true };
            thread.Start();
        }
    }
}