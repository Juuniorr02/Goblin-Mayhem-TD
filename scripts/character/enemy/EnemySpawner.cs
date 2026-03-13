using Godot;

public partial class EnemySpawner : Node
{
    [Export] public PackedScene EnemyScene;
    [Export] public Node2D EnemiesContainer;

    [Export] public Path2D CaminoA;
    [Export] public Path2D CaminoB;
    [Export] public Path2D CaminoVolador;

    [Export] public EnemyData[] EnemyTypes;

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
    if (EnemyTypes.Length == 0)
    {
        GD.PrintErr("No hay enemigos en EnemyTypes");
        return;
    }

    EnemyData data = EnemyTypes[GD.RandRange(0, EnemyTypes.Length - 1)];

    if (data?.EnemyScene == null)
    {
        GD.PrintErr("EnemyData sin EnemyScene asignada: " + data?.EnemyName);
        return;
    }

    Path2D selectedPath = null;

    if (data.IsFlying)
        selectedPath = CaminoVolador;
    else
        selectedPath = GD.Randf() < 0.5f ? CaminoA : CaminoB;

    if (selectedPath == null)
    {
        GD.PrintErr("SelectedPath es null para: " + data.EnemyName);
        return;
    }

    PathFollow2D follow = new PathFollow2D
    {
        Loop = false,
        Progress = 0,
        Rotates = false
    };
    selectedPath.AddChild(follow);

    Enemy enemy = data.EnemyScene.Instantiate<Enemy>();
    enemy.Data = data;

    follow.AddChild(enemy);

    GD.Print("Spawn: " + data.EnemyName + " en " + selectedPath.Name);
}
}