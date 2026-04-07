using Godot;
using System.Collections.Generic;

public abstract partial class BaseTower : Node2D
{
    [Export] public PackedScene BulletScene;
    [Export] public float RotationSpeed = 10.0f;
    [Export] public float Damage = 10f;

    protected List<Node2D> enemiesInRange = new();
    protected Node2D currentTarget;
    protected Marker2D muzzle;
    protected Timer shootTimer;

    public override void _Ready()
    {
        muzzle = GetNodeOrNull<Marker2D>("Muzzle");
        shootTimer = GetNodeOrNull<Timer>("ShootTimer");

        if (muzzle == null || shootTimer == null)
        {
            GD.PrintErr("Tower setup incorrect: falta Muzzle o ShootTimer");
            return;
        }

        var area = GetNode<Area2D>("DetectionRange");

        area.BodyEntered += (body) =>
        {
            if (body.IsInGroup("Enemies") && body is Node2D n)
                enemiesInRange.Add(n);
        };

        area.BodyExited += (body) =>
        {
            if (body is Node2D n)
                enemiesInRange.Remove(n);
        };

        shootTimer.Timeout += Shoot;
    }

    public override void _Process(double delta)
    {
        UpdateTarget();

        if (currentTarget == null || !IsInstanceValid(currentTarget))
            return;

        Vector2 direction = currentTarget.GlobalPosition - GlobalPosition;

        Rotation = Mathf.LerpAngle(
            Rotation,
            direction.Angle(),
            RotationSpeed * (float)delta
        );
    }

    protected virtual void UpdateTarget()
    {
        enemiesInRange.RemoveAll(e => !IsInstanceValid(e));

        float minDist = float.MaxValue;
        Node2D best = null;

        foreach (var enemy in enemiesInRange)
        {
            float dist = GlobalPosition.DistanceTo(enemy.GlobalPosition);
            if (dist < minDist)
            {
                minDist = dist;
                best = enemy;
            }
        }

        currentTarget = best;

        if (currentTarget != null && shootTimer.IsStopped())
            shootTimer.Start();
        else if (currentTarget == null)
            shootTimer.Stop();
    }

    protected abstract void Shoot();
}