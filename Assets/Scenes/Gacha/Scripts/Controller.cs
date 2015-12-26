using UnityEngine;
using System.Collections;

namespace Gacha
{
	public class Controller : MonoBehaviour
	{
		[SerializeField]
		private tk2dUIItem _playButton;

		void Awake()
		{
			_playButton.OnClick += () => { UnityEngine.SceneManagement.SceneManager.LoadScene("Game"); };
		}
	}
}
