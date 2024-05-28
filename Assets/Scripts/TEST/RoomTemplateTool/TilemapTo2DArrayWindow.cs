using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;
using System;

[System.Serializable]
public class PlaceableObject
{
    public string name;
    public Color color;
    public TemplateElement type;
}

public class TilemapTo2DArrayWindow : EditorWindow
{
    private Tilemap selectedTilemap;
    private TemplateElement[,] levelArray;
    private Color[,] cellColors;
    private bool[,] isEditableArray;
    private Vector2 scrollPosition;
    private TemplateElement tileTypes;

    private List<PlaceableObject> placeableObjects = new List<PlaceableObject>();
    private int selectedObjectIndex = -1;

    private string newObjectName = "";
    private Color newObjectColor = Color.white;
    private TemplateElement newObjectType = TemplateElement.Empty;

    private float zoomScale = 1f;

    private bool isLeftMouseDown = false;

    private string newRecordName = "";
    private bool DisplayNewTemplateRecordFields = false;


    [MenuItem("Tools/Tilemap to 2D Array")]
    public static void ShowWindow()
    {
        GetWindow<TilemapTo2DArrayWindow>("Tilemap to 2D Array");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Tilemap", EditorStyles.boldLabel);

        selectedTilemap = EditorGUILayout.ObjectField("Tilemap", selectedTilemap, typeof(Tilemap), true) as Tilemap;

        if (selectedTilemap != null && GUILayout.Button("Generate Template"))
        {
            GenerateArrayFromTilemap();
        }

        if (levelArray != null)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DisplayArrayEditor();

            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Label("Elements to build", EditorStyles.boldLabel);
        DisplayPlaceableObjects();

        GUILayout.Label("Selected element", EditorStyles.boldLabel);
        if (selectedObjectIndex != -1)
        {
            EditorGUILayout.LabelField("Name: " + placeableObjects[selectedObjectIndex].name);
            EditorGUILayout.LabelField("Type: " + placeableObjects[selectedObjectIndex].type);
            EditorGUILayout.LabelField("Color: " + placeableObjects[selectedObjectIndex].color);
        }

        GUILayout.Label("Add New Object", EditorStyles.boldLabel);
        newObjectName = EditorGUILayout.TextField("Name", newObjectName);
        newObjectType = (TemplateElement)EditorGUILayout.Popup("Type", (int)newObjectType, System.Enum.GetNames(typeof(TemplateElement)));


        newObjectColor = EditorGUILayout.ColorField("Color", newObjectColor);

        if (GUILayout.Button("Add Object"))
        {
            AddNewObject();
        }

        if (!DisplayNewTemplateRecordFields)
        {
            if (GUILayout.Button("Add New Template Record"))
            {
                DisplayNewTemplateRecordFields = true;
            }
        }
        else
        {
            GUILayout.Label("New Template Record", EditorStyles.boldLabel);
            newRecordName = EditorGUILayout.TextField("Name", newRecordName);

            if (GUILayout.Button("Create Template Record"))
            {
                CreateNewRecordInSO(newRecordName, levelArray);
                DisplayNewTemplateRecordFields = false;
                newRecordName = "";
            }
        }

    }

    private void DisplayArrayEditor()
    {
        GUILayout.BeginVertical(GUI.skin.box);

        int cellSize = Mathf.FloorToInt(20 * zoomScale);

        for (int y = levelArray.GetLength(1) - 1; y >= 0; y--)
        {
            GUILayout.BeginHorizontal();

            for (int x = 0; x < levelArray.GetLength(0); x++)
            {
                Rect rect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));

                Color buttonColor = cellColors[x, y];
                GUI.backgroundColor = buttonColor;
                if (GUI.Button(rect, GUIContent.none))
                {
                    if (selectedObjectIndex != -1)
                    {
                        TemplateElement previousType = levelArray[x, y];
                        levelArray[x, y] = placeableObjects[selectedObjectIndex].type;
                        TemplateElement newType = levelArray[x, y];
                        Debug.Log($"Cell changed from {previousType} to {newType}");
                        cellColors[x, y] = placeableObjects[selectedObjectIndex].color;
                    }
                }
                GUI.backgroundColor = Color.white;
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }

