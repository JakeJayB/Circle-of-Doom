using Godot;
using static Enemy;


public partial class Player : CharacterBody3D
{
    private Vector3 velocity;

    private const float Speed = 10.0f;
    private const float JumpVelocity = 4.5f;
    private const float camSensitivity = 0.002f;
    private float health = 10.0f;

    private bool isDead = false;
    private bool canMove = true;

    private Node3D battlePos;
    private Transform3D prevMapPos;

    private Node3D camera;
    private Node3D character;
    private AnimationPlayer anim;
    public Weapon weapon = new Weapon();

    public override void _Ready()
    {
        camera = GetNode<Node3D>("Neck");
        battlePos = GetParent().GetNode<Node3D>("Fight Scene/Player Pos");
        anim = GetNode<AnimationPlayer>("AnimationPlayer");
        character = GetNode<Node3D>("character");
    }


    public override void _PhysicsProcess(double delta)
    {
        // Moves player based on Velocity        
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

        // Handle Jump.
        if (Input.IsActionJustPressed("Jump") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        // Get the input direction and handle the movement/deceleration.
        Vector2 inputDir = Input.GetVector("Move_Right", "Move_Left", "Move_Backward", "Move_Forward");
        Vector3 direction = new Vector3(inputDir.X, 0, inputDir.Y).Rotated(Vector3.Up, camera.Rotation.Y).Normalized();

        // if player is moving
        if (direction != Vector3.Zero)
        {
            // moves the player in the direction of the input.
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;

            Vector3 rot = character.Rotation;
            rot.Y = (float) Mathf.LerpAngle(character.Rotation.Y, Mathf.Atan2(velocity.X, velocity.Z), delta * 10);
            character.Rotation = rot;
        }
        else
        {
            // decelerates the player if no input is detected.
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

        prevMapPos = Transform;
        Position = battlePos.GlobalPosition;
        character.Rotation = battlePos.GlobalRotation;
        camera.Rotation = battlePos.GlobalRotation;
        ((CameraMovement)camera).SetMove(false);

    }

    public void TeleportToMap()
    {
        canMove = true;
        ((CameraMovement)camera).SetMove(true);
        weapon.ResetWeapon();
        Input.MouseMode = Input.MouseModeEnum.Captured;
        Transform = prevMapPos;
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
    }

    public void AddHealth(float health)
    {
        this.health += health;
        if (this.health > 20)
            this.health = 20;
        GD.Print("Player Health: " + this.health);
    }

    public float GetHealth() { return health; }

    private void Animate()
    {
        if (IsOnFloor())
        {
            if (Velocity.Length() > 0)
            {
                anim.Play("walk");
            }
            else
            {
                anim.Play("idle");
            }
        }
        else
        {
            anim.Play("jump");
        }
    }

    
}
