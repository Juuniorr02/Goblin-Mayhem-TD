using Godot;
using System;

/// <summary>
/// Attach to a Sprite3D (or other Node3D) to make it face the active Camera3D each frame.
/// It will rotate only around the Y axis so the sprite remains upright.
/// </summary>
public partial class Billboard2D : Node3D
{
    private Camera3D cam;

    public override void _Ready()
    {
        cam = GetViewport().GetCamera3D();
    }

    public override void _Process(double delta)
    {
        if (cam == null) cam = GetViewport().GetCamera3D();
        if (cam == null) return;

        Vector3 dir = cam.GlobalPosition - GlobalPosition;
        // compute yaw only
        float yaw = (float)Math.Atan2(dir.X, dir.Z);
        Rotation = new Vector3(0, yaw, 0);
    }
}
