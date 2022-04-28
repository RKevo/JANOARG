using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Chart
{
    public string DifficultyName = "Normal";
    public string DifficultyLevel = "6";

    public List<LaneGroup> Groups = new List<LaneGroup>();
    public List<Lane> Lanes = new List<Lane>();

    public Color BackgroundColor = Color.black;
    public Material LaneMaterial;
    public Material HitMaterial;

    public Chart() {
        
    }
}

[System.Serializable]
public class LaneGroup : IStoryboardable 
{
    public string Name;
    public Vector3 Position;
    public float Rotation;

    public new static TimestampType[] TimestampTypes = {
        new TimestampType {
            ID = "Position_X",
            Name = "Position X",
            Get = (x) => ((LaneGroup)x).Position.x,
            Set = (x, a) => { ((LaneGroup)x).Position.x = a; },
        },
        new TimestampType {
            ID = "Position_Y",
            Name = "Position Y",
            Get = (x) => ((LaneGroup)x).Position.y,
            Set = (x, a) => { ((LaneGroup)x).Position.y = a; },
        },
        new TimestampType {
            ID = "Position_Z",
            Name = "Position Z",
            Get = (x) => ((LaneGroup)x).Position.z,
            Set = (x, a) => { ((LaneGroup)x).Position.z = a; },
        },
        new TimestampType {
            ID = "Rotation",
            Name = "Rotation",
            Get = (x) => ((LaneGroup)x).Rotation,
            Set = (x, a) => { ((LaneGroup)x).Rotation = a; },
        },
    };
}

[System.Serializable]
public class Lane : IStoryboardable 
{
    public List<HitObject> Objects = new List<HitObject>();
    public List<LaneStep> LaneSteps = new List<LaneStep>();
    public bool ExpandToInfinity = true;
    public float ZOffset;
    public string Group;

    public LaneStep GetLaneStep(float time, float laneTime, Metronome timing) 
    {
        float offset = 0;
        float timeT = timing.ToSeconds(time);
        float laneTimeT = timing.ToSeconds(laneTime);
        float curtime = laneTimeT;
        for (int a = 0; a < LaneSteps.Count; a++) 
        {
            LaneStep step = LaneSteps[a];

            float t = timing.ToSeconds(step.Offset);
            offset += step.Speed * (Mathf.Max(t, laneTimeT) - curtime);
            curtime = Mathf.Max(t, laneTimeT);

            if (step.Offset == time) return new LaneStep 
            {
                StartPos = step.StartPos,
                EndPos = step.EndPos,
                Offset = laneTime < time ? offset : float.NaN,
            };
            else if (step.Offset > time) 
            {
                if (a == 0) return new LaneStep 
                {
                    StartPos = step.StartPos,
                    EndPos = step.EndPos,
                    Offset = laneTime < time ? offset : float.NaN,
                };

                LaneStep prev = LaneSteps[a - 1];
                float p = (time - prev.Offset) / (step.Offset - prev.Offset);

                if (step.StartEaseX == "Linear" && step.StartEaseY == "Linear" &&
                    step.EndEaseX == "Linear" && step.EndEaseY == "Linear")
                {
                    return new LaneStep 
                    {
                        StartPos = Vector2.Lerp(prev.StartPos, step.StartPos, p),
                        EndPos = Vector2.Lerp(prev.EndPos, step.EndPos, p),
                        Offset = laneTime < time ? offset + (timeT - t) * step.Speed : float.NaN,
                    };
                }
                else 
                {
                    
                    return new LaneStep 
                    {
                        StartPos = new Vector2(Mathf.Lerp(prev.StartPos.x, step.StartPos.x, Ease.Get(p, step.StartEaseX, step.StartEaseXMode)),
                            Mathf.Lerp(prev.StartPos.y, step.StartPos.y, Ease.Get(p, step.StartEaseY, step.StartEaseYMode))),
                        EndPos = new Vector2(Mathf.Lerp(prev.EndPos.x, step.EndPos.x, Ease.Get(p, step.EndEaseX, step.EndEaseXMode)),
                            Mathf.Lerp(prev.EndPos.y, step.EndPos.y, Ease.Get(p, step.EndEaseY, step.EndEaseYMode))),
                        Offset = laneTime < time ? offset + (timeT - t) * step.Speed : float.NaN,
                    };
                }
            }
            
        }
        {
            float t = timing.ToSeconds(LaneSteps[LaneSteps.Count - 1].Offset);
            return new LaneStep 
            {
                StartPos = LaneSteps[LaneSteps.Count - 1].StartPos,
                EndPos = LaneSteps[LaneSteps.Count - 1].EndPos,
                Offset = laneTime < time ? offset + (timeT - t) * LaneSteps[LaneSteps.Count - 1].Speed : float.NaN,
            };
        }
    }

