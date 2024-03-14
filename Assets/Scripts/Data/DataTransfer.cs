using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class DataTransfer
    {
        public static List<CharacterType> TypeCollection = new List<CharacterType>();

        public static void ClearCollections()
        {
            TypeCollection.Clear();
        }
    }
}

