using System;
using System.Drawing;
using System.Windows.Forms;

namespace CampusEventManagement
{
    public class DashboardForm : Form
    {
        private readonly Panel sidebar;
        private readonly Panel main;

        private readonly Color ink =
            Color.FromArgb(35, 32, 55);

        private readonly Color purple =
            Color.FromArgb(111, 88, 166);

        private readonly Color pale =
            Color.FromArgb(246, 245, 250);

        private TableLayoutPanel cards;

        public DashboardForm()
        {
            Text = "Campus Event Management | Dashboard";

            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1000, 650);

            BackColor = pale;
            Font = new Font("Segoe UI", 9F);
            AutoScaleMode = AutoScaleMode.Dpi;

            // SIDEBAR
            sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 245,
                BackColor = ink,
                AutoScroll = true
            };

            // MAIN PANEL
            main = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = pale,
                Padding = new Padding(28, 22, 28, 24)
            };

            Controls.Add(main);
            Controls.Add(sidebar);

            BuildSidebar();
            BuildMain();
        }

        // SIDEBAR DESIGN
        private void BuildSidebar()
        {
            sidebar.Controls.Clear();

            // BRAND TITLE
            var brand = new Label
            {
                Text = "CAMPUS\nEVENTS",
                Font = new Font(
                    "Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(24, 25)
            };

            sidebar.Controls.Add(brand);

            // USER NAME
            var userName = new Label
            {
                Text = Session.FullName,
                Font = new Font(
                    "Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(25, 120),
                Size = new Size(195, 27),
                AutoEllipsis = true
            };

            sidebar.Controls.Add(userName);

            // USER ROLE
            var userRole = new Label
            {
                Text = Session.Role + " workspace",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(211, 203, 231),
                Location = new Point(25, 151),
                Size = new Size(195, 25),
                AutoEllipsis = true
            };

            sidebar.Controls.Add(userRole);

            // DASHBOARD
            AddMenu("Dashboard", ShowHome, 205);

            // ROLE BASED FEATURES
            int y = 255;

            foreach (var item in Modules.ForRole(Session.Role))
            {
                var selected = item;

                AddMenu(
                    item.Title,
                    () => OpenModule(selected),
                    y
                );

                y += 48;
            }

            // PROFILE
            AddMenu("My Profile", OpenProfile, y + 10);

            // SIGN OUT
            AddMenu("Sign Out", SignOut, y + 60);
        }

        // MENU BUTTON
        private void AddMenu(
            string text,
            Action action,
            int y)
        {
            var button = new Button
            {
                Text = "   " + text,
                TextAlign = ContentAlignment.MiddleLeft,

                Location = new Point(12, y),
                Size = new Size(218, 42),

                FlatStyle = FlatStyle.Flat,
                BackColor = ink,
                ForeColor = Color.White,

                Font = new Font("Segoe UI", 9.5F),
                Cursor = Cursors.Hand,

                Anchor = AnchorStyles.Top |
                         AnchorStyles.Left |
                         AnchorStyles.Right
            };

            button.FlatAppearance.BorderSize = 0;

            button.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(75, 63, 112);

            button.Click += (s, e) => action();

            sidebar.Controls.Add(button);
        }

        // MAIN DASHBOARD DESIGN
        private void BuildMain()
        {
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 112,
                BackColor = pale
            };

            // WELCOME TITLE
            header.Controls.Add(new Label
            {
                Text = "Welcome, " + Session.FullName,

                Font = new Font(
                    "Segoe UI", 24, FontStyle.Bold),

                ForeColor = ink,
                AutoSize = true,
                Location = new Point(2, 3)
            });

            // SUBTITLE
            header.Controls.Add(new Label
            {
                Text = Session.Role +
                       " dashboard  |  Manage your campus event activities",

                Font = new Font("Segoe UI", 10.5F),
                ForeColor = Color.FromArgb(112, 108, 127),

                AutoSize = true,
                Location = new Point(5, 49)
            });

            // HEADER LINE
            var line = new Panel
            {
                BackColor = Color.FromArgb(225, 221, 235),
                Height = 1,
                Dock = DockStyle.Bottom
            };

            header.Controls.Add(line);

            // FEATURE CARDS
            cards = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = pale,

                ColumnCount = 2,
                RowCount = 2,

                Padding = new Padding(0, 18, 0, 0),

                CellBorderStyle =
                    TableLayoutPanelCellBorderStyle.None
            };

            cards.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50F));

            cards.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50F));

            cards.RowStyles.Add(
                new RowStyle(SizeType.Percent, 50F));

            cards.RowStyles.Add(
                new RowStyle(SizeType.Percent, 50F));

            main.Controls.Add(cards);
            main.Controls.Add(header);

            ShowHome();
        }

        // SHOW ALL FEATURES
        private void ShowHome()
        {
            cards.SuspendLayout();

            cards.Controls.Clear();

            var modules = Modules.ForRole(Session.Role);

            for (int i = 0; i < modules.Count; i++)
            {
                var module = modules[i];

                var card = CreateCard(module);

                cards.Controls.Add(
                    card,
                    i % 2,
                    i / 2
                );
            }

            cards.ResumeLayout();
        }

        // FEATURE CARD DESIGN
        private Panel CreateCard(ModuleConfig module)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,

                BackColor = Color.White,

                Margin = new Padding(8, 8, 14, 14),

                Padding = new Padding(20),

                BorderStyle = BorderStyle.FixedSingle,

                Cursor = Cursors.Hand
            };

            // PURPLE SIDE LINE
            var accent = new Panel
            {
                BackColor = purple,
                Dock = DockStyle.Left,
                Width = 6
            };

            card.Controls.Add(accent);

            // FEATURE TITLE
            var title = new Label
            {
                Text = module.Title,

                Font = new Font(
                    "Segoe UI", 15, FontStyle.Bold),

                ForeColor = ink,

                AutoSize = true,

                Location = new Point(28, 24)
            };

            card.Controls.Add(title);

            // CRUD DESCRIPTION
            var description = new Label
            {
                Text = "Create  •  View  •  Update  •  Delete",

                Font = new Font("Segoe UI", 9.5F),

                ForeColor = Color.FromArgb(112, 108, 127),

                AutoSize = true,

                Location = new Point(29, 62)
            };

            card.Controls.Add(description);

            // OPEN MODULE BUTTON
            var open = new Button
            {
                Text = "OPEN MODULE   →",

                Size = new Size(155, 38),

                Location = new Point(28, 101),

                BackColor = purple,

                ForeColor = Color.White,

                FlatStyle = FlatStyle.Flat,

                Font = new Font(
                    "Segoe UI", 8.5F, FontStyle.Bold),

                Cursor = Cursors.Hand
            };

            open.FlatAppearance.BorderSize = 0;

            open.Click += (s, e) => OpenModule(module);

            card.Controls.Add(open);

            card.Click += (s, e) => OpenModule(module);

            title.Click += (s, e) => OpenModule(module);

            description.Click += (s, e) => OpenModule(module);

            return card;
        }

        // OPEN CRUD MODULE
        private void OpenModule(ModuleConfig module)
        {
            using (var form = new CrudForm(module))
            {
                form.ShowDialog(this);
            }
        }

        // OPEN PROFILE
        private void OpenProfile()
        {
            using (var form = new ProfileForm())
            {
                form.ShowDialog(this);
            }
        }

        // SIGN OUT
        private void SignOut()
        {
            Session.Clear();
            Close();
        }
    }
}