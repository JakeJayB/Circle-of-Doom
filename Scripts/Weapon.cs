using Godot;
using System;

public partial class Weapon : Node
{
	public WeaponType weapon;

	public Weapon()
    {
        weapon = WeaponType.NONE;
    }

    public void AssignWeapon(WeaponType weapon)
	{
		this.weapon = weapon;
	}
	
}
