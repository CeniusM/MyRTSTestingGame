using UnityEngine;

// Controlgroups and economy data for the player
public class PlayerData : MonoBehaviour
{
    public ControlGroup[] controlGroups;

    // Not one of the control groups, just the currently selected one
    public ControlGroup activeGroup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controlGroups = new ControlGroup[10];
        activeGroup = new ControlGroup();
    }

    // Update is called once per frame
    void Update()
    {
        // Probaly pretty slow, but for ui it might be fine? Idk
        foreach (var group in controlGroups)
        {
            group?.Cleanup();
        }
        activeGroup.Cleanup();
    }

    public void SelectControlGroup(int index)
    {
        activeGroup = controlGroups[index].Clone();
    }

    public void SelectUnit(BaseUnit unit)
    {
        activeGroup.ClearGroup();
        activeGroup.AddUnitToGroup(unit);
    }

    public void AddSelectUnit(BaseUnit unit)
    {
        activeGroup.AddUnitToGroup(unit);
    }

    public void DeselectAll()
    {
        activeGroup.ClearGroup();
    }
}
