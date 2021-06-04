using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player Instance;
    [SerializeField] private int health = 150;
    [SerializeField] private GameObject playerCam;
    [SerializeField] private AudioSource hurtSound;
    [SerializeField] private AudioSource hitSound;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }
    void Attack()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward * 5, out hit))
        {
            if (hit.collider.CompareTag("Creeper"))
            {
                hitSound.Play();
                hit.collider.gameObject.GetComponent<CreeperAi>().AddDamage(10);
            }
        }
    }
    public void AddDamage(int amount)
    {
        health -= amount;
        hurtSound.Play();
        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    private void OnDestroy()
    {
        SceneManager.LoadScene("GameOverScene");
        Cursor.lockState = CursorLockMode.None;
    }
}
