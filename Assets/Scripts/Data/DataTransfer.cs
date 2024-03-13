using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class DataTransfer
    {
        public static List<PlayerType> TypeCollection = new List<PlayerType>();

        public static void ClearCollections()
        {
            TypeCollection.Clear();
        }
    }
}

