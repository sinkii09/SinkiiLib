using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;


namespace SinkiiLib.Systems
{
    public class DialogueExporter : EditorWindow
    {
        private const string CSV_FOLDER_PATH = "Assets/DialogueCSVs/";

        [MenuItem("Tools/Dialogue System/Export All NPC Dialogues")]
        public static void ExportAllDialoguesToCSV()
        {
            // Ensure the folder for CSV files exists
            if (!Directory.Exists(CSV_FOLDER_PATH))
                Directory.CreateDirectory(CSV_FOLDER_PATH);

            // Find all DialogueScriptableObject assets in the project
            NPCDialogueSO[] allDialogueObjects = Resources.LoadAll<NPCDialogueSO>("");

            foreach (var dialogueObject in allDialogueObjects)
            {
                string csvFilePath = $"{CSV_FOLDER_PATH}{dialogueObject.npcID}.csv";
                WriteDialogueToCSV(dialogueObject, csvFilePath);
            }

            Debug.Log("All dialogues exported/updated successfully.");
        }

        private static void WriteDialogueToCSV(NPCDialogueSO dialogueObject, string csvFilePath)
        {
            StringBuilder csvContent = new StringBuilder();

            // Write CSV header
            csvContent.AppendLine("conversationId,isDefault,questRequirements,itemRequirements,completionTriggers,questsToActivate,dialogueLines");

            // Loop through conversations and write their data
            foreach (var conversation in dialogueObject.conversations)
            {
                string questRequirements = string.Join(";", conversation.questRequirements);
                string itemRequirements = string.Join(";", conversation.itemRequirements);
                string completionTriggers = string.Join(";", conversation.completionTriggers);
                string questsToActivate = string.Join(";", conversation.questsToActivate);
                string dialogueLines = "\"" + string.Join("\",\"", conversation.dialogueLines) + "\"";

                csvContent.AppendLine(
                    $"{conversation.conversationId},{conversation.isDefault},{questRequirements},{itemRequirements},{completionTriggers},{questsToActivate},{dialogueLines}"
                );
            }

            // Write or update the CSV file
            File.WriteAllText(csvFilePath, csvContent.ToString(), Encoding.UTF8);
        }
    }

}

