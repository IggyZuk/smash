using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public GameObject FollowObject = null;
	public float FollowSpeed = 5.0f;
	public float VelocitySpeed = 2.0f;

	private Vector2 _offset = Vector2.zero;

	private Camera _cam;

	public ParticleSystem Status;

	void Awake()
	{
		_cam = Camera.main;

		Status.Stop();
    }

	void FixedUpdate()
	{
		// Offset is used to position the camera in the inverse of the players velocity
		// This allows the player to see more in the opposite direction
		float vy = Mathf.Clamp(GameController.Instance.Player.GetVelocity().y, -5, 5f);
		_offset = Vector2.Lerp(_offset, new Vector2(0f, -vy), VelocitySpeed * Time.deltaTime);

		// Positioning the camera is a simple lerp on the Y axis
		Vector3 movedPosition = Vector3.Lerp(this.transform.position, FollowObject.transform.position + (Vector3)_offset, Time.deltaTime * FollowSpeed);
		transform.position = new Vector3(0f, movedPosition.y, transform.position.z);
	}

	public void Screenshake(float time, float magnitude)
	{
		StartCoroutine(Screenshake_Coroutine(time, magnitude));
	}

	private IEnumerator Screenshake_Coroutine(float time, float magnitude)
	{
		float t = 0f;
		while(t < time)
		{
			t += Time.deltaTime;

			float strength = Mathf.Clamp01(1f - t / time);
			_cam.transform.localPosition = new Vector3(_cam.transform.localPosition.x, Random.value * magnitude * strength, _cam.transform.localPosition.z);

			yield return null;
		}
		_cam.transform.localPosition = new Vector3(_cam.transform.localPosition.x, 0f, _cam.transform.localPosition.z);
	}

	public void ShowStatus()
	{
		Status.Clear();
		Status.Play();
	}
}
