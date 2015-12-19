using UnityEngine;

public class UIAnchor : tk2dCameraAnchor
{
	[ExecuteInEditMode]
	void Awake()
	{
		if(AnchorCamera == null)
		{
			AnchorCamera = GameObject.FindObjectOfType<tk2dCamera>().GetComponent<Camera>();
		}
	}
}
