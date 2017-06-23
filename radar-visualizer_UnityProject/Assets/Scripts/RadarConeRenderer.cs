using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RadarConeRenderer : MonoBehaviour
{
    #region public serialised vars
    [SerializeField]
    int _horizontalDivisions;
    #endregion


    #region private protected vars
    #endregion


    #region pub methods
    #endregion


    #region private protected methods
    public void GenerateCone(Vector2 angles, Vector2 rotation)
    {
        MeshFilter mFilter = GetComponent<MeshFilter>();
        Mesh mesh = mFilter.mesh;
        if (mesh == null)
            mesh = new Mesh();

        mesh.MarkDynamic();

        // VERTICIES and uvs
        List<Vector3> verticies = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        verticies.Add(Vector3.zero); // origin
        uvs.Add(new Vector2(0.5f, 0.5f));
        
        Vector2 uvRB = new Vector2(1, 0);
        Vector2 uvLB = new Vector2(0, 0);
        Vector2 uvLT = new Vector2(0, 1);
        Vector2 uvRT = new Vector2(1, 1);

        for (int i = -_horizontalDivisions/2, j = 0; i <= _horizontalDivisions/2; i++, j++)
        {
            float t = i / (float)_horizontalDivisions;
            float tuv = j / (float)_horizontalDivisions;
            Vector3 top = Constants.GetPoint(80f, new Vector2(angles.x * t + rotation.x, angles.y * 0.5f + rotation.y));
            Vector3 bot = Constants.GetPoint(80f, new Vector2(angles.x * t + rotation.x, angles.y * -0.5f + rotation.y));
            verticies.Add(top);
            verticies.Add(bot);

            uvs.Add(Vector2.Lerp(uvLT, uvRT, tuv));
            uvs.Add(Vector2.Lerp(uvLB, uvRB, tuv));
        }        
        /*
        for (int i = 0; i < verticies.Count; i++)
            Debug.DrawLine(Vector3.zero, verticies[i], Color.cyan, 2f);
            */
        mesh.SetVertices(verticies);
        mesh.SetUVs(0, uvs);

        // TRIANGLES
        //int[] tris = new int[2 + _horizontalDivisions * 2 + _horizontalDivisions + _horizontalDivisions];
        int indiciesFront = _horizontalDivisions * 6 + 6;
        int indiciesSides = 6;
        int indiciesTop = _horizontalDivisions * 3;
        int[] tris = new int[indiciesFront + indiciesSides + indiciesTop*2];
        
        // front; 0 -> d * 6 + 6
        for (int i = 0; i < _horizontalDivisions; i++)
        {
            tris[i * 6 + 0] = i*2 + 1; // tris[0] = 1;      tris[6] = 3;
            tris[i * 6 + 1] = i*2 + 2; // tris[1] = 2;      tris[7] = 4;
            tris[i * 6 + 2] = i*2 + 4; // tris[2] = 4;      tris[8] = 6;

            tris[i * 6 + 3] = i*2 + 1; // tris[3] = 1;      tris[9] = 3;
            tris[i * 6 + 4] = i*2 + 4; // tris[4] = 4;      tris[10]= 6;
            tris[i * 6 + 5] = i*2 + 3; // tris[5] = 3;      tris[11]= 5;
        }
        
        //sides
        int lastIndx = indiciesFront;
        tris[lastIndx++] = 1;
        tris[lastIndx++] = 0;
        tris[lastIndx++] = 2;

        tris[lastIndx++] = _horizontalDivisions * 2 + 2;
        tris[lastIndx++] = 0;
        tris[lastIndx++] = _horizontalDivisions * 2 + 1;

        //top
        for (int i = 0; i < _horizontalDivisions; i++)
        {
            tris[lastIndx + i * 3 + 0] = i * 2 + 3;
            tris[lastIndx + i * 3 + 1] = 0;
            tris[lastIndx + i * 3 + 2] = i * 2 + 1;
        }
        lastIndx += indiciesTop;

        //bottom
        for (int i = 0; i < _horizontalDivisions; i++)
        {
            tris[lastIndx + i * 3 + 0] = i * 2 + 2;
            tris[lastIndx + i * 3 + 1] = 0;
            tris[lastIndx + i * 3 + 2] = i * 2 + 4;
        }
        lastIndx += indiciesTop;

        mesh.SetTriangles(tris, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();

        mFilter.mesh = mesh;
    }
    #endregion


    #region events
    #endregion


    #region mono events
    #endregion
}
