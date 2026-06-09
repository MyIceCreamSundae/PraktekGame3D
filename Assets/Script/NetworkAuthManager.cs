using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using Photon.Realtime;

public class NetworkAuthManager : MonoBehaviourPunCallbacks
{
    [Header("UI Panels")]
    public GameObject loginMenuObj;
    public GameObject lobbyMenuObj;
    public GameObject roomMenuObj;

    [Header("UI Inputs & Texts")]
    public InputField playerCustomIdInp;
    public Text connectionStatusTxt;

    void Start()
    {
        // Pastikan saat game mulai, hanya menu login yang aktif
        if (loginMenuObj != null) loginMenuObj.SetActive(true);
        if (lobbyMenuObj != null) lobbyMenuObj.SetActive(false);
        if (roomMenuObj != null) roomMenuObj.SetActive(false);
    }

    void Update()
    {
        // Menampilkan status jaringan Photon secara real-time di layar
        if (connectionStatusTxt != null)
        {
            connectionStatusTxt.text = "Status: " + PhotonNetwork.NetworkClientState.ToString();
        }
    }

    // Fungsi utama yang dipanggil saat player submit ID mereka
    public void StartNetworkLogin()
    {
        if (playerCustomIdInp == null) return;

        string inputId = playerCustomIdInp.text;

        if (string.IsNullOrEmpty(inputId))
        {
            Debug.LogError("ID tidak boleh kosong!");
            return;
        }

        Debug.Log("Mengautentikasi ke PlayFab...");
        
        var request = new LoginWithCustomIDRequest
        {
            CustomId = inputId,
            CreateAccount = true // Otomatis buat akun baru di PlayFab jika belum terdaftar
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnPlayFabLoginSuccess, OnPlayFabLoginFailed);
    }

    private void OnPlayFabLoginSuccess(LoginResult result)
    {
        Debug.Log("<color=green>PlayFab Login Sukses!</color> ID: " + result.PlayFabId);

        // Setelah sukses di PlayFab, gunakan ID tersebut untuk login ke Photon PUN2
        PhotonNetwork.AuthValues = new AuthenticationValues
        {
            UserId = playerCustomIdInp.text // Set UserId unik untuk Photon
        };

        Debug.Log("Menghubungkan ke Photon Master Server..."); // ---> SUDAH DI-FIX DARI Debug.Obj MENJADI Debug.Log
        PhotonNetwork.ConnectUsingSettings();
    }

    private void OnPlayFabLoginFailed(PlayFabError error)
    {
        Debug.LogError("PlayFab Login Gagal: " + error.GenerateErrorReport());
    }

    // Callback otomatis dari Photon jika berhasil konek ke Master Server
    public override void OnConnectedToMaster()
    {
        Debug.Log("<color=green>Terhubung ke Photon Master Server!</color>");
        
        // Pindah panel dari Login ke Lobby
        if (loginMenuObj != null) loginMenuObj.SetActive(false);
        if (lobbyMenuObj != null) lobbyMenuObj.SetActive(true);

        // Join ke lobby default agar bisa menerima update daftar room (Room List)
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Terputus dari jaringan karena: " + cause.ToString());
        // Jika terputus, paksa balik ke menu login
        if (loginMenuObj != null) loginMenuObj.SetActive(true);
        if (lobbyMenuObj != null) lobbyMenuObj.SetActive(false);
        if (roomMenuObj != null) roomMenuObj.SetActive(false);
    }
}