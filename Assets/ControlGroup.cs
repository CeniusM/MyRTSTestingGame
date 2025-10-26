using Unity;
using System.Collections.Generic;

public class ControlGroup
{
    public string GroupName;
    public readonly List<BaseUnit> UnitsInGroup;

    public int Count => UnitsInGroup.Count;
    public bool IsEmpty => UnitsInGroup.Count == 0;

    public ControlGroup()
    {
        GroupName = "";
        UnitsInGroup = new List<BaseUnit>();
    }

    // This should not work tho if the units are on the other team
    public void SendCommandToAll(BaseCommand command)
    {
        foreach (var unit in UnitsInGroup)
        {
            unit.AssignCommand(command);
        }
    }

    public bool Contains(BaseUnit unit)
    {
        return UnitsInGroup.Contains(unit);
    }

    public void Cleanup()
    {
        UnitsInGroup.RemoveAll(unit => unit == null || unit.Dead);
    }

    public void AddUnitToGroup(BaseUnit unit)
    {
        if (!UnitsInGroup.Contains(unit))
            UnitsInGroup.Add(unit);
    }

    public void AddUnitsToGroup(IEnumerable<BaseUnit> units)
    {
        foreach (var unit in units)
        {
            AddUnitToGroup(unit);
        }
    }

    public void RemoveUnitFromGroup(BaseUnit unit)
    {
        if (UnitsInGroup.Contains(unit))
        {
            UnitsInGroup.Remove(unit);
        }
    }

    public void ClearGroup()
    {
        UnitsInGroup.Clear();
    }

    public ControlGroup Clone()
    {
        var newGroup = new ControlGroup
        {
            GroupName = this.GroupName
        };
        newGroup.AddUnitsToGroup(this.UnitsInGroup);
        return newGroup;
    }
}