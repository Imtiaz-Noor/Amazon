using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amazon_categories
{
    class Operation
    {
        static int result = 0;
        static SqlDataAdapter sda = null;
        public static DataTable dt = null;
        static SqlConnection conn = null;
        private static string conn_String = @"Data Source=DESKTOP-HHRN7UL;Initial Catalog=Amazon;Integrated Security=True";
        SqlConnection sqlConnection = new SqlConnection(@"Data Source=DESKTOP-HHRN7UL;Initial Catalog=Amazon;Integrated Security=True");
        public static SqlConnection connection_Open()
        {
            conn = new SqlConnection(conn_String);
            conn.Open();
            return conn;
        }
        public static void connection_Close()
        {
            conn.Close();
        }
        public static bool DB_Operation(String query)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = connection_Open();
                result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (SqlException ex)
            {
                connection_Close();
                //MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                connection_Close();
            }

        }

        public DataTable grid_view()
        {
            string query = "Select * from Complete_Urls Where Status = 0";
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            SqlDataAdapter sqlData = new SqlDataAdapter(sqlCommand);
            DataTable dataTable = new DataTable();
            sqlData.Fill(dataTable);
            return dataTable;

        }
    }
}
