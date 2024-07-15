using System.Collections;
using System.Collections.Generic;
using Nethereum.Model;
using UnityEngine;
using WalletConnect.Web3Modal;

public class TestWallet : MonoBehaviour
{
    public async void Start()
    {
        await Web3Modal.InitializeAsync();
    }

    public void OpenModal()
    {
        Web3Modal.OpenModal();
    }

    public void InfoWallet()
    {
        Debug.Log(Web3Modal.AccountController.Address);
    }
}
