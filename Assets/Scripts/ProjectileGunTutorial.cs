using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGunTutorial : MonoBehaviour
{
    //bullet
    public GameObject bullet;


    //bullet force
    public float shootForce, upwardForce;

    //Gun stats
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTab;
    public bool allowButtonHold;

    int bulletsLeft, bulletsShot;

    //bools
    bool shooting, readyToShoot, reloading;

    public Camera fpsCam;
    // where the projectile spawns.
    public Transform attackPoint;


    //bug fixing
    public bool allowInvoke = true;


    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MyInput();

    }
    private void MyInput()
    {
        //Check if allowed to hold down button and take corresponding input
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            //Set bullets shot to 0
            bulletsShot = 0;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;



        bulletsLeft--;
        bulletsShot++;

        Vector3 shootDirection = -attackPoint.position; //+targetPoint;
        //Instantiate bullet
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        //Rotate bullet to shoot direction
        currentBullet.transform.forward = shootDirection;
        //add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(shootDirection.normalized * shootForce, ForceMode.Impulse);
    }
}
