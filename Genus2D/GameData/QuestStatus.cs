using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus2D.GameData
{
    public class QuestStatus
    {

        public int QuestID { get; private set; }
        public int Progression { get; private set; }

        public QuestStatus(int questID, int progression = 0)
        {
            QuestID = questID;
            Progression = progression;

            if (GetData() == null)
                throw new Exception("Quest ID: " + questID + " has no quest data.");
        }

        public QuestData GetData()
        {
            return QuestData.GetData(QuestID);
        }

        public bool ProgressQuest()
        {
            int numQuests = GetData().Objectives.Count;
            if (Progression < numQuests)
            {
                Progression++;
                return true;
            }
            return false;
        }

        public bool Complete()
        {
            int numQuests = GetData().Objectives.Count;
            return Progression >= numQuests;
        }

        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(BitConverter.GetBytes(QuestID), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(Progression), 0, sizeof(int));
                return stream.ToArray();
            }
        }

        public static QuestStatus FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] tempBytes = new byte[sizeof(int)];

                stream.Read(tempBytes, 0, sizeof(int));
                int questID = BitConverter.ToInt32(tempBytes, 0);

                stream.Read(tempBytes, 0, sizeof(int));
                int progression = BitConverter.ToInt32(tempBytes, 0);

                QuestStatus questStatus = new QuestStatus(questID);
                questStatus.Progression = progression;
                return questStatus;
            }
        }

    }
}
