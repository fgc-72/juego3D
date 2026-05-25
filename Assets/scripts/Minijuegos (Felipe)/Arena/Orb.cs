using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Orb : MonoBehaviour
{
    public TextMeshProUGUI shapeText; // era TextMeshPro, ahora UI

    public enum MineralType { None, Rubi, Esmeralda, Cuarzo, LapisLazuli, Amatista, Zafiro, Diamante }

    private static readonly Dictionary<ShapeType, string> Symbols = new()
    {
        { ShapeType.Up,    "↑" },
        { ShapeType.Down,  "↓" },
        { ShapeType.Left,  "←" },
        { ShapeType.Right, "→" },
    };

    private static readonly Dictionary<MineralType, string> MineralColors = new()
    {
        { MineralType.Rubi,        "#E0115F" },
        { MineralType.Esmeralda,   "#50C878" },
        { MineralType.Cuarzo,      "#E6E6FA" },
        { MineralType.LapisLazuli, "#26619C" },
        { MineralType.Amatista,    "#9966CC" },
        { MineralType.Zafiro,      "#0F52BA" },
        { MineralType.Diamante,    "#B9F2FF" },
    };

    private Queue<ShapeType> queue = new Queue<ShapeType>();
    private float speed;
    private bool active = false;
    private RectTransform rt;

    public MineralType mineralType { get; private set; } = MineralType.None;
    public bool IsMineral => mineralType != MineralType.None;

    public event Action<Orb> OnReachedCenter;
    public event Action<Orb> OnCompleted;

    public ShapeType CurrentShape => queue.Count > 0 ? queue.Peek() : ShapeType.Down;

    void Awake() => rt = GetComponent<RectTransform>();

    public void Initialize(List<ShapeType> shapes, float moveSpeed, MineralType mineral = MineralType.None)
    {
        mineralType = mineral;
        queue.Clear();
        foreach (var s in shapes) queue.Enqueue(s);
        speed  = moveSpeed;
        active = true;
        RefreshDisplay();
    }

    void Update()
    {
        if (!active) return;

        rt.anchoredPosition = Vector2.MoveTowards(
            rt.anchoredPosition, Vector2.zero, speed * 150f * Time.deltaTime);

        if (rt.anchoredPosition.magnitude < 30f)
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

        string baseColor = IsMineral ? MineralColors[mineralType] : "#FFE84D";
        string dimColor  = IsMineral ? MineralColors[mineralType] : "#3a3a3a";

        for (int i = 0; i < arr.Length; i++)
        {
            string color = i == 0 ? baseColor : dimColor;
            float  scale = i == 0 ? 1.4f : 1.0f;
            sb.Append($"<color={color}><size={scale}em>{Symbols[arr[i]]}</size></color> ");
        }

        shapeText.text = sb.ToString().TrimEnd();
    }
}