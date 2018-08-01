using Genus2D.GameData;
using Genus2D.Networking;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgServer
{
    public class DatabaseConnection
    {

        private string _sqlConnectionString;
        private string _sqlDataSource;
		private string _sqlUsername;
		private string _sqlPassword;
        SqlConnection _connection;
        private bool _connected;

        public DatabaseConnection()
        {
            Connect();
            InitializeDatabase();
        }

        private void Connect()
        {
            try
            {
                _sqlDataSource = Server.Instance.GetSettingsElement("SqlDataSource").InnerText;
                _sqlUsername = Server.Instance.GetSettingsElement("SqlUsername").InnerText;
                _sqlPassword = Server.Instance.GetSettingsElement("SqlPassword").InnerText;

                _sqlConnectionString = "Data Source=" + _sqlDataSource + ";MultipleActiveResultSets=true;User ID=" + _sqlUsername + ";Password=" + _sqlPassword + ";";

                _connection = new SqlConnection(_sqlConnectionString);
                _connection.Open();
                _connected = true;
            }
            catch
            {
                _connected = false;
            }
        }

        public void Close()
        {
            if (_connected)
            {
                _connected = false;
                _connection.Close();
            }
        }

        private void InitializeDatabase()
        {
            try
            {
                _connection.ChangeDatabase("RpgMmoDatabase");
            }
            catch
            {
                string createDatabaseQuery = @"CREATE DATABASE RpgMmoDatabase";
                Insert(createDatabaseQuery);
                _connection.ChangeDatabase("RpgMmoDatabase");
            }

            SqlDataReader playersReader = ReadDatabaseTable("Players");
            if (playersReader == null)
            {
                CreatePlayersTable();
            }
            else
            {
                playersReader.Close();
            }
        }

        private void CreatePlayersTable()
        {
            string createTableQuery = @"CREATE TABLE Players(
                PlayerID int IDENTITY (1, 1) NOT NULL PRIMARY KEY,
                Username varchar(50),
                Password varchar(50),
                MapID int,
                Direction int,
                MapX int,
                MapY int,
                Health int,
                MaxHealth int
                )";

            // create table in database
            Insert(createTableQuery);
        }

        private void Insert(string command)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(command, _connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private SqlDataReader ReadDatabaseTable(string table)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM " + table, _connection);
                SqlDataReader rdr = cmd.ExecuteReader();
                return rdr;
            }
            catch
            {
                return null;
            }
        }

        public bool LoginQuery(string username, string password, out int playerID)
        {
            playerID = -1;
            bool login = false;

            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Players WHERE Username='" + username + "'", _connection);
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    if (password == rdr.GetString(2))
                    {
                        login = true;
                        playerID = rdr.GetInt32(0);
                    }
                }

                rdr.Close();
            }
            catch { }

            return login;
        }

        public bool InsertPlayerQuery(string username, string password, out string reason)
        {
            bool inserted = false;
            reason = "";

            if (username.Contains(" "))
            {
                reason = "Username contains spaces.";
            }
            else if (password.Contains(" "))
            {
                reason = "Password contains spaces.";
            }
            else
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Players WHERE Username='" + username + "'", _connection);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (!rdr.Read())
                    {
                        string insertQuery = "INSERT INTO Players (Username, Password, MapID, Direction, MapX, MapY, Health, MaxHealth) " +
                            "VALUES ('" + username + "', '" + password + "', " + "0, " + (int)Direction.Down + ", 0, 0, 1000, 1000" + ")";

                        Insert(insertQuery);
                        inserted = true;
                    }
                    else
                    {
                        reason = "Account already exists.";
                    }

                    rdr.Close();
                }
                catch { }
            }

            return inserted;
        }

        public bool UpdatePlayerQuery(PlayerPacket packet)
        {
            bool updated = false;

            try
            {
                string updateQuery = "UPDATE Players " +
                    "SET MapID=" + packet.MapID + "," +
                    "Direction=" + (int)packet.Direction + "," +
                    "MapX=" + packet.PositionX + "," +
                    "MapY=" + packet.PositionY +
                    //hp and max hp?
                    "WHERE Username='" + packet.Username + "'";

                Insert(updateQuery);
                updated = true;
            }
            catch {  }

            return updated;
        }

        public PlayerPacket RetrievePlayerQuery(int playerID)
        {
            PlayerPacket packet = null;

            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Players WHERE PlayerID='" + playerID + "'", _connection);
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {

                    packet = new PlayerPacket();
                    packet.PlayerID = rdr.GetInt32(0);
                    packet.Username = rdr.GetString(1);
                    packet.MapID = rdr.GetInt32(3);
                    packet.Direction = (Direction)rdr.GetInt32(4);
                    packet.PositionX = rdr.GetInt32(5);
                    packet.PositionY = rdr.GetInt32(6);
                    packet.RealX = packet.PositionX * 32;
                    packet.RealY = packet.PositionY * 32;
                }

                rdr.Close();
            }
            catch { }

            return packet;
        }

    }
}
