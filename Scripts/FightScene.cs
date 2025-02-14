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


        //await for player to select weapon UI button
        uiManager.DisplayUI("PickWeapon");
        weaponSelectedTaskSource = new TaskCompletionSource<bool>();
        await weaponSelectedTaskSource.Task;

        // Randomly determine the enemy. await for 2 seconds
        uiManager.DisplayUI("DetermineEnemy");
        await ToSignal(GetTree().CreateTimer(2), "timeout");

        int dieRoll = RollDice();
        enemy.AssignEnemy(Enemy.GetEnemy(dieRoll));
        uiManager.EnemyDetermined(dieRoll, Enemy.GetEnemy(dieRoll).ToString());
        player.weapon.AssignDamageType(enemy.enemyType);

        // await for 4 of seconds
        await ToSignal(GetTree().CreateTimer(4), "timeout");
        uiManager.HideDetermineEnemyUI();

        while (!player.isDead && !enemy.isDead) 
        {
            int attackRoll1, attackRoll2, dodgeRoll;
            float finalAttack;
            string dodgeResult;

            // Player Attacks Enemy
            uiManager.DisplayUI("PlayerAttack");
            await ToSignal(GetTree().CreateTimer(2), "timeout");
            attackRoll1 = RollDice();  attackRoll2 = RollDice();
            finalAttack = player.Attack(attackRoll1, attackRoll2);
            enemy.TakeDamage(finalAttack);
            uiManager.PlayerAttackDetermined(attackRoll1, attackRoll2, finalAttack);
            await ToSignal(GetTree().CreateTimer(4), "timeout");
            uiManager.HidePlayerAttackUI();
            //await ToSignal(GetTree().CreateTimer(2), "timeout");

            // Enemy Attacks Player
            uiManager.DisplayUI("EnemyAttack");
            await ToSignal(GetTree().CreateTimer(2), "timeout");
            attackRoll1 = RollDice(); attackRoll2 = RollDice();
            finalAttack = enemy.Attack(attackRoll1, attackRoll2);
            player.TakeDamage(finalAttack);
            uiManager.EnemyAttackDetermined(attackRoll1, attackRoll2, finalAttack);
            await ToSignal(GetTree().CreateTimer(4), "timeout");
            uiManager.HideEnemyAttackUI();
            //await ToSignal(GetTree().CreateTimer(2), "timeout");

            // Player Dodges Enemey
            uiManager.DisplayUI("PlayerDodge");
            await ToSignal(GetTree().CreateTimer(2), "timeout");
            dodgeRoll = RollDice();
            dodgeResult = player.Dodge(dodgeRoll, finalAttack);
            uiManager.PlayerDodgeDetermined(dodgeRoll, dodgeResult);
            await ToSignal(GetTree().CreateTimer(4), "timeout");
            uiManager.HidePlayerDodgeUI();
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
