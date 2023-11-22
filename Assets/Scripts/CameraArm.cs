using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class CameraArm : MonoBehaviour
{
    public float horizontal;
    private Camera armedCamera;
    [SerializeField] private float spinSpeed;
    public Player player;
    public float cameraRotationSpeed = 1f;
    public float distanceFromTerrain;
    public float cameraRotation = 26f;
    public float timeSpentPanning;
    public Transform distanceFromTerrain2;
    public float cameraDistance;
    private float proxy = 0f;
    private float broxy = 0f;
    public float playerSpeed;
    private static CameraArm instance;
    public static CameraArm Instance { get { return instance; } }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        armedCamera = GetComponentInChildren<Camera>();
        player = FindAnyObjectByType<Player>();
        spinSpeed = player.cameraSpeed;
    }


    void LateUpdate()
    {
        playerSpeed = player.fighterRigidbody.velocity.magnitude / player.moveSpeed;
        if (proxy < playerSpeed)
        {
            proxy += Mathf.Clamp01(0.1f * Time.deltaTime);
        } else if (proxy > playerSpeed)
        {
            proxy -= Mathf.Clamp01(0.1f * Time.deltaTime);
        }
        

        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            transform.eulerAngles = new (0, horizontal += Input.GetAxis("Horizontal") * spinSpeed * Time.deltaTime, 0);
            if (horizontal < 0f)
            {
                horizontal += 360f;
            }
            if (horizontal > 360f)
            {
                horizontal -= 360f;
            }
        }

        // camera rotation block, block as in chunk or area
        transform.position = player.transform.position;
        if (Physics.Raycast(distanceFromTerrain2.position, Vector3.down, out RaycastHit cameraHeight, 18f, 1 << 3))
        {

            if (cameraHeight.distance > 2.5f && cameraRotation > 6f)
            {
                if (cameraHeight.distance > 3f)
                {
                    timeSpentPanning += Time.deltaTime;
                }
                else
                {
                    timeSpentPanning = 0f;
                }
                if (cameraRotation == 26f)
                {
                    cameraRotationSpeed = 0.1f;
                }
                else if (cameraRotation < 26f)
                {
                    cameraRotationSpeed = Mathf.Abs((1f + timeSpentPanning) / (cameraRotation - 26f));
                }
                else if (cameraRotation > 30f)
                {
                    cameraRotationSpeed = 3f;
                } 
                else 
                { 
                    cameraRotationSpeed = 1f; 
                }
                
                armedCamera.transform.Rotate(cameraRotationSpeed * Time.deltaTime * Vector3.left);
                cameraRotation += cameraRotationSpeed * Time.deltaTime * -1f;
            }
            else if (cameraHeight.distance < 2.5f && cameraRotation < 40f)
            {
                if (cameraRotation == 26f)
                {
                    cameraRotationSpeed = 0.1f;
                }
                else if (cameraRotation > 26f)
                {
                    cameraRotationSpeed = Mathf.Abs(1 / (cameraRotation - 26f));
                }
                else if (cameraRotation < 20f)
                {
                    cameraRotationSpeed = 2f;
                }
                else
                {
                    cameraRotationSpeed = 1f;
                }
                if (cameraRotationSpeed > 6f || cameraRotationSpeed < -6f)
                {
                    cameraRotationSpeed = 0f;
                }
                armedCamera.transform.Rotate(cameraRotationSpeed * Time.deltaTime * Vector3.right);
                cameraRotation += cameraRotationSpeed * Time.deltaTime * 1f;
            }
            distanceFromTerrain = cameraHeight.distance;
        }
        // this is down here in case I change the camera distance in the rotation area or the speed area
        if (broxy < Mathf.Abs(distanceFromTerrain - 2.5f))
        {
            broxy += Time.deltaTime / 10f;
        } 
        else if (broxy > Mathf.Abs(distanceFromTerrain - 2.5f))
        {
            broxy -= Time.deltaTime / 10f;
        }

        cameraDistance = 2f + proxy + broxy;

        armedCamera.transform.localPosition = new(0f, 2f, -cameraDistance); // might still need to smooth this out
    }
}
