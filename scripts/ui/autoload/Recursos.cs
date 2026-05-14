using Godot;
using System.Text.Json;

public partial class Recursos : Node
{
    public static Recursos Instance;

    private const string PATH = "user://recursos.json";

    public int Vida;

    public int Gold;
    public int Wood;
    public int Stone;
    public int Iron;

    public int BaseVida = 100;

    public int BaseGold = 100;
    public int BaseWood = 50;
    public int BaseStone = 0;
    public int BaseIron = 0;

    public int ProdVida { get; set; }

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

        public int ProdVida { get; set; }
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

    public void RepairBase()
    {
        Vida = BaseVida + ProdVida;
    }

    public void StartLevel()
    {
        Vida = BaseVida + ProdVida;
        Gold = BaseGold + ProdGold;
        Wood = BaseWood + ProdWood;
        Stone = BaseStone + ProdStone;
        Iron = BaseIron + ProdIron;
    }

    public void FirstLevel()
    {
        Vida = 100;
        Gold = 10000;
        Wood = 10000;
        Stone = 10000;
        Iron = 10000;
    }

    public void LastLevel()
    {
        Vida = BaseVida + ProdVida;
        Gold = BaseGold + ProdGold + 500;
        Wood = BaseWood + ProdWood + 400;
        Stone = BaseStone + ProdStone + 300;
        Iron = BaseIron + ProdIron + 200;
    }

    public void MuchoDinero()
    {
        Gold = 1000000;
        Wood = 1000000;
        Stone = 1000000;
        Iron = 1000000;
    }

    public void VidaInfinita()
    {
        Vida = 999999999;
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
        BaseVida = 100;
        TotalGold = 0;
        TotalWood = 0;
        TotalStone = 0;
        TotalIron = 0;

        ProdVida = 0;
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

    public void AddProductionIslas()
    {
        Gold += (int)(ProdGold * 0.75f);
        Wood += (int)(ProdWood * 0.75f);
        Stone += (int)(ProdStone * 0.75f);
        Iron += (int)(ProdIron * 0.75f);
    }

    public void EndLevel()
    {
        TotalGold += Gold;
        TotalWood += Wood;
        TotalStone += Stone;
        TotalIron += Iron;

        SaveData();
    }

    public void KillBase()
    {
        Vida = 0;
    }

    public void DevolverRecuros()
    {
        Gold += 100;
        Wood += 50;
        Stone += 0;
        Iron += 0;
    }

    public void SaveData()
    {
        var data = new SaveDataStruct
        {
            TotalGold = TotalGold,
            TotalWood = TotalWood,
            TotalStone = TotalStone,
            TotalIron = TotalIron,

            ProdVida = ProdVida,
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

        ProdVida = data.ProdVida;
    	ProdGold = data.ProdGold;
    	ProdWood = data.ProdWood;
    	ProdStone = data.ProdStone;
    	ProdIron = data.ProdIron;
	}
}