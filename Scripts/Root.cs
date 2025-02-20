using Godot;
using System;

public partial class Root : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() => DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
	

}
