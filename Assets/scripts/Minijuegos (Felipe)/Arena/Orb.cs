using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Orb : MonoBehaviour
{
    public TextMeshPro shapeText;

    private static readonly Dictionary<ShapeType, string> Symbols = new()
    {
        { ShapeType.Up,   "↑" },
        { ShapeType.Down, "↓" },
        { ShapeType.Left,  "←" },
        { ShapeType.Right, "→" },
    };

    private Queue<ShapeType> queue = new Queue<ShapeType>();
    private float speed;
    private bool active = false;

    public event Action<Orb> OnReachedCenter;
    public event Action<Orb> OnCompleted;

    public ShapeType CurrentShape => queue.Count > 0 ? queue.Peek() : ShapeType.Down;

    public void Initialize(List<ShapeType> shapes, float moveSpeed)
    {
        queue.Clear();
        foreach (var s in shapes) queue.Enqueue(s);
        speed  = moveSpeed;
        active = true;
        RefreshDisplay();
    }

    void Update()
    {
        if (!active) return;

        transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, speed * Time.deltaTime);

        if (transform.position.magnitude < 0.25f)
        {
            active = false;
            OnReachedCenter?.Invoke(this);
        }
    }

    public bool TryMatch(ShapeType drawnShape)
    {
        if (!active || queue.Count == 0) return false;
        if (queue.Peek() != drawnShape)  return false;

        queue.Dequeue();

        if (queue.Count == 0)
        {
            active = false;
            OnCompleted?.Invoke(this);
        }
        else
        {
            RefreshDisplay();
        }
        return true;
    }

    void RefreshDisplay()
    {
        if (shapeText == null) return;

        ShapeType[] arr = queue.ToArray();
        System.Text.StringBuilder sb = new();

        for (int i = 0; i < arr.Length; i++)
        {
            string color  = i == 0 ? "#FFE84D" : "#AAAAAA";
            float  scale  = i == 0 ? 1.4f : 1.0f;
            sb.Append($"<color={color}><size={scale}em>{Symbols[arr[i]]}</size></color> ");
        }

        shapeText.text = sb.ToString().TrimEnd();
    }
}