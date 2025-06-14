using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Timeline/TrackInitDataListAsset")]
public class SceneSolution : ScriptableObject
{
    public List<TrackInitData> data;
}