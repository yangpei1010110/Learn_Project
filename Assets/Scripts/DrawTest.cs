using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DrawTest : MonoBehaviour
{
    private HashSet<Vector2Int> _changedPixels = new();

    private int[,]              _particleMap;
    private HashSet<Vector2Int> _sandPixels = new();
    private SpriteRenderer      _spriteRenderer;
    private Texture2D           _texture2D;

    // Start is called before the first frame update
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _texture2D = _spriteRenderer.sprite.texture;
        _texture2D = new Texture2D(_texture2D.width, _texture2D.height);
        _texture2D.filterMode = FilterMode.Point;
        _texture2D.wrapMode = TextureWrapMode.Clamp;
        _spriteRenderer.sprite = Sprite.Create(_texture2D, new Rect(0, 0, _texture2D.width, _texture2D.height), 0.5f * Vector2.one, _texture2D.height);
        _particleMap = new int[_texture2D.width, _texture2D.height];

        for (int j = 0; j < _texture2D.height; j++)
        {
            for (int i = 0; i < _texture2D.width; i++)
            {
                _texture2D.SetPixel(i, j, Color.black);
            }
        }

        _texture2D.Apply();
        _changedPixels.Clear();
        _sandPixels.Clear();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateInit();
        UpdateInput();
        UpdateParticles();
        UpdateMapSprite();
    }

    private void UpdateInit()
    {
        _changedPixels.Clear();
    }

    private void UpdateInput()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 thisPosition = transform.position;

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition -= (Vector2)thisPosition;
            mousePosition += Vector2.one * 0.5f;


            Vector2Int pixelPosition = new((int)(mousePosition.x * _texture2D.width), (int)(mousePosition.y * _texture2D.height));
            if (InMapRange(pixelPosition))
            {
                _sandPixels.Add(pixelPosition);
            }
        }
    }

    private void UpdateParticles()
    {
        foreach (Vector2Int sandPixel in _sandPixels.ToArray())
        {
            UpdateSand(sandPixel);
        }
    }

    private void UpdateSand(Vector2Int sandPos)
    {
        // if down is empty then move down
        Vector2Int downPos = sandPos + Vector2Int.down;
        if (!_sandPixels.Contains(downPos) && InMapRange(downPos))
        {
            _sandPixels.Remove(sandPos);
            _sandPixels.Add(downPos);
            _changedPixels.Add(sandPos);
            _changedPixels.Add(downPos);
            _particleMap[downPos.x, downPos.y] = 1;
            _particleMap[sandPos.x, sandPos.y] = 0;
            return;
        }

        // else if down left is empty then move down left
        Vector2Int downLeftPos = sandPos + Vector2Int.down + Vector2Int.left;
        if (!_sandPixels.Contains(downLeftPos) && InMapRange(downLeftPos))
        {
            _sandPixels.Remove(sandPos);
            _sandPixels.Add(downLeftPos);
            _changedPixels.Add(sandPos);
            _changedPixels.Add(downLeftPos);
            _particleMap[downLeftPos.x, downLeftPos.y] = 1;
            _particleMap[sandPos.x, sandPos.y] = 0;
            return;
        }

        // else if down right is empty then move down right
        Vector2Int downRightPos = sandPos + Vector2Int.down + Vector2Int.right;
        if (!_sandPixels.Contains(downRightPos) && InMapRange(downRightPos))
        {
            _sandPixels.Remove(sandPos);
            _sandPixels.Add(downRightPos);
            _changedPixels.Add(sandPos);
            _changedPixels.Add(downRightPos);
            _particleMap[downRightPos.x, downRightPos.y] = 1;
            _particleMap[sandPos.x, sandPos.y] = 0;
            return;
        }
    }

    private void UpdateMapSprite()
    {
        foreach (Vector2Int changedPixel in _changedPixels)
        {
            Color particleColor = _particleMap[changedPixel.x, changedPixel.y] == 1 ? Color.yellow : Color.black;
            _texture2D.SetPixel(changedPixel.x, changedPixel.y, particleColor);
        }

        _texture2D.Apply();
    }

    private bool InMapRange(Vector2Int pos) =>
        pos.x >= 0 && pos.x < _particleMap.GetLength(0) &&
        pos.y >= 0 && pos.y < _particleMap.GetLength(1);
}