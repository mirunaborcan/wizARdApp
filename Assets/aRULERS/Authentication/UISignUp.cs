using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UISignUp : MonoBehaviour
{
    string username, email, password, confirmPassword;
    [SerializeField]
    public TMP_Text warningRegisterText;

    private void OnEnable()
    {
        AuthManager.OnSignUpFailed.AddListener(OnCreateAccountFailed);
    }
    private void OnDisable()
    {
        AuthManager.OnSignUpFailed.RemoveListener(OnCreateAccountFailed);
    }
    void OnCreateAccountFailed (string error)
    {
        string[] errorString = error.Split(':');
        string screenErrorMessage = errorString[errorString.Length - 1].Trim();
        Debug.Log($"length of str is: {screenErrorMessage.Length}");
        Debug.Log($"length of str is: {screenErrorMessage}!!");
        if (string.Equals(screenErrorMessage,"Password, EncryptedRequest"))
        {
            warningRegisterText.text = "All fields are mandatory!";
        }
        else 
        {
            warningRegisterText.text = screenErrorMessage;
        }
        
    }
    public void UpdateUsername( string _username)
    {
        username = _username;
    }
    public void UpdateEmail(string _email)
    {
        email = _email;
    }
    public void UpdatePassword(string _password)
    {
        password = _password;
    }
    public void UpdateConfirmPassword(string _confirmpassword)
    {
        confirmPassword = _confirmpassword;
    }

    public void CreateAcount()
    {
            AuthManager.Instance.RegisterUserFunction(username, email, password);
        
    }
}
