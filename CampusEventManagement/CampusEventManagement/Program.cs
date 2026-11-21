using System;
using System.Windows.Forms;

namespace CampusEventManagement
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Db.Initialize();
                Application.Run(new LoginForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Application could not start.\n\n" + ex.Message +
                    "\n\nCheck SQL Server connection in Db.cs and run Database.sql.",
                    "Startup error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}