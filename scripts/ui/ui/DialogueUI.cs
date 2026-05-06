using Godot;
using System;

public partial class DialogueUI : CanvasLayer
{
    [Export] public RichTextLabel textLabel;
    [Export] public Label nameLabel;
    [Export] public AnimatedSprite2D portrait;

    // Ahora puedes escribir las frases iniciales directamente en el Inspector de Godot
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
        // Nota: Si el zoom falla, recuerda borrar estas dos líneas y configurar en Project Settings
        GetTree().Root.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;
        GetTree().Root.ContentScaleAspect = Window.ContentScaleAspectEnum.Keep;

        // Al empezar, cargamos las frases que pusiste en el Export
        if (defaultDialogueLines != null && defaultDialogueLines.Length > 0)
        {
            TriggerDialogue(defaultDialogueLines);
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

    public void TriggerDialogue(string singleLine)
    {
        TriggerDialogue(new string[] { singleLine });
    }

    public override void _Input(InputEvent @event)
    {
        if (!Visible) return;

        // Detecta tanto la tecla "Aceptar" (Espacio/Enter) como el Click Izquierdo del ratón
        bool pressedClick = @event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left;

        if (pressedClick)
        {
            if (_isWriting)
            {
                // Salta la animación y muestra todo el texto de golpe
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
