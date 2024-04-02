using Prefab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class RawMapper
    {
        public int Speed;
        public ICharacterController Controller;
    }
    public class DataTransfer
    {
        public static List<CharacterType> TypeCollection = new();
        public static List<RawMapper> RawMappers = new List<RawMapper>();

        public static void ClearCollections()
        {
            RawMappers.Clear();
            TypeCollection.Clear();
        }
    }
}

