using Godot;
using System;

public partial class GameData : Node
{
    public static bool Level1 = false;
    public static bool Level2 = false;
    public static bool Level3 = false;
    public static bool Level4 = false;
    public static bool Level5 = false;
    public static bool Level6 = false;


    public static GameData Instance;

    public override void _Ready()
    {
        Instance = this;
    }

    public void Reset()
    {
        Level1 = false;
        Level2 = false;
        Level3 = false;
        Level4 = false;
        Level5 = false;
        Level6 = false;
    }
}
