	using Godot;
using System;

public partial class DialogueUI : CanvasLayer
{
    [Export] public RichTextLabel textLabel;
    [Export] public Label nameLabel;
    [Export] public AnimatedSprite2D portrait;

    [Export] public string[] dialogueLines = new string[] {
        "¡Los goblins se acercan! Debemos proteger la aldea.",
        "Necesitamos defensas. Mira ese camino, por ahí intentarán cruzar.",
        "Selecciona una torre y colócala cerca del camino.",
        "¡Aquí vienen! ¡No dejes que pasen!"
    };

    private int _currentLine = 0;
    private bool _isWriting = false;
    private Tween _tween;

    public override void _Ready()
    {
        // Esto mantiene la UI proporcionada en pantalla completa
        GetTree().Root.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;
        GetTree().Root.ContentScaleAspect = Window.ContentScaleAspectEnum.Keep;

        Visible = true;
        StartDialogue();
    }

    public void StartDialogue()
    {
        _currentLine = 0;
        ShowCurrentLine();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept"))
        {
            if (_isWriting)
            {
                _tween?.Kill();
                if (textLabel != null) textLabel.VisibleRatio = 1.0f;
                _isWriting = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    private void ShowCurrentLine()
    {
        if (textLabel != null && _currentLine < dialogueLines.Length)
        {
            _isWriting = true;
            textLabel.Text = dialogueLines[_currentLine];
            textLabel.VisibleRatio = 0;

            _tween = CreateTween();
            float duration = textLabel.Text.Length * 0.04f;
            _tween.TweenProperty(textLabel, "visible_ratio", 1.0f, duration);
            _tween.Finished += () => _isWriting = false;
        }
    }

    private void NextLine()
    {
        _currentLine++;
        if (_currentLine < dialogueLines.Length)
        {
            ShowCurrentLine();
        }
        else
        {
            Visible = false; // Cierra el diálogo al terminar
        }
    }
}
