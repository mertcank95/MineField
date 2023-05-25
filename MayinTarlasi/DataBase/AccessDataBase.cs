using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MayinTarlasi.Models;
using MayinTarlasi.DataBase.Contract;

namespace MayinTarlasi.DataBase
{
    public class AccessDataBase : IDataBase
    {
        readonly string connectionString;

        public AccessDataBase()
        {
            connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source = " + Application.StartupPath + "\\Veritabani.mdb";
        }

        public void DataInsert(Player player)
        {
            try
            {                
                string insertQuery = "INSERT INTO Score_tbl (Name, Score, Datetime) VALUES (@name, @score, @datetime)";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    if (!CheckTable(connection, "Score_tbl"))
                    {
                        CreateTable(connection);
                    }
                    using (OleDbCommand command = new OleDbCommand(insertQuery, connection))
                    {
                        OleDbParameter[] parameters = new OleDbParameter[]
                        {
                            new OleDbParameter("@name", player.Name),
                            new OleDbParameter("@score", player.Score),
                            new OleDbParameter("@datetime", DateTime.Now.Date)
                        };
                        command.Parameters.AddRange(parameters);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (OleDbException ex)
            {
                Console.WriteLine("An OleDbException occurred: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred: " + ex.Message);
            }
        }

        public DataTable DataLoad()
        {
            try
            {
                string queryString = "SELECT Name,Score,MyDatetime FROM Score_tbl ORDER BY Score DESC";
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    if (!CheckTable(connection, "Score_tbl"))
                    {
                        CreateTable(connection);
                    }
                   
                    using (OleDbCommand command = new OleDbCommand(queryString, connection))
                    {
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        private bool CheckTable(OleDbConnection connection, string tableName)
        {
            bool tableExists = false;

            connection.Open();
            using (OleDbCommand command = new OleDbCommand())
            {
                command.Connection = connection;
                var tableSchema = connection.GetSchema("Tables");
                var tableNames = tableSchema.AsEnumerable().Select(r => r["TABLE_NAME"].ToString());
                tableExists = tableNames.Contains(tableName, StringComparer.OrdinalIgnoreCase);
            }
            connection.Close();
            return tableExists;
        }

        public void CreateTable(OleDbConnection connection)
        {
            using (OleDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE Score_tbl (ID INT PRIMARY KEY, Name TEXT, Score Number, MyDatetime DateTime)";
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }


}

