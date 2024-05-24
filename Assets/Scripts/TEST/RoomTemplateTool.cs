using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using Gameplay;

[CreateAssetMenu(fileName = "RoomConfig", menuName = "Configs/RoomConfig", order = 1)]
public class RoomConfigSO : ScriptableObject
{
    public RoomType roomType;
    public List<PlayerSpawnPointWithType> PlayerSpawnPoints;
    public List<EnemySpawnPointWithType> EnemySpawnPoints;
    public List<BuffSpawnPointWithType> BuffSpawnPoints;
}

public class CellData
{
    public string spawnData;
}

public class GridData : ScriptableObject
{
    public int rows = 10;
    public int columns = 10;
    public CellData[,] grid;

    public void InitializeGrid(int rows, int columns)
    {
        this.rows = rows;
        this.columns = columns;
        grid = new CellData[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                grid[i, j] = new CellData();
            }
        }
    }
}

public class GridEditorWindow : EditorWindow
{
    private GridData gridData;
    private Tilemap tilemap;

    [MenuItem("Tools/Grid Editor")]
    public static void ShowWindow()
    {
        GetWindow<GridEditorWindow>("Grid Editor");
    }

    private void OnEnable()
    {
        // Завантаження або створення нового GridData//
        gridData = AssetDatabase.LoadAssetAtPath<GridData>("Assets/GridData.asset");
        if (gridData == null)
        {
            gridData = ScriptableObject.CreateInstance<GridData>();
            AssetDatabase.CreateAsset(gridData, "Assets/GridData.asset");
            gridData.InitializeGrid(10, 10);
        }
    }

    private void OnGUI()
    {
        if (gridData == null) return;

        // Додати поле для Tilemap
        tilemap = (Tilemap)EditorGUILayout.ObjectField("Tilemap", tilemap, typeof(Tilemap), true);

        if (tilemap != null)
        {
            if (GUILayout.Button("Load Tilemap"))
            {
                LoadTilemapData();
            }
        }

        if (gridData.grid != null)
        {
            for (int i = 0; i < gridData.rows; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < gridData.columns; j++)
                {
                    gridData.grid[i, j].spawnData = EditorGUILayout.TextField(gridData.grid[i, j].spawnData);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        if (GUILayout.Button("Save Grid Data"))
        {
            EditorUtility.SetDirty(gridData);
            AssetDatabase.SaveAssets();
        }
    }

    private void LoadTilemapData()
    {
        BoundsInt bounds = tilemap.cellBounds;
        gridData.InitializeGrid(bounds.size.x, bounds.size.y);

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int localPlace = (new Vector3Int(x, y, (int)tilemap.transform.position.y));
                if (tilemap.HasTile(localPlace))
                {
                    gridData.grid[x - bounds.xMin, y - bounds.yMin].spawnData = "Filled";
                }
                else
                {
                    gridData.grid[x - bounds.xMin, y - bounds.yMin].spawnData = "Empty";
                }
            }
        }
    }
}
