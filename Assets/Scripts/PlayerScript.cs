﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TetherAnchor
{
    public Vector2 anchorPoint;
    public TetherAnchor previousPoint;
    public float prevangle;
    public TetherAnchor(Vector2 new_anchorPoint, TetherAnchor new_prevPoint, float new_prevangle) {
        anchorPoint = new_anchorPoint;
        previousPoint = new_prevPoint;
        prevangle = new_prevangle;
    }
}

[RequireComponent(typeof(AudioSource))]
public class PlayerScript : MonoBehaviour
{
    public Camera cam;
    public Material matDeathUI;
    public Manager manager;
    public GameObject allCollectables;
    public bool isCarryingItem = false;

    public GameObject tether;
    public LineRenderer tetherLine;
    public float throwForce = 5.0f;
    public float springForce = 10.0f;
    public float maxTetherLength = 2.0f;

    public float attachedThrust = 200.0f;
    public float detachedThrust = 5.0f;
    public float maxSpeed = 200.0f;

    public AnimationCurve accelerationCurve;
    public DistanceJoint2D tetherJoint;

    public Sprite deadDoggo;
    public Sprite happyDoggo;

    public bool inside = true;
    public bool tethered = false;
    public bool Inside {
        get { return inside; }
        set {
            //if (inside == value) return;

            inside = value;
            foreach (var audioSource in FindObjectsOfType<AudioSource>()) {
                //if (!audioSource.loop) return;

                if ((inside == false && audioSource.outputAudioMixerGroup?.audioMixer?.name == "inside") ||
                    (inside == true && audioSource.outputAudioMixerGroup?.audioMixer?.name == "outside")) {
                    audioSource.mute = true;
                    //if (audioSource.volume > 0)
                    //    StartCoroutine(AudioHelper.FadeOut(audioSource, 1f));
                }
                else {
                    audioSource.mute = false;
                    //if (audioSource.volume == 0)
                    //    StartCoroutine(AudioHelper.FadeIn(audioSource, 1f));
                }
            }
        }
    }
    public float insideDrag = 0.1f;

    public AudioClip[] damageSounds;
    public AudioClip[] damageSoundsDim;
    public AudioClip[] deathSounds;
    public AudioClip[] deathSoundsDim;
    public AudioClip[] barkSounds;
    public AudioClip[] barkSoundsDim;
    public AudioClip[] pantingSounds;
    public AudioClip[] pantingSoundsDim;
    public AudioClip pickup;
    public AudioClip[] hitWall;
    public AudioClip[] hitWallDim;

    public float health = 1.0f;
    public Slider healthBar;

    public GameObject activeItem;
    GameObject player;
    GameObject pickableItem;
    GameObject currentDamage;

    Rigidbody2D myRb;
    Rigidbody2D tetherRb;

    bool isWithinACollectable = false;
    bool isWithinDamage = false;
    bool isItemAllowed = false;

    private float tetherLength;
    private Stack<TetherAnchor> tether_points;

    private Vector2 position;

    public float outsideBufferDist;

    [SerializeField]
    private Rail currentRail;

    public Vector3[] lineRendererPoints;

    private bool autoAttachedOnExit;
    private float lastExitTime;
    private float autoConnectTimeout;

    void Start() {
        player = gameObject;
        myRb = player.GetComponent<Rigidbody2D>();
        tetherRb = tether.GetComponent<Rigidbody2D>();
        tethered = false;
        healthBar.value = health;

        transform.position = this.transform.position;
        TetherAnchor attachPoint = new TetherAnchor(new Vector2(0, 0), null, 0);
        tether_points = new Stack<TetherAnchor>();
        tether_points.Push(attachPoint);
        autoConnectTimeout = 0.1f;
    }

