using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
	// UI elements 
	// TODO: move these some place encapsulated
	[SerializeField]
	public tk2dUIItem _settingsButton;
	[SerializeField]
	public tk2dUIItem _pauseButton;
	[SerializeField]
	public MeshRenderer _logo;

	// Events
	// TODO: make these static and move them to appropriate classes
	public System.Action<Vector3, float> OnPlayerBoost;
	public System.Action<bool> OnPlayerInputBlocked;
	public System.Action<bool> OnPlayerSetVisible;

	// Public properties, many classes use these
	public Player Player { get; private set; }
	public CameraController Camera { get; private set; }

	private GoalSystem _goalSystem;

	private bool _isPaused = false;

	public static GameController Instance { get; private set; }

	void Awake()
	{
		Debug.Assert(Instance == null, "Singleton can only have one instance!");
		Instance = this;

		// Let's find the current player instance in the scene and assign it here in case anyone needs access to player later
		Player = FindObjectOfType<Player>();
		Camera = FindObjectOfType<CameraController>();

		_goalSystem = this.gameObject.GetComponent<GoalSystem>();

		// Pressing settings button will take us to gacha scene
		_settingsButton.OnClick += () => { UnityEngine.SceneManagement.SceneManager.LoadScene("Gacha"); };

		// Pressing pause button will obviously pause the game
		_pauseButton.OnClick += () => { TogglePause(); };

		OnPlayerBoost += (Vector3 v, float f) => { _logo.enabled = false; };
	}

	void Update()
	{
		// Handy to restart the game by pressing R
		if(Input.GetKeyDown(KeyCode.R))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			Time.timeScale = 1f;
		}

		if(Input.GetKeyDown(KeyCode.P))
		{
			TogglePause();
		}
	}

	void OnGUI()
	{
		if(GameSettings.Instance.IsDebug)
		{
			if(GUI.Button(new Rect(10, 10, 150, 50), "Restart"))
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
				Time.timeScale = 1f;
			}
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

	public void TogglePause()
	{
		_isPaused = !_isPaused;
		if(_isPaused) Time.timeScale = 0f;
		else Time.timeScale = 1f;
	}
}
