using UnityEngine;
using TMPro;

public class WebViewExample : MonoBehaviour
{
    [SerializeField] private string url = "https://www.google.com/";
    [SerializeField] private string StartTeStLink = "https://terrastation.page.link";
    [SerializeField] private string StartKeplrLink = "wc:";
    [SerializeField] private TMP_Text resultTxt;
    private WebViewObject webViewObject;

    void Start()
    {
        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        webViewObject.Init(
            cb: (msg) => {
                Debug.Log($"CallFromJS[{msg}]");
                HandleWebViewMessage(msg);
            },
            err: (msg) => {
                Debug.LogError($"WebView Error: {msg}");
                resultTxt.text = $"WebView Error: {msg}";
            },
            httpErr: (msg) => {
                Debug.LogError($"HTTP Error: {msg}");
                resultTxt.text = $"HTTP Error: {msg}";
            },
            started: (msg) => {
                Debug.Log($"WebView Started: {msg}");
                resultTxt.text = $"WebView Started: {msg}";
                LogAndOpenURL(msg);
            },
            enableWKWebView: true,
            transparent: false // Adjust transparency if needed
        );


        // Set WebView margins and visibility
        webViewObject.SetMargins(0, 0, 0, 500); // Adjust margins as per your layout
        webViewObject.SetVisibility(true);

        // Load initial URL
        webViewObject.LoadURL(url.Replace(" ", "%20"));

        // Add a custom JavaScript interface to handle link clicks
        webViewObject.EvaluateJS($@"
            function handleExternalLink(url) {{
                Unity.call('Attempting to navigate to: ' + url);
                if (url.startsWith('mailto:') || url.startsWith('tel:') || url.startsWith('{StartTeStLink}') || url.startsWith('{StartKeplrLink}')) {{
                    window.location.href = url;
                }} else {{
                    window.location.href = url;
                }}
            }}

            document.addEventListener('click', function(e) {{
                if (e.target.tagName === 'A') {{
                    e.preventDefault();
                    var url = e.target.href;
                    handleExternalLink(url);
                }}
            }});
        ");
    }

    void HandleWebViewMessage(string msg)
    {
        if (msg.StartsWith("RESULT:"))
        {
            string result = msg.Substring("RESULT:".Length);
            Debug.Log($"Result received: {result}");
            resultTxt.text = result;
        }
        else if (msg.StartsWith("Attempting to navigate to:"))
        {
            string link = msg.Substring("Attempting to navigate to:".Length);
            Debug.Log($"Navigation attempt to: {link}");
        }
    }

    void LogAndOpenURL(string url)
    {
        Debug.Log($"Opening URL: {url}");
        if (url.StartsWith(StartTeStLink) || url.StartsWith(StartKeplrLink))
        {
            webViewObject.LoadURL(url.Replace(" ", "%20"));
            Application.OpenURL(url.Replace(" ", "%20"));
        }
    }

    public void ChangeBG()
    {
        webViewObject.EvaluateJS("document.body.style.backgroundColor = 'red';");
    }

    public void SearchElementByClass(string className)
    {
        string jsCode = $@"
            var elements = document.getElementsByClassName('{className}');
            if (elements.length > 0) {{
                Unity.call('RESULT:' + elements[0].textContent);
            }} else {{
                Unity.call('RESULT:Element not found');
            }}
        ";
        webViewObject.EvaluateJS(jsCode);
    }

    public void TestLink()
    {
        string testUrl = "wc:61734b640dcd6aaa53f3a63b67f890da813f6262501dcd0b9384db87406c27f4@2?relay-protocol=irn&symKey=aa7f20d2c2b2c7053d822e9815d331f87fff8936a87348b846de35666bd/8c410";
        Debug.Log($"Testing link: {testUrl}");
        Application.OpenURL(testUrl);
    }
}
