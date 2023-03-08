using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Solver;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Tilemaps;

public class TileGridSpawn : MonoBehaviour
{
    public int Width;
    public int Height;
    public GameObject TilePrefab;
    public List<SpriteDescriptor> Descriptors;
    private List<SpriteDescriptor> _extendedDescriptors = new();
    private GameObject[,] _goGrid;
    private Game _game;
    private int _tileCount;
    private int _namingIndex;

    void Start()
    {
        InitGoGrid();
        GenerateSymmetries();
        _game = new Game(Width, Height, _extendedDescriptors, _goGrid);
        _tileCount = Width * Height;
    }

    private void Update()   
    {
        if (_game.GameMap.EntropyMap[1].Count < _tileCount) {
            _game.Step();
        }
    }

    void InitGoGrid()
    {
        _goGrid = new GameObject[Width,Height];

        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                Vector3 pos = new Vector3(i, j, 0);
                _goGrid[i,j] = Instantiate(TilePrefab, pos, TilePrefab.transform.rotation);
            }
        }
    }

    void GenerateSymmetries()
    {
        foreach (var spriteDescriptor in Descriptors)
        {
            SpriteDescriptor referenceDescriptor = spriteDescriptor;
            _extendedDescriptors.Add(referenceDescriptor);
            
            for (int i = 0; i < GetCycleNumber(spriteDescriptor.SymmetryType); i++)
            {
                for (int j = 0; j < GetRotationPerCycle(spriteDescriptor.SymmetryType); j++)
                {
                    SpriteDescriptor symmetryDescriptor = CopySpriteDescriptorWithTexture(referenceDescriptor, 
                        RotateTexture(referenceDescriptor.Sprite.texture, true));
                    symmetryDescriptor.Down = referenceDescriptor.Right;
                    symmetryDescriptor.Left = referenceDescriptor.Down;
                    symmetryDescriptor.Up = referenceDescriptor.Left;
                    symmetryDescriptor.Right = referenceDescriptor.Up;
                    referenceDescriptor = symmetryDescriptor;
                }

                _extendedDescriptors.Add(referenceDescriptor);
            }
        }
    }
    
    Texture2D RotateTexture(Texture2D originalTexture, bool clockwise)
    {
        Color[] original = originalTexture.GetPixels();
        Color[] rotated = new Color[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;

        int iRotated, iOriginal;
 
        for (int j = 0; j < h; j++)
        {
            for (int i = 0; i < w; i++)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }

        Texture2D rotatedTexture = new Texture2D(w, h);
        rotatedTexture.SetPixels(rotated);
        rotatedTexture.Apply();
        rotatedTexture.name = _namingIndex.ToString();
        return rotatedTexture;
    }

    int GetCycleNumber(SymmetryType symmetryType)
    {
        switch (symmetryType)
        {
            case SymmetryType.X:
                return 0;
            case SymmetryType.T:
                return 3;
            case SymmetryType.Line:
                return 1;
            case SymmetryType.Corner:
                return 3;
        }

        return 0;
    }
    
    int GetRotationPerCycle(SymmetryType symmetryType)
    {
        switch (symmetryType)
        {
            case SymmetryType.X:
                return 0;
            case SymmetryType.T:
                return 1;
            case SymmetryType.Line:
                return 1;
            case SymmetryType.Corner:
                return 1;
        }

        return 0;
    }

    SpriteDescriptor CopySpriteDescriptorWithTexture(SpriteDescriptor spriteDescriptor, Texture2D texture2D)
    {
        var newSpriteDescriptor = ScriptableObject.CreateInstance<SpriteDescriptor>();
        newSpriteDescriptor.Down = spriteDescriptor.Down;
        newSpriteDescriptor.Up = spriteDescriptor.Up;
        newSpriteDescriptor.Right = spriteDescriptor.Right;
        newSpriteDescriptor.Left = spriteDescriptor.Left;
        newSpriteDescriptor.SymmetryType = spriteDescriptor.SymmetryType;
        newSpriteDescriptor.Sprite = CopySpriteWithTexture(spriteDescriptor.Sprite, texture2D);
        return newSpriteDescriptor;
    }
    
    Sprite CopySpriteWithTexture(Sprite sprite, Texture2D texture2D)
    {
        Sprite newSprite = Sprite.Create(texture2D, sprite.rect, new Vector2(0.5f, 0.5f), sprite.pixelsPerUnit, 1, SpriteMeshType.Tight);
        newSprite.name = _namingIndex.ToString();
        _namingIndex++;
        return newSprite;
    }
}
