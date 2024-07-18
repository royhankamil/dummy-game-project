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
        webViewObject.LoadURL(url);

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
            Application.OpenURL(url);
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
        string testUrl = "https://terrastation.page.link/?link=https://terra.money?action%3Dwallet_connect%26payload%3Dwc%253A672fc31d-f2cc-4b5e-8ea9-d463d293efb7%25401%253Fbridge%253Dhttps%25253A%25252F%25252Fwalletconnect.terra.dev%2526key%253D2b58e6611398736cc97bb74300f1dd4e7406035c161372f8a85a233d4e18da7a&apn=money.terra.station&ibi=money.terra.station&isi=1548434735";
        Debug.Log($"Testing link: {testUrl}");
        Application.OpenURL(testUrl);
    }
}
