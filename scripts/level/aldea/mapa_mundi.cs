using Godot;
using System;

public partial class mapa_mundi : Node2D
{
	private entrar_nivel entrar_nivel;

	private Button btnTutorial;

	private bool menuAbierto = false;
	
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;

		entrar_nivel = GetNodeOrNull<entrar_nivel>("entrar_nivel");

		entrar_nivel.mapa_mundi = this;

		btnTutorial = GetNodeOrNull<Button>("Botones/BotonTutorial");

		ConfigurarBoton(btnTutorial);

		if (btnTutorial != null)
			btnTutorial.Pressed += OnTutorial;
	}

	private void ConfigurarBoton(Button b)
    {
        if (b == null) return;

        b.ProcessMode = ProcessModeEnum.Pausable;
        b.MouseFilter = Control.MouseFilterEnum.Stop;
    }

	private void OnTutorial()
	{
		GetTree().ChangeSceneToFile("res://scenes/level/terrain/tutorial.tscn");
	}

	private void AbrirMenu()
	{
		menuAbierto = true;
	}

	private void CerrarMenu()
	{
		menuAbierto = false;
	}
}
