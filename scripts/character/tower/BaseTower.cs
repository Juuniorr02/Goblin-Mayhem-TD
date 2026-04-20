using Godot;
using System.Collections.Generic;
using System.Linq;

public abstract partial class BaseTower : Node2D
{
    [Export] public PackedScene BulletScene;
    [Export] public float Damage = 10f;
    [Export] public float FireRate = 1.0f; 

    [Export] public bool CanTargetLand = true;
    [Export] public bool CanTargetAir = false;
    [Export] public bool CanTargetWater = false;

    // Propiedad para activar el rango visual
    [Export] public bool IsGhost = false;

    protected List<Node2D> enemiesInRange = new();
    protected Node2D currentTarget;
    protected Marker2D muzzle;
    protected Timer shootTimer;
    protected Node2D rangeVisual; // Referencia al círculo visual

    public override void _Ready()
    {
        muzzle = GetNodeOrNull<Marker2D>("muzzle") ?? GetNodeOrNull<Marker2D>("Muzzle");
        shootTimer = GetNodeOrNull<Timer>("shootTimer") ?? GetNodeOrNull<Timer>("ShootTimer");
        rangeVisual = GetNodeOrNull<Node2D>("RangeVisual");

        if (shootTimer != null)
        {
            shootTimer.WaitTime = 1.0f / Mathf.Max(FireRate, 0.01f);
            shootTimer.Timeout += Shoot;
        }

        // Si es fantasma, mostramos el rango. Si no, lo ocultamos.
        if (rangeVisual != null)
        {
            rangeVisual.Visible = IsGhost;
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

    public override void _Process(double delta) 
    {
        if (!IsGhost) UpdateTarget();
    }

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
            currentTarget = validEnemies.OrderBy(e => GlobalPosition.DistanceSquaredTo(e.GlobalPosition)).FirstOrDefault();
            if (shootTimer != null && shootTimer.IsStopped()) shootTimer.Start();
        }
        else
        {
            currentTarget = null;
            if (shootTimer != null) shootTimer.Stop();
        }
    }

    protected abstract void Shoot();
}
