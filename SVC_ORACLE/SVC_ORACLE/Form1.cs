using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace SVC_ORACLE
{
    public partial class Form1 : Form
    {
        Dictionary<string, Timer> timers = new Dictionary<string, Timer>();
        Config<int, string> profiles;
        int profilesCount = 0;

        public Form1()
        {
            InitializeComponent();
        }

        #region Profiles managing
        private void LoadProfiles()
        {
            cbProfiles.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (Timer item in timers.Values)
            {
                item.Enabled = false;
                item.Tick -= Tmr_Tick;
                item.Dispose();
            }
            cbProfiles.Items.Clear();
            timers.Clear();
            profiles = new Config<int, string>("Profiles.config");
            int i = -1;
            while (profiles[++i] != null)
            {
                cbProfiles.Items.Add(profiles[i]);
                int timerValue = Convert.ToInt32((new Config<string, string>(profiles[i] + ".profile"))["AutoRefresh"]);
                if (timerValue > 0)
                {
                    var tmr = new Timer() { Enabled = true, Interval = timerValue * 1000, Tag = i };
                    tmr.Tick += Tmr_Tick;
                    timers.Add(profiles[i], tmr);
                }
            }
            profilesCount = i;

            txtHost.Text = "";
            txtPort.Text = "";
            txtSN.Text = "";
            txtUser.Text = "";
            txtPassword.Text = "";
            fbPath.SelectedPath = "";
            txtUpdated.Text = "";
            txtSchemas.Text = "";
            numAutoRefresh.Value = 0;
        }

        private void SwitchConnection(string profileName)
        {
            var connConfig = new Config<string, string>("Connection.config");
            var profileConfig = new Config<string, string>(profileName + ".profile");
            connConfig["Host"] = profileConfig["Host"];
            connConfig["Port"] = profileConfig["Port"];
            connConfig["ServiceName"] = profileConfig["ServiceName"];
            connConfig["UserId"] = profileConfig["UserId"];
            connConfig["Password"] = profileConfig["Password"];
            OracleDB.Init();
        }

        private void AddProfile()
        {
            cbProfiles.DropDownStyle = ComboBoxStyle.DropDown;
            cbProfiles.Text = "New profile";
        }

        private void DeleteProfile()
        {
            int ind = cbProfiles.SelectedIndex;
            if (ind >= 0)
            {
                (new Config<string, string>(profiles[ind] + ".profile")).RemoveFile(); ;
                profiles[ind] = null;
                --profilesCount;

                for (int i = ind; i <= profilesCount; i++)
                {
                    profiles[i] = profiles[i + 1];
                }

                LoadProfiles();
            }

        }

        private void SaveProfile()
        {
            bool isNew = true;
            for (int i = 0; i < profilesCount; i++)
            {
                if (profiles[i] == cbProfiles.Text)
                {
                    isNew = false;
                }
            }

            if (fbPath.SelectedPath == "")
            {
                MessageBox.Show("Destination path is not choosed");
                return;
            }

            if (isNew)
            {
                profiles[profilesCount++] = cbProfiles.Text;
            }
            var profile = new Config<string, string>(cbProfiles.Text + ".profile");
            profile["Host"] = txtHost.Text.Trim();
            profile["Port"] = txtPort.Text.Trim();
            profile["ServiceName"] = txtSN.Text.Trim();
            profile["UserId"] = txtUser.Text.Trim();
            profile["Password"] = txtPassword.Text.Trim();

            SwitchConnection(cbProfiles.Text);
            if (!OracleDB.CheckOnlineUsingOracle())
            {
                MessageBox.Show("Cannot connect");
                return;
            }

            profile["Path"] = fbPath.SelectedPath;
            profile["LastUpdate"] = profile["LastUpdate"] ?? "19000101000000";
            profile["Schemas"] = txtSchemas.Text.Trim();
            profile["AutoRefresh"] = numAutoRefresh.Value.ToString();

            LoadProfiles();
        }

        private void SelectProfile(int index)
        {
            cbProfiles.SelectedIndex = index;
            SwitchConnection(profiles[index]);
            var profile = new Config<string, string>(profiles[index] + ".profile");
            txtHost.Text = profile["Host"];
            txtPort.Text = profile["Port"];
            txtSN.Text = profile["ServiceName"];
            txtUser.Text = profile["UserId"];
            txtPassword.Text = profile["Password"];
            fbPath.SelectedPath = profile["Path"];
            txtUpdated.Text = DateTime.ParseExact(profile["LastUpdate"], "yyyyMMddHHmmss", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss");
            txtSchemas.Text = profile["Schemas"];
            numAutoRefresh.Value = Convert.ToDecimal(profile["AutoRefresh"]);
            pbStatus.Value = 0;
        }
        #endregion

        #region Event handlers
        private void Form1_Load(object sender, EventArgs e)
        {
            btnAdd.Enabled = true;
            btnDelete.Enabled = true;
            LoadProfiles();
            timers = new Dictionary<string, Timer>();
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            fbPath.ShowDialog();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveProfile();
        }

        private void cbProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectProfile(cbProfiles.SelectedIndex);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddProfile();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteProfile();
        }

        private void btnFastRefresh_Click(object sender, EventArgs e)
        {
            int ind = cbProfiles.SelectedIndex;

            if (ind >= 0)
            {
                SelectProfile(ind);
                var profile = new Config<string, string>(profiles[ind] + ".profile");
                CreateDumps(profile["Path"], profile["Schemas"], DateTime.ParseExact(profile["LastUpdate"], "yyyyMMddHHmmss", CultureInfo.InvariantCulture));
                profile["LastUpdate"] = OracleDB.GetServerNow();
                SelectProfile(ind);
            }

        }

        private void btnFullRefresh_Click(object sender, EventArgs e)
        {
            int ind = cbProfiles.SelectedIndex;

            if (ind >= 0)
            {
                SelectProfile(ind);
                var profile = new Config<string, string>(profiles[ind] + ".profile");
                CreateDumps(profile["Path"], profile["Schemas"], new DateTime(1900, 1, 1));
                profile["LastUpdate"] = OracleDB.GetServerNow();
                SelectProfile(ind);
            }
        }

        private void Tmr_Tick(object sender, EventArgs e)
        {
            lock (this)
            {
                int profileId = (int)(sender as Timer).Tag;
                SelectProfile(profileId);
                var profile = new Config<string, string>(profiles[profileId] + ".profile");
                CreateDumps(profile["Path"], profile["Schemas"], DateTime.ParseExact(profile["LastUpdate"], "yyyyMMddHHmmss", CultureInfo.InvariantCulture));
                profile["LastUpdate"] = OracleDB.GetServerNow();
                SelectProfile(profileId);
            }
        }
        #endregion

        #region Source upload
        public void CreateDumps(string rootPath, string schemas, DateTime changedAfter)
        {
            try
            {
                string schemaList = "'" + schemas.Replace(",", "', '") + "'";
                Directory.CreateDirectory(rootPath);
                string sql = $@"
                SELECT OWNER, OBJECT_NAME, OBJECT_TYPE
	            FROM SYS.ALL_OBJECTS
	            WHERE LAST_DDL_TIME >= TO_DATE('{changedAfter.ToString("yyyyMMddHHmmss")}', 'YYYYMMDDHH24MISS')
	                AND OWNER IN ({schemaList}) ";
                var result = OracleDB.RequestQueue(sql);
                pbStatus.Maximum = result.Count;

                string[] ExecutingObjects = { "PROCEDURE", "FUNCTION", "PACKAGE", "PACKAGE BODY", "TRIGGER", "TYPE" };
                while (result.Count > 0)
                {
                    string owner = result.Dequeue();
                    string name = result.Dequeue();
                    string type = result.Dequeue();
                    string curPath = $@"{rootPath}/{owner}/{type}/";

                    if (ExecutingObjects.Contains(type))
                    {
                        Directory.CreateDirectory(curPath);
                        string fileName = $@"{curPath}{name}.sql";
                        File.Delete(fileName);
                        File.AppendAllText(
                            fileName,
                            GetRoutineSource(owner, type, name),
                            Encoding.ASCII
                         );
                    }
                    pbStatus.Value = pbStatus.Maximum - result.Count;
                    Refresh();
                }
            }
            catch (Exception ex)
            {
                Log.Write(LogType.ERROR, ex, "CreateDumps");
            }
        }

        public string GetRoutineSource(string schema, string type, string name)
        {
            string sql = $@"
            SELECT CASE WHEN LINE = 1 THEN
            	'CREATE OR REPLACE ' || REPLACE(UPPER(REPLACE(TEXT, CHR(34))), NAME, CHR(34) || OWNER || CHR(34) || '.' || CHR(34) || NAME || CHR(34))
            	ELSE TEXT END SRC
            FROM SYS.ALL_SOURCE
            WHERE OWNER = '{schema}'
                AND NAME = '{name}'
                AND TYPE = '{type}'
            ORDER BY OWNER, NAME, TYPE, LINE ";
            var result = OracleDB.RequestQueue(sql);

            StringBuilder sb = new StringBuilder(result.Count);
            while (result.Count > 0)
            {
                sb.Append(result.Dequeue());
            }
            return sb.ToString();
        } 
        #endregion
    }
}
