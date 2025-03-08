using Godot;
using System;

public partial class Heart : Node3D
{

	private Player player;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        player = GetParent().GetParent().GetNode<Player>("Player");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		FacePlayer();
		if (player == null) return;

		Vector3 playerPos = player.Position;
		playerPos.Y = Position.Y;

		if (playerPos.DistanceTo(Position) < 0.5f)
		{
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            player.AddHealth(rand.Next(8, 16));

			GetParent().GetParent().GetNode<UIManager>("Control").AddHealthUI(player);
            Free();
		}

	}

	private void FacePlayer()
	{
		Vector3 target = player.Position;
        target.Y = Position.Y;
        LookAt(target, Vector3.Up);
    }
}
