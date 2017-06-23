using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Cursor3DRenderer : MonoBehaviour
{
    #region public serialised vars
    [SerializeField]
    MeshFilter _meshFilter;
    #endregion


    #region private protected vars
    #endregion


    #region pub methods
    #endregion


    #region private protected methods
    [ContextMenu("Generate")]
    void Generate()
    {
        MeshFilter mFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        //float angle = Constants.RadarConfig.LRSRadarConeAngles.y;
        float angle = 60f;
        
        List<Vector3> verticies = new List<Vector3>();
        List<Color> colors = new List<Color>();
        int[] indicies = new int[13];
        for (float a = -angle/2f, i = 0; a <= angle/2f; a += angle/12f, i++)
        {
            float radA = Mathf.Deg2Rad * a;
            Vector3 pos = new Vector3(0f, Mathf.Sin(radA), Mathf.Cos(radA) - 1f);
            verticies.Add(pos);
            indicies[(int)i] = (int)i;
            colors.Add(Color.green);
        }

        mesh.SetVertices(verticies);
        mesh.SetColors(colors);

        mesh.SetIndices(indicies, MeshTopology.LineStrip, 0, true);

        mFilter.mesh = mesh;
    }
    #endregion


    #region events
    #endregion


    #region mono events
    #endregion
}
