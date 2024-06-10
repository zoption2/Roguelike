using System.Collections.Generic;
using System;
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
        [HideInInspector] public List<TemplateElementType> TemplateElementsFlat;
        [HideInInspector] public List<Vector3> CoordinatesFlat;

        public TemplateElementType[,] TemplateElement
        {
            get
            {
                TemplateElementType[,] array = new TemplateElementType[rows, cols];
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
                TemplateElementsFlat = new List<TemplateElementType>(rows * cols);
                CoordinatesFlat = new List<Vector3>(rows * cols);
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        TemplateElementsFlat.Add(value[i, j]);
                        CoordinatesFlat.Add(new Vector3(i, 0, j));
                    }
                }
            }
        }

        public Vector3[,] Coordinates
        {
            get
            {
                Vector3[,] array = new Vector3[rows, cols];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        array[i, j] = CoordinatesFlat[i * cols + j];
                    }
                }
                return array;
            }
        }

        [HideInInspector] public TemplateElementType leftExit;
        [HideInInspector] public TemplateElementType rightExit;
        [HideInInspector] public TemplateElementType topExit;
        [HideInInspector] public TemplateElementType bottomExit;

        public bool HasExit(Vector3 direction)
        {
            if (direction == Vector3.left && rightExit == TemplateElementType.Exit) return true;
            if (direction == Vector3.right && leftExit == TemplateElementType.Exit) return true;
            if (direction == Vector3.up && bottomExit == TemplateElementType.Exit) return true;
            if (direction == Vector3.down && topExit == TemplateElementType.Exit) return true;
            return false;
        }
    }

    [SerializeField]
    public List<Template> Templates;
}
