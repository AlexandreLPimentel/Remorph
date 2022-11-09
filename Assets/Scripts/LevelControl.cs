using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using System.Diagnostics;
using System.Threading;

public class LevelControl : MonoBehaviour
{

    //singleton code
    private static LevelControl instance = null;
    private int KEY_UP = 1;
    private int KEY_DOWN = -1;
    private int KEY_LEFT = -10;
    private int KEY_RIGHT = 10;
    private const int ACTION = ' ';
    private const float DELAY = 0.1f;
    private Character character = null;
    private Color portalColorInactive = new Color(120f / 255f, 120f / 255f, 120f / 255f);

    private Color[] portalColors = { new Color(255f / 255f,   0f / 255f,   0f / 255f),
                                     new Color(255f / 255f, 127f / 255f,   0f / 255f),
                                     new Color(255f / 255f, 255f / 255f,   0f / 255f),
                                     //new Color(150f / 255f, 255f / 255f,   0f / 255f), //skip this for bad visibility
                                     new Color(  0f / 255f, 255f / 255f,   0f / 255f),
                                     new Color(  0f / 255f, 255f / 255f, 127f / 255f),
                                     new Color(  0f / 255f, 255f / 255f, 255f / 255f),
                                     new Color(  0f / 255f, 127f / 255f, 255f / 255f),
                                     new Color(  0f / 255f,   0f / 255f, 255f / 255f),
                                     new Color(127f / 255f,   0f / 255f, 255f / 255f),
                                     new Color(255f / 255f,   0f / 255f, 255f / 255f),
                                     new Color(225f / 255f,   0f / 255f, 127f / 255f)
    };
    int currentPortalColor = 0;

    private LevelEntity[,,] levelLayout;
    private int currentLevel = 1;
    private Dictionary<Vector3, char> morphTriggers;
    private Dictionary<Vector3, Vector3> teleportTriggers;
    private Dictionary<Vector3, char> moveTriggers;
    private int cameraPosition = 0;
    private Vector3 cameraOffset;
    private GameObject portalParticles;
    private List<GameObject> teleportList = new List<GameObject>();
    private List<GameObject> triggerList = new List<GameObject>();
    Stopwatch stopwatch = new Stopwatch();
    Stopwatch stopwatchIdle = new Stopwatch();
    bool canLoadSpecificLevel = false;

    private Boolean isLoadingLevel;

    private static System.IO.StreamWriter file;

    protected LevelControl()
    {

    }

    public static LevelControl Instance
    {
        get
        {
            return instance;
        }
    }
    
