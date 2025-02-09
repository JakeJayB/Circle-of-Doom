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
        Random random = new Random();
        int dieRoll = random.Next(1, 7);
        enemy.AssignEnemy(Enemy.GetEnemy(dieRoll));
        uiManager.EnemyDetermined(dieRoll, Enemy.GetEnemy(dieRoll).ToString());

        // await for a couple of seconds
        await ToSignal(GetTree().CreateTimer(4), "timeout");
        uiManager.HideUI("DetermineEnemy");
        uiManager.HideUI("DieRoll");

        while (false) { }
        ResetEverything();
    }

    // This method will be called when the player selects a weapon
    public void OnWeaponSelected()
    {
        weaponSelectedTaskSource?.TrySetResult(true);
    }


    private void ResetEverything()
    {
        this.player = null;
        this.enemy = null;
        weaponSelectedTaskSource = null;
    }
}
