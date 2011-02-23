using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace l2cAlt
{
    public class OutputDebug
    {
        public static OutputDebug instance_;
        
        RichTextBox output_dlg_;

        public static OutputDebug get_instance()
        {
            if (instance_ == null)
                instance_ = new OutputDebug();

            return instance_;
        }

        public void set_output(RichTextBox output)
        {
            try
            {
                output_dlg_ = output;
            }
            catch { }
        }

        public void output_debug(string output)
        {
            try
            {
                //int offset = output_dlg_.TextLength;
                // !!!forum solution
                // http://stackoverflow.com/questions/142003/cross-thread-operation-not-valid-control-accessed-from-a-thread-other-than-the-t
                if(output_dlg_.InvokeRequired)
                    output_dlg_.Invoke(new MethodInvoker( delegate { output_dlg_.Text += output; }));
                else
                    output_dlg_.Text += output;

                
                

            }
            catch { }
        }

        public void output_debug(byte[] byte_array, int count)
        {

            for (int i = 0; i < count; i++)
            {
                if (output_dlg_.InvokeRequired)
                    output_dlg_.Invoke(new MethodInvoker(delegate { output_dlg_.Text += byte_array[i].ToString("X2") + " "; }));
                else
                    output_dlg_.Text += (byte_array[i].ToString("X2") + " ");
            }
        }
    }
}