    public new static TimestampType[] TimestampTypes = {
        new TimestampType {
            ID = "ZOffset",
            Name = "Z Offset",
            Get = (x) => ((Lane)x).ZOffset,
            Set = (x, a) => { ((Lane)x).ZOffset = a; },
        },
    };
}

[System.Serializable]
public class LaneStep : IStoryboardable 
{
    public float Offset;
    public Vector2 StartPos;
    public string StartEaseX = "Linear";
    public EaseMode StartEaseXMode;
    public string StartEaseY = "Linear";
    public EaseMode StartEaseYMode;
    public Vector2 EndPos;
    public string EndEaseX = "Linear";
    public EaseMode EndEaseXMode;
    public string EndEaseY = "Linear";
    public EaseMode EndEaseYMode;
    public float Speed = 1;

    public new static TimestampType[] TimestampTypes = {
        new TimestampType {
            ID = "StartPos_X",
            Name = "Start Position X",
            Get = (x) => ((LaneStep)x).StartPos.x,
            Set = (x, a) => { ((LaneStep)x).StartPos.x = a; },
        },
        new TimestampType {
            ID = "StartPos_Y",
            Name = "Start Position Y",
            Get = (x) => ((LaneStep)x).StartPos.y,
            Set = (x, a) => { ((LaneStep)x).StartPos.y = a; },
        },
        new TimestampType {
            ID = "EndPos_X",
            Name = "End Position X",
            Get = (x) => ((LaneStep)x).EndPos.x,
            Set = (x, a) => { ((LaneStep)x).EndPos.x = a; },
        },
        new TimestampType {
            ID = "EndPos_Y",
            Name = "End Position Y",
            Get = (x) => ((LaneStep)x).EndPos.y,
            Set = (x, a) => { ((LaneStep)x).EndPos.y = a; },
        },
        new TimestampType {
            ID = "Speed",
            Name = "Speed",
            Get = (x) => ((LaneStep)x).Speed,
            Set = (x, a) => { ((LaneStep)x).Speed = a; },
        },
    };
}

[System.Serializable]
public class HitObject : IStoryboardable 
{
    public float Offset = 0;
    public float Position;
    public float Length;
    
    public enum HitType
    {
        Normal,
        Catch,
        Flick,
    }

    public List<RailTimestamp> Rail = new List<RailTimestamp>();

    public new static TimestampType[] TimestampTypes = {
        new TimestampType {
            ID = "Position",
            Name = "Position",
            Get = (x) => ((HitObject)x).Position,
            Set = (x, a) => { ((HitObject)x).Position = a; },
        },
        new TimestampType {
            ID = "Length",
            Name = "Length",
            Get = (x) => ((HitObject)x).Length,
            Set = (x, a) => { ((HitObject)x).Length = a; },
        },
    };
}

[System.Serializable]
public class RailTimestamp : IStoryboardable 
{
    public int Offset = 0;
    public float Position;
    public Vector3 Velocity;
    

    public new static TimestampType[] TimestampTypes = {
        new TimestampType {
            ID = "Position",
            Name = "Position",
            Get = (x) => ((RailTimestamp)x).Position,
            Set = (x, a) => { ((RailTimestamp)x).Position = a; },
        },
        new TimestampType {
            ID = "Velocity_X",
            Name = "Velocity X",
            Get = (x) => ((RailTimestamp)x).Velocity.x,
            Set = (x, a) => { ((RailTimestamp)x).Velocity.x = a; },
        },
        new TimestampType {
            ID = "Velocity_Y",
            Name = "Velocity Y",
            Get = (x) => ((RailTimestamp)x).Velocity.y,
            Set = (x, a) => { ((RailTimestamp)x).Velocity.y = a; },
        },
        new TimestampType {
            ID = "Velocity_Z",
            Name = "Velocity Z",
            Get = (x) => ((RailTimestamp)x).Velocity.z,
            Set = (x, a) => { ((RailTimestamp)x).Velocity.z = a; },
        },
    };
}

public enum CoordinateMode 
{
    Local,
    Group,
    Global,
}
