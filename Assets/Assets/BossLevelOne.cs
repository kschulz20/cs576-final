using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

enum PlatformType
{
    STABLE = 0,
    UNSTABLE = 1
}

public class BossLevelOne : MonoBehaviour
{
    int rows;
    int cols;
    int row_difference;
    int function_calls;
    Color32 green_for_text;
    public Canvas platform_canvas;
    //Internal variable for the number of seconds the player has to memorize the platforms
    private int seconds_for_platform_memorization;
    public TextMeshProUGUI timer_text;
    //Timer for changing platform canvas' displayed timer
    private float timer;
    //To keep track if the player is memorizing the platforms - used by Turret.cs to know when to start firing the turret
    public bool cutscene_being_shown;
    private bool music_already_played;
    public AudioSource audio_source;
    public AudioSource boss_audio_source;
    public AudioSource lava_audio_source;
    public AudioClip apple_shoot_sfx;
    public AudioClip apple_hit_sfx;
    public AudioClip platform_fall_sfx;
    public AudioClip teleport_sfx;
    public AudioClip turret_death_sfx;
    public AudioClip lava_sizzle_sfx;
    public RenderPipelineAsset urp;
    public int player_lives = 5;
    public Text num_lives_text;

    // Start is called before the first frame update
    void Start()
    {
        GraphicsSettings.renderPipelineAsset = urp;
        //CSP stuff
        function_calls = 100000;
        //Dimensions of grid
        rows = 12;
        cols = 6;
        //For CSP calculation
        row_difference = 10;
        green_for_text = new Color32(28, 214, 21, 255);
        //Represents the platforms
        List<PlatformType>[,] grid = new List<PlatformType>[rows, cols];
        //For platforms with no assignment yet - for CSP
        List<int[]> unassigned = new List<int[]>();

        //Timer stuff
        seconds_for_platform_memorization = 1;
        //Set the timer on the platform canvas to have the amount of seconds set here
        timer_text.text = string.Format("{0}", seconds_for_platform_memorization);
        timer = seconds_for_platform_memorization;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Internal error: could not find the player object - did you remove its 'AJ' tag?");
            return;
        }
        AJ player_script = player.GetComponent<AJ>();
        player_script.walking_velocity = 9.0f;
        player_script.jump_height = 90.0f;

        cutscene_being_shown = true;
        music_already_played = false;

        // bool success = false;
        // while(!success)
        // {   
        //     //Randomly fill grid with assignments of either 0 or 1 - give 4/5 chance to 1 coming first
        //     for(int i = 0; i < rows; i++)
        //     {
        //         for(int j = 0; j < cols; j++)
        //         {
        //             if (Random.Range(1, 4) == 1)
        //                 grid[i, j] = new List<PlatformType> { PlatformType.STABLE, PlatformType.UNSTABLE };
        //             else
        //                 grid[i, j] = new List<PlatformType> { PlatformType.UNSTABLE, PlatformType.STABLE };
        //             unassigned.Add(new int[] { i, j });
        //             // Console.WriteLine("[" + i + ", " + j + "]: " + grid[i, j][0] + "," + grid[i, j][1]);
        //         }
        //     }

        //     success = BackTrackingSearch(grid, unassigned);
        //     if (!success)
        //     {
        //         Debug.Log("Trying to generate solution again...");
        //         unassigned.Clear();
        //         grid = new List<PlatformType>[rows, cols];
        //         function_calls = 0;
        //     }
        // }

        //MakePathInPlatforms(grid);
        InitializeGrid(grid);
        ScuffedMakePathInPlatforms(grid);
        DisableScriptsOnStablePlatforms(grid);

        // for(int i = 0; i < rows; i++)
        // {
        //     string print = "";
        //     print += i + ":  ";
        //     for(int j = 0; j < cols; j++)
        //     {
        //         if (grid[i,j][0] == PlatformType.STABLE)
        //             print += "S  ";
        //         else
        //             print += "U  ";
        //     }
        //     Debug.Log(print);
        // }

        lava_audio_source.Play();

