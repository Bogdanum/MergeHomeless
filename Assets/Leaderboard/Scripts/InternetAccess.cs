using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

    public class InternetAccess : MonoBehaviour
    {
        [Header("Parameters")]
        [Tooltip("Uri's List")]
        [SerializeField] private string[] uris;

        public IEnumerator TestConnection(Action<bool> callback)
        {
            foreach (string uri in uris)
            {
                // Create Unity Web Request
                UnityWebRequest request = UnityWebRequest.Get(uri);
            
                // Wait Response
                yield return request.SendWebRequest();
            
                Debug.LogError("{GameLog} => [InternetAccess] - TestConnection \n URI: " + uri + "\n Network Error: " + request.isNetworkError);
            
                // Check Network Error
                if (request.isNetworkError == false)
                {
                    // Run Callback
                    callback(true);
                
                    // Break Coroutine
                    yield break;
                }
            }
        
            // Run Callback
            callback(false);
        }
    
    }





