using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinkiiLib.Systems
{

    [CreateAssetMenu(fileName = "NPCDialogue", menuName = "Dialogue System/NPC Dialogue")]
    public class NPCDialogueSO : ScriptableObject
    {
        public string npcID;
        public DialogueConversation[] conversations;
    }

    [System.Serializable]
    public class DialogueConversation
    {
        public string conversationId;
        public bool isDefault;
        public string[] questRequirements;
        public string[] itemRequirements;
        public string[] completionTriggers;
        public string[] questsToActivate;
        public string[] dialogueLines;
    }

}
