using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
	// Events
	public System.Action<Vector3, float> OnPlayerBoost;
	public System.Action<bool> OnPlayerInputBlocked;
	public System.Action<bool> OnPlayerSetVisible;

	public Player Player { get; private set; }
	public CameraController Camera { get; private set; }

	private GoalSystem GoalSystem;

	public static GameController Instance { get; private set; }

	void Awake()
	{
		Debug.Assert(Instance == null, "Singleton can only have one instance!");
		Instance = this;

		// Let's find the current player instance in the scene and assign it here in case anyone needs access to player later
		Player = FindObjectOfType<Player>();
		Camera = FindObjectOfType<CameraController>();

		GoalSystem = this.gameObject.GetComponent<GoalSystem>();
	}

	void Update()
	{
		// Handy to restart the game by pressing R
		if(Input.GetKeyDown(KeyCode.R))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			Time.timeScale = 1f;
		}
	}

	void OnGUI()
	{
		if(GUI.Button(new Rect(Screen.width - 160, 10, 150, 50), "Restart"))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			Time.timeScale = 1f;
		}
	}

	public void PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f)
	{
		AudioSource[] audioSources = GetComponents<AudioSource>();
		AudioSource usableSource = null;

		foreach(AudioSource audioSource in audioSources)
		{
			if(audioSource.isPlaying == false)
			{
				usableSource = audioSource;
				break;
			}
		}

		if(usableSource == null) usableSource = this.gameObject.AddComponent<AudioSource>();

		usableSource.clip = clip;
		usableSource.volume = volume;
		usableSource.pitch = pitch;
		usableSource.Play();
	}
}
