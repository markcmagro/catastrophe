using UnityEngine;
using System.Collections;

/// <summary>
/// Applies and controls a customisable Game Boy effect.
/// </summary>
public class GameBoyEffect : MonoBehaviour
{
	/// <summary>
	/// Size of each retropixel, in pixels squared.
	/// </summary>
	public int retropixelSize = 4;
	/// <summary>
	/// Size of padding between each retropixel, in pixels.
	/// </summary>
	public int paddingSize = 1;
	/// <summary>
	/// Number of shades of gray used for shading.
	/// </summary>
	public int shadeCount = 4;
	/// <summary>
	/// Tint to apply to grayscale.
	/// </summary>
	public Color shadeColor = new Color(0.6078f, 0.7333f, 0.0549f, 1.0f);
	/// <summary>
	/// Each quality level increases the number of samples taken to shade each retropixel.
	/// </summary>
	public int quality = 3;

	// Local components.
	private Material gameBoyMaterial = null;
	private Camera gameBoyCamera = null;

	// Called on script startup.
	void Start()
	{
		Init();
	}

	// Performs necessary initialisation.
	private void Init()
	{
		// Obtain references to local components.
		if(gameBoyMaterial == null)
			gameBoyMaterial = new Material(Shader.Find("Hidden/Game Boy Retro Shader"));
		if(gameBoyCamera == null)
			gameBoyCamera = GetComponent<Camera>();
	}

	// Called once per frame.
	void Update()
	{
		// Handle movie camera and effect commands.
		if(Input.GetKeyDown(KeyCode.UpArrow)) // Increase retropixel size.
			retropixelSize = Mathf.Min(retropixelSize + 1, 20);
		if(Input.GetKeyDown(KeyCode.DownArrow)) // Decrease retropixel size.
			retropixelSize = Mathf.Max(retropixelSize - 1, 1);
		if(Input.GetKeyDown(KeyCode.LeftArrow)) // Increase padding size.
			paddingSize = Mathf.Max(paddingSize - 1, 0);
		if(Input.GetKeyDown(KeyCode.RightArrow)) // Decrease retropixel size.
			paddingSize = Mathf.Min(paddingSize + 1, 5);
		if(Input.GetKeyDown(KeyCode.Plus)) // Increase quality.
			quality = Mathf.Min(quality + 1, 5);
		if(Input.GetKeyDown(KeyCode.Minus)) // Decrease quality.
			quality = Mathf.Max(quality - 1, 1);
		if(Input.GetKey(KeyCode.Z)) // Zoom in.
			gameBoyCamera.orthographicSize = Mathf.Max(0.0f, gameBoyCamera.orthographicSize - 100.0f * Time.deltaTime);
		if(Input.GetKey(KeyCode.X)) // Zoom out.
			gameBoyCamera.orthographicSize = Mathf.Min(540.0f, gameBoyCamera.orthographicSize + 100.0f * Time.deltaTime);
		if(Input.GetKey(KeyCode.W)) // Pan up.
			gameBoyCamera.transform.Translate(0.0f, 100.0f * Time.deltaTime, 0.0f);
		if(Input.GetKey(KeyCode.S)) // Pan down.
			gameBoyCamera.transform.Translate(0.0f, -100.0f * Time.deltaTime, 0.0f);
		if(Input.GetKey(KeyCode.A)) // Pan left.
			gameBoyCamera.transform.Translate(-100.0f * Time.deltaTime, 0.0f, 0.0f);
		if(Input.GetKey(KeyCode.D)) // Pan right.
			gameBoyCamera.transform.Translate(100.0f * Time.deltaTime, 0.0f, 0.0f);
		if(Input.GetKey(KeyCode.Space)) // Reset camera.
		{
			gameBoyCamera.transform.position = new Vector3(0.0f, 0.0f, -10.0f);
			gameBoyCamera.orthographicSize = 540.0f;
		}
	}

	// Update is called once per frame
	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		// Set shader properties.
		gameBoyMaterial.SetInt("_XRes", Screen.width);
		gameBoyMaterial.SetInt("_YRes", Screen.height);
		gameBoyMaterial.SetInt("_RPSize", retropixelSize);
		gameBoyMaterial.SetInt("_PaddingSize", paddingSize);
		gameBoyMaterial.SetInt("_ShadeCount", shadeCount);
		gameBoyMaterial.SetColor("_ShadeColor", shadeColor);
		gameBoyMaterial.SetInt("_Quality", quality);

		// Run shader on rendered frame.
		Graphics.Blit(src, dst, gameBoyMaterial);
	}
}
