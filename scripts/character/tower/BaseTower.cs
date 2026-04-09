using Godot;
using System.Collections.Generic;
using System.Linq;

public abstract partial class BaseTower : Node2D
{
    [Export] public PackedScene BulletScene;
    [Export] public float Damage = 10f;
    
    [Export] public bool CanTargetLand = true;
    [Export] public bool CanTargetAir = false;
    [Export] public bool CanTargetWater = false;

    protected List<Node2D> enemiesInRange = new();
    protected Node2D currentTarget;
    protected Marker2D muzzle;
    protected Timer shootTimer;

    public override void _Ready()
    {
        muzzle = GetNodeOrNull<Marker2D>("muzzle") ?? GetNodeOrNull<Marker2D>("Muzzle");
        shootTimer = GetNodeOrNull<Timer>("shootTimer") ?? GetNodeOrNull<Timer>("ShootTimer");

        var area = GetNodeOrNull<Area2D>("DetectionRange") ?? GetNodeOrNull<Area2D>("detectionRange");
        if (area != null)
        {
            area.BodyEntered += (body) => {
                if (body.IsInGroup("enemies") && body is Node2D n)
                    enemiesInRange.Add(n);
            };
            area.BodyExited += (body) => {
                if (body is Node2D n)
                    enemiesInRange.Remove(n);
            };
        }

        if (shootTimer != null)
            shootTimer.Timeout += Shoot;
    }

    public override void _Process(double delta) => UpdateTarget();

    protected virtual void UpdateTarget()
    {
        // 1. Limpiar inválidos
        enemiesInRange.RemoveAll(e => !IsInstanceValid(e) || !e.IsInsideTree());

        // 2. Filtrar y Ordenar por prioridad
        // Ordenamos: Primero los que son Flying (True > False) y luego por distancia
        var bestTarget = enemiesInRange
            .Where(e => {
                if (e is Enemy enemyScript && enemyScript.Data != null)
                {
                    var data = enemyScript.Data;
                    if (data.IsFlying && !CanTargetAir) return false;
                    if (data.IsAquatic && !CanTargetWater) return false;
                    if (!data.IsFlying && !data.IsAquatic && !CanTargetLand) return false;
                    return true;
                }
                return false;
            })
            .OrderByDescending(e => ((Enemy)e).Data.IsFlying) // Los voladores van primero
            .ThenBy(e => GlobalPosition.DistanceSquaredTo(e.GlobalPosition)) // Luego el más cercano
            .FirstOrDefault();

        // 3. Asignar objetivo
        if (bestTarget != null)
        {
            currentTarget = bestTarget;
            if (shootTimer.IsStopped()) shootTimer.Start();
        }
        else
        {
            currentTarget = null;
            shootTimer.Stop();
        }
    }

    protected abstract void Shoot();
}
