using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;
using System;
using UnityEngine.XR.Interaction.Toolkit;

public class Shooter_generator : MonoBehaviour
{
    private int _miss = 0;
    private int _success = 0;
    private int stimuls_number = 0;
    private System.Random rand = new();
    [HideInInspector] public UnityEvent<int, int> points_counter;
    [SerializeField] private TMP_Text points;
    [SerializeField] private TMP_Text balls;
    [SerializeField] private TMP_Text start_text;
    [SerializeField] private SetupConfig setup_config;
    [SerializeField] private GameObject shooter;
    [SerializeField] private DataPathCreator creator;
    [SerializeField] private GameObject statistic;
    [SerializeField] private GameObject cam;
    [SerializeField] private AudioSource click;
    [SerializeField] private AudioSource start;
    [SerializeField] private AudioSource end;
    [SerializeField] private GameObject arrow;
    [SerializeField] private GameObject throw_surface;


    private bool is_false_stimuls;
    private float velocity;
    private float delta_before_shoot = -1f;
    private float delta_t;
    private Vector3 direction;
    private GameObject surface;
    private bool is_first = true;

    private void Start()
    {
        points_counter = new();
        points_counter.AddListener(ChangePoints);
        if(setup_config.config.experiment_number == 2)
            stimuls_number = (int)setup_config.config.number_of_stimuls * 2;
        else
            stimuls_number = (int)setup_config.config.number_of_stimuls;
        ChangeTextOfStimuls(stimuls_number);
        StartCoroutine(StartExperiment());
    }

