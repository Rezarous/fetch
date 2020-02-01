using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TetherAnchor
{
    public Vector2 anchorPoint;
    public TetherAnchor previousPoint;
    public float prevangle;
    public TetherAnchor (Vector2 new_anchorPoint, TetherAnchor new_prevPoint, float new_prevangle)
    {
        anchorPoint = new_anchorPoint;
        previousPoint = new_prevPoint;
        prevangle = new_prevangle;
    }
}

[RequireComponent(typeof(AudioSource))]
public class PlayerScript : MonoBehaviour {
    public Manager manager;
    public GameObject allCollectables;
    public bool isCarryingItem = false;

    public GameObject tether;
    public float throwForce = 5.0f;
    public float springForce = 10.0f;
    public float maxTetherLength = 2.0f;

    public float attachedThrust = 200.0f;
    public float detachedThrust = 5.0f;
    public float maxSpeed = 200.0f;

    public AnimationCurve accelerationCurve;

    public bool inside = true;
    public float insideDrag = 0.1f;

    public AudioClip damageSound;
    public AudioClip deathSound;
    public AudioClip pickup;

    public float health = 1.0f;
    public Slider healthBar;

    public GameObject activeItem;
    GameObject player;
    GameObject pickableItem;
    GameObject currentDamage;

    Rigidbody2D myRb;
    Rigidbody2D tetherRb;
    
    bool isWithinACollectable = false;
    bool isItemAllowed = false;

    private float tetherLength;
    private Stack<TetherAnchor> tether_points;

    private Vector2 position;

    public float outsideBufferDist;

    [SerializeField]
    private Rail currentRail;

    void Start() {
        player = gameObject;
        myRb = player.GetComponent<Rigidbody2D>();
        tetherRb = tether.GetComponent<Rigidbody2D>();

        healthBar.value = health;

        transform.position = this.transform.position;
        TetherAnchor attachPoint = new TetherAnchor(new Vector2(0, 0), null, 0);
        tether_points = new Stack<TetherAnchor>();
        tether_points.Push(attachPoint);
    }

