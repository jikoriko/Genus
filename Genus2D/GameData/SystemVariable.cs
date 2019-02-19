using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    [Serializable]
    public class SystemVariable
    {

        public string Name;
        public VariableType Type { get; private set; }
        public object Value { get; private set; }

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


        private static List<SystemVariable> _systemVariables = LoadData();

        private static List<SystemVariable> LoadData()
        {
            List<SystemVariable> variables;
            if (File.Exists("Data/SystemVariables.data"))
            {
                FileStream stream = File.Open("Data/SystemVariables.data", FileMode.Open, FileAccess.Read);
                BinaryFormatter formatter = new BinaryFormatter();
                variables = (List<SystemVariable>)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                variables = new List<SystemVariable>();
            }
            return variables;
        }

        public static void ReloadData()
        {
            _systemVariables = LoadData();
        }

        public static void SaveData()
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            FileStream stream = File.Create("Data/SystemVariables.data");
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, _systemVariables);
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
