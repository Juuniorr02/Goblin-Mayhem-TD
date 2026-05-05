using Godot;
using System;

public partial class mapa_mundi : Node2D
{
	public String nombreNivel;

	private entrar_nivel entrar_nivel;

	private Button btnTutorial;

	private bool menuAbierto = false;
	
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;

		entrar_nivel = GetNodeOrNull<entrar_nivel>("entrar_nivel");

		entrar_nivel.mapa_mundi = this;

		btnTutorial = GetNodeOrNull<Button>("Botones/Tutorial");

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

	public void OnTutorial()
	{
		if (menuAbierto) return;
		nombreNivel = "Tutorial";
        entrar_nivel?.Abrir();

        menuAbierto = true;
	}

	public void CerrarMenu()
	{
		menuAbierto = false;
	}
}
