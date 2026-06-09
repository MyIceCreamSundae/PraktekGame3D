using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

// --- PERBAIKAN 1: Mengubah MonoBehaviour menjadi MonoBehaviourPunCallbacks ---
public class RoomUIManager : MonoBehaviourPunCallbacks
{
    public Text roomNameTxt;
    public Transform playerListT;
    public PlayerData playerDataPrefab;

    /// <summary>
    /// void OnEnable() akan otomatis dijalankan ketika gameObject di SetActive(true)
    /// </summary>
    private void OnEnable() {
        // Mencegah error jika sewaktu-waktu CurrentRoom belum siap saat diakses
        if (PhotonNetwork.CurrentRoom != null && roomNameTxt != null) {
            roomNameTxt.text = PhotonNetwork.CurrentRoom.Name;
            RefreshPlayerList();
        }
    }

    void RefreshPlayerList() {
        ClearPlayerList();

        if (PhotonNetwork.CurrentRoom == null || playerListT == null) return;

        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values) {
            // Gunakan NickName jika ada, jika kosong gunakan UserId sebagai cadangan
            string displayName = string.IsNullOrEmpty(player.NickName) ? player.UserId : player.NickName;
            AddPlayerList(displayName);
        }
    }

    void ClearPlayerList() {
        if (playerListT == null) return;
        
        for (int i = 0; i < playerListT.childCount; i++) {
            Destroy(playerListT.GetChild(i).gameObject);
        }
    }

    void AddPlayerList(string playerName) {
        if (playerDataPrefab == null || playerListT == null) return;

        PlayerData spawnedPlayerData = Instantiate(playerDataPrefab, playerListT);
        if (spawnedPlayerData != null && spawnedPlayerData.playerNameTxt != null) {
            spawnedPlayerData.playerNameTxt.text = playerName;
        }
    }

    /// <summary>
    /// Jika kita belum leave room, maka kita tidak akan diperbolehkan membuat room ataupun join room.
    /// </summary>
    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    // --- PERBAIKAN 2: Menambahkan 'override' agar fungsi callback jaringan Photon aktif mendeteksi player lain ---
    public override void OnPlayerEnteredRoom(Player newPlayer) {
        string displayName = string.IsNullOrEmpty(newPlayer.NickName) ? newPlayer.UserId : newPlayer.NickName;
        AddPlayerList(displayName);
    }

    // --- PERBAIKAN 3: Menambahkan 'override' untuk mendeteksi saat player lain kabur/keluar ---
    public override void OnPlayerLeftRoom(Player otherPlayer) {
        RefreshPlayerList();
    }
}