using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomTemplateSO", menuName = "ScriptableObjects/RoomTemplateSO", order = 1)]
public class RoomTemplateSO : ScriptableObject
{
    [Serializable]
    public class Template
    {
        [HideInInspector] public string name;
        public int id;
        public string DateAdded;
        [HideInInspector] public int rows;
        [HideInInspector] public int cols;
        [HideInInspector] public List<TemplateElement> TemplateElementsFlat;

        public TemplateElement[,] TemplateElement
        {
            get
            {
                TemplateElement[,] array = new TemplateElement[rows, cols];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        array[i, j] = TemplateElementsFlat[i * cols + j];
                    }
                }
                return array;
            }
            set
            {
                rows = value.GetLength(0);
                cols = value.GetLength(1);
                TemplateElementsFlat = new List<TemplateElement>(rows * cols);
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        TemplateElementsFlat.Add(value[i, j]);
                    }
                }
            }
        }
    }

    [SerializeField]
    public List<Template> Templates;
}
