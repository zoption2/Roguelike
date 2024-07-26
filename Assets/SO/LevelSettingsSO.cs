using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSetingsSO", menuName = "ScriptableObjects/LevelSetingsSO", order = 2)]

public class LevelSetingsSO : ScriptableObject
{
    public List<TypeOfScenario> RoomsOrder = new List<TypeOfScenario>();
}
