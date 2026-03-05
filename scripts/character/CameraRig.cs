using Godot;

public partial class CameraRig : Camera2D
{
	[Export] public float MoveSpeed = 900f;
	[Export] public float ZoomSpeed = 0.15f;

	[Export] public float MinZoom = 0.2f;
	[Export] public float MaxZoom = 4.0f;

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
				UpdateZoom(-1);

			if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
				UpdateZoom(1);
		}
	}

	public override void _Process(double delta)
	{
		Vector2 inputDir = Vector2.Zero;

		if (Input.IsActionPressed("cam_up")) inputDir.Y -= 1;
		if (Input.IsActionPressed("cam_down")) inputDir.Y += 1;
		if (Input.IsActionPressed("cam_left")) inputDir.X -= 1;
		if (Input.IsActionPressed("cam_right")) inputDir.X += 1;

		if (inputDir != Vector2.Zero)
		{
			inputDir = inputDir.Normalized();
			Position += inputDir * MoveSpeed * (float)delta;
		}
	}

	private void UpdateZoom(float direction)
	{
		Vector2 zoom = Zoom;

		zoom -= new Vector2(direction, direction) * ZoomSpeed;

		zoom.X = Mathf.Clamp(zoom.X, MinZoom, MaxZoom);
		zoom.Y = Mathf.Clamp(zoom.Y, MinZoom, MaxZoom);

		Zoom = zoom;
	}
}
