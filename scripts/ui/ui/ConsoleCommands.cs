using Godot;
using System;

public partial class ConsoleCommands : Node
{
    // Escribe 'speed 0.5' para cámara lenta o 'speed 1' para normal
    public string speed(float value)
    {
        Engine.TimeScale = value;
        return $"Velocidad del motor: {value}x";
    }

    public string help()
    {
        return "Comandos: restart, recursos, win, godmode, speed X";
    }

    public string restart()
    {
        BuildTime.CanBuild = true;
		Recursos.Instance.RepairBase();
		Wave.Instance.ResetWaves();
		GetTree().ReloadCurrentScene();
        return "Reiniciando...";
    }
    
    public string cls()
    {
        // Para limpiar la consola si quieres
        return "Consola limpia (pendiente implementar clear)";
    }
	public string recursos()
    {
        Recursos.Instance.MuchoDinero();
        return "Dando recursos...";
    }
	public string win()
    {
		Wave.Instance.Skip();
        return "Has ganado!";
    }
	public string godmode()
    {
		Recursos.Instance.VidaInfinita();
        return "GOD MODE ACTIVATED";
    }

    public string kill()
    {
        Recursos.Instance.KillBase();
        return "Has muerto!";
    }
}
