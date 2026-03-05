using Godot;

public partial class Enemy : PathFollow2D
{
    [Export] public float Speed = 100f; // píxeles por segundo
    [Export] public int DamageToBase = 1;

    // Nodo opcional del Sprite para rotar si quieres que mire hacia el camino
    private Sprite2D sprite;

    public override void _Ready()
    {
        
    }

    public override void _Process(double delta)
    {
        // Avanza por el camino
        Progress += Speed * (float)delta;

        // Detectar fin del camino
        if (Progress >= GetParent<Path2D>().Curve.GetBakedLength())
        {
            ReachEnd();
        }
    }

    private void ReachEnd()
    {
        // Aquí puedes restar vida a la base, reproducir animación, etc.
        GD.Print("Enemigo llegó a la base!");

        QueueFree(); // destruir enemigo
    }
}