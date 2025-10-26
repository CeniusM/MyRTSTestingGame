using UnityEngine;

public class KeyDownHandler : MonoBehaviour
{
    private PlayerData playerData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerData = FindAnyObjectByType<PlayerData>();
    }

    // Update is called once per frame
    void Update()
    {
        bool queue = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (Input.GetKeyDown(KeyCode.W))
        {
            playerData.activeGroup.SendCommandToAll(new BaseCommand(queue, CommandType.Stop));
        }
    }
}
