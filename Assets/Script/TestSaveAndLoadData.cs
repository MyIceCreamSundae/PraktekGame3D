using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class TestSaveAndLoadData : MonoBehaviour
{
    public DataPlayer dataPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

   // Update is called once per frame
    void Update() {
         if (Input.GetKeyDown(KeyCode.S)) {
            SaveDataPlayer();
        } else if (Input.GetKeyDown(KeyCode.L)) {
            LoadDataPlayer();
        }
    }

    #region Contoh Save Data Backend
    
    public void SaveDataPlayer() {
        if (!PlayFabClientAPI.IsClientLoggedIn()) {
            Debug.LogError("Harap Login terlebih dahulu!");
            return;
        } else {
            print("Save data...");
        }

        var dataYgAkanDiSave = new Dictionary<string, string>();
        dataYgAkanDiSave.Add("data", dataPlayer.ToJson());
        // Key: "data" => nama key yg akan di set value-nya
        // Value: JsonUtility.ToJson(dataPlayer) => class data player akan dirubah menjadi format JSON, agar bisa di save, dan saat di load, Json tadi s

        UpdateUserDataRequest request = new UpdateUserDataRequest {
            Data = dataYgAkanDiSave,
            Permission = UserDataPermission.Public //Jika di set public player lain bisa mendapatkan Data Player ini yg key-nya di set Public
        };
        print("Seperti inilah sebuah class jika dijadikan JSON: \n" + JsonUtility.ToJson(dataPlayer));

        PlayFabClientAPI.UpdateUserData(request, OnSaveDataPlayerSuccess, OnSaveDataPlayerFailed);
    }

    public void OnSaveDataPlayerSuccess(UpdateUserDataResult result) {
        Debug.Log("<color=green> Save data berhasil!! </color>");
    }

    public void OnSaveDataPlayerFailed(PlayFabError error) {
        Debug.LogError("Save data gagal! \n" + error.GenerateErrorReport());
    }

    #endregion

    #region Contoh Load Data Backend
    
    public void LoadDataPlayer() {
        if (!PlayFabClientAPI.IsClientLoggedIn()) {
            Debug.LogError("Harap Login terlebih dahulu!");
            return;
        } else {
            print("Load data...");
        }

        List<string> listKeyYgDiambilDatanya = new List<string>();
        listKeyYgDiambilDatanya.Add("data"); //kalau mau ambil data lain sekaligus, copy saja line ini, lalu paste dibawahnya tinggal ganti namanya aja

        GetUserDataRequest request = new GetUserDataRequest {
            Keys = listKeyYgDiambilDatanya
        };
        PlayFabClientAPI.GetUserData(request, OnLoadDataPlayerSuccess, OnLoadDataPlayerFailed);
    }

    public void OnLoadDataPlayerSuccess(GetUserDataResult result) {
        Debug.Log("<color=green> Berhasil Mendapatkan Save Data!! </color>");
        UserDataRecord rawLoadedData;
        if (result.Data.TryGetValue("data", out rawLoadedData)) { // false jika key dengan nama "data" tidak ditemukan. Jika hasilnya False
            // out rawLoadedData => out itu sama seperti return. Dapat digambarkan menjadi rawLoadedData = (data yg di return)
            dataPlayer = JsonUtility.FromJson<DataPlayer>(rawLoadedData.Value); //Mengubah dataPlayer sesuai data yg di Load
            Debug.Log("<color=green> Load data berhasil!! </color>");
        } else {
            Debug.LogError("Save data dengan key \"data\" tidak ditemukan!");
        }
    }

    public void OnLoadDataPlayerFailed(PlayFabError error) {
        Debug.LogError("Load data gagal! \n" + error.GenerateErrorReport());
    }

    #endregion
}

[System.Serializable] //Agar terlihat di inspector
public class DataPlayer {
    public int level, exp, coins;
    public string power;

    public string ToJson(){
        return JsonUtility.ToJson(this);
    }
}