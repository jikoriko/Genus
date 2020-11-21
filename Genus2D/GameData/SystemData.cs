using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Genus2D.GameData
{
    [Serializable]
    public class SystemData
    {

        public int BaseXpCurve;
        public float XpPower;
        public float XpDivision;
        public int MaxLvl;

        public SystemData()
        {
            BaseXpCurve = 1;
            XpPower = 1.0f;
            XpDivision = 1.0f;
            MaxLvl = 1;
        }

        private static SystemData _data;
        private static SystemData LoadData()
        {
            SystemData data;
            //if (File.Exists("Data/SystemData.data"))
            //{
            //    FileStream stream = File.Open("Data/SystemData.data", FileMode.Open, FileAccess.Read);
            //    BinaryFormatter formatter = new BinaryFormatter();
            //    data = (SystemData)formatter.Deserialize(stream);
            //    stream.Close();
            //}
            if (File.Exists("Data/SystemData.xml"))
            {
                FileStream stream = File.Open("Data/SystemData.xml", FileMode.Open, FileAccess.Read);
                XmlSerializer serializer = new XmlSerializer(typeof(SystemData));
                data = (SystemData)serializer.Deserialize(stream);
                stream.Close();
            }
            else
            {
                data = new SystemData();
            }
            return data;
        }

        public static void ReloadData()
        {
            _data = LoadData();
        }

        public static void SaveData()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");

            //FileStream stream = File.Create("Data/SystemData.data");
            //BinaryFormatter formatter = new BinaryFormatter();
            //formatter.Serialize(stream, _data);
            //stream.Close();

            FileStream stream = File.Create("Data/SystemData.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(SystemData));
            serializer.Serialize(stream, _data);
            stream.Close();

        }

        public static SystemData GetData()
        {
            return _data;
        }

    }
}
