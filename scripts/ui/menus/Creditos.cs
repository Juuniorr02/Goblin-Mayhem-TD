using Godot;
using System;

public partial class Creditos : Control
{
	public Button fin;
	private bool isPaused = true;

	public override void _Ready()
	{
		fin = GetNode<Button>("%fin");

		ConfigurarBoton(fin);
		
		fin.Pressed += () => Onfin();

        GetTree().Paused = true;
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	private void ConfigurarBoton(Button b)
    {
        if (b != null)
        {
        b.ProcessMode = ProcessModeEnum.Always;
        b.MouseFilter = MouseFilterEnum.Stop;
        }
    }

	private void Onfin()
	{
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://scenes/level/aldea/mapa_mundi.tscn");
	}

}
