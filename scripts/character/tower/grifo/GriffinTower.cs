using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class GriffinTower : BaseTower
{
    private List<Griffin> _griffins = new();
    
    [ExportGroup("Griffin Settings")]
    [Export] public int MaxGriffins = 2;
    [Export] public float AttackDistance = 80f; // Distancia para considerar que ha llegado

    public override void _Ready()
    {
        base._Ready();
        
        // Creamos los grifos al inicio
        for (int i = 0; i < MaxGriffins; i++)
        {
            SpawnGriffin();
        }
    }

    private void SpawnGriffin()
    {
        if (BulletScene == null) return;
        
        var instance = BulletScene.Instantiate<Griffin>();
        // Añadimos a la escena para movimiento independiente
        GetTree().CurrentScene.AddChild(instance); 
        
        // Setup diferido para asegurar posición global correcta
        instance.CallDeferred(nameof(instance.SetupGriffin), GlobalPosition);
        _griffins.Add(instance);
    }

    protected override void UpdateTarget()
    {
        // Limpiar enemigos inválidos
        enemiesInRange.RemoveAll(e => !IsInstanceValid(e) || !e.IsInsideTree());

        if (enemiesInRange.Count == 0)
        {
            currentTarget = null;
            foreach (var g in _griffins) if (IsInstanceValid(g)) g.Target = null;
            if (shootTimer != null) shootTimer.Stop();
            return;
        }

        // Ordenar enemigos por cercanía
        var sortedEnemies = enemiesInRange
            .OrderBy(e => GlobalPosition.DistanceSquaredTo(e.GlobalPosition))
            .ToList();

        // Actualizamos el target principal para que la BaseTower mantenga el Timer vivo
        currentTarget = sortedEnemies.FirstOrDefault();

        // Repartir objetivos a los grifos
        for (int i = 0; i < _griffins.Count; i++)
        {
            var g = _griffins[i];
            if (!IsInstanceValid(g)) continue;

            if (i < sortedEnemies.Count)
                g.Target = sortedEnemies[i];
            else
                g.Target = sortedEnemies[0]; // Si sobran grifos, ayudan con el primero
        }

        // Aseguramos que el timer de disparo esté corriendo
        if (shootTimer != null && shootTimer.IsStopped())
            shootTimer.Start();
    }

    protected override void Shoot()
    {
        foreach (var g in _griffins)
        {
            if (IsInstanceValid(g) && IsInstanceValid(g.Target))
            {
                float distance = g.GlobalPosition.DistanceTo(g.Target.GlobalPosition);
                
                // Si el grifo está en rango de ataque
                if (distance <= AttackDistance)
                {
                    if (g.Target.HasMethod("TakeDamage"))
                    {
                        g.Target.Call("TakeDamage", Damage);
                    }
                }
            }
        }
    }

    public override void Build()
    {
        int amountGold = 0, amountWood = 0, amountStone = 0, amountIron = 0;
        
        amountGold = 100; amountWood = 50; amountStone = 0; amountIron = 0;

        if (Recursos.Instance.Gold >= amountGold && Recursos.Instance.Wood >= amountWood && Recursos.Instance.Stone >= amountStone && Recursos.Instance.Iron >= amountIron)
        {
            Recursos.Instance.Gold -= amountGold;
            Recursos.Instance.Wood -= amountWood;
            Recursos.Instance.Stone -= amountStone;
            Recursos.Instance.Iron -= amountIron;

            CanBuild = true;
        }
        else
        {
            CanBuild = false;
        }
    }
}
