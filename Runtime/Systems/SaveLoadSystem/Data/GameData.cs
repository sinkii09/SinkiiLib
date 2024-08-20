using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SinkiiLib.Utilities;

namespace SinkiiLib.Systems
{
    [Serializable]
    public class GameData
    {
        public string Name;
        public PlayerData PlayerData;
    }
    public interface ISaveable
    {
        SerializableGuid Id { get; set; }
    }
    public interface IBind<TData> where TData : ISaveable
    {
        SerializableGuid Id { get; set; }
        void Bind(TData data);
    }
}
