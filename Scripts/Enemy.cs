using Godot;
using System;

public partial class Enemy : CharacterBody3D
{

	private Vector3 velocity;
	private float speed = 4.0f;
	private float jumpVelocity = 2.0f;
	private float pursueRange = 10.0f;
	private float attackRange = 1.5f;
	private float health = 10.0f;
	private bool canMove = true;

	private EnemyType enemyType;
	private Player player;
	private Node3D battleScene;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Array enemyTypes = Enum.GetValues(typeof(EnemyType));
		Random random = new Random();
		enemyType = (EnemyType) enemyTypes.GetValue(random.Next(enemyTypes.Length));
		velocity = new Vector3(0, 0, 0);
		player = GetParent().GetNode<Player>("Player");
        battleScene = GetParent().GetNode<Node3D>("Fight Scene/Enemy Pos");
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

        Position = battleScene.GlobalPosition;
		Rotation = battleScene.Rotation;
	}
}
