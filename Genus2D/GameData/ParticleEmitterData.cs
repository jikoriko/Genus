using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Genus2D.GameData
{
    [Serializable]
    public class ParticleEmitterData// : IXmlSerializable
    {

        public string Name;
        public PaticleEmitterShape EmitterShape;
        public string ParticleTexture;
        public float EmissionRate;
        public float AngleMin, AngleMax;
        public float OffsetMin, OffsetMax;
        public float StartVelocity, EndVelocity;
        public float StartScale, EndScale;
        public float RotationSpeed;
        public Color4 StartColour, EndColour;
        public float MaxLife;


        public ParticleEmitterData()
        {
            Initialize("");
        }

        public ParticleEmitterData(string name)
        {
            Initialize(name);
        }

        private void Initialize(string name)
        {
            Name = name;
            EmitterShape = PaticleEmitterShape.Rectangle;
            ParticleTexture = "";
            EmissionRate = 1;
            AngleMin = 0;
            AngleMax = 360;
            OffsetMin = 0;
            OffsetMax = 0;
            StartVelocity = 1;
            EndVelocity = 1;
            StartScale = 1;
            EndScale = 1;
            RotationSpeed = 0;
            StartColour = Color4.White;
            EndColour = Color4.White;
            MaxLife = 1;
        }





        //static

        private static List<ParticleEmitterData> _emittersData;
        private static List<ParticleEmitterData> LoadEmittersData()
        {
            List<ParticleEmitterData> data = null;

            /*
            if (File.Exists("Data/ItemData.data"))
            {
                FileStream stream = File.Open("Data/ItemData.data", FileMode.Open, FileAccess.Read);
                BinaryFormatter formatter = new BinaryFormatter();
                data = (List<ItemData>)formatter.Deserialize(stream);
                stream.Close();
            }
            //*/
            //*
            if (File.Exists("Data/ParticleEmitterData.xml"))
            {
                FileStream stream = File.Open("Data/ParticleEmitterData.xml", FileMode.Open, FileAccess.Read);
                XmlSerializer serializer = new XmlSerializer(typeof(List<ParticleEmitterData>));
                data = (List<ParticleEmitterData>)serializer.Deserialize(stream);
                stream.Close();
            }
            //*/
            else
            {
                data = new List<ParticleEmitterData>();
            }

            return data;
        }

        public static void ReloadData()
        {
            _emittersData = LoadEmittersData();
        }

        public static void SaveItemData()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");

            //FileStream stream = File.Create("Data/ParticleEmitterData.data");
            //BinaryFormatter formatter = new BinaryFormatter();
            //formatter.Serialize(stream, _emittersData);
            //stream.Close();

            FileStream stream = File.Create("Data/ParticleEmitterData.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<ParticleEmitterData>));
            serializer.Serialize(stream, _emittersData);
            stream.Close();

        }

        public static void AddEmitterData(ParticleEmitterData data)
        {
            _emittersData.Add(data);
        }

        public static void RemoveEmitterData(int index)
        {
            if (index >= 0 && index < _emittersData.Count)
            {
                _emittersData.RemoveAt(index);
            }
        }

        public static ParticleEmitterData GetEmitterData(int index)
        {
            if (index >= 0 && index < _emittersData.Count)
            {
                return _emittersData[index];
            }
            return null;
        }

        public static int GetEmitterDataCount()
        {
            return _emittersData.Count;
        }

        public static List<string> GetEmitterNames()
        {
            List<string> names = new List<string>();

            for (int i = 0; i < _emittersData.Count; i++)
            {
                names.Add(_emittersData[i].Name);
            }

            return names;
        }

        /*
        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
        */
    }
}
