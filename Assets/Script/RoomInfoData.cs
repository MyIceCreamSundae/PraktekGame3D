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
        PhotonNetwork.JoinRoom(roomNameTxt.text);
    }
}