    void Update() {

        if (inside)
        {
            autoAttachedOnExit = false;
            lastExitTime = Time.time;
        } else
        {
            if (!autoAttachedOnExit)
            {
                if(Time.time - lastExitTime > autoConnectTimeout)
                {
                    autoAttachedOnExit = true;
                    attachToClosestRail();
                }
            }
        }
        

        position.x = transform.position.x;
        position.y = transform.position.y;

        matDeathUI.SetFloat("_Strength", Mathf.Clamp01((Vector2.Distance(Vector2.zero, position) - 15) / 5));

        if (Input.GetMouseButton(1)) {
            if(Input.GetMouseButtonDown(1)){
                if (isCarryingItem){
                    if (isWithinDamage) {
                        UseItem(activeItem);
                    } else if(isWithinACollectable) {
                        SwapItem();
                    } else {
                        DropItem();
                    }
                } else {
                    if(isWithinACollectable) {
                        PickUpItem(pickableItem);
                    }
                }
            }
            if(isCarryingItem && isWithinDamage){
                UseItem(activeItem);
            }
        }

        if (!tether.activeSelf) {
            tether.transform.position = transform.position;
            if (Input.GetMouseButtonDown(0)) {
                if (currentRail == null) {
                    ThrowTether();
                }
                else {
                    currentRail = null;
                    resetTether();
                    //tether.GetComponent<Tether>().DetachTether();
                    tethered = false;
                }
            }
        }
        else {
            if (Input.GetMouseButtonDown(0)) {
                currentRail = null;
                tether.SetActive(false);
            }
        }

        ///////////////////////////////////////////////////////

        if (tethered) {

            if (tether_points.Count < 2 && currentRail) {
                Vector2 closestPoint = Vector2.zero;
                float distToRail = currentRail.getDistToRail(toVec2(transform.position), out closestPoint);
                tether_points.Peek().anchorPoint = closestPoint;
                tetherLength = distToRail;
            }

            RaycastHit playerTrace;
            if (tether_points.Count > 1) {
                Vector3 dir = (toVec3(tether_points.Peek().anchorPoint) - toVec3(position));
                Vector3 prevdir = toVec3(tether_points.Peek().previousPoint.anchorPoint) - toVec3(tether_points.Peek().anchorPoint);

                bool hitBox = Physics.Raycast(toVec3(position), dir.normalized, out playerTrace, dir.magnitude, 1 << 11);
                Vector2 flippedPrevDir = new Vector2(-prevdir.y, prevdir.x);
                float angle = Mathf.Sign(Vector2.Dot(toVec2(dir).normalized, flippedPrevDir.normalized));

                if (hitBox) {
                    createNewAnchor(playerTrace, tether_points.Peek(), angle);
                    dir = (toVec3(tether_points.Peek().anchorPoint) - toVec3(position));
                    prevdir = toVec3(tether_points.Peek().previousPoint.anchorPoint) - toVec3(tether_points.Peek().anchorPoint);
                    flippedPrevDir = new Vector2(-prevdir.y, prevdir.x);
                    angle = Mathf.Sign(Vector2.Dot(toVec2(dir).normalized, flippedPrevDir.normalized));
                    tether_points.Peek().prevangle = angle;
                }

                Debug.DrawLine(tether_points.Peek().anchorPoint, position, hitBox ? Color.red : Color.white);

                if (angle != tether_points.Peek().prevangle) tether_points.Pop();

            }
            else {
                Vector3 dir = (toVec3(tether_points.Peek().anchorPoint) - toVec3(position));
                bool hitBox = Physics.Raycast(toVec3(position), dir.normalized, out playerTrace, dir.magnitude, 1 << 11);
                if (hitBox) {
                    createNewAnchor(playerTrace, tether_points.Peek(), 1);
                    dir = (toVec3(tether_points.Peek().anchorPoint) - toVec3(position));
                    Vector3 prevdir = toVec3(tether_points.Peek().previousPoint.anchorPoint) - toVec3(tether_points.Peek().anchorPoint);
                    Vector2 flippedPrevDir = new Vector2(-prevdir.y, prevdir.x);
                    float angle = Mathf.Sign(Vector2.Dot(toVec2(dir).normalized, flippedPrevDir.normalized));
                    tether_points.Peek().prevangle = angle;
                }
                Debug.DrawLine(tether_points.Peek().anchorPoint, position, tetherLength > maxTetherLength ? Color.red : Color.white);
            }

        }

        bool renderLine = true;
        if (!tethered)
        {
            if (tether.active)
            {
                tether_points.Peek().anchorPoint = toVec2(tether.transform.position);
            }
            else
            {
                renderLine = false;
            }
        }

        if (renderLine)
        {
            tetherLine.enabled = true;
            lineRendererPoints = new Vector3[tether_points.Count + 1];
            float tempTetherLength = 0;
            int num = 0;
        foreach (TetherAnchor t in tether_points) {
            if (t.previousPoint != null) {
                    Debug.DrawLine(t.previousPoint.anchorPoint, t.anchorPoint, tetherLength > maxTetherLength ? Color.red : Color.white);
                    tempTetherLength += Vector2.Distance(t.previousPoint.anchorPoint, t.anchorPoint);
                }
            lineRendererPoints[num+1] = new Vector3(t.anchorPoint.x, t.anchorPoint.y, -0.01f);
                num++;
            }

            tempTetherLength += Vector2.Distance(tether_points.Peek().anchorPoint, position);
            tetherLength = tempTetherLength;
            lineRendererPoints[0] = position;
            tetherLine.SetVertexCount(lineRendererPoints.Length);
            tetherLine.SetPositions(lineRendererPoints);
        }
        else
        {
            tetherLine.enabled = false;
        }
    }

