using Godot;
using Godot.Collections;
using System;
using System.Runtime.CompilerServices;

public enum WeaponType
{
    LONGSWORD,
    MACE,
    BOW,
    NONE
}

public enum DamageType
{
    ADVANTAGE,
    DISADVANTAGE,
    NEUTRAL
}

public partial class Weapon : Node
{
	public WeaponType weapon;
    public DamageType damage;

    private static Dictionary<WeaponType, Enemy.EnemyType> Advantage = new Dictionary<WeaponType, Enemy.EnemyType>()
    {
        {WeaponType.LONGSWORD, Enemy.EnemyType.FROG },
        {WeaponType.MACE, Enemy.EnemyType.TURTLE },
        {WeaponType.BOW, Enemy.EnemyType.NONE },
    };

    private static Dictionary<WeaponType, Enemy.EnemyType> Disadvantage = new Dictionary<WeaponType, Enemy.EnemyType>()
    {
        {WeaponType.LONGSWORD, Enemy.EnemyType.TURTLE },
        {WeaponType.MACE, Enemy.EnemyType.FROG },
        {WeaponType.BOW, Enemy.EnemyType.NONE },
    };

    public Weapon()
    {
        ResetWeapon();
    }

    public void AssignWeapon(WeaponType weapon)
	{
		this.weapon = weapon;
	}

    public void ResetWeapon()
    {
        weapon = WeaponType.NONE;
        damage = DamageType.NEUTRAL;
    }

    public string AssignDamageType(Enemy.EnemyType enemyType)
    {
        Enemy.EnemyType advantage = Advantage[this.weapon];
        Enemy.EnemyType disadvantage = Disadvantage[this.weapon];

        if (advantage == enemyType)
        {
            this.damage = DamageType.ADVANTAGE;
            GD.Print("Weapon Damage Type: " + this.damage);
            return "Advantage :)";
        }
        else if (disadvantage == enemyType)
        { 
            this.damage = DamageType.DISADVANTAGE;
            GD.Print("Weapon Damage Type: " + this.damage);
            return "Disadvantage :(";
        }
        else
        {
            this.damage = DamageType.NEUTRAL;
            GD.Print("Weapon Damage Type: " + this.damage);
            return "Neutral :|";
        }

    }

    public float Attack(int roll1, int roll2)
    {
        switch (damage)
        {
            case DamageType.ADVANTAGE:
                return Mathf.Max(roll1, roll2);
            case DamageType.DISADVANTAGE:
                return Mathf.Min(roll1, roll2);
            case DamageType.NEUTRAL:
                return (roll1 + roll2  + 0.0f) / 2;
            default:
                GD.PrintErr("Weapon: Invalid weapon DamageType. Default to DamageType.ADVANTAGE");
                return MathF.Max(roll1, roll2);
        }
    }
	
}
