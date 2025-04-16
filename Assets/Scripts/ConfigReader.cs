using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Globalization;

[CreateAssetMenu]
public class ConfigReader : ScriptableObject
{
    public Vector3 throw_area { get; private set; }
    public Vector2 throw_area_depth { get; private set; }

    public List<Vector3> throw_area_list { get; private set; }
    public float number_of_stimuls { get; private set; }
    public float value_of_velocity_increase { get; private set; }
    public int experiment_number { get; private set; }
    public float mass_of_stimul { get; private set; }
    public Vector2 delta_t { get; private set; }
    public Vector2 delta_before_shoot { get; private set; }
    public List<float> delta_before_shoot_list { get; private set; }
    public Vector2 stimuls_velocity { get; private set; }
    public List<float> stimuls_velocity_list { get; private set; }
    public List<float> throw_area_for_experiments { get; private set; }
    public bool is_false_stimuls_exists { get; private set; }
    public Vector4 target_area { get; private set; }
    public List<Vector3> target_area_list { get; private set; }

    public bool use_gravity { get; private set; }

    private string delimeter = " ";
    public float false_stimuls_percentage { get; private set; }
    public float diameter_of_stimul { get; private set; }
    public List<string> stimuls_colors { get; private set; }
    public void ReadConfig(string name)
    {
        stimuls_velocity_list = new();
        throw_area_list = new();
        target_area_list = new();
        throw_area_for_experiments = new();
        stimuls_colors = new();
        delta_before_shoot_list = new();
        using (StreamReader sw  = File.OpenText(name))
        {
            string result = sw.ReadToEnd();
            string[] lines = result.Split("\n");
            CultureInfo culture = CultureInfo.InvariantCulture;
            for(int i=0; i<lines.Length; i++)
            {
                if(lines[i].StartsWith("#"))
                {
                    Debug.Log(lines[i]);
                    Debug.Log(lines[i + 1]);
                    switch (lines[i].Split("#")[1])
                    {
                        case "throw_area":
                            var list = lines[i + 1].Split(delimeter);
                            if (list[0] == "$$")
                            {
                                foreach (var elem in list[1..])
                                {
                                    var vector = elem.Split(",");
                                    //Debug.Log(vector[0][1..]);
                                    //Debug.Log(vector[1]);
                                    //Debug.Log(vector[2][..^1]);
                                    if(vector[2][^1] != ']')
                                    {
                                        //Debug.Log("Here1 " + vector[2][^1]);
                                        throw_area_list.Add(new Vector3(float.Parse(vector[0][1..], culture), float.Parse(vector[1], culture), float.Parse(vector[2][..^2], culture)));
                                    }
                                    else
                                    {
                                        //Debug.Log("Here2 " + vector[2][^1]);
                                        throw_area_list.Add(new Vector3(float.Parse(vector[0][1..], culture), float.Parse(vector[1], culture), float.Parse(vector[2][..^1], culture)));
                                    }

                                }
                            }
                            else
                            {
                                throw_area = new Vector3(float.Parse(lines[i + 1].Split(delimeter)[0], culture), float.Parse(lines[i + 1].Split(delimeter)[1], culture), float.Parse(lines[i + 1].Split(delimeter)[2], culture));
                                throw_area_depth = new Vector2(float.Parse(lines[i + 1].Split(delimeter)[3], culture), float.Parse(lines[i + 1].Split(delimeter)[4], culture));
                            }
                            break;
                        case "target_area_list":
                            var list2 = lines[i + 1].Split(delimeter);
                            if (list2[0] != " ")
                            {
                                foreach (var elem in list2)
                                {
                                    var vector = elem.Split(",");
                                    //Debug.Log(vector[0][1..]);
                                    //Debug.Log(vector[1]);
                                    //Debug.Log(vector[2][..^1]);
                                    if (vector[2][^1] != ']')
                                    {
                                        //Debug.Log("Here1 " + vector[2][^1]);
                                        target_area_list.Add(new Vector3(float.Parse(vector[0][1..], culture), float.Parse(vector[1], culture), float.Parse(vector[2][..^2], culture)));
                                    }
                                    else
                                    {
                                        //Debug.Log("Here2 " + vector[2][^1]);
                                        target_area_list.Add(new Vector3(float.Parse(vector[0][1..], culture), float.Parse(vector[1], culture), float.Parse(vector[2][..^1], culture)));
                                    }

                                }
                            }
                            break;
                        case "number_of_stimuls":
                            number_of_stimuls = float.Parse(lines[i + 1], culture);
                            break;
                        case "experiment_number":
                            experiment_number = int.Parse(lines[i + 1], culture);
                            break;
                        case "diameter_of_stimul":
                            diameter_of_stimul = float.Parse(lines[i + 1], culture);
                            break;
                        case "value_of_velocity_increase":
                            value_of_velocity_increase = float.Parse(lines[i + 1], culture);
                            break;
                        case "false_stimuls_percentage":
                            false_stimuls_percentage = float.Parse(lines[i + 1], culture);
                            break;
                        case "mass_of_stimul":
                            mass_of_stimul = float.Parse(lines[i + 1], culture);
                            break;
                        case "delta_t":
                            delta_t = new Vector2(float.Parse(lines[i + 1].Split(delimeter)[0], culture), float.Parse(lines[i + 1].Split(delimeter)[1], culture));
                            break;
                        case "delta_before_shoot":
                            var list5 = lines[i + 1].Split(delimeter);
                            if (list5[0] == "$$")
                            {
                                foreach (var elem in list5[1..])
                                {
                                    delta_before_shoot_list.Add(float.Parse(elem, culture));
                                }
                            }
                            else
                                  delta_before_shoot = new Vector2(float.Parse(lines[i + 1].Split(delimeter)[0], culture), float.Parse(lines[i + 1].Split(delimeter)[1], culture));
                            break;
                        case "throw_area_for_experiments":
                            var list3 = lines[i + 1].Split(delimeter);
                            foreach (var elem in list3)
                            {
                                throw_area_for_experiments.Add(float.Parse(elem, culture));
                            }
                            break;
                        case "stimuls_velocity":
                            var list1 = lines[i + 1].Split(delimeter);
                            if (list1[0] == "$$")
                            {
                                foreach(var elem in list1[1..])
                                {
                                    stimuls_velocity_list.Add(float.Parse(elem, culture));
                                }
                            }
                            else
                                stimuls_velocity = new Vector2(float.Parse(lines[i + 1].Split(delimeter)[0], culture), float.Parse(lines[i + 1].Split(delimeter)[1], culture));
                            break;
                        case "is_false_stimuls_exists":
                            is_false_stimuls_exists = bool.Parse(lines[i + 1]);
                            break;
                        case "target_area":
                            target_area = new Vector4(float.Parse(lines[i + 1].Split(delimeter)[0], culture), float.Parse(lines[i + 1].Split(delimeter)[1], culture), float.Parse(lines[i + 1].Split(delimeter)[2], culture), float.Parse(lines[i + 1].Split(delimeter)[3], culture));
                            break;
                        case "use_gravity":
                            use_gravity = bool.Parse(lines[i + 1]);
                            break;
                        case "stimuls_colors":
                            var list4 = lines[i + 1].Split(delimeter);
                            foreach (var elem in list4)
                            { 
                                stimuls_colors.Add(elem);
                            }
                            break;
                    }
                    
                }
            }
        }
    }
}
