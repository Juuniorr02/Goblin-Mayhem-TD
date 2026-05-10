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
    private string scenePath;
    private int contador = 0, constructionwave = 0;

    [Export] private EnemySpawner spawner;
    [Export] private MenuVictoria menuVictoria;

    // =========================
    // COOLDOWN SYSTEM (NUEVO)
    // =========================
    private float waveCooldown = 0f;
    private bool waitingCooldown = false;
    private bool waitingAfterPress = false;

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

        var towersNode = GetTree().CurrentScene.FindChild("Towers", true, false);
        if (towersNode != null)
        {
            towersNode.Connect("OnCancelSelection", Callable.From(() =>
            {
                if (infoPanel != null)
                    infoPanel.Visible = false;
            }));
        }

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

        if (infoPanel != null)
            infoPanel.Visible = false;

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

        if (Borrar != null)
            Borrar.Pressed += () => { if (infoPanel != null) infoPanel.Visible = false; };

        waveButton.Pressed += OnWaveButtonPressed;

        UpdateIU();
    }

    public override void _Process(double delta)
    {
        UpdateIU();
        UpdateWaveCooldown((float)delta);
        UpdateWaveButtonState();
    }

    // =========================
    // UI UPDATE
    // =========================
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
                contador++;
            }
            else if (contador == 0 && Wave.Instance.CurrentWave == 0)
            {
                Recursos.Instance.StartLevel();
                contador++;
            }
        }
    }

    // =========================
    // ENEMIGOS
    // =========================
    private bool NoEnemiesAlive()
    {
        return GetTree().GetNodesInGroup("enemies").Count == 0;
    }

    // =========================
    // COOLDOWN LOGIC
    // =========================
    private void UpdateWaveCooldown(float delta)
    {
        // si hay enemigos → reset total
        if (!NoEnemiesAlive())
        {
            waveCooldown = 0f;
            waitingCooldown = false;
            waitingAfterPress = false;
            return;
        }

        // cooldown después de pulsar
        if (waitingAfterPress)
        {
            waveCooldown += delta;

            if (waveCooldown >= 5f)
            {
                waitingAfterPress = false;
                waveCooldown = 0f;

                waveAnimation.Play("wave");
            }

            return;
        }

        // espera inicial cuando ya no hay enemigos
        if (!waitingCooldown)
        {
            waitingCooldown = true;
            waveCooldown = 0f;

            waveAnimation.Play("wave");
        }

        if (waitingCooldown)
        {
            waveCooldown += delta;
        }
    }

    private void UpdateWaveButtonState()
    {
        if (waveButton == null) return;

        bool canPress =
            NoEnemiesAlive() &&
            waitingCooldown &&
            waveCooldown >= 5f &&
            !waitingAfterPress;

        waveButton.Disabled = !canPress;
    }

    // =========================
    // WAVE BUTTON
    // =========================
    private void OnWaveButtonPressed()
    {
        if (waveButton.Disabled)
            return;

        // 🔥 activa cooldown después de pulsar
        waitingAfterPress = true;
        waveCooldown = 0f;

        if (constructionwave == 0)
        {
            if (Wave.Instance == null) return;

            waveLabel.Visible = true;
            Martillito.Visible = false;
            BuildTime.CanBuild = false;

            Wave.Instance.StartNextWave();

            spawner?.StartWave(Wave.Instance.CurrentWave - 1);

            Recursos.Instance.AddProduction();
            constructionwave++;
        }
        else
        {
            waveLabel.Visible = false;
            Martillito.Visible = true;
            BuildTime.CanBuild = true;
            constructionwave--;
        }
    }

    // =========================
    // TOWER UI
    // =========================
    private void ShowTowerInfo(TowerData data)
    {
        if (data == null)
        {
            if (infoPanel != null)
                infoPanel.Visible = false;
            return;
        }

        if (infoPanel != null)
            infoPanel.Visible = true;

        icon.Texture = data.Icon;
        nameLabel.Text = data.Name;
        costLabel.Text = data.Cost.ToString();
    }

    private void OnTowerPressed(TowerData data)
    {
        ShowTowerInfo(data);
    }

    // =========================
    // HELPERS
    // =========================
    private void ConfigurarTextureBoton(TextureButton b)
    {
        if (b != null)
        {
            b.ProcessMode = ProcessModeEnum.Always;
            b.MouseFilter = Control.MouseFilterEnum.Stop;
        }
    }

    private void ConfigurarBoton(Button b)
    {
        if (b != null)
        {
            b.ProcessMode = ProcessModeEnum.Always;
            b.MouseFilter = Control.MouseFilterEnum.Stop;
        }
    }
}