using NaughtyAttributes;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class UnityWebRequestExample : MonoBehaviour
{
    
    [SerializeField] private string sessionToken;
    [SerializeField] private TMP_Text textbox1;
    private string text1;
    [SerializeField] private TMP_Text textbox2;
    private string text2;
    private const string RETRIEVE_USER_ENDPOINT = "https://parseapi.back4app.com/users/me";
    private const string APP_ID = "7LGeZ1TOgif1T2XF39d7m8ACRyfOoykTywMVsa1q";
    private const string REST_API_KEY = "NUgoGuGrTb8u2oWPvnaeIRR77BWh7DATnw5IjUey";

    private void Awake()
    {
        var args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {

            if (args[i].Equals("--sessionToken", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                sessionToken = args[i + 1].Trim('"');
                break;
            }
        }

        if (!string.IsNullOrEmpty(sessionToken))
            Debug.Log("Session token provided on command line.");
        else
            Debug.Log("No session token found on command line.");
    }
    private void Start()
    {
        if (!string.IsNullOrEmpty(sessionToken))
        {
            GetPlayerData();
        }
    }

    [Button]
    private async void GetPlayerData()
    {
        try
        {
            var playerData = await GetData();
            var displayName = ExtractDisplayName(playerData);
            textbox1.text = sessionToken;
            textbox2.text = displayName;
            Debug.Log(playerData);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async Task <string> GetData()
    {
        using (var uwr = UnityWebRequest.Get(RETRIEVE_USER_ENDPOINT))
        {
            uwr.SetRequestHeader("X-Parse-Application-Id",APP_ID);
            uwr.SetRequestHeader("X-Parse-REST-API-Key",REST_API_KEY);
            uwr.SetRequestHeader("X-Parse-Session-Token",sessionToken);
            uwr.downloadHandler = new DownloadHandlerBuffer();

            await uwr.SendWebRequest();

            if(uwr.result == UnityWebRequest.Result.Success)
            {
                return uwr.downloadHandler.text;
            }
            else
            {
                throw new Exception("Request failed: " + uwr.error);
            }
        }
    }

    private string ExtractDisplayName(string json)
    {
        if (string.IsNullOrEmpty(json))
            return "player";

        var m = Regex.Match(json, @"""DisplayName""\s*:\s*""([^""]*)""");
        if (m.Success && m.Groups.Count > 1)
        {
            var raw = m.Groups[1].Value;
            return Regex.Unescape(raw);
        }

        Debug.LogWarning("displayName not found in JSON, using fallback.");
        return "player";
    }
}