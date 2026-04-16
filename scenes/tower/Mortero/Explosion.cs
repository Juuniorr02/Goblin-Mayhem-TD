using Godot;

public partial class Explosion : GpuParticles2D
{
    [Export] public float ParticleSize = 3.0f; 

    public override void _Ready()
    {
        // Accedemos al material de proceso (donde se definen tamaños, gravedad, etc)
        if (ProcessMaterial is ParticleProcessMaterial mat)
        {
            // Cambiamos la escala base de las partículas
            mat.ScaleMin = ParticleSize;
            mat.ScaleMax = ParticleSize * 1.5f; // Un poco de variación para que se vea mejor
        }

        OneShot = true;
        Emitting = true;
        Finished += OnFinished;
    }

    private void OnFinished() => QueueFree();
}
