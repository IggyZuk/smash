using UnityEngine;
using System.Collections;

public class FPSCounter : MonoBehaviour
{
	public tk2dTextMesh FPSText;

	void Update()
	{
		float fps = (int)(1.0f / Time.unscaledDeltaTime);
		FPSText.text = string.Format("FPS: {0}", fps);
		if(fps > 40) FPSText.color = Color.green;
		else if(fps <= 40 && fps > 20) FPSText.color = Color.yellow;
		else if(fps <= 20) FPSText.color = Color.red;
	}
}
