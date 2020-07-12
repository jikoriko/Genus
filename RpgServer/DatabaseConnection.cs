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
                Connected INTEGER,
                Username TEXT,
                Password TEXT,
                MapID INTEGER,
                SpriteID INTEGER,
                Direction INTEGER,
                MapX INTEGER,
                MapY INTEGER,
                OnBridge INTEGER,
                Level INTEGER,
                Experience INTEGER,
                ClassID INTEGER,
                HP INTEGER,
                MP INTEGER,
                Stamina INTEGER,
                Gold INTEGER,
                Inventory TEXT,
                Equipment TEXT,
                Quests TEXT,
                InvestmentPoints INTEGER,
                VitalityPoints INTEGER,
                InteligencePoints INTEGER,
                StrengthPoints INTEGER,
                AgilityPoints INTEGER,
                MeleeDefencePoints INTEGER,
                RangeDefencePoints INTEGER,
                MagicDefencePoints INTEGER
                )";

                // create table in database
                Insert(createTableQuery);
            }
            else
            {
                string createTableQuery = @"CREATE TABLE Players(
                PlayerID int IDENTITY (1, 1) NOT NULL PRIMARY KEY,
                Connected int,
                Username varchar(50),
                Password varchar(50),
                MapID int,
                SpriteID int,
                Direction int,
                MapX int,
                MapY int,
                OnBridge int,
                Level int,
                Experience int,
                ClassID int,
                HP int,
                MP int,
                Stamina int,
                Gold int,
                Inventory varchar,
                Equipment varchar,
                Quests varchar,
                InvestmentPoints int,
                VitalityPoints int,
                InteligencePoints int,
                StrengthPoints int,
                AgilityPoints int,
                MeleeDefencePoints int,
                RangeDefencePoints int,
                MagicDefencePoints int
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
                        if (password == rdr.GetString(3))
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
                        if (password == rdr.GetString(3))
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
                    SpawnPoint spawn = Genus2D.GameData.MapInfo.GetSpawnPoint(0);
                    if (spawn == null) spawn = new SpawnPoint(0, 0, 0, "default");
                    string insertQuery = "INSERT INTO Players (Connected, Username, Password, MapID, SpriteID, Direction, MapX, MapY, OnBridge, Level, Experience, ClassID, " +
                        "HP, MP, Stamina, Gold, Inventory, Equipment, Quests, " +
                        "InvestmentPoints, VitalityPoints, InteligencePoints, StrengthPoints, AgilityPoints, MeleeDefencePoints, RangeDefencePoints, MagicDefencePoints) " +
                        "VALUES (0, '" + username + "', '" + password + "', " + spawn.MapID + ", 0, 0, " + spawn.MapX + ", " + spawn.MapY + ", 0, 1, 0, -1, " + //set a valid class id
                            "10, 10, 10, 0, '', '0,0,0,0,0,0,0,0,0,-1,0', '', 0, 0, 0, 0, 0, 0, 0, 0)";


                    if (_sqlite)
                    {
                        SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM Players WHERE Username='" + username + "'", _sqliteConnection);
                        SQLiteDataReader rdr = cmd.ExecuteReader();

                        if (!rdr.Read())
                        {
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
                string updateQuery = "UPDATE Players SET " +
                    "MapID=" + packet.MapID + "," +
                    "SpriteID=" + packet.SpriteID + "," +
                    "Direction=" + (int)packet.Direction + "," +
                    "MapX=" + packet.PositionX + "," +
                    "MapY=" + packet.PositionY + "," +
                    "OnBridge=" + (packet.OnBridge ? 1 : 0) + "," +
                    "Level=" + packet.Data.Level + "," +
                    "Experience=" + packet.Data.Experience + "," +
                    "ClassID=" + packet.Data.GetClassID() + "," +
                    "HP=" + packet.Data.HP + "," +
                    "MP=" + packet.Data.MP + "," +
                    "Stamina=" + packet.Data.Stamina + "," +
                    "Gold=" + packet.Data.Gold + "," +
                    "Inventory='" + packet.Data.GetInventoryString() + "'," +
                    "Equipment='" + packet.Data.GetEquipmentString() + "'," +
                    "Quests='" + packet.Data.GetQuestsString() + "'," +
                    "InvestmentPoints=" + packet.Data.InvestmentPoints + "," +
                    "VitalityPoints=" + packet.Data.InvestedStats.Vitality + "," +
                    "InteligencePoints=" + packet.Data.InvestedStats.Inteligence + "," +
                    "StrengthPoints=" + packet.Data.InvestedStats.Strength  + "," +
                    "AgilityPoints=" + packet.Data.InvestedStats.Agility + "," +
                    "MeleeDefencePoints=" + packet.Data.InvestedStats.MeleeDefence + "," +
                    "RangeDefencePoints=" + packet.Data.InvestedStats.RangeDefence  + "," +
                    "MagicDefencePoints=" + packet.Data.InvestedStats.MagicDefence + " " +
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
                        packet.OnBridge = Convert.ToInt32(rdr["OnBridge"]) == 1 ? true : false;
                        packet.Data = new PlayerData();
                        packet.Data.Level = Convert.ToInt32(rdr["Level"]);
                        packet.Data.Experience = Convert.ToInt32(rdr["Experience"]);
                        packet.Data.SetClassID(Convert.ToInt32(rdr["ClassID"]));
                        packet.Data.HP = Convert.ToInt32(rdr["HP"]);
                        packet.Data.MP = Convert.ToInt32(rdr["MP"]);
                        packet.Data.Stamina = Convert.ToInt32(rdr["Stamina"]);
                        packet.Data.Gold = Convert.ToInt32(rdr["Gold"]);
                        packet.Data.ParseInventoryString((string)rdr["Inventory"]);
                        packet.Data.ParseEquipmentString((string)rdr["Equipment"]);
                        packet.Data.ParseQuestsString((string)rdr["Quests"]);
                        packet.Data.InvestedStats.Vitality = Convert.ToInt32(rdr["VitalityPoints"]);
                        packet.Data.InvestedStats.Inteligence = Convert.ToInt32(rdr["InteligencePoints"]);
                        packet.Data.InvestedStats.Strength = Convert.ToInt32(rdr["StrengthPoints"]);
                        packet.Data.InvestedStats.Agility = Convert.ToInt32(rdr["AgilityPoints"]);
                        packet.Data.InvestedStats.MeleeDefence = Convert.ToInt32(rdr["MeleeDefencePoints"]);
                        packet.Data.InvestedStats.RangeDefence = Convert.ToInt32(rdr["RangeDefencePoints"]);
                        packet.Data.InvestedStats.MagicDefence = Convert.ToInt32(rdr["MagicDefencePoints"]);
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
                        packet.PlayerID = Convert.ToInt32(rdr["PlayerID"]);
                        packet.Username = (string)rdr["Username"];
                        packet.MapID = Convert.ToInt32(rdr["MapID"]);
                        packet.SpriteID = Convert.ToInt32(rdr["SpriteID"]);
                        packet.Direction = (FacingDirection)(Convert.ToInt32(rdr["Direction"]));
                        packet.PositionX = Convert.ToInt32(rdr["MapX"]);
                        packet.PositionY = Convert.ToInt32(rdr["MapY"]);
                        packet.RealX = packet.PositionX * 32;
                        packet.RealY = packet.PositionY * 32;
                        packet.OnBridge = Convert.ToInt32(rdr["OnBridge"]) == 1 ? true : false;
                        packet.Data = new PlayerData();
                        packet.Data.Level = Convert.ToInt32(rdr["Level"]);
                        packet.Data.Experience = Convert.ToInt32(rdr["Experience"]);
                        packet.Data.SetClassID(Convert.ToInt32(rdr["ClassID"]));
                        packet.Data.HP = Convert.ToInt32(rdr["HP"]);
                        packet.Data.MP = Convert.ToInt32(rdr["MP"]);
                        packet.Data.Stamina = Convert.ToInt32(rdr["Stamina"]);
                        packet.Data.Gold = Convert.ToInt32(rdr["Gold"]);
                        packet.Data.ParseInventoryString((string)rdr["Inventory"]);
                        packet.Data.ParseEquipmentString((string)rdr["Equipment"]);
                        packet.Data.ParseQuestsString((string)rdr["Quests"]);
                        packet.Data.InvestedStats.Vitality = Convert.ToInt32(rdr["VitalityPoints"]);
                        packet.Data.InvestedStats.Inteligence = Convert.ToInt32(rdr["InteligencePoints"]);
                        packet.Data.InvestedStats.Strength = Convert.ToInt32(rdr["StrengthPoints"]);
                        packet.Data.InvestedStats.Agility = Convert.ToInt32(rdr["AgilityPoints"]);
                        packet.Data.InvestedStats.MeleeDefence = Convert.ToInt32(rdr["MeleeDefencePoints"]);
                        packet.Data.InvestedStats.RangeDefence = Convert.ToInt32(rdr["RangeDefencePoints"]);
                        packet.Data.InvestedStats.MagicDefence = Convert.ToInt32(rdr["MagicDefencePoints"]);
                    }

                    rdr.Close();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            return packet;
        }

    }
}
