using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : NetworkBehaviour
{
    public enum PlayerState
    {
        waitStartRound,
        playing,
        waitEndRound
    };

    [SyncVar]
    public int id;

    // ID of panel currently operated by this player.
    [SyncVar]
    public int panelId = 0;

    // Determines whether a player is done with his turn (solved or timer elapsed).
    [SyncVar]
    public bool isDone = false;

    // Determines whether a player has solved his panel.
    [SyncVar]
    public bool isSuccessful = false;

    // Round time.
    [SyncVar]
    public float roundTime = 5.0f;

    // Round time.
    [SyncVar]
    public bool isTutorial = false;

    // Round timer for this player.
    public float timer = 0.0f;

    public PlayerState playerState = PlayerState.waitStartRound;

    private Panel[] panels = null;

    public AudioClip timerClip = null;

    public AudioClip timeoutClip = null;

    public AudioClip timeoutClipTutorial = null;

    public AudioClip enterLobbyClip = null;

    public AudioClip exitLobbyClip = null;

    private AudioSource audioSource = null;

    // Use this for initialization
    void Start()
    {
        Init();
    }

    // Sets up the player.
    [Client]
    void Init()
    {
        // If this player is the current player, activate its camera.
        if(isLocalPlayer)
        {
            if(audioSource == null)
                audioSource = GetComponent<AudioSource>();
            GetComponent<Camera>().enabled = true;
            GetComponent<AudioListener>().enabled = true;
            audioSource.enabled = true;
            GetComponent<Camera>().tag = "MainCamera";
            GetComponent<Camera>().enabled = false;

            // Obtain references to all panels.
            panels = FindObjectsOfType<Panel>();

            PlayerConnected();
        }
    }

    public void SetUpRound()
    {
        timer = roundTime;
        for(int panelIdx = 0; panelIdx < panels.Length; panelIdx++)
        {
            Panel curPanel = panels[panelIdx];
            // Take ownership of panel and reset it.
            if(curPanel.id == panelId)
            {
                curPanel.gameObject.SetActive(true);
                curPanel.ResetPanel();
            }
            else
            {
                // Disable other panels.
                curPanel.gameObject.SetActive(false);
            }
        }
    }

    public void Update()
    {
        if(isLocalPlayer)
        {
            switch(playerState)
            {
            case PlayerState.playing:
                timer -= Time.deltaTime;
                if(timer <= audioSource.clip.length && !audioSource.isPlaying)
                {
                    audioSource.time = audioSource.clip.length - timer;
                    audioSource.Play();
                }
                if(timer <= 0.0f)
                {
                    audioSource.PlayOneShot(timeoutClip);
                    StopPlayer(false);
                }
                break;
            }
        }
    }

    public void StopPlayer(bool sequenceComplete)
    {
        Debug.Log("Stopping round!");
        for(int panelIdx = 0; panelIdx < panels.Length; panelIdx++)
        {
            Panel curPanel = panels[panelIdx];
            if(curPanel.id == panelId)
            {
                curPanel.musicSource.Stop();
                curPanel.tutorialSource.Stop();
            }
        }
        playerState = PlayerState.waitEndRound;
        CmdSetPlayerDone(true, sequenceComplete);
    }

    [Command]
    public void CmdSetPlayerDone(bool isDone, bool isSuccessful)
    {
        this.isSuccessful = isSuccessful;
        this.isDone = isDone;
    }

    [ClientRpc]
    public void RpcStartMatch()
    {
        StartCoroutine(StartMatch());
    }

    public IEnumerator StartMatch()
    {
        if(isLocalPlayer)
        {
            if(isTutorial)
                timeoutClip = timeoutClipTutorial;
            audioSource.Stop();
            audioSource.clip = exitLobbyClip;
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
            audioSource.Stop();
            audioSource.clip = timerClip;
            StartRound();
        }
    }

    [ClientRpc]
    public void RpcStartRound()
    {
        StartRound();
    }

    public void StartRound()
    {
        if(isLocalPlayer)
        {
            Debug.Log("Starting round!");
            SetUpRound();
            playerState = PlayerState.playing;
            GetComponent<Camera>().enabled = enabled;
        }
    }

    public static Player GetLocalPlayer()
    {
        Player[] players = FindObjectsOfType<Player>();
        for(int playerIdx = 0; playerIdx < players.Length; playerIdx++)
        {
            Player curPlayer = players[playerIdx];
            if(curPlayer.isLocalPlayer)
                return curPlayer;
        }
        return null;
    }

    [ClientRpc]
    public void RpcWin()
    {
        if(isLocalPlayer)
        {
            Application.LoadLevel("win");
        }
    }

    [ClientRpc]
    public void RpcLose()
    {
        if(isLocalPlayer)
        {
            Application.LoadLevel("lose");
        }
    }

    public void PlayerConnected()
    {
        if(isLocalPlayer)
        {
            for(int panelIdx = 0; panelIdx < panels.Length; panelIdx++)
            {
                Panel curPanel = panels[panelIdx];
                //curPanel.gameObject.SetActive(false);
            }
            audioSource.clip = enterLobbyClip;
            audioSource.Play();
        }
    }
}
