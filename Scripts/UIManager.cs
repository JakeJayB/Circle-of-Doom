using Godot;
using System;

public partial class UIManager : Control
{
    private Player player;
    private FightScene fightScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		var ui = GetChildren();
		
		foreach(var node in ui)
        {
            CanvasItem item = (CanvasItem)node;
			item.Visible = false;
        }

        player = GetParent().GetNode<Player>("Player");
        fightScene = GetParent().GetNode<FightScene>("Fight Scene");
    }

	public void DisplayUI(string uiGroup)
    {
		var ui = GetTree().GetNodesInGroup(uiGroup);

        if(ui == null)
        {
            GD.PrintErr("UIManager: UI group not found");
            return;
        }

        foreach (var node in ui)
        {
            CanvasItem item = (CanvasItem)node;
			item.Visible = true;
        }
        
    }

    public void HideUI(string uiGroup)
    {
        var ui = GetTree().GetNodesInGroup(uiGroup);

        if (ui == null)
        {
            GD.PrintErr("UIManager: UI group not found");
            return;
        }

        foreach (var node in ui)
        {
            CanvasItem item = (CanvasItem)node;
            item.Visible = false;
        }
    }

    public void WeaponSelect(string weapon)
	{
        switch (weapon)
        {
            case "Bow":
                player.weapon.AssignWeapon(WeaponType.BOW);
                break;
            case "Sword":
                player.weapon.AssignWeapon(WeaponType.LONGSWORD);
                break;
            case "Mace":
                player.weapon.AssignWeapon(WeaponType.MACE);
                break;
            default:
                GD.PrintErr("UIManager: Weapon not found");
                return;
        }

        fightScene.OnWeaponSelected();
        HideUI("PickWeapon");
    }

    public void EnemyDetermined(int dieRoll, string enemyName)
    {
        GetNode<Label>("DieNumberText").Text = dieRoll.ToString();
        GetNode<Label>("YourEnemyText").Text += enemyName;
        DisplayUI("DieRoll");
    }

}
