using Godot;

public partial class EnemySpawner : Node
{
    [Export] public PackedScene EnemyScene;
    [Export] public Node2D EnemiesContainer;

    [Export] public Path2D CaminoA;
    [Export] public Path2D CaminoB;

    [Export] public float SpawnInterval = 1.0f;
    [Export] public int EnemiesPerWave = 10;

    private int enemiesSpawned = 0;
    private float spawnTimer = 0f;
    private bool spawning = false;

    public override void _Process(double delta)
    {
        if (!spawning) return;

        spawnTimer += (float)delta;

        if (spawnTimer >= SpawnInterval && enemiesSpawned < EnemiesPerWave)
        {
            SpawnEnemyRandomPath();
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

private void SpawnEnemyRandomPath()
{
    if (EnemyScene == null) return;

    // Elegir camino aleatorio
    Path2D selectedPath = GD.Randf() < 0.7f ? CaminoA : CaminoB;
    if (selectedPath == null) return;

    // Crear PathFollow2D y ponerlo en el Path2D
    PathFollow2D follow = new PathFollow2D
    {
        Loop = false,
        Progress = 0,
        Rotates = true // ← rotación automática en Godot 4
    };
    selectedPath.AddChild(follow);

    // Instanciar enemigo y agregarlo como hijo del PathFollow2D
    Enemy enemy = EnemyScene.Instantiate<Enemy>();
    follow.AddChild(enemy);

    GD.Print("[Spawner] Spawneando enemigo en " + selectedPath.Name);
}
}