using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class UIuserProfile : MonoBehaviour
{
    [SerializeField]
    public TMP_Text username;
    
    private string email; 

    private string _playFabUsername;
    private string _playFabEmail;

    // Call this function to get the username of the logged-in PlayFab user
    public void GetPlayFabUsername()
    {
        var request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request, OnGetAccountInfoSuccess, OnGetAccountInfoFailure);
        
    }

    // Callback for a successful GetAccountInfo request
    private void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        _playFabUsername = result.AccountInfo.Username;
        _playFabEmail = result.AccountInfo.PrivateInfo.Email;
        username.text = _playFabUsername;
        email = _playFabEmail;
        //Debug.Log("PlayFab Username: " + _playFabUsername);
    }

    // Callback for a failed GetAccountInfo request
    private void OnGetAccountInfoFailure(PlayFabError error)
    {
        Debug.LogError("Failed to get PlayFab account info: " + error.GenerateErrorReport());
    }

    public void ResetPassword()
    {
        AuthManager.Instance.ResetPasswordFunction(email);
    }


}

