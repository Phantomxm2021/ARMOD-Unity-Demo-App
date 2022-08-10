using System;
using System.Collections;
using System.Threading.Tasks;
using com.phantoms.models.Runtime;
using SDKEntry.Runtime;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UnityARMODApp.Runtime
{
    public static class Utility
    {
        /// <summary>
        /// Generate a sprite from web url, and set it to image
        /// </summary>
        /// <param name="_imageUri">image url</param>
        /// <param name="_image">UI image component</param>
        /// <returns>IEnumerator</returns>
        public static IEnumerator TryAcquireSpriteFromUri(Uri _imageUri, Image _image)
        {
            var tmp_WebRequest = UnityWebRequestTexture.GetTexture(_imageUri);
            yield return tmp_WebRequest.SendWebRequest();
            switch (tmp_WebRequest.result)
            {
                case UnityWebRequest.Result.InProgress:
                    break;

                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(tmp_WebRequest.error);
                    break;

                case UnityWebRequest.Result.Success:
                    //Success, we need convert it(texture) to sprite
                    var tmp_WebTexture = ((DownloadHandlerTexture) tmp_WebRequest.downloadHandler).texture;
                    var tmp_Sprite = Sprite.Create(tmp_WebTexture,
                        new Rect(0, 0, tmp_WebTexture.width, tmp_WebTexture.height),
                        new Vector2(0, 0));

                    //Set to image
                    _image.sprite = tmp_Sprite;
                    break;
            }
        }

        /// <summary>
        /// Start ARMOD
        /// </summary>
        /// <param name="_configuration">ARMOD SDK Configure</param>
        /// <param name="_projectId">unique id of AR Experience project</param>
        public static async void LaunchAR(SDKConfiguration _configuration, string _projectId)
        {
            //Reinitialization
            SDKInitialization.Initialize();

            //Since initialization takes a certain amount of time, the delay is 125 milliseconds.
            await Task.Delay(125);
            //Init and launch
            var tmp_SDKEntryPoint = UnityEngine.Object.FindObjectOfType<SDKEntryPoint>();
            tmp_SDKEntryPoint.InitSDK(JsonUtility.ToJson(_configuration));
            tmp_SDKEntryPoint.LaunchXRQuery(_projectId);
            
        }


        /// <summary>
        /// Stop ARMOD
        /// </summary>
        public static void DisableAR()
        {
            var tmp_SDKEntryPoint = UnityEngine.Object.FindObjectOfType<SDKEntryPoint>();
            tmp_SDKEntryPoint.Dispose();
            
            //Reload the Main scene for ready
            SceneManager.LoadScene("Main");
        }


        public static void CleanCache()
        {
            Caching.ClearCache();
        }
    }
}