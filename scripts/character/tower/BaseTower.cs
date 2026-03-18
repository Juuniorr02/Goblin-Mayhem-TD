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
        muzzle = GetNode<Marker2D>("Muzzle");
        shootTimer = GetNode<Timer>("ShootTimer");
        
        var area = GetNode<Area2D>("DetectionRange");
        area.BodyEntered += (body) => { if (body is Node2D n) enemiesInRange.Add(n); };
        area.BodyExited += (body) => { if (body is Node2D n) enemiesInRange.Remove(n); };
        
        shootTimer.Timeout += Shoot;
    }

    public override void _Process(double delta)
    {
        UpdateTarget();
        if (currentTarget != null && IsInstanceValid(currentTarget))
        {
            Vector2 direction = currentTarget.GlobalPosition - GlobalPosition;
            Rotation = (float)Mathf.LerpAngle(Rotation, direction.Angle(), RotationSpeed * (float)delta);
        }
    }

    protected virtual void UpdateTarget()
    {
        enemiesInRange.RemoveAll(e => !IsInstanceValid(e));
        currentTarget = enemiesInRange.Count > 0 ? enemiesInRange[0] : null;

        if (currentTarget != null && shootTimer.IsStopped()) shootTimer.Start();
        else if (currentTarget == null) shootTimer.Stop();
    }

    // Cada torre puede disparar de forma diferente (proyectil, rayo, etc.)
    protected abstract void Shoot();
}
