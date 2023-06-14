using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UISignIn : MonoBehaviour
{
    string username, password;
    [SerializeField]
    public TMP_Text warningLoginText;

    private void OnEnable()
    {
        AuthManager.OnSignInFailed.AddListener(LoginAccountFailed);
        AuthManager.OnSignInSuccess.AddListener(LoginAccountSuccess);
    }
    private void OnDisable()
    {
        AuthManager.OnSignInFailed.RemoveListener(LoginAccountFailed);
        AuthManager.OnSignInSuccess.RemoveListener(LoginAccountSuccess);
    }
    void LoginAccountSuccess()
    {
        SceneManager.LoadScene("Menu");
    }
    void LoginAccountFailed(string error)
    {
        string[] errorString = error.Split(':');
        string screenErrorMessage = errorString[errorString.Length - 1].Trim();
        warningLoginText.text = screenErrorMessage;
    }
    public void UpdateUsername(string _username)
    {
        username = _username;
    }
    public void UpdatePassword(string _password)
    {
        password = _password;
    }
    public void Login()
    {
        AuthManager.Instance.LoginUserFunction(username, password);
    }
    public void ResetPassword()
    {
        AuthManager.Instance.ResetPasswordFunction(username);
    }
}
