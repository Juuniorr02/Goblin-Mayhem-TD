using Godot;

public partial class DialogueUI : CanvasLayer
{
	[Export] public RichTextLabel textLabel;
	[Export] public Label nameLabel;
	[Export] public AnimatedSprite2D portrait;

	// Variables para el diálogo inicial (puedes cambiarlas desde el Inspector)
	[Export] public string InitialName = "Caballero Errante";
	[Export] public string InitialText = "¡Hola! Bienvenido a esta escena.";
	[Export] public string InitialAnim = "default";

	public override void _Ready()
	{
	GD.Print("Iniciando Diálogo...");
	Visible = true; // Forzamos visibilidad del contenedor
	ShowDialogue(InitialName, InitialText, InitialAnim);
	}

	public void ShowDialogue(string characterName, string text, string animation = "default")
	{
		if (nameLabel != null) nameLabel.Text = characterName;
		
		if (textLabel != null)
		{
			textLabel.Text = text;
			textLabel.VisibleRatio = 0; // Ocultamos el texto al inicio
			
			// Creamos el efecto de máquina de escribir
			var tween = CreateTween();
			// Calculamos la duración basada en el largo del texto (ej: 0.05s por letra)
			float duration = text.Length * 0.05f; 
			tween.TweenProperty(textLabel, "visible_ratio", 1.0f, duration);
		}

		// Si tienes el AnimatedSprite2D configurado, reproduce la animación
		if (portrait != null && portrait.SpriteFrames.HasAnimation(animation))
		{
			portrait.Play(animation);
		}
	}
}
