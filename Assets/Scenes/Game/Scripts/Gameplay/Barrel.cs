using UnityEngine;
using System.Collections;

public class Barrel : MonoBehaviour
{
	private tk2dSprite _sprite;

	private bool _isPlayerInside = false;
	private bool _isShooting = false;

	void Awake()
	{
		// Find and assign references to components
		_sprite = GetComponent<tk2dSprite>();
	}

	void Start()
	{
		InputController.OnTouchBegan += TouchBegan;

		StartCoroutine(Rotation_Coroutine());
	}

	void OnDestroy()
	{
		InputController.OnTouchBegan -= TouchBegan;
	}

	// When the player is inside the barrel we can tap anywhere on the screen to shoot
	private void TouchBegan(Vector2 touchPosition)
	{
		if(_isPlayerInside)
		{
			_isShooting = true;
		}
	}

	// As the player hits the barrel we'll set off a timer for it to fire
	void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.tag == "Player" && _isPlayerInside == false)
		{
			StartCoroutine(PrepareToFire_Coroutine());
		}
	}

	// Rotate the barrel at random speeds
	private IEnumerator Rotation_Coroutine()
	{
		float rotationSpeed = 360f; //Random.Range(200.0f, 600.0f);

		float t = 0f;

		while(true)
		{
			t += Time.deltaTime;

			this.transform.rotation = Quaternion.AngleAxis(rotationSpeed * t, Vector3.forward);

			Debug.DrawLine(this.transform.position, this.transform.position + this.transform.up);

			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator PrepareToFire_Coroutine()
	{
		//GameController.Instance.PlaySound(GameController.SoundId.Collect);

		GameController.Instance.OnPlayerInputBlocked(true);
		GameController.Instance.OnPlayerSetVisible(false);

		_isPlayerInside = true;
		_sprite.color = Color.red;

		float t = 0.0f;
		float time = 2.0f;

		while(t < time)
		{
			t += Time.deltaTime;

			this.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 2f, Mathf.SmoothStep(0f, 1f, t / time));

			GameController.Instance.Player.transform.position = transform.position;

			if(_isShooting) break;

			yield return null;
		}

		this.transform.localScale = Vector3.one;
		_sprite.color = Color.white;

		Shoot(this.transform.up);

		StartCoroutine(BlockPlayerInputBitLonger_Coroutine());
	}

	private void Shoot(Vector3 dir)
	{
		GameController.Instance.OnPlayerSetVisible(true);

		GameController.Instance.OnPlayerBoost(dir, 1.25f);

		VisualUtils.AddExplosion(this.transform.position);

		_isShooting = false;
	}

	private IEnumerator BlockPlayerInputBitLonger_Coroutine()
	{
		yield return new WaitForSeconds(0.5f);

		GameController.Instance.OnPlayerInputBlocked(false);

		VisualUtils.AddExplosion(this.transform.position);

		GameObject.Destroy(this.gameObject);

		_isPlayerInside = false;
	}
}
