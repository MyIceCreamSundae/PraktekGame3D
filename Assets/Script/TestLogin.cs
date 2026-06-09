using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class TestLogin : MonoBehaviour {

    // Start is called before the first frame update
    void Start() {
        
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.P)) { //Apakah player menekan tombol P?
            print("mencoba login...");
            LoginWithPlayfab();
        }
       
    }

    /// <summary>
    /// testing Login dengan playfab
    /// </summary>
    public void LoginWithPlayfab() {
        LoginWithCustomIDRequest playerInfo = new LoginWithCustomIDRequest {
            CustomId = "Pro Player", //Custom ID harus unik (tidak boleh sama dengan player lain) cocok juga sebagai username :)
            CreateAccount = true, //Otomatis register jika akun ini blm register?
        };
        PlayFabClientAPI.LoginWithCustomID(playerInfo, OnLoginSuccess, OnLoginFailed);
    }

    void OnLoginSuccess(LoginResult result) {
        // \n berfungsi sebagai paragraf baru (ibarat menekan tombol Enter pas lagi ngetik)
        // <color=green> beri warna sampai sini </color> berfungsi untuk memberikan warna pada text Console/Log
        string successTitle = "<color=green> Login success! </color>\n";
        string deskripsi = "Berikut info yg bisa anda dapat dari result: \n" +
            "Playfab ID: " + result.PlayFabId + "\n" +
            "Baru aja register: " + result.NewlyCreated + "\n" +
            "Last login: " + result.LastLoginTime + "\n" +
            "Dan masih banyak lagi yg lainnya. Temukan dengan cara ketik result. di void ini";

        Debug.Log(successTitle + deskripsi);
    }

    void OnLoginFailed(PlayFabError error) {
        Debug.LogError("Oops... ada error! \n" + error.GenerateErrorReport());
    }
}