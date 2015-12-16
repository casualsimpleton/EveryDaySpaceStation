//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// BoundsDrawing - A GL Line drawing script, based off
//https://www.assetstore.unity3d.com/en/#!/content/10962
// Created: December 7 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 7 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

public class BoundsDrawing : MonoBehaviour
{
    public static Material LineVertMaterial;
    public Transform _transform { get; private set; }
    [SerializeField]
    protected Color32 _lineColor;
    List<Vector3> _verts;

    public Color32 LineColor { get { return _lineColor; } set { _lineColor = value; } }

    public void Create()
    {
        if (_transform == null)
        {
            _transform = this.transform;
        }

        if (LineVertMaterial == null)
        {
            LineVertMaterial = new Material(DefaultFiles.Singleton.lineDrawingShader);
        }

        if (_verts == null)
        {
            _verts = new List<Vector3>();
        }
        else
        {
            _verts.Clear();
        }
    }

    public void Reset()
    {
        _verts.Clear();

        _transform.parent = PoolManager.Singleton._transform;
        
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Draws a square
    /// </summary>
    /// <param name="pivotPoint">Anchor point of the object (should be transform.position)</param>
    /// <param name="size">Size</param>
    public void DrawSquare(Vector3 pivotPoint, Vector3 size)
    {
        //12 lines, 2 points each
        //BOTTOM
        //Bottom Z+ edge
        _verts.Add(new Vector3(pivotPoint.x - (size.x * 0.5f), 0f, pivotPoint.z + (size.z * 0.5f)));
        _verts.Add(new Vector3(pivotPoint.x + (size.x * 0.5f), 0f, pivotPoint.z + (size.z * 0.5f)));

        //Right X+ edge
        _verts.Add(new Vector3(pivotPoint.x + (size.x * 0.5f), 0f, pivotPoint.z + (size.z * 0.5f)));
        _verts.Add(new Vector3(pivotPoint.x + (size.x * 0.5f), 0f, pivotPoint.z - (size.z * 0.5f)));

        //Back edge Z-
        _verts.Add(new Vector3(pivotPoint.x + (size.x * 0.5f), 0f, pivotPoint.z - (size.z * 0.5f)));
        _verts.Add(new Vector3(pivotPoint.x - (size.x * 0.5f), 0f, pivotPoint.z - (size.z * 0.5f)));

        //Left Edge Z-
        _verts.Add(new Vector3(pivotPoint.x - (size.x * 0.5f), 0f, pivotPoint.z - (size.z * 0.5f)));
        _verts.Add(new Vector3(pivotPoint.x - (size.x * 0.5f), 0f, pivotPoint.z + (size.z * 0.5f)));

        //TOP
        //Bottom Z+ edge
        _verts.Add(new Vector3(pivotPoint.x - (size.x * 0.5f), size.y, pivotPoint.z + (size.z * 0.5f)));
        _verts.Add(new Vector3(pivotPoint.x + (size.x * 0.5f), size.y, pivotPoint.z + (size.z * 0.5f)));

        //Right X+ edge
        _verts.Add(new Vector3(pivotPoint.x + (size.x * 0.5f), size.y, pivotPoint.z + (size.z * 0.5f)));
        _verts.Add(new Vector3(pivotPoint.x + (size.x * 0.5f), size.y, pivotPoint.z - (size.z * 0.5f)));

        //Back edge Z-
        _verts.Add(new Vector3(pivotPoint.x + (size.x * 0.5f), size.y, pivotPoint.z - (size.z * 0.5f)));
        _verts.Add(new Vector3(pivotPoint.x - (size.x * 0.5f), size.y, pivotPoint.z - (size.z * 0.5f)));

        //Left Edge Z-
        _verts.Add(new Vector3(pivotPoint.x - (size.x * 0.5f), size.y, pivotPoint.z - (size.z * 0.5f)));
        _verts.Add(new Vector3(pivotPoint.x - (size.x * 0.5f), size.y, pivotPoint.z + (size.z * 0.5f)));

        //Vertical Lines
        _verts.Add(new Vector3(pivotPoint.x - (size.x * 0.5f), 0f, pivotPoint.z + (size.z * 0.5f)));
        _verts.Add(new Vector3(pivotPoint.x - (size.x * 0.5f), size.y, pivotPoint.z + (size.z * 0.5f)));

        _verts.Add(new Vector3(pivotPoint.x + (size.x * 0.5f), 0f, pivotPoint.z + (size.z * 0.5f)));
        _verts.Add(new Vector3(pivotPoint.x + (size.x * 0.5f), size.y, pivotPoint.z + (size.z * 0.5f)));

        _verts.Add(new Vector3(pivotPoint.x + (size.x * 0.5f), 0f, pivotPoint.z - (size.z * 0.5f)));
        _verts.Add(new Vector3(pivotPoint.x + (size.x * 0.5f), size.y, pivotPoint.z - (size.z * 0.5f)));

        _verts.Add(new Vector3(pivotPoint.x - (size.x * 0.5f), 0f, pivotPoint.z - (size.z * 0.5f)));
        _verts.Add(new Vector3(pivotPoint.x - (size.x * 0.5f), size.y, pivotPoint.z - (size.z * 0.5f)));
    }

    void OnPostRender()
    {
        LineVertMaterial.SetPass(0);

        GL.Begin(GL.LINES);
        GL.Color(_lineColor);
        for (int i = 0; i < _verts.Count; i+=2)
        {
            GL.Vertex(_verts[i]);
            GL.Vertex(_verts[i+1]);
        }

        GL.End();
    }
}
