using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManage : MonoBehaviour
{
    public Transform[] spawnLocations;
    public GameObject[] playerPrefabs;
    private GameObject[] spawnedPlayers;

    private void Start()
    {
        //Four players
        spawnedPlayers = new GameObject[4];

        PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
    }

    private void OnDestroy()
    {
        PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
    }

    void OnPlayerJoined(PlayerInput playerInput)
    {
        int playerIndex = playerInput.playerIndex;

        // Check if the player index is within the bounds of the player prefabs array
        if (playerIndex >= 0 && playerIndex < playerPrefabs.Length)
        {

            //Check if spawn location index is within the bounds of the spawn location array
            if(playerIndex >= 0 && playerIndex < spawnLocations.Length)
            {
                //Player will spawn at corresponding spawn location 
                GameObject playerPrefab = playerPrefabs[playerIndex];
                Transform spawnLocation = spawnLocations[playerIndex];
                GameObject spawnedPlayer = Instantiate(playerPrefab, spawnLocation.position, spawnLocation.rotation);

                //Tracking spawned players
                spawnedPlayers[playerIndex] = spawnedPlayer;

                Debug.Log("Player " + playerIndex + " joined the game.");
            }
            else
            {
                Debug.LogWarning("Spawn location index out of range.");
            }
        }
        else
        {
            Debug.LogWarning("Player index out of range for players.");
        }
    }


}
