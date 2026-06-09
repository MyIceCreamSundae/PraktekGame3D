using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomInfoData : MonoBehaviour
{
    public Text roomNameTxt, playersCountTxt;

    public void Setup(RoomInfo roomInfo) {
        roomNameTxt.text = roomInfo.Name;

        string maxPlayer;
        if (roomInfo.MaxPlayers <= 0) {
            maxPlayer = "unlimited";
        } else {
            maxPlayer = roomInfo.MaxPlayers.ToString();
        }

        playersCountTxt.text = roomInfo.PlayerCount + " / " + maxPlayer;
    }

    public void JoinRoom() {
        // PERBAIKAN: Deteksi isi teks sebelum mengirim perintah ke Photon
        if (roomNameTxt == null || string.IsNullOrEmpty(roomNameTxt.text)) {
            Debug.LogError("<color=red>[Error Join]</color> Komponen roomNameTxt kosong atau belum dihubungkan di Inspector prefab!");
            return;
        }

        Debug.Log("<color=yellow>[Mencoba Join via Tombol Prefab]</color> Mengirim nama room: " + roomNameTxt.text);
        PhotonNetwork.JoinRoom(roomNameTxt.text);
    }
}