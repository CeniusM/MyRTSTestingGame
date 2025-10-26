using UnityEngine;

public class MyUnit : BaseUnit
{
    public override void MoveTowards(Vector2 destination)
    {
        transform.position = Vector2.MoveTowards(transform.position, destination, Attributes.MoveSpeed * Time.deltaTime);
        Debug.Log($"Unit moved to {destination}");
    }

    private SpriteRenderer spriteRenderer;
    private PlayerData playerData;
    private bool IsSelected => playerData.activeGroup.Contains(this);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Attributes = new UnitAttributes(0.5f, 3);

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        playerData = FindAnyObjectByType<PlayerData>();
    }

    // Update is called once per frame, only for graphics
    void Update()
    {
        if (IsSelected)
            spriteRenderer.color = Color.red;
        else
            spriteRenderer.color = Color.white;

        // Move to fixed later
        HandleCommand();
    }
}
