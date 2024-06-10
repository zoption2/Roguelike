using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TemplatePlacebleElements : ScriptableObject
{
    [Serializable]
    public class TemplatePlacebleElement
    {
        public string Name;
        public Color Color;
        public TemplateElementType Type;
    }

    [SerializeField]
    public List<TemplatePlacebleElement> PlacebleElements;
}
