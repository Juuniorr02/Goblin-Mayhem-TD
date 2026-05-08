using Godot;

public partial class Uimapa : Control
{
	public Button btnAldea;
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;

		btnAldea = GetNodeOrNull<Button>("%Aldea");

		ConfigurarBoton(btnAldea);

		if (btnAldea != null)
			btnAldea.Pressed += OnAldea;
	}

	private void ConfigurarBoton(Button b)
    {
        if (b == null) return;

        b.ProcessMode = ProcessModeEnum.Always;
        b.MouseFilter = MouseFilterEnum.Stop;
    }

	private void OnAldea()
	{
		if (GameData.Level1 == true)
			btnAldea.Disabled = false;
		else
			btnAldea.Disabled = true;
		GetTree().ChangeSceneToFile("res://scenes/level/aldea/aldea.tscn");
	}
}
