using Godot;
using System.Collections.Generic;

public partial class Pathfinding : Node
{
    [Export] public Vector2I mapSize;         // Tamaño del mapa en tiles
    public TileMapLayer tileMap;              // Capa de suelo
    private AStarGrid2D astar = new AStarGrid2D();

    // IDs de tiles según tu mapa
    private readonly HashSet<int> pathTiles = new HashSet<int> {120,121,128,129,130,131,148,149,150,151};
    private readonly HashSet<int> spawnTiles = new HashSet<int> {122};
    private readonly HashSet<int> baseTiles = new HashSet<int> {136};
    private readonly HashSet<int> blockedTiles = new HashSet<int> {340,332,333,334,335,336,337,338,339,572,573,574,575,576,577,578,579,392,96,54};

    private Vector2I gridOffset;
    public bool IsReady { get; private set; } = false;

    public override void _Ready()
    {
        // Si no asignaste el TileMapLayer en el inspector, lo busca
        if (tileMap == null)
            tileMap = GetTree().Root.FindChild("Suelo", true, false) as TileMapLayer;

        if (tileMap == null)
        {
            GD.PrintErr("Pathfinding: No se encontró la capa de suelo.");
            return;
        }

        Rect2I usedRect = tileMap.GetUsedRect();
        gridOffset = usedRect.Position;
        mapSize = usedRect.Size;

        GD.Print("Mapa usado desde ", usedRect.Position, " hasta ", usedRect.Position + usedRect.Size);

        // Configurar AStarGrid
        astar.Region = new Rect2I(gridOffset, mapSize);
        astar.CellSize = new Vector2(1,1);
        astar.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;

        BuildGrid();
        astar.Update(); // Inicializa la grilla

        IsReady = true;

        // Arrancar primera oleada si existe spawner
        EnemySpawner spawner = GetTree().Root.FindChild("EnemySpawner", true, false) as EnemySpawner;
        spawner?.StartWave();
    }

    private void BuildGrid()
    {
        for (int x = 0; x < mapSize.X; x++)
        {
            for (int y = 0; y < mapSize.Y; y++)
            {
                Vector2I pos = new Vector2I(x, y) + gridOffset;
                int tileId = tileMap.GetCellSourceId(pos);

                bool walkable = pathTiles.Contains(tileId) || spawnTiles.Contains(tileId) || baseTiles.Contains(tileId);
                bool blocked = blockedTiles.Contains(tileMap.GetCellSourceId(pos));

                astar.SetPointSolid(pos, !walkable || blocked);
            }
        }
    }

    public Vector2I[] GetPath(Vector2I start, Vector2I end)
    {
        Godot.Collections.Array<Vector2I> godotArray = astar.GetIdPath(start, end);
        Vector2I[] result = new Vector2I[godotArray.Count];
        for (int i = 0; i < godotArray.Count; i++)
            result[i] = godotArray[i];
        return result;
    }

    // Convierte tile a posición global en píxeles sobre la capa de suelo
    internal Vector2 TileToGlobalPos(Vector2I tilePos)
    {
        if (tileMap == null) return Vector2.Zero;

        // Tamaño de un tile en píxeles según Tiled
        float tileWidth = 232f;   // ancho de un tile
        float tileHeight = 110f;  // alto de un tile

        // Posición isométrica
        float x = (tilePos.X - tilePos.Y) * tileWidth / 2f;
        float y = (tilePos.X + tilePos.Y) * tileHeight / 2f;

        return tileMap.GlobalPosition + new Vector2(x, y);
    }
}