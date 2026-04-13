using Godot;

public partial class Explosion : GpuParticles2D
{
    public override void _Ready()
    {
        // Aseguramos que emita solo una vez
        OneShot = true;
        Emitting = true;

        Finished += OnFinished;
    }

    private void OnFinished()
    {
        QueueFree();
    }
}
