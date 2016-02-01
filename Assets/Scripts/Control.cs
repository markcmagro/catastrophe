using UnityEngine;
using System.Collections;

public class Control : MonoBehaviour
{
    [System.Serializable]
    public class ControlSprites
    {
        public Sprite offSprite = null;
        public Sprite onSprite = null;
    };

    public int id = 0;

    public AudioClip pressClip = null;

    private BoxCollider2D boxCollider = null;

    private AudioSource audioSource = null;

    public Sprite offSprite = null;

    public Sprite onSprite = null;

    public ControlSprites[] controlSprites = new ControlSprites[0];

    public int subType = 0;

    public bool isPressed = false;

    private bool isFrozen = false;

    // Use this for initialization
    void Start()
    {
        Init();
    }

    void Init()
    {
        if(boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();
        if(audioSource == null)
            audioSource = GetComponent<AudioSource>();

        //offSprite = controlSprites[0].offSprite;
        //onSprite = controlSprites[0].onSprite;
    }

    // Update is called once per frame
    void Update()
    {
        if(Player.GetLocalPlayer().playerState != Player.PlayerState.playing || isPressed || isFrozen)
            return;

        if(Input.GetMouseButtonDown(0))
        {
            if(boxCollider.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                Debug.Log("Control hit!");
                isPressed = true;
                GetComponent<SpriteRenderer>().sprite = onSprite;
                audioSource.PlayOneShot(pressClip);
                transform.parent.GetComponent<Panel>().CommitPress(id);
            }
        }
        else if(Input.touchCount > 0)
        {
            if(boxCollider.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position)))
            {
                Debug.Log("Control hit!");
                isPressed = true;
                GetComponent<SpriteRenderer>().sprite = onSprite;
                audioSource.PlayOneShot(pressClip);
                transform.parent.GetComponent<Panel>().CommitPress(id);
            }
        }
    }

    public void Reset()
    {
        isPressed = false;
        GetComponent<SpriteRenderer>().sprite = offSprite;
    }

    public IEnumerator Freeze(float time)
    {
        isFrozen = true;
        while(time > 0.0f)
        {
            yield return null;
            time -= Time.deltaTime;
        }
        isFrozen = false;
    }
}
