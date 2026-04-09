using Godot;
using System.Collections.Generic;
using System.Linq;

public abstract partial class BaseTower : Node2D
{
    [Export] public PackedScene BulletScene;
    [Export] public float Damage = 10f;
    
    // Configuración de la torre (Ajustable desde el Inspector)
    [Export] public bool CanTargetLand = true;
    [Export] public bool CanTargetAir = false;
    [Export] public bool CanTargetWater = false;

    protected List<Node2D> enemiesInRange = new();
    protected Node2D currentTarget;
    protected Marker2D muzzle;
    protected Timer shootTimer;

    public override void _Ready()
    {
        // Buscamos los nodos necesarios
        muzzle = GetNodeOrNull<Marker2D>("muzzle") ?? GetNodeOrNull<Marker2D>("Muzzle");
        shootTimer = GetNodeOrNull<Timer>("shootTimer") ?? GetNodeOrNull<Timer>("ShootTimer");

        // Configuración del área de detección
        var area = GetNodeOrNull<Area2D>("DetectionRange") ?? GetNodeOrNull<Area2D>("detectionRange");
        if (area != null)
        {
            area.BodyEntered += (body) => {
                if (body.IsInGroup("enemies") && body is Node2D n)
                    enemiesInRange.Add(n);
            };
            area.BodyExited += (body) => {
                if (body is Node2D n)
                    enemiesInRange.Remove(n);
            };
        }

        if (shootTimer != null)
            shootTimer.Timeout += Shoot;
    }

    public override void _Process(double delta) => UpdateTarget();

    protected virtual void UpdateTarget()
    {
        // 1. Limpiar enemigos que han muerto o salido de la escena
        enemiesInRange.RemoveAll(e => !IsInstanceValid(e) || !e.IsInsideTree());

        // 2. Filtrar enemigos que la torre PUEDE atacar legalmente
        var validEnemies = enemiesInRange.Where(e => {
            if (e is Enemy enemyScript && enemyScript.Data != null)
            {
                var data = enemyScript.Data;
                if (data.IsFlying && !CanTargetAir) return false;
                if (data.IsAquatic && !CanTargetWater) return false;
                if (!data.IsFlying && !data.IsAquatic && !CanTargetLand) return false;
                return true;
            }
            // Si el enemigo no tiene script 'Enemy', asumimos que es terrestre
            return CanTargetLand; 
        });

        // 3. Elegir el mejor objetivo (Prioridad Aérea + Cercanía)
        if (validEnemies.Any())
        {
            currentTarget = validEnemies
                .OrderByDescending(e => (e is Enemy en && en.Data != null) ? en.Data.IsFlying : false)
                .ThenBy(e => GlobalPosition.DistanceSquaredTo(e.GlobalPosition))
                .FirstOrDefault();

            // Iniciar el temporizador de disparo si no está corriendo
            if (shootTimer != null && shootTimer.IsStopped()) 
                shootTimer.Start();
        }
        else
        {
            currentTarget = null;
            if (shootTimer != null) 
                shootTimer.Stop();
        }
    }

    // Este método lo llenan Cannon.cs o ArcherTower.cs
    protected abstract void Shoot();
}
