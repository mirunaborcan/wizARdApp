using System.Collections;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Events;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance;
    public static UnityEvent OnSignInSuccess = new UnityEvent();
    public static UnityEvent<string> OnSignInFailed = new UnityEvent<string>();
    public static UnityEvent<string> OnSignUpFailed = new UnityEvent<string>();
    
    public TMP_Text warningRegisterText;
    public TMP_Text warningLoginText;

    string playFabUserID;

    public static UnityEvent<string, string> OnUserDataRetrived = new UnityEvent<string, string>();

    private void Awake()
    {
        Instance = this;
    }

    /* REGISTER FUNCTIONALITY */
    public void RegisterUserFunction(string username, string email, string password)
    {
        PlayFabClientAPI.RegisterPlayFabUser(
            new RegisterPlayFabUserRequest()
            {
                Email = email,
                Password = password,
                Username = username,
                DisplayName = username,
                RequireBothUsernameAndEmail = true
            },
            response =>
            {
                Debug.Log($"Succesfully created!");
                warningRegisterText.text = "Account created!";
            },
            error =>
            {
                OnSignUpFailed.Invoke(error.GenerateErrorReport());
                Debug.Log($"Error! {error.GenerateErrorReport()}");
            }
        );
    }

    /* LOGIN FUNCTIONALITY */
    public void LoginUserFunction(string username, string password)
    {
        PlayFabClientAPI.LoginWithPlayFab
            (new LoginWithPlayFabRequest()
            {
                Username = username,
                Password = password
            },
            response =>
            {
                OnSignInSuccess.Invoke();
                Debug.Log($"Login succes!");
                //get current user ID
                playFabUserID = response.PlayFabId;
            },
            error =>
            {
                OnSignInFailed.Invoke(error.GenerateErrorReport());
                Debug.Log($"Error! {error.GenerateErrorReport()}");
            }
         );
    }


    /* GET USER DATA */
    public void GetUserData (string key)
    {
        PlayFabClientAPI.GetUserData
            (new GetUserDataRequest()
            {
                PlayFabId = playFabUserID,
                Keys = new List<string> ()
                {
                    key
                }
            },
            response =>
            {
                Debug.Log($"Successful getting data!");
                if (response.Data.ContainsKey(key))
                {
                    OnUserDataRetrived.Invoke(key, response.Data[key].Value);
                }
                else
                {
                    OnUserDataRetrived.Invoke(key, null);
                }
            },
            error =>
            {
                Debug.Log($"Error getting data! {error.ErrorMessage}");
            }
        ); 
    }

    public void SetUserData (string key, string value)
    {
        PlayFabClientAPI.UpdateUserData
             (new UpdateUserDataRequest()
             {
                Data = new Dictionary<string, string>()
                {
                    { key, value }
                }
             },
             response =>
             {
                 Debug.Log($"Successful setting data!");
                
             },
             error =>
             {
                 Debug.Log($"Error setting data! {error.ErrorMessage}");
             }
         ) ; 
    }
}
