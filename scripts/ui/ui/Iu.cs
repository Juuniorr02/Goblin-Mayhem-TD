using Godot;
using System;

public partial class Iu : Control
{
    private TextureButton waveButton;
    private Label waveLabel;
    private Label goldLabel;
    private Label healthLabel;
    private Label ironLabel;
    private Label woodLabel;
    private Label stoneLabel;

    public Button Archer;
    public Button Cannon;
    public Button Mortar;
    public Button Flame;
    public Button Ballista;
    public Button Wizard;
    public Button Bloon;
    public Button Nest;
    public Button Ship;
    public Button Atun;
    public Button Borrar;

    private AnimatedSprite2D waveAnimation;
    private Timer cooldownTimer;

    private string scenePath;

    private int contador = 0;

    private int constructionwave = 0;

    [Export] private EnemySpawner spawner;

    public override void _Ready()
    {
        scenePath = GetTree().CurrentScene.SceneFilePath;
        waveButton = GetNode<TextureButton>("%WaveButton");
        waveLabel = GetNode<Label>("%WaveLabel");

        goldLabel = GetNode<Label>("%GoldLabel");
        healthLabel = GetNode<Label>("%HealthLabel");
        ironLabel = GetNode<Label>("%IronLabel");
        woodLabel = GetNode<Label>("%WoodLabel");
        stoneLabel = GetNode<Label>("%StoneLabel");

        Archer = GetNodeOrNull<Button>("%Archer");
        Cannon = GetNodeOrNull<Button>("%Cannon");
        Mortar = GetNodeOrNull<Button>("%Mortar");
        Flame = GetNodeOrNull<Button>("%Flame");
        Ballista = GetNodeOrNull<Button>("%Ballista");
        Wizard = GetNodeOrNull<Button>("%Wizard");
        Bloon = GetNodeOrNull<Button>("%Bloon");
        Nest = GetNodeOrNull<Button>("%Nest");
        Ship = GetNodeOrNull<Button>("%Ship");
        Atun = GetNodeOrNull<Button>("%Atun");
        Borrar = GetNodeOrNull<Button>("%Borrar");

        ConfigurarBoton(Archer);
        ConfigurarBoton(Cannon);
        ConfigurarBoton(Mortar);
        ConfigurarBoton(Flame);
        ConfigurarBoton(Ballista);
        ConfigurarBoton(Wizard);
        ConfigurarBoton(Bloon);
        ConfigurarBoton(Nest);
        ConfigurarBoton(Ship);
        ConfigurarBoton(Atun);
        ConfigurarBoton(Borrar);

        waveAnimation = GetNode<AnimatedSprite2D>("%WaveSprite");

        cooldownTimer = new Timer();
        cooldownTimer.WaitTime = 20;
        cooldownTimer.OneShot = true;
        AddChild(cooldownTimer);

        cooldownTimer.Timeout += OnCooldownFinished;
        waveButton.Pressed += OnWaveButtonPressed;

        UpdateIU();
    }
    
    private void ConfigurarBoton(Button b)
    {
        if (b == null) return;

        b.ProcessMode = ProcessModeEnum.Always;
        b.MouseFilter = MouseFilterEnum.Stop;
    }

    public override void _Process(double delta)
    {
        UpdateIU();
    }

    private void UpdateIU()
    {
        if (Base.Instance != null || Recursos.Instance != null)
        {
            goldLabel.Text = Recursos.Instance.Gold.ToString();
            healthLabel.Text = Base.Instance.Health.ToString();
            ironLabel.Text = Recursos.Instance.Iron.ToString();
            woodLabel.Text = Recursos.Instance.Wood.ToString();
            stoneLabel.Text = Recursos.Instance.Stone.ToString();
        }

        if (Wave.Instance != null)
        {
            waveLabel.Text = Wave.Instance.CurrentWave.ToString();
            if (contador == 0 && scenePath == "res://scenes/level/terrain/level1.tscn")
            {
                Recursos.Instance.FirstLevel();
                contador ++;
            }
            else if (contador == 0 && Wave.Instance.CurrentWave == 0)
            {
                Recursos.Instance.StartLevel();
                contador ++;
            }
        }
    }

    private void OnWaveButtonPressed()
    {
        if(constructionwave == 0)
        {
        BuildTime.CanBuild = false;
        MouseFilter = MouseFilterEnum.Stop;
        if (Wave.Instance == null) return;

        Wave.Instance.StartNextWave();

        waveAnimation.Stop();
        waveAnimation.Frame = 0;
        waveAnimation.Play("wave");

        int targetIndex = Wave.Instance.CurrentWave - 1;
        spawner?.StartWave(targetIndex);

        waveButton.Disabled = true;
        cooldownTimer.Start();
        Recursos.Instance.AddProduction();
        constructionwave++;
        }

        else if (constructionwave == 1)
        {
            BuildTime.CanBuild = true;
            MouseFilter = MouseFilterEnum.Stop;
            constructionwave --;
        }
        
    }

    private void OnCooldownFinished()
    {
        waveButton.Disabled = false;
    }
}
