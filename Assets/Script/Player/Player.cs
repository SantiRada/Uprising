using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour {

    [Header("Movement")]
    public float speed = 5f;
    public float speedRun = 8f;
    [HideInInspector] public bool canMove = true;
    private Vector3 _initialPosition;

    [Header("Camera Movement")]
    public float sensitivity = 2f;
    public Transform cameraTransform;

    [Header("Jetpack")]
    [Tooltip("Fuerza del Jetpack")] public float flyForce = 5f;
    [Tooltip("Mínimo Fuel necesario para comenzar el vuelo")] public float minFuelToFly;
    public float totalFuel;
    [Range(0f, 1f)] public float delayToFuel;
    [Tooltip("Multiplicador de gasto de combustible, a mayor número más rápido se gasta")] public float multiplierSpent;
    [Space]
    public TextMeshProUGUI textFuel;
    public Image imgFuel;
    public GameObject positionMinFuel;
    private float _baseFuel;
    private bool _isFly = false;

    [Header("Breathing System")]
    public TextMeshProUGUI textBreathing;
    public Image imageBreathing;
    public Image alphaImg;
    public float breathing = 30f;
    public float delayToBreathing = 0.5f;
    [Tooltip("Valor negativo hasta el que se llega antes de morir definitivamente")] public float limitToDead;
    private float _breathingBase;
    private bool isBreathing = true; // True si está en una atmósfera

    private float rotationX = 0f;
    private Rigidbody rb;
    private InputPlayerSystem _inputs;

    private void Awake()
    {
        _inputs = GetComponent<InputPlayerSystem>();

        _breathingBase = breathing;
        _baseFuel = totalFuel;
    }
    private void Start()
    {
        _initialPosition = transform.position;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();

        positionMinFuel.transform.position = new Vector3(minFuelToFly * imgFuel.GetComponent<RectTransform>().rect.width / _baseFuel, positionMinFuel.transform.position.y, positionMinFuel.transform.position.z);
    }
    private void Update()
    {
        if (!canMove) return;
        if (MenuController.state == StatePlayer.Pause) return;

        Move();

        if(_inputs.moveCamera != Vector3.zero) LookAround("stick");
        else LookAround("mouse");
        Fly();

        #region BreathingUI
        if(breathing >= 0)
        {
            textBreathing.text = breathing.ToString("f0") + " o2";
            imageBreathing.fillAmount = breathing / _breathingBase;
        }
        if (!isBreathing)
        {
            breathing -= Time.deltaTime;

            if (breathing <= limitToDead) FaintPlayer();
        }

        float maxBreath = _breathingBase / 4;
        float minBreath = 7;
        float currentAlpha = 0;

        if (breathing < maxBreath)
        {
            float range = Mathf.Max(maxBreath - minBreath, 0.01f);
            currentAlpha = Mathf.Clamp01((maxBreath - breathing) / range) * 100;
        }
        else { currentAlpha = 0; }

        // Aplicar alpha al color
        alphaImg.color = new Color(0, 1, 1, currentAlpha / 100f);

        #endregion

        #region FlyUI
        textFuel.text = totalFuel.ToString("f0") + "/" + _baseFuel.ToString("f0");
        imgFuel.fillAmount = totalFuel / _baseFuel;
        #endregion
    }
    private void Move()
    {
        if(_inputs.moveInput == Vector3.zero)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        float moveX = _inputs.moveInput.x;
        float moveZ = _inputs.moveInput.z;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        Vector3 newVelocity;

        if(_inputs.useRun == 1)
            newVelocity = new Vector3(move.x * speedRun, rb.velocity.y, move.z * speedRun);
        else
            newVelocity = new Vector3(move.x * speed, rb.velocity.y, move.z * speed);

        rb.velocity = newVelocity;
    }
    private void LookAround(string control)
    {
        if (control == "stick")
        {
            Vector3 moveCam = _inputs.moveCamera;

            float moveX = moveCam.x * sensitivity;
            float moveY = moveCam.y * sensitivity;

            rotationX -= moveY;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);

            cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.Rotate(Vector3.up * moveX);
        }
        else
        {
            Vector3 moveCam = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);

            float moveX = moveCam.x * sensitivity;
            float moveY = moveCam.y * sensitivity;

            rotationX -= moveY;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);

            cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.Rotate(Vector3.up * moveX);
        }
    }
    private void Fly()
    {
        if (_inputs.useJetpack == 1 && totalFuel > 0)
        {
            if (totalFuel > minFuelToFly)
            {
                StopCoroutine("CompleteFuel");
                totalFuel -= (Time.deltaTime * multiplierSpent);
                rb.velocity = new Vector3(rb.velocity.x, flyForce, rb.velocity.z);
                _isFly = true;
            }
            else
            {
                if (_isFly)
                {
                    StopCoroutine("CompleteFuel");
                    totalFuel -= (Time.deltaTime * multiplierSpent);
                    rb.velocity = new Vector3(rb.velocity.x, flyForce, rb.velocity.z);
                }
            }
        }

        if (_inputs.useJetpack == 2 || totalFuel <= 0)
        {
            _isFly = false;
            StartCoroutine("CompleteFuel");
            rb.velocity = Vector3.zero;
            _inputs.useJetpack = 0;
        }
    }
    private IEnumerator CompleteFuel()
    {
        for (int i = 0; i < _baseFuel; i++)
        {
            if (_baseFuel > totalFuel)
            {
                totalFuel++;
                yield return new WaitForSeconds(delayToFuel);
            }
            else break;
        }

        totalFuel= _baseFuel;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Atmosphere"))
        {
            isBreathing = true;
            StartCoroutine("CompleteBreathing");
        }
    }
    private IEnumerator CompleteBreathing()
    {
        for(int i = 0; i < _breathingBase; i++)
        {
            if (_breathingBase > breathing)
            {
                breathing++;
                yield return new WaitForSeconds(delayToBreathing);
            }
            else break;
        }

        breathing = _breathingBase;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Atmosphere"))
        {
            isBreathing = false;
            StopCoroutine("CompleteBreathing");
        }
    }
    public void FaintPlayer()
    {
        canMove = false;
        Debug.Log("El jugador se ha desmayado.");

        StartCoroutine("ResetGame");
    }
    private IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(.25f);
        isBreathing = true;
        _isFly = false;
        StartCoroutine("CompleteBreathing");
        StartCoroutine("CompleteFuel");
        yield return new WaitForSeconds(1f);
        transform.position = _initialPosition;
        canMove = true;
    }
}