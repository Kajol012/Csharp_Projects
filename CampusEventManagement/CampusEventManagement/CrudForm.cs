using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CampusEventManagement
{
    public class CrudForm : Form
    {
        private readonly ModuleConfig config;
        private readonly ModuleAccessPolicy accessPolicy;
        private readonly DataGridView grid;
        private readonly Dictionary<string, TextBox> inputs = new Dictionary<string, TextBox>();
        private int selectedId;

        public CrudForm(ModuleConfig module)
        {
            config = module;
            // Polymorphism: the base-class reference calls the appropriate override.
            accessPolicy = config.OwnerColumn == null
                ? (ModuleAccessPolicy)new AdminModuleAccessPolicy()
                : new OwnerModuleAccessPolicy();
            Text = "Campus Event Management | " + config.Title;
            StartPosition = FormStartPosition.CenterParent;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1000, 680);
            BackColor = Color.FromArgb(246, 245, 250);
            Font = new Font("Segoe UI", 9F);
            AutoScaleMode = AutoScaleMode.Dpi;

            var header = new Panel { Dock = DockStyle.Top, Height = 92, BackColor = Color.White, Padding = new Padding(28, 14, 28, 8) };
            header.Controls.Add(new Label
            {
                Text = config.Title,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 35, 63),
                AutoSize = true,
                Location = new Point(26, 12)
            });
            header.Controls.Add(new Label
            {
                Text = "Manage records: create, view, update and delete",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(29, 54)
            });

            var body = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24, 18, 24, 20),
                BackColor = Color.FromArgb(246, 245, 250),
                ColumnCount = 1,
                RowCount = 3
            };
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            body.RowStyles.Add(new RowStyle(SizeType.Percent, 48F));
            body.RowStyles.Add(new RowStyle(SizeType.Percent, 37F));
            body.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));

            var recordsPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(14) };
            recordsPanel.Controls.Add(new Label
            {
                Text = "RECORDS",
                Dock = DockStyle.Top,
                Height = 28,
                ForeColor = Color.FromArgb(101, 78, 157),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            });
            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                GridColor = Color.FromArgb(235, 232, 242)
            };
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(239, 236, 247);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(55, 48, 77);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(226, 219, 243);
            grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(39, 35, 63);
            grid.SelectionChanged += SelectRow;
            recordsPanel.Controls.Add(grid);

            var fieldsPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(14), AutoScroll = true };
            fieldsPanel.Controls.Add(new Label
            {
                Text = "RECORD DETAILS",
                Dock = DockStyle.Top,
                Height = 28,
                ForeColor = Color.FromArgb(101, 78, 157),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            });
            var fieldFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(2, 8, 2, 4),
                BackColor = Color.White
            };
            foreach (string field in config.Fields)
            {
                var fieldPanel = new Panel { Width = 285, Height = 65, Margin = new Padding(7) };
                fieldPanel.Controls.Add(new Label
                {
                    Text = field,
                    Location = new Point(2, 1),
                    AutoSize = true,
                    ForeColor = Color.FromArgb(90, 86, 105),
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold)
                });
                var box = new TextBox { Location = new Point(2, 24), Width = 265, Font = new Font("Segoe UI", 9.5F) };
                inputs[field] = box;
                fieldPanel.Controls.Add(box);
                fieldFlow.Controls.Add(fieldPanel);
            }
            fieldsPanel.Controls.Add(fieldFlow);

            var actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 12, 0, 0),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.FromArgb(246, 245, 250)
            };
            AddButton(actions, "CREATE", Color.FromArgb(72, 145, 112), Create);
            AddButton(actions, "UPDATE", Color.FromArgb(101, 78, 157), Update);
            AddButton(actions, "DELETE", Color.FromArgb(190, 83, 98), Delete);
            AddButton(actions, "CLEAR", Color.FromArgb(112, 111, 128), (s, e) => Clear());
            AddButton(actions, "REFRESH", Color.FromArgb(64, 112, 157), (s, e) => LoadRows());

            body.Controls.Add(recordsPanel, 0, 0);
            body.Controls.Add(fieldsPanel, 0, 1);
            body.Controls.Add(actions, 0, 2);
            Controls.Add(body);
            Controls.Add(header);
            LoadRows();
        }

        private void AddButton(FlowLayoutPanel panel, string text, Color color, EventHandler click)
        {
            var button = new Button
            {
                Text = text,
                Width = 125,
                Height = 40,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0, 0, 12, 0),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += click;
            panel.Controls.Add(button);
        }

        private SqlParameter[] BuildParameters()
        {
            var list = new List<SqlParameter>();
            foreach (string field in config.Fields)
                list.Add(Db.P("@" + field, inputs[field].Text.Trim()));

            if (config.Table == "Users")
            {
                string role = inputs["Role"].Text.Trim();
                if (!new[] { "Admin", "Organizer", "Student", "Faculty" }.Contains(role, StringComparer.OrdinalIgnoreCase))
                    throw new Exception("Role must be Admin, Organizer, Student, or Faculty.");
                string active = inputs["IsActive"].Text.Trim();
                if (active != "0" && active != "1" &&
                    !active.Equals("true", StringComparison.OrdinalIgnoreCase) &&
                    !active.Equals("false", StringComparison.OrdinalIgnoreCase))
                    throw new Exception("IsActive must be True/False or 1/0.");
            }
            return list.ToArray();
        }

        private void LoadRows()
        {
            try
            {
                string columns = "Id," + string.Join(",", config.Fields);
                string sql = "SELECT " + columns + " FROM " + config.Table +
                    accessPolicy.GetSelectFilter(config.OwnerColumn) +
                    " ORDER BY Id DESC";
                grid.DataSource = Db.Query(sql, accessPolicy.GetOwnerParameters(config.OwnerColumn));
                if (grid.Columns.Contains("Id")) grid.Columns["Id"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load records.\\n" + ex.Message, "Database error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectRow(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            DataGridViewRow row = grid.SelectedRows[0];
            if (row.Cells["Id"].Value == null) return;
            selectedId = Convert.ToInt32(row.Cells["Id"].Value);
            foreach (string field in config.Fields)
                inputs[field].Text = row.Cells[field].Value == DBNull.Value ? "" : Convert.ToString(row.Cells[field].Value);
        }

        private void Create(object sender, EventArgs e)
        {
            try
            {
                SqlParameter[] parameters = BuildParameters();
                if (config.Table == "Users")
                {
                    const string sql = @"INSERT INTO Users
                        (FullName,Username,Email,PasswordHash,Role,Phone,Department,IsActive)
                        VALUES(@FullName,@Username,@Email,@NewPasswordHash,@Role,@Phone,@Department,@IsActive)";
                    var all = parameters.Concat(new[] { Db.P("@NewPasswordHash", Db.Hash("ChangeMe123!")) }).ToArray();
                    Db.Execute(sql, all);
                    MessageBox.Show("User created. Initial password: ChangeMe123!");
                }
                else
                {
                    string names = string.Join(",", config.Fields);
                    string values = string.Join(",", config.Fields.Select(f => "@" + f));
                    string sql = "INSERT INTO " + config.Table + "(" + names;
                    if (config.OwnerColumn != null) sql += "," + config.OwnerColumn;
                    sql += ") VALUES(" + values;
                    if (config.OwnerColumn != null) sql += ",@owner";
                    sql += ")";
                    var all = config.OwnerColumn == null ? parameters :
                        parameters.Concat(new[] { Db.P("@owner", Session.UserId) }).ToArray();
                    Db.Execute(sql, all);
                    MessageBox.Show("Record created successfully.");
                }
                LoadRows();
                Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Create failed.\\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Update(object sender, EventArgs e)
        {
            if (selectedId == 0) { MessageBox.Show("Select a record from the table first."); return; }
            try
            {
                var parameters = BuildParameters().ToList();
                string set = string.Join(",", config.Fields.Select(f => f + "=@" + f));
                string sql = "UPDATE " + config.Table + " SET " + set + " WHERE Id=@id" +
                    accessPolicy.GetMutationFilter(config.OwnerColumn);
                parameters.Add(Db.P("@id", selectedId));
                parameters.AddRange(accessPolicy.GetOwnerParameters(config.OwnerColumn));
                int changed = Db.Execute(sql, parameters.ToArray());
                MessageBox.Show(changed > 0 ? "Record updated successfully." : "No record was updated.");
                LoadRows(); Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed.\\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Delete(object sender, EventArgs e)
        {
            if (selectedId == 0) { MessageBox.Show("Select a record from the table first."); return; }
            if (MessageBox.Show("Are you sure you want to delete this record?", "Confirm delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try
            {
                var parameters = new List<SqlParameter> { Db.P("@id", selectedId) };
                string sql = "DELETE FROM " + config.Table + " WHERE Id=@id" +
                    accessPolicy.GetMutationFilter(config.OwnerColumn);
                parameters.AddRange(accessPolicy.GetOwnerParameters(config.OwnerColumn));
                int changed = Db.Execute(sql, parameters.ToArray());
                MessageBox.Show(changed > 0 ? "Record deleted successfully." : "No record was deleted.");
                LoadRows(); Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete failed.\\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Clear()
        {
            selectedId = 0;
            foreach (TextBox box in inputs.Values) box.Clear();
            grid.ClearSelection();
        }
    }
}
