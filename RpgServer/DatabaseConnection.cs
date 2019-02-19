using Genus2D.GameData;
using Genus2D.Networking;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgServer
{
    public class DatabaseConnection
    {

        private string _sqlConnectionString;
        private bool _sqlite;
        SQLiteConnection _sqliteConnection;
        SqlConnection _sqlConnection;
        private bool _connected;

        public DatabaseConnection()
        {
            Connect();
            InitializeDatabase();
        }

        private void Connect()
        {
            _sqlite = Server.Instance.GetSettingsElement("SQLite").InnerText.ToLower() == "true" ? true : false;
            try
            {
                if (_sqlite)
                {
                    if (!File.Exists("Data/sv.db")) SQLiteConnection.CreateFile("Data/sv.db");
                    _sqliteConnection = new SQLiteConnection("Data Source=Data/sv.db;Version=3;");
                    _sqliteConnection.Open();
                }
                else
                {
                    _sqlConnectionString = Server.Instance.GetSettingsElement("SqlConnectionString").InnerText;
                    _sqlConnection = new SqlConnection(_sqlConnectionString);
                    _sqlConnection.Open();
                }
                _connected = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _connected = false;
            }
        }

        public void Close()
        {
            if (_connected)
            {
                _connected = false;
                if (_sqlite) _sqliteConnection.Close();
                else _sqlConnection.Close();
            }
        }

        private void InitializeDatabase()
        {
            if (_sqlite)
            {
                SQLiteDataReader playersReader = ReadDatabaseTableSQLite("Players");
                if (playersReader == null)
                {
                    Console.WriteLine("Creating tables...");
                    CreatePlayersTable();
                    Console.WriteLine("created table");
                }
                else
                {
                    playersReader.Close();
                }
            }
            else
            {
                try
                {
                    _sqlConnection.ChangeDatabase("RpgMmoDatabase");
                }
                catch
                {
                    string createDatabaseQuery = @"CREATE DATABASE RpgMmoDatabase";
                    Insert(createDatabaseQuery);
                    _sqlConnection.ChangeDatabase("RpgMmoDatabase");
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
        }

        private void CreatePlayersTable()
        {
            if (_sqlite)
            {
                string createTableQuery = @"CREATE TABLE Players(
                PlayerID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                Username TEXT,
                Password TEXT,
                MapID INTEGER,
                SpriteID INTEGER,
                Direction INTEGER,
                MapX INTEGER,
                MapY INTEGER,
                Health INTEGER,
                MaxHealth INTEGER
                )";

                // create table in database
                Insert(createTableQuery);
            }
            else
            {
                string createTableQuery = @"CREATE TABLE Players(
                PlayerID int IDENTITY (1, 1) NOT NULL PRIMARY KEY,
                Username varchar(50),
                Password varchar(50),
                MapID int,
                SpriteID int,
                Direction int,
                MapX int,
                MapY int,
                Health int,
                MaxHealth int
                )";

                // create table in database
                Insert(createTableQuery);
            }
        }

        private void Insert(string command)
        {
            try
            {
                if (_sqlite)
                {
                    SQLiteCommand cmd = new SQLiteCommand(command, _sqliteConnection);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    SqlCommand cmd = new SqlCommand(command, _sqlConnection);
                    cmd.ExecuteNonQuery();
                }
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
                SqlCommand cmd = new SqlCommand("SELECT * FROM " + table, _sqlConnection);
                SqlDataReader rdr = cmd.ExecuteReader();
                return rdr;
            }
            catch
            {
                return null;
            }
        }

        private SQLiteDataReader ReadDatabaseTableSQLite(string table)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM " + table, _sqliteConnection);
                SQLiteDataReader rdr = cmd.ExecuteReader();
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
                if (_sqlite)
                {
                    SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM Players WHERE Username='" + username + "'", _sqliteConnection);
                    SQLiteDataReader rdr = cmd.ExecuteReader();
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
                else
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Players WHERE Username='" + username + "'", _sqlConnection);
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

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
                    if (_sqlite)
                    {
                        SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM Players WHERE Username='" + username + "'", _sqliteConnection);
                        SQLiteDataReader rdr = cmd.ExecuteReader();

                        if (!rdr.Read())
                        {
                            SpawnPoint spawn = Genus2D.GameData.MapInfo.GetSpawnPoint(0);
                            if (spawn == null) spawn = new SpawnPoint(0, 0, 0, "default");
                            string insertQuery = "INSERT INTO Players (Username, Password, MapID, SpriteID, Direction, MapX, MapY, Health, MaxHealth) " +
                                "VALUES ('" + username + "', '" + password + "', " + spawn.MapID + ", 0," + (int)FacingDirection.Down + ", " + spawn.MapX + ", " + spawn.MapY + ", 1000, 1000" + ")";

                            Insert(insertQuery);
                            inserted = true;
                        }
                        else
                        {
                            reason = "Account already exists.";
                        }

                        rdr.Close();
                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand("SELECT * FROM Players WHERE Username='" + username + "'", _sqlConnection);
                        SqlDataReader rdr = cmd.ExecuteReader();

                        if (!rdr.Read())
                        {
                            string insertQuery = "INSERT INTO Players (Username, Password, MapID, SpriteID, Direction, MapX, MapY, Health, MaxHealth) " +
                                "VALUES ('" + username + "', '" + password + "', " + "0, 0, " + (int)FacingDirection.Down + ", 0, 0, 1000, 1000" + ")";

                            Insert(insertQuery);
                            inserted = true;
                        }
                        else
                        {
                            reason = "Account already exists.";
                        }

                        rdr.Close();
                    }
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
                    "SpriteID=" + packet.SpriteID + "," +
                    "Direction=" + (int)packet.Direction + "," +
                    "MapX=" + packet.PositionX + "," +
                    "MapY=" + packet.PositionY + " " +
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
                if (_sqlite)
                {
                    SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM Players WHERE PlayerID='" + playerID + "'", _sqliteConnection);
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        packet = new PlayerPacket();
                        packet.PlayerID = Convert.ToInt32(rdr["PlayerID"]);
                        packet.Username = (string)rdr["Username"];
                        packet.MapID = Convert.ToInt32(rdr["MapID"]);
                        packet.SpriteID = Convert.ToInt32(rdr["SpriteID"]);
                        packet.Direction = (FacingDirection)(Convert.ToInt32(rdr["Direction"]));
                        packet.PositionX = Convert.ToInt32(rdr["MapX"]);
                        packet.PositionY = Convert.ToInt32(rdr["MapY"]);
                        packet.RealX = packet.PositionX * 32;
                        packet.RealY = packet.PositionY * 32;

                        packet.Data = new PlayerData();
                    }

                    rdr.Close();
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Players WHERE PlayerID='" + playerID + "'", _sqlConnection);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {

                        packet = new PlayerPacket();
                        packet.PlayerID = rdr.GetInt32(0);
                        packet.Username = rdr.GetString(1);
                        packet.MapID = rdr.GetInt32(3);
                        packet.SpriteID = rdr.GetInt32(4);
                        packet.Direction = (FacingDirection)rdr.GetInt32(5);
                        packet.PositionX = rdr.GetInt32(6);
                        packet.PositionY = rdr.GetInt32(7);
                        packet.RealX = packet.PositionX * 32;
                        packet.RealY = packet.PositionY * 32;

                        packet.Data = new PlayerData();
                    }

                    rdr.Close();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return packet;
        }

    }
}
