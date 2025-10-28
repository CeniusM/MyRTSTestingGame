using System.Collections.Generic;
using UnityEngine;

public enum UserInputType
{
    Keypress,
    MouseClick,
    //MouseMovement
}

public class UserInput
{
    public UserInputType InputType;

    public KeyCode KeyCode;
    public Vector2 MouseClickPosition;
    //public int
}

public class InputTracker : MonoBehaviour
{
    //public List<UserInput> History;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //History = new List<UserInput>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
