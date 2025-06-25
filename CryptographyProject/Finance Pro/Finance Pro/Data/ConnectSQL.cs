using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Finance_Pro.Data
{
    public class ConnectSQL : IDisposable
    {
        // Chuỗi kết nối cơ sở dữ liệu
        private static string IP = "localhost";
        private static string port = "3306";
        private static string database = "sqlfinancepro";
        private readonly string connectionString = $"Server={IP};Port={port};Database={database};Uid=root;Pwd=;";

        private MySqlConnection connection;
        private bool disposed = false;

        public ConnectSQL()
        {
            connection = new MySqlConnection(connectionString);
            connection.Open(); // Mở kết nối ngay khi khởi tạo
        }

        // Thêm phương thức GetConnection để lấy kết nối
        public MySqlConnection GetConnection()
        {
            return connection;
        }

        // Thực hiện câu truy vấn trả về kết quả
        public List<Dictionary<string, object>> ExecuteQuery(string query)
        {
            var results = new List<Dictionary<string, object>>();

            using (var cmd = new MySqlCommand(query, connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.GetValue(i);
                        }

                        results.Add(row);
                    }
                }
            }
            return results;
        }

        // Thực hiện câu lệnh không trả về kết quả
        public int ExecuteNonQuery(string query)
        {
            using (var cmd = new MySqlCommand(query, connection))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        // Dispose để đóng kết nối
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Giải phóng kết nối
                    if (connection != null)
                    {
                        connection.Close();
                        connection.Dispose();
                        connection = null;
                    }
                }

                disposed = true;
            }
        }

        ~ConnectSQL()
        {
            Dispose(false);
        }
    }
}