    private void ChangeTextOfStimuls(int number)
    {
        balls.text = $"Бросков осталось: {number}";
    }    
    private void ChangePoints(int miss, int success)
    {
        _miss += miss;
        _success += success;
        points.text = $"Отраженные: {_success} \nПропущенные: {_miss}";
        ChangeTextOfStimuls(stimuls_number - _miss - _success);
    }
    private IEnumerator StartExperiment()
    {
        arrow.SetActive(true);
        for (int i = 0;i < 5; i++)
        {
            click.Play();
            start_text.text = $"Эксперимент начнется через {5 - i} секунд";
            yield return new WaitForSeconds(1f);
        }
        arrow.SetActive(false);
        start_text.gameObject.SetActive(false);
        start.Play();
        float max_delta = 0f;
        if(setup_config.config.experiment_number == 1 || setup_config.config.experiment_number == 2)
        {
            float y;
            if (setup_config.config.throw_area_for_experiments.Count == 2)
            {
                y = Camera.main.transform.position.y;
            }
            else y = setup_config.config.throw_area_for_experiments[2];
            surface = Instantiate(throw_surface, new Vector3(0, y, setup_config.config.throw_area_for_experiments[0]), Quaternion.identity);
            surface.transform.localScale = new Vector3(1, 1, 0.1f) * setup_config.config.throw_area_for_experiments[0];
        }
        if(setup_config.config.experiment_number == 2)
        {
            for (int i = 0; i < stimuls_number; i+=2)
            {
                //surface.SetActive(true);
                var new_shooter = SetupExperiment(i, setup_config.config.experiment_number);
                new_shooter.GetComponent<Shooter_controller>().direction = direction;
                new_shooter.SetActive(false);
                if (delta_before_shoot > max_delta) max_delta = delta_before_shoot;
                var first_delta = delta_before_shoot;
                is_first = false;
                var new_shooter2 = SetupExperiment(i, setup_config.config.experiment_number);
                new_shooter2.GetComponent<Shooter_controller>().direction = direction;
                new_shooter2.SetActive(false);
                yield return new WaitForSeconds(delta_t);
                surface.SetActive(false);
                new_shooter.SetActive(true);
                new_shooter2.SetActive(true);
                new_shooter.GetComponent<Shooter_controller>().is_false_stimul = is_false_stimuls;
                new_shooter.GetComponent<Shooter_controller>().velocity = velocity;
                new_shooter.GetComponent<Shooter_controller>().delta_before_shoot = first_delta;
                new_shooter.GetComponent<Shooter_controller>().mass_of_stimul = setup_config.config.mass_of_stimul;
                new_shooter.GetComponent<Shooter_controller>().use_gravity = setup_config.config.use_gravity;
                new_shooter.GetComponent<Shooter_controller>().diameter_of_stimul = setup_config.config.diameter_of_stimul;
                new_shooter2.GetComponent<Shooter_controller>().mass_of_stimul = setup_config.config.mass_of_stimul;
                new_shooter2.GetComponent<Shooter_controller>().use_gravity = setup_config.config.use_gravity;
                new_shooter2.GetComponent<Shooter_controller>().diameter_of_stimul = setup_config.config.diameter_of_stimul;
                if (setup_config.config.experiment_number == 2)
                {
                    var color_string = setup_config.config.stimuls_colors[UnityEngine.Random.Range(0, setup_config.config.stimuls_colors.Count)];
                    if (color_string == "green")
                    {
                        new_shooter.GetComponent<Shooter_controller>().color = Color.green;

                    }
                    if (color_string == "blue")
                    {
                        new_shooter.GetComponent<Shooter_controller>().color = Color.blue;

                    }
                    color_string = setup_config.config.stimuls_colors[UnityEngine.Random.Range(0, setup_config.config.stimuls_colors.Count)];
                    if (color_string == "green")
                    {
                        new_shooter2.GetComponent<Shooter_controller>().color = Color.green;

                    }
                    if (color_string == "blue")
                    {
                        new_shooter2.GetComponent<Shooter_controller>().color = Color.blue;

                    }
                }
                new_shooter.GetComponent<StimulDataWriter>().config_path = setup_config.full_path;
                new_shooter.GetComponent<StimulDataWriter>().data_path = creator.data_path;
                new_shooter.GetComponent<StimulDataWriter>().stimul_number = i;
                new_shooter2.GetComponent<StimulDataWriter>().config_path = setup_config.full_path;
                new_shooter2.GetComponent<StimulDataWriter>().data_path = creator.data_path;
                new_shooter2.GetComponent<Shooter_controller>().is_false_stimul = is_false_stimuls;
                new_shooter2.GetComponent<Shooter_controller>().velocity = velocity;
                new_shooter2.GetComponent<Shooter_controller>().delta_before_shoot = delta_before_shoot;
                new_shooter2.GetComponent<StimulDataWriter>().stimul_number = i+1;
                if (delta_before_shoot > max_delta) max_delta = delta_before_shoot;
                is_first = true;
                yield return new WaitForSeconds(max_delta);

            }
        }
        else
        {
            for (int i = 0; i < stimuls_number; i++)
            {
               //surface.SetActive(true);
                var new_shooter = SetupExperiment(i, setup_config.config.experiment_number);
                //SuperBallSpawnAnimator ballSpawnAnim = new_shooter.GetComponentInChildren<SuperBallSpawnAnimator>();
                //ballSpawnAnim.animTime = delta_before_shoot;
                //new_shooter.transform.GetChild(0).localScale = new_shooter.transform.localScale * setup_config.config.diameter_of_stimul;
                //ballSpawnAnim.Init();
                new_shooter.SetActive(false);
                yield return new WaitForSeconds(delta_t);
                surface.SetActive(false);
                new_shooter.SetActive(true);
                new_shooter.GetComponent<Shooter_controller>().is_false_stimul = is_false_stimuls;
                new_shooter.GetComponent<Shooter_controller>().velocity = velocity;
                new_shooter.GetComponent<Shooter_controller>().delta_before_shoot = delta_before_shoot;
                new_shooter.GetComponent<Shooter_controller>().direction = direction;
                new_shooter.GetComponent<Shooter_controller>().mass_of_stimul = setup_config.config.mass_of_stimul;
                new_shooter.GetComponent<Shooter_controller>().use_gravity = setup_config.config.use_gravity;
                new_shooter.GetComponent<Shooter_controller>().diameter_of_stimul = setup_config.config.diameter_of_stimul;
                new_shooter.GetComponent<Shooter_controller>().surface = surface;
                if (setup_config.config.experiment_number == 1)
                {
                    var color_string = setup_config.config.stimuls_colors[UnityEngine.Random.Range(0, setup_config.config.stimuls_colors.Count)];
                    if (color_string == "green")
                        new_shooter.GetComponent<Shooter_controller>().color = Color.green;
                    if (color_string == "blue")
                        new_shooter.GetComponent<Shooter_controller>().color = Color.blue;

                }
                new_shooter.GetComponent<StimulDataWriter>().config_path = setup_config.full_path;
                new_shooter.GetComponent<StimulDataWriter>().data_path = creator.data_path;
                new_shooter.GetComponent<StimulDataWriter>().stimul_number = i;
                yield return new WaitForSeconds(delta_before_shoot);
                //while(new_shooter.GetComponent<Shooter_controller>().is_catched == false || new_shooter != null)
                //{
                //    continue;
                //}
            }
        }
        yield return new WaitForSeconds(delta_t);
        end.Play();
        statistic.transform.rotation = Quaternion.identity;
        statistic.transform.position = new Vector3(0, cam.transform.position.y, 4);
        start_text.text = $"Процент пойманых мячей: {(float)_success/stimuls_number * 100f} %";
        start_text.gameObject.SetActive(true);
    }

