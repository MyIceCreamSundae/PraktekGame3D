using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks {

    public static GameManager instance;

    public CoinScript coinScript;

    public Text[] playerName, playerScoreTxt;
    public int[] playerScore;

    public Text winTxt;

    int maxScore = 20;

    void Awake()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            playerName[i].text = PhotonNetwork.PlayerList[i].UserId;
        }

        instance = this;

        coinScript.gameObject.SetActive(false);

        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { "IsReadyForBattle", true }
        });
    }

    /// <summary>
    /// Dipanggil otomatis ketika ada player yang berubah Custom Properties-nya
    /// </summary>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // Jika koin belum aktif dan kita adalah MasterClient...
        if (!coinScript.gameObject.activeSelf && PhotonNetwork.IsMasterClient)
        {
            int playerReadyCount = 0;

            // Cek status semua player di dalam room
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.ContainsKey("IsReadyForBattle"))
                {
                    if ((bool)player.CustomProperties["IsReadyForBattle"])
                    {
                        playerReadyCount++;
                    }
                }
            }

            // Jika semua player sudah ready, munculkan koin secara online lewat MasterClient
            if (playerReadyCount >= PhotonNetwork.PlayerList.Length)
            {
                coinScript.gameObject.SetActive(true);
                coinScript.MoveCoin();
            }
        }
    }

    /// <summary>
    /// Dipanggil otomatis ketika ada player lain yang meninggalkan Room/Game
    /// </summary>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        winTxt.text = "Musuh Kabur! Kamu Menang!";
        coinScript.gameObject.SetActive(false);
    }
}