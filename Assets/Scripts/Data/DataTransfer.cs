using Prefab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay;

namespace Gameplay
{
    public class DataTransfer
    {
        public static List<CharacterType> TypeCollection = new();

        public static void ClearCollections()
        {
            TypeCollection.Clear();
        }
    }
}

