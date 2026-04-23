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
            if (Wave.Instance.CurrentWave == 0)
            {
                Recursos.Instance.StartLevel();
            }
        }
    }

    private void OnWaveButtonPressed()
    {
        MouseFilter = MouseFilterEnum.Stop;
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
        Recursos.Instance.AddProduction();
    }

    private void OnCooldownFinished()
    {
        waveButton.Disabled = false;
    }
}
