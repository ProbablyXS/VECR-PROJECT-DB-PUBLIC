using Microsoft.VisualBasic;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MONGODB
{
    public partial class Form1 : Form
    {
        public static string TOKEN;

        public static int SelectedIndexDate = 0;

        public static bool checkOnlyNotUsedLicense = false;

        public static string oldHwidCode;

        public Form1()
        {
            InitializeComponent();
        }

        private void GetAllTokenFromDocuments(object sender, EventArgs e)
        {
            DB.GetAllTokenFromDocuments(listBox1, comboBox2, label3);
        }

        private void buttonDeleteAllDocuments_Click(object sender, EventArgs e)
        {

            DialogResult MsgBoxYesNo = MessageBox.Show("Are you want sure to delete everything database collection ?",
                "WARNING",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (MsgBoxYesNo == DialogResult.Yes)
            {
                DB.DeleteAllDocuments();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedIndexDate = comboBox1.SelectedIndex;

            if (SelectedIndexDate == 5)
            {
                dateTimePicker1.Visible = true;
            }
            else
            {
                dateTimePicker1.Visible = false;
            }
        }

        private async void ButtonAddUser_Click(object sender, EventArgs e) //ADD USER
        {

            if (textBox1.Text == "token")
            {
                return;
            }

            if (DB.CheckingTokenExist(textBox1.Text) == false) { return; } //checking if token already exist from the DB

            else
            {
                if (textBox2.Text == "uuid" && textBox3.Text == "pcName" && textBox4.Text == "machineId" && textBox5.Text == "macAddress")
                {

                    string randomUUID = randString();
                    await Task.Delay(1);
                    string randompcName = randString();
                    await Task.Delay(1);
                    string randommachineId = randString();
                    await Task.Delay(1);
                    string randommacAddress = randString();

                    //oldHwidCode = DECRYPTO.AES_Encrypt(randomUUID + "\n" + randompcName + "\n" + randommachineId + "\n" + randommacAddress + "\n");
                    oldHwidCode = DECRYPTO.AES_Encrypt(randomUUID + Constants.vbCrLf + randompcName + Constants.vbCrLf + randommachineId + Constants.vbCrLf + randommacAddress);
                    label6.Text = oldHwidCode.Substring(0, 10) + "...";

                    label8.Text = textBox1.Text;


                    label2.Visible = true;
                    label5.Visible = true;
                    label6.Visible = true;
                    label7.Visible = true;
                    label8.Visible = true;

                    DB.AddUser(int.Parse(textBox1.Text), textBox11.Text, textBox16.Text, randomUUID, randompcName, randommachineId, randommacAddress);
                }
                else if (textBox11.Text != "email" && textBox16.Text != "discordId" && textBox2.Text != "uuid" && textBox3.Text != "pcName" && textBox4.Text != "machineId" && textBox5.Text != "macAddress")
                {

                    DB.AddUserWithCreatingDate(int.Parse(textBox1.Text), textBox11.Text, textBox16.Text, textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text);
                }

                //REFRESH
                DB.GetAllTokenFromDocuments(listBox1, comboBox2, label3);
            }
        }

        private string randString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var randVal = new Random();
            var stringChars = new char[randVal.Next(8, 16)];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[randVal.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return finalString;
        }

        private void buttonUpdateUser_Click(object sender, EventArgs e) //UPDATE USER
        {
            if (listBox1.SelectedIndex != -1)
            {
                DB.UpdateUser(Convert.ToInt32(listBox1.SelectedItem), Convert.ToInt32(textBox6.Text), textBox15.Text, textBox17.Text, textBox7.Text, textBox8.Text, textBox9.Text, textBox10.Text);

                //REFRESH
                DB.GetAllTokenFromDocuments(listBox1, comboBox2, label3);
            }
            else
            {
                MessageBox.Show("Please select a token into the listbox", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                DialogResult MsgBoxYesNo = MessageBox.Show("Are you want sure to delete this selected collection ?",
                    "WARNING",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (MsgBoxYesNo == DialogResult.Yes)
                {
                    DB.DeleteUser(Convert.ToInt32(listBox1.SelectedItem));

                    //REFRESH
                    DB.GetAllTokenFromDocuments(listBox1, comboBox2, label3);

                    richTextBox1.Clear();
                }
            }
            else
            {
                DialogResult MsgBoxYesNo = MessageBox.Show("Please select a token from the list",
                   "WARNING",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Information);

            }
        } //DELETE USER

        private void button7_Click(object sender, EventArgs e) //GENERATE A RANDOM TOKEN
        {
            TOKEN = DB.GenerateToken();

            textBox1.Text = TOKEN;
            textBox6.Text = TOKEN;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            textBox14.Text = DECRYPTO.AES_Decrypt(textBox13.Text);
        }

        private void button11_Click(object sender, EventArgs e)
        {

            if (textBox14.Lines.Length > 3)
            {
                textBox2.Text = textBox14.Lines[0]; //UUID
                textBox3.Text = textBox14.Lines[1]; //PCNAME
                textBox4.Text = textBox14.Lines[2]; //MACHINEID
                textBox5.Text = textBox14.Lines[3]; //MACADDRESS

                textBox7.Text = textBox14.Lines[0]; //UUID
                textBox8.Text = textBox14.Lines[1]; //PCNAME
                textBox9.Text = textBox14.Lines[2]; //MACHINEID
                textBox10.Text = textBox14.Lines[3]; //MACADDRESS
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.Items.Count >= 1)
                {
                    //COPY TO CLIPBOARD WITH RIGHT CLICK
                    Clipboard.SetText(listBox1.SelectedItem.ToString());

                    richTextBox1.Clear();

                    //READ
                    DB.ReadAllDocuments(listBox1, comboBox2, richTextBox1);
                }

            }
            catch { }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            textBox6.Text = richTextBox1.Lines[0].Remove(0, richTextBox1.Lines[0].IndexOf(":") + 1).Trim(); //TOKEN
            textBox15.Text = richTextBox1.Lines[1].Remove(0, richTextBox1.Lines[1].IndexOf(":") + 1).Trim(); //EMAIL
            textBox17.Text = richTextBox1.Lines[2].Remove(0, richTextBox1.Lines[2].IndexOf(":") + 1).Trim(); //DISCORDID
            textBox7.Text = richTextBox1.Lines[3].Remove(0, richTextBox1.Lines[3].IndexOf(":") + 1).Trim(); //UUID
            textBox8.Text = richTextBox1.Lines[4].Remove(0, richTextBox1.Lines[4].IndexOf(":") + 1).Trim(); //PCNAME
            textBox9.Text = richTextBox1.Lines[5].Remove(0, richTextBox1.Lines[5].IndexOf(":") + 1).Trim(); //MACHINEID
            textBox10.Text = richTextBox1.Lines[6].Remove(0, richTextBox1.Lines[6].IndexOf(":") + 1).Trim(); //MACADDRESS
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //ALLOW ONLY INTEGER VALUE FROM THE TEXTBOX
            if (char.IsLetter(e.KeyChar) || (e.KeyChar == '.'))
            {
                e.Handled = true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DB.DeleteAllExpiredLicense();

            //REFRESH ALL
            DB.GetAllTokenFromDocuments(listBox1, comboBox2, label3);
            listBox1.SelectedIndex = 0;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            DB.SearchToken(textBox12, comboBox2, listBox1, label3);
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            if (textBox12.Text.Length >= 1)
            {
                DB.SearchToken(textBox12, comboBox2, listBox1, label3);
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;

            if (txt.Text.ToLower() == "token".ToLower() ||
                txt.Text.ToLower() == "email".ToLower() ||
                txt.Text.ToLower() == "discordId".ToLower() ||
                txt.Text.ToLower() == "uuid".ToLower() ||
                txt.Text.ToLower() == "pcName".ToLower() ||
                txt.Text.ToLower() == "machineId".ToLower() ||
                txt.Text.ToLower() == "macAddress".ToLower())
            {
                txt.Text = "";
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;

            if (txt.Text.Length == 0)
            {
                if (txt.Name == "textBox1" ||
                    txt.Name == "textBox6")
                {
                    txt.Text = "token";
                }
                else if (txt.Name == "textBox11" ||
                         txt.Name == "textBox15")
                {
                    txt.Text = "email";
                }
                else if (txt.Name == "textBox16" ||
                         txt.Name == "textBox17")
                {
                    txt.Text = "discordId";
                }
                else if (txt.Name == "textBox2" ||
                         txt.Name == "textBox7")
                {
                    txt.Text = "uuid";
                }
                else if (txt.Name == "textBox3" ||
                         txt.Name == "textBox8")
                {
                    txt.Text = "pcName";
                }
                else if (txt.Name == "textBox4" ||
                         txt.Name == "textBox9")
                {
                    txt.Text = "machineId";
                }
                else if (txt.Name == "textBox5" ||
                         txt.Name == "textBox10")
                {
                    txt.Text = "macAddress";
                }
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DB.SelectedCustomDate = dateTimePicker1.Value;
        }

        private void label6_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(oldHwidCode);

            label2.ForeColor = Color.LimeGreen;
            label6.ForeColor = Color.LimeGreen;
        }

        private void label8_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(label8.Text);

            label7.ForeColor = Color.LimeGreen;
            label8.ForeColor = Color.LimeGreen;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            try
            {
                textBox14.Text = DECRYPTO.AES_Encrypt(textBox7.Text + Constants.vbCrLf +
                textBox8.Text + Constants.vbCrLf +
                textBox9.Text + Constants.vbCrLf +
                textBox10.Text);
            }
            catch { }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                checkOnlyNotUsedLicense = true;
            }
            else
            {
                checkOnlyNotUsedLicense = false;
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            DB.DeleteAllNotUsedLicense();

            //REFRESH ALL
            DB.GetAllTokenFromDocuments(listBox1, comboBox2, label3);
            try
            {
                listBox1.SelectedIndex = 0;
            }
            catch { }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            TESTAsync();
        }



        public async Task TESTAsync()
        {

            string txtfile = "generate.txt";

            if (File.Exists(txtfile))
            {
                File.Delete(txtfile);
            }

            for (int i = 0; i < numericUpDown1.Value; i++)
            {
                TOKEN = DB.GenerateToken();

                if (DB.CheckingTokenExist(textBox1.Text) == false) { return; } //checking if token already exist from the DB

                else
                {
                    string randomUUID = randString();
                    await Task.Delay(1);
                    string randompcName = randString();
                    await Task.Delay(1);
                    string randommachineId = randString();
                    await Task.Delay(1);
                    string randommacAddress = randString();

                    //oldHwidCode = DECRYPTO.AES_Encrypt(randomUUID + "\n" + randompcName + "\n" + randommachineId + "\n" + randommacAddress + "\n");
                    oldHwidCode = DECRYPTO.AES_Encrypt(randomUUID + Constants.vbCrLf + randompcName + Constants.vbCrLf + randommachineId + Constants.vbCrLf + randommacAddress);
                    label6.Text = oldHwidCode.Substring(0, 10) + "...";

                    label8.Text = textBox1.Text;

                    label2.Visible = true;
                    label5.Visible = true;
                    label6.Visible = true;
                    label7.Visible = true;
                    label8.Visible = true;

                    DB.AddUser(int.Parse(TOKEN), textBox11.Text, textBox16.Text, randomUUID, randompcName, randommachineId, randommacAddress);

                    StreamWriter s = new StreamWriter(txtfile, true);

                    s.WriteLine(i + 1 + "#" + Environment.NewLine +
                        "token: " + TOKEN + Environment.NewLine +
                        "oldhwid: " + oldHwidCode + Environment.NewLine);

                    s.Close();
                }
            }

            //REFRESH
            DB.GetAllTokenFromDocuments(listBox1, comboBox2, label3);
        }
    }
}
