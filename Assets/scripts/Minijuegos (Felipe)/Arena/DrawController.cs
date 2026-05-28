//En la línea 51 hay un emulador de toque con mouse

using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawController : MonoBehaviour
{
    [Header("Trazo")]
    public float lineWidth = 0.05f;
    public Color lineColor = Color.white;

    [Header("Sensibilidad")]
    public float minPointDistance = 3f;
    public int minPoints = 1;

    public event Action<ShapeType> OnShapeDrawn;

    public bool IsEnabled { get; set; } = true;

    private LineRenderer lr;
    private readonly List<Vector2> points = new List<Vector2>();
    private bool drawing = false;
    private Camera cam;

    void Awake()
    {
        cam = Camera.main;

        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.startColor = lineColor;
        lr.endColor = lineColor;
        lr.positionCount = 0;

        if (lr.material == null || lr.material.shader.name.Contains("Default-Line") == false)
            lr.material = new Material(Shader.Find("Sprites/Default"));
    }

    void Update()
    {
        if (!IsEnabled) return;
        HandleTouch();
    }

    void HandleTouch()
    {
        // -------- TOUCH REAL PARA CELULAR --------
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            switch (t.phase)
            {
                case TouchPhase.Began:
                    BeginDraw(t.position);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:

                    if (drawing)
                    {
                        AppendPoint(t.position);

                        // Detecta apenas haya movimiento suficiente
                        if (points.Count >= 2)
                        {
                            ShapeType shape = Scanner.Recognize(points);
                            OnShapeDrawn?.Invoke(shape);

                            // Reinicia inmediatamente
                            points.Clear();
                            lr.positionCount = 0;

                            AddToLine(t.position);
                        }
                    }

                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (drawing)
                        FinishDraw();
                    break;
            }

            return;
        }

        // -------- MOUSE SOLO PARA EDITOR / PC --------
    #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            BeginDraw(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0) && drawing)
        {
            AppendPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0) && drawing)
        {
            FinishDraw();
        }
    #endif
    }

    void BeginDraw(Vector3 screenPos)
    {
        drawing = true;
        points.Clear();
        lr.positionCount = 0;
        AddToLine(screenPos);
    }

    void AppendPoint(Vector3 screenPos)
{
    Vector2 wp = ToWorld(screenPos);

    if (points.Count == 0)
    {
        AddToLine(screenPos);
        return;
    }

    float distance = Vector2.Distance(wp, points[points.Count - 1]);

    // Mucho más sensible para móvil
    if (distance >= 0.005f)
    {
        AddToLine(screenPos);
    }
}

    void FinishDraw()
    {
        drawing = false;
        lr.positionCount = 0;

        if (points.Count >= minPoints)
        {
            ShapeType shape = Scanner.Recognize(points);
            OnShapeDrawn?.Invoke(shape);
        }

        points.Clear();
    }

    void AddToLine(Vector3 screenPos)
    {
        Vector2 wp = ToWorld(screenPos);
        points.Add(wp);
        lr.positionCount = points.Count;
        lr.SetPosition(points.Count - 1, new Vector3(wp.x, wp.y, 0f));
    }

    Vector2 ToWorld(Vector3 screenPos)
{
    return screenPos;
}
}

