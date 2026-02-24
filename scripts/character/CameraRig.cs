using Godot;

public partial class CameraRig : Camera3D
{
    [Export] public float MoveSpeed = 10f;
    [Export] public float ZoomSpeed = 2f;
    [Export] public float MinHeight = 5f;
    [Export] public float MaxHeight = 50f;
    [Export] public float RotateSpeed = 0.3f;

    private bool rotating = false;

    public override void _Input(InputEvent @event)
    {
        // ZOOM con rueda
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Pressed)
            {
                if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
                    Zoom(1);
                else if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
                    Zoom(-1);

                // Rotación con botón derecho
                if (mouseEvent.ButtonIndex == MouseButton.Right)
                    rotating = true;
            }
            else if (mouseEvent.ButtonIndex == MouseButton.Right)
                rotating = false;
        }

        // ROTACIÓN con drag derecho
        if (@event is InputEventMouseMotion motionEvent && rotating)
        {
            Vector2 delta = motionEvent.Relative;
            RotateY(-delta.X * RotateSpeed * 0.01f);

            float newPitch = RotationDegrees.X - delta.Y * RotateSpeed * 0.01f;
            newPitch = Mathf.Clamp(newPitch, -80, -15);
            RotationDegrees = new Vector3(newPitch, RotationDegrees.Y, RotationDegrees.Z);
        }
    }

    public override void _Process(double delta)
    {
        Vector3 inputDir = Vector3.Zero;

        if (Input.IsActionPressed("cam_up")) inputDir.Z -= 1;
        if (Input.IsActionPressed("cam_down")) inputDir.Z += 1;
        if (Input.IsActionPressed("cam_left")) inputDir.X -= 1;
        if (Input.IsActionPressed("cam_right")) inputDir.X += 1;

        if (inputDir != Vector3.Zero)
        {
            inputDir = inputDir.Normalized();

            // Use the camera's orientation to steer movement, but ignore any vertical component
            // so the rig stays at the same Y height.
            // Transform the input vector by the camera's basis, then zero out Y.
            Vector3 direction = Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Z);
            direction.Y = 0;
            if (direction != Vector3.Zero)
                direction = direction.Normalized();

            GlobalPosition += direction * MoveSpeed * (float)delta;
        }
    }

    private void Zoom(float direction)
    {
        Vector3 pos = GlobalPosition;
        pos.Y = Mathf.Clamp(pos.Y + direction * ZoomSpeed, MinHeight, MaxHeight);
        GlobalPosition = pos;
    }
}