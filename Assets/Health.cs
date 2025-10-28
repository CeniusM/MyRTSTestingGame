using System;

public class Health
{
    public float HP { get; set; }
    public float MaxHP { get; set; }

    public int HPInt => (int)MathF.Floor(HP);
    public int MaxHPInt => (int)MathF.Floor(MaxHP);

    public float PecentHP => HP / MaxHP;
    public bool IsDead => HP <= 0 && IsMortal;
    public bool IsMortal => MaxHP != 0;

    public Health(float maxHP) : this(maxHP, maxHP)
    {

    }

    public Health(float hP, float maxHP)
    {
        HP = hP;
        MaxHP = maxHP;
    }

    public override string ToString()
    {
        return $"{(int)HP}/{(int)MaxHP}";
    }
}