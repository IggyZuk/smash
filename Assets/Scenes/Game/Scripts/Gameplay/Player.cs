using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	private Rigidbody2D _rb;
	private tk2dSprite _sprite;

	private PlayerSettings _settings;

	private bool _isInputBlocked = false;

	private enum JumpState
	{
		Floating,
		FirstJump,
		SecondJump,
		Attack
	}

	private JumpState jumpState = JumpState.Floating;

	private Vector2 _startTouchPos;
	private Vector2 _endTouchPos;

	void Awake()
	{
		// Find and assign references to components
		_rb = GetComponent<Rigidbody2D>();
		_sprite = GetComponent<tk2dSprite>();
	}

	void Start()
	{
		// Let's get the player settings from our game settings
		_settings = GameSettings.Instance.PlayerSettings;

		// Register to game events
		GameController.Instance.OnPlayerBoost += Boost;
		GameController.Instance.OnPlayerInputBlocked += (bool blocked) => { _isInputBlocked = blocked; };
		GameController.Instance.OnPlayerSetVisible += (bool visible) => { GetComponent<MeshRenderer>().enabled = visible; };

		InputController.OnTouchBegan += TouchBegan;
		InputController.OnTouchEnded += TouchEnded;

		// We'll add the initial jump to get the game started
		Jump(Vector2.up);
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
		WorldUtils.StayInBounds(ref _rb);

		Debug.DrawLine(_rb.position, _rb.position + _rb.velocity.normalized * 2.0f, Color.magenta);
	}

	private void UpdateJumpState()
	{
		// Find direction of the swipe so we can decide if we're in attack mode
		Vector2 direction = (_endTouchPos - _startTouchPos).normalized;

		// Check & update jumping state
		if(jumpState != JumpState.Attack && Vector2.Dot(direction, Vector2.up) < 0)
		{
			jumpState = JumpState.Attack;
			_sprite.color = Color.red;
			Jump(direction);
		}
		else if(jumpState == JumpState.Floating)
		{
			jumpState = JumpState.FirstJump;
			_sprite.color = new Color(0.25f, 0.75f, 0.25f);
			Jump(direction);
		}
		else if(jumpState == JumpState.FirstJump)
		{
			jumpState = JumpState.SecondJump;
			_sprite.color = Color.green;
			Jump(direction);
		}
	}

	void OnCollisionStay2D(Collision2D collision)
	{
		if(collision.gameObject.tag == "Ground")
		{
			jumpState = JumpState.Floating;
			_sprite.color = Color.white;
		}
	}

	private void Jump(Vector3 dir, float magnitude = 1f)
	{
		_rb.velocity = Vector2.zero;
		_rb.AddForce(dir * _settings.JumpHeight * magnitude, ForceMode2D.Impulse);

		VisualUtils.AddDarkHit(new Vector3(this.transform.position.x, this.transform.position.y, -1f));
		GameController.Instance.PlaySound(GameSettings.Instance.AudioSettings.Swipe);
	}

	private void Boost(Vector3 dir, float magnitude = 1f)
	{
		_rb.angularVelocity = 0f;
		_rb.AddTorque(-_rb.velocity.x * 0.8f, ForceMode2D.Impulse);

		_rb.velocity = new Vector2(_rb.velocity.x, 0f);
		_rb.AddForce(dir * (_settings.JumpHeight * 2f) * magnitude, ForceMode2D.Impulse);

		jumpState = JumpState.Floating;
		_sprite.color = Color.white;

		_sprite.SetSprite(string.Format("Turtle0{0}", Random.Range(1, 9)));
	}

	public Vector2 GetVelocity()
	{
		return _rb.velocity;
	}
}
