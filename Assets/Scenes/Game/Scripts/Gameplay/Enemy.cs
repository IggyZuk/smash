using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	[SerializeField]
	private tk2dSprite _sprite;
	private Rigidbody2D _rb;

	private EnemySettings _settings;
	private EnemySettings _settingsModified;

	private bool _isAlive = true;

	private float _time;
	private float _speed;

	void Awake()
	{
		// Find and assign references to components
		_rb = GetComponent<Rigidbody2D>();
	}

	void Start()
	{
		// Make a deep copy of the enemy settings from the game settings
		_settings = GameSettings.Instance.EnemySettings;
		_settingsModified = GameSettings.Instance.EnemySettings.DeepCopy(_settings);

		_time = Random.value * 360f;
		_speed = Random.value * 5f;

		_rb.velocity = new Vector2(_speed, 0f);
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
			_isAlive = false;

			float damageValue = 1f - (float)(_settingsModified.Health - 1) / (float)_settings.Health;
			float boost = Mathf.Lerp(0.5f, 1f, damageValue);

			_rb.gravityScale = 0.5f;

			Vector2 vel = GameController.Instance.Player.GetVelocity();
			vel.x += _rb.velocity.x;
			vel.y = GameSettings.Instance.DamageSettings.Boost * 0.5f;

			_rb.velocity = Vector2.zero;
			_rb.AddForce(vel, ForceMode2D.Impulse);

			_sprite.SetSprite(Random.value < 0.5 ? "GoombaDead1" : "GoombaDead2");

			VisualUtils.AddHit(this.transform.position);

			GameController.Instance.PlaySound(GameSettings.Instance.AudioSettings.Smash, 1f, damageValue + 1f);



			if(--_settingsModified.Health <= 0)
			{
				Explode();
				GameController.Instance.PlaySound(GameSettings.Instance.AudioSettings.Death);

				GameController.Instance.OnPlayerBoost(Vector3.up, boost * 1.25f);
			}
			else
			{
				_sprite.color = Color.Lerp(Color.white, Color.red, damageValue);
				GameController.Instance.Camera.Screenshake(0.25f, 0.5f);

				GameController.Instance.OnPlayerBoost(Vector3.up, boost);

				// Also let's add goggles
				GameObject go = GameObject.Instantiate(GameSettings.Instance.Prefabs.Bone, this.transform.position, Quaternion.AngleAxis(Random.value * 360f, Vector3.forward)) as GameObject;
				Rigidbody2D boneRigidbody = go.GetComponent<Rigidbody2D>();

				Vector2 explodeVel = new Vector2(Random.Range(-5f, 5f), 10f);
				boneRigidbody.AddForce(explodeVel, ForceMode2D.Impulse);

				boneRigidbody.angularVelocity = 0.0f;
				boneRigidbody.AddTorque(-boneRigidbody.velocity.x * 0.5f, ForceMode2D.Impulse);

			}
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
