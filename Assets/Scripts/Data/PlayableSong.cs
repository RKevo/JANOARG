﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayableSong : ScriptableObject
{
    public string SongName;
    public string AltSongName;
    public string SongArtist;
    public string AltSongArtist;
    public string Location = "Prototype";
    public string Genre = "Genreless";
    public AudioClip Clip;

    public Metronome Timing = new Metronome(140);

    [FormerlySerializedAs("Charts")]
    public List<Chart> ChartsOld = new List<Chart>();

    public List<ExternalChartMeta> Charts = new List<ExternalChartMeta>();
}
