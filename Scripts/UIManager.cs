using Godot;
using System;

public partial class UIManager : Control
{
    private Player player;
    private FightScene fightScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        player = GetParent().GetNode<Player>("Player");
        fightScene = GetParent().GetNode<FightScene>("Fight Scene");

        ResetEverything();
        SetupEnemyCountUI();

    }

    public void ResetEverything()
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
    }

    public void DisplayUI(string uiGroup)
    {
		Godot.Collections.Array<Node> ui = GetTree().GetNodesInGroup(uiGroup);

        if (ui == null)
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

    public void SetupEnemyCountUI()
    {
        int currEnemyCount = GetTree().GetNodesInGroup("Enemy").Count;
        GetNode<Label>("EnemyCount/CountText").Text = currEnemyCount + " Enemies Left";
        DisplayUI("EnemyCount");

    }

    public async void AddHealthUI(Player player)
    {
        UpdateHealthUI(player, null);
        DisplayUI("AddHealth");
        await ToSignal(GetTree().CreateTimer(4), "timeout");
        HideUI("AddHealth");
    }

    public void SetupHealthUI(Player player, Enemy enemy)
    {
        GetNode<Label>("Health/PlayerDamageText").Text = "Damage Type: None";
        UpdateHealthUI(player, enemy);
        DisplayUI("Health");
    }

    public void UpdateHealthUI(Player player, Enemy enemy)
    {
        if (player != null)
        {
            float playerHealth = player.GetHealth();
            GetNode<Label>("Health/PlayerHealthText").Text = "Health: " + playerHealth;
            GetNode<ProgressBar>("Health/PlayerHealthBar").Value = playerHealth;
        }

        if (enemy != null)
        {
            float enemyHealth = enemy.GetHealth();
            GetNode<Label>("Health/EnemyHealthText").Text = "Enemy Health: " + enemyHealth;
            GetNode<ProgressBar>("Health/EnemyHealthBar").Value = enemyHealth;
        }
    }  

    public void UpdateEnemyName(string name) => GetNode<Label>("Health/EnemyNameText").Text = name;

    public void UpdateDamageType(string type) => GetNode<Label>("Health/PlayerDamageText").Text = "Damage Type: " + type; 

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

    public void UpdateAttackDieOptions(DamageType damageType)
    {
        string DieOption = "Damage Type:\n";

        switch (damageType)
        {
            case DamageType.ADVANTAGE:
                DieOption += "Advantage: Highest Dice Value";
                break;
            case DamageType.NEUTRAL:
                DieOption += "Neutral: Average of Dice Values";
                break;
            case DamageType.DISADVANTAGE:
                DieOption += "Disadvantage: Lowest Dice Value";
                break;
        }

        GetNode<Label>("PlayerAttack/AttackRollDieOptions").Text = DieOption;
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
        GetNode<Label>("DetermineEnemy/YourEnemyText").Text = "Your Enemy is a " + enemyName;
        GetNode<Label>("DetermineEnemy/DamageTypeText").Text = "Damage to Enemy: " + damageType;
        UpdateEnemyName(enemyName);
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
