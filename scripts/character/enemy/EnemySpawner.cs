using Godot;

public partial class EnemySpawner : Node
{
    [Export] public PackedScene EnemyScene;
    [Export] public Pathfinding PathfindingNode;
    [Export] public Node2D EnemiesContainer;

    [Export] public float SpawnInterval = 1.0f;
    [Export] public int EnemiesPerWave = 10;

    private int enemiesSpawned = 0;
    private float spawnTimer = 0f;

    private Vector2I spawnTile;
    private Vector2I baseTile;
    private bool spawning = false;

    public override void _Ready()
    {
        if (PathfindingNode == null)
        {
            GD.PrintErr("EnemySpawner: PathfindingNode no asignado.");
            return;
        }

        spawnTile = FindTileById(122);
        baseTile = FindTileById(136);
    }

    public override void _Process(double delta)
    {
        if (!spawning) return;

        spawnTimer += (float)delta;
        if (spawnTimer >= SpawnInterval && enemiesSpawned < EnemiesPerWave)
        {
            SpawnEnemy();
            spawnTimer = 0f;
            enemiesSpawned++;
        }
    }

    public void StartWave()
    {
        enemiesSpawned = 0;
        spawnTimer = 0f;
        spawning = true;
    }

    private void SpawnEnemy()
    {
        if (EnemyScene == null || EnemiesContainer == null) return;

        var enemy = EnemyScene.Instantiate<Enemy>();

        // Obtener path en tiles
        Vector2I[] pathTiles = PathfindingNode.GetPath(spawnTile, baseTile);

        // Convertir path a posiciones globales sobre la capa de suelo
        Vector2[] pathGlobal = new Vector2[pathTiles.Length];
        for (int i = 0; i < pathTiles.Length; i++)
            pathGlobal[i] = PathfindingNode.TileToGlobalPos(pathTiles[i]);

        enemy.SetPath(pathGlobal);

        EnemiesContainer.AddChild(enemy);
    }

    private Vector2I FindTileById(int tileId)
    {
        var tileMap = PathfindingNode.tileMap;
        if (tileMap == null) return Vector2I.Zero;

        Rect2I usedRect = tileMap.GetUsedRect();
        for (int x = usedRect.Position.X; x < usedRect.Position.X + usedRect.Size.X; x++)
        {
            for (int y = usedRect.Position.Y; y < usedRect.Position.Y + usedRect.Size.Y; y++)
            {
                Vector2I pos = new Vector2I(x, y);
                if (tileMap.GetCellSourceId(pos) == tileId)
                    return pos;
            }
        }

        GD.PrintErr("No se encontró el tile con ID " + tileId);
        return Vector2I.Zero;
    }
}