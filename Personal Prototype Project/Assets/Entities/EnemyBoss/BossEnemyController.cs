using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyController : EnemyManager
{
    public GameObject bossCirclingSword;
    public GameObject bossMiekkaPrefab;
    public GameObject bossArrowPrefab;

    public Vector3 startRotation;
    public Vector2 localPosition;
    public float threshold;
    public float targetDistance;

    public bool bossCirclingSwordsCooldown;
    public bool bossStrikeCooldown;
    public bool bossShootCooldown;
    public bool holdingPosition;

    public bool bossPhase2;
    public bool bossPhase2CirclingSwordsOnlyExecuteOnce;
    public bool bossPhase3;
    public bool bossPhase3CirclingSwordsOnlyExecuteOnce;

    // Start is called before the first frame update
    void OnGUI()
    {
        GUI.Label(new Rect(10, 30, 200, 30), "Enemy Health: " + enemyHealth);
    }
    void Start()
    {
        enemyHealth = 100;
        speed = 5;
    }

    // Update is called once per frame
    void Update()
    {
        targetDistance = PlayerDistanceCalculation();
        if (enemyHealth < 1)
        {
            Destroy(gameObject);
        }
        
        if (targetDistance >= 3.5 && targetDistance <= 30 && holdingPosition == false)
        {
            MoveTowardsPlayer();
        }
        
        if (targetDistance <= 3.5 && bossStrikeCooldown == false)
        {
            AIstrike();
            StartCoroutine(BossStrikeCooldown());
        }
        if (targetDistance <= 10 && bossShootCooldown == false)
        {
            BossShoot();
            if (bossPhase2 == true)
            {
                StartCoroutine(Phase2Barrage());
            }
            StartCoroutine(BossShootCooldown());

        }
        //CirclingSwords/Py�riv�t miekat mekaniikka
        //Local variablet deletoituu/unloadaa aina kun method on executennu. Ei pysty tekem��n onlyExecuteOnce methodia, joka toimii vain kerran, local variableilla.
        //Jos haluat, ett� automaattisesti, copypastaamalla tulee onlyExecuteOnce method, methodin nimen perusteella, sun pit�� tehd� siit� onlyExecuteOnce boolista globaali, methodin sis�ll�.
        //Mieti tulevaisuudessa, teetk� sen vai onko ihan sama, koska se, ett� teet t�mm�sen viritelm�n maksaa sun aikaa 15 sekuntia.
        if (bossPhase2 == true && bossPhase2CirclingSwordsOnlyExecuteOnce == false)
        {
            Instantiate(bossCirclingSword, transform.position, bossCirclingSword.transform.rotation, transform.parent = transform);
            bossPhase2CirclingSwordsOnlyExecuteOnce = true;
        }
        if (bossPhase3 == true)
        {
            if (bossPhase3 == true && bossPhase3CirclingSwordsOnlyExecuteOnce == false)
            {
                StartCoroutine(CirclingSwords());
                bossPhase3CirclingSwordsOnlyExecuteOnce = true;
            }
        }
        /*
         * Vector2.Angle ei toimi koska, kohteen "direction" muuttuu liikkuessa ja arvot heittelee. Toimii hyvin jos kohde ei liiku.
         * K�yt� alla olevaa kaavaa, jos haluat, ett� kahden pisteen v�lill� on AINA sama angle, aivan sama onko kohde liikkunu vai ei. +-180 astetta on v�li.
         * Id�ss� on 0, L�nness� 180. Pohjoispuoli on 0-180, Etel�puoli on 0-(-180)
         */
        void AIstrike()
        {
            float selfPosX = transform.position.x;
            float selfPosY = transform.position.y;
            float playerPosX = PlayerController.playerTransform.position.x;
            float playerPosY = PlayerController.playerTransform.position.y;

            Vector2 Point_1 = new Vector2(selfPosX, selfPosY);
            Vector2 Point_2 = new Vector2(playerPosX, playerPosY);
            float angle = Mathf.Atan2(Point_2.y - Point_1.y, Point_2.x - Point_1.x) * Mathf.Rad2Deg;

            /*
            Debug.Log("Angle is = " + angle);
            */

            //East
            if (angle >= -45 && angle <= 45)
            {

                startRotation = new Vector3(0f, 0f, -45f);
                localPosition = new Vector2(1, 1);
                threshold = -0.92f;
            }
            //North
            else if (angle >= 45 && angle <= 135)
            {
                startRotation = new Vector3(0f, 0f, 45f);
                localPosition = new Vector2(-1, 1);
                threshold = -0.38f;
            }
            //South
            else if (angle >= -135 && angle <= -45)
            {

                startRotation = new Vector3(0f, 0f, 225f);
                localPosition = new Vector2(1, -1);
                threshold = 0.92f;
            }
            //West
            /*
             * Muista, > jatkuu loputtomiin ja moottori ei salli sit� rajaksi (ellet erikseen k�yt�n Infinity ja MegativeInfinity). Arvot pit�� t�rm�t�, jotta tulee raja/true!
             * Eli aina muista, jos on kummatkin pelk�st��n >, niin ilmota rajat sille ja tarvii kaksi erillist� parametri�.
             * Tee niikuin alla, muista t�m�!!! Taas kerran 2-3 tuntia aikaa hukkaan, ennen ku tajusit t�n.
             * (else if angle >= 135 && angle <= -135) on aina false, eli EI TOIMI! Never again.
             */
            else if ((angle >= 135 && angle <= 180) || (angle >= -135 && angle <= -180))
            {
                startRotation = new Vector3(0f, 0f, 135f);
                localPosition = new Vector2(-1, -1);
                threshold = 0.38f;
            }
            Instantiate(bossMiekkaPrefab, transform.position, bossMiekkaPrefab.transform.rotation, transform.parent = transform);
        }
        /* Boss Shoot/projectile logiikka. Laskee pelaajan ja omat kordinaatit ja niitten v�lisen kulman.
         * T�m� on ampumiskulman, miss� p�in pelaaja on. Instantiate sitten spawnaa projectilen pelaajaa p�in suunnattuna.)
         */
        void BossShoot()
        {
            float selfPosX = transform.position.x;
            float selfPosY = transform.position.y;
            float playerPosX = PlayerController.playerTransform.position.x;
            float playerPosY = PlayerController.playerTransform.position.y;

            Vector2 Point_1 = new Vector2(selfPosX, selfPosY);
            Vector2 Point_2 = new Vector2(playerPosX, playerPosY);
            float rotation = Mathf.Atan2(Point_2.y - Point_1.y, Point_2.x - Point_1.x) * Mathf.Rad2Deg;

            //Miksi vitussa t�ss� pit�� rotationiin laittaa -90, ett� toi kaava pit�� paikkansa, ku PlayerMiekassa sit� ei tarvi laittaa. wtf? :D
            Vector3 projectileStartRotation = new Vector3(0f, 0f, rotation - 90);
            Quaternion quaternion = Quaternion.Euler(projectileStartRotation);

            /*
            Debug.Log("Rotation is = " + rotation);
            Debug.Log("StartRotation is = " + startRotation);
            */
            Instantiate(bossArrowPrefab, transform.position, quaternion);
            // Phase 3 Ominaisuus, 5 projectilea, mitk� forkkaa, kaikki eri kulmilla, joka tekee conen.
            if (bossPhase3 == true)
            {
                Vector3 children1Rotation = new Vector3(0f, 0f, rotation - 60);
                Vector3 children2Rotation = new Vector3(0f, 0f, rotation - 75);
                Vector3 children3Rotation = new Vector3(0f, 0f, rotation - 105);
                Vector3 children4Rotation = new Vector3(0f, 0f, rotation - 120);

                Quaternion children1Quaternion = Quaternion.Euler(children1Rotation);
                Quaternion children2Quaternion = Quaternion.Euler(children2Rotation);
                Quaternion children3Quaternion = Quaternion.Euler(children3Rotation);
                Quaternion children4Quaternion = Quaternion.Euler(children4Rotation);

                Instantiate(bossArrowPrefab, transform.position, children1Quaternion);
                Instantiate(bossArrowPrefab, transform.position, children2Quaternion);
                Instantiate(bossArrowPrefab, transform.position, children3Quaternion);
                Instantiate(bossArrowPrefab, transform.position, children4Quaternion);
            }
        }
        float PlayerDistanceCalculation()
        {
            targetDistance = Vector2.Distance(PlayerController.playerTransform.position, transform.position);
            return targetDistance;
        }
        void MoveTowardsPlayer()
        {
            Vector2 lookDirection = (PlayerController.playerTransform.position - transform.position).normalized;
            transform.Translate(lookDirection * speed * Time.deltaTime);
        }
        IEnumerator BossShootCooldown()
        {
            bossShootCooldown = true;
            yield return new WaitForSeconds(3);
            bossShootCooldown = false;
        }
        IEnumerator BossStrikeCooldown()
        {
            bossStrikeCooldown = true;
            yield return new WaitForSeconds(3);
            bossStrikeCooldown = false;
        }
        IEnumerator Phase2Barrage()
        {
            holdingPosition = true;
            yield return new WaitForSeconds((float)0.15);
            BossShoot();
            yield return new WaitForSeconds((float)0.15);
            BossShoot();
            yield return new WaitForSeconds((float)0.15);
            BossShoot();
            yield return new WaitForSeconds((float)0.15);
            BossShoot();
            yield return new WaitForSeconds((float)0.5);
            holdingPosition = false;

        }
        IEnumerator CirclingSwords()
        {
            Instantiate(bossCirclingSword, transform.position, bossCirclingSword.transform.rotation, transform.parent = transform);
            yield return new WaitForSeconds((float)0.2);
            Instantiate(bossCirclingSword, transform.position, bossCirclingSword.transform.rotation, transform.parent = transform);
            yield return new WaitForSeconds((float)0.2);
            Instantiate(bossCirclingSword, transform.position, bossCirclingSword.transform.rotation, transform.parent = transform);
            yield return new WaitForSeconds((float)0.2);
            Instantiate(bossCirclingSword, transform.position, bossCirclingSword.transform.rotation, transform.parent = transform);
            yield return new WaitForSeconds((float)0.2);
            Instantiate(bossCirclingSword, transform.position, bossCirclingSword.transform.rotation, transform.parent = transform);
        }
    }
}