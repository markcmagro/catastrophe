using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Panel : NetworkBehaviour
{
    [SyncVar]
    public int id = 0;

    [SyncVar]
    public int buttonCount = 0;

    [SyncVar]
    public string correctSequenceString = "";
    /*
    [SyncVar]
    public float red = 0.0f;

    [SyncVar]
    public float green = 0.0f;

    [SyncVar]
    public float blue = 0.0f;
    */
    public SyncListFloat posList = new SyncListFloat();

    private int[] correctSequence = null;

    private ArrayList currentSequence = null;

    public AudioClip goodSequence = null;

    public AudioClip badSequence = null;

    public AudioClip completeSequence = null;

    public AudioClip completeSequenceTutorial = null;

    private AudioSource audioSource = null;

    public AudioSource musicSource = null;

    public AudioSource tutorialSource = null;

    public AudioClip[] musicClips = null;

    public AudioClip[] tutorialClips = null;

    public AudioClip musicClipTutorial = null;

    public Control control1x1Prefab = null;

    public Control control1x2Prefab = null;

    public Control control2x1Prefab = null;

    public Control control2x2Prefab = null;

    public Sprite[] panelSprites = null;

    public PanelVisualGenerator pvgPub = null;

    // Use this for initialization
    void Start()
    {
        Init();
    }

    void Init()
    {
        if(Player.GetLocalPlayer().isTutorial)
            completeSequence = completeSequenceTutorial;
        //buttonCount = FindObjectOfType<NetMgr>().controlCount;
        TypeButtonVisualAspect[] buttonArr;

        MatchMgr matchMgr = FindObjectOfType<MatchMgr>();
        if(!matchMgr.isTutorialMatch)
        {
            buttonArr = new TypeButtonVisualAspect[buttonCount];
            switch(buttonCount)
            {
            case 2:
                buttonArr[0] = TypeButtonVisualAspect.Size2x2;
                buttonArr[1] = TypeButtonVisualAspect.Size2x2;
                break;
            case 3:
                buttonArr[0] = TypeButtonVisualAspect.Size2x2;
                buttonArr[1] = TypeButtonVisualAspect.Size1x1;
                buttonArr[2] = TypeButtonVisualAspect.Size2x2;
                break;
            case 4:
                buttonArr[0] = TypeButtonVisualAspect.Size1x2;
                buttonArr[1] = TypeButtonVisualAspect.Size2x2;
                buttonArr[2] = TypeButtonVisualAspect.Size1x1;
                buttonArr[3] = TypeButtonVisualAspect.Size2x2;
                break;
            case 5:
                buttonArr[0] = TypeButtonVisualAspect.Size2x2;
                buttonArr[1] = TypeButtonVisualAspect.Size1x2;
                buttonArr[2] = TypeButtonVisualAspect.Size2x1;
                buttonArr[3] = TypeButtonVisualAspect.Size1x1;
                buttonArr[4] = TypeButtonVisualAspect.Size2x2;
                break;
            case 6:
                buttonArr[0] = TypeButtonVisualAspect.Size2x2;
                buttonArr[1] = TypeButtonVisualAspect.Size1x1;
                buttonArr[2] = TypeButtonVisualAspect.Size2x1;
                buttonArr[3] = TypeButtonVisualAspect.Size1x2;
                buttonArr[4] = TypeButtonVisualAspect.Size2x1;
                buttonArr[5] = TypeButtonVisualAspect.Size1x2;
                break;
            case 7:
                buttonArr[0] = TypeButtonVisualAspect.Size2x2;
                buttonArr[1] = TypeButtonVisualAspect.Size1x1;
                buttonArr[2] = TypeButtonVisualAspect.Size2x1;
                buttonArr[3] = TypeButtonVisualAspect.Size1x2;
                buttonArr[4] = TypeButtonVisualAspect.Size2x2;
                buttonArr[5] = TypeButtonVisualAspect.Size1x1;
                buttonArr[6] = TypeButtonVisualAspect.Size2x1;
                break;
            }
        }
        else
        {
            buttonCount = 3;
            buttonArr = new TypeButtonVisualAspect[buttonCount];
            buttonArr[0] = TypeButtonVisualAspect.Size2x2;
            buttonArr[1] = TypeButtonVisualAspect.Size2x2;
            buttonArr[2] = TypeButtonVisualAspect.Size2x2;
            tutorialSource.clip = tutorialClips[id - 1];
        }

        GenButtons(buttonArr);

        if(isServer)
        {
            SetUpPanel();
            if(matchMgr.isTutorialMatch)
            {
                for(int btnIdx = 0; btnIdx < pvgPub.listOfButtons.Length; btnIdx++)
                {
                    ButtonVisualAspect curButton = pvgPub.listOfButtons[btnIdx];
                    curButton.transform.localPosition = new Vector3(posList[btnIdx * 3], posList[btnIdx * 3 + 1], -1.0f);
                    curButton.GetComponent<Control>().offSprite = curButton.GetComponent<Control>().controlSprites[(int)posList[btnIdx * 3 + 2]].offSprite;
                    curButton.GetComponent<Control>().onSprite = curButton.GetComponent<Control>().controlSprites[(int)posList[btnIdx * 3 + 2]].onSprite;
                }
            }
        }
        else
        {
            for(int btnIdx = 0; btnIdx < pvgPub.listOfButtons.Length; btnIdx++)
            {
                ButtonVisualAspect curButton = pvgPub.listOfButtons[btnIdx];
                curButton.transform.localPosition = new Vector3(posList[btnIdx * 3], posList[btnIdx * 3 + 1], -1.0f);
                curButton.GetComponent<Control>().offSprite = curButton.GetComponent<Control>().controlSprites[(int)posList[btnIdx * 3 + 2]].offSprite;
                curButton.GetComponent<Control>().onSprite = curButton.GetComponent<Control>().controlSprites[(int)posList[btnIdx * 3 + 2]].onSprite;
            }
        }
        /*
        PanelVisualGenerator pvgen1 = GetComponent<PanelVisualGenerator>();
        for(int btnIdx = 0; btnIdx < pvgen1.listOfButtons.Length; btnIdx++)
        {
            ButtonVisualAspect curButton = pvgen1.listOfButtons[btnIdx];
            curButton.GetComponent<Control>().offSprite = curButton.GetComponent<Control>().controlSprites[(int)posList[btnIdx * 2 + 2]].offSprite;
            curButton.GetComponent<Control>().onSprite = curButton.GetComponent<Control>().controlSprites[(int)posList[btnIdx * 2 + 2]].onSprite;
        }
        */

        GetComponent<SpriteRenderer>().sprite = panelSprites[id - 1];
        // Obtain reference to audio source.
        if(audioSource == null)
            audioSource = GetComponent<AudioSource>();
        // Decode correct sequence string.
        DecodeCorrectSequence();
    }

    public void RandomizePanel()
    {
        // Place the controls 'randomly'.
        pvgPub.DoPlaceButton();

        for(int btnIdx = 0; btnIdx < pvgPub.listOfButtons.Length; btnIdx++)
        {
            ButtonVisualAspect curButton = pvgPub.listOfButtons[btnIdx];
            posList.Add(curButton.transform.localPosition.x);
            posList.Add(curButton.transform.localPosition.y);
            posList.Add(curButton.GetComponent<Control>().subType);
            curButton.GetComponent<Control>().offSprite = curButton.GetComponent<Control>().controlSprites[curButton.GetComponent<Control>().subType].offSprite;
            curButton.GetComponent<Control>().onSprite = curButton.GetComponent<Control>().controlSprites[curButton.GetComponent<Control>().subType].onSprite;
        }
    }

    public void TutorializePanel()
    {
        ButtonVisualAspect curButton = pvgPub.listOfButtons[0];
        posList.Add(-100.0f);
        posList.Add(0.0f);
        posList.Add(curButton.GetComponent<Control>().subType);
        curButton.GetComponent<Control>().offSprite = curButton.GetComponent<Control>().controlSprites[curButton.GetComponent<Control>().subType].offSprite;
        curButton.GetComponent<Control>().onSprite = curButton.GetComponent<Control>().controlSprites[curButton.GetComponent<Control>().subType].onSprite;

        curButton = pvgPub.listOfButtons[1];
        posList.Add(0.0f);
        posList.Add(0.0f);
        posList.Add(curButton.GetComponent<Control>().subType);
        curButton.GetComponent<Control>().offSprite = curButton.GetComponent<Control>().controlSprites[curButton.GetComponent<Control>().subType].offSprite;
        curButton.GetComponent<Control>().onSprite = curButton.GetComponent<Control>().controlSprites[curButton.GetComponent<Control>().subType].onSprite;

        curButton = pvgPub.listOfButtons[2];
        posList.Add(100.0f);
        posList.Add(0.0f);
        posList.Add(curButton.GetComponent<Control>().subType);
        curButton.GetComponent<Control>().offSprite = curButton.GetComponent<Control>().controlSprites[curButton.GetComponent<Control>().subType].offSprite;
        curButton.GetComponent<Control>().onSprite = curButton.GetComponent<Control>().controlSprites[curButton.GetComponent<Control>().subType].onSprite;
    }

    void SetUpPanel()
    {
        MatchMgr matchMgr = FindObjectOfType<MatchMgr>();

        if(!matchMgr.isTutorialMatch)
        {
            // Generate random button positions.
            RandomizePanel();
            // Generate random correct press sequence.
            correctSequence = GetRandomIntSequence(buttonCount);
        }
        else
        {
            TutorializePanel();
            correctSequence = new int[3];
            switch(id)
            {
            case 1:
                correctSequence[0] = 1;
                correctSequence[1] = 2;
                correctSequence[2] = 3;
                break;
            case 2:
                correctSequence[0] = 3;
                correctSequence[1] = 2;
                correctSequence[2] = 1;
                break;
            case 3:
                correctSequence[0] = 1;
                correctSequence[1] = 3;
                correctSequence[2] = 2;
                break;
            case 4:
                correctSequence[0] = 2;
                correctSequence[1] = 1;
                correctSequence[2] = 3;
                break;
            }
        }

        // Encode correct sequence.
        EncodeCorrectSequence();
        // Reset panel.
        ResetPanel();
    }

    void DecodeCorrectSequence()
    {
        string[] seqStrings = correctSequenceString.Split(',');
        correctSequence = new int[seqStrings.Length];

        for(int stringIdx = 0; stringIdx < seqStrings.Length; stringIdx++)
            correctSequence[stringIdx] = int.Parse(seqStrings[stringIdx]);
    }

    public void ResetPanel()
    {
        currentSequence = new ArrayList();
        if(Player.GetLocalPlayer().isTutorial)
            musicSource.clip = musicClipTutorial;
        else
            musicSource.clip = musicClips[id - 1];
        musicSource.Play();
        if(Player.GetLocalPlayer().isTutorial)
            tutorialSource.Play();
        Control[] controls = FindObjectsOfType<Control>();
        for(int cntrlIdx = 0; cntrlIdx < controls.Length; cntrlIdx++)
        {
            controls[cntrlIdx].Reset();
        }
    }

    void EncodeCorrectSequence()
    {
        correctSequenceString = "";

        for(int idx = 0; idx < correctSequence.Length; idx++)
            correctSequenceString += correctSequence[idx].ToString() + ",";

        if(correctSequenceString.Length > 0)
            correctSequenceString = correctSequenceString.Remove(correctSequenceString.Length - 1);
    }

    // Return an array of n integers.
    // Each integer from 1 to n appears exactly once.
    int[] GetRandomIntSequence(int n)
    {
        int[] array;
        int array_size;

        array = new int[n];
        for(int i = 0; i < n; i++)
            array[i] = i + 1;

        array_size = n;

        for(int i = 0; i < n; i++)
        {
            // Choose a random position in the array.
            int pos = Random.Range(0, n - 1);

            // Get the element at position "pos".
            int e = array[pos];

            // Swap the element at position "pos" with the element at position "array_size - 1".
            array[pos] = array[array_size - 1];
            array[array_size - 1] = e;

            // Reduce the array size by 1.
            array_size--;
        }

        return array;
    }

    public void CommitPress(int buttonId)
    {
        currentSequence.Add(buttonId);
        if(CheckSequenceComplete())
        {
            // Signal done to server and wait.
            audioSource.PlayOneShot(completeSequence);
            Player.GetLocalPlayer().StopPlayer(true);
        }
        else
        {
            if(CheckSequenceFail())
            {
                // Reset panel.
                audioSource.PlayOneShot(badSequence);
                ResetPanel();
                Control[] controls = FindObjectsOfType<Control>();
                for(int cntrlIdx = 0; cntrlIdx < controls.Length; cntrlIdx++)
                {
                    StartCoroutine(controls[cntrlIdx].Freeze(1.0f));
                }
            }
            else
            {
                // Good! Continue!
                audioSource.PlayOneShot(goodSequence);
            }
        }
    }

    public bool CheckSequenceComplete()
    {
        if(currentSequence.Count != correctSequence.Length)
            return false;

        bool seqCorrect = true;
        for(int idx = 0; idx < currentSequence.Count; idx++)
        {
            if((int)currentSequence[idx] != correctSequence[idx])
                seqCorrect = false;
        }
        return seqCorrect;
    }

    public bool CheckSequenceFail()
    {
        bool seqFail = false;
        for(int idx = 0; idx < currentSequence.Count; idx++)
        {
            if((int)currentSequence[idx] != correctSequence[idx])
                seqFail = true;
        }
        return seqFail;
    }

    public void GenButtons(TypeButtonVisualAspect[] buttonAspects)
    {
        pvgPub.listOfButtons = new ButtonVisualAspect[buttonAspects.Length];
        for(int btnIdx = 0; btnIdx < buttonAspects.Length; btnIdx++)
        {
            Control newControl = null;
            switch(buttonAspects[btnIdx])
            {
            case TypeButtonVisualAspect.Size1x1:
                newControl = GameObject.Instantiate<Control>(control1x1Prefab);
                break;
            case TypeButtonVisualAspect.Size1x2:
                newControl = GameObject.Instantiate<Control>(control1x2Prefab);
                break;
            case TypeButtonVisualAspect.Size2x1:
                newControl = GameObject.Instantiate<Control>(control2x1Prefab);
                break;
            case TypeButtonVisualAspect.Size2x2:
                newControl = GameObject.Instantiate<Control>(control2x2Prefab);
                break;
            }

            int controlSubType = Random.Range(0, newControl.controlSprites.Length);
            newControl.subType = controlSubType;

            newControl.id = btnIdx + 1;
            newControl.transform.parent = transform;
            newControl.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);
            pvgPub.listOfButtons[btnIdx] = newControl.GetComponent<ButtonVisualAspect>();
        }
    }
}
