public class AttackStats
{
    public readonly string Name;
    public float Damage;
    public float Range;
    public float AttackSpeed; // Wait between attacks

    public bool CanAttackGround;
    public bool CanAttackAir;

    public float AttacksPerSecond => 1 / AttackSpeed;

    //public type strongAgainst

    public AttackStats(string name, float damage, float range, float attackSpeed, bool canAttackGround, bool canAttackAir)
    {
        Name = name;
        Damage = damage;
        Range = range;
        AttackSpeed = attackSpeed;
        CanAttackGround = canAttackGround;
        CanAttackAir = canAttackAir;
    }
}

public class PremadeAttackStats
{
    public AttackStats Small_MeleeAttack => new AttackStats(
            name: "Small_MeleeAttack",
            damage: 10,
            range: 1,
            attackSpeed: 2,
            canAttackGround: true,
            canAttackAir: false
        );

    public AttackStats Medium_RangeAttack => new AttackStats(
            name: "Medium_RangeMeleeAttack",
            damage: 15,
            range: 5,
            attackSpeed: 1.5f,
            canAttackGround: true,
            canAttackAir: true
        );
}