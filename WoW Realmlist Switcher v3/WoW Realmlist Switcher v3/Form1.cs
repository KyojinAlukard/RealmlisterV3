using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using WoW_Realmlist_Switcher_v3.Properties;

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

namespace WoW_Realmlist_Switcher_v3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists(Settings.Default["xml_filename"].ToString()))
            {
                if (new FileInfo(Settings.Default["xml_filename"].ToString()).Length != 0)
                    LoadXMLToListbox();
                else
                {
                    CreateDefaultXMLConfFile();
                    LoadXMLToListbox();
                }
            }
            else
            {
                CreateDefaultXMLConfFile();
                LoadXMLToListbox();
            }
            checkBox1.Checked = Settings.Default.clear_cache;
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
                Process.Start(WoWExePathDataList[listBox1.SelectedItem.ToString()]);
                if (Settings.Default.clear_cache)
                {
                    string MainDirectory = WoWExePathDataList[listBox1.SelectedItem.ToString()].Replace("Wow.exe", "");
                    if (Directory.Exists(MainDirectory + "\\Cache"))
                        Directory.Delete(MainDirectory + "\\Cache", true);
                }
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

        private void mephobiaButton4_Click_1(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.ShowDialog();
        }
    }
}
