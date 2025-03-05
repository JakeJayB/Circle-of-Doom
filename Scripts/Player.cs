using Godot;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Channels;
using static Enemy;


public partial class Player : CharacterBody3D
{
    private Vector3 velocity;
    private Vector3 neckGlobalPosition;

    private float rotation_angle = 0f;
    private const float Speed = 10.0f;
    private const float JumpVelocity = 4.5f;
    private const float camSensitivity = 0.004f;
    private float cameraPitch = 0.0f;
    //private float health = 20.0f;
    private float health = 10.0f;
    private float rotationAngle;

    private bool isDead = false;
    private bool canMove = true;

    private Camera3D camera;
    private Node3D battlePos;
    private Transform3D mapPos;
    public Weapon weapon = new Weapon();


    public override void _Ready()
    {
        camera = GetNode<Camera3D>("Neck/Camera3D");
        battlePos = GetParent().GetNode<Node3D>("Fight Scene/Player Pos");
    }

    public override void _Input(InputEvent @event)
    {
        if (canMove && @event is InputEventMouseMotion m)
        {
            RotateY(-m.Relative.X * camSensitivity);

            // Rotate camera on the X-axis for vertical movement (pitch)
            cameraPitch -= m.Relative.Y * camSensitivity;

            // Clamp the pitch to prevent flipping
            cameraPitch = Mathf.Clamp(cameraPitch, Mathf.DegToRad(-20f), Mathf.DegToRad(35f));

            // Apply the clamped pitch to the camera's rotation
            camera.Rotation = new Vector3(cameraPitch, camera.Rotation.Y, camera.Rotation.Z);
        }
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

        // Handle Jump.
        if (Input.IsActionJustPressed("Jump") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputDir = Input.GetVector("Move_Right", "Move_Left", "Move_Backward", "Move_Forward");
        Vector3 direction = (GlobalTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
        }
        Velocity = velocity;

    }

    public void TeleportToBattle()
    {
        canMove = false;
        Input.MouseMode = Input.MouseModeEnum.Visible;

        velocity.X = 0;
        velocity.Z = 0;
        Velocity = velocity;

        mapPos = Transform;
        Transform = battlePos.GlobalTransform;
        camera.Rotation = new Vector3(Mathf.DegToRad(-20f), camera.Rotation.Y, camera.Rotation.Z);

    }

    public void TeleportToMap()
    {
        canMove = true;
        weapon.ResetWeapon();
        Input.MouseMode = Input.MouseModeEnum.Captured;
        Transform = mapPos;
    }

    public float Attack(int roll1, int roll2)
    {
        return weapon.Attack(roll1, roll2);
    }

    public string Dodge(int dodgeRoll, float prevAttack)
    {
        if (dodgeRoll == 1 || dodgeRoll == 2)
        {
            GD.Print("Player Health Now: " + health);
            isPlayerDead();
            return "Full Damage...";
        }
        else if (dodgeRoll == 3 || dodgeRoll == 4)
        {
            prevAttack /= 2;
            health += prevAttack;
            isPlayerDead();
            GD.Print("Player Health Now: " + health);
            return "Half Damage";
        }
        else if (dodgeRoll == 5 || dodgeRoll == 6)
        {
            health += prevAttack;
            isPlayerDead();
            GD.Print("Player Health Now: " + health);
            return "Full Dodge!";
        }
        else
        {
            GD.PrintErr("Player: Invalid dodge die roll. Default to Full Damage");
            GD.Print("Player Health Now: " + health);
            isPlayerDead();
            return "Full Damage...";
        }
            
    }
    public bool isPlayerDead()
    {
        if (!isDead && health <= 0)
        {
            isDead = true;
            GD.Print("Player is Dead");
        }
        return isDead;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        GD.Print("Player Health: " + health);
    }

    public float GetHealth() { return health; }

}
