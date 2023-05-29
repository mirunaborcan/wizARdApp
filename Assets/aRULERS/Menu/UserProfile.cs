using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserProfile : MonoBehaviour
{
    [SerializeField] ProfileData profileData;
    private void OnEnable()
    {
        AuthManager.OnUserDataRetrived.AddListener(UserDataRetrieved);
    }

    private void UserDataRetrieved(string key, string value)
    {
        if(key == "ProfileData")
        {
            profileData = JsonUtility.FromJson<ProfileData>(value);
        }
    }

    private void OnDisable()
    {
        AuthManager.OnUserDataRetrived.RemoveListener(UserDataRetrieved);

    }

    [ContextMenu ("Get Profile Data")]
    void GetUserData()
    {
        AuthManager.Instance.GetUserData("ProfileData");
    }
    
    void SetUserData()
    {
        AuthManager.Instance.SetUserData("ProfileData", JsonUtility.ToJson(profileData));
    }
}

[System.Serializable]
public class ProfileData
{
    public string userName;
}