using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [Header("UI Menus")]
    public GameObject loginMenuObj;
    public GameObject lobbyMenuObj;
    public GameObject roomMenuObj;

    [Header("UI Elements")]
    public Text connectionStatusTxt;
    public InputField roomNameInp;

    [Header("Photon Room & List Setup")]
    public RoomUIManager roomUIManager;
    // --- KEDUA VARIABEL DI BAWAH INI YANG SEBELUMNYA REBUT/BELUM ADA ---
    public Transform roomListContent; 
    public RoomInfoData roomInfoDataPrefab; 

    void SaatDiMenuLogin() {
        loginMenuObj.SetActive(true);
        lobbyMenuObj.SetActive(false);
        roomMenuObj.SetActive(false);
    }

    void SaatDiMenuLobby() {
        loginMenuObj.SetActive(false);
        lobbyMenuObj.SetActive(true);
        roomMenuObj.SetActive(false);
    }

    void SaatDiDalamRoom() {
        loginMenuObj.SetActive(false);
        lobbyMenuObj.SetActive(false);
        roomMenuObj.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        SaatDiMenuLogin();
    }

    // Update is called once per frame
    void Update()
    {
        if (connectionStatusTxt != null) {
            connectionStatusTxt.text = PhotonNetwork.NetworkClientState.ToString();
        }
    }

    public void LoginWithId(string id) {
        PhotonNetwork.AuthValues = new AuthenticationValues {
            UserId = id
        };

        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// Dipanggil otomatis ketika status sudah "ConnectedToMaster"
    /// </summary>
    public override void OnConnectedToMaster()
    {
        SaatDiMenuLobby();
        PhotonNetwork.JoinLobby(); //agar kita bisa mendapatkan Room List, maka kita harus JoinLobby terlebih dahulu....
    }

    public void CreateRoom() {
        RoomOptions roomOptions = new RoomOptions {
            MaxPlayers = 2, //Karena pada tutorial kali ini harus bermain dengan 2 player
            PublishUserId = true
        };

        PhotonNetwork.CreateRoom(roomNameInp.text, roomOptions);
    }

    public void JoinRoom() {
        PhotonNetwork.JoinRoom(roomNameInp.text);
    }

    /// <summary>
    /// Dipanggil otomatis ketika room berhasil dibuat
    /// </summary>
    public override void OnCreatedRoom()
    {
        print("OnCreatedRoom!");
        //Note: OnJoinedRoom akan dipanggil Setelah OnCreatedRoom
    }

    /// <summary>
    /// Dipanggil otomatis ketika status sudah "Joined"
    /// </summary>
    public override void OnJoinedRoom()
    {
        print("OnJoinedRoom!");
        SaatDiDalamRoom();
    }

    public override void OnDisconnected(DisconnectCause cause) {
        Debug.LogError("Disconnected karena: " + cause.ToString());
        SaatDiMenuLogin();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoom();
        foreach (RoomInfo roomInfo in roomList) {
            if (roomInfo.PlayerCount == roomInfo.MaxPlayers) continue; //Jika room sudah full, maka tidak perlu ditampilkan

            RoomInfoData spawnedRoom = Instantiate(roomInfoDataPrefab, roomListContent);
            spawnedRoom.Setup(roomInfo);
        }
    }

    void ClearRoom() {
        if (roomListContent == null) return;
        
        for (int i = 0; i < roomListContent.childCount; i++) {
            Destroy(roomListContent.GetChild(i).gameObject);
        }
    }
}