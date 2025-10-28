using System.Collections.Generic;
using UnityEngine;

/*

This makes it easy to add simple effects throughout the project.
Each effect will be shown for one frame.

Features
-Lines (Thickness, Color)
-Square (Size, Color)
-Hollow Square (Size, Line Thickness, Color)
//-Circle (Radius, Color)

All effects are made child objects so the unity hierarchy is not too cluttered.

*/

// Should move all the effects to the Update loop rather than FixedUpdate through out the project for consistency
// And keep the logic itself in FixedUpdate
public class EffectsRenderer : MonoBehaviour
{
    private Material _defaultMaterial;
    [SerializeField] private Sprite _squareSprite; // Just unity's buildin square sprite
    [SerializeField] private GameObject _squarePrefab;

    private List<GameObject> _activeEffects = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _defaultMaterial = new Material(Shader.Find("Sprites/Default"));
    }

    // Updated before all other objects (hopefully) its a bit janky but works for now
    void FixedUpdate()
    {
        // Clear old effects
        foreach (var effect in _activeEffects)
        {
            if (effect != null)
                Destroy(effect);
        }
        _activeEffects.Clear();
    }

    //// Updated after all other normal objects updates have had a chance to add effects
    //void LateUpdate()
    //{

    //}

    public void DrawLine(Vector2 from, Vector2 to, Color color, float thickness = 0.1f)
    {
        GameObject lineObj = new GameObject("Effect_Line");
        lineObj.transform.parent = transform;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = _defaultMaterial;
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = thickness;
        lr.endWidth = thickness;
        lr.positionCount = 2;
        lr.SetPosition(0, new Vector3(from.x, from.y, 0));
        lr.SetPosition(1, new Vector3(to.x, to.y, 0));
        lr.sortingOrder = 11; // Make sure it's visible above ground

        _activeEffects.Add(lineObj);
    }

    public void DrawSquare(Vector2 center, float size, Color color)
    {
        DrawSquare(center, new Vector2(size, size), color);
    }

    // No working sadly
    public void DrawSquare(Vector2 center, Vector2 size, Color color)
    {
        GameObject squareObject = new GameObject("Effect_Square");
        squareObject.transform.parent = transform;

        SpriteRenderer sr = squareObject.AddComponent<SpriteRenderer>();
        sr.material = _defaultMaterial;
        sr.color = color;
        sr.sortingOrder = 10;
        sr.sprite = _squareSprite;

        // Position and scale
        squareObject.transform.position = new Vector3(center.x, center.y, 0);
        squareObject.transform.localScale = new Vector3(size.x, size.y, 1);

        _activeEffects.Add(squareObject);
    }

    public void DrawHollowSquare(Vector2 p, float size, float thickness, Color color)
    {
        Vector2 NW = new Vector2(p.x - size / 2, p.y + size / 2);
        Vector2 NE = new Vector2(p.x + size / 2, p.y + size / 2);
        Vector2 SW = new Vector2(p.x - size / 2, p.y - size / 2);
        Vector2 SE = new Vector2(p.x + size / 2, p.y - size / 2);
        DrawLine(NW, NE, color, thickness);
        DrawLine(NE, SE, color, thickness);
        DrawLine(SE, SW, color, thickness);
        DrawLine(SW, NW, color, thickness);
    }

    //public void DrawCircle(Vector2 p, float radius, Color color)
    //{
    //    GameObject circleObject = new GameObject("Effect_Circle");
    //    circleObject.transform.parent = transform;

    //    SpriteRenderer sr = circleObject.AddComponent<SpriteRenderer>();
    //}
}
