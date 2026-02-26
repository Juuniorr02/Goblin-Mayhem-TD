using Godot;
using System.Collections.Generic;

public partial class menu_pausa : CanvasLayer
{
	private ColorRect overlay;
	private Button botonVolver;
	private Button botonQuitarPausa;
	private TextureRect textureRect;
	private bool isPaused = false;

	public override void _Ready()
	{
    	ProcessMode = ProcessModeEnum.Always;

    	overlay = GetNodeOrNull<ColorRect>("Overlay");
    	if (overlay != null)
        overlay.MouseFilter = Control.MouseFilterEnum.Ignore;

    	botonVolver = GetNodeOrNull<Button>("volver");
    	botonQuitarPausa = GetNodeOrNull<Button>("quitar_pausa");
    	textureRect = GetNodeOrNull<TextureRect>("TextureRect");

		if (botonVolver != null)
		{
			botonVolver.ProcessMode = ProcessModeEnum.Always;
			botonVolver.MouseFilter = Control.MouseFilterEnum.Stop;
		}

		if (botonQuitarPausa != null)
		{
			botonQuitarPausa.ProcessMode = ProcessModeEnum.Always;
			botonQuitarPausa.MouseFilter = Control.MouseFilterEnum.Stop;
		}

    	// Conectar señales de botones
    	if (botonVolver != null)
			botonVolver.Pressed += () => QuitarPausa();
        	botonVolver.Pressed += () => GetTree().ChangeSceneToFile("res://scenes/ui/Menu.tscn");
			

    	if (botonQuitarPausa != null)
        	botonQuitarPausa.Pressed += () => QuitarPausa();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (Input.IsActionJustPressed("pausa"))
			{
				if (!isPaused)
					Pausar();
				else
					QuitarPausa();
				
				GetTree().Root.SetInputAsHandled();
			}
		}
	}

	private void Pausar()
	{
		isPaused = true;
		GetTree().Paused = true;

		// Mostrar overlay
		if (overlay != null)
			overlay.Visible = true;

		// Mostrar botones
		if (botonVolver != null) botonVolver.Visible = true;
		if (botonQuitarPausa != null) botonQuitarPausa.Visible = true;
		if (textureRect != null) textureRect.Visible = true;
	}

	public void QuitarPausa()
	{
		isPaused = false;
		GetTree().Paused = false;

		// Ocultar overlay
		if (overlay != null)
			overlay.Visible = false;

		// Ocultar botones
		if (botonVolver != null) botonVolver.Visible = false;
		if (botonQuitarPausa != null) botonQuitarPausa.Visible = false;
		if (textureRect != null) textureRect.Visible = false;
	}
}
