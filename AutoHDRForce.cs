using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace AutoHDRForce
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    class GameEntry
    {
        public string SubkeyName { get; set; }
        public string Name { get; set; }
        public string D3DBehaviors { get; set; }

        public bool AutoHDREnabled
        {
            get { return D3DBehaviors != null && D3DBehaviors.Contains("BufferUpgradeOverride=1"); }
        }

        public bool TenBitEnabled
        {
            get { return D3DBehaviors != null && D3DBehaviors.Contains("BufferUpgradeEnable10Bit=1"); }
        }

        public string StatusText
        {
            get
            {
                if (!AutoHDREnabled) return "Off";
                return TenBitEnabled ? "HDR + 10-bit" : "HDR";
            }
        }
    }

    class MainForm : Form
    {
        const string D3D_KEY = @"SOFTWARE\Microsoft\Direct3D";

        ListView listView;
        Button btnAdd;
        Button btnEnable;
        Button btnDisable;
        Button btnRemove;
        CheckBox chk10Bit;
        Label lblStatus;

        public MainForm()
        {
            Text = "Auto HDR Force";
            Size = new Size(620, 460);
            MinimumSize = new Size(500, 350);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9f);

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                RowCount = 3,
                ColumnCount = 1
            };
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Game list
            listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };
            listView.Columns.Add("Game", 340);
            listView.Columns.Add("Status", 100);
            listView.Columns.Add("Subkey", 120);
            listView.SelectedIndexChanged += ListView_SelectedIndexChanged;
            panel.Controls.Add(listView, 0, 0);

            // Button bar
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 5, 0, 5)
            };

            btnAdd = new Button { Text = "Add Game...", Width = 100, Height = 30 };
            btnAdd.Click += BtnAdd_Click;

            btnEnable = new Button { Text = "Enable HDR", Width = 100, Height = 30, Enabled = false };
            btnEnable.Click += BtnEnable_Click;

            btnDisable = new Button { Text = "Disable HDR", Width = 100, Height = 30, Enabled = false };
            btnDisable.Click += BtnDisable_Click;

            btnRemove = new Button { Text = "Remove", Width = 80, Height = 30, Enabled = false };
            btnRemove.Click += BtnRemove_Click;

            chk10Bit = new CheckBox
            {
                Text = "10-bit color",
                AutoSize = true,
                Checked = true,
                Padding = new Padding(10, 6, 0, 0)
            };

            buttonPanel.Controls.Add(btnAdd);
            buttonPanel.Controls.Add(btnEnable);
            buttonPanel.Controls.Add(btnDisable);
            buttonPanel.Controls.Add(btnRemove);
            buttonPanel.Controls.Add(chk10Bit);
            panel.Controls.Add(buttonPanel, 0, 1);

            // Status bar
            lblStatus = new Label
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                ForeColor = Color.DimGray,
                Padding = new Padding(0, 0, 0, 5)
            };
            panel.Controls.Add(lblStatus, 0, 2);

            Controls.Add(panel);

            LoadGames();
        }

        GameEntry SelectedGame()
        {
            if (listView.SelectedItems.Count == 0) return null;
            return listView.SelectedItems[0].Tag as GameEntry;
        }

        void UpdateButtons()
        {
            var g = SelectedGame();
            bool sel = g != null;
            btnEnable.Enabled = sel;
            btnDisable.Enabled = sel && g.AutoHDREnabled;
            btnRemove.Enabled = sel;
        }

        void SetStatus(string msg)
        {
            lblStatus.Text = msg;
        }

        // --- Registry Operations ---

        void EnsureD3DKey()
        {
            using (var key = Registry.CurrentUser.CreateSubKey(D3D_KEY))
            {
                // CreateSubKey opens or creates — nothing else needed
            }
        }

        List<GameEntry> ReadAllGames()
        {
            var games = new List<GameEntry>();
            using (var d3dKey = Registry.CurrentUser.OpenSubKey(D3D_KEY))
            {
                if (d3dKey == null) return games;
                foreach (string subName in d3dKey.GetSubKeyNames())
                {
                    if (subName == "MostRecentApplication") continue;
                    using (var sub = d3dKey.OpenSubKey(subName))
                    {
                        if (sub == null) continue;
                        string name = sub.GetValue("Name") as string;
                        if (name == null) continue;
                        string behaviors = sub.GetValue("D3DBehaviors") as string;
                        games.Add(new GameEntry
                        {
                            SubkeyName = subName,
                            Name = name,
                            D3DBehaviors = behaviors
                        });
                    }
                }
            }
            return games;
        }

        string FindFreeSubkey()
        {
            using (var d3dKey = Registry.CurrentUser.OpenSubKey(D3D_KEY))
            {
                if (d3dKey == null) return "Application0";
                for (int i = 0; ; i++)
                {
                    string name = "Application" + i;
                    using (var sub = d3dKey.OpenSubKey(name))
                    {
                        if (sub == null) return name;
                    }
                }
            }
        }

        void SetD3DBehaviors(string subkeyName, string value)
        {
            string path = D3D_KEY + @"\" + subkeyName;
            using (var key = Registry.CurrentUser.OpenSubKey(path, true))
            {
                if (key == null) return;
                if (value != null)
                    key.SetValue("D3DBehaviors", value, RegistryValueKind.String);
                else
                    key.DeleteValue("D3DBehaviors", false);
            }
        }

        void DeleteGameEntry(string subkeyName)
        {
            using (var d3dKey = Registry.CurrentUser.OpenSubKey(D3D_KEY, true))
            {
                if (d3dKey == null) return;
                d3dKey.DeleteSubKey(subkeyName, false);
            }
        }

        void AddGame(string exePath)
        {
            // Normalize path separators
            exePath = exePath.Replace('/', '\\');

            // Check if already exists
            var games = ReadAllGames();
            foreach (var g in games)
            {
                if (string.Equals(g.Name, exePath, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("This game is already in the list.", "Already Exists",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            EnsureD3DKey();
            string subkey = FindFreeSubkey();
            string path = D3D_KEY + @"\" + subkey;
            using (var key = Registry.CurrentUser.CreateSubKey(path))
            {
                key.SetValue("Name", exePath, RegistryValueKind.String);
            }

            LoadGames();
            SetStatus("Added: " + exePath);
        }

        // --- UI Logic ---

        void LoadGames()
        {
            listView.Items.Clear();
            var games = ReadAllGames();
            foreach (var g in games)
            {
                var item = new ListViewItem(g.Name);
                item.SubItems.Add(g.StatusText);
                item.SubItems.Add(g.SubkeyName);
                item.Tag = g;

                if (g.AutoHDREnabled)
                    item.ForeColor = Color.DarkGreen;

                listView.Items.Add(item);
            }
            UpdateButtons();
        }

        void ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "Select Game Executable";
                dlg.Filter = "Executables (*.exe)|*.exe|All Files (*.*)|*.*";
                dlg.CheckFileExists = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    AddGame(dlg.FileName);
                }
            }
        }

        void BtnEnable_Click(object sender, EventArgs e)
        {
            var g = SelectedGame();
            if (g == null) return;

            string val = chk10Bit.Checked
                ? "BufferUpgradeOverride=1;BufferUpgradeEnable10Bit=1"
                : "BufferUpgradeOverride=1";

            SetD3DBehaviors(g.SubkeyName, val);
            LoadGames();
            SetStatus("Enabled Auto HDR for: " + g.Name);
        }

        void BtnDisable_Click(object sender, EventArgs e)
        {
            var g = SelectedGame();
            if (g == null) return;

            SetD3DBehaviors(g.SubkeyName, null);
            LoadGames();
            SetStatus("Disabled Auto HDR for: " + g.Name);
        }

        void BtnRemove_Click(object sender, EventArgs e)
        {
            var g = SelectedGame();
            if (g == null) return;

            var result = MessageBox.Show(
                "Remove \"" + g.Name + "\" from the list?\nThis will also remove any HDR settings.",
                "Confirm Remove",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            DeleteGameEntry(g.SubkeyName);
            LoadGames();
            SetStatus("Removed: " + g.Name);
        }
    }
}
