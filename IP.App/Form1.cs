using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;


namespace IP.App
{
    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;
        byte[] buffer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
      
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            txtLocalIP.Text = GetLocalIP();
            txtRemoteIP.Text = GetLocalIP();

        }

        private string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();

            }
            return "127.0.0.1";
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            //Binding socket
            epLocal = new IPEndPoint(IPAddress.Parse(txtLocalIP.Text), Convert.ToInt32(txtLocalPort.Text));
            sck.Bind(epLocal);

            //Connecting to remote IP
            epRemote = new IPEndPoint(IPAddress.Parse(txtRemoteIP.Text), Convert.ToInt32(txtRemotePort.Text));
            sck.Connect(epRemote);

            //Listening the specific port
            buffer = new byte[1500];
            sck.BeginReceiveMessageFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);


        }
        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                byte[] ReceivedData = new byte[1500];
                ReceivedData = (byte[])aResult.AsyncState;

                //converting byte to string

                ASCIIEncoding aEncoding = new ASCIIEncoding();
                string ReceivedMessage = aEncoding.GetString(ReceivedData);


                //Adding message to Listbox

                listMessage.Items.Add("Friend: " + ReceivedMessage);

                buffer = new byte[1500];
                sck.BeginReceiveMessageFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            //convert string message to byte
            ASCIIEncoding aEncoding = new ASCIIEncoding();
            byte[] SendingMessage = new byte[1500];
            SendingMessage = aEncoding.GetBytes(txtMessage.Text);


            //Sending the encoded message
            sck.Send(SendingMessage);

            //adding to the list box

            listMessage.Items.Add("Me: " + txtMessage.Text);
            txtMessage.Text = "";


        }
    }
}
