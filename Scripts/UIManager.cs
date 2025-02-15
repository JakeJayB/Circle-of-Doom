using Godot;
using System;

public partial class UIManager : Control
{
    private Player player;
    private FightScene fightScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		var uiGroups = GetChildren();



        foreach (Node group in uiGroups)
        {
            CanvasItem item;

            foreach (Node node in group.GetChildren())
            {
                item = (CanvasItem)node;
			    item.Visible = false;
            }
            item = (CanvasItem)group;
            item.Visible = false;
        }

        player = GetParent().GetNode<Player>("Player");
        fightScene = GetParent().GetNode<FightScene>("Fight Scene");
    }


    public void DisplayUI(string uiGroup)
    {
		Godot.Collections.Array<Node> ui = GetTree().GetNodesInGroup(uiGroup);

        if(ui == null)
        {
            GD.PrintErr("UIManager: UI group not found");
            return;
        }

        foreach (Node node in ui)
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

    public void PlayerAttackButton()
    {
        GetNode<Label>("PlayerAttack/AttackRollDieOptions").Visible = false;
        GetNode<Button>("PlayerAttack/PlayerAttackButton").Visible = false;
        fightScene.OnPlayerRollAttackDie();
    }

    public void PlayerDodgeButton()
    {
        GetNode<Label>("PlayerDodge/DodgeRollDieOptions").Visible = false;
        GetNode<Button>("PlayerDodge/PlayerDodgeButton").Visible = false;
        fightScene.OnPlayerRollDodgeDie();
    }

    public void EnemyDetermined(int dieRoll, string enemyName, string damageType)
    {
        GetNode<Label>("DetermineEnemy/DieEnemyText").Text = dieRoll.ToString();
        GetNode<Label>("DetermineEnemy/YourEnemyText").Text += enemyName;
        GetNode<Label>("DetermineEnemy/DamageTypeText").Text = "Damage Type to Enemy: " + damageType;
        DisplayUI("DieEnemyRoll");
    }


    public void PlayerAttackDetermined(int roll1, int roll2, float finalAttack)
    {
        GetNode<Label>("PlayerAttack/DiePlayerAttackText").Text = roll1.ToString();
        GetNode<Label>("PlayerAttack/DiePlayerAttackText2").Text = roll2.ToString();
        GetNode<Label>("PlayerAttack/DamageDealtText").Text = "Damage Dealt to Enemy: " + finalAttack.ToString();
        DisplayUI("DiePlayerAttackRoll");
    }

    public void EnemyAttackDetermined(int roll1, int roll2, float finalAttack)
    {
        GetNode<Label>("EnemyAttack/DieEnemyAttackText").Text = roll1.ToString();
        GetNode<Label>("EnemyAttack/DieEnemyAttackText2").Text = roll2.ToString();
        GetNode<Label>("EnemyAttack/DamageDealtText").Text = "Damage Dealt to Player: " + finalAttack.ToString();
        DisplayUI("DieEnemyAttackRoll");
    }

    public void PlayerDodgeDetermined(int roll, string dodgeResult)
    {
        GetNode<Label>("PlayerDodge/DieDodgeText").Text = roll.ToString();
        GetNode<Label>("PlayerDodge/DodgeResultText").Text = "Player Dodge Result: " + dodgeResult;
        DisplayUI("DieDodgeRoll");
    }

    public void HideDetermineEnemyUI()
    {
        HideUI("DetermineEnemy");
        HideUI("DieEnemyRoll");
    }

    public void HidePlayerAttackUI()
    {
        HideUI("PlayerAttack");
        HideUI("DiePlayerAttackRoll");
    }

    public void HideEnemyAttackUI()
    {
        HideUI("EnemyAttack");
        HideUI("DieEnemyAttackRoll");
    }

    public void HidePlayerDodgeUI()
    {
        HideUI("PlayerDodge");
        HideUI("DieDodgeRoll");
    }
}
