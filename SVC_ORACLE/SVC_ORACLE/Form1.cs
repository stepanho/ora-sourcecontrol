﻿using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;
using Utils;

namespace SVC_ORACLE
{
    public partial class Form1 : Form
    {
        Dictionary<int, System.Timers.Timer> timers = new Dictionary<int, System.Timers.Timer>();
        BackgroundWorker bw;
        string[] AllObjects = { "PROCEDURE", "FUNCTION", "PACKAGE", "PACKAGE BODY", "TRIGGER", "TYPE" };
        string[] ExecutingObjects = { "PROCEDURE", "FUNCTION", "PACKAGE", "PACKAGE BODY", "TRIGGER", "TYPE" };
        System.Threading.AutoResetEvent are = new System.Threading.AutoResetEvent(true);
        Config<int, string> profiles;
        Config<string, string> git;

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
                foreach (var item in timers.Values)
                {
                    item.Close();
                }
                cbProfiles.Items.Clear();
                timers.Clear();
                profiles = new Config<int, string>("Profiles.config");
                GitInit();

                foreach (var item in profiles)
                {
                    cbProfiles.Items.Add(item.Value);
                    int timerValue = Convert.ToInt32((new Config<string, string>(item.Value + ".profile"))["AutoRefresh"]);
                    if (timerValue > 0)
                    {
                        var tmr = new System.Timers.Timer() { Enabled = true, Interval = timerValue * 1000, AutoReset = true };
                        tmr.Elapsed += Tmr_Elapsed;
                        timers.Add(item.Key, tmr);
                    }
                }

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
                (new Config<string, string>(profiles[ind] + ".profile")).RemoveFile();
                
                for (int i = ind; i < profiles.Count - 1; i++)
                {
                    profiles[i] = profiles[i + 1];
                }
                profiles.Remove(profiles.Count - 1);

