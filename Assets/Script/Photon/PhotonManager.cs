using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
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
    public InputField playerIdInp; 
    public InputField roomNameInp;

    [Header("Photon Room & List Setup")]
    public RoomUIManager roomUIManager;
    // Memastikan penulisan variabel seragam agar error CS0103 hilang
    public Transform roomListContent; 
    public RoomInfoData roomInfoDataPrefab; 

    void SaatDiMenuLogin() {
        if (loginMenuObj != null) loginMenuObj.SetActive(true);
        if (lobbyMenuObj != null) lobbyMenuObj.SetActive(false);
        if (roomMenuObj != null) roomMenuObj.SetActive(false);
    }

    void SaatDiMenuLobby() {
        if (loginMenuObj != null) loginMenuObj.SetActive(false);
        if (lobbyMenuObj != null) lobbyMenuObj.SetActive(true);
        if (roomMenuObj != null) roomMenuObj.SetActive(false);
    }

    void SaatDiDalamRoom() {
        if (loginMenuObj != null) loginMenuObj.SetActive(false);
        if (lobbyMenuObj != null) lobbyMenuObj.SetActive(false);
        if (roomMenuObj != null) roomMenuObj.SetActive(true);
    }

    void Start()
    {
        SaatDiMenuLogin();
    }

    void Update()
    {
        if (connectionStatusTxt != null) {
            connectionStatusTxt.text = "Status: " + PhotonNetwork.NetworkClientState.ToString();
        }
    }

    public void LoginWithPlayFab() {
        if (playerIdInp == null || string.IsNullOrEmpty(playerIdInp.text)) {
            Debug.LogError("ID Player tidak boleh kosong!");
            return;
        }

        Debug.Log("Mengautentikasi ke PlayFab...");
        var request = new LoginWithCustomIDRequest {
            CustomId = playerIdInp.text,
            CreateAccount = true 
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnPlayFabLoginSuccess, OnPlayFabLoginFailed);
    }

    private void OnPlayFabLoginSuccess(LoginResult result) {
        Debug.Log("<color=green>PlayFab Login Sukses!</color> Menyambungkan ke Photon...");
        
        PhotonNetwork.AuthValues = new AuthenticationValues {
            UserId = playerIdInp.text
        };

        PhotonNetwork.ConnectUsingSettings();
    }

    private void OnPlayFabLoginFailed(PlayFabError error) {
        Debug.LogError("PlayFab Login Gagal: " + error.GenerateErrorReport());
    }

    public override void OnConnectedToMaster()
    {
        SaatDiMenuLobby();
        if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMaster)
        {
            PhotonNetwork.JoinLobby(); 
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("<color=green>Pemain berhasil masuk ke Lobby Jaringan!</color>");
        SaatDiMenuLobby();
    }

    public void CreateRoom() {
        if (roomNameInp == null || string.IsNullOrEmpty(roomNameInp.text)) {
            Debug.LogError("Nama Room tidak boleh kosong!");
            return;
        }

        // Proteksi: Hanya boleh Create jika statusnya sudah di dalam Lobby / Master Server
        if (!PhotonNetwork.InLobby && PhotonNetwork.NetworkClientState != ClientState.ConnectedToMaster) {
            Debug.LogWarning("Belum siap membuat room. Status saat ini: " + PhotonNetwork.NetworkClientState);
            return;
        }

        RoomOptions roomOptions = new RoomOptions {
            MaxPlayers = 2, 
            PublishUserId = true
        };

        PhotonNetwork.CreateRoom(roomNameInp.text, roomOptions);
    }

    public void JoinRoom() {
        if (roomNameInp == null || string.IsNullOrEmpty(roomNameInp.text)) return;

        // PROTEKSI BARU: Mencegah error 'Client is not ready for operations'
        if (!PhotonNetwork.InLobby && PhotonNetwork.NetworkClientState != ClientState.ConnectedToMaster) {
            Debug.LogWarning("Belum siap bergabung ke room. Status jaringan saat ini: " + PhotonNetwork.NetworkClientState);
            return;
        }

        Debug.Log("Mencoba bergabung ke room: " + roomNameInp.text);
        PhotonNetwork.JoinRoom(roomNameInp.text);
    }

    public override void OnCreatedRoom()
    {
        print("OnCreatedRoom!");
    }

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
        if (roomListContent == null || roomInfoDataPrefab == null) return;

        foreach (RoomInfo roomInfo in roomList) {
            if (roomInfo.RemovedFromList || !roomInfo.IsOpen || !roomInfo.IsVisible || roomInfo.PlayerCount == roomInfo.MaxPlayers) continue;

            RoomInfoData spawnedRoom = Instantiate(roomInfoDataPrefab, roomListContent);
            spawnedRoom.Setup(roomInfo);
        }
    }

    void ClearRoom() {
        if (roomListContent == null) return;
        
        for (int i = roomListContent.childCount - 1; i >= 0; i--) {
            Destroy(roomListContent.GetChild(i).gameObject);
        }
    }
}