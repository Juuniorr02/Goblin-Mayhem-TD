using Godot;
using System;

public partial class Iu : Control
{
    private TextureButton waveButton;
    private Label waveLabel;
    private Label goldLabel, healthLabel, ironLabel, woodLabel, stoneLabel;
    private PanelContainer infoPanel; 
    private TextureRect Martillito;

    public TextureButton Archer, Cannon, Mortar, Flame, Ballista, Wizard, Bloon, Nest, Ship, Atun;
    public Button Borrar;

    private TextureRect icon;
    private Label nameLabel, costLabel;

    [Export] public TowerData CannonData, ArcherData, MortarData, FlameData, BallistaData, WizardData, BloonData, NestData, ShipData, AtunData;

    private AnimatedSprite2D waveAnimation;
    private Timer cooldownTimer;
    private string scenePath;
    private int contador = 0, constructionwave = 0;
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

        infoPanel = GetNodeOrNull<PanelContainer>("%InfoPanel");

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

        // CONEXIÓN CON EL NODO TOWERS
        var towersNode = GetTree().CurrentScene.FindChild("Towers", true, false);
        if (towersNode != null)
        {
            towersNode.Connect("OnCancelSelection", Callable.From(() => {
                if (infoPanel != null) infoPanel.Visible = false;
            }));
        }

        ConfigurarTextureBoton(Archer); ConfigurarTextureBoton(Cannon); ConfigurarTextureBoton(Mortar);
        ConfigurarTextureBoton(Flame); ConfigurarTextureBoton(Ballista); ConfigurarTextureBoton(Wizard);
        ConfigurarTextureBoton(Bloon); ConfigurarTextureBoton(Nest); ConfigurarTextureBoton(Ship);
        ConfigurarTextureBoton(Atun); ConfigurarBoton(Borrar);

        waveAnimation = GetNode<AnimatedSprite2D>("%WaveSprite");
        if (infoPanel != null) infoPanel.Visible = false;

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

        if (Borrar != null) Borrar.Pressed += () => { if (infoPanel != null) infoPanel.Visible = false; };

        UpdateIU();
    }

    private void ConfigurarTextureBoton(TextureButton b) { if (b != null) { b.ProcessMode = ProcessModeEnum.Always; b.MouseFilter = MouseFilterEnum.Stop; } }
    private void ConfigurarBoton(Button b) { if (b != null) { b.ProcessMode = ProcessModeEnum.Always; b.MouseFilter = MouseFilterEnum.Stop; } }

    public override void _Process(double delta) { UpdateIU(); }

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
            
            // --- RECUPERADA LÓGICA DE INICIO ---
            if (contador == 0 && scenePath == "res://scenes/level/terrain/level1.tscn")
            {
                Recursos.Instance.FirstLevel();
                contador++;
            }
            else if (contador == 0 && Wave.Instance.CurrentWave == 0)
            {
                Recursos.Instance.StartLevel();
                contador++;
            }
        }
    }

    private void OnWaveButtonPressed()
    {
        if(constructionwave == 0)
        {
            waveLabel.Visible = true; Martillito.Visible = false; BuildTime.CanBuild = false;
            if (Wave.Instance == null) return;
            Wave.Instance.StartNextWave();
            waveAnimation.Play("wave");
            spawner?.StartWave(Wave.Instance.CurrentWave - 1);
            waveButton.Disabled = true; cooldownTimer.Start();
            Recursos.Instance.AddProduction(); constructionwave++;
        }
        else
        {   
            waveLabel.Visible = false; Martillito.Visible = true; BuildTime.CanBuild = true;
            constructionwave --;
        }
    }

    private void OnCooldownFinished() { if (Wave.Instance != null && menuVictoria != null) waveButton.Disabled = Wave.Instance.CurrentWave >= menuVictoria.WinningWave; }

    private void ShowTowerInfo(TowerData data)
    {
        if (data == null) { if (infoPanel != null) infoPanel.Visible = false; return; }
        if (infoPanel != null) infoPanel.Visible = true;
        icon.Texture = data.Icon; nameLabel.Text = data.Name; costLabel.Text = data.Cost.ToString();
    }

    private void OnTowerPressed(TowerData data) { ShowTowerInfo(data); }
}
