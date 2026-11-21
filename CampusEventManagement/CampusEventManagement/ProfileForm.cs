using System;
using System.Drawing;
using System.Windows.Forms;

namespace CampusEventManagement
{
    public class ProfileForm : Form
    {
        private TextBox name, email, phone, department;

        public ProfileForm()
        {
            Text = "Campus Event Management | My Profile";
            Size = new Size(620, 610);
            MinimumSize = new Size(570, 560);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(247, 246, 251);
            Font = new Font("Segoe UI", 9F);

            var card = new Panel
            {
                Location = new Point(45, 30),
                Size = new Size(510, 500),
                BackColor = Color.White
            };
            Controls.Add(card);

            card.Controls.Add(new Label
            {
                Text = "My Profile",
                Font = new Font("Segoe UI", 23, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 35, 63),
                AutoSize = true,
                Location = new Point(30, 25)
            });
            card.Controls.Add(new Label
            {
                Text = "Update your own account information",
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(34, 70)
            });

            try
            {
                var dt = Db.Query(
                    "SELECT FullName,Email,Phone,Department FROM Users WHERE UserId=@id",
                    Db.P("@id", Session.UserId));

                if (dt.Rows.Count == 0)
                    throw new Exception("Your user profile could not be found.");

                var row = dt.Rows[0];
                name = Field(card, "Full name", 125, Convert.ToString(row["FullName"]));
                email = Field(card, "Email", 205, Convert.ToString(row["Email"]));
                phone = Field(card, "Phone", 285, Convert.ToString(row["Phone"]));
                department = Field(card, "Department", 365, Convert.ToString(row["Department"]));

                var save = new Button
                {
                    Text = "SAVE CHANGES",
                    Location = new Point(32, 445),
                    Size = new Size(430, 42),
                    BackColor = Color.FromArgb(101, 78, 157),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };
                save.FlatAppearance.BorderSize = 0;
                save.Click += Save;
                card.Controls.Add(save);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open profile.\n" + ex.Message);
                Close();
            }
        }

        private TextBox Field(Control parent, string label, int y, string value)
        {
            parent.Controls.Add(new Label
            {
                Text = label,
                Location = new Point(34, y - 22),
                AutoSize = true,
                ForeColor = Color.FromArgb(85, 80, 101)
            });
            var box = new TextBox
            {
                Text = value,
                Location = new Point(32, y),
                Width = 430,
                Font = new Font("Segoe UI", 10)
            };
            parent.Controls.Add(box);
            return box;
        }

        private void Save(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(name.Text) || string.IsNullOrWhiteSpace(email.Text))
            {
                MessageBox.Show("Full name and email are required.");
                return;
            }

            try
            {
                Db.Execute(@"UPDATE Users
                    SET FullName=@n, Email=@e, Phone=@p, Department=@d
                    WHERE UserId=@id",
                    Db.P("@n", name.Text.Trim()),
                    Db.P("@e", email.Text.Trim()),
                    Db.P("@p", phone.Text.Trim()),
                    Db.P("@d", department.Text.Trim()),
                    Db.P("@id", Session.UserId));

                Session.FullName = name.Text.Trim();
                MessageBox.Show("Profile updated successfully.");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not update profile.\n" + ex.Message);
            }
        }
    }
}
