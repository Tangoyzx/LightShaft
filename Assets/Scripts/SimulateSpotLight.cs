using UnityEngine;
using System.Collections;

public class SimulateSpotLight : MonoBehaviour {
	public float fieldOfView = 60.0f;
	public float aspect = 720.0f / 1280.0f;
	public float near = 1;
	public float far = 50;

	[HideInInspector]
	public Vector3 vMinBound;

	[HideInInspector]
	public Vector3 vMaxBound;

	private Vector4[] vertices;

	void Awake()
	{
		var nearWidth = near * Mathf.Tan(fieldOfView * 0.5f * Mathf.Deg2Rad);
		var farWidth = nearWidth * far / near;

		var nearHeight = nearWidth * aspect;
		var farHeight = farWidth * aspect;

		vertices = new Vector4[8];
		vertices[0] = new Vector4(-nearWidth, nearHeight, near, 1);
		vertices[1] = new Vector4(nearWidth, nearHeight, near, 1);
		vertices[2] = new Vector4(nearWidth, -nearHeight, near, 1);
		vertices[3] = new Vector4(-nearWidth, -nearHeight, near, 1);

		vertices[4] = new Vector4(-farWidth, farHeight, far, 1);
		vertices[5] = new Vector4(farWidth, farHeight, far, 1);
		vertices[6] = new Vector4(farWidth, -farHeight, far, 1);
		vertices[7] = new Vector4(-farWidth, -farHeight, far, 1);
	}

	void Update() {
		vMinBound.x = float.MaxValue;
		vMinBound.y = float.MaxValue;
		vMinBound.z = float.MaxValue;
		
		vMaxBound.x = float.MinValue;
		vMaxBound.y = float.MinValue;
		vMaxBound.z = float.MinValue;

		var matFrustumWorld = transform.localToWorldMatrix;
		
		for(int i = 0; i < 8; i++)
		{
			var worldPos = matFrustumWorld * vertices[i];
			vMinBound.x = Mathf.Min(worldPos.x, vMinBound.x);
			vMinBound.y = Mathf.Min(worldPos.y, vMinBound.y);
			vMinBound.z = Mathf.Min(worldPos.z, vMinBound.z);

			vMaxBound.x = Mathf.Max(worldPos.x, vMaxBound.x);
			vMaxBound.y = Mathf.Max(worldPos.y, vMaxBound.y);
			vMaxBound.z = Mathf.Max(worldPos.z, vMaxBound.z);
		}
		Debug.Log(vMinBound);
		
	}

	 void OnDrawGizmos()
	 {
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = Color.red;
		Gizmos.DrawFrustum(transform.position, fieldOfView, far, near, aspect);

		Gizmos.matrix = Matrix4x4.identity;
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(new Vector3(vMinBound.x, vMinBound.y, vMinBound.z), new Vector3(vMaxBound.x, vMinBound.y, vMinBound.z));
		Gizmos.DrawLine(new Vector3(vMinBound.x, vMinBound.y, vMinBound.z), new Vector3(vMinBound.x, vMaxBound.y, vMinBound.z));
		Gizmos.DrawLine(new Vector3(vMaxBound.x, vMinBound.y, vMinBound.z), new Vector3(vMaxBound.x, vMaxBound.y, vMinBound.z));
		Gizmos.DrawLine(new Vector3(vMaxBound.x, vMaxBound.y, vMinBound.z), new Vector3(vMinBound.x, vMaxBound.y, vMinBound.z));

		Gizmos.DrawLine(new Vector3(vMinBound.x, vMinBound.y, vMinBound.z), new Vector3(vMinBound.x, vMinBound.y, vMaxBound.z));
		Gizmos.DrawLine(new Vector3(vMinBound.x, vMaxBound.y, vMinBound.z), new Vector3(vMinBound.x, vMaxBound.y, vMaxBound.z));
		Gizmos.DrawLine(new Vector3(vMaxBound.x, vMinBound.y, vMinBound.z), new Vector3(vMaxBound.x, vMinBound.y, vMaxBound.z));
		Gizmos.DrawLine(new Vector3(vMaxBound.x, vMaxBound.y, vMinBound.z), new Vector3(vMaxBound.x, vMaxBound.y, vMaxBound.z));

		Gizmos.DrawLine(new Vector3(vMinBound.x, vMinBound.y, vMaxBound.z), new Vector3(vMaxBound.x, vMinBound.y, vMaxBound.z));
		Gizmos.DrawLine(new Vector3(vMinBound.x, vMinBound.y, vMaxBound.z), new Vector3(vMinBound.x, vMaxBound.y, vMaxBound.z));
		Gizmos.DrawLine(new Vector3(vMaxBound.x, vMinBound.y, vMaxBound.z), new Vector3(vMaxBound.x, vMaxBound.y, vMaxBound.z));
		Gizmos.DrawLine(new Vector3(vMaxBound.x, vMaxBound.y, vMaxBound.z), new Vector3(vMinBound.x, vMaxBound.y, vMaxBound.z));
	 }
}
