using System;
using System.Drawing;
using System.Windows.Forms;

namespace CampusEventManagement
{
    public class LoginForm : Form
    {
        private TextBox username;
        private TextBox password;

        private readonly Color ink =
            Color.FromArgb(39, 35, 63);

        private readonly Color purple =
            Color.FromArgb(101, 78, 157);

        public LoginForm()
        {
            Text = "Campus Event Management | Sign In";

            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1100, 700);
            MinimumSize = new Size(950, 620);

            BackColor = Color.FromArgb(247, 246, 251);
            Font = new Font("Segoe UI", 9F);
            AutoScaleMode = AutoScaleMode.Dpi;

            // LEFT BRAND PANEL
            var brandPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 410,
                BackColor = ink
            };

            Controls.Add(brandPanel);

            // MAIN TITLE
            brandPanel.Controls.Add(new Label
            {
                Text = "CAMPUS\nEVENTS",
                Font = new Font("Segoe UI", 31, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(48, 145)
            });

            // TAGLINE
            brandPanel.Controls.Add(new Label
            {
                Text = "PLAN  •  CONNECT  •  CELEBRATE",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(218, 209, 239),
                AutoSize = true,
                Location = new Point(52, 275)
            });

            // DESCRIPTION
            brandPanel.Controls.Add(new Label
            {
                Text = "One place for campus events,\nactivities and community.",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(190, 183, 211),
                AutoSize = true,
                Location = new Point(52, 325)
            });

            // RIGHT LOGIN CARD
            var card = new Panel
            {
                Size = new Size(480, 520),
                Location = new Point(455, 65),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top |
                         AnchorStyles.Bottom |
                         AnchorStyles.Left |
                         AnchorStyles.Right
            };

            Controls.Add(card);

            // WELCOME TITLE
            card.Controls.Add(new Label
            {
                Text = "Welcome back",
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                ForeColor = ink,
                AutoSize = true,
                Location = new Point(38, 42)
            });

            // SUBTITLE
            card.Controls.Add(new Label
            {
                Text = "Sign in to continue to your dashboard",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(42, 94)
            });

            // USERNAME FIELD
            username = AddField(
                card,
                "Username",
                165
            );

            // PASSWORD FIELD
            password = AddField(
                card,
                "Password",
                265
            );

            password.UseSystemPasswordChar = true;

            // SIGN IN BUTTON
            var login = new Button
            {
                Text = "SIGN IN",
                Location = new Point(40, 365),
                Size = new Size(390, 48),
                BackColor = purple,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold
                ),
                Cursor = Cursors.Hand
            };

            login.FlatAppearance.BorderSize = 0;
            login.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(83, 62, 139);

            login.Click += DoLogin;

            card.Controls.Add(login);

            // SIGN UP LINK
            var signup = new LinkLabel
            {
                Text = "New here? Create a Student / Organizer account",
                AutoSize = true,
                Location = new Point(42, 440),
                LinkColor = purple,
                Font = new Font("Segoe UI", 9.5F)
            };

            signup.LinkClicked += (s, e) =>
            {
                using (var form = new RegisterForm())
                {
                    form.ShowDialog(this);
                }
            };

            card.Controls.Add(signup);

            // INITIAL ACCOUNT INFORMATION
            card.Controls.Add(new Label
            {
                Text = "Initial accounts: admin / ChangeMe123!   |   faculty / ChangeMe123!",
                AutoSize = true,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 7.5F),
                Location = new Point(42, 485)
            });

            AcceptButton = login;
        }

        // REUSABLE TEXTBOX METHOD
        private TextBox AddField(
            Control parent,
            string label,
            int y)
        {
            parent.Controls.Add(new Label
            {
                Text = label,
                Location = new Point(42, y - 27),
                AutoSize = true,
                ForeColor = Color.FromArgb(85, 80, 101),
                Font = new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold
                )
            });

            var box = new TextBox
            {
                Location = new Point(40, y),
                Width = 390,
                Height = 36,
                Font = new Font("Segoe UI", 11)
            };

            parent.Controls.Add(box);

            return box;
        }

        // LOGIN FUNCTION
        private void DoLogin(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(username.Text) ||
                string.IsNullOrWhiteSpace(password.Text))
            {
                MessageBox.Show(
                    "Enter both username and password.",
                    "Required Fields",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            try
            {
                var dt = Db.Query(
                    "SELECT TOP 1 * FROM Users " +
                    "WHERE Username=@u " +
                    "AND PasswordHash=@p " +
                    "AND IsActive=1",

                    Db.P("@u", username.Text.Trim()),
                    Db.P("@p", Db.Hash(password.Text))
                );

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "Invalid username or password, or inactive account.",
                        "Login Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                var row = dt.Rows[0];

                Session.UserId =
                    Convert.ToInt32(row["UserId"]);

                Session.FullName =
                    Convert.ToString(row["FullName"]);

                Session.Username =
                    Convert.ToString(row["Username"]);

                Session.Role =
                    Convert.ToString(row["Role"]);

                Hide();

                using (var dashboard = new DashboardForm())
                {
                    dashboard.ShowDialog(this);
                }

                Session.Clear();

                username.Clear();
                password.Clear();

                Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Sign in failed.\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}