    private void GenerateArrayFromTilemap()
    {
        var bounds = GetTilemapBounds(selectedTilemap);
        Debug.Log($"Bounds: xMin={bounds.xMin}, yMin={bounds.yMin}, xMax={bounds.xMax}, yMax={bounds.yMax}");

        int width = bounds.size.x;
        int height = bounds.size.y;

        if (width == 0 || height == 0)
        {
            Debug.LogWarning("Tilemap bounds are zero. Make sure the Tilemap is not empty.");
            return;
        }

        levelArray = new TemplateElement[width, height];
        cellColors = new Color[width, height];
        isEditableArray = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int cellPosition = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                TileBase tile = selectedTilemap.GetTile(cellPosition);

                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    if (tile != null)
                    {
                        levelArray[x, y] = DetermineTileType(tile);
                        Debug.Log(levelArray[x, y]);
                        cellColors[x, y] = Color.white;
                        isEditableArray[x, y] = true;
                    }
                    else
                    {
                        levelArray[x, y] = 0;
                        cellColors[x, y] = Color.black;
                        isEditableArray[x, y] = false;
                    }
                }
                else
                {
                    levelArray[x, y] = 0;
                    cellColors[x, y] = Color.black;
                    isEditableArray[x, y] = false;
                }
            }
        }

        Debug.Log("Array generated");
    }

    private BoundsInt GetTilemapBounds(Tilemap tilemap)
    {
        int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;
        bool hasTiles = false;

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                hasTiles = true;
                if (pos.x < minX) minX = pos.x;
                if (pos.y < minY) minY = pos.y;
                if (pos.x > maxX) maxX = pos.x;
                if (pos.y > maxY) maxY = pos.y;
            }
        }
        if (!hasTiles)
        {
            Debug.LogWarning("No tiles found in the tilemap.");
            return new BoundsInt(0, 0, 0, 0, 0, 0);
        }
        return new BoundsInt(minX, minY, 0, maxX - minX + 1, maxY - minY + 1, 1);
    }

    private TemplateElement DetermineTileType(TileBase tile)
    {
        string tileName = tile.name.ToLower();
        if (tileName.Contains("ground"))
            return TemplateElement.Empty;
        return TemplateElement.None;
    }

    private void DisplayPlaceableObjects()
    {
        GUILayout.BeginVertical(GUI.skin.box);

        for (int i = 0; i < placeableObjects.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(placeableObjects[i].name);
            GUILayout.Label(placeableObjects[i].type.ToString());
            GUILayout.Label(placeableObjects[i].color.ToString());
            if (GUILayout.Button("Select"))
            {
                selectedObjectIndex = i;
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }

    private void AddNewObject()
    {
        PlaceableObject newObj = new PlaceableObject();
        newObj.name = newObjectName;
        newObj.color = newObjectColor;
        newObj.type = newObjectType;

        placeableObjects.Add(newObj);
    }

    private void OnSceneGUI()
    {
        ProcessEvent(Event.current);
    }

    private void ProcessEvent(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.ScrollWheel:
                HandleZoom(currentEvent.delta.y);
                break;
            case EventType.MouseUp:
                if (currentEvent.button == 0)
                {
                    isLeftMouseDown = false;
                }
                break;
            case EventType.MouseDrag:
                if (isLeftMouseDown)
                {
                    HandlePan(currentEvent.delta);
                }
                break;
        }
    }

    private void HandleZoom(float delta)
    {
        zoomScale += delta * 0.01f;
        zoomScale = Mathf.Clamp(zoomScale, 0.1f, 10f);
        SceneView.RepaintAll();
    }

    private void HandlePan(Vector2 delta)
    {
        scrollPosition -= delta / zoomScale;
        SceneView.RepaintAll();
    }

    private void CreateNewRecordInSO(string name, TemplateElement[,] levelArray)
    {
        RoomTemplateSO roomTemplateSO = AssetDatabase.LoadAssetAtPath<RoomTemplateSO>("Assets/SO/RoomTemplateSO.asset");

        if (roomTemplateSO == null)
        {
            Debug.LogError("RoomTemplateSO not found at path: Assets/SO/RoomTemplateSO.asset");
            return;
        }

        int id = 1;
        if (roomTemplateSO.Templates.Count > 0)
            id = roomTemplateSO.Templates.Max(t => t.id) + 1;

        RoomTemplateSO.Template newTemplate = new RoomTemplateSO.Template();
        newTemplate.name = name;
        newTemplate.id = id;
        newTemplate.TemplateElement = levelArray;

        roomTemplateSO.Templates.Add(newTemplate);

        EditorUtility.SetDirty(roomTemplateSO);
        AssetDatabase.SaveAssets();
    }

}
