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
    [Export] private NodePath gridMapPath = new NodePath("");
    private GridMap gridMap;
    public const int WIDTH = 20;
    public const int HEIGHT = 20;    
    public const float TILE_SIZE = 1f;
    public int item;
    [Export] public int pathItemId = 1;
    [Export] public int spawnItemId = 2;
    [Export] public int goalItemId = 3;
    // last known counts after generating (used for fallback logic)
    private int lastCountPath = 0;
    private int lastCountSpawn = 0;
    private int lastCountGoal = 0;

    public TileData[,] Grid;
    // visual helpers
    private Node3D visualContainer;
    private MeshInstance3D[,] visualTiles;
    private StandardMaterial3D[] materialByType;
    private Label3D spawnLabel;
    private Label3D goalLabel;
    private System.Collections.Generic.List<MeshInstance3D> pathMarkers;

    public override void _Ready()
    {
        // Try to resolve the GridMap node. Use exported NodePath if set, otherwise look for a child named "GridMap".
        if (!string.IsNullOrEmpty(gridMapPath.ToString()))
        {
            gridMap = GetNodeOrNull<GridMap>(gridMapPath);
            if (gridMap == null)
                GD.PrintErr($"GridManager: exported gridMapPath '{gridMapPath}' did not resolve to a GridMap node.");
        }

        if (gridMap == null)
        {
            gridMap = GetNodeOrNull<GridMap>("GridMap");
            if (gridMap == null)
                GD.PrintErr("GridManager: could not find a child GridMap node. Please assign the GridMap in the inspector or add a child named 'GridMap'.");
        }

        if (gridMap != null)
            GenerateGridFromGridMap();

        // If generation produced no path/spawn/goal, try common sibling/child GridMap names (Ground)
        if (lastCountPath == 0 && lastCountSpawn == 0 && lastCountGoal == 0)
        {
            var alt = GetNodeOrNull<GridMap>("Ground");
            if (alt != null && alt != gridMap)
            {
                GD.Print($"GridManager: initial GridMap had no path/spawn/goal; switching to child GridMap 'Ground' and regenerating.");
                gridMap = alt;
                GenerateGridFromGridMap();
            }
        }
    }

    private void GenerateGridFromGridMap()
    {
        Grid = new TileData[WIDTH, HEIGHT];
        int countPath = 0; int countSpawn = 0; int countGoal = 0; int countBuild = 0;
        var itemCounts = new System.Collections.Generic.Dictionary<int,int>();
        int sampleLogged = 0;

        for (int x = 0; x < WIDTH; x++)
        for (int z = 0; z < HEIGHT; z++)
        {
            item = gridMap.GetCellItem(new Vector3I(x, 0, z));

            // track raw GridMap item ids for debugging
            if (!itemCounts.ContainsKey(item)) itemCounts[item] = 0;
            itemCounts[item]++;
            if (item != 0 && sampleLogged < 20)
            {
                GD.Print($"GridManager: sample nonzero item id={item} at ({x},{z})");
                sampleLogged++;
            }

            TileType type = TileType.Buildable;

            // GridMap.GetCellItem returns -1 for empty cells. We map specific item ids
            // (as set in the inspector via pathItemId/spawnItemId/goalItemId) to TileType.
            if (item == pathItemId)
                type = TileType.Path;
            else if (item == spawnItemId)
                type = TileType.Spawn;
            else if (item == goalItemId)
                type = TileType.Goal;

            if (type == TileType.Path) countPath++;
            if (type == TileType.Spawn) countSpawn++;
            if (type == TileType.Goal) countGoal++;
            if (type == TileType.Buildable) countBuild++;

            Grid[x, z] = new TileData
            {
                Height = 0,
                Type = type,
                HasTower = false
            };
        }
        GD.Print($"GridManager: Generated grid from GridMap. Path={countPath} Spawn={countSpawn} Goal={countGoal} Buildable={countBuild}");
        GD.Print("GridManager: raw GridMap item id counts:");
        foreach (var kv in itemCounts)
            GD.Print($"  id={kv.Key} count={kv.Value}");
        // store last counts
        lastCountPath = countPath;
        lastCountSpawn = countSpawn;
        lastCountGoal = countGoal;
        // update visuals
        RefreshVisualMap();
    }

    // Public wrapper so UI or other scripts can force a refresh
    public void RefreshFromGridMap()
    {
        if (gridMap != null)
            GenerateGridFromGridMap();
        else
            GD.PrintErr("GridManager: RefreshFromGridMap called but gridMap is null");
    }

    private void EnsureVisuals()
    {
        if (visualContainer == null)
        {
            visualContainer = GetNodeOrNull<Node3D>("Visuals");
            if (visualContainer == null)
            {
                visualContainer = new Node3D();
                visualContainer.Name = "Visuals";
                AddChild(visualContainer);
            }
        }

        if (visualTiles == null)
        {
            visualTiles = new MeshInstance3D[WIDTH, HEIGHT];
        }

        if (materialByType == null)
        {
            materialByType = new StandardMaterial3D[5];
            // Empty
            materialByType[(int)TileType.Empty] = new StandardMaterial3D() { AlbedoColor = new Color(0.6f,0.6f,0.6f) };
            // Path
            materialByType[(int)TileType.Path] = new StandardMaterial3D() { AlbedoColor = new Color(0.55f,0.27f,0.07f) };
            // Buildable
            materialByType[(int)TileType.Buildable] = new StandardMaterial3D() { AlbedoColor = new Color(0.2f,0.8f,0.2f) };
            // Spawn
            materialByType[(int)TileType.Spawn] = new StandardMaterial3D() { AlbedoColor = new Color(0.1f,0.6f,1.0f) };
            // Goal
            materialByType[(int)TileType.Goal] = new StandardMaterial3D() { AlbedoColor = new Color(1.0f,0.1f,0.1f) };
        }
    }

    public void RefreshVisualMap()
    {
        EnsureVisuals();

        // Clear previous visuals
        for (int x = 0; x < WIDTH; x++)
        for (int z = 0; z < HEIGHT; z++)
        {
            if (visualTiles[x,z] != null)
            {
                visualTiles[x,z].QueueFree();
                visualTiles[x,z] = null;
            }
        }

        // Create tiles
        for (int x = 0; x < WIDTH; x++)
        for (int z = 0; z < HEIGHT; z++)
        {
            var t = Grid[x,z];
            var mi = new MeshInstance3D();
            var box = new BoxMesh();
            box.Size = new Vector3(TILE_SIZE, 0.05f, TILE_SIZE);
            mi.Mesh = box;
            mi.MaterialOverride = materialByType[(int)t.Type];
            mi.Position = new Vector3(x * TILE_SIZE, 0.01f, z * TILE_SIZE);
            visualContainer.AddChild(mi);
            visualTiles[x,z] = mi;
        }

        // update markers and path
        UpdateMarkersAndPath();
    }

    private void UpdateMarkersAndPath()
    {
        // remove old markers/line if any
        if (spawnLabel != null) spawnLabel.QueueFree();
        if (goalLabel != null) goalLabel.QueueFree();
        if (pathMarkers != null)
        {
            foreach (var m in pathMarkers) if (m != null) m.QueueFree();
        }

        spawnLabel = null; goalLabel = null; pathMarkers = new System.Collections.Generic.List<MeshInstance3D>();

        // find spawn/goal coordinates (first occurrences)
        Vector3? spawnPos = null;
        Vector3? goalPos = null;
        for (int x = 0; x < WIDTH; x++)
        for (int z = 0; z < HEIGHT; z++)
        {
            var t = Grid[x,z];
            if (t.Type == TileType.Spawn && spawnPos == null)
                spawnPos = new Vector3(x * TILE_SIZE, 0.2f, z * TILE_SIZE);
            if (t.Type == TileType.Goal && goalPos == null)
                goalPos = new Vector3(x * TILE_SIZE, 0.2f, z * TILE_SIZE);
        }

        // create labels
        if (spawnPos != null)
        {
            spawnLabel = new Label3D();
            spawnLabel.Text = "SPAWN";
            spawnLabel.Transform = new Transform3D(Basis.Identity, (Vector3)spawnPos + new Vector3(0,0.5f,0));
            spawnLabel.Modulate = new Color(0.2f,0.6f,1.0f);
            visualContainer.AddChild(spawnLabel);
        }

        if (goalPos != null)
        {
            goalLabel = new Label3D();
            goalLabel.Text = "GOAL";
            goalLabel.Transform = new Transform3D(Basis.Identity, (Vector3)goalPos + new Vector3(0,0.5f,0));
            goalLabel.Modulate = new Color(1.0f,0.2f,0.2f);
            visualContainer.AddChild(goalLabel);
        }

        // draw path if both exist and AStarManager sibling available
        if (spawnPos != null && goalPos != null)
        {
            // try to locate AStarManager as a sibling of the Map node (../../AStarManager)
            Node astarn = GetNodeOrNull<Node>("../../AStarManager");
            if (astarn != null && astarn is AStarManager asm)
            {
                // pass grid indices as WaveManager does
                int sx = (int)((Vector3)spawnPos.Value).X;
                int sz = (int)((Vector3)spawnPos.Value).Z;
                int gx = (int)((Vector3)goalPos.Value).X;
                int gz = (int)((Vector3)goalPos.Value).Z;
                // convert back to grid indices (world pos was x*TILE_SIZE)
                sx = sx / (int)TILE_SIZE;
                sz = sz / (int)TILE_SIZE;
                gx = gx / (int)TILE_SIZE;
                gz = gz / (int)TILE_SIZE;
                var path = asm.GetPath(new Vector3(sx,0,sz), new Vector3(gx,0,gz));
                if (path != null && path.Count > 0)
                {
                    // create small sphere markers for each waypoint
                    foreach (var p in path)
                    {
                        var m = new MeshInstance3D();
                        var sph = new SphereMesh();
                        sph.Radius = 0.08f;
                        m.Mesh = sph;
                        var mat = new StandardMaterial3D();
                        mat.AlbedoColor = new Color(1.0f,0.9f,0.2f);
                        m.MaterialOverride = mat;
                        m.Position = new Vector3(p.X, 0.25f, p.Z);
                        visualContainer.AddChild(m);
                        pathMarkers.Add(m);
                    }
                }
            }
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