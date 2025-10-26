using System.Collections.Generic;
using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    public LayerMask unitLayer;
    public LayerMask groundLayer;
    public Color selectionBoxColor = new Color(0, 1, 0, 0.2f); // translucent green

    private PlayerData playerData;
    private Vector2 dragStartPos;
    private bool isDragging = false;

    private Texture2D _whiteTexture;

    void Start()
    {
        playerData = FindAnyObjectByType<PlayerData>();
        _whiteTexture = Texture2D.whiteTexture;
    }

    void Update()
    {
        HandleClicking();
    }

    void HandleClicking()
    {
        bool isShiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // --- LEFT MOUSE BUTTON DOWN ---
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = Input.mousePosition;
            isDragging = true;
        }

        // --- LEFT MOUSE BUTTON UP ---
        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                // If mouse hasn't moved much, treat as click
                // SelectSingleUnit

                Vector2 dragEndPos = Input.mousePosition;
                SelectUnitsInRectangle(dragStartPos, dragEndPos, isShiftDown);
            }

            isDragging = false;
        }

        // --- RIGHT CLICK (Move Command) ---
        if (Input.GetMouseButtonDown(1) && !playerData.activeGroup.IsEmpty)
        {
            Vector2 targetPos = mouseWorldPos;

            BaseCommand command = new BaseCommand(isShiftDown, CommandType.Move)
            {
                TargetPos = targetPos
            };

            playerData.activeGroup.SendCommandToAll(command);
        }
    }

    void OnGUI()
    {
        if (isDragging)
        {
            var rect = GetScreenRect(dragStartPos, Input.mousePosition);
            DrawScreenRect(rect, selectionBoxColor);
            DrawScreenRectBorder(rect, 2, Color.green);
        }
    }

    private void SelectUnitsInRectangle(Vector2 startScreenPos, Vector2 endScreenPos, bool additive)
    {
        Vector2 min = Camera.main.ScreenToWorldPoint(new Vector3(Mathf.Min(startScreenPos.x, endScreenPos.x), Mathf.Min(startScreenPos.y, endScreenPos.y)));
        Vector2 max = Camera.main.ScreenToWorldPoint(new Vector3(Mathf.Max(startScreenPos.x, endScreenPos.x), Mathf.Max(startScreenPos.y, endScreenPos.y)));

        Collider2D[] hits = Physics2D.OverlapAreaAll(min, max, unitLayer);

        if (!additive)
            playerData.DeselectAll();

        foreach (var hit in hits)
        {
            BaseUnit unit = hit.GetComponent<BaseUnit>();
            if (unit != null)
                playerData.AddSelectUnit(unit);
        }
    }

    //private void SelectSingleUnit(Vector2 worldPos, bool additive)
    //{
    //    RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero); // zero direction = point check

    //    if (hit.collider != null)
    //    {
    //        BaseUnit unit = hit.collider.GetComponent<BaseUnit>();
    //        if (unit != null)
    //        {
    //            if (isShiftDown)
    //                playerData.AddSelectUnit(unit);
    //            else
    //                playerData.SelectUnit(unit);
    //        }
    //    }
    //    else
    //    {
    //        playerData.DeselectAll();
    //    }
    //}

    #region Drawing Utilities
    private static Texture2D _texture;
    private static Texture2D GetWhiteTexture()
    {
        if (_texture == null)
        {
            _texture = new Texture2D(1, 1);
            _texture.SetPixel(0, 0, Color.white);
            _texture.Apply();
        }
        return _texture;
    }

    public static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Top
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    public static Rect GetScreenRect(Vector2 screenPosition1, Vector2 screenPosition2)
    {
        // Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        var topLeft = Vector2.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector2.Max(screenPosition1, screenPosition2);
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }
    #endregion
}
