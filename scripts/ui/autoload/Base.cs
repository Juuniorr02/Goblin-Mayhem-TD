using Godot;
using System;

public partial class Base : Node
{
    public static Base Instance;

    public int Health = 100;
    public int Gold = 200;
    public int Wood = 50;
    public int Stone = 0;
    public int Iron = 0;

    public override void _Ready()
    {
        Instance = this;
    }

    public void AddGold(int amount)
    {
        Gold += amount;
    }

    public void DamageBase(int damage)
    {
        Health -= damage;
    }

    public void RepairBase()
    {
        Health = 100;
    }

    public void AddIron(int amount)
	{
		Iron += amount;
	}

	public void AddWood(int amount)
	{
		Wood += amount;
	}

	public void AddStone(int amount)
	{
		Stone += amount;
	}
}