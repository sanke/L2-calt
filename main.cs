#define DEBUG

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;


namespace l2cAlt
{
 

    public partial class main : Form
    {
        [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        //bf key for decrypting,encrypting data taken from l2j
        //(byte) 0x6b, (byte) 0x60, (byte) 0xcb, (byte) 0x5b,
        //(byte) 0x82, (byte) 0xce, (byte) 0x90, (byte) 0xb1,
        //(byte) 0xcc, (byte) 0x2b, (byte) 0x6c, (byte) 0x55,
        //(byte) 0x6c, (byte) 0x6c, (byte) 0x6c, (byte) 0x6c

        const int WM_VSCROLL = 277;
        const int SB_BOTTOM = 7;

        byte[] blowfish_key_ = {   0x6B, 0x60, 0xCB, 0x5B, 0x82,
                                   0xCE, 0x90, 0xB1, 0xCC, 0x2B,
                                   0x6C, 0x55, 0x6C, 0x6C, 0x6C, 
                                   0x6C };

        byte[] RSA_key_ = new byte[4];

        private BlowfishEngine blowfish_cipher_ = new BlowfishEngine();
        
        private TcpClient l2_client_ = new TcpClient();
        //private Stream recv_send_stream_;
        private Connection connection_;

        private byte[] data_buffer_ = new byte[65533];
        private byte[] decoded_buffer_ = new byte[65533];
        private byte[] RSA_enc_key_ = new byte[128];
        
        private OutputDebug output_;
        
        public main()
        {
            InitializeComponent();
            output_ = OutputDebug.get_instance();
            output_.set_output(dlgOutput);
            
        }

        // Connect button click
        private void dlgConnect_Click(object sender, EventArgs e)
        {
            connection_ = new Connection();
            connection_.connect("83.171.11.57", 2106);//178.162.166.11

//            l2_client_.Connect("178.162.166.11", 2106);
//            recv_send_stream_ = l2_client_.GetStream();
//            int byte_count = recv_send_stream_.Read(data_buffer_, 0, 65533);

//#if DEBUG
//            output_.output_debug("-client-recv[" + byte_count + "]: ");
//            output_.output_debug(data_buffer_, byte_count);
//#endif
//            blowfish_cipher_.init(false, blowfish_key_);
//            blowfish_cipher_.processBigBlock(data_buffer_, 0x02, decoded_buffer_, 0x00, byte_count - 2);

//            Global.decXORPass(decoded_buffer_, 0x00, byte_count - 2,
//                System.BitConverter.ToInt32(decoded_buffer_, byte_count - 10));

//            RSA_key_[0] = decoded_buffer_[1];
//            RSA_key_[1] = decoded_buffer_[2];
//            RSA_key_[2] = decoded_buffer_[3];
//            RSA_key_[3] = decoded_buffer_[4];

//            for (int i = 0; i < 128; i++)
//            {
//                RSA_enc_key_[i] = decoded_buffer_[9 + i];
//            }

//            for (int i = 0; i < 0x40; i++)
//            {
//                RSA_enc_key_[0x40 + i] = (byte)(RSA_enc_key_[0x40 + i] ^ RSA_enc_key_[i]);
//            }

//            for (int i = 0; i < 4; i++)
//            {
//                RSA_enc_key_[0x0d + i] = (byte)(RSA_enc_key_[0x0d + i] ^ RSA_enc_key_[0x34 + i]);
//            }
//            // step 2 : xor first 0x40 bytes with  last 0x40 bytes 
//            for (int i = 0; i < 0x40; i++)
//            {
//                RSA_enc_key_[i] = (byte)(RSA_enc_key_[i] ^ RSA_enc_key_[0x40 + i]);
//            }
//            // step 1 : 0x4d-0x50 <-> 0x00-0x04
//            for (int i = 0; i < 4; i++)
//            {
//                byte temp = RSA_enc_key_[0x00 + i];
//                RSA_enc_key_[0x00 + i] = RSA_enc_key_[0x4D + i];
//                RSA_enc_key_[0x4d + i] = temp;
//            }


//#if DEBUG

//            output_.output_debug("-client-recv-dec: ");
//            output_.output_debug(decoded_buffer_, byte_count);
//#endif
//            for (int i = 0; i < 16; i++)
//                blowfish_key_[i] = decoded_buffer_[i + 153];
//#if DEBUG
//            output_.output_debug("-client-new-blowfish-key: ");
//            output_.output_debug(blowfish_key_, 16);
//            output_.output_debug("-client-RSA-key: ");
//            output_.output_debug(RSA_enc_key_, 128);
//            output_.output_debug("-client-GameGuard-packet");
//#endif
//            // wtf Oo (?)
  

//            byte[] send_packet = new byte[40];
//            byte[] pre_send_packet = new byte[40];

//            send_packet[0] = 0x07; // Request GG packet
//            send_packet[1] = RSA_key_[0];
//            send_packet[2] = RSA_key_[1];
//            send_packet[3] = RSA_key_[2];
//            send_packet[4] = RSA_key_[3];

//            game_guard_fill(send_packet, 0x05);

//            ulong checksum = Global.CheckSum(send_packet, 0x18);

//            send_packet[24] = (byte)(checksum & 0xff);
//            send_packet[25] = (byte)(checksum >> 0x08 & 0xff);
//            send_packet[26] = (byte)(checksum >> 0x10 & 0xff);
//            send_packet[27] = (byte)(checksum >> 0x18 & 0xff);

//            blowfish_cipher_.init(true, blowfish_key_);
//            blowfish_cipher_.processBigBlock(send_packet, 0, pre_send_packet, 0, 40);

//            byte[] out_packet = new byte[42];

//            out_packet[0] = 0x2A;
//            out_packet[1] = 0x00;

//            pre_send_packet.CopyTo(out_packet, 2);

//            recv_send_stream_.Write(out_packet, 0, 42);


//            byte_count = recv_send_stream_.Read(data_buffer_, 0, 65533);
//#if DEBUG
//            output_.output_debug("-client-recv-GOOD?[" + byte_count + "]: ");
//            output_.output_debug(data_buffer_, byte_count);
//#endif
//            blowfish_cipher_.init(false, blowfish_key_);
//            blowfish_cipher_.processBigBlock(data_buffer_, 2, decoded_buffer_, 0, byte_count - 2);
//            //Global.decXORPass(decoded_buffer_, 0x00, byte_count - 2,
//            // System.BitConverter.ToInt32(decoded_buffer_, byte_count - 10));
//#if DEBUG
//            output_.output_debug("-client-recv-decGOOD?++++: ");
//            output_.output_debug(decoded_buffer_, byte_count);
//#endif
        }
        

        private void game_guard_fill(byte[] data, int offset)
        {
            data[00 + offset] = 0x23;
            data[01 + offset] = 0x92;
            data[02 + offset] = 0x90;
            data[03 + offset] = 0x4d;
            data[04 + offset] = 0x18;
            data[05 + offset] = 0x30;
            data[06 + offset] = 0xb5;
            data[07 + offset] = 0x7c;
            data[08 + offset] = 0x96;
            data[09 + offset] = 0x61;
            data[10 + offset] = 0x41;
            data[11 + offset] = 0x47;
            data[12 + offset] = 0x05;
            data[13 + offset] = 0x07;
            data[14 + offset] = 0x96;
            data[15 + offset] = 0xfb;
            data[16 + offset] = 0x00;
            data[17 + offset] = 0x00;
            data[18 + offset] = 0x00;
        }

        private void dlgOutput_TextChanged(object sender, EventArgs e)
        {
            IntPtr ptrWparam = new IntPtr(SB_BOTTOM);
            IntPtr ptrLparam = new IntPtr(0);
            SendMessage(dlgOutput.Handle, WM_VSCROLL, ptrWparam, ptrLparam);   
        }

        private void main_Load(object sender, EventArgs e)
        {

        }

        private void main_FormClosed(object sender, FormClosedEventArgs e)
        {
            connection_.socket_.Close();
        }


    }
}
