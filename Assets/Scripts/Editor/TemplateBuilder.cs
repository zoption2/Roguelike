using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class TemplateBuilder : EditorWindow
{
    #region Fields
    private Tilemap selectedTilemap;
    private TemplateElementType[,] _levelArray;
    private TemplateElementType[,] _originalLevelArray;

    private Color[,] _cellColors;
    private bool[,] _isEditableArray;
    private Vector2 _scrollPosition;

    private List<TemplatePlacebleElements.TemplatePlacebleElement> _placeableObjects = new List<TemplatePlacebleElements.TemplatePlacebleElement>();
    private int _selectedObjectIndex = -1;

    private string _newObjectName = "";
    private Color _newObjectColor = Color.white;
    private TemplateElementType _newObjectType = TemplateElementType.Ground;

    private float _zoomScale = 1f;

    private string _newRecordName = "";
    private bool _DisplayNewTemplateRecordFields = false;
    private bool _displayNewObjectFields = false;

    private TemplatePlacebleElements _templatePlacebleElementsSO;
    private TypeOfScenario _selectedScenarioType = TypeOfScenario.DefaultRoom;

    private bool _showInitialOptions = true;
    private bool _createNewArray = false;
    private bool _createFromTilemap = false;
    private bool _editExistingTemplate = false;
    private bool _resizeArray = false;

    private bool _isMousePressed = false;

    private int _arrayWidth = 0;
    private int _arrayHeight = 0;

    private GUIStyle _headerStyle;
    private GUIStyle _buttonStyle;

    private RoomTemplateSO _roomTemplateSO;
    private List<RoomTemplateSO.Template> _templates = new List<RoomTemplateSO.Template>();
    private int _selectedTemplateIndex = -1;

    private int _deleteIndex = -1;

    private const string C_TemplatePlaceableElementsPath = "Assets/SO/TemplatePlacebleElementsSO.asset";
    private const string C_RoomTemplateSOPath = "Assets/SO/RoomTemplateSO.asset";
    #endregion

    [MenuItem("Tools/Template Builder")]
    public static void ShowWindow()
    {
        var window = GetWindow<TemplateBuilder>("Template Builder");
        window.minSize = new Vector2(300, 400);
    }

    #region GUI 
    private void OnEnable()
    {
        _templatePlacebleElementsSO = AssetDatabase.LoadAssetAtPath<TemplatePlacebleElements>(C_TemplatePlaceableElementsPath);
        if (_templatePlacebleElementsSO == null)
        {
            _templatePlacebleElementsSO = CreateInstance<TemplatePlacebleElements>();
            AssetDatabase.CreateAsset(_templatePlacebleElementsSO, C_TemplatePlaceableElementsPath);
            AssetDatabase.SaveAssets();
            Debug.Log("Created new TemplatePlacebleElements SO at " + C_TemplatePlaceableElementsPath);
        }
        else
        {
            _placeableObjects = new List<TemplatePlacebleElements.TemplatePlacebleElement>(_templatePlacebleElementsSO.PlacebleElements);
            Debug.Log($"Loaded {_placeableObjects.Count} placeable objects from SO.");
        }

        _roomTemplateSO = AssetDatabase.LoadAssetAtPath<RoomTemplateSO>(C_RoomTemplateSOPath);
        if (_roomTemplateSO == null)
        {
            _roomTemplateSO = CreateInstance<RoomTemplateSO>();
            AssetDatabase.CreateAsset(_roomTemplateSO, C_RoomTemplateSOPath);
            AssetDatabase.SaveAssets();
            Debug.Log("Created new RoomTemplateSO at " + C_RoomTemplateSOPath);
        }
        else
        {
            _templates = _roomTemplateSO.Templates;
            Debug.Log($"Loaded {_templates.Count} templates from SO.");
        }
    }

    private void OnGUI()
    {
        if (_headerStyle == null)
        {
            _headerStyle = new GUIStyle(GUI.skin.label);
            _headerStyle.fontSize = 20;
            _headerStyle.alignment = TextAnchor.MiddleCenter;
        }

        if (_buttonStyle == null)
        {
            _buttonStyle = new GUIStyle(GUI.skin.button);
            _buttonStyle.fontSize = 14;
        }

        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            _isMousePressed = true;
        }
        else if (e.type == EventType.MouseUp && e.button == 0)
        {
            _isMousePressed = false;
        }

        if (_showInitialOptions)
        {
            ShowInitialOptions();
        }
        else if (_createNewArray)
        {
            ShowNewArrayOptions();
        }
        else if (_createFromTilemap)
        {
            ShowTilemapOptions();
        }
        else if (_editExistingTemplate)
        {
            ShowEditTemplateOptions();
        }
        else if (_resizeArray)
        {
            ShowResizeArrayOptions();
        }
        else
        {
            ShowMainGUI();
        }
    }

    private void ShowInitialOptions()
    {
        GUILayout.Space(20);
        GUILayout.Label("TEMPLATE BUILDER", _headerStyle, GUILayout.Height(40));

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Empty Template", _buttonStyle, GUILayout.Height(50)))
        {
            _createNewArray = true;
            _showInitialOptions = false;
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Template From Tilemap", _buttonStyle, GUILayout.Height(50)))
        {
            _createFromTilemap = true;
            _showInitialOptions = false;
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Edit Existing Template", _buttonStyle, GUILayout.Height(50)))
        {
            _editExistingTemplate = true;
            _showInitialOptions = false;
        }

        GUILayout.FlexibleSpace();
    }

    private void ShowNewArrayOptions()
    {
        GUILayout.Label("Enter Array Size", EditorStyles.boldLabel);

        _arrayWidth = EditorGUILayout.IntField("Width", _arrayWidth);
        _arrayHeight = EditorGUILayout.IntField("Height", _arrayHeight);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Array"))
        {
            CreateEmptyArray();
            _createNewArray = false;
        }
        if (GUILayout.Button("Back"))
        {
            _createNewArray = false;
            _showInitialOptions = true;
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
            _createFromTilemap = false;
        }
        if (GUILayout.Button("Back"))
        {
            _createFromTilemap = false;
            _showInitialOptions = true;
        }
        EditorGUILayout.EndHorizontal();
    }

    private void ShowEditTemplateOptions()
    {
        GUILayout.Space(10);

        GUILayout.Label("Select Template to Edit", EditorStyles.largeLabel);

        GUILayout.Space(20);

        if (_templates.Count == 0)
        {
            GUILayout.Label("No available templates to edit", EditorStyles.label);
        }
        else
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            for (int i = 0; i < _templates.Count; i++)
            {
                var template = _templates[i];
                GUIStyle style = new GUIStyle(GUI.skin.box);
                if (_selectedTemplateIndex == i)
                {
                    style.normal.background = MakeTex(1, 1, new Color(115 / 255f, 115 / 255f, 115 / 255f));
                    style.normal.textColor = Color.white;
                }

                EditorGUILayout.BeginVertical(style);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(template.name, GUILayout.Width(100));
                GUILayout.FlexibleSpace();
                GUILayout.Label(template.DateAdded, GUILayout.Width(200));
                GUILayout.FlexibleSpace();
                if (_deleteIndex == i)
                {
                    if (GUILayout.Button("Delete", GUILayout.Width(80), GUILayout.Height(24)))
                    {
                        DeleteTemplate(i);
                    }
                    if (GUILayout.Button("Cancel", GUILayout.Width(80), GUILayout.Height(24)))
                    {
                        _deleteIndex = -1;
                    }
                }
                else
                {
                    if (GUILayout.Button("Delete", GUILayout.Width(80), GUILayout.Height(24)))
                    {
                        _deleteIndex = i;
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();

                Rect rect = GUILayoutUtility.GetLastRect();
                if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                {
                    _selectedTemplateIndex = i;
                    Repaint();
                }
            }
            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Edit Template"))
        {
            if (_selectedTemplateIndex >= 0 && _selectedTemplateIndex < _templates.Count)
            {
                LoadTemplate(_templates[_selectedTemplateIndex]);
            }
        }
        if (GUILayout.Button("Back"))
        {
            _editExistingTemplate = false;
            _showInitialOptions = true;
        }
        EditorGUILayout.EndHorizontal();
    }

    private void ShowResizeArrayOptions()
    {
        GUILayout.Label("Enter New Array Size", EditorStyles.boldLabel);

        _arrayWidth = EditorGUILayout.IntField("New Width", _arrayWidth);
        _arrayHeight = EditorGUILayout.IntField("New Height", _arrayHeight);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Resize Array"))
        {
            ResizeArray(_arrayWidth, _arrayHeight);
            _resizeArray = false;
        }
        if (GUILayout.Button("Back"))
        {
            _resizeArray = false;
            _showInitialOptions = false;
        }
        EditorGUILayout.EndHorizontal();
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    private void ShowMainGUI()
    {
        GUILayout.Space(10);

        if (_levelArray != null)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            DisplayArrayEditor();

            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        GUILayout.Label("Selected element", EditorStyles.boldLabel);
        if (_selectedObjectIndex != -1)
        {
            EditorGUILayout.LabelField("Type: " + _placeableObjects[_selectedObjectIndex].Type);
            EditorGUILayout.LabelField("Color: " + _placeableObjects[_selectedObjectIndex].Color);
        }

        GUILayout.Space(10);

        GUILayout.Label("Elements to build", EditorStyles.boldLabel);
        DisplayPlaceableObjects();

        if (!_displayNewObjectFields)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add New Object"))
            {
                _displayNewObjectFields = true;
            }
            GUILayout.EndHorizontal();
        }
        else
        {
            _newObjectName = EditorGUILayout.TextField("Name", _newObjectName);
            _newObjectType = (TemplateElementType)EditorGUILayout.Popup("Type", (int)_newObjectType, System.Enum.GetNames(typeof(TemplateElementType)));
            _newObjectColor = EditorGUILayout.ColorField("Color", _newObjectColor);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Object"))
            {
                AddNewObject();
                _displayNewObjectFields = false;
            }
            if (GUILayout.Button("Cancel"))
            {
                _displayNewObjectFields = false;
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        GUILayout.Label("Scenario Type", EditorStyles.boldLabel);
        _selectedScenarioType = (TypeOfScenario)EditorGUILayout.EnumPopup("Scenario Type", _selectedScenarioType);

        GUILayout.Space(10);

        if (!_DisplayNewTemplateRecordFields)
        {
            if (GUILayout.Button("Add New Template Record"))
            {
                _DisplayNewTemplateRecordFields = true;
                if (_selectedTemplateIndex >= 0 && _selectedTemplateIndex < _templates.Count)
                {
                    _newRecordName = _templates[_selectedTemplateIndex].name;
                }
            }
        }
        else
        {
            GUILayout.Label("New Template Record", EditorStyles.boldLabel);
            _newRecordName = EditorGUILayout.TextField("Name", _newRecordName);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Template Record"))
            {
                CreateOrUpdateRecordInSO(_newRecordName, _levelArray);
                _DisplayNewTemplateRecordFields = false;
                _newRecordName = "";
            }
            if (GUILayout.Button("Cancel"))
            {
                _DisplayNewTemplateRecordFields = false;
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Resize Array", GUILayout.Height(50)))
        {
            _resizeArray = true;
        }
    }

    #endregion

    #region MainWindowLogic
    private void CreateEmptyArray()
    {
        if (_arrayWidth <= 0 || _arrayHeight <= 0)
        {
            Debug.LogError("Width and Height must be greater than 0.");
            return;
        }

        _levelArray = new TemplateElementType[_arrayWidth, _arrayHeight];
        _cellColors = new Color[_arrayWidth, _arrayHeight];
        _isEditableArray = new bool[_arrayWidth, _arrayHeight];

        for (int x = 0; x < _arrayWidth; x++)
        {
            for (int y = 0; y < _arrayHeight; y++)
            {
                _levelArray[x, y] = TemplateElementType.Ground;
                _cellColors[x, y] = Color.white;
                _isEditableArray[x, y] = true;
            }
        }

        Debug.Log($"Created empty array of size {_arrayWidth}x{_arrayHeight}.");
    }

    private void ResizeArray(int newWidth, int newHeight)
    {
        if (newWidth <= 0 || newHeight <= 0)
        {
            Debug.LogError("Width and Height must be greater than 0.");
            return;
        }

        var newLevelArray = new TemplateElementType[newWidth, newHeight];
        var newCellColors = new Color[newWidth, newHeight];
        var newIsEditableArray = new bool[newWidth, newHeight];

        for (int x = 0; x < newWidth; x++)
        {
            for (int y = 0; y < newHeight; y++)
            {
                if (x < _levelArray.GetLength(0) && y < _levelArray.GetLength(1))
                {
                    newLevelArray[x, y] = _levelArray[x, y];
                    newCellColors[x, y] = _cellColors[x, y];
                    newIsEditableArray[x, y] = _isEditableArray[x, y];
                }
                else
                {
                    newLevelArray[x, y] = TemplateElementType.Ground;
                    newCellColors[x, y] = Color.white;
                    newIsEditableArray[x, y] = true;
                }
            }
        }

        _levelArray = newLevelArray;
        _cellColors = newCellColors;
        _isEditableArray = newIsEditableArray;

        Debug.Log($"Resized array to {newWidth}x{newHeight}.");
    }

    private void DisplayArrayEditor()
    {
        GUILayout.BeginVertical(GUI.skin.box);

        int cellSize = Mathf.FloorToInt(20 * _zoomScale);

        for (int y = _levelArray.GetLength(1) - 1; y >= 0; y--)
        {
            GUILayout.BeginHorizontal();

            for (int x = 0; x < _levelArray.GetLength(0); x++)
            {
                Rect rect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));

                Color buttonColor = _cellColors[x, y];
                GUI.backgroundColor = buttonColor;
                if (GUI.Button(rect, GUIContent.none) || (_isMousePressed && rect.Contains(Event.current.mousePosition)))
                {
                    if (_selectedObjectIndex != -1)
                    {
                        TemplateElementType previousType = _levelArray[x, y];
                        _levelArray[x, y] = _placeableObjects[_selectedObjectIndex].Type;
                        TemplateElementType newType = _levelArray[x, y];
                        Debug.Log($"Cell changed from {previousType} to {newType}");
                        _cellColors[x, y] = _placeableObjects[_selectedObjectIndex].Color;
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

        _levelArray = new TemplateElementType[width, height];
        _cellColors = new Color[width, height];
        _isEditableArray = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int cellPosition = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                TileBase tile = selectedTilemap.GetTile(cellPosition);

                if (tile != null)
                {
                    _levelArray[x, y] = DetermineTileType(tile);
                    _cellColors[x, y] = tile is Tile concreteTile ? concreteTile.color : Color.white;
                    _isEditableArray[x, y] = true;
                }
                else
                {
                    _levelArray[x, y] = TemplateElementType.None;
                    _cellColors[x, y] = Color.black;
                    _isEditableArray[x, y] = false;
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

    private TemplateElementType DetermineTileType(TileBase tile)
    {
        string tileName = tile.name.ToLower();
        if (tileName.Contains("ground"))
            return TemplateElementType.Ground;
        return TemplateElementType.None;
    }

    private void DisplayPlaceableObjects()
    {
        GUILayout.BeginVertical(GUI.skin.box);

        float nameWidth = Screen.width * 0.25f;
        float typeWidth = Screen.width * 0.25f;
        float colorWidth = Screen.width * 0.35f;
        float buttonWidth = Screen.width * 0.15f;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Name", GUILayout.Width(nameWidth));
        GUILayout.Label("Type", GUILayout.Width(typeWidth));
        GUILayout.Label("Color", GUILayout.Width(colorWidth));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        for (int i = 0; i < _placeableObjects.Count; i++)
        {
            var obj = _placeableObjects[i];
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(obj.Name, GUILayout.Width(nameWidth));
            GUILayout.Label(obj.Type.ToString(), GUILayout.Width(typeWidth));

            Rect colorRect = EditorGUILayout.GetControlRect(GUILayout.Width(colorWidth));
            EditorGUI.DrawRect(new Rect(colorRect.x, colorRect.y, colorWidth - buttonWidth, colorRect.height), obj.Color);

            GUILayout.FlexibleSpace();

            GUILayout.Space(-buttonWidth);

            if (GUILayout.Button("Select", GUILayout.Width(buttonWidth)))
            {
                _selectedObjectIndex = i;
            }

            EditorGUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }

    private void AddNewObject()
    {
        TemplatePlacebleElements.TemplatePlacebleElement newObj = new TemplatePlacebleElements.TemplatePlacebleElement();
        newObj.Name = _newObjectName;
        newObj.Color = _newObjectColor;
        newObj.Type = _newObjectType;

        _placeableObjects.Add(newObj);

        if (_templatePlacebleElementsSO != null)
        {
            if (_templatePlacebleElementsSO.PlacebleElements == null)
            {
                _templatePlacebleElementsSO.PlacebleElements = new List<TemplatePlacebleElements.TemplatePlacebleElement>();
            }

            _templatePlacebleElementsSO.PlacebleElements.Add(newObj);
            EditorUtility.SetDirty(_templatePlacebleElementsSO);
            AssetDatabase.SaveAssets();
            Debug.Log($"Added new object: {newObj.Name}, Type: {newObj.Type}, Color: {newObj.Color}");
        }
        else
        {
            Debug.LogError("TemplatePlacebleElements SO is null. Failed to add new object.");
        }
    }

    private void CreateOrUpdateRecordInSO(string name, TemplateElementType[,] levelArray)
    {
        if (_roomTemplateSO == null)
        {
            Debug.LogError("RoomTemplateSO not found at path: Assets/SO/RoomTemplateSO.asset");
            return;
        }

        if (_roomTemplateSO.Templates == null)
        {
            _roomTemplateSO.Templates = new List<RoomTemplateSO.Template>();
        }

        var existingTemplate = _selectedTemplateIndex >= 0 && _selectedTemplateIndex < _templates.Count
                                ? _templates[_selectedTemplateIndex]
                                : null;

        if (existingTemplate != null && name == existingTemplate.name)
        {
            existingTemplate.TemplateElement = (TemplateElementType[,])levelArray.Clone();
            existingTemplate.ScenarioType = _selectedScenarioType;
            existingTemplate.DateAdded = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            AssignExits(existingTemplate, levelArray);
            Debug.Log($"Updated existing template: {name}");
        }
        else
        {
            var sameNameTemplate = _roomTemplateSO.Templates.FirstOrDefault(t => t.name == name);
            if (sameNameTemplate != null)
            {
                Debug.LogError($"Template with name {name} already exists. Choose a different name.");
                return;
            }

            int id = _roomTemplateSO.Templates.Count > 0 ? _roomTemplateSO.Templates.Max(t => t.id) + 1 : 1;

            RoomTemplateSO.Template newTemplate = new RoomTemplateSO.Template
            {
                name = name,
                id = id,
                TemplateElement = (TemplateElementType[,])levelArray.Clone(),
                ScenarioType = _selectedScenarioType,
                DateAdded = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            AssignExits(newTemplate, levelArray);
            _roomTemplateSO.Templates.Add(newTemplate);
            Debug.Log($"Added new template: {name}");

            if (existingTemplate != null)
            {
                existingTemplate.TemplateElement = _originalLevelArray;
                Debug.Log($"Restored original template: {existingTemplate.name}");
            }
        }

        EditorUtility.SetDirty(_roomTemplateSO);
        AssetDatabase.SaveAssets();
    }


    private void AssignExits(RoomTemplateSO.Template template, TemplateElementType[,] levelArray)
    {
        template.leftExit = FindExit(levelArray, ExitDirection.Left);
        template.rightExit = FindExit(levelArray, ExitDirection.Right);
        template.topExit = FindExit(levelArray, ExitDirection.Top);
        template.bottomExit = FindExit(levelArray, ExitDirection.Bottom);
    }

    private TemplateElementType FindExit(TemplateElementType[,] levelArray, ExitDirection direction)
    {
        int rows = levelArray.GetLength(0);
        int cols = levelArray.GetLength(1);

        switch (direction)
        {
            case ExitDirection.Left:
                for (int i = 0; i < rows; i++)
                {
                    if (levelArray[i, 0] == TemplateElementType.Exit)
                    {
                        return levelArray[i, 0];
                    }
                }
                break;
            case ExitDirection.Right:
                for (int i = 0; i < rows; i++)
                {
                    if (levelArray[i, cols - 1] == TemplateElementType.Exit)
                    {
                        return levelArray[i, cols - 1];
                    }
                }
                break;
            case ExitDirection.Top:
                for (int j = 0; j < cols; j++)
                {
                    if (levelArray[0, j] == TemplateElementType.Exit)
                    {
                        return levelArray[0, j];
                    }
                }
                break;
            case ExitDirection.Bottom:
                for (int j = 0; j < cols; j++)
                {
                    if (levelArray[rows - 1, j] == TemplateElementType.Exit)
                    {
                        return levelArray[rows - 1, j];
                    }
                }
                break;
        }
        return TemplateElementType.None;
    }

    private void LoadTemplate(RoomTemplateSO.Template template)
    {
        if (template == null || template.TemplateElement == null)
        {
            Debug.LogError("Template or TemplateElement is null.");
            return;
        }

        _levelArray = template.TemplateElement;
        _selectedScenarioType = template.ScenarioType;
        int rows = _levelArray.GetLength(0);
        int cols = _levelArray.GetLength(1);
        _originalLevelArray = new TemplateElementType[rows, cols];

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                _originalLevelArray[x, y] = _levelArray[x, y];
            }
        }

        _cellColors = new Color[rows, cols];
        _isEditableArray = new bool[rows, cols];

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                var elementType = _levelArray[x, y];
                if (elementType == TemplateElementType.None)
                {
                    _cellColors[x, y] = Color.black;
                }
                else
                {
                    var placeableObject = _placeableObjects.FirstOrDefault(obj => obj.Type == elementType);
                    if (placeableObject != null)
                    {
                        _cellColors[x, y] = placeableObject.Color;
                    }
                    else
                    {
                        _cellColors[x, y] = Color.white;
                    }
                }
                _isEditableArray[x, y] = true;
            }
        }

        _editExistingTemplate = false;
        _showInitialOptions = false;
    }

    private void DeleteTemplate(int index)
    {
        if (index >= 0 && index < _templates.Count)
        {
            _templates.RemoveAt(index);
            EditorUtility.SetDirty(_roomTemplateSO);
            AssetDatabase.SaveAssets();
            _deleteIndex = -1;
            _selectedTemplateIndex = -1;
        }
    }

    #endregion
}