    private GameObject SetupExperiment(int i, int experiment_number)
    {
        Vector3 start_point_in_global = new();
        Vector3 end_point = new();
        Vector3 start_point;
        if (experiment_number == 0)
        {
            if (setup_config.config.throw_area_list.Count == 0)
            {
                start_point = GenerateStartPointWithDistribution();
            }
            else
            {
                start_point = setup_config.config.throw_area_list[UnityEngine.Random.Range(0, setup_config.config.throw_area_list.Count)];
            }
            start_point_in_global = new Vector3(start_point.z * (float)Math.Sin(Math.PI * start_point.x / 180), start_point.y, start_point.z * (float)Math.Cos(Math.PI * start_point.x / 180));
        }
        if (experiment_number == 1)
        {
            System.Random rando = new();
            var result = rando.Next(0, 2) * 2 - 1;
            start_point_in_global = surface.transform.position + result * new Vector3(setup_config.config.throw_area_for_experiments[1], 0,0);
        }
        if (experiment_number == 2)
        { 
            if(is_first)
                start_point_in_global = surface.transform.position + new Vector3(setup_config.config.throw_area_for_experiments[1], 0, 0);
            else
                start_point_in_global = surface.transform.position - new Vector3(setup_config.config.throw_area_for_experiments[1], 0, 0);
        }
        if (setup_config.config.stimuls_velocity_list.Count == 0)
            velocity = GenerateValueWithRandom(setup_config.config.stimuls_velocity[0], setup_config.config.stimuls_velocity[1]);
        else
            velocity = setup_config.config.stimuls_velocity_list[UnityEngine.Random.Range(0, setup_config.config.stimuls_velocity_list.Count)];
        velocity *= 1f + (setup_config.config.value_of_velocity_increase - 1f) * i / stimuls_number;
        if (setup_config.config.is_false_stimuls_exists)
        {
            System.Random random = new();
            if ((float)random.NextDouble() < setup_config.config.false_stimuls_percentage / 100f)
                is_false_stimuls = true;
            else
                is_false_stimuls = false;
        }
        else is_false_stimuls = false;
        if(setup_config.config.delta_before_shoot_list.Count != 0)
        {
            var temp =  setup_config.config.delta_before_shoot_list[UnityEngine.Random.Range(0, setup_config.config.delta_before_shoot_list.Count)];
            if (experiment_number == 2)
            {
                if(delta_before_shoot != -1f)
                {
                    while (temp == delta_before_shoot)
                        temp = setup_config.config.delta_before_shoot_list[UnityEngine.Random.Range(0, setup_config.config.delta_before_shoot_list.Count)];
                }
            }
            delta_before_shoot = temp;
            //Debug.Log(delta_before_shoot);
        }
        else
            delta_before_shoot = GenerateValueWithRandom(setup_config.config.delta_before_shoot[0], setup_config.config.delta_before_shoot[1]);
        delta_t = GenerateValueWithRandom(setup_config.config.delta_t[0], setup_config.config.delta_t[1]);
        if (setup_config.config.target_area_list.Count == 0)
        {
            end_point = GenerateEndPointWithDistribution();
        }
        else
        {
            end_point = setup_config.config.target_area_list[UnityEngine.Random.Range(0, setup_config.config.target_area_list.Count)];
        }
        if (experiment_number == 2)
        {
            //if (is_first)
            //    end_point = setup_config.config.target_area_list[0];
            //else
            //    end_point = setup_config.config.target_area_list[1];
            end_point = setup_config.config.target_area_list[UnityEngine.Random.Range(0, setup_config.config.target_area_list.Count)];

        }
        direction = end_point - start_point_in_global;
        return Instantiate(shooter, start_point_in_global, Quaternion.identity);
    }
    private Vector3 GenerateEndPointWithDistribution()
    {
        var target_area = setup_config.config.target_area;
        float x = (float)(0 + target_area[0] / 3.5 * GenerateStdNormal());
        //Debug.Log("x : " + x);
        if(x > 0)
        {
            x = target_area[0] - x;
        }
        else x = - target_area[0] - x;
        float y = (float)((target_area[2] - target_area[1])/2 + (target_area[2] - target_area[1]) / 7 * GenerateStdNormal());
        if (y > (target_area[2] - target_area[1])/2)
        {
            y = target_area[2] - y + (target_area[2] - target_area[1]) / 2;
        }
        //else y = (target_area[2] - target_area[1]) / 2 - (y - target_area[1]);
        return new Vector3(x, y, -target_area[3]);
    }
    private Vector3 GenerateStartPointWithDistribution()
    {
        var throw_area = setup_config.config.throw_area;
        var throw_area_depth = setup_config.config.throw_area_depth;
        
        float angle =
                     (float)(0 + throw_area[0] / 3.5 * GenerateStdNormal());
        float height =
                     (float)((throw_area[2] - throw_area[1])/2 + (throw_area[2] - throw_area[1]) / 7 * GenerateStdNormal());
        float depth = GenerateValueWithRandom(throw_area_depth[0], throw_area_depth[1]);
        return new Vector3(angle, height, depth);
    }

    private float GenerateValueWithRandom(float a, float b)
    {
        return (float)(rand.NextDouble() * (b - a) + a);
    }

    private double GenerateStdNormal()
    {
        double u1 = 1.0 - rand.NextDouble();
        double u2 = 1.0 - rand.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                     Math.Sin(2.0 * Math.PI * u2);
        return randStdNormal;
    }
}
