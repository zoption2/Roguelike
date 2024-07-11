using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IReward
{
    public RewardType Type { get; set; }
    public int Count { get; set; }
}
public class Reward : IReward
{
    public RewardType Type { get; set; }
    public int Count { get; set; }
    public Reward(RewardType type, int count)
    {
        Type = type;
        Count = count;
    }
}
