using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameSettings : MonoBehaviour
{
	public Prefabs Prefabs;
	public WorldSettings WorldSettings;
	public PlayerSettings PlayerSettings;
	public EnemySettings EnemySettings;
	public AudioSettings AudioSettings;

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
	public GameObject FireExplosion;
	public GameObject Hit;
	public GameObject DarkHit;
}

[System.Serializable]
public class WorldSettings
{
	public float EdgeMargin = 0.04f;
	public float EdgeVelocityFriction = 0.75f;
	public float SlowMotion = 0.3f;
	public float SlowMotionWait = 1f;
}

[System.Serializable]
public class PlayerSettings
{
	public float JumpHeight = 5f;
}

[System.Serializable]
public class EnemySettings
{
	public int Health = 2;

	public EnemySettings DeepCopy(EnemySettings original)
	{
		EnemySettings settings = new EnemySettings();
		settings.Health = original.Health;
		return settings;
	}
}

[System.Serializable]
public class AudioSettings
{
	public AudioClip Collect;
	public AudioClip Death;
	public AudioClip Explosion;
	public AudioClip Milestone;
	public AudioClip Smash;
	public AudioClip Swipe;
	public AudioClip Dive;
	public AudioClip ScorePoint;
}
