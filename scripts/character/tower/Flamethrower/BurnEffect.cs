using Godot;

public partial class BurnEffect : Node
{
    private float _dmgPerSecond;
    private float _duration;
    private float _elapsed = 0f;
    private float _tickTimer = 0f;

    // ESTO ES LO QUE FALTA: El constructor que recibe los 2 argumentos
    public BurnEffect(float dmg, float duration)
    {
        _dmgPerSecond = dmg;
        _duration = duration;
    }

    public void Reset() => _elapsed = 0f;

        public override void _Process(double delta)
    {
        _elapsed += (float)delta;
        _tickTimer += (float)delta;

        if (_tickTimer >= 0.5f)
        {
            if (GetParent() is Enemy enemy)
            {
                enemy.TakeDamage(_dmgPerSecond * 0.5f);
                enemy.Modulate = new Color(2, 1, 0.5f); // Naranja brillante
            }
            _tickTimer = 0f;
        }
        else if (_tickTimer > 0.2f) // Un poco más de tiempo para el parpadeo
        {
            if (GetParent() is CanvasItem parent)
                parent.Modulate = new Color(1, 1, 1); // Normal
        }

        if (_elapsed >= _duration)
        {
            // Antes de morir, aseguramos el color
            if (GetParent() is CanvasItem parent)
                parent.Modulate = new Color(1, 1, 1);
            QueueFree();
        }
    }

    // Por seguridad, si el efecto se borra por cualquier otra razón (ej. muere el enemigo)
    public override void _ExitTree()
    {
        if (GetParent() is CanvasItem parent && IsInstanceValid(parent))
        {
            parent.Modulate = new Color(1, 1, 1);
        }
    }

}