    // Use this for initialization
    void Awake()
    {

        //First take care of singleton stuff

        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        character = new Character(new Vector3(0, 0, 0));
        morphTriggers = new Dictionary<Vector3, char>();
        teleportTriggers = new Dictionary<Vector3, Vector3>();
        moveTriggers = new Dictionary<Vector3, char>();
        portalParticles = Resources.Load("Portal", typeof(GameObject)) as GameObject;

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        //Now load the game stuff

        levelLayout = new LevelEntity[20, 20, 20];
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                for (int k = 0; k < 20; k++)
                {
                    levelLayout[i, j, k] = new LevelEntity(new Vector3(i, j, k));
                }
            }
        }

        restartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            log("pressed any key");
        }

        if (Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.Joystick1Button7))
        {
            if (!isLoadingLevel) {
                isLoadingLevel = true; //forced here because of how coroutines work
                character.morph(MyDictionary.SHAPE_CUBE, false);
                StartCoroutine(loadLevel(currentLevel));
                log("restarted level");
            }
            return;
        }

        if (Input.GetKey(KeyCode.Backslash))
        {
            canLoadSpecificLevel = !canLoadSpecificLevel;
        }

        if (canLoadSpecificLevel && !isLoadingLevel)
        {
            if (Input.GetKey(KeyCode.Alpha1))
            {
                isLoadingLevel = true;
                currentLevel = 1;
                StartCoroutine(loadLevel(currentLevel));
                character.morph(MyDictionary.SHAPE_CUBE, true);
            }
            else if (Input.GetKey(KeyCode.Alpha2))
            {
                isLoadingLevel = true;
                currentLevel = 2;
                StartCoroutine(loadLevel(currentLevel));
                character.morph(MyDictionary.SHAPE_CUBE, true);
            }
            else if (Input.GetKey(KeyCode.Alpha3))
            {
                isLoadingLevel = true;
                currentLevel = 3;
                StartCoroutine(loadLevel(currentLevel));
                character.morph(MyDictionary.SHAPE_CUBE, true);
            }
            else if (Input.GetKey(KeyCode.Alpha4))
            {
                isLoadingLevel = true;
                currentLevel = 4;
                StartCoroutine(loadLevel(currentLevel));
                character.morph(MyDictionary.SHAPE_CUBE, true);
            }
            else if (Input.GetKey(KeyCode.Alpha5))
            {
                isLoadingLevel = true;
                currentLevel = 5;
                StartCoroutine(loadLevel(currentLevel));
                character.morph(MyDictionary.SHAPE_CUBE, true);
            }
            else if (Input.GetKey(KeyCode.Alpha6))
            {
                isLoadingLevel = true;
                currentLevel = 6;
                StartCoroutine(loadLevel(currentLevel));
                character.morph(MyDictionary.SHAPE_CUBE, true);
            }
            else if (Input.GetKey(KeyCode.Alpha7))
            {
                isLoadingLevel = true;
                currentLevel = 7;
                StartCoroutine(loadLevel(currentLevel));
                character.morph(MyDictionary.SHAPE_CUBE, true);
            }
            else if (Input.GetKey(KeyCode.Alpha8))
            {
                isLoadingLevel = true;
                currentLevel = 8;
                StartCoroutine(loadLevel(currentLevel));
                character.morph(MyDictionary.SHAPE_CUBE, true);
            }
            else if (Input.GetKey(KeyCode.Alpha9))
            {
                isLoadingLevel = true;
                currentLevel = 9;
                StartCoroutine(loadLevel(currentLevel));
                character.morph(MyDictionary.SHAPE_CUBE, true);
            }
            else if (Input.GetKey(KeyCode.N))
            {
                isLoadingLevel = true;
                currentLevel++;
                if (currentLevel > 12)
                {
                    currentLevel = 1;
                }
                StartCoroutine(loadLevel(currentLevel));
                character.morph(MyDictionary.SHAPE_CUBE, true);
            }
        }

        if (isLoadingLevel)
        {
            return;
        }

        if (Camera.main.orthographic)
        {
            if (Input.anyKeyDown)
            {
                Camera.main.orthographic = false;
                stopwatch.Reset();
                stopwatch.Start();
            }
            else
            {
                return;
            }
        }

        if(Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.Joystick1Button6))
        {
            log("forced game finish");
            currentLevel = 1;
            restartGame();
        }

        parseCameraControl();
        parseCharacterControl();
        
    }

    void OnGUI()
    {
        if (!isLoadingLevel && Camera.main.orthographic)
        {
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height - 25, 200, 50), "Press any key to continue.", centeredStyle);
        }
    }

    void LateUpdate()
    {
        if (isLoadingLevel || Camera.main.orthographic)
        {
            return;
        }
        transform.position = character.getShapePosition() + cameraOffset;
        transform.LookAt(character.getShapePosition());
    }


    private IEnumerator loadLevel(int levelId)
    {

        isLoadingLevel = true;
        currentPortalColor = 0;

        stopwatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopwatch.Elapsed;

        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        log("completed level " + (currentLevel - 1) + " in " + elapsedTime);

        foreach (LevelEntity entity in levelLayout)
        {
            entity.unload();
        }

        try
        {
            string line;
            int layerNumber = 0; //height coordinate y
            int subLineNumber = 0; //depth coordinate x
            int columnNumber = 0; //width coordinate z

            TextAsset puzdata = (TextAsset)Resources.Load("Levels/level" + levelId, typeof(TextAsset));
            StringReader reader = new StringReader(puzdata.text);

            //This is the legacy way of reading it, hopefully the other one works on every platform
            //StreamReader reader = new StreamReader("Levels/level" + levelId + ".txt", Encoding.Default);

            foreach(GameObject teleport in teleportList)
            {
                Destroy(teleport);
            }
            teleportList.Clear();

            foreach (GameObject trigger in triggerList)
            {
                Destroy(trigger);
            }
            triggerList.Clear();

            bool parsingTransformations = false;
            Camera.main.orthographic = true;

            using (reader)
            {
                line = reader.ReadLine();
                int size = Convert.ToInt32(line.Split('*')[0]);
                Camera.main.orthographicSize = size*0.6f;
                do
                {
                    line = reader.ReadLine();
                    //Parse the level entries
                    if (line != null && line[0] != 'T' && !parsingTransformations)
                    {
                        string[] entries = line.Split(',');
                        if (entries.Length > 0)
                        {
                            if (entries.Length == 1)
                            {
                                layerNumber = Convert.ToInt32(entries[0]);
                                moveCameraToPreview(layerNumber, size);
                                subLineNumber = 0;
                                continue;
                            }
                            columnNumber = 0;
                            foreach (string entry in entries)
                            {
                                int type = levelLayout[subLineNumber, layerNumber - 1, columnNumber].load(entry);
                                if (entry[0] != 'N')
                                {
                                    yield return new WaitForSeconds(DELAY * 5 / size);
                                }
                                if(type == 1)
                                {
                                    float heightToAdd = levelLayout[subLineNumber, layerNumber - 1, columnNumber].getPlacementHeight();
                                    character.forcePosition(new Vector3(subLineNumber, layerNumber - 1 + heightToAdd, columnNumber));
                                }
                                columnNumber++;
                            }
                            subLineNumber++;
                        }
                    }
                    else if(line != null && line[0] == 'T')
                    {
                        parsingTransformations = true;
                        morphTriggers.Clear();
                        teleportTriggers.Clear();
                        moveTriggers.Clear();
                    }
                    else if(line != null)
                    {
                        if(line[8] == ':')
                        {
                            int x = Convert.ToInt32(line.Substring(0, 2));
                            int y = Convert.ToInt32(line.Substring(3, 2));
                            int z = Convert.ToInt32(line.Substring(6, 2));
                            morphTriggers.Add(new Vector3(x, y, z), line[9]);

                            float height = levelLayout[x, y, z].getPlacementHeight();
                            GameObject gameObject;
                            character.map.TryGetValue(line[9], out gameObject);
                            GameObject newObject = GameObject.Instantiate(gameObject, new Vector3(x, y + height + 0.1f, z), Quaternion.identity);
                            Vector3 scale = newObject.transform.localScale;
                            scale.x *= 0.5f;
                            scale.y *= 0.5f;
                            scale.z *= 0.5f;
                            newObject.transform.localScale = scale;
                            triggerList.Add(newObject);
                            yield return new WaitForSeconds(DELAY * 5 / size);
                        }
                        else if(line[8] == '=')
                        {
                            int x = Convert.ToInt32(line.Substring(0, 2));
                            int y = Convert.ToInt32(line.Substring(3, 2));
                            int z = Convert.ToInt32(line.Substring(6, 2));

                            int a = Convert.ToInt32(line.Substring(9, 2));
                            int b = Convert.ToInt32(line.Substring(12, 2));
                            int c = Convert.ToInt32(line.Substring(15, 2));
                            teleportTriggers.Add(new Vector3(x, y, z), new Vector3(a, b, c));
                            teleportTriggers.Add(new Vector3(a, b, c), new Vector3(x, y, z));
                            float height1 = levelLayout[x, y, z].getPlacementHeight();
                            float height2 = levelLayout[a, b, c].getPlacementHeight();
                            teleportList.Add(GameObject.Instantiate(portalParticles, new Vector3(x, y + height1, z), portalParticles.transform.rotation));
                            teleportList.Add(GameObject.Instantiate(portalParticles, new Vector3(a, b + height2, c), portalParticles.transform.rotation));
                            yield return new WaitForSeconds(DELAY * 5 / size);
                        }
                        else if(line[8] == '>')
                        {
                            int x = Convert.ToInt32(line.Substring(0, 2));
                            int y = Convert.ToInt32(line.Substring(3, 2));
                            int z = Convert.ToInt32(line.Substring(6, 2));
                            moveTriggers.Add(new Vector3(x, y, z), line[9]);

                        }
                    }
                } while (line != null);
                reader.Close();
            }
        }
        finally
        {
            isLoadingLevel = false;
        }


    }

    private void moveCameraToPreview(int height, int size)
    {
        transform.position = new Vector3(size / 2, height + 2, size / 2);
        transform.LookAt(new Vector3(size / 2, 0, size / 2));
    }

    private void parseCharacterControl()
    {
        if (character.isMoving())
        {
            if (stopwatchIdle.IsRunning)
            {
                stopwatchIdle.Stop();
            }
            return;
        }
        else if (!stopwatchIdle.IsRunning)
        {
            stopwatchIdle.Start();
        }

        if (keyPressed(ACTION))
        {
            procMoveTriggers(character.getPosition());
        }

        int move = 0;

        if (keyPressed(KEY_UP))
        {
            move += KEY_UP;
        }

        if (keyPressed(KEY_DOWN))
        {
            move += KEY_DOWN;
        }

        if (keyPressed(KEY_RIGHT))
        {
            move += KEY_RIGHT;
        }

        if (keyPressed(KEY_LEFT))
        {
            move += KEY_LEFT;
        }

        if (move == 0 || move > 10 || move < -10)
        {
            //more than one key was pressed at the same time or none pressed at all
            return;
        }
        
        Vector3 currentPosition = character.getPosition();
        Vector3 newPosition = new Vector3(currentPosition.x + ((int)move / 10), (int) currentPosition.y, currentPosition.z + move % 10);
        
        char shape = character.getShape();

        float? height = canMove(currentPosition, newPosition, shape);
        if (height != null)
        {
            newPosition.Set(newPosition.x, newPosition.y + height.Value, newPosition.z);

            int direction;

            switch (move)
            {
                case 1:
                    direction = -1;
                    break;
                case 10:
                    direction = 0;
                    break;
                case -1:
                    direction = -3;
                    break;
                case -10:
                    direction = -2;
                    break;
                default:
                    direction = 0;
                    break;
            }

            MyDictionary.Platform originType = levelLayout[(int) currentPosition.x, (int) currentPosition.y, (int) currentPosition.z].getShape();
            MyDictionary.Platform destinationType = levelLayout[(int) newPosition.x, (int) newPosition.y, (int) newPosition.z].getShape();
            string animation;
            if(destinationType == MyDictionary.Platform.Stairs && originType != MyDictionary.Platform.High)
            {
                animation = "MoveUp";
            }
            else if (originType == MyDictionary.Platform.Ramp && destinationType != MyDictionary.Platform.High)
            {
                animation = "MoveDown";
            }
            else
            {
                animation = "Move";
            }

            StartCoroutine(character.move(newPosition, direction, animation));
        }
        else
        {
            StartCoroutine(character.tryMove(newPosition));
        }

    }

    private void parseCameraControl()
    {
        float direction = 0;
        float distance = 1.5f;
        float height = 0.86604f;

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button5))
        {
            direction += distance;
            log("rotated camera");
        }
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Joystick1Button4))
        {
            direction -= distance;
            log("rotated camera");
        }

        if(direction != 0)
        {
            Vector3 position = new Vector3();
            switch (cameraPosition)
            {
                case 0:
                    position = new Vector3(direction, height, 0);
                    if (direction > 0)
                    {
                        cameraPosition = 3;
                    }
                    else
                    {
                        cameraPosition = 1;
                    }
                    break;
                case 1:
                    position = new Vector3(0, height, -direction);
                    if (direction > 0)
                    {
                        cameraPosition = 0;
                    }
                    else
                    {
                        cameraPosition = 2;
                    }
                    break;
                case 2:
                    position = new Vector3(-direction, height, 0);
                    if (direction > 0)
                    {
                        cameraPosition = 1;
                    }
                    else
                    {
                        cameraPosition = 3;
                    }
                    break;
                case 3:
                    position = new Vector3(0, height, direction);
                    if (direction > 0)
                    {
                        cameraPosition = 2;
                    }
                    else
                    {
                        cameraPosition = 0;
                    }
                    break;
                default:
                    break;
            }
            cameraOffset = position;  
            if(direction < 0)
            {
                int tmpKey = KEY_LEFT;
                KEY_LEFT = KEY_UP;
                KEY_UP = KEY_RIGHT;
                KEY_RIGHT = KEY_DOWN;
                KEY_DOWN = tmpKey;
            }
            if(direction > 0)
            {
                int tmpKey = KEY_LEFT;
                KEY_LEFT = KEY_DOWN;
                KEY_DOWN = KEY_RIGHT;
                KEY_RIGHT = KEY_UP;
                KEY_UP = tmpKey;
            }
        }
    }

    private Boolean keyPressed(int key)
    {

        if (key == KEY_LEFT && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || (Input.GetAxis("Horizontal") < 0)))
        {
            return true;
        }

        if (key == KEY_RIGHT && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || (Input.GetAxis("Horizontal") > 0)))
        {
            return true;
        }

        if (key == KEY_UP && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || (Input.GetAxis("Vertical") > 0)))
        {
            return true;
        }

        if (key == KEY_DOWN && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || (Input.GetAxis("Vertical") < 0)))
        {
            return true;
        }

        if(key == ACTION && (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.Joystick1Button0) || Input.GetKey(KeyCode.Joystick1Button1) || Input.GetKey(KeyCode.Joystick1Button2) || Input.GetKey(KeyCode.Joystick1Button3)))
        {
            return true;
        }
        
        return false;

    }

    private float? canMove(Vector3 origin, Vector3 destination, char shape)
    {
        if(destination.x < 0 || destination.y < 0 || destination.z < 0)
        {
            return null;
        }

        LevelEntity destinationTile = levelLayout[(int) destination.x, (int) destination.y, (int) destination.z];

        if(destinationTile == null)
        {
            return null;
        }

        LevelEntity originTile = levelLayout[(int)origin.x, (int)origin.y, (int)origin.z];

        if(originTile.getShape() == MyDictionary.Platform.Stairs && destinationTile.getShape() == MyDictionary.Platform.Low)
        {
            return null;
        }

        if ((originTile.getShape() == MyDictionary.Platform.High && destinationTile.getShape() == MyDictionary.Platform.Low)
            || (originTile.getShape() == MyDictionary.Platform.Low&& destinationTile.getShape() == MyDictionary.Platform.High)
            || (originTile.getShape() == MyDictionary.Platform.Start&& destinationTile.getShape() == MyDictionary.Platform.High)
            || (destinationTile.getShape() == MyDictionary.Platform.End && destinationTile.getShape() == MyDictionary.Platform.High))
        {
            return null;
        }

        if(character.getShape() != MyDictionary.SHAPE_SPHERE && originTile.getShape() == MyDictionary.Platform.Ramp)
        {
            return null;
        }

        Vector3 directionVector = destination - origin;
        int direction;
        if(directionVector.x < -0.5f)
        {
            direction = MyDictionary.ENTRANCE_RIGHT;
        }
        else if (directionVector.x > 0.5f)
        {
            direction = MyDictionary.ENTRANCE_LEFT;
        }
        else if (directionVector.z < -0.5f)
        {
            direction = MyDictionary.ENTRANCE_UP;
        }
        else if (directionVector.z > 0.5f)
        {
            direction = MyDictionary.ENTRANCE_DOWN;
        }
        else
        {
            return null;
        }
        
        if (destinationTile.canBeWalkedBy(shape) && destinationTile.canEnterFrom(direction) && originTile.canExitTo(direction))
        {
            return destinationTile.getPlacementHeight();
        }
        return null;
    }

    public void procMoveTriggers(Vector3 pos)
    {
        Vector3 characterPosition = character.getPosition();
        characterPosition.x = (int)characterPosition.x;
        characterPosition.y = (int)characterPosition.y;
        characterPosition.z = (int)characterPosition.z;
        if (levelLayout[(int)characterPosition.x, (int)characterPosition.y, (int)characterPosition.z].getShape() == MyDictionary.Platform.End)
        {
            currentLevel++;
            if (currentLevel > 12)
            {
                currentLevel = 1;
                log("completed every level");
                restartGame();
            }
            else
            {
                StartCoroutine(loadLevel(currentLevel));
            }
            character.morph(MyDictionary.SHAPE_CUBE, true);
            return;
        }
        Vector3 destination;
        if (teleportTriggers.TryGetValue(characterPosition, out destination)){
            destination.y = (int) destination.y + levelLayout[(int) destination.x, (int) destination.y, (int) destination.z].getPlacementHeight();
            StartCoroutine(character.moveASAP(destination));
            bool changed = false;
            foreach(GameObject obj in teleportList)
            {
                Vector3 wouldBePortalPosition = new Vector3(character.getPosition().x, character.getPosition().y, character.getPosition().z);
                wouldBePortalPosition.y = (int) wouldBePortalPosition.y + levelLayout[(int)wouldBePortalPosition.x, (int)wouldBePortalPosition.y, (int)wouldBePortalPosition.z].getPlacementHeight();
                if (obj.transform.position.Equals(destination) || obj.transform.position.Equals(wouldBePortalPosition))
                {
                    ParticleSystem.MainModule settings = obj.GetComponent<ParticleSystem>().main;
                    if (settings.startColor.color.Equals(portalColorInactive))
                    {
                        settings.startColor = portalColors[currentPortalColor];
                        changed = true;
                    }
                }
            }
            if (changed)
            {
                currentPortalColor++;
                log("ACTION? used portal");
            }
            if(currentPortalColor == portalColors.Length)
            {
                currentPortalColor = 0;
            }
        }
        char shape;
        if (morphTriggers.TryGetValue(characterPosition, out shape))
        {
            character.morph(shape, true);
        }

        if (moveTriggers.TryGetValue(characterPosition, out shape))
        {
            levelLayout[(int)characterPosition.x, (int)characterPosition.y, (int)characterPosition.z].load(shape + "000000000");
        }
    }

    private void restartGame()
    {
        if (file != null)
        {
            if (stopwatchIdle.IsRunning)
            {
                stopwatchIdle.Stop();
            }

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopwatchIdle.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            log("total idle time " + elapsedTime);
            stopwatchIdle.Reset();
            file.Close();
        }
        string filename = DateTime.Now.ToString("yyyyMMddHHmmss");
        file = new System.IO.StreamWriter(System.IO.Path.Combine(".", filename + ".txt"));
        log("Game Start");

        character.morph(MyDictionary.SHAPE_CUBE, false);
        StartCoroutine(loadLevel(1));
        cameraOffset = new Vector3(0, 0.86604f, -1.5f);
    }

    public void log(string what)
    {
        file.WriteLine(what + "\r\n");
    }
}
