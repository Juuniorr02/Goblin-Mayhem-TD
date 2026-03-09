using Godot;
using System.Collections.Generic;

public partial class Pathfinding : Node
{
    [Export] public Vector2I mapSize;
    public TileMapLayer tileMap;
    private AStarGrid2D astar = new AStarGrid2D();

    private readonly HashSet<int> pathTiles = new HashSet<int> {120,121,128,129,130,131,148,149,150,151};
    private readonly HashSet<int> spawnTiles = new HashSet<int> {122};
    private readonly HashSet<int> baseTiles = new HashSet<int> {136};
    private readonly HashSet<int> blockedTiles = new HashSet<int> {340,332,333,334,335,336,337,338,339,572,573,574,575,576,577,578,579,392,96,54};

    private Vector2I gridOffset;
    public bool IsReady { get; private set; } = false;

    public override void _Ready()
    {
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

        astar.Region = new Rect2I(gridOffset, mapSize);
        astar.CellSize = new Vector2(1,1);
        astar.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;

        BuildGrid();
        astar.Update();

        IsReady = true;
        GD.Print("AStarGrid2D está listo: IsReady = ", IsReady);
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
                bool blocked = blockedTiles.Contains(tileId);

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

    public List<Vector2I> GetWalkableNeighbors(Vector2I tile)
    {
        List<Vector2I> neighbors = new List<Vector2I>();
        Vector2I[] dirs = { Vector2I.Up, Vector2I.Down, Vector2I.Left, Vector2I.Right };

        foreach (var dir in dirs)
        {
            Vector2I n = tile + dir;
            if (!astar.IsInBoundsv(n)) continue;
            if (!astar.IsPointSolid(n)) neighbors.Add(n);
        }

        GD.Print($"Tile {tile}, vecinos caminables: {string.Join(", ", neighbors)}");
        return neighbors;
    }

    public Vector2I GetNextTileRandom(Vector2I currentTile, Vector2I nextPathTile, Vector2I previousTile, Vector2I targetTile, Queue<Vector2I> visitedTiles, float randomChance = 0.35f)
    {
        List<Vector2I> neighbors = GetWalkableNeighbors(currentTile);

        neighbors.Remove(previousTile);
        neighbors.RemoveAll(t => visitedTiles.Contains(t));

        int currentDist = currentTile.DistanceSquaredTo(targetTile);
        int tolerance = 4;

        List<Vector2I> validNeighbors = new List<Vector2I>();
        foreach (var n in neighbors)
        {
            if (n.DistanceSquaredTo(targetTile) <= currentDist + tolerance)
                validNeighbors.Add(n);
        }

        GD.Print($"Tile {currentTile}, valid neighbors para desviarse: {string.Join(", ", validNeighbors)}");

        if (validNeighbors.Count < 2)
            return nextPathTile;

        if (GD.Randf() < randomChance)
        {
            int r = (int)(GD.Randi() % validNeighbors.Count);
            GD.Print($"Tile {currentTile} toma desvío hacia {validNeighbors[r]}");
            return validNeighbors[r];
        }

        GD.Print($"Tile {currentTile} sigue el path hacia {nextPathTile}");
        return nextPathTile;
    }

    public Vector2I GetNextTileFromPath(Vector2I currentTile, Vector2I previousTile, Vector2I targetTile, ref Vector2I[] path, ref int pathIndex, Queue<Vector2I> visitedTiles)
    {
        if (pathIndex >= path.Length - 1)
            return currentTile;

        Vector2I nextTile = path[pathIndex + 1];
        Vector2I chosenTile = GetNextTileRandom(currentTile, nextTile, previousTile, targetTile, visitedTiles);

        if (chosenTile != nextTile)
        {
            GD.Print($"Recalculando path desde {chosenTile} hasta {targetTile}");
            path = GetPath(chosenTile, targetTile);
            pathIndex = 0;
        }
        else
        {
            pathIndex++;
        }

        return chosenTile;
    }

    internal Vector2 TileToGlobalPos(Vector2I tilePos)
    {
        if (tileMap == null) return Vector2.Zero;

        float tileWidth = 232f;
        float tileHeight = 110f;

        float x = (tilePos.X - tilePos.Y) * tileWidth / 2f;
        float y = (tilePos.X + tilePos.Y) * tileHeight / 2f;

        return tileMap.GlobalPosition + new Vector2(x, y);
    }
}