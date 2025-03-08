using Godot;
using System;
using System.Diagnostics;

public partial class Enemy : CharacterBody3D
{

	private Vector3 velocity;
	private float speed = 8f;
	private float jumpVelocity = 2.0f;
	private float pursueRange = 15.0f;
	private float attackRange = 1.5f;
    private float health = 20.0f;

    public bool isDead = false;
    private bool canMove = true;

	public EnemyType enemyType;
	private Player player;
    private AnimationPlayer anim;

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
			GD.PrintErr("Enemy: Invalid enemy die roll. Default to Dino");
			return EnemyType.DINO;

    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		enemyType = EnemyType.NONE;
		player = GetParent().GetParent().GetNode<Player>("Player");
		fightScene = GetParent().GetParent().GetNode<FightScene>("Fight Scene");
        battlePos = GetParent().GetParent().GetNode<Node3D>("Fight Scene/Enemy Pos");
        anim = GetNode<AnimationPlayer>("AnimationPlayer");
    }

	public override void _PhysicsProcess(double delta)
	{
        MoveAndSlide();
		Animate();

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
			//velocity = Velocity;
			velocity.X = direction.X * speed;
			velocity.Z = direction.Z * speed;
		}
		else
		{
			velocity.X = 0;
			velocity.Z = 0;
		}

		Velocity = velocity;
        if (new Vector2(Velocity.X, Velocity.Z).Length() > 0.0)
        {
			float rotation_direction = 0.0f;
            rotation_direction = new Vector2(Velocity.Z, Velocity.X).Angle();
            Vector3 rot = Rotation;
            rot.Y = (float)(Mathf.LerpAngle(Rotation.Y, rotation_direction, delta * 10));
            Rotation = rot;
        }

    }	

	private void TeleportToBattle()
	{
		canMove = false;

        velocity.X = 0;
        velocity.Z = 0;
        Velocity = velocity;

        Position = battlePos.GlobalPosition;
		Rotation = battlePos.GlobalRotation;
	}

    public void AssignEnemy(EnemyType enemyType)
    {
        this.enemyType = enemyType;
    }

	public float Attack(int roll1, int roll2)
    {
        return Mathf.Max(roll1, roll2);
    }

    public void TakeDamage(float damage)
	{
		this.health -= damage;
		GD.Print("Enemy Health: " + health);
	}

	public bool isEnemyDead()
    {
		if(!isDead && this.health <= 0)
		{
            isDead = true;
			GD.Print("Enemy is Dead");
		}
        return isDead;
    }

    public void DestroyEnemy()
    {
        this.Visible = false;
		Free();
    }

	public float GetHealth() { return health; }


    private void Animate()
    {
        if (Velocity.Length() > 0.0)
        {
            anim.Play("walk");
        }
        else
        {
            anim.Play("idle");
        }
    }
}
