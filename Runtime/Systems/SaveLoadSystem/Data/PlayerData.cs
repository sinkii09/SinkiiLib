using SinkiiLib.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinkiiLib.Systems
{
    [Serializable]
    public class PlayerData : ISaveable
    {
        [SerializeField] public SerializableGuid Id { get; set; }
        public Vector3 position;
    }
}
