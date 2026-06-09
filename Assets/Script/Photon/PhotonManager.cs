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
    public InputField playerIdInp; // -> Tambahan baru untuk menangkap Input ID Player saat login
    public InputField roomNameInp;

    [Header("Photon Room & List Setup")]
    public RoomUIManager roomUIManager;
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

    // --- SEKARANG LOGIN LEWAT PLAYFAB DULU ---
    public void LoginWithPlayFab() {
        if (playerIdInp == null || string.IsNullOrEmpty(playerIdInp.text)) {
            Debug.LogError("ID Player tidak boleh kosong!");
            return;
        }

        Debug.Log("Mengautentikasi ke PlayFab...");
        var request = new LoginWithCustomIDRequest {
            CustomId = playerIdInp.text,
            CreateAccount = true // Otomatis daftar jika ID belum ada
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnPlayFabLoginSuccess, OnPlayFabLoginFailed);
    }

    private void OnPlayFabLoginSuccess(LoginResult result) {
        Debug.Log("<color=green>PlayFab Login Sukses!</color> Menyambungkan ke Photon...");
        
        // Setelah PlayFab sukses, baru oper ID-nya ke Photon
        PhotonNetwork.AuthValues = new AuthenticationValues {
            UserId = playerIdInp.text
        };

        PhotonNetwork.ConnectUsingSettings();
    }

    private void OnPlayFabLoginFailed(PlayFabError error) {
        Debug.LogError("PlayFab Login Gagal: " + error.GenerateErrorReport());
    }

   /// <summary>
    /// Dipanggil otomatis ketika status sudah "ConnectedToMaster"
    /// </summary>
    public override void OnConnectedToMaster()
    {
        SaatDiMenuLobby();
        
        // --- PERBAIKAN: Berikan proteksi agar tidak memaksa JoinLobby jika statusnya belum siap ---
        if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMaster)
        {
            Debug.Log("Status valid, mencoba bergabung ke Lobby...");
            PhotonNetwork.JoinLobby(); 
        }
        else
        {
            Debug.LogWarning("JoinLobby ditunda karena status saat ini: " + PhotonNetwork.NetworkClientState);
        }
    }

    public void CreateRoom() {
        if (roomNameInp == null || string.IsNullOrEmpty(roomNameInp.text)) {
            Debug.LogError("Nama Room tidak boleh kosong!");
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
        
        for (int i = 0; i < roomListContent.childCount; i++) {
            Destroy(roomListContent.GetChild(i).gameObject);
        }
    }
}