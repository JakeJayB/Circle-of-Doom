using Godot;
using System;

public partial class MainMenu : Node
{

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }



    public void _on_start_button_pressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/GameScene.tscn");
	}

	public  void _on_quit_button_pressed()
	{
		GetTree().Quit();
	}

    public void GoToMainMenu()
    {
        GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
    }

}
