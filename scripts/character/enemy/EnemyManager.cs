using Godot;

public partial class EnemyManager : Node
{
    [Export] public PackedScene EnemyScene;
    [Export] public float SpawnInterval = 1f;
    [Export] public int EnemiesPerWave = 10;

    private float timer = 0f;
    private int spawned = 0;

    private PathManager pathManager;
    private Node3D enemyContainer;

    public override void _Ready()
    {
        pathManager = GetParent().GetNode<PathManager>("PathManager");

        enemyContainer = new Node3D();
        enemyContainer.Name = "Enemies";
        // parent may still be initializing; defer the child addition to avoid "busy setting up children" errors
        GetParent().CallDeferred("add_child", enemyContainer);

        GD.Print("EnemyManager: deferred adding enemy container to parent");
    }

    public override void _Process(double delta)
    {
        if (spawned >= EnemiesPerWave)
            return;

        timer += (float)delta;
        if (timer >= SpawnInterval)
        {
            timer = 0;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (EnemyScene == null)
        {
            GD.PrintErr("EnemyScene NOT assigned!");
            return;
        }

        Enemy enemy = EnemyScene.Instantiate<Enemy>();
        enemy.PathPoints = pathManager.PathPoints;
        enemyContainer.AddChild(enemy);

        spawned++;
    }
}