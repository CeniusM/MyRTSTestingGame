using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
    public GUID id = new GUID();

    public bool IsBuilding;
    public AttackStats AttackStats;
    public Health Health;
    //public Armour ArmourStats;
    public int Kills;

    public bool HasAttack => AttackStats != null;
    public bool IsAlive => !Health.IsDead;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
