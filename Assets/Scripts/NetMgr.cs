using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class NetMgr : NetworkBehaviour
{
    public float difficulty = 0.0f;

    public int controlCount = 0;

    public int timer = 0;

	public bool flagTraining = false;

    public void Start()
    {
        SetDifficulty();
    }

    public void SetDifficulty()
    {
        controlCount = (int)FindObjectOfType<Slider>().value;
        timer = (int)Mathf.Floor(0.5f * (controlCount * (controlCount + 1)));
    }

	public void SetTraining(bool stateFlag) {
		flagTraining = stateFlag;
	}
}
