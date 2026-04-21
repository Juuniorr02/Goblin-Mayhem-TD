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

    private AnimatedSprite2D waveAnimation;
    private Timer cooldownTimer;

    [Export] private EnemySpawner spawner;

    public override void _Ready()
    {
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

        waveAnimation = GetNode<AnimatedSprite2D>("%WaveSprite");

        cooldownTimer = new Timer();
        cooldownTimer.WaitTime = 80; // Tiempo de espera entre oleadas
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
        // Asumiendo que Base.Instance y Wave.Instance existen y funcionan
        if (Base.Instance != null)
        {
            goldLabel.Text = Base.Instance.Gold.ToString();
            healthLabel.Text = Base.Instance.Health.ToString();
            ironLabel.Text = Base.Instance.Iron.ToString();
            woodLabel.Text = Base.Instance.Wood.ToString();
            stoneLabel.Text = Base.Instance.Stone.ToString();
        }

        if (Wave.Instance != null)
        {
            waveLabel.Text = Wave.Instance.CurrentWave.ToString();
        }
    }

    private void OnWaveButtonPressed()
    {
        MouseFilter = Control.MouseFilterEnum.Stop;
        if (Wave.Instance == null) return;

        // 1. Iniciamos la lógica de la siguiente oleada
        Wave.Instance.StartNextWave();

        // 2. Animación
        waveAnimation.Stop();
        waveAnimation.Frame = 0;
        waveAnimation.Play("wave");

        // 3. Sincronizamos con el Spawner: 
        // Si CurrentWave es 1, pasamos el índice 0 al spawner.
        int targetIndex = Wave.Instance.CurrentWave - 1;
        spawner?.StartWave(targetIndex);

        // 4. Bloqueamos botón
        waveButton.Disabled = true;
        cooldownTimer.Start();
    }

    private void OnCooldownFinished()
    {
        waveButton.Disabled = false;
    }
}
