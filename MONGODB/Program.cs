using System;
using System.Windows.Forms;

namespace MONGODB
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Initialization components
            Form1 myForm = new Form1();
            myForm.comboBox1.SelectedIndex = 0;
            myForm.comboBox2.SelectedIndex = 0;

            //Check accessing DB
            if (DB.ConnectDB() == true)
            {
                //Run form
                Application.Run(myForm);
            }
            else
            {
                MessageBox.Show("Cannot connect to the DB", "PROBLEM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
    }
}
