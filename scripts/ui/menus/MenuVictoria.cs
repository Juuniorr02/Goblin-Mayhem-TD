using System;
using Godot;

public partial class MenuVictoria : CanvasLayer
{
    private Button btnSiguiente;
    private Button btnVolver;
    private Button btnReiniciar;
	private int waveActual;

    private String CurrentScene => GetTree().CurrentScene.SceneFilePath;

	private int winningWave;

	[Export]public int WinningWave;

    private bool isPaused = false;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        btnSiguiente = GetNodeOrNull<Button>("PanelContainer/VBoxContainer/botones/siguiente");
        btnVolver = GetNodeOrNull<Button>("PanelContainer/VBoxContainer/botones/volver");
        btnReiniciar = GetNodeOrNull<Button>("PanelContainer/VBoxContainer/botones/reiniciar");

        ConfigurarBoton(btnSiguiente);
        ConfigurarBoton(btnVolver);
        ConfigurarBoton(btnReiniciar);

        if (btnSiguiente != null)
            btnSiguiente.Pressed += OnSiguiente;

        if (btnReiniciar != null)
            btnReiniciar.Pressed += OnReiniciar;

        if (btnVolver != null)
            btnVolver.Pressed += OnGuardarSalir;

        Visible = false;
    }

	public override void _Process(double delta)
    {
        UpdateMenuVictoria();
    }

    private void ConfigurarBoton(Button b)
    {
        if (b == null) return;

        b.ProcessMode = ProcessModeEnum.Always;
        b.MouseFilter = Control.MouseFilterEnum.Stop;
    }

    public void UpdateMenuVictoria()
	{
		waveActual = Wave.Instance.CurrentWave;
		
		if (Wave.Instance.CurrentWave == WinningWave)
		{
            if(CurrentScene == "res://scenes/level/terrain/tutorial.tscn")
            {
                GameData.Level1 = true;
                GameData.Level2 = true;
            }
            else if(CurrentScene == "res://scenes/level/terrain/montana1.tscn")
            {
                GameData.Level3 = true;
            }
            else if(CurrentScene == "res://scenes/level/terrain/pantano1.tscn")
            {
                GameData.Level4 = true;
            }
            else if(CurrentScene == "res://scenes/level/terrain/islas1.tscn")
            {
                GameData.Level5 = true;
            }
            if(CurrentScene == "res://scenes/level/terrain/castilloMalvado.tscn")
            {
                GameData.Level6 = true;
            }
            else
            {
                GD.Print("Vaya pringao esta jugando el primer nivel o el ultimo.");
            }
            SaveSystem.Instance.SaveGame();
			isPaused = true;
        	GetTree().Paused = true;
        	Visible = true;
        	Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}

    private void Pausar()
    {
        Recursos.Instance.EndLevel();
        var save = GetNode<SaveSystem>("/root/SaveSystem");
    	save.SaveGame();
        BuildTime.CanBuild = true;
        isPaused = true;
        GetTree().Paused = true;
        Visible = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    private void QuitarPausa()
    {
        isPaused = false;
        GetTree().Paused = false;
        Visible = false;
    }

    private void OnReiniciar()
	{
        BuildTime.CanBuild = true;
        Recursos.Instance.RepairBase();
        Wave.Instance.ResetWaves();
        QuitarPausa();
    	GetTree().ReloadCurrentScene();
	}

	private void OnGuardarSalir()
	{
        Recursos.Instance.StartLevel();
        Wave.Instance.ResetWaves();
		QuitarPausa();
    	Input.MouseMode = Input.MouseModeEnum.Visible;
    	GetTree().ChangeSceneToFile("res://scenes/level/aldea/mapa_mundi.tscn");
	}

	private void OnSiguiente()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
        QuitarPausa();
        Recursos.Instance.StartLevel();
        Wave.Instance.ResetWaves();
        if(CurrentScene == "res://scenes/level/terrain/castilloMalvado.tscn")
        {
            GetTree().ChangeSceneToFile("res://scenes/ui/menus/creditos.tscn");
        }
    	else if(CurrentScene == "res://scenes/level/terrain/tutorial.tscn")
        {
            GetTree().ChangeSceneToFile("res://scenes/level/aldea/aldea.tscn");
        }
    	else
        {
            GetTree().ChangeSceneToFile("res://scenes/level/aldea/mapa_mundi.tscn");
	    }
    }
}