using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	private Rigidbody2D _rb;
	private tk2dSprite _sprite;

	private EnemySettings _settings;

	private bool _isAlive = true;

	private float _time;
	private float _speed;

	void Awake()
	{
		// Find and assign references to components
		_rb = GetComponent<Rigidbody2D>();
		_sprite = GetComponent<tk2dSprite>();
	}

	void Start()
	{
		// Make a deep copy of the enemy settings from the game settings
		_settings = GameSettings.Instance.EnemySettings.DeepCopy(GameSettings.Instance.EnemySettings);

		_time = Random.value * 360f;
		_speed = Random.value * 5f;

		_rb.velocity = new Vector2(_speed, 0f);

		Debug.Log(_rb.velocity);
	}

	void FixedUpdate()
	{
		// Some basic movement for the enemy
		if(_isAlive)
		{
			_time += Time.deltaTime;

			// Keep moving into the direction you were last looking at
			float direction = Mathf.Sign(_rb.velocity.x);
			_rb.velocity = Vector2.zero;
			_rb.AddForce(new Vector3(direction * _speed, 0f), ForceMode2D.Impulse);

			Debug.DrawLine(transform.position, transform.position + (Vector3)_rb.velocity);
		}

		WorldUtils.StayInBounds(ref _rb);

		Debug.DrawLine(_rb.position, _rb.position + _rb.velocity.normalized * 2.0f, Color.magenta);
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		// If we've collided with the player then fire an event to let the player know! Also let's take away some health for the enemy
		if(collider.tag == "Player")
		{
			_rb.gravityScale = 0.9f;
			Vector2 vel = GameController.Instance.Player.GetVelocity();
			vel.y = 7.0f;
			_rb.AddForce(vel, ForceMode2D.Impulse);

			_sprite.SetSprite(Random.value < 0.5f ? "GoombaDead1" : "GoombaDead2");

			_isAlive = false;

			VisualUtils.AddHit(this.transform.position);

			if(--_settings.Health <= 0)
			{
				GameController.Instance.OnPlayerBoost(Vector3.up, 1.1f);
				Explode();
				GameController.Instance.PlaySound(GameSettings.Instance.AudioSettings.Death);
			}
			else
			{
				GameController.Instance.OnPlayerBoost(Vector3.up, 0.75f);
				GameController.Instance.Camera.Screenshake(0.25f, 0.5f);
				GameController.Instance.PlaySound(GameSettings.Instance.AudioSettings.Smash);
			}

			_sprite.color = Color.Lerp(Color.white, Color.red, 1f / (float)_settings.Health);
		}
	}

	private void Explode()
	{
		VisualUtils.AddExplosion(this.transform.position);

		for(int i = 0; i < 6; ++i)
		{
			GameObject go = GameObject.Instantiate(GameSettings.Instance.Prefabs.Bone, this.transform.position, Quaternion.AngleAxis(Random.value * 360f, Vector3.forward)) as GameObject;
			Rigidbody2D boneRigidbody = go.GetComponent<Rigidbody2D>();

			Vector2 vel = new Vector2(Random.Range(-20f, 20f), Random.value * 20f);
			boneRigidbody.AddForce(vel, ForceMode2D.Impulse);

			boneRigidbody.angularVelocity = 0.0f;
			boneRigidbody.AddTorque(-boneRigidbody.velocity.x * 0.8f, ForceMode2D.Impulse);
		}

		GetComponentInChildren<ParticleSystem>().transform.SetParent(null);
		GameObject.Destroy(this.gameObject);
	}
}
