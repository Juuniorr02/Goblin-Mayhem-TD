using Godot;
using System;

public partial class DialogueUI : CanvasLayer
{
    [Export] public RichTextLabel textLabel;
    [Export] public Label nameLabel;
    [Export] public AnimatedSprite2D portrait;

    [Export] public string[] defaultDialogueLines = new string[] {
        "¡Los goblins se acercan! Debemos proteger la aldea.",
        "Selecciona una torre y colócala cerca del camino."
    };

    private string[] _currentDialogueSet;
    private int _currentLine = 0;
    private bool _isWriting = false;
    private Tween _tween;

    public override void _Ready()
    {
        // Configuraciones de escala
        GetTree().Root.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;
        GetTree().Root.ContentScaleAspect = Window.ContentScaleAspectEnum.Keep;

        // Accedemos al Autoload para ver si ya se mostró antes
        var GameData = GetNode<GameData>("/root/GameData");

        if (!GameData.AldeaVisitada && defaultDialogueLines != null && defaultDialogueLines.Length > 0)
        {
            TriggerDialogue(defaultDialogueLines);
            // Marcamos como visto para que no se repita
            GameData.AldeaVisitada = true; 
        }
        else
        {
            Visible = false;
        }
    }

    public void TriggerDialogue(string[] lines)
    {
        _currentDialogueSet = lines;
        _currentLine = 0;
        Visible = true;
        ShowCurrentLine();
    }

    public override void _Input(InputEvent @event)
    {
        if (!Visible) return;

        bool pressedClick = @event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left;
        // También incluimos la acción "ui_accept" (Enter/Espacio) por comodidad
        bool pressedKey = @event.IsActionPressed("ui_accept");

        if (pressedClick || pressedKey)
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
        if (textLabel != null && _currentLine < _currentDialogueSet.Length)
        {
            _isWriting = true;
            textLabel.Text = _currentDialogueSet[_currentLine];
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
        if (_currentLine < _currentDialogueSet.Length)
        {
            ShowCurrentLine();
        }
        else
        {
            Visible = false;
        }
    }
}
