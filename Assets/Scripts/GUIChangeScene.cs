using UnityEngine;
using System.Collections;

public class GUIChangeScene : MonoBehaviour {

	// Change scene by parameter
	public void DoChangeScene (string NextScene) {
		Application.LoadLevel(NextScene);
	}
}
