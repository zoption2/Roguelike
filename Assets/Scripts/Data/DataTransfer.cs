using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class DataTransfer
    {
        public static List<int> IdCollection = new List<int>();
        public static List<PlayerType> TypeCollection = new List<PlayerType>();

        public static void ClearCollections()
        {
            IdCollection.Clear();
            TypeCollection.Clear();
        }
    }
}