        StartCoroutine(WaitXSeconds(seconds_for_platform_memorization));
    }

    IEnumerator WaitXSeconds(int time_to_wait)
    {
        yield return new WaitForSeconds(time_to_wait);
        platform_canvas.gameObject.SetActive(false);
    }

    void DisableScriptsOnStablePlatforms(List<PlatformType>[,] grid)
    {
        for(int i = 0; i < rows; i++)
        {
            for(int j = 0; j < cols; j++)
            {
                if (grid[i, j][0] == PlatformType.STABLE)
                {
                    int platform_number = grid_to_int(i, j);
                    GameObject platform = GameObject.Find("Platform " + "(" + platform_number + ")");
                    if (platform == null)
                        Debug.Log("Error: could not find Platform game object");
                    platform.GetComponent<PlatformFall>().enabled = false;
                    platform.GetComponent<BoxCollider>().enabled = false;

                    TextMeshProUGUI platform_as_text = GameObject.Find("PlatformText " + "(" + platform_number + ")").GetComponent<TextMeshProUGUI>();
                    if (platform_as_text == null)
                        Debug.Log("Error: could not find game object PlatformText");
                    platform_as_text.text = "1";
                    platform_as_text.color = green_for_text;
                }
            }
        }
    }

    void InitializeGrid(List<PlatformType>[,] grid)
    {
        for(int i = 0; i < rows; i++)
        {
            for(int j = 0; j < cols; j++)
            {
                grid[i, j] = new List<PlatformType> { PlatformType.UNSTABLE };
            }
        }
    }

    void ScuffedMakePathInPlatforms(List<PlatformType>[,] grid)
    {
        int start_platform_col = Random.Range(0, cols - 1);
        grid[0, start_platform_col][0] = PlatformType.STABLE;

        for(int i = 1; i < rows; i++)
        {
            int col_to_be_stable = -1;
            bool stop_searching = false;
            while(!stop_searching)
            {
                col_to_be_stable = Random.Range(0, cols - 1);
                if (col_to_be_stable - 1 > -1 && col_to_be_stable + 1 < cols)
                {
                    if (grid[i - 1, col_to_be_stable - 1][0] == PlatformType.STABLE ||
                        grid[i - 1, col_to_be_stable + 1][0] == PlatformType.STABLE)
                    {
                        stop_searching = true;
                    }
                }
                else if (grid[i - 1, col_to_be_stable][0] == PlatformType.STABLE)
                {
                    stop_searching = true;
                }
            }
            grid[i, col_to_be_stable][0] = PlatformType.STABLE;
        }
    }

    void MakePathInPlatforms(List<PlatformType>[,] grid)
    {
        //Randomly select starting platform
        int start_platform_int = find_stable_platform_in_row(0, grid);
        //Randomly select goal platform
        int goal_platform_int = find_stable_platform_in_row(rows - 1, grid);
        //Get the coordinate representation of the start platform on the grid
        int[] start_platform_coords = int_to_grid(start_platform_int);
        int start_platform_row = start_platform_coords[0];
        int start_platform_col = start_platform_coords[1];

        //Create queue of numbers [0, rows*cols]
        //Create distance array of size rows*cols, initialize all values to infinity
        //Set distance[player position] = 0
        int[] distances = new int[rows*cols];
        for(int i = 0; i < rows*cols; i++) {
            distances[i] = int.MaxValue;
        }
        //distance[starting platform position] = 0
        distances[start_platform_int] = 0;

        int[] parents = new int[rows*cols];
        for(int i = 0; i < rows*cols; i++) {
            parents[i] = -1;
        }

        //Representation of priority queue where values are int arrays int[] where int[][0] = priority/distance, int[][1] = value/vertex
        List<int[]> priority_list = new List<int[]>(rows*cols);
        for(int i = 0; i < rows*cols; i++) {
            priority_list.Add(new int[] { distances[i], i });
        }

        //Up right, up, up left, right, and left
        int[][] directions_array = new int[][] { 
                                                new int[] { 1, 1 },
                                                new int[] { 1, 0 },
                                                new int[] { 1, -1 },
                                                new int[] { 0, 1 },
                                                new int[] { 0, -1 }
                                            };

        while(priority_list.Count != 0)
        {
            //Get lowest priority (distance) element from int[] values of priority_list
            int lowest_priority = priority_list.Min(distance_and_val => distance_and_val[0]);
            //Get actual value of the first lowest priority (distance) element from priority_list
            int u = priority_list.FirstOrDefault(distance_and_val => distance_and_val[0] == lowest_priority)[1];
            
            //To remove u from the priority_list, remove at index u (the value of u is also its index in the list based on the way we constructed it)
            int index_of_u = priority_list.FindIndex(distance_and_val => distance_and_val[1] == u);
            priority_list.RemoveAt(index_of_u);

            //Convert u to grid coordinates
            int[] u_coords = int_to_grid(u);
            int u_row = u_coords[0];
            int u_col = u_coords[1];

            //Skip the last row of the platforms because there are no platforms ahead of them
            if (u_row == (rows - 1)) {
                continue;
            }

            for (int i = 0; i < 5; i++)
            {
                int[] directions = directions_array[i];
                if ((u_col + directions[1] > -1) && (u_col + directions[1] < cols))
                {
                    int v_row = u_row + directions[0];
                    int v_col = u_col + directions[1];
                    int v = grid_to_int(v_row, v_col);
                    //Check if the neighbor v is in the priority_list
                    if (priority_list.Any(distance_and_val => distance_and_val[1] == v))
                    {
                        //If the neighbor is an unstable platform, moving into it should be a non-zero cost
                        int edge_weight = 0;
                        if (grid[v_row, v_col][0] == PlatformType.UNSTABLE)
                        {
                            edge_weight = 1000;
                        }

                        int d = distances[u] + edge_weight;
                        if (d < distances[v]) {
                            distances[v] = d;
                            parents[v] = u;
                        }

                        if (v == goal_platform_int) {
                            priority_list.Clear();
                            break;
                        }
                        
                        //Update priority of v
                        int index_of_v = priority_list.FindIndex(distance_and_val => distance_and_val[1] == v);
                        priority_list[index_of_v][0] = distances[v];
                    }
                }
            }
        }

        //Get rid of unstable platforms in shortest path between player & goal platform using the parents array constructed from Dijkstra's
        delete_walls(start_platform_int, goal_platform_int, parents, grid);
    }

    //Returns the integer representation of a stable platform in the given row of the grid
    int find_stable_platform_in_row(int row_index, List<PlatformType>[,] grid)
    {
        int platform_num;
        while(true)
        {
            int guess_index = Random.Range(0, cols - 1);
            if (grid[row_index, guess_index][0] == PlatformType.STABLE)
            {
                platform_num = grid_to_int(row_index, guess_index);
                break;
            }
        }
        return platform_num;
    }

    //Helper method to convert integers to grid positions
    int grid_to_int(int grid_pos_x, int grid_pos_y)
    {
        return (grid_pos_x * cols) + (grid_pos_y);
    }

    //Helper method to convert grid positions to integers
    int[] int_to_grid(int num)
    {
        int quotient = num / cols;
        int remainder = num % cols;
        return new int[] { quotient, remainder };
    }

    //Helper method to delete any walls in the path from u to v
    void delete_walls(int u, int v, int[] parents, List<PlatformType>[,] grid)
    {
        //Debug.Log("u: " + u + ", v: " + v);
        if (u < 0 || v < 0)
        {
            return;
        }

        if (u != v)
        {
            delete_walls(u, parents[v], parents, grid);
            int[] v_coords = int_to_grid(v);
            if (grid[v_coords[0], v_coords[1]][0] == PlatformType.UNSTABLE)
            {
                grid[v_coords[0], v_coords[1]][0] = PlatformType.STABLE;
            }
        }
        else {
            return;
        }
    }

    bool BackTrackingSearch(List<PlatformType>[,] grid, List<int[]> unassigned)
    {
        if (function_calls++ > 100000)       
            return false;

        //Base case
        if (unassigned.Count == 0)
            return true;

        //Randomly select unassigned platform
        int[] unassigned_platform = unassigned[Random.Range(0, unassigned.Count - 1)];
        int row = unassigned_platform[0];
        int col = unassigned_platform[1];
        foreach (PlatformType attempt_platform in grid[row, col]) {
            if (CheckConsistency(grid, unassigned_platform, attempt_platform))
            {
                //Remove platform from unassigned & update grid to have that assignment
                unassigned.Remove(unassigned_platform);
                List<PlatformType> old_assignment = new List<PlatformType>();
                old_assignment.AddRange(grid[row, col]);
                grid[row, col] = new List<PlatformType> { attempt_platform };

                //Call function recursively with updated unassigned & grid
                bool result = BackTrackingSearch(grid, unassigned);

                //If the result is true, return the result of the recursive call
                if (result)
                    return result;
                //Otherwise, add that platform back to unassigned and revert grid to its old assignment
                unassigned.Add(unassigned_platform);
                grid[row, col] = new List<PlatformType>();
                grid[row, col].AddRange(old_assignment);
            }
        }
        return false;
    }

    //Check if attempted assignment is consistent with the constraints or not
    bool CheckConsistency(List<PlatformType>[,] grid, int[] cell_pos, PlatformType platform)
    {
        int row = cell_pos[0];
        int col = cell_pos[1];

        List<PlatformType> old_assignment = new List<PlatformType>();
        old_assignment.AddRange(grid[row, col]);
        grid[row, col] = new List<PlatformType> { platform };

        //Check consistency
        bool are_we_consistent =  ExactlyOneStablePlatformPerRow(grid) && ConsecutiveStablePlatformsDontExceedThree(grid)
                        && StablePlatformsAlwaysNearAnother(grid);

        grid[row, col] = new List<PlatformType>();
        grid[row, col].AddRange(old_assignment);
        return are_we_consistent;
    }

    //Returns true if each row of grid has exactly 1 stable platform in each of its rows
    bool ExactlyOneStablePlatformPerRow(List<PlatformType>[,] grid)
    {
        for(int i = 0; i < rows; i++)
        {
            int num_stable_platforms = 0;
            for(int j = 0; j < cols; j++)
            {
                if (grid[i, j][0] == PlatformType.STABLE)
                    num_stable_platforms++;
            }
            if (num_stable_platforms > 2 || num_stable_platforms == 0)
                return false;
        }
        return true;
    }

    bool ConsecutiveStablePlatformsDontExceedThree(List<PlatformType>[,] grid)
    {
        //Only check up to rows-2 because any row past rows-2 couldn't have 3 consecutive stable platforms
        for(int i = 0; i < rows - 2; i++)
        {
            for(int j = 0; j < cols; j++)
            {
                //If this platform is stable
                if (grid[i, j][0] == PlatformType.STABLE)
                {
                    int num_consec_stable = 1;
                    for(int k = 1; k < 3; k++)
                    {
                        if (grid[i + k, j][0] == PlatformType.STABLE)
                            num_consec_stable++;
                    }
                    if (num_consec_stable > 3)
                        return false;
                }
            }
        }
        return true;
    }

    bool StablePlatformsAlwaysNearAnother(List<PlatformType>[,] grid)
    {
        int num_platforms_with_constraint = 0;
        for(int i = 0; i < rows - 1; i++)
        {
            for(int j = 0; j < cols; j++)
            {
                if (grid[i, j][0] == PlatformType.STABLE)
                {
                    bool is_up = false;
                    bool is_up_left = false;
                    bool is_up_right = false;

                    if (grid[i + 1, j][0] == PlatformType.STABLE)
                        is_up = true;
                    
                    if (j + 1 < cols)
                    {
                        if (grid[i + 1, j + 1][0] == PlatformType.STABLE)
                            is_up_right = true;
                    }

                    if (j - 1 > 0)
                    {
                        if (grid[i + 1, j - 1][0] == PlatformType.STABLE)
                            is_up_left = true;
                    }

                    if (is_up || is_up_left || is_up_right)
                    {
                        num_platforms_with_constraint++;
                    }
                }
            }
        }
        if (num_platforms_with_constraint >= 2 * rows - row_difference)
            return true;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if(player_lives == 0)
        {
            SceneManager.LoadScene("FinishLose");
        }
        num_lives_text.text = string.Format("Lives: {0}", player_lives);
        //Update timer displayed on platform canvas
        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            timer_text.text = string.Format("{0}", Mathf.FloorToInt(timer));
        }
        else
        {
            if (!music_already_played)
            {
                boss_audio_source.Play();
                music_already_played = true;
            }
            cutscene_being_shown = false;
        }

        lava_audio_source.loop = true;
        boss_audio_source.loop = true;
    }
}
