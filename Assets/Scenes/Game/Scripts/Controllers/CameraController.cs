using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public GameObject FollowObject = null;
	public float FollowSpeed = 5.0f;
	public float Offset = -1.5f;

	private Camera _cam;

	public ParticleSystem Status;

	private float _highestPoint = 0f;
	private float _followSpeed = 0f;

	void Awake()
	{
		_cam = Camera.main;

		Status.Stop();
	}

	void FixedUpdate()
	{
		Vector3 followPos = FollowObject.transform.position;

		// Allow the camera to only move up
		if(followPos.y > _highestPoint) _highestPoint = followPos.y;
		else followPos.y = _highestPoint;

		// Camera move speed is defined by the distance of the player from the center of the screen
		float distance = _cam.WorldToViewportPoint(GameController.Instance.Player.transform.position).y;
		float moveSpeed = Mathf.Lerp(0f, FollowSpeed, distance);

		// Positioning the camera is a simple lerp on the Y axis
		Vector3 movedPosition = Vector3.Lerp(this.transform.position, followPos + (Vector3.up * Offset), moveSpeed * Time.deltaTime);
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
