using System;
using System.Drawing;
using System.Windows.Forms;

namespace CampusEventManagement
{
    public class RegisterForm : Form
    {
        private TextBox name, user, email, pass, phone, dept;
        private ComboBox role;

        public RegisterForm()
        {
            Text = "Campus Event Management | Create Account";
            Size = new Size(570, 760);
            MinimumSize = new Size(540, 700);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(247, 246, 251);
            Font = new Font("Segoe UI", 9F);

            var card = new Panel
            {
                Location = new Point(45, 25),
                Size = new Size(460, 660),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(card);

            card.Controls.Add(new Label
            {
                Text = "Create your account",
                Font = new Font("Segoe UI", 23, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 35, 63),
                AutoSize = true,
                Location = new Point(28, 25)
            });
            card.Controls.Add(new Label
            {
                Text = "Register as a Student or Organizer",
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(32, 70)
            });

            name = AddField(card, "Full name", 125);
            user = AddField(card, "Username", 205);
            email = AddField(card, "Email", 285);
            phone = AddField(card, "Phone", 365);
            dept = AddField(card, "Department", 445);
            pass = AddField(card, "Password (minimum 6 characters)", 525);
            pass.UseSystemPasswordChar = true;

            card.Controls.Add(new Label
            {
                Text = "Account type",
                Location = new Point(32, 600),
                AutoSize = true,
                ForeColor = Color.FromArgb(85, 80, 101)
            });
            role = new ComboBox
            {
                Location = new Point(30, 622),
                Width = 185,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            role.Items.AddRange(new object[] { "Student", "Organizer" });
            role.SelectedIndex = 0;
            card.Controls.Add(role);

            var create = new Button
            {
                Text = "CREATE ACCOUNT",
                Location = new Point(230, 615),
                Size = new Size(195, 40),
                BackColor = Color.FromArgb(101, 78, 157),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            create.FlatAppearance.BorderSize = 0;
            create.Click += Create;
            card.Controls.Add(create);
        }

        private TextBox AddField(Control parent, string label, int y)
        {
            parent.Controls.Add(new Label
            {
                Text = label,
                Location = new Point(32, y - 22),
                AutoSize = true,
                ForeColor = Color.FromArgb(85, 80, 101)
            });
            var box = new TextBox
            {
                Location = new Point(30, y),
                Width = 395,
                Font = new Font("Segoe UI", 10)
            };
            parent.Controls.Add(box);
            return box;
        }

        private void Create(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(name.Text) ||
                string.IsNullOrWhiteSpace(user.Text) ||
                string.IsNullOrWhiteSpace(email.Text) ||
                pass.Text.Length < 6)
            {
                MessageBox.Show("Fill in the required fields. Password must be at least 6 characters.");
                return;
            }

            try
            {
                if (Db.Scalar("SELECT COUNT(*) FROM Users WHERE Username=@u",
                    Db.P("@u", user.Text.Trim())) > 0)
                {
                    MessageBox.Show("This username is already taken.");
                    return;
                }

                Db.Execute(@"INSERT INTO Users
                    (FullName,Username,Email,PasswordHash,Role,Phone,Department,IsActive)
                    VALUES(@n,@u,@e,@p,@r,@ph,@d,1)",
                    Db.P("@n", name.Text.Trim()),
                    Db.P("@u", user.Text.Trim()),
                    Db.P("@e", email.Text.Trim()),
                    Db.P("@p", Db.Hash(pass.Text)),
                    Db.P("@r", role.Text),
                    Db.P("@ph", phone.Text.Trim()),
                    Db.P("@d", dept.Text.Trim()));

                MessageBox.Show("Account created successfully. You can sign in now.");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create account.\n" + ex.Message,
                    "Registration error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
