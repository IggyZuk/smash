using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]

public class ScreenspaceSquares : MonoBehaviour
{
	public Shader shader;

	[Range(2, 64)]
	public int Scale = 64;

	private Texture2D _texture;
	private Material _material;

	private Player _player;

	void Awake()
	{
		// Create the texture
		_texture = new Texture2D(2, 2);
		_texture.filterMode = FilterMode.Point;

		for(int i = 0; i < _texture.width; ++i)
		{
			for(int j = 0; j < _texture.height; ++j)
			{
				bool isLight = (i + j) % 2 == 0;
				_texture.SetPixel(i, j, isLight ? new Color(0.2f, 0.15f, 0.25f) : Color.black);
			}
		}
		_texture.Apply();

	}

	void Start()
	{
		_player = GameController.Instance.Player;
	}

	protected Material material
	{
		get
		{
			if(_material == null)
			{
				_material = new Material(shader);
				_material.hideFlags = HideFlags.HideAndDontSave;
			}
			return _material;
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if(shader == null) return;
		Material mat = material;
		Graphics.Blit(source, destination, mat);

		mat.SetTexture("_Detail", _texture);
		mat.SetFloat("_Scale", Scale);
		mat.SetFloat("_Offset", Camera.main.transform.position.y / 25f);
	}

	void OnDisable()
	{
		if(_material)
		{
			DestroyImmediate(_material);
		}
	}
}
