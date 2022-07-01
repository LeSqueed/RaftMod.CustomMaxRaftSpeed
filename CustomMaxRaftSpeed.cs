using UnityEngine;

public class CustomMaxRaftSpeed : Mod
{
    private const string ModNamePrefix = "<color=#42a7f5>CustomMax</color><color=#FF0000>RaftSpeed</color>";
    private const float DefaultMaxSpeed = 1.5f;
    private const float DefaultWaterDriftSpeed = 1.5f;
    private const string MaxSpeedArgumentIsOutOfRangeMessage = "maxspeed: Requires a factor between 1 and 20. (x times of the default speed)\n" +
                                                               "e.g. \"maxspeed 1.5\" will increase your speed to 150%, while \"maxspeed 20\" will increase your speed to 2000%)";

    private float m_maxSpeedFactor = DefaultMaxSpeed;
    private float m_waterDriftSpeedFactor = DefaultWaterDriftSpeed;
    public static CustomMaxRaftSpeed instance;

    public void Start()
    {
        instance = this;
        Debug.Log(string.Format("{0} has been loaded!", ModNamePrefix));
    }

    public void OnModUnload()
    {
        var raft = ComponentManager<Raft>.Value;
        if (raft == null)
        {
            return;
        }

        raft.maxSpeed = DefaultMaxSpeed;
        raft.waterDriftSpeed = DefaultWaterDriftSpeed;

        Debug.Log(string.Format("{0} has been unloaded!", ModNamePrefix));
        Destroy(gameObject);
    }

    public void Update()
    {
        if (Raft_Network.InMenuScene ||
            !Raft_Network.IsHost)
        {
            return;
        }

        var raft = ComponentManager<Raft>.Value;
        if (raft == null)
        {
            return;
        }

        raft.maxSpeed = DefaultMaxSpeed * m_maxSpeedFactor;
        raft.waterDriftSpeed = DefaultWaterDriftSpeed * m_waterDriftSpeedFactor;
    }

    private void SetMaximumRaftSpeed(string[] args)
    {
        if (args.Length != 1)
        {
            Debug.Log(MaxSpeedArgumentIsOutOfRangeMessage);
            return;
        }

        float maxSpeedFloat = -1f;
        int maxSpeedInt = -1;
        if ((!float.TryParse(args[0], out maxSpeedFloat) && !int.TryParse(args[0], out maxSpeedInt)))
        {
            Debug.Log(MaxSpeedArgumentIsOutOfRangeMessage);
            return;
        }

        if (maxSpeedFloat < 0f)
        {
            maxSpeedFloat = maxSpeedInt;
        }

        if (maxSpeedFloat < 1f || maxSpeedFloat > 20f)
        {
            Debug.Log(MaxSpeedArgumentIsOutOfRangeMessage);
            return;
        }

        m_maxSpeedFactor = maxSpeedFloat;
        m_waterDriftSpeedFactor = maxSpeedFloat;
        Debug.Log(string.Format("{0}: Max speed factor was set to {1}", ModNamePrefix, maxSpeedFloat));
    }

    [ConsoleCommand(name: "maxspeed", docs: "Modify the maximal speed of your raft")]
    public static void backupCommand(string[] args)
    {
        instance.SetMaximumRaftSpeed(args);
    }
}
