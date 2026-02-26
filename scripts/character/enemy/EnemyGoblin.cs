using Godot;
using System.Collections.Generic;

public partial class EnemyGoblin : Sprite3D
{
    [Export] public float Speed = 2.0f;
    [Export] public int MaxHealth = 20;

    private int health;
    private List<Vector3> waypoints = new();
    private int current = 0;

    public override void _Ready()
    {
        health = MaxHealth;
        AddToGroup("enemies");
    }

    public void SetWaypoints(List<Vector3> pts)
    {
        waypoints = pts ?? new List<Vector3>();
        current = 0;
        if (waypoints.Count > 0)
            GlobalPosition = waypoints[0];
    }

    public override void _Process(double delta)
    {
        // face camera
        var cam = GetViewport().GetCamera3D();
        if (cam != null)
        {
            var dirToCam = cam.GlobalPosition - GlobalPosition;
            float yaw = (float)System.Math.Atan2(dirToCam.X, dirToCam.Z);
            Rotation = new Vector3(0, yaw, 0);
        }

        if (waypoints.Count == 0) return;

        if (current >= waypoints.Count - 1)
        {
            QueueFree();
            return;
        }

        Vector3 target = waypoints[current + 1];
        Vector3 dir = (target - GlobalPosition).Normalized();
        GlobalPosition += dir * (float)(Speed * delta);

        if (GlobalPosition.DistanceTo(target) < 0.1f)
            current++;
    }

    public void ApplyDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0) QueueFree();
    }
}
