//
// LocalizedGUITexture.cs
//
//
// Written by Niklas Borglund and Jakob Hillerström
//

using UnityEngine.UI;

namespace SmartLocalization
{
using UnityEngine;
using System.Collections;

public class LocalizedGUITexture : MonoBehaviour 
{
	public string localizedKey = "INSERT_KEY_HERE";
	
	void Start () 
	{
		//Subscribe to the change language event
		LanguageManager languageManager = LanguageManager.Instance;
		languageManager.OnChangeLanguage += OnChangeLanguage;
		
		//Run the method one first time
		OnChangeLanguage(languageManager);
	}

	void OnDestroy()
	{
		if(LanguageManager.HasInstance)
		{
			LanguageManager.Instance.OnChangeLanguage -= OnChangeLanguage;
		}
	}
	
	void OnChangeLanguage(LanguageManager languageManager)
	{
		//Initialize all your language specific variables here
		GetComponent<RawImage>().texture = LanguageManager.Instance.GetTexture(localizedKey);
	}
}
}//namespace SmartLocalization