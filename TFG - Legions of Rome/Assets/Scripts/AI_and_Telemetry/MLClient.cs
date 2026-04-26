using UnityEngine;
using System;
using System.Collections;
using System.Text;
using UnityEngine.Networking;

public class MLClient : MonoBehaviour
{
    public static MLClient Instance { get; private set; }

    public string apiUrl = "http://127.0.0.1:8000/predict";

    public static event Action<string> OnProfileReceived;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public class PlayerMetricsData
    {
        public float APM;
        public float Precision;
        public float AvgDistance;
        public float DamageTakenPerMin;
        public float DamageDealtPerMin;
        public float RangedRatio;
        public float DashPerMin;
        public float ShieldPerMin;
        public float TimeInRiskZone;
        public string RewardChosen;
    }

    private class PredictionResponse
    {
        public string predicted_profile;
    }

    public void RequestProfilePrediction(PlayerMetricsData metrics)
    {
        StartCoroutine(PostMetricsCoroutine(metrics));
    }

    private IEnumerator PostMetricsCoroutine(PlayerMetricsData metrics)
    {
        string jsonBody = JsonUtility.ToJson(metrics);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("[ML Client] Enviando telemetría a Python...");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"[ML Client] Error de conexión API: {request.error}");
            }
            else
            {
                string responseText = request.downloadHandler.text;
                PredictionResponse response = JsonUtility.FromJson<PredictionResponse>(responseText);

                Debug.Log($"[ML Client] 🎯 Predicción del SVM recibida: Perfil [{response.predicted_profile}]");

                OnProfileReceived?.Invoke(response.predicted_profile);
            }
        }
    }
}
