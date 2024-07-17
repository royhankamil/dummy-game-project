using UnityEngine;
using TMPro;

public class WebViewExample : MonoBehaviour
{
    [SerializeField] private string url = "https://www.google.com/";
    [SerializeField] private TMP_Text resultTxt;
    private WebViewObject webViewObject;

    void Start()
    {
        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        webViewObject.Init(
            cb: (msg) => {
                Debug.Log(string.Format("CallFromJS[{0}]", msg));
                // Handle message from the web page here
                if (msg.StartsWith("RESULT:"))
                {
                    string result = msg.Substring("RESULT:".Length);
                    Debug.Log("hasil : " + result);
                    resultTxt.text = result;
                }
            },
            err: (msg) => {
                Debug.Log(string.Format("CallOnError[{0}]", msg));
            },
            httpErr: (msg) => {
                Debug.Log(string.Format("CallOnHttpError[{0}]", msg));
            },
            started: (msg) => {
                Debug.Log(string.Format("CallOnStarted[{0}]", msg));
            },
            ld: (msg) => {
                Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
                // Akses elemen dengan ID "SIvCob" setelah halaman selesai dimuat
                webViewObject.EvaluateJS(@"
                    var element = document.getElementById('SIvCob');
                    if (element) {
                        Unity.call('RESULT:' + element.textContent);
                    } else {
                        Unity.call('RESULT:Element not found');
                    }
                ");
            },
            enableWKWebView: true);
        // Mengatur margin WebView menjadi 0 untuk menampilkan secara penuh
        webViewObject.SetMargins(0, 0, 30, 0);
        webViewObject.SetVisibility(true);

        webViewObject.LoadURL(url);
    }

    public void ChangeBG()
    {
        webViewObject.EvaluateJS("document.body.style.backgroundColor = 'red';");
    }
}
