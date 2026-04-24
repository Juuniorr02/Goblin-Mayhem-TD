using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class TunaHatchery : BaseTower
{
    private List<Tuna> _activeTunas = new();
    
    [ExportGroup("Hatchery Settings")]
    [Export] public int MaxTunas = 3;
    [Export] public float RespawnCooldown = 1.5f;
    private float _respawnTimer = 0f;

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Process(double delta)
    {
        // Limpiar tunas muertas
        _activeTunas.RemoveAll(t => !IsInstanceValid(t));

        // Respawn
        if (_activeTunas.Count < MaxTunas)
        {
            _respawnTimer += (float)delta;
            if (_respawnTimer >= RespawnCooldown)
            {
                SpawnTuna();
                _respawnTimer = 0f;
            }
        }

        UpdateTarget();
    }

    private void SpawnTuna()
    {
        if (BulletScene == null) return;
        
        var tuna = BulletScene.Instantiate<Tuna>();
        GetTree().CurrentScene.AddChild(tuna); 
        tuna.GlobalPosition = GlobalPosition + new Vector2((float)GD.RandRange(-20, 20), (float)GD.RandRange(-20, 20));
        tuna.SetupTuna(GlobalPosition, Damage);
        _activeTunas.Add(tuna);
    }

    protected override void UpdateTarget()
    {
        enemiesInRange.RemoveAll(e => !IsInstanceValid(e) || !e.IsInsideTree());

        if (enemiesInRange.Count == 0)
        {
            foreach (var t in _activeTunas) if (IsInstanceValid(t)) t.Target = null;
            return;
        }

        // Ordenar por el que está más cerca de la torre
        var targetEnemy = enemiesInRange.OrderBy(e => GlobalPosition.DistanceSquaredTo(e.GlobalPosition)).First();

        foreach (var tuna in _activeTunas)
        {
            if (IsInstanceValid(tuna))
            {
                tuna.Target = targetEnemy;
            }
        }
    }

    protected override void Shoot() { /* No hace nada manual */ }
}
