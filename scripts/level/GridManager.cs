using Godot;

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
    public const int WIDTH = 20;
    public const int HEIGHT = 20;
    public const float TILE_SIZE = 1f;

    public TileData[,] Grid;

    public override void _Ready()
    {
        GenerateGrid();
        CreateVisualMap();
    }

    // Creates a simple mesh for every tile so you can see the "map" in the editor/runtime.
    // Different tile types get different colors; all tiles are placed in the XZ plane.
    private void CreateVisualMap()
    {
        // parent node used for organization
        Node3D container = new Node3D { Name = "TileMap" };
        AddChild(container);

        for (int x = 0; x < WIDTH; x++)
        for (int z = 0; z < HEIGHT; z++)
        {
            var data = Grid[x, z];
            var tile = new MeshInstance3D();
            tile.Mesh = new BoxMesh
            {
                Size = new Vector3(TILE_SIZE, 0.1f, TILE_SIZE)
            };

            // color depending on type
            tile.MaterialOverride = GetMaterialForType(data.Type);

            tile.Position = new Vector3(x * TILE_SIZE, 0.05f, z * TILE_SIZE);
            container.AddChild(tile);
        }
    }

    private static StandardMaterial3D GetMaterialForType(TileType type)
    {
        var mat = new StandardMaterial3D();
        switch (type)
        {
            case TileType.Empty:      mat.AlbedoColor = Colors.DarkGray; break;
            case TileType.Buildable:  mat.AlbedoColor = Colors.LightGray; break;
            case TileType.Path:       mat.AlbedoColor = Colors.Green; break;
            case TileType.Spawn:      mat.AlbedoColor = Colors.Blue; break;
            case TileType.Goal:       mat.AlbedoColor = Colors.Red; break;
        }
        return mat;
    }

    private void GenerateGrid()
    {
        Grid = new TileData[WIDTH, HEIGHT];

        for (int x = 0; x < WIDTH; x++)
        {
            for (int z = 0; z < HEIGHT; z++)
            {
                Grid[x, z] = new TileData
                {
                    Height = 0,
                    Type = TileType.Buildable,
                    HasTower = false
                };
            }
        }

        // Camino horizontal
        for (int x = 0; x < WIDTH; x++)
            Grid[x, 10].Type = TileType.Path;

        Grid[0, 10].Type = TileType.Spawn;
        Grid[WIDTH - 1, 10].Type = TileType.Goal;
    }
}