using Genus2D.GameData;
using Genus2D.Networking;
using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml;

namespace RpgServer
{
    public class Server
    {
        public static Server Instance { get; private set; }

        private TcpListener _tcpServerListener;
        private int _serverPort;
        private bool _running;

        DatabaseConnection _databaseConnection;

        private long ticks, prevTicks;
        private double _deltaTime;

        private Dictionary<int, ServerMap> _serverMaps;
        private List<GameClient> _gameClients;

        private XmlDocument _settingsXml;

        private static Thread _serverThread;

        public Server()
        {
            Instance = this;

            _running = false;
            _serverMaps = new Dictionary<int, ServerMap>();
            _gameClients = new List<GameClient>();

            _deltaTime = 0.0;
            ticks = DateTime.Now.Ticks;
            prevTicks = ticks;
        }

        public void AddGameClient(GameClient client)
        {
            if (!_gameClients.Contains(client))
                _gameClients.Add(client);
        }

        public void RemoveGameClient(GameClient client)
        {
            _gameClients.Remove(client);
        }

        public GameClient FindClientByID(int id)
        {
            for (int i = 0; i < _gameClients.Count; i++)
            {
                if (_gameClients[i].GetPacket().PlayerID == id)
                    return _gameClients[i];
            }
            return null;
        }

        public void Start()
        {
            if (!_running)
            {
                _running = true;

                LoadXmlSettings();
                _databaseConnection = new DatabaseConnection();
                StartServer();
            }
        }

        private void LoadXmlSettings()
        {
            _settingsXml = new XmlDocument();
            _settingsXml.Load("Data/ServerSettings.xml");

            XmlElement xElement = _settingsXml.DocumentElement["ServerPort"];
            _serverPort = int.Parse(xElement.InnerText);
        }

        public XmlElement GetSettingsElement(string name)
        {
            XmlElement xElement = _settingsXml.DocumentElement[name];
            return xElement;
        }

        public DatabaseConnection GetDatabaseConnection()
        {
            return _databaseConnection;
        }

        private void StartServer()
        {
            _tcpServerListener = new TcpListener(IPAddress.Any, _serverPort);
            _tcpServerListener.Start();

            /*
            for (int i = 0; i < MapInfo.NumberMaps(); i++)
            {
                CheckMapStart(i);
            }
            */

            _serverThread = new Thread(new ThreadStart(Run));
            _serverThread.Start();

            Console.WriteLine("Server Started...");
            RecieveClients();
        }

        public int ServerPort()
        {
            return _serverPort;
        }

        public void Stop()
        {
            if (_running)
            {
                _running = false;
            }
        }

        public float GetDeltaTime()
        {
            return (float)_deltaTime;
        }

        private void RecieveClients()
        {
            while (_running)
            {
                try
                {
                    TcpClient tcpClient = _tcpServerListener.AcceptTcpClient();
                    GameClient.RecieveClient(tcpClient);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }

        public void ChangeClientsMap(GameClient client)
        {
            int mapID = client.GetMapPlayer().GetMapID();
            CheckMapStart(mapID);
            _serverMaps[mapID].AddClient(client);
        }

        private void CheckMapStart(int mapID)
        {
            if (!_serverMaps.ContainsKey(mapID))
            {
                _serverMaps.Add(mapID, new ServerMap(this, mapID));
            }
        }

        public ServerMap GetServerMap(int mapID)
        {
            if (mapID >= 0 && mapID < _serverMaps.Count)
                return _serverMaps[mapID];
            return null;
        }

        private void Run()
        {
            while (_running)
            {
                try
                {
                    //calculate a time delta here for map updates
                    prevTicks = ticks;
                    ticks = DateTime.Now.Ticks;
                    _deltaTime = (ticks - prevTicks) / 10000000.0;

                    for (int i = 0; i < _serverMaps.Count; i++)
                    {
                        ServerMap map = _serverMaps.ElementAt(i).Value;
                        map.Update((float)_deltaTime);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Stop();
                }
            }

            while (_gameClients.Count > 0)
            {
                _gameClients[0].Disconnect();

            }
            _databaseConnection.Close();
        }

        public Thread GetServerThread()
        {
            return _serverThread;
        }

        public void SendMessage(MessagePacket message)
        {
            for (int i = 0; i < _gameClients.Count; i++)
            {
                _gameClients[i].RecieveMessage(message);
            }
        }

    }
}
