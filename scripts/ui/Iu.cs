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

    private AnimatedSprite2D waveAnimation;
    private Timer cooldownTimer;

    [Export] private EnemySpawner spawner;

    public override void _Ready()
    {
        waveButton = GetNode<TextureButton>("IU/IU/IU/Top/Wave/Wave/Wave/Wave");
        waveLabel = GetNode<Label>("IU/IU/IU/Top/Wave/Wave/Wave/Wave/WaveLabel");

        goldLabel = GetNode<Label>("%GoldLabel");
        healthLabel = GetNode<Label>("%HealthLabel");
        ironLabel = GetNode<Label>("%IronLabel");
        woodLabel = GetNode<Label>("%WoodLabel");
        stoneLabel = GetNode<Label>("%StoneLabel");

        waveAnimation = GetNode<AnimatedSprite2D>("IU/IU/IU/Top/Wave/Wave/Wave/Wave/WaveSprite");

        cooldownTimer = new Timer();
        cooldownTimer.WaitTime = 80;
        cooldownTimer.OneShot = true;
        AddChild(cooldownTimer);

        cooldownTimer.Timeout += OnCooldownFinished;
        waveButton.Pressed += OnWaveButtonPressed;

        UpdateIU();
    }

    public override void _Process(double delta)
    {
        UpdateIU();
    }

    private void UpdateIU()
    {
        goldLabel.Text = Base.Instance.Gold.ToString();
        healthLabel.Text = Base.Instance.Health.ToString();
        ironLabel.Text = Base.Instance.Iron.ToString();
        woodLabel.Text = Base.Instance.Wood.ToString();
        stoneLabel.Text = Base.Instance.Stone.ToString();

        waveLabel.Text = Wave.Instance.CurrentWave.ToString();
    }

    private void OnWaveButtonPressed()
    {
        Wave.Instance.StartNextWave();

        waveAnimation.Stop();
        waveAnimation.Frame = 0;
        waveAnimation.Play("wave");

        spawner?.StartWave();

        waveButton.Disabled = true;
        cooldownTimer.Start();
    }

    private void OnCooldownFinished()
    {
        waveButton.Disabled = false;
    }
}