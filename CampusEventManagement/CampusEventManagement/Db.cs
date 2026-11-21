using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace CampusEventManagement
{
    static class Db
    {
        // Change this if your SQL Server instance has a different name.
        public const string Server = @".\SQLEXPRESS";
        public const string Database = "CampusEventDB";
        static string MasterConnection { get { return @"Data Source=" + Server + ";Initial Catalog=master;Integrated Security=True;TrustServerCertificate=True;"; } }
        public static string ConnectionString { get { return @"Data Source=" + Server + ";Initial Catalog=" + Database + ";Integrated Security=True;TrustServerCertificate=True;"; } }

        public static void Initialize()
        {
            using (var c = new SqlConnection(MasterConnection))
            using (var cmd = new SqlCommand("IF DB_ID(@db) IS NULL EXEC('CREATE DATABASE " + Database + "')", c))
            {
                cmd.Parameters.AddWithValue("@db", Database);
                c.Open(); cmd.ExecuteNonQuery();
            }
            using (var c = new SqlConnection(ConnectionString))
            {
                c.Open();
                string sql = System.IO.File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database.sql"));
                foreach (string batch in sql.Split(new[] { "\nGO", "\ngo" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    using (var cmd = new SqlCommand(batch, c)) cmd.ExecuteNonQuery();
                }
            }
            EnsureUser("System Administrator", "admin", "admin@campus.local", "Admin");
            EnsureUser("Faculty Coordinator", "faculty", "faculty@campus.local", "Faculty");
        }

        static void EnsureUser(string name, string username, string email, string role)
        {
            if (Scalar("SELECT COUNT(*) FROM Users WHERE Username=@u", P("@u", username)) == 0)
                Execute(@"INSERT INTO Users(FullName,Username,Email,PasswordHash,Role,IsActive)
                          VALUES(@n,@u,@e,@p,@r,1)",
                    P("@n", name), P("@u", username), P("@e", email),
                    P("@p", Hash("ChangeMe123!")), P("@r", role));
        }

        public static string Hash(string value)
        {
            using (var sha = SHA256.Create())
                return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(value ?? "")));
        }
        public static SqlParameter P(string name, object value) { return new SqlParameter(name, value ?? DBNull.Value); }
        public static DataTable Query(string sql, params SqlParameter[] p)
        {
            using (var c = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, c))
            using (var da = new SqlDataAdapter(cmd))
            {
                if (p != null) cmd.Parameters.AddRange(p);
                var dt = new DataTable(); da.Fill(dt); return dt;
            }
        }
        public static int Execute(string sql, params SqlParameter[] p)
        {
            using (var c = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, c))
            {
                if (p != null) cmd.Parameters.AddRange(p);
                c.Open(); return cmd.ExecuteNonQuery();
            }
        }
        public static int Scalar(string sql, params SqlParameter[] p)
        {
            using (var c = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, c))
            {
                if (p != null) cmd.Parameters.AddRange(p);
                c.Open(); return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }
}