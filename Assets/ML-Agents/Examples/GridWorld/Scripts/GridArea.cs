using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.MLAgents;


public class GridArea : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> actorObjs;
    [HideInInspector]
    public int[] players;

    public GameObject trueAgent;
    public int numObstacles;

    Camera m_AgentCam;

    public GameObject goalPref;
    public GameObject pitPref;
    GameObject[] m_Objects;

    GameObject m_Plane;
    GameObject m_Sn;
    GameObject m_Ss;
    GameObject m_Se;
    GameObject m_Sw;

    Vector3 m_InitialPosition;

    EnvironmentParameters m_ResetParams;

    public void Start()
    {
        m_ResetParams = Academy.Instance.EnvironmentParameters;

        m_Objects = new[] { goalPref, pitPref };

        m_AgentCam = transform.Find("agentCam").GetComponent<Camera>();

        actorObjs = new List<GameObject>();

        var sceneTransform = transform.Find("scene");

        m_Plane = sceneTransform.Find("Plane").gameObject;
        m_Sn = sceneTransform.Find("sN").gameObject;
        m_Ss = sceneTransform.Find("sS").gameObject;
        m_Sw = sceneTransform.Find("sW").gameObject;
        m_Se = sceneTransform.Find("sE").gameObject;
        m_InitialPosition = transform.position;
    }

    void SetEnvironment()
    {
        transform.position = m_InitialPosition * (m_ResetParams.GetWithDefault("gridSize", 5f) + 1);
        var playersList = new List<int>();

        //for (var i = 0; i < (int)m_ResetParams.GetWithDefault("numObstacles", 1); i++)
        for (var i = 0; i < numObstacles+1; i++)
        {
            playersList.Add(1);
        }

        for (var i = 0; i < (int)m_ResetParams.GetWithDefault("numGoals", 1f); i++)
        {
            playersList.Add(0);
        }
        players = playersList.ToArray();

        var gridSize = (int)m_ResetParams.GetWithDefault("gridSize", 5f);
        m_Plane.transform.localScale = new Vector3(gridSize / 10.0f, 1f, gridSize / 10.0f);
        m_Plane.transform.localPosition = new Vector3((gridSize - 1) / 2f, -0.5f, (gridSize - 1) / 2f);
        m_Sn.transform.localScale = new Vector3(1, 1, gridSize + 2);
        m_Ss.transform.localScale = new Vector3(1, 1, gridSize + 2);
        m_Sn.transform.localPosition = new Vector3((gridSize - 1) / 2f, 0.0f, gridSize);
        m_Ss.transform.localPosition = new Vector3((gridSize - 1) / 2f, 0.0f, -1);
        m_Se.transform.localScale = new Vector3(1, 1, gridSize + 2);
        m_Sw.transform.localScale = new Vector3(1, 1, gridSize + 2);
        m_Se.transform.localPosition = new Vector3(gridSize, 0.0f, (gridSize - 1) / 2f);
        m_Sw.transform.localPosition = new Vector3(-1, 0.0f, (gridSize - 1) / 2f);

        m_AgentCam.orthographicSize = (gridSize) / 2f;
        m_AgentCam.transform.localPosition = new Vector3((gridSize - 1) / 2f, gridSize + 1f, (gridSize - 1) / 2f);
    }

    public void AreaReset()
    {
        var gridSize = (int)m_ResetParams.GetWithDefault("gridSize", 5f);
        foreach (var actor in actorObjs)
        {
            DestroyImmediate(actor);
        }
        SetEnvironment();

        actorObjs.Clear();

        var numbers = new HashSet<int>();
        while (numbers.Count < players.Length + 1)
        {
            numbers.Add(Random.Range(0, gridSize * gridSize));
        }
        var numbersA = numbers.ToArray();

        for (var i = 0; i < players.Length; i++)
        {
            var x = (numbersA[i]) / gridSize;
            var y = (numbersA[i]) % gridSize;
            var actorObj = Instantiate(m_Objects[players[i]], transform);
            actorObj.transform.localPosition = new Vector3(x, -0.25f, y);
            actorObjs.Add(actorObj);
        }

        var xA = (numbersA[players.Length]) / gridSize;
        var yA = (numbersA[players.Length]) % gridSize;
        trueAgent.transform.localPosition = new Vector3(xA, -0.25f, yA);
    }
}
