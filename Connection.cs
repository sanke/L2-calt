using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace l2cAlt
{
    public class SocketPacket
    {
        public Socket socket_;
        public byte[] data_buffer_ = new byte[65533];
        
    }

    public class Connection
    {
        private OutputDebug output_;
        private IPEndPoint end_point_;
        public Socket socket_;
        //private ManualResetEvent manual_reset_event_;
        private AsyncCallback callback_;
        private IAsyncResult async_result_;
        private PacketForge packet_forge_;
        public Connection()
        {
            output_ = OutputDebug.get_instance();

            packet_forge_ = new PacketForge(this);
            //manual_reset_event_ = new ManualResetEvent(false);
        }

        public void connect(string addr, int port)
        {
            try
            {
                end_point_ = new IPEndPoint(IPAddress.Parse(addr), port);
                socket_ = new Socket(end_point_.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket_.Connect(end_point_);
                if (socket_.Connected)
                {
                    output_.output_debug("-client: connection to server established\n");
                    wait_for_data();
                }
                else
                {
                    output_.output_debug("-client: can't connect to login server\n");
                    return;
                }
            }
            catch (SocketException se)
            {
                output_.output_debug("-client: " + se.Message + "\n");
            }
        }

        public void wait_for_data()
        {
            try
            {
                if (callback_ == null)
                    callback_ = new AsyncCallback(packet_received);
                SocketPacket socket_packet = new SocketPacket();
                socket_packet.socket_ = socket_;
                
                async_result_ = socket_.BeginReceive(socket_packet.data_buffer_,
                    0,
                    socket_packet.data_buffer_.Length,
                    SocketFlags.None,
                    callback_,
                    socket_packet);
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\nOnDataReceived: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        public void send_packet(byte[] packet_data, int send_size,Socket socket)
        {
            try
            {
                socket.BeginSend(packet_data, 0, send_size, SocketFlags.None, new AsyncCallback(packet_send), socket_);
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
        }
        private void packet_send(IAsyncResult ar)
        {
            int send_size;
            Socket socket = (Socket)ar.AsyncState;
            send_size = socket.EndSend(ar);
            output_.output_debug("-client: packet sent with lenght of " + send_size + ".\n");
        }
        private void packet_received(IAsyncResult ar)
        {
            try
            {
                int recv_size;
                SocketPacket socket_packet = (SocketPacket)ar.AsyncState;
                recv_size = socket_packet.socket_.EndReceive(ar);

                if (recv_size == 0)
                {
                    output_.output_debug("-client: connection closed.");
                    socket_packet.socket_.Close();
                    return;
                }
                
                output_.output_debug("-server: data packet[" + recv_size + "]: ");
                output_.output_debug(socket_packet.data_buffer_, recv_size);
                output_.output_debug("\n-server: packet end.\n");

                packet_forge_.process_packet(socket_packet.data_buffer_, recv_size, socket_packet.socket_);
                
                wait_for_data();
                
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\nOnDataReceived: Socket has been closed\n");
            }
        }
    }
}
