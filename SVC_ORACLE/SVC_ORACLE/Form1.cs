using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utils;

namespace SVC_ORACLE
{
    public partial class Form1 : Form
    {
        Dictionary<int, Timer> timers = new Dictionary<int, Timer>();
        BackgroundWorker bw;
        Config<int, string> profiles;
        int profilesCount = 0;

        public Form1()
        {
            InitializeComponent();
        }

        #region Profiles managing
        private void LoadProfiles()
        {
            try
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
                        timers.Add(i, tmr);
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
            catch (Exception ex)
            {
                Log.Write(LogType.ERROR, ex, "LoadProfiles method");
            }
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
            try
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
            catch (Exception ex)
            {
                Log.Write(LogType.ERROR, ex, "SaveProfile method");
            }
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

        private void EnableAll(bool enable)
        {
            cbProfiles.Enabled = enable;
            btnFastRefresh.Enabled = enable;
            btnFullRefresh.Enabled = enable;
            btnAdd.Enabled = enable;
            btnDelete.Enabled = enable;
            btnSave.Enabled = enable;
        }      
        #endregion

        #region Event handlers
        private void Form1_Load(object sender, EventArgs e)
        {
            btnAdd.Enabled = true;
            btnDelete.Enabled = true;
            LoadProfiles();
            bw = new BackgroundWorker() { WorkerReportsProgress = true };
            bw.DoWork += Bw_DoWork;
            bw.ProgressChanged += Bw_ProgressChanged;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
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
                if (!bw.IsBusy)
                {
                    SelectProfile(ind);
                    bw.RunWorkerAsync(new Tuple<int, bool>(ind, false));
                }
                else
                {
                    Log.Write(LogType.ABNORMAL, null, "Cannot start fast refresh due to process busy. Profile " + profiles[ind]);
                }
            }

        }

        private void btnFullRefresh_Click(object sender, EventArgs e)
        {
            int ind = cbProfiles.SelectedIndex;

            if (ind >= 0)
            { 
                if (!bw.IsBusy)
                {
                    SelectProfile(ind);
                    bw.RunWorkerAsync(new Tuple<int, bool>(ind, true));
                }
                else
                {
                    Log.Write(LogType.ABNORMAL, null, "Cannot start full refresh due to process busy. Profile " + profiles[ind]);
                }
            }
        }

        private void Tmr_Tick(object sender, EventArgs e)
        {
            int profileId = (int)(sender as Timer).Tag;

            if (!bw.IsBusy)
            {
                SelectProfile(profileId);
                bw.RunWorkerAsync(new Tuple<int, bool>(profileId, false));
            }
            else
            {
                Log.Write(LogType.ABNORMAL, null, "Timer cannot start job due to process busy. Profile " + profiles[profileId]);
            }
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var param = e.Argument as Tuple<int, bool>;
            var profile = new Config<string, string>(profiles[param.Item1] + ".profile");

            Invoke((System.Threading.ThreadStart)delegate
            {
                EnableAll(false);
            });

            DateTime dateForUpdate = DateTime.ParseExact(profile["LastUpdate"], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            string now = OracleDB.GetServerNow();
            e.Result = CreateDumps
            (
                profile["Path"],
                profile["Schemas"],
                param.Item2 ? new DateTime(1900, 1, 1) : dateForUpdate,
                param.Item1
            );
            profile["LastUpdate"] = now;
        }

        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Maximum = ((int[])e.UserState)[1];
            pbStatus.Value = ((int[])e.UserState)[0];
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pbStatus.Value = 0;
            pbStatus.Maximum = 1;
            EnableAll(true);

            if (e.Error == null)
            {
                var result = (int)e.Result;
                SelectProfile(result);
                Log.Write(LogType.NORMAL, null, $"Refreshing completed, profile: {profiles[result]}");
            }
            else
            {
                Log.Write(LogType.ERROR, e.Error, "Error during executing Bw_DoWork");
            }
        }
        #endregion

        #region Source upload
        public int CreateDumps(string rootPath, string schemas, DateTime changedAfter, int profileId)
        {
            string schemaList = "'" + schemas.Replace(",", "', '") + "'";
            Directory.CreateDirectory(rootPath);
            string[] AllObjects = { "PROCEDURE", "FUNCTION", "PACKAGE", "PACKAGE BODY", "TRIGGER", "TYPE" };
            string[] ExecutingObjects = { "PROCEDURE", "FUNCTION", "PACKAGE", "PACKAGE BODY", "TRIGGER", "TYPE" };

            string sql = $@"
            SELECT OWNER, OBJECT_NAME, OBJECT_TYPE
	        FROM SYS.ALL_OBJECTS
	        WHERE LAST_DDL_TIME >= TO_DATE('{changedAfter.ToString("yyyyMMddHHmmss")}', 'YYYYMMDDHH24MISS')
	            AND OWNER IN ({schemaList})
                AND OBJECT_TYPE IN ({"'" + String.Join("', '", AllObjects) + "'"})
            ORDER BY 1, 2, 3 ";
            var result = OracleDB.RequestQueue(sql);
            var objectCount = result.Count / 3;
            
            if (objectCount > 0)
            {
                Log.Write(LogType.NORMAL, null, $"Found {objectCount} objects for refresh, profile {profiles[profileId]}");
            }

            while (result.Count > 0)
            {
                string owner = result.Dequeue();
                string name = result.Dequeue();
                string type = result.Dequeue();
                string curPath = $"{rootPath}\\{owner}\\{type}\\";

                if (ExecutingObjects.Contains(type))
                {
                    Directory.CreateDirectory(curPath);
                    string fileName = $@"{curPath}{name}.sql";
                    File.Delete(fileName);
                    string src = GetRoutineSource(owner, type, name);
                    if (src != null)
                    {
                        File.AppendAllText(
                            fileName,
                            src,
                            Encoding.UTF8
                         );
                    }
                }
                bw.ReportProgress(profileId, new int[] { objectCount - result.Count / 3, objectCount });
                
            }
            return profileId;
        }

        public string GetRoutineSource(string schema, string type, string name)
        {
            string sql = $@"
            SELECT CASE WHEN LINE = 1 THEN
            	'CREATE OR REPLACE ' || REPLACE(UPPER(REPLACE(TEXT, CHR(34))), NAME, CHR(34) || OWNER || CHR(34) || '.' || CHR(34) || NAME || CHR(34))
            	ELSE TEXT END SRC
            FROM SYS.ALL_SOURCE A
            WHERE NOT EXISTS (SELECT 'X' FROM SYS.ALL_SOURCE WHERE A.OWNER = OWNER AND A.TYPE = TYPE AND A.NAME = NAME AND LINE = 1 AND INSTR(TEXT, 'wrapped') > 0)
                AND OWNER = '{schema}'
                AND NAME = '{name}'
                AND TYPE = '{type}'
            ORDER BY OWNER, NAME, TYPE, LINE ";
            var result = OracleDB.RequestQueue(sql);

            if (result == null || result.Count == 0)
            {
                Log.Write(LogType.ABNORMAL, null, "GetRoutineSource - return empty source (result: " + (result == null ? "null)" : "Count=0)" + $" Owner={schema}, Type={type}, Name={name}"));
                return null;
            }
            else
            {
                StringBuilder sb = new StringBuilder(result.Count);
                while (result.Count > 0)
                {
                    sb.Append(result.Dequeue());
                }
                return sb.ToString().Replace("\n", "\r\n").Replace("\0", "");
            }
        } 
        #endregion
    }
}
