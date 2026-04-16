using Godot;
using System;

public partial class Uialdea : Control
{
    private Label goldLabel;
    private Label ironLabel;
    private Label woodLabel;
    private Label stoneLabel;


    public override void _Ready()
    {
        goldLabel = GetNode<Label>("%GoldLabel");
        ironLabel = GetNode<Label>("%IronLabel");
        woodLabel = GetNode<Label>("%WoodLabel");
        stoneLabel = GetNode<Label>("%StoneLabel");

        UpdateIU();
    }

    public override void _Process(double delta)
    {
        UpdateIU();
    }

    private void UpdateIU()
    {
        goldLabel.Text = Base.Instance.Gold.ToString();
        ironLabel.Text = Base.Instance.Iron.ToString();
        woodLabel.Text = Base.Instance.Wood.ToString();
        stoneLabel.Text = Base.Instance.Stone.ToString();
    }
}