    public void resetTether() {
        tether_points = new Stack<TetherAnchor>();
        tether_points.Push(new TetherAnchor(position, null, 0));
    }

    void FixedUpdate() {
        if (transform.position.magnitude > 20) {
            Die();
        }
        Camera.main.transform.position = transform.position + new Vector3(0, 0, -10);
        //bool tethered = tether.GetComponent<Tether>().tethered;

        if (currentRail == null) {
            Vector3 tetherDiff = tether.transform.position - transform.position;
            if (tetherDiff.magnitude > maxTetherLength) {
                tether.SetActive(false);
            }
        }

        if (tethered) {
            tetherJoint.connectedAnchor = tether_points.Peek().anchorPoint;
            float lastSegmentLength = Vector2.Distance(tether_points.Peek().anchorPoint, position);
            float jointlength = maxTetherLength - (tetherLength - lastSegmentLength);
            tetherJoint.distance = jointlength;
        }
        else {
            tetherJoint.distance = 1000.0f;
        }

        if (myRb.velocity.magnitude > maxSpeed) {
            myRb.velocity = myRb.velocity.normalized * maxSpeed;
        }

        float thrust = tethered || inside ? attachedThrust : detachedThrust;
        Vector3 accelerationVector = new Vector3(Input.GetAxis("Horizontal") * thrust, Input.GetAxis("Vertical") * thrust, 0);
        Vector3 accelComponentInDir = myRb.velocity * (Vector3.Dot(accelerationVector, myRb.velocity) / Mathf.Pow(myRb.velocity.magnitude, 2));
        if (myRb.velocity.magnitude > 0) {
            accelComponentInDir *= 1 - accelerationCurve.Evaluate(Mathf.Clamp01(myRb.velocity.magnitude / maxSpeed));
            float dotprod = Mathf.Clamp01(Vector3.Dot(accelerationVector, myRb.velocity));
            accelerationVector = Vector3.Lerp(accelerationVector, accelerationVector - accelComponentInDir, dotprod);
        }

        accelerationVector = cam.transform.TransformVector(accelerationVector);

        if (manager.gameActive) {
            myRb.AddForce(toVec2(accelerationVector));
        }
    }

    void ThrowTether() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mouseDiff = mousePos - transform.position;
        mouseDiff.z = 0;

