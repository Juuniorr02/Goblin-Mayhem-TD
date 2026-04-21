using Godot;
using System.Text.Json;

public partial class Recursos : Node
{
    private const string PATH = "user://recursos.json";

    public int Gold;
    public int Wood;
    public int Stone;
    public int Iron;

    public int BaseGold = 100;
    public int BaseWood = 100;
    public int BaseStone = 50;
    public int BaseIron = 20;

    public int ProdGold = 10;
	public int ProdWood = 5;
    public int ProdStone = 3;
    public int ProdIron = 2;
    
	public int TotalGold = 0;
	public int TotalWood = 0;
    public int TotalStone = 0;
    public int TotalIron = 0;

    public override void _Ready()
    {
        LoadData();
    }

    public void StartLevel()
    {
        Gold = BaseGold;

        Wood = BaseWood + ProdWood;
        Stone = BaseStone + ProdStone;
        Iron = BaseIron + ProdIron;

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

	public bool UpgradeGoldProduction(int cost, int amount)
	{
		if (TotalGold < cost) return false;

		TotalGold -= cost;
		ProdGold += amount;

		SaveData();
		return true;
	}

    public bool UpgradeWoodProduction(int cost, int amount)
    {
        if (TotalWood < cost) return false;

        TotalWood -= cost;
        ProdWood += amount;

        SaveData();
        return true;
    }

    public bool UpgradeStoneProduction(int cost, int amount)
    {
        if (TotalStone < cost) return false;

        TotalStone -= cost;
        ProdStone += amount;

        SaveData();
        return true;
    }

    public bool UpgradeIronProduction(int cost, int amount)
    {
        if (TotalIron < cost) return false;

        TotalIron -= cost;
        ProdIron += amount;

        SaveData();
        return true;
    }

    private class SaveDataStruct
    {
        public int TotalGold;
        public int TotalWood;
        public int TotalStone;
        public int TotalIron;

        public int ProdGold;
        public int ProdWood;
        public int ProdStone;
        public int ProdIron;
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
            SaveData(); // crea archivo inicial
            return;
        }

        using var file = FileAccess.Open(PATH, FileAccess.ModeFlags.Read);
        string json = file.GetAsText();

        var data = JsonSerializer.Deserialize<SaveDataStruct>(json);

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