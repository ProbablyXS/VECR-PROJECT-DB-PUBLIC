using Microsoft.VisualBasic;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MONGODB
{
    class DB
    {

        public static DateTime ResultEndDate = DateTime.Now;
        public static DateTime SelectedCustomDate;

        public static MongoClient client;
        public static IMongoDatabase db;
        public static IMongoCollection<VECRDOCUMENTS> collection;

        public static bool ConnectDB()
        {
            try
            {
                client = new MongoClient(ConnectionString);
                db = client.GetDatabase(databaseName);
                collection = db.GetCollection<VECRDOCUMENTS>(CollectionName);

                return true;
            }
            catch
            {

                return false;
            }
        }

        public static void ReadAllDocuments(object token, object searchFor, object text)
        {
            ListBox listbox = token as ListBox;
            RichTextBox textbox = text as RichTextBox;
            ComboBox SearchFor = searchFor as ComboBox;

            MongoClient dbClient = new MongoClient(DB.ConnectionString);
            //var dbList = dbClient.ListDatabases().ToList(); //GET ALL DATABASE NAME LIST
            var database = dbClient.GetDatabase(DB.databaseName);
            var collection = database.GetCollection<BsonDocument>(DB.CollectionName);

            //READ ALL DOCUMENT
            var AllDocuments = collection.Find(new BsonDocument()).ToList(); //READ ALL DOCUMENTS

            foreach (BsonDocument doc in AllDocuments)
            {
                if (doc.GetValue("token").ToString() == listbox.SelectedItem.ToString())
                {
                    textbox.Text += "TOKEN: " + doc.GetValue("token").ToString() + Constants.vbCrLf;
                    textbox.Text += "EMAIL: " + doc.GetValue("email").ToString() + Constants.vbCrLf;
                    textbox.Text += "DISCORDID: " + doc.GetValue("discordId").ToString() + Constants.vbCrLf;
                    textbox.Text += "UUID: " + doc.GetValue("uuid").ToString() + Constants.vbCrLf;
                    textbox.Text += "PCNAME: " + doc.GetValue("pcName").ToString() + Constants.vbCrLf;
                    textbox.Text += "MACHINE ID: " + doc.GetValue("machineId").ToString() + Constants.vbCrLf;
                    textbox.Text += "MAC ADDRESS: " + doc.GetValue("macAddress").ToString() + Constants.vbCrLf;

                    if (doc.GetValue("StartDate").ToString() == "0")
                    {
                        string Text = "STATUS: NOT USED \n";
                        textbox.AppendText(Text);
                        textbox.Select(textbox.Text.IndexOf(Text), Text.Length);
                        textbox.SelectionColor = Color.DarkOrange;
                    }
                    else
                    {
                        textbox.Text += "START DATE: " + doc.GetValue("StartDate").ToString() + Constants.vbCrLf;
                    }

                    try
                    {

                        if (doc.GetValue("EndDate").ToString() != "0")
                        {
                            var CompareTime = DateTime.Parse(doc.GetValue("EndDate").ToString());
                            var CurrentTime = DateTime.Parse(ResultEndDate.ToString("dd/MM/yyyy"));
                            if (DateTime.Compare(CompareTime, CurrentTime) > 0)
                            {
                                textbox.Text += "END DATE: " + doc.GetValue("EndDate").ToString();
                            }
                            else
                            {
                                string Text = "END DATE: " + doc.GetValue("EndDate").ToString() + " EXPIRED";
                                textbox.AppendText(Text);
                                textbox.Select(textbox.Text.IndexOf(Text), Text.Length);
                                textbox.SelectionColor = Color.DarkRed;
                            }
                        }
                        else
                        {
                            string Text = "LICENSE: " + doc.GetValue("EndDate").ToString() + " DAYS";
                            textbox.AppendText(Text);
                            textbox.Select(textbox.Text.IndexOf(Text), Text.Length);
                            textbox.SelectionColor = Color.DarkCyan;
                        }
                    }
                    catch
                    {
                        string Text = "LICENSE: " + doc.GetValue("EndDate").ToString() + " DAYS";
                        textbox.AppendText(Text);
                        textbox.Select(textbox.Text.IndexOf(Text), Text.Length);
                        textbox.SelectionColor = Color.DarkCyan;
                    }
                    return;
                }
            }
        }

        public static void DeleteAllExpiredLicense()
        {

            MongoClient dbClient = new MongoClient(DB.ConnectionString);
            //var dbList = dbClient.ListDatabases().ToList(); //GET ALL DATABASE NAME LIST
            var database = dbClient.GetDatabase(DB.databaseName);
            var collection = database.GetCollection<BsonDocument>(DB.CollectionName);

            //READ ALL DOCUMENT
            var AllDocuments = collection.Find(new BsonDocument()).ToList(); //READ ALL DOCUMENTS

            foreach (BsonDocument doc in AllDocuments)
            {
                try
                {
                    var CompareTime = DateTime.Parse(doc.GetValue("EndDate").ToString());
                    var CurrentTime = DateTime.Parse(ResultEndDate.ToString("dd/MM/yyyy"));
                    if (DateTime.Compare(CompareTime, CurrentTime) > 0)
                    {
                    }
                    else
                    {
                        DB.DeleteUser(Convert.ToInt32(doc.GetValue("token")));
                    }
                }
                catch { }
            }
        }

        public static void DeleteAllNotUsedLicense()
        {

            MongoClient dbClient = new MongoClient(DB.ConnectionString);
            //var dbList = dbClient.ListDatabases().ToList(); //GET ALL DATABASE NAME LIST
            var database = dbClient.GetDatabase(DB.databaseName);
            var collection = database.GetCollection<BsonDocument>(DB.CollectionName);

            //READ ALL DOCUMENT
            var AllDocuments = collection.Find(new BsonDocument()).ToList(); //READ ALL DOCUMENTS

            foreach (BsonDocument doc in AllDocuments)
            {
                try
                {
                    if (doc.GetValue("StartDate").ToString() == "0")
                    {
                        DB.DeleteUser(Convert.ToInt32(doc.GetValue("token")));
                    }
                }
                catch { }
            }
        }

        public static void AddUser(int token, string email, string discordId, string uuid, string pcName, string machineId, string macAddress)
        {
            var person = new VECRDOCUMENTS
            {
                token = token,
                email = email,
                discordId = discordId,
                uuid = uuid,
                pcName = pcName,
                machineId = machineId,
                macAddress = macAddress,
                //StartDate = ResultEndDate.ToString("dd/MM/yyyy"), //Current date on your computer
                StartDate = "0", //Current date on your computer
                //EndDate = CalculDate().ToString("dd/MM/yyyy"), //Current date on your computer + Subscription Days
                EndDate = CalculDate().ToString(), //Current date on your computer + Subscription Days
                isAlive = 0,
            };

            collection.InsertOneAsync(person);
        }

        public static void AddUserWithCreatingDate(int token, string email, string discordId, string uuid, string pcName, string machineId, string macAddress)
        {
            var person = new VECRDOCUMENTS
            {
                token = token,
                email = email,
                discordId = discordId,
                uuid = uuid,
                pcName = pcName,
                machineId = machineId,
                macAddress = macAddress,
                StartDate = ResultEndDate.ToString("dd/MM/yyyy"), //Current date on your computer
                EndDate = CalculDateWithGeneratingLicense().ToString("dd/MM/yyyy"), //Current date on your computer + Subscription Days
                isAlive = 0,
            };

            collection.InsertOneAsync(person);
        }

        public static void UpdateUser(int oldtoken, int newtoken, string email, string discordId, string uuid, string pcName, string machineId, string macAddress)
        {

            //UPDATE DATA
            var filter = Builders<VECRDOCUMENTS>.Filter.Eq(test => test.token, Convert.ToInt32(oldtoken)); //READ SPECIFIED ID (Used token in this case)

            var update = Builders<VECRDOCUMENTS>.Update.Set(test => test.token, Convert.ToInt32(newtoken))
                                                     .Set(test => test.email, email)
                                                     .Set(test => test.discordId, discordId)
                                                     .Set(test => test.uuid, uuid)
                                                     .Set(test => test.pcName, pcName)
                                                     .Set(test => test.machineId, machineId)
                                                     .Set(test => test.macAddress, macAddress);
            collection.UpdateOneAsync(filter, update);
        }

        public static void DeleteUser(int token)
        {
            var collection = db.GetCollection<VECRDOCUMENTS>(DB.CollectionName);

            var filter = Builders<VECRDOCUMENTS>.Filter.Eq(test => test.token, Convert.ToInt32(token)); //READ SPECIFIED ID (Used token in this case)

            collection.DeleteOne(filter);
        }

        public static void DeleteAllDocuments()
        {
            var collection = db.GetCollection<VECRDOCUMENTS>(DB.CollectionName);

            collection.DeleteManyAsync(Builders<VECRDOCUMENTS>.Filter.Empty);
        }

        public static void GetAllTokenFromDocuments(object sender, object SearchFor, object TotalCount)
        {
            ListBox listbox = sender as ListBox;
            Label totalcount = TotalCount as Label;
            ComboBox searchfor = SearchFor as ComboBox;

            listbox.Items.Clear();

            var collection = db.GetCollection<BsonDocument>(DB.CollectionName);

            //READ ALL DOCUMENTS
            var AllDocuments = collection.Find(new BsonDocument()).ToList(); //READ ALL DOCUMENTS

            foreach (BsonDocument doc in AllDocuments)
            {
                if (Form1.checkOnlyNotUsedLicense == true)
                {
                    if (doc.GetValue("StartDate").ToString() == "0")
                    {
                        listbox.Items.Add(doc.GetValue("token").ToString());
                    }
                }
                else
                {
                    listbox.Items.Add(doc.GetValue("token").ToString());
                }
            }

            GetTotalTokenCountFromDocuments(listbox, totalcount);
        }

        public static void GetTotalTokenCountFromDocuments(object listboxCount, object sender)
        {
            ListBox listbox = listboxCount as ListBox;
            Label label = sender as Label;

            label.Text = "result: " + listbox.Items.Count.ToString();
        }

        public static int CalculDate()
        {

            //DateTime result = ResultEndDate;
            int result = 0;

            //1 WEEK
            if (Form1.SelectedIndexDate == 0)
            {
                result = 7;
                //result = ResultEndDate.AddDays(7);
            }
            //1 MONTH
            else if (Form1.SelectedIndexDate == 1)
            {
                result = 30;
                //result = ResultEndDate.AddDays(30);
            }
            //3 MONTH
            else if (Form1.SelectedIndexDate == 2)
            {
                result = 90;
                //result = ResultEndDate.AddDays(90);
            }
            //6 MONTH
            else if (Form1.SelectedIndexDate == 3)
            {
                result = 180;
                //result = ResultEndDate.AddDays(180);
            }
            //1 YEAR
            else if (Form1.SelectedIndexDate == 4)
            {
                result = 365;
                //result = ResultEndDate.AddDays(365);
            }
            //CUSTOM
            else if (Form1.SelectedIndexDate == 5)
            {
                double val = (SelectedCustomDate - ResultEndDate).TotalDays;
                result = (int)val;
                //result = ResultEndDate.AddDays(val);
            }

            return result;
        }

        public static DateTime CalculDateWithGeneratingLicense()
        {

            DateTime result = ResultEndDate;
            //DateTime result;

            //1 WEEK
            if (Form1.SelectedIndexDate == 0)
            {
                //result = 7;
                result = ResultEndDate.AddDays(7);
            }
            //1 MONTH
            else if (Form1.SelectedIndexDate == 1)
            {
                //result = 30;
                result = ResultEndDate.AddDays(30);
            }
            //3 MONTH
            else if (Form1.SelectedIndexDate == 2)
            {
                //result = 90;
                result = ResultEndDate.AddDays(90);
            }
            //6 MONTH
            else if (Form1.SelectedIndexDate == 3)
            {
                //result = 180;
                result = ResultEndDate.AddDays(180);
            }
            //1 YEAR
            else if (Form1.SelectedIndexDate == 4)
            {
                //result = 365;
                result = ResultEndDate.AddDays(365);
            }
            //CUSTOM
            else if (Form1.SelectedIndexDate == 5)
            {
                double val = (SelectedCustomDate - ResultEndDate).TotalDays;
                //result = (int)val;
                result = ResultEndDate.AddDays(val);
            }

            return result;
        }

        public static void SearchToken(object token, object searchFor, object sender, object TotalCount)
        {
            TextBox textbox = token as TextBox;
            ListBox listbox = sender as ListBox;
            Label totalcount = TotalCount as Label;
            ComboBox SearchFor = searchFor as ComboBox;

            listbox.Items.Clear();

            var collection = db.GetCollection<BsonDocument>(DB.CollectionName);

            //READ ALL DOCUMENTS
            var AllDocuments = collection.Find(new BsonDocument()).ToList(); //READ ALL DOCUMENTS

            foreach (BsonDocument doc in AllDocuments)
            {
                if (doc.GetValue(SearchFor.Text).ToString().StartsWith(textbox.Text))
                {
                    listbox.Items.Add(doc.GetValue("token").ToString());
                }
            }

            totalcount.Text = "result: " + listbox.Items.Count.ToString();
        }

        public static bool CheckingTokenExist(string token)
        {
            //READ ALL DOCUMENTS
            var collection = db.GetCollection<BsonDocument>(DB.CollectionName);
            var AllDocuments = collection.Find(new BsonDocument()).ToList(); //READ ALL DOCUMENTS

            foreach (BsonDocument doc in AllDocuments)
            {
                if (doc.GetValue("token").ToString() == token)
                {
                    return false;
                }
            }

            return true;
        }

        public static string GenerateToken()
        {
            Random rdm = new Random();
            string result = null;

            for (int i = 0; i < 8; i++)
            {
                result += rdm.Next(1, 10).ToString(); //0 - 9
            }

            //READ ALL DOCUMENTS
            var collection = db.GetCollection<BsonDocument>(DB.CollectionName);
            var AllDocuments = collection.Find(new BsonDocument()).ToList(); //READ ALL DOCUMENTS

            foreach (BsonDocument doc in AllDocuments)
            {
                if (doc.GetValue("token").ToString() == result)
                {
                    GenerateToken();
                }
            }

            return result;
        }
    }
}
