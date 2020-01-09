using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace MapViewer
{

    public partial class Form2 : Form
    {
        //int ss = 0;
        //int s = 0;
        int n = 3;
        int sanoq = 0;
        public Form2()
        {
            InitializeComponent();
            ///GlobalVar.GlobalVar1 = 
        }

        private void timer1_Tick(object sender, EventArgs e)
        //{ sanoq++;
        {
            sanoq++;
            if (sanoq > n)
            {                
                this.Close();
            }
            //    StreamReader sw = new StreamReader("D:\\Для патента (NEW)\\Project\\MapViewer\\bin\\Debug\\pw.txt");

            //    String satr = "";
            //    satr = sw.ReadLine();
            //        sw.Close();
            //        s = Convert.ToInt32(satr);
            //       // MessageBox.Show(satr);
            //        if (s>0)
            //        {

            //            Form1 f1 = new Form1();
            //    f1.Show();
            //            this.Hide();
            //            this.timer1.Enabled = false;
            //        }
            //        else if(s==0)
            //        {
            //            Form3 f3 = new Form3();
            //f3.Show();
            //            this.Hide();
            //            this.timer1.Enabled = false;
            //}


        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //ss--;
            //File.Delete("D:\\Для патента (NEW)\\Project\\MapViewer\\bin\\Debug\\crack.txt");
            //StreamWriter a = new StreamWriter("D:\\Для патента (NEW)\\Project\\MapViewer\\bin\\Debug\\crack.txt");
            //a.WriteLine(ss);
            //a.Close();
        }
    }
}
