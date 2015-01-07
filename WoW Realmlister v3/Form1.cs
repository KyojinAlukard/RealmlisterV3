using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using WoW_Realmlister_v3.Properties;

/*
* <ServerInfo>
* <Realm>
* <Name>My Localhost Server</Name>
* <Realmlist>127.0.0.1</Realmlist>
* <rPath>C:\WoW\Realmlist.wtf</rPath>
* <wPath>C:\WoW\Wow.exe</wPath>
* </Realm>
* <ServerInfo>
*/

namespace WoW_Realmlister_v3
{
    public partial class Form1 : Form
    {
        private const int EM_SETCUEBANNER = 0x1501; //! Used to set placeholder text

        //! Key codes to send to the client
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;
        private const uint WM_CHAR = 0x0102;
        private const int VK_RETURN = 0x0D;
        private const int VK_TAB = 0x09;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists(Settings.Default["xml_filename"].ToString()))
            {
                if (new FileInfo(Settings.Default["xml_filename"].ToString()).Length != 0)
                {
                    LoadXMLToListbox();
                    LoadXMLAccountsToComboBox();
                }
                else
                {
                    CreateDefaultXMLConfFile();
                    LoadXMLToListbox();
                    LoadXMLAccountsToComboBox();
                }
            }
            else
            {
                CreateDefaultXMLConfFile();
                LoadXMLToListbox();
                LoadXMLAccountsToComboBox();
            }
            checkBox1.Checked = Settings.Default.clear_cache;
            checkBox2.Checked = Settings.Default.auto_account_login;
        }

        public void LoadXMLAccountsToComboBox()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Settings.Default["xml_filename"].ToString());

                // Node Lists
                XmlNodeList UsernameNodes = doc.SelectNodes("ServerInfo/Account/Username");
                XmlNodeList PasswordNodes = doc.SelectNodes("ServerInfo/Account/Password");

                List<string> Usernames = new List<string>();
                List<string> Passwords = new List<string>();

                if (AccountsDataList.Count > 0)
                    AccountsDataList.Clear();

                //- Account Usernames
                foreach (XmlNode temp_username1 in UsernameNodes)
                {
                    Usernames.Add(temp_username1.InnerText);
                }
                //- Account Passwords
                foreach (XmlNode temp_password1 in PasswordNodes)
                {
                    Passwords.Add(temp_password1.InnerText);
                }
                //- Account Usernames & Passwords
                for (int i = 0; i < Usernames.Count(); i++)
                {
                    try { AccountsDataList.Add(Usernames[i], Passwords[i]); }
                    catch (ArgumentException) { }
                }

                comboBox1.Items.Clear();

                // Add Accounts to comboBox1
                foreach (KeyValuePair<string, string> entry in AccountsDataList)
                {
                    comboBox1.Items.Add(entry.Key);
                }

                if (AccountsDataList.Count > 0)
                    comboBox1.SelectedIndex = 0;
            }
            catch
            {
                CreateDefaultXMLConfFile();
            }
        }

        public void LoadXMLToListbox()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Settings.Default["xml_filename"].ToString());

                // Node Lists
                XmlNodeList RealmNameNodes = doc.SelectNodes("ServerInfo/Realm/Name");
                XmlNodeList RealmlistNodes = doc.SelectNodes("ServerInfo/Realm/Realmlist");
                XmlNodeList RealmlistPathNodes = doc.SelectNodes("ServerInfo/Realm/rPath");
                XmlNodeList WoWExePathNodes = doc.SelectNodes("ServerInfo/Realm/wPath");

                // String Lists
                List<string> RealmNames = new List<string>();
                List<string> RealmlistAddresses = new List<string>();
                List<string> RealmlistPaths = new List<string>();
                List<string> WoWExePaths = new List<string>();

                if (RealmlistAddressDataList.Count > 0)
                    RealmlistAddressDataList.Clear();

                if (RealmlistPathDataList.Count > 0)
                    RealmlistPathDataList.Clear();

                if (WoWExePathDataList.Count > 0)
                    WoWExePathDataList.Clear();

                //- Realmlist Addresses id:1
                foreach (XmlNode temp_realmlist1 in RealmlistNodes)
                {
                    RealmlistAddresses.Add(temp_realmlist1.InnerText);
                }
                //- Realmlist Paths id:2
                foreach (XmlNode temp_realmlist2 in RealmlistPathNodes)
                {
                    RealmlistPaths.Add(temp_realmlist2.InnerText);
                }
                //- WoW Exe Paths id:3
                foreach (XmlNode temp_realmlist3 in WoWExePathNodes)
                {
                    WoWExePaths.Add(temp_realmlist3.InnerText);
                }
                //- Realm Names id:1
                foreach (XmlNode Realm in RealmNameNodes)
                {
                    RealmNames.Add(Realm.InnerText);
                }
                //- Realm Names & Realmlist Addresses id:1
                for (int i = 0; i < RealmNames.Count(); i++)
                {
                    try { RealmlistAddressDataList.Add(RealmNames[i], RealmlistAddresses[i]); }
                    catch (ArgumentException) { }
                }
                //- Realm Names & Realmlist Paths id:2
                for (int i = 0; i < RealmlistPaths.Count(); i++)
                {
                    try { RealmlistPathDataList.Add(RealmNames[i], RealmlistPaths[i]); }
                    catch (ArgumentException) { }
                }
                //- Realm Names & WoW Exe Paths id:3
                for (int i = 0; i < WoWExePaths.Count(); i++)
                {
                    try { WoWExePathDataList.Add(RealmNames[i], WoWExePaths[i]); }
                    catch (ArgumentException) { }
                }

                //if (listBox1.Items.Count > 0)
                    listBox1.Items.Clear();

                // Realmlist Addresses Data List add to listbox1
                foreach (KeyValuePair<string, string> entry in RealmlistAddressDataList)
                {
                    listBox1.Items.Add(entry.Key);
                }

                if (RealmlistAddressDataList.Count > 0)
                    listBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                //create xml file
                CreateDefaultXMLConfFile();
                MessageBox.Show(ex.ToString());
            }
        }

        public void CreateDefaultXMLConfFile()
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();

                XmlNode serverInfoNode = xmlDoc.CreateElement("ServerInfo");
                xmlDoc.AppendChild(serverInfoNode);

                // Realms part
                XmlNode realmNode = xmlDoc.CreateElement("Realm");
                serverInfoNode.AppendChild(realmNode);

                XmlNode realmNameNode = xmlDoc.CreateElement("Name");
                realmNameNode.InnerText = "My Localhost Server";
                realmNode.AppendChild(realmNameNode);

                XmlNode realmlistNode = xmlDoc.CreateElement("Realmlist");
                realmlistNode.InnerText = "127.0.0.1";
                realmNode.AppendChild(realmlistNode);

                XmlNode realmlistPathNode = xmlDoc.CreateElement("rPath");
                realmlistPathNode.InnerText = "C:\\wow\\realmlist.wtf";
                realmNode.AppendChild(realmlistPathNode);

                XmlNode wowExePathNode = xmlDoc.CreateElement("wPath");
                wowExePathNode.InnerText = "C:\\wow\\Wow.exe";
                realmNode.AppendChild(wowExePathNode);

                // Accounts Part
                XmlNode accountNode = xmlDoc.CreateElement("Account");
                serverInfoNode.AppendChild(accountNode);

                XmlNode usernameNode = xmlDoc.CreateElement("Username");
                usernameNode.InnerText = "Test"; //- Username: Test
                accountNode.AppendChild(usernameNode);

                XmlNode passwordNode = xmlDoc.CreateElement("Password");
                passwordNode.InnerText = "VGVzdA=="; //- Password: Test
                accountNode.AppendChild(passwordNode);

                xmlDoc.Save(Settings.Default["xml_filename"].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Process.Start(Settings.Default["author_url"].ToString());
        }

        private void mephobiaButton4_Click(object sender, EventArgs e)
        {
            try
            {
                string realmlist = "Set realmlist " + RealmlistAddressDataList[listBox1.SelectedItem.ToString()];
                StreamWriter openFile = new StreamWriter(RealmlistPathDataList[listBox1.SelectedItem.ToString()]);
                openFile.WriteLine(realmlist);
                openFile.Close();
            }
            catch { MessageBox.Show("Invalid realmlist.wtf not found"); }

            try
            {
                if (Settings.Default.clear_cache)
                {
                    string MainDirectory = WoWExePathDataList[listBox1.SelectedItem.ToString()].Replace("Wow.exe", "");
                    if (Directory.Exists(MainDirectory + "\\Cache"))
                        Directory.Delete(MainDirectory + "\\Cache", true);
                }

                Process process = Process.Start(WoWExePathDataList[listBox1.SelectedItem.ToString()]);

                string accountName = comboBox1.Text.ToString();
                Thread.Sleep(600);

                //! Run this code in a new thread so the main form does not freeze up.
                new Thread(() =>
                {
                    if (Settings.Default.auto_account_login)
                    {
                        try
                        {
                            Thread.CurrentThread.IsBackground = true;

                            while (!process.WaitForInputIdle()) ;

                            Thread.Sleep(2000);

                            foreach (char accNameLetter in accountName)
                            {
                                SendMessage(process.MainWindowHandle, WM_CHAR, new IntPtr(accNameLetter), IntPtr.Zero);
                                Thread.Sleep(30);
                            }

                            //! Switch to password field
                            if (!String.IsNullOrWhiteSpace(MyFunctions.base64Decode(AccountsDataList[comboBox1.SelectedItem.ToString()])))
                            {
                                SendMessage(process.MainWindowHandle, WM_KEYUP, new IntPtr(VK_TAB), IntPtr.Zero);
                                SendMessage(process.MainWindowHandle, WM_KEYDOWN, new IntPtr(VK_TAB), IntPtr.Zero);

                                foreach (char accPassLetter in MyFunctions.base64Decode(AccountsDataList[comboBox1.SelectedItem.ToString()]))
                                {
                                    SendMessage(process.MainWindowHandle, WM_CHAR, new IntPtr(accPassLetter), IntPtr.Zero);
                                    Thread.Sleep(30);
                                }

                                //! Login to account
                                SendMessage(process.MainWindowHandle, WM_KEYUP, new IntPtr(VK_RETURN), IntPtr.Zero);
                                SendMessage(process.MainWindowHandle, WM_KEYDOWN, new IntPtr(VK_RETURN), IntPtr.Zero);

                                //! Login to char (disabled atm, will be used in next feature)
                                //if (Settings.Default.LoginToChar)
                                //{
                                //    Thread.Sleep(1500);
                                //    SendMessage(process.MainWindowHandle, WM_KEYUP, new IntPtr(VK_RETURN), IntPtr.Zero);
                                //    SendMessage(process.MainWindowHandle, WM_KEYDOWN, new IntPtr(VK_RETURN), IntPtr.Zero);
                                //}
                            }

                            Thread.CurrentThread.Abort();
                        }
                        catch
                        {
                            Thread.CurrentThread.Abort();
                        }
                    }
                }).Start();
            }
            catch { MessageBox.Show("Invalid Wow.exe not found"); }

            this.WindowState = FormWindowState.Minimized;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(WoWExePathDataList[listBox1.SelectedItem.ToString()]))
                {
                    PlayButton.Text = "Invalid Realm Settings";
                    PlayButton.Enabled = false;
                    return;
                }

                if (!File.Exists(RealmlistPathDataList[listBox1.SelectedItem.ToString()]))
                {
                    PlayButton.Text = "Invalid Realm Settings";
                    PlayButton.Enabled = false;
                    return;
                }

                PlayButton.Text = "Let's Play";
                PlayButton.Enabled = true;
            }
            catch { }
        }

        private void mephobiaButton1_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.ShowDialog();
            listBox1.Items.Clear();
            LoadXMLToListbox();
        }

        private void mephobiaButton3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure wanna delete?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(Settings.Default["xml_filename"].ToString());
                    string path = "ServerInfo/Realm[Name='" + listBox1.SelectedItem.ToString() + "']";
                    XmlNode node = doc.SelectSingleNode(path);
                    node.ParentNode.RemoveChild(node);
                    doc.Save(Settings.Default["xml_filename"].ToString());
                }
                catch (NullReferenceException)
                {
                    MessageBox.Show("Server not selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                LoadXMLToListbox();
                listBox1.ResetText();
                listBox1.Refresh();
            }
        }

        private void checkBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Settings.Default.clear_cache = checkBox1.Checked;
            Properties.Settings.Default.Save();
        }

        private void mephobiaButton2_Click(object sender, EventArgs e)
        {
            try
            {
                Form2 form2 = new Form2(listBox1.SelectedItem.ToString(),
                    RealmlistAddressDataList[listBox1.SelectedItem.ToString()],
                    RealmlistPathDataList[listBox1.SelectedItem.ToString()],
                    WoWExePathDataList[listBox1.SelectedItem.ToString()]);

                form2.ShowDialog();
                LoadXMLToListbox();
                listBox1.Refresh();
                listBox1.ResetText();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("No server selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // this store structure Realm Name, Realmlist Address
        private Dictionary<string, string> RealmlistAddressDataList = new Dictionary<string, string>();

        // this store structure Realm Name, realmlist.wtf path
        private Dictionary<string, string> RealmlistPathDataList = new Dictionary<string, string>();

        // this store structure Realm Name, Wow.exe path
        private Dictionary<string, string> WoWExePathDataList = new Dictionary<string, string>();

        // this store structure Account Name, Account Password
        private Dictionary<string, string> AccountsDataList = new Dictionary<string, string>();

        private void mephobiaButton4_Click_1(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.ShowDialog();
        }

        private void mephobiaButton5_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            form4.ShowDialog();
            comboBox1.Items.Clear();
            LoadXMLAccountsToComboBox();
        }

        private void mephobiaButton6_Click(object sender, EventArgs e)
        {
            try
            {
                Form4 form4 = new Form4(comboBox1.SelectedItem.ToString(),
                    AccountsDataList[comboBox1.SelectedItem.ToString()]);

                form4.ShowDialog();
                LoadXMLAccountsToComboBox();
                comboBox1.Refresh();
                comboBox1.ResetText();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("No account selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void mephobiaButton7_Click(object sender, EventArgs e)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Settings.Default["xml_filename"].ToString());
                string path = "ServerInfo/Account[Username='" + comboBox1.SelectedItem.ToString() + "']";
                XmlNode node = doc.SelectSingleNode(path);
                node.ParentNode.RemoveChild(node);
                doc.Save(Settings.Default["xml_filename"].ToString());
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Account not selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            LoadXMLAccountsToComboBox();
            comboBox1.ResetText();
            comboBox1.Refresh();
        }

        private void checkBox2_MouseDown(object sender, MouseEventArgs e)
        {
            Settings.Default.auto_account_login = checkBox2.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
