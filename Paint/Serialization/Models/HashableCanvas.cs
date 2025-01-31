﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Paint.Serialization.Models;

internal class HashableCanvas {
    [JsonProperty("canvas_size")]
    public required Tuple<int, int> CanvasSize { get; set; }

    [JsonProperty("figures")]
    public required List<HashableFigure> Figures { get; set; }
}
