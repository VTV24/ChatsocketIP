using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace Server
{
    public partial class Server : Form
    {

        IPEndPoint ip;
        Socket server;
        List<Socket> clientList = new List<Socket>();
        public Server()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Connect();
        }
        private void btn_Send_Click(object sender, EventArgs e)
        {
            foreach (var item in clientList)
            {
                Send(item);
            }
            textBox1.Text = "";
        }

        void Connect()
        {
            ip = new IPEndPoint(IPAddress.Any, 2000);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            //doi ket noi tu client
            server.Bind(ip);

            Thread listen = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        server.Listen(100);
                        Socket client = server.Accept();
                        clientList.Add(client);

                        Thread receive = new Thread(Receive);
                        receive.IsBackground = true;
                        receive.Start(client);
                    }
                }
                catch
                {
                    ip = new IPEndPoint(IPAddress.Any, 2000);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                }
            });
            listen.IsBackground = true;
            listen.Start();



        }
        /// <summary>
        /// dong ket noi toi server
        /// </summary>
        void CloseConnect()
        {
            server.Close();
        }
        /// <summary>
        /// gui du lieu di
        /// </summary>
        void Send(Socket client)
        {
            if (textBox1.Text != "")
                client.Send(Serialize(textBox1.Text));
        }
        /// <summary>
        /// nhan du lieu toi
        /// </summary>
        void Receive(object obj)
        {
            Socket client = obj as Socket;
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
                clientList.Remove(client);
                client.Close();
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
        /// <summary>
        /// them message vao khung chat
        /// </summary>
        /// <param name="message"></param>
        void AddMessage(string message)
        {
            listView1.Items.Add(new ListViewItem(message));
        }
    }
}
