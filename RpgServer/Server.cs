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

        private TcpListener _tcpListener;
        private int _port;
        private bool _running;

        DatabaseConnection _databaseConnection;

        private long ticks, prevTicks;
        private double _deltaTime;

        private Dictionary<int, MapInstance> _mapInstances;
        private List<GameClient> _gameClients;

        private XmlDocument _settingsXml;

        public Server()
        {
            Instance = this;

            _running = false;
            _mapInstances = new Dictionary<int, MapInstance>();
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

            XmlElement xElement = _settingsXml.DocumentElement["Port"];
            _port = int.Parse(xElement.InnerText);
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
            _tcpListener = new TcpListener(IPAddress.Any, _port);
            _tcpListener.Start();

            Thread runThread = new Thread(new ThreadStart(Run));
            runThread.Start();

            Console.WriteLine("Server Started...");
            for (int i = 0; i < MapInfo.NumberMaps(); i++)
            {
                CheckMapStart(i);
            }
            RecieveClients();
        }

        public void Stop()
        {
            if (_running)
            {
                _running = false;
                _databaseConnection.Close();
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
                    TcpClient tcpClient = _tcpListener.AcceptTcpClient();
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
            int mapID = client.GetMapID();
            CheckMapStart(mapID);
            _mapInstances[mapID].AddClient(client);
        }

        private void CheckMapStart(int mapID)
        {
            if (!_mapInstances.ContainsKey(mapID))
            {
                _mapInstances.Add(mapID, new MapInstance(this, mapID));
            }
        }

        public MapInstance GetMapInstance(int id)
        {
            if (id >= 0 && id < _mapInstances.Count)
                return _mapInstances[id];
            return null;
        }

        private void Run()
        {
            while (_running)
            {
                //calculate a time delta here for map updates
                prevTicks = ticks;
                ticks = DateTime.Now.Ticks;
                _deltaTime = (ticks - prevTicks) / 10000000.0;

                for (int i = 0; i < _mapInstances.Count; i++)
                {
                    MapInstance map = _mapInstances.ElementAt(i).Value;
                    map.Update((float)_deltaTime);
                }
            }
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
