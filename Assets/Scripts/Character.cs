using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character {

    private const float animationTimeSeconds = 1f;

    GameObject thisEntity;
    Animator thisAnimator;

    Vector3 myPosition;

    public Dictionary<char, GameObject> map = new Dictionary<char, GameObject>();

    bool moving = false;
    bool waitForPortal = false;

    char myShape;

    protected Character()
    {

    }

    public Character(Vector3 pos)
    {
        myPosition = pos;
        map.Add(MyDictionary.SHAPE_CUBE, Resources.Load("Cube", typeof(GameObject)) as GameObject);
        map.Add(MyDictionary.SHAPE_SPHERE, Resources.Load("Sphere", typeof(GameObject)) as GameObject);
        map.Add(MyDictionary.SHAPE_CROSS, Resources.Load("Cross", typeof(GameObject)) as GameObject);
        map.Add(MyDictionary.SHAPE_CYLINDER_CROSS, Resources.Load("CylinderCross", typeof(GameObject)) as GameObject);
        //map.Add(MyDictionary.SHAPE_PYRAMID, Resources.Load("Pyramid", typeof(GameObject)) as GameObject);
        //map.Add(MyDictionary.SHAPE_STICK, Resources.Load("Stick", typeof(GameObject)) as GameObject);
        myShape = MyDictionary.SHAPE_CUBE;
    }

    public void morph(char shape, bool playSound)
    {
        float oldY = 0;
        if (thisEntity != null)
        {
            oldY = thisEntity.GetComponentsInChildren<Transform>()[1].localScale.y / 2;
        }
        GameObject objectToLoad;
        map.TryGetValue(shape, out objectToLoad);
        Object.Destroy(thisEntity);
        Object.Destroy(thisAnimator);
        if (playSound)
        {
            LevelControl.Instance.GetComponents<AudioSource>()[3].Play();
        }
        thisEntity = GameObject.Instantiate(objectToLoad);
        thisEntity.transform.position = myPosition;
        thisAnimator = thisEntity.GetComponentInChildren<Animator>();
        Vector3 pos = myPosition;
        pos.y = pos.y + - oldY + (thisEntity.GetComponentsInChildren<Transform>()[1].localScale.y / 2);
        myPosition = pos;
        thisEntity.transform.position = pos;
        thisAnimator.Play("Idle");
        myShape = shape;
    }

    public IEnumerator move(Vector3 pos, int direction, string animation)
    {
        if (!moving && !waitForPortal)
        {
            moving = true;
            //Animate here
            thisEntity.transform.rotation = Quaternion.Euler(0, direction * 90, 0);
            thisAnimator.Play(animation);
            yield return new WaitForSeconds(animationTimeSeconds*1.1f);
            LevelControl.Instance.GetComponents<AudioSource>()[1].Play();
            pos.y = pos.y + (thisEntity.GetComponentsInChildren<Transform>()[1].localScale.y / 2);
            myPosition = pos;
            thisEntity.transform.position = pos;
            thisAnimator.Play("Idle");
            LevelControl.Instance.log("moved to " + pos);
            LevelControl.Instance.procMoveTriggers(pos);
            moving = false;
        }
        yield return 0;
    }

    public IEnumerator moveASAP(Vector3 pos)
    {
        waitForPortal = true;

        moving = true;
        //Animate here
        yield return new WaitForSeconds(animationTimeSeconds/3);
        LevelControl.Instance.GetComponents<AudioSource>()[1].Play();
        pos.y = pos.y + (thisEntity.GetComponentsInChildren<Transform>()[1].localScale.y / 2);
        myPosition = pos;
        thisEntity.transform.position = pos;
        moving = false;
        LevelControl.Instance.log("movedASAP to " + pos);
        waitForPortal = false;
        yield return new WaitForSeconds(0.001f);
    }

    public void forcePosition(Vector3 pos)
    {
        pos.y = pos.y + (thisEntity.GetComponentsInChildren<Transform>()[1].localScale.y / 2);
        myPosition = pos;
        thisEntity.transform.position = pos;
        LevelControl.Instance.log("position forced to " + pos);
    }

    public void forcePortalWait()
    {
        waitForPortal = true;
    }

    public Vector3 getPosition()
    {
        return myPosition;
    }

    public Vector3 getShapePosition()
    {
        return thisEntity.GetComponentsInChildren<Transform>()[1].position;
    }

    public IEnumerator tryMove(Vector3 pos)
    {
        if (!moving && !waitForPortal)
        {
            moving = true;
            LevelControl.Instance.GetComponents<AudioSource>()[2].Play();
            Light light = thisEntity.GetComponentInChildren<Light>();
            light.enabled = false;
            if (myShape.Equals(MyDictionary.SHAPE_CUBE) || myShape.Equals(MyDictionary.SHAPE_SPHERE))
            {
                Color previous = thisEntity.GetComponentInChildren<Renderer>().material.color;
                Color previousEmission = thisEntity.GetComponentInChildren<Renderer>().material.GetColor("_EmissionColor");
                thisEntity.GetComponentInChildren<Renderer>().material.color = Color.black;
                thisEntity.GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", Color.black);
                yield return new WaitForSeconds(animationTimeSeconds / 2);
                thisEntity.GetComponentInChildren<Renderer>().material.color = previous;
                thisEntity.GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", previousEmission);
            }
            else
            {
                Color[] previous = new Color[50];
                Color[] previousEmission = new Color[50];
                int i = 0;
                foreach (Renderer renderer in thisEntity.GetComponentsInChildren<Renderer>())
                {
                    previous[i] = renderer.material.color;
                    previousEmission[i] = renderer.material.GetColor("_EmissionColor");
                    i++;
                    renderer.material.color = Color.black;
                    renderer.material.SetColor("_EmissionColor", Color.black);
                }

                yield return new WaitForSeconds(animationTimeSeconds / 2);

                i = 0;
                foreach (Renderer renderer in thisEntity.GetComponentsInChildren<Renderer>())
                {
                    renderer.material.color = previous[i];
                    renderer.material.SetColor("_EmissionColor", previousEmission[i]);
                    i++;
                }

            }
            light.enabled = true;
            moving = false;
            LevelControl.Instance.log("invalid move at " + myPosition);
        }
    }

    public bool isMoving()
    {
        return moving;
    }

    public char getShape()
    {
        return myShape;
    }

    public void setShape(char shape)
    {
        this.myShape = shape;
    }
}
