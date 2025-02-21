using Godot;
using System;
using System.Threading.Tasks;

public partial class FightScene : StaticBody3D
{
    private UIManager uiManager;
    private Player player;
    private Enemy enemy;
    private TaskCompletionSource<bool> weaponSelectedTaskSource;
    private TaskCompletionSource<bool> playerRollAttackDieTaskSource;
    private TaskCompletionSource<bool> playerRollDodgeDieTaskSource;



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
        // Event handlers for UI button
        weaponSelectedTaskSource = new TaskCompletionSource<bool>();

        //await for player to select weapon UI button
        uiManager.SetupHealthUI(player, enemy);
        uiManager.DisplayUI("PickWeapon");
        await weaponSelectedTaskSource.Task;

        // Randomly determine the enemy. await for 2 seconds
        uiManager.DisplayUI("DetermineEnemy");
        await ToSignal(GetTree().CreateTimer(2), "timeout");

        int dieRoll = RollDice();
        enemy.AssignEnemy(Enemy.GetEnemy(dieRoll));
        string damageType = player.weapon.AssignDamageType(enemy.enemyType);
        uiManager.EnemyDetermined(dieRoll, Enemy.GetEnemy(dieRoll).ToString(), damageType);
        uiManager.UpdateDamageType(damageType);
            
        // await for 4 of seconds
        await ToSignal(GetTree().CreateTimer(4), "timeout");
        uiManager.HideDetermineEnemyUI();

        while (!player.isPlayerDead()) 
        {
            // for die rolls 
            int attackRoll1, attackRoll2, dodgeRoll;
            float finalAttack;
            string dodgeResult;

            // event handlers for UI buttons
            playerRollAttackDieTaskSource = new TaskCompletionSource<bool>();
            playerRollDodgeDieTaskSource = new TaskCompletionSource<bool>();

            // Player Attacks Enemy
            uiManager.UpdateAttackDieOptions(player.weapon.damage);
            uiManager.DisplayUI("PlayerAttack");
            await(playerRollAttackDieTaskSource.Task);
            await ToSignal(GetTree().CreateTimer(1), "timeout");
            attackRoll1 = RollDice();  attackRoll2 = RollDice();
            finalAttack = player.Attack(attackRoll1, attackRoll2);
            enemy.TakeDamage(finalAttack);
            uiManager.PlayerAttackDetermined(attackRoll1, attackRoll2, finalAttack);
            uiManager.UpdateHealthUI(null, enemy);
            await ToSignal(GetTree().CreateTimer(5), "timeout");
            uiManager.HidePlayerAttackUI();
            //await ToSignal(GetTree().CreateTimer(2), "timeout");

            if(enemy.isEnemyDead())
                break;
            

            // Enemy Attacks Player
            uiManager.DisplayUI("EnemyAttack");
            await ToSignal(GetTree().CreateTimer(3), "timeout");
            attackRoll1 = RollDice(); attackRoll2 = RollDice();
            finalAttack = enemy.Attack(attackRoll1, attackRoll2);
            player.TakeDamage(finalAttack);
            uiManager.EnemyAttackDetermined(attackRoll1, attackRoll2, finalAttack);
            await ToSignal(GetTree().CreateTimer(5), "timeout");
            uiManager.HideEnemyAttackUI();
            //await ToSignal(GetTree().CreateTimer(2), "timeout");

            // Player Dodges Enemey
            uiManager.DisplayUI("PlayerDodge");
            await (playerRollDodgeDieTaskSource.Task);
            await ToSignal(GetTree().CreateTimer(1), "timeout");
            dodgeRoll = RollDice();
            dodgeResult = player.Dodge(dodgeRoll, finalAttack);
            uiManager.PlayerDodgeDetermined(dodgeRoll, dodgeResult);
            uiManager.UpdateHealthUI(player, null);
            await ToSignal(GetTree().CreateTimer(5), "timeout");
            uiManager.HidePlayerDodgeUI();
        }

        if (player.isPlayerDead())
            Win();
        else if (enemy.isEnemyDead())
            Lose();
        
        ResetEverything();
    }

    private void Win()
    {
        uiManager.
    }

    private int RollDice()
    {
        Random random = new Random();
        return random.Next(1, 7);
    }

    private void ResetEverything()
    {
        player = null;
        enemy = null;
        weaponSelectedTaskSource = null;
        playerRollAttackDieTaskSource = null;
        playerRollDodgeDieTaskSource = null;
    }

    // These methods update the event handlers 
    public void OnWeaponSelected() => weaponSelectedTaskSource?.TrySetResult(true);
    
    public void OnPlayerRollAttackDie() => playerRollAttackDieTaskSource?.TrySetResult(true);
    
    public void OnPlayerRollDodgeDie() => playerRollDodgeDieTaskSource?.TrySetResult(true);
    
}
