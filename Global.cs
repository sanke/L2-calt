using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace l2cAlt
{
    public class Global
    {
        public static void decXORPass(byte[] raw, int offset, int size, int key)
        {
            int stop = 4 + offset;
            int pos = size - 12;
            int edx;
            int ecx = key; // Initial xor key

            while (stop <= pos)
            {
                edx = (raw[pos] & 0xFF);
                edx |= (raw[pos + 1] & 0xFF) << 8;
                edx |= (raw[pos + 2] & 0xFF) << 16;
                edx |= (raw[pos + 3] & 0xFF) << 24;

                edx ^= ecx;

                ecx -= edx;

                raw[pos] = (byte)(edx & 0xFF);
                raw[pos + 1] = (byte)(edx >> 8 & 0xFF);
                raw[pos + 2] = (byte)(edx >> 16 & 0xFF);
                raw[pos + 3] = (byte)(edx >> 24 & 0xFF);
                pos -= 4;
            }
        }

        public static ulong CheckSum(byte[] raw, int count)
        {
            ulong chksum = 0;
            ulong ecx = 0;
            int i = 0;

            for (i = 0; i < count; i += 4)
            {
                ecx = ((ulong)raw[i] & 0xff);
                ecx |= ((ulong)raw[i + 1] << 8 & 0xff00);
                ecx |= ((ulong)raw[i + 2] << 0x10 & 0xff0000);
                ecx |= ((ulong)raw[i + 3] << 0x18 & 0xff000000);

                chksum = chksum ^ ecx;
            }
            /*
                        ecx = raw[i] &0xff;
                        ecx |= raw[i+1] << 8 &0xff00;
                        ecx |= raw[i+2] << 0x10 &0xff0000;
                        ecx |= raw[i+3] << 0x18 &0xff000000;
            */
            raw[i] = (byte)(chksum & 0xff);
            raw[i + 1] = (byte)(chksum >> 0x08 & 0xff);
            raw[i + 2] = (byte)(chksum >> 0x10 & 0xff);
            raw[i + 3] = (byte)(chksum >> 0x18 & 0xff);

            //store the checksum in the last 4 bytes
            return chksum;
        }
    }
}
