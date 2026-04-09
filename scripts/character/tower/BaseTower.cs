using Godot;
using System.Collections.Generic;

public abstract partial class BaseTower : Node2D
{
    [Export] public PackedScene BulletScene;
    [Export] public float Damage = 10f;

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

    public override void _Process(double delta)
    {
        UpdateTarget();
        // Se ha eliminado la lógica de rotación aquí
    }

    protected virtual void UpdateTarget()
    {
        enemiesInRange.RemoveAll(e => !IsInstanceValid(e) || !e.IsInsideTree());

        if (enemiesInRange.Count > 0)
        {
            currentTarget = enemiesInRange[0]; // Apuntar al primero en la lista
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
