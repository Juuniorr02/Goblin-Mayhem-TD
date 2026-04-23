using Godot;
using System.Text.Json;

public partial class Recursos : Node
{
    public static Recursos Instance;

    private const string PATH = "user://recursos.json";

    public int Gold;
    public int Wood;
    public int Stone;
    public int Iron;

    public int BaseGold = 1000;
    public int BaseWood = 1000;
    public int BaseStone = 1000;
    public int BaseIron = 1000;

    public int ProdGold { get; set; }
    public int ProdWood { get; set; }
    public int ProdStone { get; set; }
    public int ProdIron { get; set; }

    public int TotalGold { get; set; }
    public int TotalWood { get; set; }
    public int TotalStone { get; set; }
    public int TotalIron { get; set; }

    private class SaveDataStruct
    {
        public int TotalGold { get; set; }
        public int TotalWood { get; set; }
        public int TotalStone { get; set; }
        public int TotalIron { get; set; }

        public int ProdGold { get; set; }
        public int ProdWood { get; set; }
        public int ProdStone { get; set; }
        public int ProdIron { get; set; }
    }

    public override void _Ready()
    {
        Instance = this;
        LoadData();
    }

    public void StartLevel()
    {
        Gold = BaseGold + ProdGold;
        Wood = BaseWood + ProdWood;
        Stone = BaseStone + ProdStone;
        Iron = BaseIron + ProdIron;
    }

    public void FirstLevel()
    {
        Gold = 10000;
        Wood = 10000;
        Stone = 10000;
        Iron = 10000;
    }

    public void FirstLevelEnd()
    {
        Gold = 0;
        Wood = 0;
        Stone = 0;
        Iron = 0;
    }

    public void NewGame()
    {
        TotalGold = 0;
        TotalWood = 0;
        TotalStone = 0;
        TotalIron = 0;

        ProdGold = 100;
        ProdWood = 50;
        ProdStone = 0;
        ProdIron = 0;

        SaveData();
    }

    public void AddProduction()
    {
        Gold += ProdGold;
        Wood += ProdWood;
        Stone += ProdStone;
        Iron += ProdIron;
    }

    public void EndLevel()
    {
        TotalGold += Gold;
        TotalWood += Wood;
        TotalStone += Stone;
        TotalIron += Iron;

        SaveData();
    }

    public void SaveData()
    {
        var data = new SaveDataStruct
        {
            TotalGold = TotalGold,
            TotalWood = TotalWood,
            TotalStone = TotalStone,
            TotalIron = TotalIron,

            ProdGold = ProdGold,
            ProdWood = ProdWood,
            ProdStone = ProdStone,
            ProdIron = ProdIron
        };

        string json = JsonSerializer.Serialize(data);

        using var file = FileAccess.Open(PATH, FileAccess.ModeFlags.Write);
        file.StoreString(json);
    }

    public void LoadData()
	{
    	if (!FileAccess.FileExists(PATH))
    	{
        	SaveData();
       		return;
    	}

    	using var file = FileAccess.Open(PATH, FileAccess.ModeFlags.Read);
    	string json = file.GetAsText();
		
    	if (string.IsNullOrEmpty(json) || json == "{}")
    	{
        	GD.Print("JSON vacío, usando valores por defecto");
        	SaveData();
        	return;
    	}

    	var data = JsonSerializer.Deserialize<SaveDataStruct>(json);

    	if (data == null)
    	{
        	GD.Print("Error al deserializar, usando valores por defecto");
        	SaveData();
        	return;
    	}

    	TotalGold = data.TotalGold;
    	TotalWood = data.TotalWood;
    	TotalStone = data.TotalStone;
    	TotalIron = data.TotalIron;

    	ProdGold = data.ProdGold;
    	ProdWood = data.ProdWood;
    	ProdStone = data.ProdStone;
    	ProdIron = data.ProdIron;
	}
}