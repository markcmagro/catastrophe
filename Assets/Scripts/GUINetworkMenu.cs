using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class GUINetworkMenu : MonoBehaviour {

	public NetworkManager targetManager;

    public InputField ifAddress = null;
    public InputField ifPort = null;

	public Slider difficultySlider = null;
	public NetMgr sceneNetMgr = null;

	public void ConnectClient () {
		targetManager.StartClient ();
	}

	public void StartHost () {
		targetManager.StartHost();
	}

	public void StartServer () {
		targetManager.StartServer();
	}

	void Start () {
		FindSceneNetMgr ();
	}

	void OnLevelWasLoaded(int level) {
		FindSceneNetMgr ();
	}

	public void SetDifficulty() {
		sceneNetMgr.difficulty = difficultySlider.value;
	}

	public void SetTraining(bool flag) {
		sceneNetMgr.SetTraining(flag);
	}

	void FindSceneNetMgr () {
		sceneNetMgr = FindObjectOfType<NetMgr> ();
		targetManager = FindObjectOfType<NetworkManager> ();
		if (difficultySlider != null)
			difficultySlider.value = sceneNetMgr.difficulty;
	}

    public void SetAddress()
    {
        targetManager.networkAddress = ifAddress.text;
        targetManager.networkPort = int.Parse(ifPort.text);
    }
}
