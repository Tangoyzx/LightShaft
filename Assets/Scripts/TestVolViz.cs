using UnityEngine;
using System.Collections;

public class TestVolViz : MonoBehaviour {
	public SimulateSpotLight light;
	public RenderTexture shadowMap;
	public Material mat;
	public Camera viewer;
	public Transform debugCube;
	// Use this for initialization
	void Start () {
		InitLightZBuffer();
		var mf = gameObject.AddComponent<MeshFilter>();
		var mr = gameObject.AddComponent<MeshRenderer>();

		var numShells = 100;
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
		
		mat.SetMatrix("worldToViewMat", viewer.transform.worldToLocalMatrix);
		mat.SetMatrix("viewToWorldMat", viewer.transform.localToWorldMatrix);
		mat.SetMatrix("worldToLightMat", light.transform.worldToLocalMatrix);
		mat.SetMatrix("lightToWorldMat", light.transform.localToWorldMatrix);
		mat.SetMatrix("lightProjectionMat", light.shadowCamera.projectionMatrix);
	}

	private void InitLightZBuffer() {
		var zc1 = light.far/light.near;
		var zc0 = 1.0f - zc1;
		var zc2 = zc0 / light.far;
		var zc3 = zc1 / light.far;

		var vv = new Vector4(zc0, zc1, zc2, zc3);
		Debug.Log("zb: " + vv);
		mat.SetVector("_LightZBufferParams", vv);
	}
}
