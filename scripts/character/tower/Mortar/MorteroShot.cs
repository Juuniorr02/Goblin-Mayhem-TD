using Godot;
using System.Collections.Generic;

public partial class MorteroShot : Area2D
{
    [Export] public float Speed = 400.0f;
    [Export] public float ArcHeight = 200.0f; 
    [Export] public float ExplosionRadius = 70.0f;
    
    // IMPORTANTE: PackedScene permite arrastrar el archivo .tscn desde el inspector
    [Export] public PackedScene ExplosionEffect; 

    private Vector2 startPos;
    private Vector2 targetPos;
    private float progress = 0.0f;
    private float damage;
    private Node2D visualContainer;

    public void Launch(Vector2 target, float damageValue)
    {
        startPos = GlobalPosition;
        targetPos = target;
        damage = damageValue;
        
        // Buscamos el contenedor visual que subirá y bajará
        visualContainer = GetNode<Node2D>("VisualContainer");

        float distance = startPos.DistanceTo(targetPos);
        float duration = distance / Speed;

        // Creamos el movimiento suave de 0 a 1
        Tween tween = CreateTween();
        tween.TweenProperty(this, "progress", 1.0f, duration);
        tween.Finished += OnImpact;
    }

    public override void _Process(double delta)
    {
        // 1. Mover la base (la sombra) hacia el destino
        GlobalPosition = startPos.Lerp(targetPos, progress);

        // 2. Calcular la parábola visual (altura)
        // La fórmula 4*h*p*(1-p) llega al pico máximo en la mitad del camino
        float currentHeight = 4 * ArcHeight * progress * (1.0f - progress);
        
        if (visualContainer != null)
        {
            // Movemos el sprite hacia arriba (Y negativo)
            visualContainer.Position = new Vector2(0, -currentHeight);
            
            // Opcional: que el proyectil rote un poco en el aire
            visualContainer.Rotation += 5.0f * (float)delta;
        }
    }

    private void OnImpact()
    {
        // 1. Crear el efecto de explosión
        if (ExplosionEffect != null)
        {
            // Instanciamos como GpuParticles2D (o Node2D si es una escena compleja)
            var exp = ExplosionEffect.Instantiate<GpuParticles2D>();
            GetTree().CurrentScene.AddChild(exp);
            exp.GlobalPosition = GlobalPosition;
        }

        // 2. Aplicar el daño en área
        ApplyExplosionDamage();

        // 3. Eliminar el proyectil
        QueueFree();
    }

    private void ApplyExplosionDamage()
    {
        // Buscamos enemigos en el radio usando la física de Godot
        var spaceState = GetWorld2D().DirectSpaceState;
        var query = new PhysicsShapeQueryParameters2D();
        
        var circle = new CircleShape2D();
        circle.Radius = ExplosionRadius;
        
        query.Shape = circle;
        query.Transform = GlobalTransform;
        query.CollisionMask = CollisionMask; // Usa la máscara configurada en el Area2D

        var results = spaceState.IntersectShape(query);

        foreach (var result in results)
        {
            var body = result["collider"].As<Node>();
            if (body.IsInGroup("enemies") && body.HasMethod("TakeDamage"))
            {
                body.Call("TakeDamage", damage);
            }
        }
    }
}
