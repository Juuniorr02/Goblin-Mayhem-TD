using Godot;
using System;

public enum TileType
{
    Empty,
    Path,
    Buildable,
    Spawn,
    Goal
}

public struct TileData
{
    public int Height;
    public TileType Type;
    public bool HasTower;
}

public partial class GridManager : Node
{
    [Export] private GridMap gridMap;
    public const int WIDTH = 20;
    public const int HEIGHT = 20;

    public TileData[,] Grid;

    public override void _Ready()
    {
        GenerateGridFromGridMap();
    }

    private void GenerateGridFromGridMap()
    {
        Grid = new TileData[WIDTH, HEIGHT];

        for (int x = 0; x < WIDTH; x++)
        for (int z = 0; z < HEIGHT; z++)
        {
            int item = gridMap.GetCellItem(new Vector3I(x, 0, z));

            TileType type = TileType.Buildable;

            switch (item)
            {
                case 1: type = TileType.Path; break;
                case 2: type = TileType.Spawn; break;
                case 3: type = TileType.Goal; break;
            }

            Grid[x, z] = new TileData
            {
                Height = 0,
                Type = type,
                HasTower = false
            };
        }
    }

    public bool IsBuildable(int x, int z)
    {
        if (x < 0 || x >= WIDTH || z < 0 || z >= HEIGHT) return false;
        return Grid[x, z].Type == TileType.Buildable && !Grid[x, z].HasTower;
    }

    public void SetTileType(int x, int z, TileType type)
    {
        if (x < 0 || x >= WIDTH || z < 0 || z >= HEIGHT) return;
        Grid[x, z].Type = type;
    }
}