using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameSettings : MonoBehaviour
{
	public Prefabs Prefabs;

	public static GameSettings Instance { get; private set; }

	void Awake()
	{
		Debug.Assert(Instance == null, "Singleton can only have one instance!");
		Instance = this;

		Screen.SetResolution(480, 800, false, 60);

		//Object.DontDestroyOnLoad(gameObject);
	}
}

[System.Serializable]
public class Prefabs
{
	[Header("Objects")]
	public GameObject Player;
	public GameObject Enemy;
	public GameObject Barrel;
	public GameObject Bone;
	[Header("FX")]
	public GameObject Explosion;
	public GameObject Hit;
}
