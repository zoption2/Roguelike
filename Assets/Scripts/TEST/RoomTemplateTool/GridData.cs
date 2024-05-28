using UnityEngine;

[CreateAssetMenu(fileName = "GridData", menuName = "Configs/GridData", order = 1)]
public class GridData : ScriptableObject
{
    public int rows;
    public int columns;
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

[System.Serializable]
public class CellData
{
    public string spawnData;
}
