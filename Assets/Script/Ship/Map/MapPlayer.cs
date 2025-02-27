using UnityEngine;

public class MapPlayer : MonoBehaviour
{
    public float moveSpeed = 300f;
    private RectTransform _character;
    private RectTransform _mapRect; // RectTransform del mapa

    private Vector2 _movement;
    private Vector2 _lastDirection = Vector2.right;

    private InputInterfaceSystem _inputs;

    private void Awake()
    {
        _inputs = FindAnyObjectByType<InputInterfaceSystem>();
        _character = GetComponent<RectTransform>();
        _mapRect = _character.parent.GetComponent<RectTransform>(); // Obtenemos el padre
    }

    private void Update()
    {
        _movement = _inputs.movement.normalized;

        // Aplicamos el movimiento
        Vector2 newPosition = _character.anchoredPosition + _movement * moveSpeed * Time.deltaTime;

        // Limitamos la posición dentro del mapa
        newPosition = ClampToParentBounds(newPosition);

        // Asignamos la nueva posición al jugador
        _character.anchoredPosition = newPosition;

        // Rotación del personaje
        if (_movement != Vector2.zero)
        {
            _lastDirection = _movement;
            float angle = Mathf.Atan2(_movement.y, _movement.x) * Mathf.Rad2Deg;
            _character.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }

    private Vector2 ClampToParentBounds(Vector2 position)
    {
        // Tamaño del mapa
        Vector2 parentSize = _mapRect.rect.size;
        Vector2 playerSize = _character.rect.size;

        // Calculamos los límites
        float minX = -parentSize.x / 2 + playerSize.x / 2;
        float maxX = parentSize.x / 2 - playerSize.x / 2;
        float minY = -parentSize.y / 2 + playerSize.y / 2;
        float maxY = parentSize.y / 2 - playerSize.y / 2;

        // Clampeamos la posición dentro de los límites
        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        return position;
    }
}
