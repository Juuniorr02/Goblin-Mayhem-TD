using System;
using Godot;

public partial class MenuVictoria : CanvasLayer
{
    private Button btnSiguiente;
    private Button btnVolver;
    private Button btnReiniciar;

    private string CurrentScene => GetTree().CurrentScene.SceneFilePath;

    [Export] public int WinningWave;

    private bool isPaused = false;

    // 🔥 estado de victoria
    private bool checkingVictory = false;
    private float victoryTimer = 0f;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        AddToGroup("victory");

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
        UpdateVictoryLogic((float)delta);
    }

    // 🔥 lógica estable sin async
    private void UpdateVictoryLogic(float delta)
    {
        if (Wave.Instance == null)
            return;

        // 🔥 ya estamos esperando confirmación
        if (checkingVictory)
        {
            victoryTimer += delta;

            if (victoryTimer >= 5f)
            {
                if (Wave.Instance.CurrentWave >= WinningWave && NoEnemiesAlive() && Recursos.Instance.Vida >= 1)
                {
                    HandleVictory();
                }

                checkingVictory = false;
                victoryTimer = 0f;
            }

            return;
        }

        if (Wave.Instance.CurrentWave >= WinningWave && NoEnemiesAlive() && Recursos.Instance.Vida >= 1)
        {
            checkingVictory = true;
            victoryTimer = 0f;
        }
    }

    private void HandleVictory()
    {
        if (CurrentScene == "res://scenes/level/terrain/tutorial.tscn")
        {
            GameData.Level1 = true;
            GameData.Level2 = true;
        }
        else if (CurrentScene == "res://scenes/level/terrain/montana1.tscn")
        {
            GameData.Level3 = true;
        }
        else if (CurrentScene == "res://scenes/level/terrain/pantano1.tscn")
        {
            GameData.Level4 = true;
        }
        else if (CurrentScene == "res://scenes/level/terrain/islas1.tscn")
        {
            GameData.Level5 = true;
        }
        else if (CurrentScene == "res://scenes/level/terrain/castilloMalvado.tscn")
        {
            GameData.Level6 = true;
        }
        else
        {
            return;
        }

        Pausar();
    }

    private bool NoEnemiesAlive()
    {
        return GetTree().GetNodesInGroup("enemies").Count == 0;
    }

    private void Pausar()
    {
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

    private void ConfigurarBoton(Button b)
    {
        if (b == null) return;

        b.ProcessMode = ProcessModeEnum.Always;
        b.MouseFilter = Control.MouseFilterEnum.Stop;
    }

    private void OnReiniciar()
    {
        Recursos.Instance.EndLevel();

        BuildTime.CanBuild = true;
        Recursos.Instance.RepairBase();
        Wave.Instance.ResetWaves();

        var save = GetNode<SaveSystem>("/root/SaveSystem");
        save.SaveGame();

        QuitarPausa();
        GetTree().ReloadCurrentScene();
    }

    private void OnGuardarSalir()
    {
        Recursos.Instance.EndLevel();

        var save = GetNode<SaveSystem>("/root/SaveSystem");
        save.SaveGame();

        Recursos.Instance.StartLevel();
        Wave.Instance.ResetWaves();

        QuitarPausa();
        Input.MouseMode = Input.MouseModeEnum.Visible;

        GetTree().ChangeSceneToFile("res://scenes/level/aldea/mapa_mundi.tscn");
    }

    private void OnSiguiente()
    {
        Recursos.Instance.EndLevel();

        var save = GetNode<SaveSystem>("/root/SaveSystem");
        save.SaveGame();

        Input.MouseMode = Input.MouseModeEnum.Visible;

        QuitarPausa();
        Recursos.Instance.StartLevel();
        Wave.Instance.ResetWaves();

        if (CurrentScene == "res://scenes/level/terrain/castilloMalvado.tscn")
        {
            GetTree().ChangeSceneToFile("res://scenes/ui/menus/creditos.tscn");
        }
        else if (CurrentScene == "res://scenes/level/terrain/tutorial.tscn")
        {
            GetTree().ChangeSceneToFile("res://scenes/level/aldea/aldea.tscn");
        }
        else
        {
            GetTree().ChangeSceneToFile("res://scenes/level/aldea/mapa_mundi.tscn");
        }
    }
}