using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    [SerializeField] public float PushStrength = 10f; // Push per second

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        // Should later also move the units based on commands and their attributes, and nav meshes but that is for later

        // Sperate units
        BaseUnit[] allUnits = FindObjectsByType<BaseUnit>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < allUnits.Length; i++)
        {
            BaseUnit unit = allUnits[i];

            for (int j = 0; j < allUnits.Length; j++)
            {
                if (i == j)
                    continue;

                BaseUnit otherUnit = allUnits[j];
                Vector2 toOther = otherUnit.transform.position - unit.transform.position;
                float distance = toOther.magnitude;
                float minDistance = unit.Attributes.Radius + otherUnit.Attributes.Radius;
                if (distance < minDistance)
                {
                    Vector2 pushDirection = toOther.normalized;
                    float pushAmount = (minDistance - distance) * PushStrength * Time.fixedDeltaTime;

                    pushAmount = Mathf.Min(pushAmount, distance);

                    // For later, some units might be immovable, and some units might be heavier than others,
                    // a zergling should not push a ultra the same amout as another ultra

                    unit.transform.position -= (Vector3)(pushDirection * (pushAmount / 2f));
                    otherUnit.transform.position += (Vector3)(pushDirection * (pushAmount / 2f));
                }

            }
        }
    }
}
