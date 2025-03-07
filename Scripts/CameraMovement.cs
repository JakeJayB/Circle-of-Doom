using Godot;
using System;

public partial class CameraMovement : Node3D
{
    private bool canMove = true;
    private float camSensitivity = 0.002f;
    private float cameraPitch = 0.0f;

    public override void _Input(InputEvent @event)
    {
        if (canMove && @event is InputEventMouseMotion m)
        {

            // Rotates entire player on the X-axis for horizontal movement (yaw)
            RotateY(-m.Relative.X * camSensitivity);

            // Rotate camera on the Y-axis for vertical movement (pitch)
            cameraPitch -= m.Relative.Y * camSensitivity;

            // Clamp the pitch to prevent flipping
            cameraPitch = Mathf.Clamp(cameraPitch, Mathf.DegToRad(-20f), Mathf.DegToRad(35f));

            // Apply the clamped pitch to the camera's rotation
            Rotation = new Vector3(-cameraPitch, Rotation.Y, Rotation.Z);
        }
    }

    public void SetMove(bool status) => canMove = status;
}