        tether.SetActive(true);
        tetherRb.velocity = myRb.velocity;
        tetherRb.AddForce(mouseDiff.normalized * throwForce, ForceMode2D.Impulse);
    }

    public void SetRail(GameObject rail) {
        currentRail = rail.GetComponent<Rail>();
        currentRail.resetRail();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Damage") {
            currentDamage = other.gameObject;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        AudioHelper.PlayInside(hitWall[Random.Range(0, hitWall.Length)], 0.01f, 0.05f, 0.2f);
        AudioHelper.PlayOutside(hitWallDim[Random.Range(0, hitWallDim.Length)], 0.01f, 0.05f, 0.2f);
    }

    void OnTriggerStay2D(Collider2D obj) {
        if (obj.tag == "Collectable") {
            isWithinACollectable = true;
            pickableItem = obj.transform.gameObject;
        }
        if (obj.tag == "Damage") {
            isWithinDamage = true;
            currentDamage = obj.gameObject;
        }
        if (obj.tag == "Inside") {
            Inside = true;
            myRb.drag = insideDrag;
        }

    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Collectable") {
            isWithinACollectable = false;
        }
        if (other.tag == "Inside") {
            Inside = false;   
            myRb.drag = 0;
        }
        if (other.tag == "Damage") {
            isWithinDamage = false;
            currentDamage = null;
        }
    }

    private void attachToClosestRail()
    {
        Rail[] allrails = GameObject.Find("Rails").transform.GetComponentsInChildren<Rail>();
        Rail closest = allrails[0];
        float dist = Mathf.Infinity;
        foreach(Rail r in allrails)
        {
            float tempdist = Vector3.Distance(r.transform.position, transform.position);
            if (tempdist < dist)
            {
                dist = tempdist;
                closest = r;
            }
        }
        tethered = true;
        SetRail(closest.transform.gameObject);
        resetTether();
    }

    void PickUpItem(GameObject obj) {
        AudioSource.PlayClipAtPoint(pickup, transform.position);
        activeItem = obj.gameObject;
        activeItem.transform.parent = player.transform;
        obj.GetComponent<BoxCollider2D>().enabled = false;
        PlaceItemCorrectly(activeItem);
        isCarryingItem = true;
    }

    void SwapItem() {
        AudioSource.PlayClipAtPoint(pickup, transform.position);
        DropItem();
        PickUpItem(pickableItem);
    }

    void DropItem() {
        activeItem.transform.parent = allCollectables.transform;
        activeItem.GetComponent<BoxCollider2D>().enabled = true;
        isCarryingItem = false;
    }

    void UseItem(GameObject item) {
        isItemAllowed = ItemCheck();
        if (currentDamage != null && isItemAllowed) {
            currentDamage.GetComponent<DamageController>().ReduceDamage();
            item.GetComponent<ToolBehaviour>().UseAndReduce();

            AudioHelper.PlayInside(barkSounds[Random.Range(0, barkSounds.Length)], 0.05f);
            AudioHelper.PlayOutside(barkSoundsDim[Random.Range(0, barkSoundsDim.Length)], 0.05f);
        }
    }

    bool ItemCheck() {
        if (currentDamage == null) {
            return false;
        }
        else if (activeItem.GetComponent<TypeManager>().type == TypeManager.Type.FireEx &&
                currentDamage.GetComponent<TypeManager>().type == TypeManager.Type.Fire) {
            return true;
        }
        else if (activeItem.GetComponent<TypeManager>().type == TypeManager.Type.Wrench &&
                currentDamage.GetComponent<TypeManager>().type == TypeManager.Type.Detachable) {
            return true;
        }
        else if (activeItem.GetComponent<TypeManager>().type == TypeManager.Type.Tape &&
                currentDamage.GetComponent<TypeManager>().type == TypeManager.Type.Damageable) {
            return true;
        }
        return false;
    }

    public void ThisItemDied(TypeManager.Type giveType) {
        if (activeItem.GetComponent<TypeManager>().type == giveType) {
            activeItem.GetComponent<ToolBehaviour>().DestroyTool();
            isCarryingItem = false;
            activeItem = null;
        }
    }

    void PlaceItemCorrectly(GameObject obj) {
        obj.transform.position = new Vector3(transform.position.x, transform.position.y, -0.1f);
    }

    public void Damage() {
        health -= 0.34f;
        healthBar.value = health;
        var sounds = Inside ? damageSounds : damageSoundsDim;
        AudioSource.PlayClipAtPoint(sounds[Random.Range(0, sounds.Length)], transform.position);

        if (health <= 0) {
            Die();
        }
    }

    public void Die() {
        var sounds = Inside ? damageSounds : damageSoundsDim;

        health = 0.0f;
        healthBar.value = health;
        sounds = Inside ? deathSounds : deathSoundsDim;
        GetComponent<SpriteRenderer>().sprite = deadDoggo;
        AudioSource.PlayClipAtPoint(sounds[Random.Range(0, sounds.Length)], transform.position);
        manager.GameOver();
    }

    public void GoodBoy() {
        GetComponent<SpriteRenderer>().sprite = happyDoggo;
    }

    private void createNewAnchor(RaycastHit hit, TetherAnchor prevPoint, float prevangle) {
        Vector3 hitLocalSpace = hit.transform.InverseTransformPoint(hit.point);
        Vector2 offset = new Vector2(Mathf.Sign(hitLocalSpace.x), Mathf.Sign(hitLocalSpace.y));

        Vector2 newTetherPosition = new Vector2(
            (offset.x * (hit.transform.localScale.x * 0.5f)) + offset.x * outsideBufferDist,
            (offset.y * (hit.transform.localScale.y * 0.5f)) + offset.y * outsideBufferDist
            );

        newTetherPosition = hit.transform.TransformPoint(newTetherPosition);
        tether_points.Push(new TetherAnchor(toVec3(newTetherPosition), prevPoint, prevangle));
    }
    public Vector3 toVec3(Vector2 vin) {
        return new Vector3(vin.x, vin.y, 0);
    }

    public Vector2 toVec2(Vector3 vin) {
        return new Vector2(vin.x, vin.y);
    }
}
