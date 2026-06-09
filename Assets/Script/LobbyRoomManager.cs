using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyRoomManager : MonoBehaviourPunCallbacks
{
    [Header("UI Panels")]
    public GameObject lobbyMenuObj;
    public GameObject roomMenuObj;

    [Header("Room Create Inputs")]
    public InputField roomNameInp;

    [Header("Scroll View Room List Setup")]
    public Transform roomListContent; // Tempat prefab room dilahirkan (Viewport->Content)
    public RoomInfoData roomInfoPrefab; // Prefab UI untuk menampung data room

    // Fungsi untuk tombol Create Room
    public void CreateNewRoom()
    {
        if (roomNameInp == null) return;

        string roomName = roomNameInp.text;
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("Nama Room tidak boleh kosong!");
            return;
        }

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2, // Batasan player sesuai kebutuhan multiplayer kamu
            PublishUserId = true
        };

        Debug.Log("Mencoba membuat room: " + roomName);
        PhotonNetwork.CreateRoom(roomName, options);
    }

    // Fungsi untuk tombol Join Room secara manual via input text
    public void JoinRoomViaInput()
    {
        if (roomNameInp == null) return;

        string roomName = roomNameInp.text;
        if (!string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.JoinRoom(roomName);
        }
    }

    // Callback saat berhasil masuk ke dalam room (baik via Create maupun Join)
    public override void OnJoinedRoom()
    {
        Debug.Log("<color=green>Berhasil masuk ke dalam room: </color>" + PhotonNetwork.CurrentRoom.Name);
        
        // Tukar panel dari Lobby ke dalam Room UI
        if (lobbyMenuObj != null) lobbyMenuObj.SetActive(false);
        if (roomMenuObj != null) roomMenuObj.SetActive(true);
    }

    // Callback otomatis dari Photon ketika ada perubahan status room di lobby
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Mendapat update daftar Room dari server.");
        ClearOldRoomList();

        if (roomListContent == null || roomInfoPrefab == null) return;

        foreach (RoomInfo room in roomList)
        {
            // Jangan tampilkan room di list jika room tersebut dihapus dari master, 
            // ditutup (closed), invisible, atau kapasitasnya sudah penuh.
            if (room.RemovedFromList || !room.IsOpen || !room.IsVisible || room.PlayerCount >= room.MaxPlayers)
            {
                continue;
            }

            // Spawn UI item baru ke dalam Scroll View Content
            RoomInfoData spawnedRoomItem = Instantiate(roomInfoPrefab, roomListContent);
            if (spawnedRoomItem != null)
            {
                spawnedRoomItem.Setup(room);
            }
        }
    }

    // Fungsi helper untuk membersihkan list lama di UI agar tidak duplikat menumpuk
    private void ClearOldRoomList()
    {
        if (roomListContent == null) return;

        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }
    }
}