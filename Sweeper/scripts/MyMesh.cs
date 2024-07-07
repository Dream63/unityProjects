using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

using UnityEngine.UIElements;

public class MyMesh 
{
    int width;
    int height;
    float tileSize;
    Vector3 startPos;
    public Vector2[] uv;
    public Vector3[] vertices;
    public int[] triangles;
    public Mesh myMesh = new Mesh();

    public MyMesh(int width1, int height1, float tileSize1, GameObject meshObject, Vector3 startPos1)
    {
        this.width = width1; 
        this.height = height1;
        this.tileSize = tileSize1;
        this.startPos = startPos1;
        if(width*height*4 > 64000)
            myMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        vertices =  new Vector3[4 * (height * width)];
        uv = new Vector2[4 * (height * width)];
        triangles = new int[6 * (height * width)];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int index = i * height + j; 

                vertices[index * 4 + 0] = new Vector3(tileSize * i,       tileSize * j      );
                vertices[index * 4 + 1] = new Vector3(tileSize * i,       tileSize * (j + 1));
                vertices[index * 4 + 2] = new Vector3(tileSize * (i + 1), tileSize * (j + 1));
                vertices[index * 4 + 3] = new Vector3(tileSize * (i + 1), tileSize * j      );

                uv[index * 4 + 0] = new Vector2(0, 0);
                uv[index * 4 + 1] = new Vector2(0, 1);
                uv[index * 4 + 2] = new Vector2(1, 1);
                uv[index * 4 + 3] = new Vector2(1, 0);

                triangles[index * 6 + 0] = index * 4 + 0;
                triangles[index * 6 + 1] = index * 4 + 1;
                triangles[index * 6 + 2] = index * 4 + 2;
                triangles[index * 6 + 3] = index * 4 + 0;
                triangles[index * 6 + 4] = index * 4 + 2;
                triangles[index * 6 + 5] = index * 4 + 3;
            }
        }
        
        // Code storing
        if(false)
        {
        //vertices[index * 4 + 0] = new Vector3(tileSize * i, tileSize * j);
        //vertices[index * 4 + 1] = new Vector3(tileSize * i, tileSize * (j + 1));
        //vertices[index * 4 + 2] = new Vector3(tileSize * (i + 1), tileSize * (j + 1));
        //vertices[index * 4 + 3] = new Vector3(tileSize * (i + 1), tileSize * j);

        //uv[index * 4 + 0] = new Vector2(0, 0);
        //uv[index * 4 + 1] = new Vector2(0, 1);
        //uv[index * 4 + 2] = new Vector2(1, 1);
        //uv[index * 4 + 3] = new Vector2(1, 0);

        //triangles[index * 6 + 0] = index * 4 + 0;
        //triangles[index * 6 + 1] = index * 4 + 1;
        //triangles[index * 6 + 2] = index * 4 + 2;
        //triangles[index * 6 + 3] = index * 4 + 0;
        //triangles[index * 6 + 4] = index * 4 + 2;
        //triangles[index * 6 + 5] = index * 4 + 3;


        // vertices[0] = new Vector3(0, 0);
        // vertices[1] = new Vector3(0, 1);
        // vertices[2] = new Vector3(1, 1);
        // vertices[3] = new Vector3(1, 0);

        // uv[0] = new Vector2(0, 0);
        // uv[1] = new Vector2(0, 1);
        // uv[2] = new Vector2(1, 1);
        // uv[3] = new Vector2(1, 0);

        // triangles[0] = 0;
        // triangles[1] = 1;
        // triangles[2] = 2;
        // triangles[3] = 0;
        // triangles[4] = 2;
        // triangles[5] = 3;
        }
        
        myMesh.vertices = vertices;
        myMesh.uv = uv;
        myMesh.triangles = triangles;

        meshObject.transform.position = startPos;
        meshObject.GetComponent<MeshFilter>().mesh = myMesh;
    }

    public void MeshUvUpdate(Vector2 value00, Vector2 value11, int index)
    {
        uv[index * 4 + 0] = new Vector2(value00.x, value00.y);
        uv[index * 4 + 1] = new Vector2(value00.x, value11.y);
        uv[index * 4 + 2] = new Vector2(value11.x, value11.y);
        uv[index * 4 + 3] = new Vector2(value11.x, value00.y);
        myMesh.uv = uv;

    }
}
