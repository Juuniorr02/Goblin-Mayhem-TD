using Godot;
using System;

public partial class MenuController : Node
{
    public override void _Ready()
    {
        // Connect UI buttons by searching the Menu children (handles exported tscn name variations)
        Node menuRoot = GetParent();
        Button newBtn = FindButton(menuRoot, "Nueva partida", "NewGame");
        Button loadBtn = FindButton(menuRoot, "Cargar partida", "LoadGame");
        Button optionsBtn = FindButton(menuRoot, "Opciones", "Options");
        Button quitBtn = FindButton(menuRoot, "Salir", "Quit");

        if (newBtn != null)
            newBtn.Pressed += OnNewGame;
        if (loadBtn != null)
            loadBtn.Pressed += OnLoadGame;
        if (optionsBtn != null)
            optionsBtn.Pressed += OnOptions;
        if (quitBtn != null)
            quitBtn.Pressed += OnQuit;

        // find message dialog (AcceptDialog)
        messageDialog = menuRoot.GetNodeOrNull<AcceptDialog>("MessageDialog");
        if (messageDialog == null)
        {
            // search recursively
            foreach (var c in menuRoot.GetChildren())
            {
                if (c is AcceptDialog ad)
                {
                    messageDialog = ad;
                    break;
                }
            }
        }
        if (messageDialog != null)
            dialogLabel = messageDialog.GetNodeOrNull<Label>("DialogLabel");
    }

    private AcceptDialog messageDialog;
    private Label dialogLabel;

    private Button FindButton(Node root, string textMatch, string nameContains)
    {
        if (root == null) return null;
        foreach (var c in root.GetChildren())
        {
            if (c is Button b)
            {
                if (b.Text == textMatch || b.Name.ToString().Contains(nameContains))
                    return b;
            }
            if (c is Node n)
            {
                var found = FindButton(n, textMatch, nameContains);
                if (found != null) return found;
            }
        }
        return null;
    }

    public void OnNewGame()
    {
        // Load the main level scene (Level 1)
        GetTree().ChangeSceneToFile("res://scenes/level_1.tscn");
    }

    public void OnLoadGame()
    {
        ShowMessage("Cargar partida no implementado todavía.");
    }

    public void OnOptions()
    {
        ShowMessage("Opciones no implementado todavía.");
    }

    public void OnQuit()
    {
        GetTree().Quit();
    }

    private void ShowMessage(string text)
    {
        if (dialogLabel != null)
            dialogLabel.Text = text;
        if (messageDialog != null)
            messageDialog.PopupCentered();
        else
            GD.Print(text);
    }
}
