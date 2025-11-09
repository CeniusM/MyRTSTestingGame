using UnityEngine;

// This is where the main logic of the game comes from
public class WorldManager : MonoBehaviour
{
    private InputTracker _inputTracker;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputTracker = FindAnyObjectByType<InputTracker>();

        if (_inputTracker == null)
        {
            Debug.LogError("InputTracker not found in the scene!");
        }
        else
        {
            Debug.Log("WorldManager connected to InputTracker.");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Test UserInputs
        if (_inputTracker == null || _inputTracker.LastFrameInputsHistory == null)
            return;

        var inputs = _inputTracker.LastFrameInputsHistory;

        if (inputs.Length > 0)
        {
            Debug.Log("Frame Input Events:");
            foreach (var input in inputs)
            {
                Debug.Log(" * " + input);
            }
        }
    }
}
