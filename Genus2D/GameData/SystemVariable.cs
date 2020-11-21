using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Genus2D.GameData
{
    [Serializable]
    public class SystemVariable : IXmlSerializable
    {

        public string Name;
        public VariableType Type { get; private set; }
        public object Value { get; private set; }

        public SystemVariable()
        {
            Name = "";
            Type = VariableType.Integer;
            Value = 0;
        }
        
        public SystemVariable(string name)
        {
            Name = name;
            Type = VariableType.Integer;
            Value = 0;
        }

        public void SetVariableType(VariableType type)
        {
            if (Type == type)
                return;
            Type = type;
            switch (type)
            {
                case VariableType.Integer:
                    Value = 0;
                    break;
                case VariableType.Float:
                    Value = 0.0f;
                    break;
                case VariableType.Bool:
                    Value = false;
                    break;
                case VariableType.Text:
                    Value = "";
                    break;
            }
        }

        public bool SetValue(object value)
        {
            switch (Type)
            {
                case VariableType.Integer:
                    return SetValue(((int)value).ToString());
                case VariableType.Float:
                    return SetValue(((float)value).ToString());
                case VariableType.Bool:
                    return SetValue(((bool)value).ToString());
                case VariableType.Text:
                    return SetValue((string)value);
            }
            return false;
        }

        public bool SetValue(string valueString)
        {
            bool valueSet = true;
            object value;
            try
            {
                switch (Type)
                {
                    case VariableType.Integer:
                        value = int.Parse(valueString);
                        Value = value;
                        break;
                    case VariableType.Float:
                        value = float.Parse(valueString);
                        Value = value;
                        break;
                    case VariableType.Bool:
                        valueString = valueString.ToLower();
                        if (valueString == "true")
                            value = true;
                        else if (valueString == "false")
                            value = false;
                        else
                            throw new Exception("Incorrect bool text.");
                        Value = value;
                        break;
                    case VariableType.Text:
                        Value = valueString;
                        break;
                }
            }
            catch
            {
                valueSet = false;
            }
            return valueSet;
        }

        public XmlSchema GetSchema()
        {
            return (null);
        }

        public void ReadXml(XmlReader reader)
        {
            string xml = reader.ReadOuterXml();
            reader = XmlReader.Create(new StringReader(xml));

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == "Name")
                    {
                        reader.Read();
                        Name = reader.ReadContentAsString();
                    }
                    else if (reader.LocalName == "VariableType")
                    {
                        reader.Read();
                        Type = (VariableType)Enum.Parse(typeof(VariableType), reader.ReadContentAsString());
                    }
                    else if (reader.LocalName == "Value")
                    {
                        reader.Read();
                        SetValue(reader.ReadContentAsString());
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Name");
            writer.WriteString(Name);
            writer.WriteEndElement();

            writer.WriteStartElement("VariableType");
            writer.WriteString(Type.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Value");
            writer.WriteString(Value.ToString());
            writer.WriteEndElement();
        }





        private static List<SystemVariable> _systemVariables;

        private static List<SystemVariable> LoadData()
        {
            List<SystemVariable> data;
            //if (File.Exists("Data/SystemVariables.data"))
            //{
            //    FileStream stream = File.Open("Data/SystemVariables.data", FileMode.Open, FileAccess.Read);
            //    BinaryFormatter formatter = new BinaryFormatter();
            //    variables = (List<SystemVariable>)formatter.Deserialize(stream);
            //    stream.Close();
            //}
            if (File.Exists("Data/SystemVariables.xml"))
            {
                FileStream stream = File.Open("Data/SystemVariables.xml", FileMode.Open, FileAccess.Read);
                XmlSerializer serializer = new XmlSerializer(typeof(List<SystemVariable>));
                data = (List<SystemVariable>)serializer.Deserialize(stream);
                stream.Close();
            }
            else
            {
                data = new List<SystemVariable>();
            }
            return data;
        }

        public static void ReloadData()
        {
            _systemVariables = LoadData();
        }

        public static void SaveData()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");

            //FileStream stream = File.Create("Data/SystemVariables.data");
            //BinaryFormatter formatter = new BinaryFormatter();
            //formatter.Serialize(stream, _systemVariables);
            //stream.Close();

            FileStream stream = File.Create("Data/SystemVariables.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<SystemVariable>));
            serializer.Serialize(stream, _systemVariables);
            stream.Close();
        }

        public static void AddSystemVariable(SystemVariable variable)
        {
            if (variable != null)
                _systemVariables.Add(variable);
        }

        public static void RemoveSystemVariable(int index)
        {
            if (index >= 0 && index < _systemVariables.Count)
                _systemVariables.RemoveAt(index);
        }

        public static SystemVariable GetSystemVariable(int index)
        {
            if (index >= 0 && index < _systemVariables.Count)
                return _systemVariables[index];
            return null;
        }

        public static int SystemVariablesCount()
        {
            return _systemVariables.Count;
        }

        public static List<string> GetVariableNames()
        {
            List<string> names = new List<string>();

            for (int i = 0; i < _systemVariables.Count; i++)
            {
                names.Add(_systemVariables[i].Name);
            }

            return names;
        }

    }
}
