using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    public partial class Client : Form
    {
        IPEndPoint ip;
        Socket client;

        public Client()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Connect();
        }
        private void btn_Send_Click(object sender, EventArgs e)
        {
            Send();
            textBox1.Text = "";
        }
        private void Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseConnect();
        }
        /// <summary>
        /// tao ket noi toi server
        /// </summary>
        void Connect()
        {
            try
            {
                ip = new IPEndPoint(IPAddress.Parse("192.168.1.5"), 2000);
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                client.Connect(ip);
            }
            catch
            {
                MessageBox.Show("Server Nonreponsive", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Thread listen = new Thread(Receive);
            listen.IsBackground = true;
            listen.Start();

        }
        /// <summary>
        /// dong ket noi toi server
        /// </summary>
        void CloseConnect()
        {
            client.Close();
        }
        /// <summary>
        /// gui du lieu di
        /// </summary>
        void Send()
        {
            if (textBox1.Text != "")
            {
                client.Send(Serialize(textBox1.Text));
            }
        }
        /// <summary>
        /// nhan du lieu toi
        /// </summary>
        void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    client.Receive(data);

                    string message = (string)Deserialize(data);
                    AddMessage(message);
                }
            }
            catch
            {
                CloseConnect();
            }
        }
        /// <summary>
        /// phan manh du lieu
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        byte[] Serialize(string obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, obj);

            return stream.ToArray();
        }
        /// <summary>
        /// ghep manh du lieu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();

            return formatter.Deserialize(stream);
        }

        void AddMessage(string message)
        {
            listView1.Items.Add(new ListViewItem(message));
        }
    }
}
