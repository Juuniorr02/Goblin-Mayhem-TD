using Godot;
using System;

public partial class TowerCannon : Sprite3D
{
    [Export] public float Range = 5.0f;
    [Export] public float FireRate = 1.0f; // shots per second
    private double cooldown = 0.0;
    private Camera3D cam;

    public override void _Process(double delta)
    {
        // make the sprite face the camera (Y axis only)
        if (cam == null) cam = GetViewport().GetCamera3D();
        if (cam != null)
        {
            Vector3 dirToCam = cam.GlobalPosition - GlobalPosition;
            float yaw = (float)Math.Atan2(dirToCam.X, dirToCam.Z);
            Rotation = new Vector3(0, yaw, 0);
        }
        cooldown -= delta;
        if (cooldown <= 0)
        {
            // simple placeholder: find nearest enemy and "shoot" (not implemented fully)
            var enemy = FindNearestEnemy();
            if (enemy != null)
            {
                ShootAt(enemy);
                cooldown = 1.0 / FireRate;
            }
        }
    }

    private Node3D FindNearestEnemy()
    {
        float best = float.MaxValue;
        Node3D bestNode = null;
        foreach (var child in GetTree().GetNodesInGroup("enemies"))
        {
            if (child is Node3D n)
            {
                float d = GlobalPosition.DistanceTo(n.GlobalPosition);
                if (d < best && d <= Range)
                {
                    best = d;
                    bestNode = n;
                }
            }
        }
        return bestNode;
    }

    private void ShootAt(Node3D enemy)
    {
        // placeholder: simply call a damage method if exists
        if (enemy.HasMethod("ApplyDamage"))
            enemy.Call("ApplyDamage", 10);
    }
}
