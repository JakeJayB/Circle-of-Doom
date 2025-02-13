using Godot;
using System;
using System.Diagnostics;

public partial class Enemy : CharacterBody3D
{

	private Vector3 velocity;
	private float speed = 4.0f;
	private float jumpVelocity = 2.0f;
	private float pursueRange = 10.0f;
	private float attackRange = 1.5f;
	private float health = 30.0f;
	private bool canMove = true;

	public EnemyType enemyType;
	private Player player;
	private FightScene fightScene;
	private Node3D battlePos;


    public enum EnemyType
    {
        DINO,
        FROG,
        TURTLE,
        NONE
    }


    public static EnemyType GetEnemy(int dieRoll)
	{
		if (dieRoll == 1 || dieRoll == 2)
			return EnemyType.DINO;
		else if (dieRoll == 3 || dieRoll == 4)
			return EnemyType.FROG;
		else if (dieRoll == 5 || dieRoll == 6)
            return EnemyType.TURTLE;
        else
			GD.PrintErr("Invalid die roll. Default to Dino");
			return EnemyType.DINO;

    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		enemyType = EnemyType.NONE;
		velocity = new Vector3(0, 0, 0);
		player = GetParent().GetNode<Player>("Player");
        fightScene = GetParent().GetNode<FightScene>("Fight Scene");

		// Gets "Enemy Pos" Node3D from the Fight Scene.
        battlePos = GetParent().GetNode<Node3D>("Fight Scene/Enemy Pos");
    }

	public override void _PhysicsProcess(double delta)
	{
        MoveAndSlide();
        // Add the gravity.
        if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;

			if (!canMove)
				Velocity = velocity;
		}

		if (!canMove) return;

		float distance = Position.DistanceTo(player.Position);
		if (distance < attackRange)
		{
			fightScene.StartCoroutine(player, this);
            TeleportToBattle();
			player.TeleportToBattle();
			return;
		}
		else if (distance < pursueRange)
		{
			Vector3 target = player.Position;
			target.Y = Position.Y;

			Vector3 direction = Position.DirectionTo(target);
			velocity = Velocity;
			velocity.X = direction.X * speed;
			velocity.Z = direction.Z * speed;
		}
		else
		{
			velocity.X = 0;
			velocity.Z = 0;
		}

		Velocity = velocity;
	}	

	private void TeleportToBattle()
	{
		canMove = false;

        velocity.X = 0;
        velocity.Z = 0;
        Velocity = velocity;

        Position = battlePos.GlobalPosition;
		Rotation = battlePos.Rotation;
	}

    public void AssignEnemy(EnemyType enemy)
    {
        this.enemyType = enemy;
    }

	public void TakeDamage(float damage)
	{
		this.health -= damage;
		GD.Print("Enemy Health: " + health);
	}
}
