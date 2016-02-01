using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MatchMgr : NetworkBehaviour
{
    public Panel panelPrefab = null;

    public int playerCount = 4;

    // Connected players.
    private Player[] players = null;

    // Zero-based, goes from 0 to 3 in sequence.
    // Set to 0 at the start of a level.
    // Incremented after each round.
    private int round_count;

    // Sequence index;
    // Zero-based, goes from 0 to 2, chosen at random.
    // Set at the start of a level.
    private int sequence_idx;

    public bool isTutorialMatch = false;

    // Describes which panel is given to each player in each round.
    private int[,,] sequence_array = new int[3, 4, 4]
    {
        { { 1, 2, 3, 4 }, { 4, 1, 2, 3 }, { 2, 3, 4, 1 }, { 3, 4, 1, 2 } },
        { { 1, 2, 3, 4 }, { 2, 4, 1, 3 }, { 3, 1, 4, 2 }, { 4, 3, 2, 1 } },
        { { 1, 2, 3, 4 }, { 3, 4, 2, 1 }, { 4, 3, 1, 2 }, { 2, 1, 4, 3 } }
    };

    private int roundsLeft = 4;

    // Use this for initialization
    void Start()
    {
        if(isServer)
            InitServer();
    }

    public void InitServer()
    {
        round_count = 0;
        sequence_idx = Random.Range(0, 2);

        // Spawn panels for all players.
        SpawnPanels();

        // Wait for players to connect to game.
        StartCoroutine(WaitForPlayersToConnect());
    }

    void SpawnPanels()
    {
        // Spawn 4 panels, and assign IDs.
        Panel panel1 = GameObject.Instantiate(panelPrefab);
        NetworkServer.Spawn(panel1.gameObject);
        panel1.transform.position = new Vector3(0.0f, 0.0f, 10.0f);
        panel1.id = 1;
        panel1.buttonCount = FindObjectOfType<NetMgr>().controlCount;
        Panel panel2 = GameObject.Instantiate(panelPrefab);
        NetworkServer.Spawn(panel2.gameObject);
        panel2.transform.position = new Vector3(0.0f, 0.0f, 10.0f);
        panel2.id = 2;
        panel2.buttonCount = FindObjectOfType<NetMgr>().controlCount;
        Panel panel3 = GameObject.Instantiate(panelPrefab);
        NetworkServer.Spawn(panel3.gameObject);
        panel3.transform.position = new Vector3(0.0f, 0.0f, 10.0f);
        panel3.id = 3;
        panel3.buttonCount = FindObjectOfType<NetMgr>().controlCount;
        Panel panel4 = GameObject.Instantiate(panelPrefab);
        NetworkServer.Spawn(panel4.gameObject);
        panel4.transform.position = new Vector3(0.0f, 0.0f, 10.0f);
        panel4.id = 4;
        panel4.buttonCount = FindObjectOfType<NetMgr>().controlCount;
    }

    // Get the panel assignment for a specific round.
    // round goes from 0 to 3.
    int[] GetPanelAssignment(int round)
    {
        int[] array;

        array = new int[4];
        for (int i = 0; i < 4; i++)
            array[i] = sequence_array[sequence_idx, round, i];

        return array;
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

    public IEnumerator WaitForPlayersToConnect()
    {
        // Wait for players to connect.
        int prevPlayerCount = 0;
        while(FindObjectsOfType<Player>().Length < playerCount)
        {
            if(prevPlayerCount != FindObjectsOfType<Player>().Length)
            {
                prevPlayerCount = FindObjectsOfType<Player>().Length;
                Debug.Log("Player " + prevPlayerCount + " has connected.");
            }
            yield return new WaitForSeconds(0.5f);
        }

        // Assign IDs to players.
        players = FindObjectsOfType<Player>();
        Debug.Log("Player " + players.Length + " has connected.");

        // Assign panels to players randomly.
        ReassignPanels();
        yield return null;

        for(int playerIdx = 0; playerIdx < players.Length; playerIdx++)
        {
            if(!isTutorialMatch)
                players[playerIdx].roundTime = FindObjectOfType<NetMgr>().timer;
            else
                players[playerIdx].roundTime = 20.0f;
            players[playerIdx].id = playerIdx + 1;
            // Tell players to set up match.
            players[playerIdx].RpcStartMatch();
        }

        StartCoroutine(WaitForPlayersDone());

        // Tell players to start first round.
        //StartCoroutine(StartRound());
    }

    public IEnumerator StartRound()
    {
        // Assign panels to players randomly.
        ReassignPanels();
        yield return null;

        for(int playerIdx = 0; playerIdx < players.Length; playerIdx++)
        {
            Player curPlayer = players[playerIdx];
            // Reset player done flags.
            curPlayer.isDone = false;
            curPlayer.isSuccessful = false;
            // Tell players to start round.
            players[playerIdx].RpcStartRound();
        }

        // Wait for all players to finish their round.
        StartCoroutine(WaitForPlayersDone());
    }

    public IEnumerator WaitForPlayersDone()
    {
        bool playersDone;
        // Loop while not all players are done.
        do
        {
            yield return new WaitForSeconds(0.5f);
            playersDone = true;
            for(int playerIdx = 0; playerIdx < players.Length; playerIdx++)
                playersDone = playersDone && players[playerIdx].isDone;
        } while(!playersDone);

        Debug.Log("All players are done.");

        // Check success status.
        if(!CheckResult())
        {
            if(!isTutorialMatch)
                roundsLeft--;

            if(roundsLeft > 0)
            {
                Debug.Log("No solution. Moving to next round.");
                // Wait a few seconds before progressing to next round.
                if(!isTutorialMatch)
                    yield return new WaitForSeconds(3.0f);
                else
                    yield return new WaitForSeconds(15.0f);
                // Move to next round.
                round_count = (round_count + 1) % 4;
                // Tell players to start next round.
                StartCoroutine(StartRound());
            }
            else
            {
                Debug.Log("Game over!");
                PlayersLose();
                //Application.LoadLevel("menu");
            }
        }
        else
        {
            PlayersWin();
            //Application.LoadLevel("menu");
        }
    }

    public void ReassignPanels()
    {
        Debug.Log("Reassigning panels.");

        int[] panelIds = GetPanelAssignment(round_count);
        for(int playerIdx = 0; playerIdx < players.Length; playerIdx++)
        {
            players[playerIdx].panelId = panelIds[playerIdx];
            if(isTutorialMatch)
                players[playerIdx].isTutorial = true;
        }
    }

    public bool CheckResult()
    {
        for(int playerIdx = 0; playerIdx < players.Length; playerIdx++)
        {
            if(!players[playerIdx].isSuccessful)
                return false;
        }

        return true;
    }

    public void PlayersWin()
    {
        for(int playerIdx = 0; playerIdx < players.Length; playerIdx++)
            players[playerIdx].RpcWin();
    }

    public void PlayersLose()
    {
        for(int playerIdx = 0; playerIdx < players.Length; playerIdx++)
            players[playerIdx].RpcLose();
    }
}
