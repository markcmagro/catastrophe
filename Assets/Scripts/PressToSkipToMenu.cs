using UnityEngine;
using System.Collections;

public class PressToSkipToMenu : MonoBehaviour
{
    float timer = 1.0f;
    // Update is called once per frame
    void Update()
    {
        if(timer > 0.0f)
        {
            timer -= Time.deltaTime;
            return;
        }

        if(Input.anyKeyDown || Input.touchCount > 0)
            Application.LoadLevel("menu");
    }
}
