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

		if(mapa_mundi.nombreNivel == "Montana1")
		{
			Nombre.Text = "Montaña 1";
		}

		if(mapa_mundi.nombreNivel == "Pantano1")
		{
			Nombre.Text = "Pantano 1";
		}

		if(mapa_mundi.nombreNivel == "Pantano2")
		{
			Nombre.Text = " No implementado ";
		}

		if(mapa_mundi.nombreNivel == "Islas1")
		{
			Nombre.Text = "Islas 1";
		}

		if(mapa_mundi.nombreNivel == "Castillo Malvado")
		{
			Nombre.Text = "Castillo Malvado";
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
		if(mapa_mundi.nombreNivel == "Tutorial")
		{
			Recursos.Instance.StartLevel();
			GetTree().ChangeSceneToFile("res://scenes/level/terrain/tutorial.tscn");
		}

		if(mapa_mundi.nombreNivel == "Montana1")
		{
			Recursos.Instance.StartLevel();
			GetTree().ChangeSceneToFile("res://scenes/level/terrain/montana1.tscn");
		}

		if(mapa_mundi.nombreNivel == "Pantano1")
		{
			Recursos.Instance.StartLevel();
			GetTree().ChangeSceneToFile("res://scenes/level/terrain/pantano1.tscn");
		}

		if(mapa_mundi.nombreNivel == "Pantano2")
		{
			btnEntrar.Disabled = true;
		}

		if(mapa_mundi.nombreNivel == "Islas1")
		{
			Recursos.Instance.StartLevel();
			GetTree().ChangeSceneToFile("res://scenes/level/terrain/islas1.tscn");
		}

		if(mapa_mundi.nombreNivel == "Castillo Malvado")
		{
			Recursos.Instance.StartLevel();
			GetTree().ChangeSceneToFile("res://scenes/level/terrain/castillo_malvado.tscn");
		}
	}

	private void OnVolver()
	{
		btnEntrar.Disabled = false;
		isPaused = false;
        GetTree().Paused = false;
		mapa_mundi.CerrarMenu();
        Visible = false;
	}
}
