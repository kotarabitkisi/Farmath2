using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager instance;
    public TMP_Dropdown LanguageDropdown;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        ChangeLanguage();
    }
    public void ChangeLanguage()
    {
        StartCoroutine(SetLanguage(LanguageDropdown.value));
    }

    private IEnumerator SetLanguage(int index)
    {
        yield return LocalizationSettings.InitializationOperation;

        var selectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        LocalizationSettings.SelectedLocale = selectedLocale;
        LocalizeStringEvent[] Components = Resources.FindObjectsOfTypeAll<LocalizeStringEvent>();
        for (int i = 0; i < Components.Length; i++)
        {
            Components[i].RefreshString();
        }
        InitializeAllTexts();
        Debug.Log("Dil deðiþti: " + selectedLocale.Identifier.Code);
    }
    public string TurnToString(LocalizedString localized, object[] arguments)
    {
        if (localized == null || localized.IsEmpty)
        {
            Debug.LogWarning("LocalizedString is null or empty.");
            return "";
        }

        try
        {
            return localized.GetLocalizedString(arguments);
        }
        catch (Exception e)
        {
            Debug.LogError($"Localization error: {e.Message}");
            return "";
        }
    }
    public void InitializeAllTexts()
    {
        GameManager GM = GameManager.instance;
        for (int i = 0; i < GM.ShopManagement.cardsOnShop.Count; i++)
        {
            ShopManager ShopManagement_ = GM.ShopManagement;
            ShopManagement_.allShopNameTxt[i].text = TurnToString(ShopManagement_.cardsOnShop[i].CardName, null);
        }
        for (int i = 0; i < GM.explorations.Length; i++)
        {
            if (GM.explorations[i].isExplored)
                GM.OpenExplore(i, false);
        }
        for (int i = 0; i < GM.deckPlacing.openedCards.Count; i++)
        {
            GM.deckPlacing.openedCards[i].GetComponent<CardData>().InitializeImages();
        }
    }
}
