using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GoalSystem : MonoBehaviour
{
	public tk2dTextMesh ScoreText;
	public tk2dTextMesh GoalText;

	private LineRenderer _line;
	private Transform _playerTransform;

	private float _maxHeight = 0f;
	private float _deltaPosition = 10.0f;
	private const float ADD_ENTITY_DISTANCE = 13.5f;

	private int _count = 0;
	private float _nextHeight = Mathf.NegativeInfinity;
	private float _multiplier = 2f;
	private float _addition = 20.0f;

	void Start()
	{
		_playerTransform = GameController.Instance.Player.transform;

		NextGoal();
	}

	void Update()
	{
		if(_playerTransform.position.y > _nextHeight)
		{
			NextGoal();
			//GameController.Instance.PlaySound(GameController.SoundId.Milestone);
		}

		// Set next goal text
		float scaleFactor = Mathf.Abs(Mathf.Cos(Time.timeSinceLevelLoad * 4.0f) * 0.5f);
		_line.SetWidth(scaleFactor, scaleFactor);
		GoalText.text = string.Format("Next Goal: {0}m", Mathf.Floor(_nextHeight - _playerTransform.position.y));

		// Add new enemies
		if(_playerTransform.position.y > _maxHeight)
		{
			_deltaPosition += _playerTransform.position.y - _maxHeight;
			if(_deltaPosition > ADD_ENTITY_DISTANCE)
			{
				Vector2 entityPosition = _playerTransform.position;

				entityPosition.x = Random.Range(WorldUtils.GetLeftEdge(), WorldUtils.GetRightEdge());
				entityPosition.y += 10f;

				GameObject entity;
				if(Random.value < 0.85f)
				{
					entity = GameSettings.Instance.Prefabs.Enemy;
				}
				else
				{
					entity = GameSettings.Instance.Prefabs.Barrel;
				}

				Instantiate(entity, entityPosition, Quaternion.identity);

				_deltaPosition -= ADD_ENTITY_DISTANCE;
			}

			// Update height score
			_maxHeight = Mathf.Max(0f, _playerTransform.position.y);
			ScoreText.text = string.Format("Height: {0}m", Mathf.Floor(_maxHeight));
		}
	}

	public void NextGoal()
	{
		++_count;
		_nextHeight = _playerTransform.position.y + _addition * (_count * _multiplier);
		if(_line)
		{
			_line.SetWidth(0.2f, 0.2f);
			_line.SetColors(Color.gray, Color.gray);
			StartCoroutine(SlowMotion_Coroutine());
			GameController.Instance.Camera.ShowStatus();
		}

		_line = new GameObject().AddComponent<LineRenderer>();
		_line.material = new Material(Shader.Find("Sprites/Default"));
		Color randomColor = new Color(Random.value, Random.value, Random.value);
		_line.SetColors(randomColor, randomColor);
		_line.SetWidth(0.2f, 0.2f);

		_line.SetPosition(0, new Vector3(WorldUtils.GetLeftEdge(), _nextHeight, 0));
		_line.SetPosition(1, new Vector3(WorldUtils.GetRightEdge(), _nextHeight, 0));
	}

	IEnumerator SlowMotion_Coroutine()
	{
		//AndroidNotificationManager.instance.ScheduleLocalNotification("New Highscore", "Holy smokes, you're doing awesome!", 1);

		Time.timeScale = 0.3f;
		Time.fixedDeltaTime = 0.02f * Time.timeScale;

		yield return new WaitForSeconds(1.0f * Time.timeScale);

		float t = 0f;
		float time = 0.25f;

		while(t < time)
		{
			t += Time.unscaledDeltaTime;

			Time.timeScale = Mathf.Lerp(0.3f, 1f, t / time);
			Time.fixedDeltaTime = 0.02f * Time.timeScale;

			yield return null;
		}
	}
}
