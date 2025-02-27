using UnityEngine;

public class LightDepthController : MonoBehaviour {

    public Light directionalLight;
    private Transform player;
    [Tooltip("Profundidad m�nima (oscuro total)")] public float minDepth = -50f;
    [Tooltip("Profundidad m�xima (iluminaci�n normal)")] public float maxDepth = 5f;
    public float minIntensity = 0f;
    public float maxIntensity = 1f;
    [Tooltip("Velocidad de suavizado de la transici�n")] public float smoothSpeed = 2f;

    private float targetIntensity;

    private void Start()
    {
        player = FindAnyObjectByType<Player>().transform;

        // Usa la luz principal si no est� asignada
        if (directionalLight == null) directionalLight = RenderSettings.sun;
    }
    private void Update()
    {
        float depthFactor = Mathf.Clamp01((player.position.y - minDepth) / (maxDepth - minDepth));
        targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, depthFactor);

        directionalLight.intensity = Mathf.Lerp(directionalLight.intensity, targetIntensity, Time.deltaTime * smoothSpeed);

        directionalLight.shadowStrength = Mathf.Lerp(1f, 0.5f, depthFactor);
    }
}