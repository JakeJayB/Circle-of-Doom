using Godot;
using System;
using System.Threading.Tasks;

public partial class FightScene : StaticBody3D
{
    private UIManager uiManager;
    private Player player;
    private Enemy enemy;
    private TaskCompletionSource<bool> weaponSelectedTaskSource;


    public override void _Ready()
    {
        uiManager = GetParent().GetNode<UIManager>("Control");
    }

    public void StartCoroutine(Player player, Enemy enemy)
	{
        this.player = player;
        this.enemy = enemy;
        Fight();
	}

	private async void Fight()
    {

        uiManager.DisplayUI("PickWeapon");

        //await for player to select weapon UI button
        weaponSelectedTaskSource = new TaskCompletionSource<bool>();
        await weaponSelectedTaskSource.Task;

        uiManager.DisplayUI("DetermineEnemy");

        // await for a couple of seconds
        await ToSignal(GetTree().CreateTimer(2), "timeout");

        // Randomly determine the enemy
        int dieRoll = RollDice();
        enemy.AssignEnemy(Enemy.GetEnemy(dieRoll));
        uiManager.EnemyDetermined(dieRoll, Enemy.GetEnemy(dieRoll).ToString());
        player.weapon.AssignDamageType(enemy.enemyType);

        // await for a couple of seconds
        await ToSignal(GetTree().CreateTimer(4), "timeout");
        uiManager.HideDetermineEnemyUI();

        while (true) 
        {
            // Player Attacks Enemy
            uiManager.DisplayUI("PlayerAttack");
            await ToSignal(GetTree().CreateTimer(2), "timeout");
            int attackRoll1 = RollDice(); int attackRoll2 = RollDice();
            float finalAtack = player.weapon.Attack(attackRoll1, attackRoll2);
            enemy.TakeDamage(finalAtack);
            uiManager.PlayerAttackDetermined(attackRoll1, attackRoll2, finalAtack);
            await ToSignal(GetTree().CreateTimer(4), "timeout");
            uiManager.HidePlayerAttackUI();

            // Enemy Attacks Player

            // Player Dodges Enemey
            break;
        }
        ResetEverything();
    }

    private int RollDice()
    {
        Random random = new Random();
        return random.Next(1, 7);
    }


    private void ResetEverything()
    {
        this.player = null;
        this.enemy = null;
        weaponSelectedTaskSource = null;
    }

    // This method will be called when the player selects a weapon
    public void OnWeaponSelected()
    {
        weaponSelectedTaskSource?.TrySetResult(true);
    }
}
