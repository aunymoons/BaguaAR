using System;
using System.Collections.Generic;

[Serializable]
public class RequestDataImg2Img
{
    public string prompt;
    public string negative_prompt;
    public int steps;
    public int cfg_scale;
    public string sampler_name;
    public int width;
    public int height;
    public bool restore_faces;
    public float denoising_strength;
    public List<string> init_images;
    public string mask;
    public int inpainting_fill;
    public bool inpaint_full_res;
    public string sampler_index;
    public bool save_images;
    public string script_name;
    // New fields for controlnet_units
    public AlwaysonScripts alwayson_scripts;
}
