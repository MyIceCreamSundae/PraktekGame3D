using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomUIManager : MonoBehaviour
{
    public Text roomNameTxt;
    public Transform playerListT;
    public PlayerData playerDataPrefab;

    /// <summary>
    /// void OnEnable() akan otomatis dijalankan ketika gameObject di SetActive(true)
    /// </summary>
    private void OnEnable() {
        roomNameTxt.text = PhotonNetwork.CurrentRoom.Name;
        RefreshPlayerList();
    }

    void RefreshPlayerList() {
        ClearPlayerList();

        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values) {
            AddPlayerList(player.UserId);
        }
    }

    void ClearPlayerList() {
        for (int i = 0; i < playerListT.childCount; i++) {
            Destroy(playerListT.GetChild(i).gameObject);
        }
    }

    void AddPlayerList(string playerName) {
        PlayerData spawnedPlayerData = Instantiate(playerDataPrefab, playerListT);
        spawnedPlayerData.playerNameTxt.text = playerName;
    }

    /// <summary>
    /// Jika kita belum leave room, maka kita tidak akan diperbolehkan membuat room ataupun join room.
    /// </summary>
    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    public void OnPlayerEnteredRoom(Player newPlayer) {
        AddPlayerList(newPlayer.UserId);
    }

    public void OnPlayerLeftRoom(Player otherPlayer) {
        RefreshPlayerList();
    }
}