    void Update() {
        position.x = transform.position.x;
        position.y = transform.position.y;

        if(Input.GetKeyDown(KeyCode.Space)){
            if(isWithinACollectable && !isCarryingItem){
                PickUpItem(pickableItem);
            }else if(isWithinACollectable && isCarryingItem){
                SwapItem();
            }else if(isCarryingItem){
                DropItem();
            }
        }

        if(Input.GetKey(KeyCode.F) && isCarryingItem) {
            UseItem(activeItem);
        }

        ///////////////////////////////////////////////////////
        
        
        if(tether_points.Count < 2 && currentRail)
        {
            Vector2 closestPoint = Vector2.zero;
            float distToRail = currentRail.getDistToRail(toVec2(transform.position), out closestPoint);
            tether_points.Peek().anchorPoint = closestPoint;
            tetherLength = distToRail;
        }

        ///////////////////////////////////////////////////////
        
        RaycastHit playerTrace;
        if (tether_points.Count > 1)
        {
            Vector3 dir = (toVec3(tether_points.Peek().anchorPoint) - toVec3(position));
            Vector3 prevdir = toVec3(tether_points.Peek().previousPoint.anchorPoint) - toVec3(tether_points.Peek().anchorPoint);
            
            bool hitBox = Physics.Raycast(toVec3(position), dir.normalized, out playerTrace, dir.magnitude, 1 << 11);
            Vector2 flippedPrevDir = new Vector2(-prevdir.y, prevdir.x);
            float angle = Mathf.Sign(Vector2.Dot(toVec2(dir).normalized, flippedPrevDir.normalized));

            if (hitBox) 
            {
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
        else
        {
            Vector3 dir = (toVec3(tether_points.Peek().anchorPoint) - toVec3(position));
            bool hitBox = Physics.Raycast(toVec3(position), dir.normalized, out playerTrace, dir.magnitude, 1 << 11);
            if (hitBox)
            {
                createNewAnchor(playerTrace, tether_points.Peek(), 1);
                dir = (toVec3(tether_points.Peek().anchorPoint) - toVec3(position));
                Vector3 prevdir = toVec3(tether_points.Peek().previousPoint.anchorPoint) - toVec3(tether_points.Peek().anchorPoint);
                Vector2 flippedPrevDir = new Vector2(-prevdir.y, prevdir.x);
                float angle = Mathf.Sign(Vector2.Dot(toVec2(dir).normalized, flippedPrevDir.normalized));
                tether_points.Peek().prevangle = angle;
            }
            Debug.DrawLine(tether_points.Peek().anchorPoint, position, tetherLength > maxTetherLength ? Color.red : Color.white);
        }

        float tempTetherLength = 0;
        foreach(TetherAnchor t in tether_points)
        {
            if (t.previousPoint != null)
            {
                Debug.DrawLine(t.previousPoint.anchorPoint, t.anchorPoint, tetherLength > maxTetherLength ? Color.red : Color.white);
                tempTetherLength += Vector2.Distance(t.previousPoint.anchorPoint, t.anchorPoint);
            }
        }
        tempTetherLength += Vector2.Distance(tether_points.Peek().anchorPoint, position);
        tetherLength = tempTetherLength;
    }

    void FixedUpdate() {
        Camera.main.transform.position = transform.position + new Vector3(0, 0, -10);

        /*if (!tether.activeSelf) {
            tether.transform.position = transform.position;
            if (Input.GetMouseButtonDown(0)) {
                ThrowTether();
            }
        } else {
            if (Input.GetMouseButtonDown(0)) {
                currentRail = null;
                tether.SetActive(false);
            }
        }*/

        /*if (currentRail == null) {
            Vector3 tetherDiff = tether.transform.position - transform.position;
            if (tetherDiff.magnitude > maxTetherLength) {
                tetherRb.AddForce(-tetherDiff.normalized * springForce, ForceMode2D.Impulse);
                myRb.AddForce(tetherDiff.normalized * springForce, ForceMode2D.Impulse);
            }
        } else if (tetherLength > maxTetherLength) {
            Vector3 effectiveTether = toVec3(tether_points.Peek().anchorPoint - position);
            myRb.AddForce(effectiveTether.normalized * springForce, ForceMode2D.Impulse);
        }*/

        // bool tethered = tether.GetComponent<Tether>().tethered;
        if(myRb.velocity.magnitude > maxSpeed)
        {
            myRb.velocity = myRb.velocity.normalized * maxSpeed;
        }


        bool tethered = true;
        float thrust = tethered || inside ? attachedThrust : detachedThrust;
        thrust *= accelerationCurve.Evaluate(Mathf.Clamp01(myRb.velocity.magnitude / maxSpeed));
        if (manager.gameActive) {
            myRb.AddForce(new Vector3(Input.GetAxis("Horizontal") * thrust, Input.GetAxis("Vertical") * thrust, 0));
        }
    }

    void ThrowTether() {
        /*Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mouseDiff = mousePos - transform.position;
        mouseDiff.z = 0;

        tether.SetActive(true);
        tetherRb.velocity = myRb.velocity;
        tetherRb.AddForce(mouseDiff.normalized * throwForce, ForceMode2D.Impulse);
        myRb.AddForce(-mouseDiff.normalized * throwForce, ForceMode2D.Impulse);*/
    }

    public void SetRail(GameObject rail) {
        currentRail = rail.GetComponent<Rail>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Damage") {
            currentDamage = other.gameObject;
        } else if (other.tag == "Inside") {
            inside = true;
            myRb.drag = insideDrag;
        }
    }

    void OnTriggerStay2D(Collider2D obj) {
        if(obj.tag == "Collectable") {
            isWithinACollectable = true;
            pickableItem = obj.transform.gameObject;
        }
        if(obj.tag == "Damage") {
            currentDamage = obj.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        isWithinACollectable = false;
        if (other.tag == "Inside") {
            inside = false;
            myRb.drag = 0;
        }
        if (other.tag == "Damage") {
            currentDamage = null;
        }
    }

    void PickUpItem(GameObject obj) {
        AudioSource.PlayClipAtPoint(pickup, transform.position);
        activeItem = obj.gameObject;
        activeItem.transform.parent = player.transform;
        obj.GetComponent<BoxCollider2D>().enabled = false;
        PlaceItemCorrectly(activeItem);
        isCarryingItem = true;
    }
    
    void SwapItem(){
        AudioSource.PlayClipAtPoint(pickup, transform.position);
        DropItem();
        PickUpItem(pickableItem);
    }

    void DropItem(){
        activeItem.transform.parent = allCollectables.transform;
        activeItem.GetComponent<BoxCollider2D>().enabled = true;
        isCarryingItem = false;
    }

    void UseItem(GameObject item){
        isItemAllowed = ItemCheck();
        if(currentDamage != null && isItemAllowed){
            currentDamage.GetComponent<DamageController>().ReduceDamage();
            item.GetComponent<ToolBehaviour>().UseAndReduce();
        }
    }

    bool ItemCheck(){
        if(currentDamage == null){
            return false;
        }
        else if (activeItem.GetComponent<TypeManager>().type == TypeManager.Type.FireEx && 
                currentDamage.GetComponent<TypeManager>().type == TypeManager.Type.Fire){
            return true;
        }
        else if (activeItem.GetComponent<TypeManager>().type == TypeManager.Type.Wrench && 
                currentDamage.GetComponent<TypeManager>().type == TypeManager.Type.Detachable){
            return true;
        }
        else if (activeItem.GetComponent<TypeManager>().type == TypeManager.Type.Tape && 
                currentDamage.GetComponent<TypeManager>().type == TypeManager.Type.Damageable){
            return true;
        }
        return false;
    }

    public void ThisItemDied(TypeManager.Type giveType) {
        if(activeItem.GetComponent<TypeManager>().type == giveType){
            activeItem.GetComponent<ToolBehaviour>().DestroyTool();
            isCarryingItem = false;
            activeItem = null;
        }
    }

    void PlaceItemCorrectly(GameObject obj){
        obj.transform.position = new Vector3(transform.position.x, transform.position.y, -0.1f);
    }

    public void Damage() {
        health -= 0.34f;
        healthBar.value = health;
        if (health <= 0) {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
            manager.GameOver();
        } else {
            AudioSource.PlayClipAtPoint(damageSound, transform.position);
        }
    }

    private void createNewAnchor(RaycastHit hit, TetherAnchor prevPoint, float prevangle)
    {
        Vector3 hitLocalSpace = hit.transform.InverseTransformPoint(hit.point);
        Vector2 offset = new Vector2(Mathf.Sign(hitLocalSpace.x), Mathf.Sign(hitLocalSpace.y));

        Vector2 newTetherPosition = new Vector2(
            (offset.x * (hit.transform.localScale.x * 0.5f)) + offset.x * outsideBufferDist,
            (offset.y * (hit.transform.localScale.y * 0.5f)) + offset.y * outsideBufferDist
            );

        newTetherPosition = hit.transform.TransformPoint(newTetherPosition);
        tether_points.Push(new TetherAnchor(toVec3(newTetherPosition), prevPoint, prevangle));
    }
    public Vector3 toVec3(Vector2 vin)
    {
        return new Vector3(vin.x, vin.y, 0);
    }

    public Vector2 toVec2(Vector3 vin)
    {
        return new Vector2(vin.x, vin.y);
    }
}
