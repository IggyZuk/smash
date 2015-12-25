using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class Player : MonoBehaviour
{
	[SerializeField]
	private tk2dSprite _sprite;

	private Rigidbody2D _rb;

	private PlayerSettings _settings;

	private bool _isInputBlocked = false;
	private bool _isGameOver = false;

	private string _playerSkinPrefix;

	private enum JumpState
	{
		Floating,
		FirstJump,
		SecondJump,
		Attack
	}

	private JumpState _jumpState = JumpState.Floating;

	private Vector2 _startTouchPos;
	private Vector2 _endTouchPos;

	private Vector2 _startDivePos;

	void Awake()
	{
		// Find and assign references to components
		_rb = GetComponent<Rigidbody2D>();
	}

	void Start()
	{
		// Let's get the player settings from our game settings
		_settings = GameSettings.Instance.PlayerSettings;

		// Register to game events
		GameController.Instance.OnPlayerBoost += Boost;
		GameController.Instance.OnPlayerInputBlocked += (bool blocked) => { _isInputBlocked = blocked; };
		GameController.Instance.OnPlayerSetVisible += (bool visible) => { _sprite.GetComponent<MeshRenderer>().enabled = visible; };

		InputController.OnTouchBegan += TouchBegan;
		InputController.OnTouchEnded += TouchEnded;

		// FiFind the current player skin prefix for the sprites
		_playerSkinPrefix = string.Format("Player0{0}", _settings.SkinIdx);
	}

	void OnDestroy()
	{
		// Un-register to game events
		InputController.OnTouchBegan -= TouchBegan;
		InputController.OnTouchEnded -= TouchEnded;
	}

	public void TouchBegan(Vector2 touchPosition)
	{
		_startTouchPos = touchPosition;
	}

	public void TouchEnded(Vector2 touchPosition)
	{
		if(_isInputBlocked) return;

		_endTouchPos = touchPosition;

		UpdateJumpState();
	}

	void FixedUpdate()
	{
		if(_rb.velocity.y > 8f) _sprite.SetSprite(string.Format("{0}_Jump", _playerSkinPrefix));
		else if(_rb.velocity.y > 0f) _sprite.SetSprite(string.Format("{0}_InAir", _playerSkinPrefix));
		else if(_jumpState == JumpState.Attack) _sprite.SetSprite(string.Format("{0}_Dive", _playerSkinPrefix));

		// Turn side to side
		_sprite.scale = (Vector3.right * Mathf.Sign(_rb.velocity.x) + Vector3.up) * 1f;

		if(Camera.main.WorldToViewportPoint(this.transform.position).y < 0 - 0.1f)
		{
			if(_isGameOver == false)
			{
				StartCoroutine(Lose_Coroutine(3.266f));
				_isInputBlocked = true;
			}
		}

		WorldUtils.StayInBounds(ref _rb);

		Debug.DrawLine(_rb.position, _rb.position + _rb.velocity.normalized * 2.0f, Color.magenta);
	}

	private IEnumerator Lose_Coroutine(float time)
	{
		_isGameOver = true;

		GameController.Instance.PlaySound(GameSettings.Instance.AudioSettings.Lose);

		float t = 0f;
		while(t < time)
		{
			t += Time.unscaledDeltaTime;
			yield return null;
		}

		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		Time.timeScale = 1f;
	}

	private void UpdateJumpState()
	{
		// Find direction of the swipe so we can decide if we're in attack mode
		Vector2 direction = _endTouchPos - _startTouchPos;
		if(direction.magnitude > 25f) direction.Normalize();
		else direction = Vector2.up;

		// Check & update jumping state
		if(_jumpState != JumpState.Attack && Vector2.Dot(direction, Vector2.up) < 0)
		{
			_jumpState = JumpState.Attack;
			_startDivePos = _rb.position;
			Jump(direction);

			//_sprite.color = Color.red;
			GameController.Instance.PlaySound(GameSettings.Instance.AudioSettings.Dive, 0.5f, 1f);
		}
		else if(_jumpState == JumpState.Floating)
		{
			_jumpState = JumpState.FirstJump;
			Jump(direction);

			//_sprite.color = new Color(0.25f, 0.75f, 0.25f);
			GameController.Instance.PlaySound(GameSettings.Instance.AudioSettings.Swipe, 1f, 1f);
		}
		else if(_jumpState == JumpState.FirstJump)
		{
			_jumpState = JumpState.SecondJump;
			Jump(direction);

			//_sprite.color = Color.green;
			GameController.Instance.PlaySound(GameSettings.Instance.AudioSettings.Swipe, 1f, 1.25f);
		}
	}

	void OnCollisionStay2D(Collision2D collision)
	{
		if(collision.gameObject.tag == "Ground")
		{
			_jumpState = JumpState.Floating;
			_sprite.color = Color.white;
			_sprite.SetSprite(string.Format("{0}_Static", _playerSkinPrefix));
		}
	}

	private void Jump(Vector3 dir, float magnitude = 1f)
	{
		//transform.rotation = Quaternion.AngleAxis(_rb.velocity.x * 5f, Vector3.forward);

		_rb.velocity = Vector2.zero;
		_rb.AddForce(dir * _settings.JumpHeight * magnitude, ForceMode2D.Impulse);

		VisualUtils.AddDarkHit(new Vector3(this.transform.position.x, this.transform.position.y, -1f));
	}

	private void Boost(Vector3 dir, float magnitude = 1f)
	{
		// Add upper force
		_rb.velocity = new Vector2(_rb.velocity.x, 0f);
		_rb.AddForce(dir * GameSettings.Instance.DamageSettings.Boost * magnitude, ForceMode2D.Impulse);

		// Reset the state; allow the player to jump again
		_jumpState = JumpState.Floating;
		_sprite.color = Color.white;
	}

	public Vector2 GetVelocity()
	{
		return _rb.velocity;
	}

	public Vector2 GetStartDivePos()
	{
		if(_jumpState == JumpState.Attack) return _startDivePos;
		return _rb.position;
	}
}
