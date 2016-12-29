using System;
using System.Threading;
using System.Windows.Forms;

namespace AsyncDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = DateTime.Now.ToString();
        }

        int Add(int a, int b)
        {
            Thread.Sleep(5000);
            return a + b;
        }

        void GetResult(IAsyncResult ar)
        {
            var fe = (Func<int, int, int>)ar.AsyncState;
            int ans = fe.EndInvoke(ar);
            Action<int> act = UpdateListBox;
            this.Invoke(act, ans);
       }

        private void UpdateListBox(int ans)
        {
            listBox1.Items.Add(ans);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int no1 = int.Parse(textBox1.Text);
                int no2 = int.Parse(textBox2.Text);
                int ans = Add(no1, no2);
                listBox1.Items.Add(ans);
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            catch (OverflowException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            finally
            {
                MessageBox.Show("Finally");
            }
            MessageBox.Show("OK");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int no1 = int.Parse(textBox1.Text);
            int no2 = int.Parse(textBox2.Text);
            Func<int, int, int> f = Add;
            //f.BeginInvoke(no1, no2, GetResult, f);
            //-------------------------------------------------
            f.BeginInvoke(no1, no2, ar => 
            {
                Action act = () => listBox1.Items.Add(f.EndInvoke(ar));
                this.Invoke(act);
            }, null);

            //MessageBox.Show("OK");
        }

        private void button3_Click(object sender, EventArgs e)
        {

            ThreadPool.QueueUserWorkItem(o =>
            {
                int no1, no2;
                lock(textBox1.Text)
                {
                    no1 = int.Parse(textBox1.Text);
                    no2 = int.Parse(textBox2.Text);
                }
                int ans = Add(no1, no2);
                Action act = () => listBox1.Items.Add(ans);
                this.Invoke(act);
            });
        }
        void A()
        {
            try
            {
                B();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR when call B");
                throw ex;
            }
        }

        void B()
        {
            try
            {
                C();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR when call C");
                throw ex;
            }
        }

        void C()
        {
            int no1 = 10;
            int no2 = 0;
            int no3 = no1 / no2;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            A();
        }
    }
}
