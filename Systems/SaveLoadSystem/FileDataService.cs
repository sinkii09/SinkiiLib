using SinkiiLib.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SinkiiLib.Systems
{
    public class FileDataService : IDataService
    {
        string filePath;
        string fileExtension;
        JsonSerializer serializer;

        
        public FileDataService(JsonSerializer serializer)
        {
            filePath = Application.persistentDataPath;
            fileExtension = "json";
            this.serializer = serializer;
        }
        string GetPathToFile(string fileName)
        {
            return Path.Combine(filePath, string.Concat(fileName,".",fileExtension));
        }
        public GameData Load(string name)
        {
            string fileLocation = GetPathToFile(name);

            if (!File.Exists(fileLocation))
            {
                throw new ArgumentException($"No persisted GameData with name '{name}'");
            }
            string encryptedData = File.ReadAllText(fileLocation);
            string decryptedData = DataEncryption.Decrypt(encryptedData);
            return serializer.Deserialize<GameData>(decryptedData);
        }

        public void Save(GameData data, bool overwrite = true)
        {
            string fileLocation = GetPathToFile(data.Name);

            if (!overwrite && File.Exists(fileLocation))
            {
                throw new IOException($"The file '{data.Name}.{fileExtension}' already exists and cannot be overwritten.");
            }
            string jsonData = serializer.Serialize(data);

            string encryptedData = DataEncryption.Encrypt(jsonData);

            File.WriteAllText(fileLocation, encryptedData);
        }
        public void Delete(string name)
        {
            string fileLocation = GetPathToFile(name);

            if (File.Exists(fileLocation))
            {
                File.Delete(fileLocation);
            }
        }

        public void DeleteAll()
        {
            foreach (string filePath in Directory.GetFiles(filePath))
            {
                File.Delete(filePath);
            }
        }

        public IEnumerable<string> ListSaves()
        {
            foreach (string path in Directory.EnumerateFiles(filePath))
            {
                if (Path.GetExtension(path) == fileExtension)
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
        }

    }
}
