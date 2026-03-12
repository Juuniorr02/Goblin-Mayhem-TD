using Godot;
using System;

public partial class Iu : Control
{
    private TextureButton waveButton;
    private Label waveLabel;
    private AnimatedSprite2D waveAnimation;
    private Timer cooldownTimer;

    [Export] private EnemySpawner spawner;

    private int round = 0;

    public override void _Ready()
    {
        waveButton = GetNode<TextureButton>("IU/IU/IU/Top/Wave/Wave/Wave/TextureButton");
        waveLabel = GetNode<Label>("IU/IU/IU/Top/Wave/Wave/Wave/TextureButton/Label");
        waveAnimation = GetNode<AnimatedSprite2D>("IU/IU/IU/Top/Wave/Wave/Wave/TextureButton/AnimatedSprite2D");

        // Crear timer para el cooldown
        cooldownTimer = new Timer();
        cooldownTimer.WaitTime = 80;
        cooldownTimer.OneShot = true;
        AddChild(cooldownTimer);

        cooldownTimer.Timeout += OnCooldownFinished;

        waveButton.Pressed += OnWaveButtonPressed;
    }

    private void OnWaveButtonPressed()
    {
        // sumar ronda
        round++;
        waveLabel.Text = round.ToString();

        // reiniciar animación aunque esté en curso
        waveAnimation.Stop();
        waveAnimation.Frame = 0;
        waveAnimation.Play("wave");

        // ejecutar spawn
        spawner?.StartWave();

        // bloquear botón
        waveButton.Disabled = true;

        // iniciar cooldown
        cooldownTimer.Start();
    }

    private void OnCooldownFinished()
    {
        waveButton.Disabled = false;
    }
}