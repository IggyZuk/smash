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

	private int _scoreValue = 0;
	private Color _originalScoreColor;

	private float _scorePitch = 1f;
	private float _lastHeight = 0f;

	void Start()
	{
		_playerTransform = GameController.Instance.Player.transform;

		_originalScoreColor = ScoreText.color;

		NextGoal();
	}

	void Update()
	{
		if(_playerTransform.position.y > _nextHeight)
		{
			_lastHeight = _nextHeight;
			NextGoal();
			GameController.Instance.PlaySound(GameSettings.Instance.AudioSettings.Milestone, 1.5f);
			_scorePitch = 1f;
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

				entityPosition.x = Random.Range(WorldUtils.GetLeftEdge() + 1f, WorldUtils.GetRightEdge() - 1f);
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

			_maxHeight = Mathf.Max(0f, _playerTransform.position.y);

			// Update height score
			int score = (int)Mathf.Floor(_maxHeight / 2.5f);
			if(score > _scoreValue)
			{
				_scoreValue = score;
				ScoreText.text = string.Format("Height: {0}m", score);
				StartCoroutine(EmphasizeScore_Coroutine(0.25f));
				_scorePitch = (_playerTransform.position.y - _lastHeight) / (_nextHeight - _lastHeight) + 1f * 0.5f;
				GameController.Instance.PlaySound(GameSettings.Instance.AudioSettings.ScorePoint, 0.25f, _scorePitch);
			}
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
			StartCoroutine(SlowMotion_Coroutine(0.25f));
			GameController.Instance.Camera.ShowStatus();
		}

		_line = new GameObject("Line").AddComponent<LineRenderer>();
		_line.material = new Material(Shader.Find("Sprites/Default"));
		Color randomColor = new Color(0f, 1f, 0.7f);
		_line.SetColors(randomColor, randomColor);
		_line.SetWidth(0.2f, 0.2f);

		_line.SetPosition(0, new Vector3(WorldUtils.GetLeftEdge(), _nextHeight, 0));
		_line.SetPosition(1, new Vector3(WorldUtils.GetRightEdge(), _nextHeight, 0));
	}

	IEnumerator SlowMotion_Coroutine(float time)
	{
		//AndroidNotificationManager.instance.ScheduleLocalNotification("New Highscore", "Holy smokes, you're doing awesome!", 1);

		float fixedDeltaTime = Time.fixedDeltaTime;

		Time.timeScale = GameSettings.Instance.WorldSettings.SlowMotion;
		Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;

		yield return new WaitForSeconds(GameSettings.Instance.WorldSettings.SlowMotionWait * Time.timeScale);

		float t = 0f;

		while(t < time)
		{
			t += Time.unscaledDeltaTime;

			Time.timeScale = Mathf.Lerp(GameSettings.Instance.WorldSettings.SlowMotion, 1f, t / time);
			Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;

			yield return null;
		}
	}

	private IEnumerator EmphasizeScore_Coroutine(float time)
	{
		float t = 0f;

		while(t < time)
		{
			t += Time.deltaTime;
			float tSmooth = Mathf.SmoothStep(0f, 1f, t / time);

			ScoreText.transform.localScale = Vector3.Lerp(Vector3.one * 1.25f, Vector3.one, tSmooth);
			ScoreText.color = Color.Lerp(new Color(0f, 1f, 0.7f), _originalScoreColor, tSmooth);

			yield return null;
		}

	}
}
