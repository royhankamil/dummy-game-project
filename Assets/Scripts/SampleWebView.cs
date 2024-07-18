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
        webViewObject.EvaluateJS(@"
            function handleExternalLink(url) {
                // Check if URL is an external link (mailto:, tel:, etc.)
                if (url.startsWith('mailto:') || url.startsWith('tel:') || url.startsWith('intent:')) {
                    // Open URL using Unity's Application.OpenURL to handle external apps
                    Unity.call('handleExternalLink:' + url);
                } else {
                    // Handle other URLs as needed (e.g., open in the same WebView)
                    // Example: window.location.href = url;
                }
            }

            // Intercept clicks on <a> tags to prevent default behavior
            document.addEventListener('click', function(e) {
                if (e.target.tagName === 'A') {
                    e.preventDefault();
                    var url = e.target.href;
                    handleExternalLink(url);
                }
            });
        ");
    }

    void HandleWebViewMessage(string msg)
    {
        if (msg.StartsWith("handleExternalLink:"))
        {
            string url = msg.Substring("handleExternalLink:".Length);
            Debug.Log($"Handling external link: {url}");
            Application.OpenURL(url); // Open external links using Unity's API
        }
        else if (msg.StartsWith("RESULT:"))
        {
            string result = msg.Substring("RESULT:".Length);
            Debug.Log($"Result received: {result}");
            resultTxt.text = result;
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
}
