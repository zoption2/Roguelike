using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class TilemapTo2DArrayWindow : EditorWindow
{
    private Tilemap selectedTilemap;
    private TemplateElement[,] levelArray;
    private Color[,] cellColors;
    private bool[,] isEditableArray;
    private Vector2 scrollPosition;

    private List<TemplatePlacebleElements.TemplatePlacebleElement> placeableObjects = new List<TemplatePlacebleElements.TemplatePlacebleElement>();
    private int selectedObjectIndex = -1;

    private string newObjectName = "";
    private Color newObjectColor = Color.white;
    private TemplateElement newObjectType = TemplateElement.Empty;

    private float zoomScale = 1f;

    private string newRecordName = "";
    private bool DisplayNewTemplateRecordFields = false;
    private bool displayNewObjectFields = false;

    private TemplatePlacebleElements templatePlacebleElementsSO;

    private bool showInitialOptions = true;
    private bool createNewArray = false;
    private bool createFromTilemap = false;
    private bool editExistingTemplate = false;

    private int arrayWidth = 0;
    private int arrayHeight = 0;

    private GUIStyle headerStyle;
    private GUIStyle buttonStyle;

    private RoomTemplateSO roomTemplateSO;
    private List<RoomTemplateSO.Template> templates = new List<RoomTemplateSO.Template>();
    private int selectedTemplateIndex = -1;

    [MenuItem("Tools/Template Builder")]
    public static void ShowWindow()
    {
        var window = GetWindow<TilemapTo2DArrayWindow>("Template Builder");
        window.minSize = new Vector2(300, 400);
    }

    private void OnEnable()
    {
        templatePlacebleElementsSO = AssetDatabase.LoadAssetAtPath<TemplatePlacebleElements>("Assets/SO/TemplatePlacebleElementsSO.asset");
        roomTemplateSO = AssetDatabase.LoadAssetAtPath<RoomTemplateSO>("Assets/SO/RoomTemplateSO.asset");

        if (templatePlacebleElementsSO != null)
        {
            placeableObjects = new List<TemplatePlacebleElements.TemplatePlacebleElement>(templatePlacebleElementsSO.PlacebleElements);
            Debug.Log($"Loaded {placeableObjects.Count} placeable objects from SO.");
            foreach (var obj in placeableObjects)
            {
                Debug.Log($"Object: {obj.Name}, Type: {obj.Type}, Color: {obj.Color}");
            }
        }
        else
        {
            Debug.LogError("Failed to load TemplatePlacebleElements SO.");
        }

        if (roomTemplateSO != null)
        {
            templates = roomTemplateSO.Templates;
            Debug.Log($"Loaded {templates.Count} templates from SO.");
        }
        else
        {
            Debug.LogError("Failed to load RoomTemplateSO.");
        }
    }

    

    private void OnGUI()
    {
        if (headerStyle == null)
        {
            headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.fontSize = 20;
            headerStyle.alignment = TextAnchor.MiddleCenter;
        }

        if (buttonStyle == null)
        {
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 14;
        }

        if (showInitialOptions)
        {
            ShowInitialOptions();
        }
        else if (createNewArray)
        {
            ShowNewArrayOptions();
        }
        else if (createFromTilemap)
        {
            ShowTilemapOptions();
        }
        else if (editExistingTemplate)
        {
            ShowEditTemplateOptions();
        }
        else
        {
            ShowMainGUI();
        }
    }

    private void ShowInitialOptions()
    {
        GUILayout.Space(20);
        GUILayout.Label("TEMPLATE BUILDER", headerStyle, GUILayout.Height(40));

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Empty Template", buttonStyle, GUILayout.Height(50)))
        {
            createNewArray = true;
            showInitialOptions = false;
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Template From Tilemap", buttonStyle, GUILayout.Height(50)))
        {
            createFromTilemap = true;
            showInitialOptions = false;
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Edit Existing Template", buttonStyle, GUILayout.Height(50)))
        {
            editExistingTemplate = true;
            showInitialOptions = false;
        }

        GUILayout.FlexibleSpace();
    }

    private void ShowNewArrayOptions()
    {
        GUILayout.Label("Enter Array Size", EditorStyles.boldLabel);

        arrayWidth = EditorGUILayout.IntField("Width", arrayWidth);
        arrayHeight = EditorGUILayout.IntField("Height", arrayHeight);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Array"))
        {
            CreateEmptyArray();
            createNewArray = false;
        }
        if (GUILayout.Button("Back"))
        {
            createNewArray = false;
            showInitialOptions = true;
        }
        EditorGUILayout.EndHorizontal();
    }

    private void ShowTilemapOptions()
    {
        GUILayout.Label("Select Tilemap", EditorStyles.boldLabel);

        selectedTilemap = EditorGUILayout.ObjectField("Tilemap", selectedTilemap, typeof(Tilemap), true) as Tilemap;

        EditorGUILayout.BeginHorizontal();
        if (selectedTilemap != null && GUILayout.Button("Generate Template"))
        {
            GenerateArrayFromTilemap();
            createFromTilemap = false;
        }
        if (GUILayout.Button("Back"))
        {
            createFromTilemap = false;
            showInitialOptions = true;
        }
        EditorGUILayout.EndHorizontal();
    }

    private void ShowEditTemplateOptions()
    {
        GUILayout.Label("Select Template to Edit", EditorStyles.boldLabel);

        if (templates.Count == 0)
        {
            GUILayout.Label("No available templates to edit", EditorStyles.label);
        }
        else
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            for (int i = 0; i < templates.Count; i++)
            {
                var template = templates[i];
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"{template.name} (Added: {template.DateAdded})", EditorStyles.label);
                if (GUILayout.Button("Select"))
                {
                    selectedTemplateIndex = i;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Edit Template"))
        {
            if (selectedTemplateIndex >= 0 && selectedTemplateIndex < templates.Count)
            {
                LoadTemplate(templates[selectedTemplateIndex]);
            }
        }
        if (GUILayout.Button("Back"))
        {
            editExistingTemplate = false;
            showInitialOptions = true;
        }
        EditorGUILayout.EndHorizontal();
    }

    private void ShowMainGUI()
    {
        GUILayout.Space(10);

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

        GUILayout.Space(10);

        GUILayout.Label("Selected element", EditorStyles.boldLabel);
        if (selectedObjectIndex != -1)
        {
            EditorGUILayout.LabelField("Type: " + placeableObjects[selectedObjectIndex].Type);
            EditorGUILayout.LabelField("Color: " + placeableObjects[selectedObjectIndex].Color);
        }

        GUILayout.Space(10);

        GUILayout.Label("Elements to build", EditorStyles.boldLabel);
        DisplayPlaceableObjects();

        if (!displayNewObjectFields)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add New Object"))
            {
                displayNewObjectFields = true;
            }
            GUILayout.EndHorizontal();
        }
        else
        {
            newObjectName = EditorGUILayout.TextField("Name", newObjectName);
            newObjectType = (TemplateElement)EditorGUILayout.Popup("Type", (int)newObjectType, System.Enum.GetNames(typeof(TemplateElement)));
            newObjectColor = EditorGUILayout.ColorField("Color", newObjectColor);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Object"))
            {
                AddNewObject();
                displayNewObjectFields = false;
            }
            if (GUILayout.Button("Cancel"))
            {
                displayNewObjectFields = false;
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        if (!DisplayNewTemplateRecordFields)
        {
            if (GUILayout.Button("Add New Template Record"))
            {
                DisplayNewTemplateRecordFields = true;
                if (selectedTemplateIndex >= 0 && selectedTemplateIndex < templates.Count)
                {
                    newRecordName = templates[selectedTemplateIndex].name;
                }
            }
        }
        else
        {
            GUILayout.Label("New Template Record", EditorStyles.boldLabel);
            newRecordName = EditorGUILayout.TextField("Name", newRecordName);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Template Record"))
            {
                CreateOrUpdateRecordInSO(newRecordName, levelArray);
                DisplayNewTemplateRecordFields = false;
                newRecordName = "";
            }
            if (GUILayout.Button("Cancel"))
            {
                DisplayNewTemplateRecordFields = false;
            }
            GUILayout.EndHorizontal();
        }
    }


    private void CreateEmptyArray()
    {
        if (arrayWidth <= 0 || arrayHeight <= 0)
        {
            Debug.LogError("Width and Height must be greater than 0.");
            return;
        }

        levelArray = new TemplateElement[arrayWidth, arrayHeight];
        cellColors = new Color[arrayWidth, arrayHeight];
        isEditableArray = new bool[arrayWidth, arrayHeight];

        for (int x = 0; x < arrayWidth; x++)
        {
            for (int y = 0; y < arrayHeight; y++)
            {
                levelArray[x, y] = TemplateElement.Empty;
                cellColors[x, y] = Color.white;
                isEditableArray[x, y] = true;
            }
        }

        Debug.Log($"Created empty array of size {arrayWidth}x{arrayHeight}.");
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
                        levelArray[x, y] = placeableObjects[selectedObjectIndex].Type;
                        TemplateElement newType = levelArray[x, y];
                        Debug.Log($"Cell changed from {previousType} to {newType}");
                        cellColors[x, y] = placeableObjects[selectedObjectIndex].Color;
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

        if (placeableObjects.Count == 0)
        {
            GUILayout.Label("No placeable objects found in ScriptableObject.");
        }
        else
        {
            foreach (var obj in placeableObjects)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(obj.Name);
                GUILayout.Label(obj.Type.ToString());
                GUILayout.Label(obj.Color.ToString());
                if (GUILayout.Button("Select"))
                {
                    selectedObjectIndex = placeableObjects.IndexOf(obj);
                }
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndVertical();
    }

    private void AddNewObject()
    {
        TemplatePlacebleElements.TemplatePlacebleElement newObj = new TemplatePlacebleElements.TemplatePlacebleElement();
        newObj.Name = newObjectName;
        newObj.Color = newObjectColor;
        newObj.Type = newObjectType;

        placeableObjects.Add(newObj);

        if (templatePlacebleElementsSO != null)
        {
            templatePlacebleElementsSO.PlacebleElements.Add(newObj);
            EditorUtility.SetDirty(templatePlacebleElementsSO);
            AssetDatabase.SaveAssets();
            Debug.Log($"Added new object: {newObj.Name}, Type: {newObj.Type}, Color: {newObj.Color}");
        }
    }

    private void CreateOrUpdateRecordInSO(string name, TemplateElement[,] levelArray)
    {
        if (roomTemplateSO == null)
        {
            Debug.LogError("RoomTemplateSO not found at path: Assets/SO/RoomTemplateSO.asset");
            return;
        }

        var existingTemplate = roomTemplateSO.Templates.FirstOrDefault(t => t.name == name);
        if (existingTemplate != null)
        {
            existingTemplate.TemplateElement = levelArray;
            existingTemplate.DateAdded = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Debug.Log($"Updated existing template: {name}");
        }
        else
        {
            int id = 1;
            if (roomTemplateSO.Templates.Count > 0)
                id = roomTemplateSO.Templates.Max(t => t.id) + 1;

            RoomTemplateSO.Template newTemplate = new RoomTemplateSO.Template();
            newTemplate.name = name;
            newTemplate.id = id;
            newTemplate.TemplateElement = levelArray;
            newTemplate.DateAdded = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            roomTemplateSO.Templates.Add(newTemplate);
            Debug.Log($"Added new template: {name}");
        }

        EditorUtility.SetDirty(roomTemplateSO);
        AssetDatabase.SaveAssets();
    }


    private void LoadTemplate(RoomTemplateSO.Template template)
    {
        levelArray = template.TemplateElement;
        cellColors = new Color[levelArray.GetLength(0), levelArray.GetLength(1)];
        isEditableArray = new bool[levelArray.GetLength(0), levelArray.GetLength(1)];

        for (int x = 0; x < levelArray.GetLength(0); x++)
        {
            for (int y = 0; y < levelArray.GetLength(1); y++)
            {
                var elementType = levelArray[x, y];
                if (elementType == TemplateElement.None)
                {
                    cellColors[x, y] = Color.black;
                }
                else
                {
                    var placeableObject = placeableObjects.FirstOrDefault(obj => obj.Type == elementType);
                    if (placeableObject != null)
                    {
                        cellColors[x, y] = placeableObject.Color;
                    }
                    else
                    {
                        cellColors[x, y] = Color.white;
                    }
                }
                isEditableArray[x, y] = true;
            }
        }

        editExistingTemplate = false;
        showInitialOptions = false;
    }
}
