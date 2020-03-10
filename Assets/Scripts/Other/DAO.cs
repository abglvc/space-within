using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Other {
    public class DAO {
        public DataStorage data;

        public DAO() {
            data = Load();
        }

        public void Save() {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/dataStorage.dat", FileMode.OpenOrCreate);
            bf.Serialize(file, data);
            file.Close();
        }

        private DataStorage Load() {
            //File.Delete(Application.persistentDataPath+"/dataStorage.dat");
            if (File.Exists(Application.persistentDataPath + "/dataStorage.dat")) {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/dataStorage.dat", FileMode.Open);
                DataStorage dataStorage = (DataStorage) bf.Deserialize(file);
                file.Close();
                return dataStorage;
            }
            else return new DataStorage();
        }
    }
}