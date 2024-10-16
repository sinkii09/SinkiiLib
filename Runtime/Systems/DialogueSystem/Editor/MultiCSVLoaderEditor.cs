using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace SinkiiLib.Systems
{

    public class MultiCSVLoaderEditor : EditorWindow
    {
        private string csvFolderPath = "Assets/NPCDialogueCSVs/";
        private string outputPath = "Assets/NPCDialogues/";

        [MenuItem("Tools/Dialogue System/Load All NPC Dialogues")]
        public static void ShowWindow()
        {
            GetWindow<MultiCSVLoaderEditor>("NPC Dialogue Loader");
        }

        private void OnGUI()
        {
            GUILayout.Label("NPC Dialogue Loader", EditorStyles.boldLabel);

            csvFolderPath = EditorGUILayout.TextField("CSV Folder Path:", csvFolderPath);
            outputPath = EditorGUILayout.TextField("Output Path:", outputPath);

            if (GUILayout.Button("Load All NPC Dialogues"))
            {
                LoadAllCSVFiles();
            }
        }

        private void LoadAllCSVFiles()
        {
            if (!Directory.Exists(csvFolderPath))
            {
                Debug.LogError($"CSV folder path '{csvFolderPath}' does not exist.");
                return;
            }

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            string[] csvFiles = Directory.GetFiles(csvFolderPath, "*.csv");

            foreach (string csvFile in csvFiles)
            {
                string npcID = Path.GetFileNameWithoutExtension(csvFile);
                LoadCSVToScriptableObject(npcID, csvFile);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("All NPC CSV files have been loaded into ScriptableObjects.");
        }

        private void LoadCSVToScriptableObject(string npcID, string csvFilePath)
        {
            List<DialogueConversation> conversations = new List<DialogueConversation>();

            string[] lines = File.ReadAllLines(csvFilePath);

            for (int i = 1; i < lines.Length; i++) // Skip header
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] columns = ParseCSVLine(lines[i]);

                DialogueConversation convo = new DialogueConversation
                {
                    conversationId = columns[0],
                    isDefault = bool.Parse(columns[1]),
                    questRequirements = columns[2].Split('|'),
                    itemRequirements = columns[3].Split('|'),
                    completionTriggers = columns[4].Split('|'),
                    questsToActivate = columns[5].Split('|'),
                    dialogueLines = columns[6].Split('|')
                };

                conversations.Add(convo);
            }

            NPCDialogueSO npcDialogueSO = LoadOrCreateNPCDialogueSO(npcID);
            npcDialogueSO.conversations = conversations.ToArray();

            string assetPath = $"{outputPath}{npcID}_Dialogue.asset";
            if (AssetDatabase.GetAssetPath(npcDialogueSO) == "")
                AssetDatabase.CreateAsset(npcDialogueSO, assetPath);

            EditorUtility.SetDirty(npcDialogueSO);
        }

        private NPCDialogueSO LoadOrCreateNPCDialogueSO(string npcID)
        {
            string assetPath = $"{outputPath}{npcID}_Dialogue.asset";
            NPCDialogueSO npcDialogueSO = AssetDatabase.LoadAssetAtPath<NPCDialogueSO>(assetPath);

            if (npcDialogueSO == null)
            {
                npcDialogueSO = CreateInstance<NPCDialogueSO>();
                npcDialogueSO.npcID = npcID;
            }

            return npcDialogueSO;
        }

        //private string[] ParseCSVLine(string line)
        //{
        //    List<string> result = new List<string>();
        //    string pattern = @"(?:^|,)(?:""(?<value>[^""]*)""|(?<value>[^,""]*))";

        //    foreach (Match match in Regex.Matches(line, pattern))
        //    {
        //        result.Add(match.Groups["value"].Value);
        //    }

        //    return result.ToArray();
        //}
        private static string[] ParseCSVLine(string line)
        {
            List<string> result = new List<string>();
            bool insideQuotes = false;
            string currentField = "";

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    insideQuotes = !insideQuotes; // Toggle the insideQuotes flag
                }
                else if (c == ',' && !insideQuotes)
                {
                    result.Add(currentField.Replace("\"", "")); // Remove surrounding quotes
                    currentField = ""; // Reset for next field
                }
                else
                {
                    currentField += c; // Add character to the current field
                }
            }

            // Add the last field
            result.Add(currentField.Replace("\"", ""));
            return result.ToArray();
        }
    }

}
