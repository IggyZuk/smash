using UnityEngine;
using System.Collections;

public class VisualUtils : MonoBehaviour
{
	public static void AddExplosion(Vector3 pos)
	{
		Instantiate(GameSettings.Instance.Prefabs.Explosion, pos, Quaternion.identity);
		GameController.Instance.Camera.Screenshake(0.5f, 1f);
	}

	public static void AddFireExplosion(Vector3 pos)
	{
		Instantiate(GameSettings.Instance.Prefabs.FireExplosion, pos, Quaternion.identity);
		GameController.Instance.Camera.Screenshake(0.5f, 1f);
	}

	public static void AddHit(Vector3 pos)
	{
		Instantiate(GameSettings.Instance.Prefabs.Hit, pos, Quaternion.identity);
	}

	public static void AddDarkHit(Vector3 pos)
	{
		Instantiate(GameSettings.Instance.Prefabs.DarkHit, pos, Quaternion.identity);
	}
}