                LoadProfiles();
            }

        }

        private void SaveProfile()
        {
            try
            {
                bool isNew = !profiles.Values.Contains(cbProfiles.Text);

                if (fbPath.SelectedPath == "")
                {
                    MessageBox.Show("Destination path is not choosed");
                    return;
                }

                if (isNew)
                { 
                    profiles[profiles.Count] = cbProfiles.Text;
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
            txtUpdated.Text = DateTime.ParseExact(profile["LastUpdate"] ?? "19000101000000", "yyyyMMddHHmmss", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss");
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

        private void PushNewWork(int profileId, bool isFull, DateTime execTime)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(delegate
            {
                (int ProfileId, bool IsFull, DateTime Timestamp) param = (profileId, isFull, execTime);
                are.WaitOne();
                Invoke((System.Threading.ThreadStart)delegate
                {
                    bw.RunWorkerAsync(param);
                });
            });
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
                PushNewWork(ind, false, DateTime.Now);
            }
        }

        private void btnFullRefresh_Click(object sender, EventArgs e)
        {
            int ind = cbProfiles.SelectedIndex;
            if (ind >= 0)
            {
                PushNewWork(ind, true, DateTime.Now);
            }
        }

        private void Tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            int profileId = timers.Select(o => o).Where(o => o.Value == sender as System.Timers.Timer).Select(o => o.Key).ToArray()[0];
            PushNewWork(profileId, false, e.SignalTime);
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var param =  ((int ProfileId, bool IsFull, DateTime Timestamp))e.Argument;
            var profile = new Config<string, string>(profiles[param.ProfileId] + ".profile");

            Invoke((System.Threading.ThreadStart)delegate
            {
                SelectProfile(param.ProfileId);
                EnableAll(false);
            });

            DateTime dateForUpdate = DateTime.ParseExact(profile["LastUpdate"], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            string now = param.Timestamp.ToString("yyyyMMddHHmmss");
            e.Result = CreateDumps
            (
                profile["Path"],
                profile["Schemas"],
                param.IsFull ? new DateTime(1900, 1, 1) : dateForUpdate,
                param.ProfileId
            );
            profile["LastUpdate"] = now ?? profile["LastUpdate"];
            if (GitRepository(param.ProfileId) != null && IsNeedPull(param.ProfileId))
            {
                GitPullWithStash(param.ProfileId);
            }
        }

        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Maximum = ((int[])e.UserState)[1];
            pbStatus.Value = ((int[])e.UserState)[0];
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool isNonEmpty = pbStatus.Value > 0;
            pbStatus.Value = 0;
            pbStatus.Maximum = 1;
            EnableAll(true);

            if (e.Error == null)
            {
                var result = (int)e.Result;
                SelectProfile(result);
                if (isNonEmpty)
                {
                    Log.Write(LogType.NORMAL, null, $"Refreshing completed, profile: {profiles[result]}");
                }
            }
            else
            {
                Log.Write(LogType.ERROR, e.Error, "Error during executing Bw_DoWork");
            }

            are.Set();
        }
        #endregion

        #region Source upload
        public int CreateDumps(string rootPath, string schemas, DateTime changedAfter, int profileId)
        {
            string schemaList = "'" + schemas.Replace(",", "', '") + "'";
            int hostCheck = IsRemotePathReachable(rootPath);
            if (hostCheck != 0)
            {
                throw new DirectoryNotFoundException($"Host is unreachable, profile {profiles[profileId]}, error code {hostCheck}");
            }
            Directory.CreateDirectory(rootPath);

            string sql = $@"
            SELECT OWNER, OBJECT_NAME, OBJECT_TYPE
	        FROM SYS.ALL_OBJECTS
	        WHERE LAST_DDL_TIME >= :dt
	            AND OWNER IN ({schemaList})
                AND OBJECT_TYPE IN ({"'" + string.Join("', '", AllObjects) + "'"})
            ORDER BY 1, 2, 3 ";
            var result = OracleDB.RequestQueue(sql, new Parameter("dt", changedAfter));
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

            DeleteDroppedObjects(rootPath, schemas, profileId);

            return profileId;
        }

        public int IsRemotePathReachable(string path)
        {
            try
            {
                string[] arr = path.Split(new char[] { '\\' }, 4);
                if (arr.Length >= 3 && arr[0] == "" && arr[1] == "" && arr[2] != "") //remote address detection
                {
                    var reply = (new Ping()).Send(arr[2], 1000);
                    if (reply.Status != IPStatus.Success)
                    {
                        return (int)reply.Status;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Log.Write(LogType.ERROR, ex, "Path check error");
                return -2;
            }
        }

        public string GetRoutineSource(string schema, string type, string name)
        {
            string sql = $@"
            SELECT CASE WHEN LINE = 1 THEN
            	'CREATE OR REPLACE ' || REPLACE(UPPER(REPLACE(TEXT, CHR(34))), NAME, CHR(34) || OWNER || CHR(34) || '.' || CHR(34) || NAME || CHR(34))
            	ELSE TEXT END SRC
            FROM SYS.ALL_SOURCE A
            WHERE NOT EXISTS (SELECT 'X' FROM SYS.ALL_SOURCE WHERE A.OWNER = OWNER AND A.TYPE = TYPE AND A.NAME = NAME AND LINE = 1 AND INSTR(TEXT, 'wrapped') > 0)
                AND OWNER = :schema
                AND NAME = :name
                AND TYPE = :type
            ORDER BY OWNER, NAME, TYPE, LINE ";
            var result = OracleDB.RequestQueue(
                sql,
                new Parameter("schema", schema),
                new Parameter("name", name),
                new Parameter("type", type)
            );

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
        
        public int DeleteDroppedObjects(string rootPath, string schemas, int profileId)
        {
            string schemaList = "'" + schemas.Replace(",", "', '") + "'";

            string sql = $@"
            SELECT OWNER || '\'  || OBJECT_TYPE || '\' || OBJECT_NAME
	        FROM SYS.ALL_OBJECTS
	        WHERE 1=1
	            AND OWNER IN ({schemaList})
                AND OBJECT_TYPE IN ({"'" + string.Join("', '", AllObjects) + "'"})
            ORDER BY OWNER, OBJECT_TYPE, OBJECT_NAME ";

            var dbObjList = OracleDB.RequestQueue(sql).Select(o => $@"{rootPath}\{o}.sql");
            var fileList = Directory.GetFiles(rootPath, "*.sql", SearchOption.AllDirectories);

            var resultList = fileList.Except(dbObjList).ToList();

            foreach (var item in resultList)
            {
                File.Delete(item);
            }

            return profileId;
        }
        #endregion

        #region Git
        private void GitInit()
        {
            string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            username = username.Substring(username.IndexOf('\\') + 1); //remove domain name

            git = new Config<string, string>("Git.config");
            git["SshPublicPath"] = git["SshPublicPath"] ?? $@"C:\Users\{username}\.ssh\id_rsa.pub";
            git["SshPrivatePath"] = git["SshPrivatePath"] ?? $@"C:\Users\{username}\.ssh\id_rsa";
            git["SshPasshrase"] = git["SshPasshrase"] ?? "";
            git["Name"] = git["Name"] ?? "Source Exporter";
            git["Email"] = git["Email"] ?? "noreply@git.com";
            git["GitServerUsername"] = git["GitServerUsername"] ?? "git";
        }

        private void GitPullWithStash(int profileId)
        {
            var profile = new Config<string, string>(profiles[profileId] + ".profile");

            using (var repo = new Repository(Repository.Discover(profile["Path"])))
            {
                PullOptions options;
                Signature signature;
                Stash stash = null;
                try
                {
                    options = new PullOptions();
                    signature = new Signature(git["Name"], git["Email"], new DateTimeOffset(DateTime.Now));

                    stash = repo.Stashes.Add(signature, StashModifiers.IncludeUntracked | StashModifiers.KeepIndex);
                    if (stash != null)
                    {
                        Log.Write(LogType.NORMAL, null, $"Git stash added, profile: {profiles[profileId]}");
                    }

                    options.FetchOptions = new FetchOptions()
                    {
                        CredentialsProvider = new CredentialsHandler(
                        (url, usernameFromUrl, types) =>
                            new SshUserKeyCredentials()
                            {
                                Username = git["GitServerUsername"],
                                Passphrase = git["SshPasshrase"],
                                PublicKey = git["SshPublicPath"],
                                PrivateKey = git["SshPrivatePath"],
                            }
                        )
                    };

                    var pullResult = Commands.Pull(repo, signature, options);

                    if (pullResult.Commit != null || pullResult.Status.ToString() == "UpToDate")
                    {
                        Log.Write(LogType.NORMAL, null, $"Git pull succeeded, result: {pullResult.Status}, profile: {profiles[profileId]}");
                    }
                    else
                    {
                        Log.Write(LogType.ABNORMAL, null, $"Git pull failed, result: {pullResult.Status}, profile: {profiles[profileId]}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(LogType.ERROR, ex, $"Git pull error, profile: {profiles[profileId]}");
                }
                finally
                {
                    if (stash != null)
                    {
                        var res3 = repo.Stashes.Pop(0);
                        Log.Write(LogType.NORMAL, null, $"Git stash pop proceeded with result {res3}, profile: {profiles[profileId]}");
                        if (repo.Stashes.Count() > 0)
                        {
                            Log.Write(LogType.ABNORMAL, null, $"Git stash collection is not empty, profile: {profiles[profileId]}");
                        }
                    }
                }
            }
        }

        private void GitFetch(int profileId, int attempt = 1)
        {
            using (var repo = GitRepository(profileId))
            {
                try
                {
                    var options = new FetchOptions()
                    {
                        CredentialsProvider = new CredentialsHandler(
                            (url, usernameFromUrl, types) => new SshUserKeyCredentials()
                            {
                                Username = git["GitServerUsername"],
                                Passphrase = git["SshPasshrase"],
                                PublicKey = git["SshPublicPath"],
                                PrivateKey = git["SshPrivatePath"],
                            }
                        )
                    };
                    foreach (Remote remote in repo.Network.Remotes)
                    {
                        IEnumerable<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);                       
                        Commands.Fetch(repo, remote.Name, refSpecs, options, "");
                    }
                }
                catch (LibGit2SharpException ex) when (ex.Message == "Failed to start SSH session: Unable to exchange encryption keys" && attempt < 3)
                {
                    GitFetch(profileId, ++attempt);
                }
                catch (Exception ex)
                {
                    Log.Write(LogType.ERROR, ex, $"Git fetch error, profile: {profiles[profileId]}");
                }
            }
        }

        private bool IsNeedPull(int profileId)
        {
            GitFetch(profileId);
            using (var repo = GitRepository(profileId))
            {
                foreach (var local in repo.Branches)
                {
                    if (local.IsCurrentRepositoryHead)
                    {
                        return local.TrackingDetails.BehindBy > 0;
                    }
                }
            }
            return false;
        }
        
        private Repository GitRepository(int profileId)
        {
            var profile = new Config<string, string>(profiles[profileId] + ".profile");
            try
            {
                return new Repository(Repository.Discover(profile["Path"]));
            }
            catch (LibGit2SharpException)
            {
                return null;
            }
            catch (Exception ex)
            {
                Log.Write(LogType.ERROR, ex, $"Git repository check error, profile: {profiles[profileId]}");
                return null;
            }
        }
        #endregion
    }
}
