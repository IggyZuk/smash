using UnityEngine;
using System.Collections;

public class FPSCounter : MonoBehaviour
{
	public tk2dTextMesh FPSText;

	void Update()
	{
		float fps = (int)(1.0f / Time.unscaledDeltaTime);
		FPSText.text = string.Format("FPS: {0}", fps);

		Color color = Color.white;
		if(fps > 40f) color = Color.green;
		else if(fps <= 40f && fps > 20f) color = Color.yellow;
		else if(fps <= 20f) color = Color.red;
		color.a = 0.5f;

		FPSText.color = color;
	}
}
