using Godot;
using System.Collections.Generic;
using System.Linq;

public abstract partial class BaseTower : Node2D
{
    [Export] public PackedScene BulletScene;
    [Export] public float Damage = 10f;
    
    // NUEVO: Export para la velocidad de disparo (Disparos por segundo)
    // Ejemplo: 1.0 = un disparo cada segundo | 0.5 = un disparo cada 2 segundos | 10.0 = metralleta
    [Export] public float FireRate = 1.0f; 

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

        // NUEVO: Configuramos el tiempo del timer basado en el FireRate
        if (shootTimer != null)
        {
            // El WaitTime es el inverso de la cadencia (1 / disparos por segundo)
            shootTimer.WaitTime = 1.0f / Mathf.Max(FireRate, 0.01f); // Evitamos división por cero
            shootTimer.Timeout += Shoot;
        }

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
    }

    public override void _Process(double delta) => UpdateTarget();

    protected virtual void UpdateTarget()
    {
        enemiesInRange.RemoveAll(e => !IsInstanceValid(e) || !e.IsInsideTree());

        var validEnemies = enemiesInRange.Where(e => {
            if (e is Enemy enemyScript && enemyScript.Data != null)
            {
                var data = enemyScript.Data;
                if (data.IsFlying && !CanTargetAir) return false;
                if (data.IsAquatic && !CanTargetWater) return false;
                if (!data.IsFlying && !data.IsAquatic && !CanTargetLand) return false;
                return true;
            }
            return CanTargetLand; 
        });

        if (validEnemies.Any())
        {
            currentTarget = validEnemies
                .OrderByDescending(e => (e is Enemy en && en.Data != null) ? en.Data.IsFlying : false)
                .ThenBy(e => GlobalPosition.DistanceSquaredTo(e.GlobalPosition))
                .FirstOrDefault();

            if (shootTimer != null && shootTimer.IsStopped()) 
                shootTimer.Start();
        }
        else
        {
            currentTarget = null;
            if (shootTimer != null) 
                shootTimer.Stop();
        }
    }

    protected abstract void Shoot();
}
