using Godot;
using System;

public partial class entrar_nivel : CanvasLayer
{
	public mapa_mundi mapa_mundi;

	private Label Nombre;
	private Button btnEntrar;
	private Button btnVolver;

	private bool isPaused = false;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;

		Nombre = GetNodeOrNull<Label>("%Nombre");
		btnEntrar = GetNodeOrNull<Button>("%Entrar");
		btnVolver = GetNodeOrNull<Button>("%Volver");

		ConfigurarBoton(btnEntrar);
		ConfigurarBoton(btnVolver);

		if (btnEntrar != null)
			btnEntrar.Pressed += OnEntrar;

		if (btnVolver != null)
			btnVolver.Pressed += OnVolver;

		Visible = false;
	}

	private void ConfigurarBoton(Button b)
    {
        if (b == null) return;

        b.ProcessMode = ProcessModeEnum.Always;
        b.MouseFilter = Control.MouseFilterEnum.Stop;
    }

	public override void _Input(InputEvent e)
    {
        if (e.IsActionPressed("pausa"))
        {
            if (isPaused) OnVolver();
        }
    }

	public void Abrir()
	{
		if(mapa_mundi.nombreNivel == "Tutorial")
		{
			Nombre.Text = "Tutorial";
		}
		isPaused = true;
        GetTree().Paused = true;
        Visible = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
	}
	private void OnEntrar()
	{
		isPaused = false;
        GetTree().Paused = false;
		mapa_mundi.CerrarMenu();
        Visible = false;
		GetTree().ChangeSceneToFile("res://scenes/level/terrain/tutorial.tscn");
	}

	private void OnVolver()
	{
		isPaused = false;
        GetTree().Paused = false;
		mapa_mundi.CerrarMenu();
        Visible = false;
	}
}
