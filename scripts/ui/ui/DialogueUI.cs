using Godot;
using System;

public partial class DialogueUI : CanvasLayer
{
    [Export] public RichTextLabel textLabel;
    [Export] public Label nameLabel;
    [Export] public AnimatedSprite2D portrait;

    private string[] _currentDialogueSet; // Almacena las frases actuales
    private int _currentLine = 0;
    private bool _isWriting = false;
    private Tween _tween;

    public override void _Ready()
    {
        GetTree().Root.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;
        GetTree().Root.ContentScaleAspect = Window.ContentScaleAspectEnum.Keep;
        
        Visible = false; // Empieza oculto hasta que alguien lo llame
    }

    // MÉTODO CLAVE: Llama a esto desde otra clase
    public void TriggerDialogue(string[] lines)
    {
        _currentDialogueSet = lines;
        _currentLine = 0;
        Visible = true;
        ShowCurrentLine();
    }

    // Sobrecarga por si solo quieres pasar una frase suelta
    public void TriggerDialogue(string singleLine)
    {
        TriggerDialogue(new string[] { singleLine });
    }

    public override void _Input(InputEvent @event)
    {
        if (!Visible) return;

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
