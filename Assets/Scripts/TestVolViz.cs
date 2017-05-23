using UnityEngine;
using System.Collections;

public class TestVolViz : MonoBehaviour {
	public SimulateSpotLight light;
	public Material mat;
	// Use this for initialization
	void Start () {
		var mf = gameObject.AddComponent<MeshFilter>();
		var mr = gameObject.AddComponent<MeshRenderer>();

		var numShells = 20;
		var numRows = 1;
		var numColnums = 1;

		var numTris = numShells * numRows * numColnums * 2;

		var vertsPerShell = (numRows + 1) * (numColnums + 1);
		int vertsPerRow = numColnums + 1;

		var vertices = (numRows + 1) * (numColnums + 1) * numShells;
		var indexList = new int[numTris * 3];
		var curIndex = 0;

		for(var i = 0; i < numShells; i++)
		{
			for(var row = 0; row < numRows; row++)
			{
				for(var col = 0; col < numColnums; col++)
				{
					indexList[curIndex++] = i * vertsPerShell + row * vertsPerRow + col;
					indexList[curIndex++] = i * vertsPerShell + row * vertsPerRow + col + 1;
					indexList[curIndex++] = i * vertsPerShell + (row + 1) * vertsPerRow + col;

					indexList[curIndex++] = i * vertsPerShell + (row + 1) * vertsPerRow + col;
					indexList[curIndex++] = i * vertsPerShell + row * vertsPerRow + col + 1;
					indexList[curIndex++] = i * vertsPerShell + (row + 1) * vertsPerRow + col + 1;
				}
			}
		}

		var verticeList = new Vector3[vertices];
		curIndex = 0;
		for(var shell = 0; shell < numShells; shell++)
		{
			for(var row = 0; row <= numRows; row++)
			{
				for(var col = 0; col <= numColnums; col++)
				{
					var fx = (float)col / (float)numColnums;
					var fy = (float)row / (float)numRows;
					var fz = (float)shell / (float)(numShells - 1);

					verticeList[curIndex] = new Vector3(fx, fy, fz);

					curIndex ++;
				}
			}
		}

		mf.mesh.vertices = verticeList;
		mf.mesh.triangles = indexList;

		mr.material = mat;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		mat.SetVector("vMinBound", light.vMinBound);
		mat.SetVector("vMaxBound", light.vMaxBound);
	}
}
