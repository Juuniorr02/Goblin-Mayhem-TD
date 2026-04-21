using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class TunaHatchery : BaseTower
{
    [Export] public int MaxTunas = 3;
    [Export] public float SpawnCooldown = 10.0f;
    [Export] public float ExplosionDamage = 25f;
    [Export] public float ExplosionRadius = 80f;

    private List<Node2D> _activeTunas = new();
    private float _spawnTimer = 0f;

    public override void _Ready()
    {
        base._Ready();
        // Inicialmente spawneamos los que quepan
        for(int i = 0; i < MaxTunas; i++) SpawnTuna();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        // Timer manual de spawn
        _spawnTimer += (float)delta;
        if (_spawnTimer >= SpawnCooldown)
        {
            _spawnTimer = 0f;
            SpawnTuna();
        }
    }

    private void SpawnTuna()
    {
        _activeTunas.RemoveAll(t => !IsInstanceValid(t));

        if (_activeTunas.Count < MaxTunas && BulletScene != null)
        {
            var tuna = BulletScene.Instantiate<Node2D>();
            GetTree().CurrentScene.AddChild(tuna);
            
            tuna.GlobalPosition = GlobalPosition + new Vector2((float)GD.RandRange(-20, 20), (float)GD.RandRange(-20, 20));
            
            // Pasamos datos al atún (asumiendo que tiene script de Tuna)
            if (tuna.HasMethod("SetupTuna"))
                tuna.Call("SetupTuna", GlobalPosition, Damage, ExplosionRadius);

            _activeTunas.Add(tuna);
        }
    }

    protected override void UpdateTarget()
    {
        base.UpdateTarget();
        foreach (var t in _activeTunas)
        {
            if (IsInstanceValid(t))
                t.Set("Target", currentTarget);
        }
    }

    protected override void Shoot() { /* Los atunes son el proyectil */ }
}
