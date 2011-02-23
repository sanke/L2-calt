using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace l2cAlt
{
    public class PacketForge
    {

        byte[] blowfish_key_ = {   0x6B, 0x60, 0xCB, 0x5B, 0x82,
                                   0xCE, 0x90, 0xB1, 0xCC, 0x2B,
                                   0x6C, 0x55, 0x6C, 0x6C, 0x6C, 
                                   0x6C };

        private Connection connection_;
        private OutputDebug output_;

        //private byte[] data_buffer_ = new byte[65533];
        private byte[] decoded_buffer_ = new byte[65533];
        private byte[] RSA_enc_key_ = new byte[128];
        private byte[] RSA_key_ = new byte[4];
        private byte[] login_ok_ = new byte[8];
        private byte[] play_ok_ = new byte[9];
        private bool first_packet_, login_;

        private BlowfishEngine blowfish_cipher_;
        private Crypt crypt_;
        // Constructor
        public PacketForge(Connection connection)
        {
            first_packet_ = true;
            login_ = true;
            connection_ = connection;
            blowfish_cipher_ = new BlowfishEngine();
            crypt_ = new Crypt();
            output_ = OutputDebug.get_instance();
        }

        // Main processor
        public void process_packet(byte[] data_buffer, int packet_lenght, Socket socket)
        {
            if (first_packet_ && login_)
            {
                process_first_packet(data_buffer, packet_lenght, socket);
            }
            else
            {
                if (login_)
                {
                    blowfish_cipher_.init(false, blowfish_key_);
                    blowfish_cipher_.processBigBlock(data_buffer, 0x02, decoded_buffer_, 0x00, packet_lenght - 2);
                    output_.output_debug("-server: packet code: ");
                    output_.output_debug(decoded_buffer_, 1);
                    output_.output_debug("\n");
                    switch (decoded_buffer_[0])
                    {
                        case 0x0B:
                            output_.output_debug("-client: login packet\n");
                            craft_login_packet(socket);
                            break;
                        case 0x03:
                            output_.output_debug("-client: login accepted.\n");
                            craft_login_ok_packet(socket, decoded_buffer_);
                            break;
                        case 0x01:
                            output_.output_debug("-client: login fail.\n");
                            break;
                        case 0x04:
                            output_.output_debug("-server: total servers: ");
                            output_.output_debug(decoded_buffer_[1].ToString());
                            output_.output_debug("\n");
                            craft_select_server_packet(socket, decoded_buffer_);
                            break;
                        case 0x07:
                            output_.output_debug("-server: server selected.\n-client: connecting to game server\n");
                            play_ok_[0] = decoded_buffer_[1];
                            play_ok_[1] = decoded_buffer_[2];
                            play_ok_[2] = decoded_buffer_[3];
                            play_ok_[3] = decoded_buffer_[4];
                            play_ok_[4] = decoded_buffer_[5];
                            play_ok_[5] = decoded_buffer_[6];
                            play_ok_[6] = decoded_buffer_[7];
                            play_ok_[7] = decoded_buffer_[8];
                            play_ok_[8] = decoded_buffer_[9];

                            login_ = false;
                            socket.Close();
                            connection_.connect("178.162.141.22", 2106);

                            craft_gameserver_protocol_packet(connection_.socket_);
                            break;
                    }
                }
                else
                {
                    Buffer.BlockCopy(data_buffer, 2, data_buffer, 0, packet_lenght - 2);
                    
                    output_.output_debug("-server: GS packet code: ");
                    output_.output_debug(data_buffer, 1);
                    output_.output_debug("\n");

                    switch (data_buffer[0])
                    {
                        case 0x2E:
                            output_.output_debug("-server: auth key packet\n");

                            break;
                    }
                }
            }
        }

        private void craft_gameserver_protocol_packet(Socket socket)
        {
            byte[] auth_packet = new byte[267];
            // at offset 0x04 we declare our protocol version in this case 83(Gracia Epilogue).
            byte[] packet = {
                                0x0B,0x01,0x0E,0x53,0x00,0x00,0x00,0x09,0x07,0x54,0x56,0x03,0x09,0x0B,
                                0x01,0x07,0x02,0x54,0x54,0x56,0x07,0x00,0x02,0x55,0x56,0x00,0x51,0x00,
                                0x53,0x57,0x04,0x07,0x55,0x08,0x54,0x01,0x07,0x01,0x53,0x00,0x56,0x55,
                                0x56,0x01,
                                0x06,0x05,0x04,0x51,0x03,0x08,0x51,0x08,0x51,0x56,0x04,0x54,0x06,0x55,
                                0x08,0x02,0x09,0x51,0x56,0x01,0x53,0x06,0x55,0x04,0x53,0x00,0x56,0x56,
                                0x53,0x01,0x09,0x02,0x09,0x01,0x51,0x54,0x51,0x09,0x55,0x56,0x09,0x03,
                                0x04,0x07,0x05,0x55,0x04,0x06,0x55,0x04,0x06,0x09,0x04,0x51,0x01,0x08,
                                0x08,0x06,0x05,0x52,0x06,0x04,0x01,0x07,0x54,0x03,0x06,0x52,0x55,0x06,
                                0x55,0x55,0x51,0x01,0x02,0x04,0x54,0x03,0x55,0x54,0x01,0x57,0x51,0x55,
                                0x05,0x52,0x05,0x54,0x07,0x51,0x51,0x55,0x07,0x02,0x53,0x53,0x00,0x52,
                                0x05,0x52,0x07,0x01,0x54,0x00,0x03,0x05,0x05,0x08,0x06,0x05,0x05,0x06,
                                0x03,0x00,0x0D,0x08,0x01,0x07,0x09,0x03,0x51,0x03,0x07,0x53,0x09,0x51,
                                0x06,0x07,0x54,0x0A,0x50,0x56,0x02,0x52,0x04,0x05,0x55,0x51,0x02,0x53,
                                0x00,0x08,0x54,0x04,0x52,0x56,0x06,0x02,0x09,0x00,0x08,0x03,0x53,0x56,
                                0x01,0x05,0x00,0x55,0x06,0x08,0x56,0x04,0x0D,0x06,0x07,0x52,0x06,0x07,
                                0x04,0x0A,0x06,0x01,0x04,0x54,0x04,0x00,0x05,0x02,0x04,0x54,0x00,0x09,
                                0x52,0x53,0x05,0x04,0x01,0x04,0x05,0x05,0x01,0x52,0x51,0x52,0x0D,0x06,
                                0x51,0x08,0x09,0x54,0x53,0x00,0x0D,0x01,0x02,0x03,0x54,0x53,0x01,0x05,
                                0x03,0x08,0x56,0x54,0x07,0x02,0x54,0x0B,0x06,0x11,0x5D,0x1F,0x60 
                            };

            

            connection_.send_packet(packet, 267, socket);
            // NOTE: rpg-club specific or smthng strange, but each game server packet needs to be xor'ed with 0x1A every 16th byte.
            // byte[i+=16]=byte[i] ^ 0x1A
        }
        // Connect to server! For now hardcoded third server
        private void craft_select_server_packet(Socket socket, byte[] decoded_buffer)
        {
            byte[] select_packet = new byte[32];
            byte[] preselect_packet = new byte[32];
            output_.output_debug("-client: selecting server 3\n");
            
            select_packet[0] = 0x02;
            select_packet[1] = login_ok_[0];
            select_packet[2] = login_ok_[1];
            select_packet[3] = login_ok_[2];
            select_packet[4] = login_ok_[3];
            select_packet[5] = login_ok_[4];
            select_packet[6] = login_ok_[5];
            select_packet[7] = login_ok_[6];
            select_packet[8] = login_ok_[7];

            select_packet[9] = 0x03; // server number

            select_packet[10] = 0x00;
            select_packet[11] = 0x00;
            select_packet[12] = 0x00;
            select_packet[13] = 0x00;
            select_packet[14] = 0x00;
            select_packet[15] = 0x00;

            ulong checksum = Global.CheckSum(select_packet, 16);

            select_packet[16] = (byte)(checksum & 0xff);
            select_packet[17] = (byte)(checksum >> 0x08 & 0xff);
            select_packet[18] = (byte)(checksum >> 0x10 & 0xff);
            select_packet[19] = (byte)(checksum >> 0x18 & 0xff);


            blowfish_cipher_.init(true, blowfish_key_);
            blowfish_cipher_.processBigBlock(select_packet, 0, preselect_packet, 0, 32);

            byte[] out_packet = new byte[34];

            out_packet[0] = 0x22;
            out_packet[1] = 0x00;
            preselect_packet.CopyTo(out_packet, 2);

            connection_.send_packet(out_packet, 34, socket);
        }
        // Login ok!
        private void craft_login_ok_packet(Socket socket,byte[] decoded_buffer)
        {
            byte[] login_ok = new byte[32];

            login_ok[0] = 0x05;
            login_ok[1] = decoded_buffer[1];
            login_ok[2] = decoded_buffer[2];
            login_ok[3] = decoded_buffer[3];
            login_ok[4] = decoded_buffer[4];
            login_ok[5] = decoded_buffer[5];
            login_ok[6] = decoded_buffer[6];
            login_ok[7] = decoded_buffer[7];
            login_ok[8] = decoded_buffer[8];
            login_ok[9] = 0x04;
            login_ok[10] = 0x00;
            login_ok[11] = 0x00;
            login_ok[12] = 0x00;
            login_ok[13] = 0x00;
            login_ok[14] = 0x00;
            login_ok[15] = 0x00;

            login_ok_[0] = decoded_buffer[1];
            login_ok_[1] = decoded_buffer[2];
            login_ok_[2] = decoded_buffer[3];
            login_ok_[3] = decoded_buffer[4];
            login_ok_[4] = decoded_buffer[5];
            login_ok_[5] = decoded_buffer[6];
            login_ok_[6] = decoded_buffer[7];
            login_ok_[7] = decoded_buffer[8];

            ulong checksum = Global.CheckSum(login_ok, 16);

            login_ok[16] = (byte)(checksum & 0xff);
            login_ok[17] = (byte)(checksum >> 0x08 & 0xff);
            login_ok[18] = (byte)(checksum >> 0x10 & 0xff);
            login_ok[19] = (byte)(checksum >> 0x18 & 0xff);

            byte[] prelogin_ok = new byte[32];

            blowfish_cipher_.init(true, blowfish_key_);
            blowfish_cipher_.processBigBlock(login_ok, 0, prelogin_ok, 0, 32);

            byte[] login_ok_packet = new byte[34];

            login_ok_packet[0] = 0x22;
            login_ok_packet[1] = 0x00;
            prelogin_ok.CopyTo(login_ok_packet, 2);

            connection_.send_packet(login_ok_packet, 34, socket);
        }
        // Send our login info!
        private void craft_login_packet(Socket socket)
        {
            byte[] login_info = new byte[128];

            string name = "sanke";
            string password = "ponulis";

            login_info[0x5B] = 0x24;
            
            for (int i = 0; i < name.Length; i++)
            {
                login_info[0x5E + i] = (byte)name[i];
            }
            
            for (int i = 0; i < password.Length; i++)
            {
                login_info[0x6C + i] = (byte)password[i];
            }

            byte[] exponent = { 1, 0, 1 };

            System.Security.Cryptography.RSAParameters RSAKeyInfo = new System.Security.Cryptography.RSAParameters();

            //Set RSAKeyInfo to the public key values. 
            RSAKeyInfo.Modulus = RSA_enc_key_;
            RSAKeyInfo.Exponent = exponent;

            RSA_Managed rsa_managed = new RSA_Managed();
            rsa_managed.ImportParameters(RSAKeyInfo);

            byte[] encrypted_bytes = new byte[128];

            encrypted_bytes = rsa_managed.EncryptValue(login_info);

            byte[] login_send = new byte[176];
            byte[] prelogin_send = new byte[176];

            encrypted_bytes.CopyTo(login_send, 1);

            login_send[129] = RSA_key_[0];
            login_send[130] = RSA_key_[1];
            login_send[131] = RSA_key_[2];
            login_send[132] = RSA_key_[3];
            login_send[133] = 0x23;//GG reply start
            login_send[134] = 0x01;
            login_send[135] = 0x00;
            login_send[136] = 0x00;
            login_send[137] = 0x67;//
            login_send[138] = 0x45;
            login_send[139] = 0x00;
            login_send[140] = 0x00;
            login_send[141] = 0xAB;//
            login_send[142] = 0x89;
            login_send[143] = 0x00;
            login_send[144] = 0x00;
            login_send[145] = 0xEF;//
            login_send[146] = 0xCD;
            login_send[147] = 0x00;
            login_send[148] = 0x00;//GG reply stop
            login_send[149] = 0x08;//
            login_send[150] = 0x00;
            login_send[151] = 0x00;
            login_send[152] = 0x00;
            login_send[153] = 0x00;//
            login_send[154] = 0x00;
            login_send[155] = 0x00;
            login_send[156] = 0x00;
            login_send[157] = 0x00;//
            login_send[158] = 0x00;
            login_send[159] = 0x00;

            ulong checksum = Global.CheckSum(login_send, 160);

            login_send[160] = (byte)(checksum & 0xff);
            login_send[161] = (byte)(checksum >> 0x08 & 0xff);
            login_send[163] = (byte)(checksum >> 0x10 & 0xff);
            login_send[163] = (byte)(checksum >> 0x18 & 0xff);

            blowfish_cipher_.init(true, blowfish_key_);
            blowfish_cipher_.processBigBlock(login_send, 0, prelogin_send, 0, 176);

            byte[] login_packet = new byte[178];

            login_packet[0] = 0xB2;
            login_packet[1] = 0x00;

            prelogin_send.CopyTo(login_packet, 2);

            connection_.send_packet(login_packet, 178, socket);
        }
        //First login packet arrived!
        private void process_first_packet(byte[] data_buffer, int byte_count, Socket socket)
        {
            first_packet_ = false;
            blowfish_cipher_.init(false, blowfish_key_);
            blowfish_cipher_.processBigBlock(data_buffer, 0x02, decoded_buffer_, 0x00, byte_count - 2);

            Global.decXORPass(decoded_buffer_, 0x00, byte_count - 2,
                System.BitConverter.ToInt32(decoded_buffer_, byte_count - 10));

            RSA_key_[0] = decoded_buffer_[1];
            RSA_key_[1] = decoded_buffer_[2];
            RSA_key_[2] = decoded_buffer_[3];
            RSA_key_[3] = decoded_buffer_[4];

            for (int i = 0; i < 128; i++)
            {
                RSA_enc_key_[i] = decoded_buffer_[9 + i];
            }

            for (int i = 0; i < 0x40; i++)
            {
                RSA_enc_key_[0x40 + i] = (byte)(RSA_enc_key_[0x40 + i] ^ RSA_enc_key_[i]);
            }

            for (int i = 0; i < 4; i++)
            {
                RSA_enc_key_[0x0d + i] = (byte)(RSA_enc_key_[0x0d + i] ^ RSA_enc_key_[0x34 + i]);
            }
            // step 2 : xor first 0x40 bytes with  last 0x40 bytes 
            for (int i = 0; i < 0x40; i++)
            {
                RSA_enc_key_[i] = (byte)(RSA_enc_key_[i] ^ RSA_enc_key_[0x40 + i]);
            }
            // step 1 : 0x4d-0x50 <-> 0x00-0x04
            for (int i = 0; i < 4; i++)
            {
                byte temp = RSA_enc_key_[0x00 + i];
                RSA_enc_key_[0x00 + i] = RSA_enc_key_[0x4D + i];
                RSA_enc_key_[0x4d + i] = temp;
            }

            output_.output_debug("-client: unscrambled RSA key: ");
            output_.output_debug(RSA_enc_key_, 128);

            for (int i = 0; i < 16; i++)
                blowfish_key_[i] = decoded_buffer_[i + 153];

            output_.output_debug("\n-client: new Blowfish key: ");
            output_.output_debug(blowfish_key_, 16);
            output_.output_debug("\n");

            craft_first_response(socket);
            //connection_.send_packet(blowfish_key_, 16, socket);

        }

        // first packet to dispatch to a server
        private void craft_first_response(Socket socket)
        {
            output_.output_debug("-client: GG packet\n");
            byte[] send_packet = new byte[40];
            byte[] presend_packet = new byte[40];

            send_packet[0] = 0x07; // Request GG packet
            send_packet[1] = RSA_key_[0];
            send_packet[2] = RSA_key_[1];
            send_packet[3] = RSA_key_[2];
            send_packet[4] = RSA_key_[3];

            game_guard_fill(send_packet, 5);

            ulong checksum = Global.CheckSum(send_packet, 24);

            send_packet[24] = (byte)(checksum & 0xff);
            send_packet[25] = (byte)(checksum >> 0x08 & 0xff);
            send_packet[26] = (byte)(checksum >> 0x10 & 0xff);
            send_packet[27] = (byte)(checksum >> 0x18 & 0xff);

            blowfish_cipher_.init(true, blowfish_key_);
            blowfish_cipher_.processBigBlock(send_packet, 0, presend_packet, 0, 40);

            byte[] out_packet = new byte[42];

            out_packet[0] = 0x2A;
            out_packet[1] = 0x0;

            presend_packet.CopyTo(out_packet, 2);

            connection_.send_packet(out_packet, 42, socket);
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
    }
}
