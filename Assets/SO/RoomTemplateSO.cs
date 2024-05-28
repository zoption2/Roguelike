using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RoomTemplateSO : ScriptableObject
{
    [Serializable]
    public class Template
    {
        [HideInInspector] public string name;
        public int id;
        public TemplateElement[,] TemplateElement;
    }

    [SerializeField]
    public List<Template> Templates;
}
