using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.PrefabProvider
{
    public interface IPrefabProviderByType
    {
        /// <summary>
        /// Return prefab of pointed type without instantiating, if exists
        /// </summary>
        GameObject GetPrefab<TType>() where TType : MonoBehaviour;
    }
}
