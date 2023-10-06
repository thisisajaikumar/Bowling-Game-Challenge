using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    public static Ball Instance { get; private set; }
    public int Point { get; set; }

    [SerializeField] Rigidbody rb;
    [SerializeField, Range(5f, 100f)] float startSpeed = 40f;
    [SerializeField] float minX = -5f, maxX = 5f;
    [SerializeField] Text resuleText;
    [SerializeField] ParticleSystem particleEffect;
    [SerializeField,Range(0,5)] float slowMotionScale = 0.2f;
    [SerializeField] AudioSource AudioSource = null;
    [SerializeField] AudioClip launchclip;

    private Transform _arrow;
    private bool _ballMoving, _doneVfx;
    private List<GameObject> _pins = new();
    private readonly Dictionary<GameObject, Transform> _pinsDefaultTransform = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _doneVfx = false;
        resuleText.text = string.Empty;
        _arrow = GameObject.FindGameObjectWithTag("Arrow").transform;
        rb = GetComponent<Rigidbody>();

        _pins = GameObject.FindGameObjectsWithTag("Pin").ToList();

        foreach (var pin in _pins)
        {
            _pinsDefaultTransform.Add(pin, pin.transform);
        }
    }

    void Update()
    {
        if (_ballMoving)
        {
            return;
        }

        ChangePosition();
    }

    void ChangePosition()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector3 touchPosition;

            if (Input.touchCount > 0)
            {
                touchPosition = Input.GetTouch(0).position;
            }
            else
            {
                touchPosition = Input.mousePosition;
            }

            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    float newX = Mathf.Clamp(hit.point.x, minX, maxX);
                    Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z);
                    transform.position = newPosition;
                    _arrow.position = newPosition;
                }
            }
        }
    }


    public void Launch()
    {
        AudioSource.PlayOneShot(launchclip);
        StartCoroutine(Shoot());
    }


    private IEnumerator Shoot()
    {
        _ballMoving = true;
        _arrow.gameObject.SetActive(false);
        rb.isKinematic = false;
       

        Vector3 forceVector = _arrow.forward * (startSpeed * _arrow.transform.localScale.z);
        Vector3 forcePosition = transform.position + (transform.right * 0.5f);
        rb.AddForceAtPosition(forceVector, forcePosition, ForceMode.Impulse);

        yield return new WaitForSecondsRealtime(7);
        _ballMoving = false;
        GenerateFeedBack();

        yield return new WaitForSecondsRealtime(3);
        SceneManager.LoadSceneAsync("Game");
    }

    public void VFX()
    {
        if (!_doneVfx)
        {
            StartCoroutine(PlayVFX());
        }
    }

    IEnumerator PlayVFX()
    {
        if (!particleEffect.isPlaying)
        {
            _doneVfx = true;
            float originalTimeScale = Time.timeScale;
            Time.timeScale = slowMotionScale;
            rb.isKinematic = true;
            particleEffect.Play();
            yield return new WaitForSeconds(0.5f);
            AudioSource.Play();
            rb.isKinematic = false;
            rb.AddForce(Vector3.forward, ForceMode.Impulse);
            Time.timeScale = originalTimeScale;
        }
    }


    private void GenerateFeedBack()
    {
        string feedback = Point switch
        {
            0 => "Nothing!",
            > 0 and < 3 => "You are learning Now!",
            >= 3 and < 6 => "It was close!",
            >= 6 and < 10 => "It was nice!",
            _ => "Perfect! You are a master!"
        };

        resuleText.text = feedback;
    }
}