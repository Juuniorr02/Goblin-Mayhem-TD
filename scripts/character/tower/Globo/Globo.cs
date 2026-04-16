using Godot;

public partial class Globo : BaseTower
{
    [ExportGroup("Movimiento del Globo")]
    [Export] public float FloatAmplitude = 10.0f; // Arriba/Abajo
    [Export] public float FloatSpeed = 2.0f;
    [Export] public float DriftRadius = 15.0f;   // Radio máximo de movimiento lateral
    [Export] public float DriftSpeed = 0.5f;    // Velocidad del vaivén lateral

    private Vector2 _anchorPosition;
    private float _timePassed = 0.0f;

    public override void _Ready()
    {
        base._Ready();
        // Guardamos la posición exacta donde lo colocaste como centro
        _anchorPosition = Position;
        
        CanTargetLand = true;
        CanTargetAir = false; 
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        _timePassed += (float)delta;

        // 1. Calculamos el movimiento lateral (Eje X e Y de forma circular/aleatoria)
        // Usamos Sin y Cos con diferentes velocidades para que no sea un círculo perfecto
        float driftX = Mathf.Sin(_timePassed * DriftSpeed) * DriftRadius;
        float driftY = Mathf.Cos(_timePassed * DriftSpeed * 0.7f) * (DriftRadius * 0.5f);

        // 2. Calculamos el balanceo vertical (Flotar)
        float floatY = Mathf.Sin(_timePassed * FloatSpeed) * FloatAmplitude;

        // 3. Aplicamos la posición final relativa al anclaje
        Position = new Vector2(
            _anchorPosition.X + driftX,
            _anchorPosition.Y + driftY + floatY
        );
    }

    protected override void Shoot()
    {
        if (!IsInstanceValid(currentTarget) || BulletScene == null) return;

        var bombNode = BulletScene.Instantiate();
        GetTree().CurrentScene.AddChild(bombNode);

        if (bombNode is GloboBomba bomb)
        {
            bomb.GlobalPosition = muzzle?.GlobalPosition ?? GlobalPosition;
            bomb.Launch(currentTarget.GlobalPosition, Damage);
            GD.Print("[GLOBO] Bomba soltada desde posición a la deriva.");
        }
    }
}
