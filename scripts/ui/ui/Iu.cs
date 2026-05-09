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

    private TextureRect Martillito;

    public TextureButton Archer;
    public TextureButton Cannon;
    public TextureButton Mortar;
    public TextureButton Flame;
    public TextureButton Ballista;
    public TextureButton Wizard;
    public TextureButton Bloon;
    public TextureButton Nest;
    public TextureButton Ship;
    public TextureButton Atun;
    public Button Borrar;

    private TextureRect icon;
    private Label nameLabel;
    private Label costLabel;

    [Export] public TowerData CannonData;
    [Export] public TowerData ArcherData;
    [Export] public TowerData MortarData;
    [Export] public TowerData FlameData;
    [Export] public TowerData BallistaData;
    [Export] public TowerData WizardData;
    [Export] public TowerData BloonData;
    [Export] public TowerData NestData;
    [Export] public TowerData ShipData;
    [Export] public TowerData AtunData;

    private AnimatedSprite2D waveAnimation;
    private Timer cooldownTimer;

    private string scenePath;

    private int contador = 0;

    private int constructionwave = 0;

    private MenuVictoria menuVictoria;

    [Export] private EnemySpawner spawner;

    public override void _Ready()
    {
        scenePath = GetTree().CurrentScene.SceneFilePath;
        waveButton = GetNode<TextureButton>("%WaveButton");
        waveLabel = GetNode<Label>("%WaveLabel");

        Martillito = GetNode<TextureRect>("%Martillito");

        goldLabel = GetNode<Label>("%GoldLabel");
        healthLabel = GetNode<Label>("%HealthLabel");
        ironLabel = GetNode<Label>("%IronLabel");
        woodLabel = GetNode<Label>("%WoodLabel");
        stoneLabel = GetNode<Label>("%StoneLabel");

        Archer = GetNodeOrNull<TextureButton>("%Archer");
        Cannon = GetNodeOrNull<TextureButton>("%Cannon");
        Mortar = GetNodeOrNull<TextureButton>("%Mortar");
        Flame = GetNodeOrNull<TextureButton>("%Flame");
        Ballista = GetNodeOrNull<TextureButton>("%Ballista");
        Wizard = GetNodeOrNull<TextureButton>("%Wizard");
        Bloon = GetNodeOrNull<TextureButton>("%Bloon");
        Nest = GetNodeOrNull<TextureButton>("%Nest");
        Ship = GetNodeOrNull<TextureButton>("%Ship");
        Atun = GetNodeOrNull<TextureButton>("%Atun");
        Borrar = GetNodeOrNull<Button>("%Borrar");

        icon = GetNode<TextureRect>("%Icon");
        nameLabel = GetNode<Label>("%Name");
        costLabel = GetNode<Label>("%Cost");

        menuVictoria = GetTree().GetFirstNodeInGroup("victory") as MenuVictoria;

        ConfigurarTextureBoton(Archer);
        ConfigurarTextureBoton(Cannon);
        ConfigurarTextureBoton(Mortar);
        ConfigurarTextureBoton(Flame);
        ConfigurarTextureBoton(Ballista);
        ConfigurarTextureBoton(Wizard);
        ConfigurarTextureBoton(Bloon);
        ConfigurarTextureBoton(Nest);
        ConfigurarTextureBoton(Ship);
        ConfigurarTextureBoton(Atun);
        ConfigurarBoton(Borrar);

        waveAnimation = GetNode<AnimatedSprite2D>("%WaveSprite");

        cooldownTimer = new Timer();
        cooldownTimer.WaitTime = 20;
        cooldownTimer.OneShot = true;
        AddChild(cooldownTimer);

        cooldownTimer.Timeout += OnCooldownFinished;
        waveButton.Pressed += OnWaveButtonPressed;

            Cannon.Pressed += () => OnTowerPressed(CannonData);
            Archer.Pressed += () => OnTowerPressed(ArcherData);
            Mortar.Pressed += () => OnTowerPressed(MortarData);
            Flame.Pressed += () => OnTowerPressed(FlameData);
            Ballista.Pressed += () => OnTowerPressed(BallistaData);
            Wizard.Pressed += () => OnTowerPressed(WizardData);
            Bloon.Pressed += () => OnTowerPressed(BloonData);
            Nest.Pressed += () => OnTowerPressed(NestData);
            Ship.Pressed += () => OnTowerPressed(ShipData);
            Atun.Pressed += () => OnTowerPressed(AtunData);

        UpdateIU();
    }
    
    private void ConfigurarTextureBoton(TextureButton b)
    {
        if (b == null) return;

        b.ProcessMode = ProcessModeEnum.Always;
        b.MouseFilter = MouseFilterEnum.Stop;
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
        if (Recursos.Instance != null)
        {
            goldLabel.Text = Recursos.Instance.Gold.ToString();
            healthLabel.Text = Recursos.Instance.Vida.ToString();
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
        waveLabel.Visible = true;
        Martillito.Visible = false;
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
            waveLabel.Visible = false;
            Martillito.Visible = true;
            BuildTime.CanBuild = true;
            MouseFilter = MouseFilterEnum.Stop;
            constructionwave --;
        }
        
    }

    private void OnCooldownFinished()
    {
        if (Wave.Instance == null || menuVictoria == null)
            return;

        waveButton.Disabled = Wave.Instance.CurrentWave >= menuVictoria.WinningWave;
    }

    private void ShowTowerInfo(TowerData data)
    {
        if (data == null) return;

        icon.Texture = data.Icon;
        nameLabel.Text = data.Name;
        costLabel.Text = data.Cost.ToString();
    }

    private void OnTowerPressed(TowerData data)
    {
        GD.Print("CLICK TOWER: " + data);
        ShowTowerInfo(data);
    }
}