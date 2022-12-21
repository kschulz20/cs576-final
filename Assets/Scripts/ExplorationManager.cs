using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

enum TileType
{
    WALL = 0,
    FLOOR = 1
}

public class ExplorationManager : MonoBehaviour
{

    public GameObject notebook_text;
    public GameObject notebook_left_button;
    public GameObject notebook_right_button;
    public GameObject to_do_text;
    public GameObject checklist_left_button;
    public GameObject checklist_right_button;
    public GameObject dialogue_text;
    public GameObject interact_box;
    public GameObject timer_text;
    public Button pause_button;
    public Material maze_material;

    public GameObject phone_1;
    public GameObject briefcase;
    public GameObject paper_1;
    public GameObject laptop_1;
    public GameObject prof_1;
    public GameObject paper_2;
    public GameObject maze_item;
    public GameObject tablet;
    public GameObject textbook;
    public GameObject prof_2;

    internal int checklist_page = 1;
    internal int notebook_page = 1;
    internal List<string> needed_items = new List<string> { "Cellphone", "Briefcase", "Paper 1", "Laptop 1", "Talk to Professor Baime Pavila", "Paper 2", "Tablet", "Textbook", "Talk to Professor Maniel Bheldon", "Laptop 2"};      //Items needed for collection still
    internal List<string> notebook_notes = new List<string>();     //Collected items so far
    internal List<string> prof_talked_to = new List<string>();
    internal Dictionary<string, string> stage_one_item_to_info = new Dictionary<string, string>();     //Maps item name to corresponding info item gives for stage 1
    internal Dictionary<string, string> stage_one_item_to_descrip = new Dictionary<string, string>();      //Maps item name to corresponding item description for stage 1
    internal Dictionary<string, bool> stage_one_item_near = new Dictionary<string, bool>();     //Keep track if player is near stage 1 items
    internal bool powerup_landed_on_player_recently = false;
    internal float timestamp_powerup_landed = float.MaxValue;

    private float timer = 300.0f;     //Timer to indicate when exploration stage will end
    private List<string> stage_one_items = new List<string> { "Cellphone", "Briefcase", "Paper 1", "Laptop 1", "Talk to Professor Baime Pavila", "Paper 2", "Tablet", "Textbook", "Talk to Professor Maniel Bheldon", "Laptop 2" };      //Required items for stage 1
    private List<string> stage_one_info = new List<string> { "Variables are used to store info in computer programs",
                                                             "In Java, variables can be type integer (int)",
                                                             "In Java, variables can be alphabetical characters (String)",
                                                             "Variables can be updated and reassigned",
                                                             "To add to an integer variable, do int variable += addedNum",
                                                             "To print out a String in Java with a newline, do System.out.println(printedString);",
                                                             "In the for loop for(int i = 0; i < X; i++), it will iterate only when i < X", 
                                                             "for(int i = 0; i < X; i++), i will start at 0 first iteration and increment by 1 every next loop",
                                                             "The code under if statement only execute if the condition is true, else the else statement executes",
                                                             "For while loops while(X), it runs only when the X condition is true" };
    private List<string> stage_one_descrip = new List<string> { "Professor Rawrrington's cellphone. It contains some insights on what will be on the exam",
                                                                "This briefcase contains documents inscribed with some info about Java variables",
                                                                "The paper is a research article about Java String variables. Definitely useful for the test!",
                                                                "Professor Rawrrington's laptop that contains notes about the upcoming exam.",
                                                                "Prof. Baime Pavila: Why hello there. Are you prepared for the upcoming CS121 exam? I taught the class before, just remember how to do operations with variables and you will do great! :)",
                                                                "A highly scientific paper about printing Strings in Java. This will be beyond useful to ace the test.",
                                                                "The tablet contains Professor Binea's notes on how for loops work. Looks complex and difficult.",
                                                                "A textbook on how for loop iterations work. Highly technical.",
                                                                "Prof. Maniel Bheldon: Have your CS121 exam soon? It will be easy as cake, especially if you understand how if else statements work! I can give you a brief overview right now! ;)",
                                                                "Professor Mario's confidential laptop. It has instructions on how while loops work." };

    private List<float[]> item_pos;
    private GameObject[] door_blocks;
    private int width = 74;
    private int length = 73;
    private int maze_width = 16;
    private int maze_length = 16;
    private int maze_item_pos_x = -1;
    private int maze_item_pos_z = -1;
    private int entrance_1_x = 0;
    private int entrance_1_z = 8;
    private int entrance_2_x = 8;
    private int entrance_2_z = 0;
    private float storey_height = 10f;   // height of walls in maze
    private float first_floor_y = 1.0f;
    private float second_floor_y = 7.5f;
    private Vector3 player_starting_pos = new Vector3(25.0f, 1.0f, 0.0f);
    private int function_calls = 0;     // number of function calls during backtracking for solving the CSP
    private float radius_items_apart_from_each_other = 15.0f;
    private float wall_check_radius = 4.0f;
    private float doorway_check_radius = 5.0f;
    private float radius_from_player = 10.0f;
    private List<TileType>[,] grid;
    private float player_health = 1.0f;
    private int num_powerup = 0;
    private List<int[]> pos_powerup;
    public AudioSource audio_source;
    public AudioClip item_pickup_sfx;
    private bool is_paused = false;

    private bool won = false;
    private bool gameover = false;
    public RenderPipelineAsset render_pipeline;
    public TerrainData exploration_stage_terrain;
    public Material terrain_material;

    // a helper function that randomly shuffles the elements of a list (useful to randomize the solution to the CSP)
    private void Shuffle<T>(ref List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Terrain>().materialTemplate = terrain_material;
        gameObject.GetComponent<TerrainCollider>().terrainData = exploration_stage_terrain;
        GraphicsSettings.renderPipelineAsset = render_pipeline;
        audio_source.Play();
        notebook_left_button.SetActive(false);
        notebook_right_button.SetActive(false);
        checklist_left_button.SetActive(false);
        interact_box.SetActive(false);
        checklist_right_button.SetActive(true);
        timer_text.GetComponent<Text>().text = "Time Left: " + timer.ToString("0.0");
        num_powerup = 0;
        player_health = 1.0f;
        powerup_landed_on_player_recently = false;
        timestamp_powerup_landed = float.MaxValue;
        won = false;
        gameover = false;
        is_paused = false;

        pause_button.onClick.AddListener(PauseButtonFunction);

        string to_do_string = "";
        int to_do_counter = checklist_page * 5 - 4;
        for (int i = checklist_page * 5 - 5; i <= checklist_page * 5 - 1; i++)
        {
            if (i >= 0 && i < needed_items.Count)
            {
                to_do_string += to_do_counter.ToString() + ". " + needed_items[i] + "\n";
                to_do_counter++;
            }
        }
        for(int i = 0; i < stage_one_items.Count; i++)
        {
            stage_one_item_to_info.Add(stage_one_items[i], stage_one_info[i]);
            stage_one_item_to_descrip.Add(stage_one_items[i], stage_one_descrip[i]);
            stage_one_item_near.Add(stage_one_items[i], false);
        }
        to_do_text.GetComponent<Text>().text = to_do_string;

        door_blocks = GameObject.FindGameObjectsWithTag("DoorwayBlock");

        // initialize 2D grid
        List<int>[,] item_grid = new List<int>[width, length];
        // useful to keep variables that are unassigned so far
        //List<int[]> unassigned = new List<int[]>();
        item_pos = new List<float[]>();
        bool success = false;
        while (!success)
        {
            for (int i = 0; i < stage_one_items.Count; i++)
            {
                while (true)
                {
                    // try a random location in the grid
                    int wr = Random.Range(0, width);
                    int lr = Random.Range(0, length);
                    int first_or_second = Random.Range(1, 3);

                    // if grid location is empty/free, place it there
                    //(x > 28 && x < 67) && (z > 17 && z < 57)
                    if (item_grid[wr, lr] == null && !(wr > 35 && wr < 74 && lr > 33 && lr < 73))
                    {
                        item_grid[wr, lr] = new List<int> { 1 };
                        if(first_or_second == 1)
                        {
                            item_pos.Add(new float[3] { (float)wr, first_floor_y, (float)lr });
                        }
                        else
                        {
                            item_pos.Add(new float[3] { (float)wr, second_floor_y, (float)lr });
                        }
                        break;
                    }
                }
            }
            success = CheckItemLocationConstraints();
            if (!success)
            {
                Debug.Log("Could not find valid solution for item positions - will try again");
                item_grid = new List<int>[width, length];
                function_calls = 0;
                item_pos = new List<float[]>();
            }
        }

        List<GameObject> items = new List<GameObject>() { phone_1, briefcase, paper_1, laptop_1, prof_1, paper_2, tablet, textbook, prof_2 };
        for(int i = 0; i < items.Count; i++)
        {
            items[i].transform.position = new Vector3(item_pos[i][0] - 7.0f, item_pos[i][1], item_pos[i][2] - 16.0f);
        }

        success = false;
        List<TileType>[,] grid = new List<TileType>[maze_width, maze_length];
        List<int[]> unassigned = new List<int[]>();
        while (!success)
        {
            for (int w = 0; w < maze_width; w++)
                for (int l = 0; l < maze_length; l++)
                    if ((w == entrance_1_x && l == entrance_1_z) || (w == entrance_2_x && l == entrance_2_z))
                        {
                            grid[w, l] = new List<TileType> { TileType.FLOOR };
                        }
                    else if (w == 0 || l == 0 || w == maze_width - 1 || l == maze_length - 1) {
                        grid[w, l] = new List<TileType> { TileType.WALL };
                    }
                    else
                    {
                        if (grid[w, l] == null) // does not have virus already or some other assignment from previous run
                        {
                            List<TileType> candidate_assignments = new List<TileType> { TileType.WALL, TileType.FLOOR };
                            Shuffle<TileType>(ref candidate_assignments);

                            grid[w, l] = candidate_assignments;
                            unassigned.Add(new int[] { w, l });
                        }
                    }

            // YOU MUST IMPLEMENT this function!!!
            success = BackTrackingSearch(grid, unassigned);
            if (!success)
            {
                Debug.Log("Could not find valid solution - will try again");
                unassigned.Clear();
                grid = new List<TileType>[maze_width, maze_length];
                function_calls++;
            }
            if(!success && function_calls > 1000) //bug testing
            {
                Debug.Log("NO SOLUTION");
                break;
            }
        }
        DrawDungeon(grid);
    }

    // Update is called once per frame
    void Update()
    {
        audio_source.loop = true;
        //check win/lose condition
        if(gameover)
        {
            if (needed_items.Count == 0)
            {
                SceneManager.LoadScene("SampleScene");
            }
            else
            {
                SceneManager.LoadScene("FinishLose");
            }
        }
        if(timer >= 0.0f && !gameover)
        {
            if(!is_paused) 
            {
                timer -= Time.deltaTime;
                timer_text.GetComponent<Text>().text = "Time Left: " + timer.ToString("0.0");
            }
        }
        else
        {
            gameover = true;
        }
    }

    //y is fixed for now
    // while (((x > 28 && x < 67) && (z > 17 && z < 57)) || !validPosition)
    bool IsItemsNotInsideWall()
    {
        bool is_valid = true;
        for(int i = 0; i < item_pos.Count; i++)
        {
            Vector3 pos = new Vector3(item_pos[i][0], item_pos[i][1], item_pos[i][2]);
            Collider[] colliders = Physics.OverlapSphere(pos, wall_check_radius);
            foreach (Collider col in colliders)
            {
                if (col.tag == "Wall")
                {
                    is_valid = false;
                }
            }
        }
        return is_valid;
    }

    bool IsItemsNotCloseToEachOther()
    {
        bool is_valid = true;
        for(int i = 0; i < item_pos.Count; i++)
        {
            for(int j = i+1; j < item_pos.Count; j++)
            {
                Vector3 pos_1 = new Vector3(item_pos[i][0], item_pos[i][1], item_pos[i][2]);
                Vector3 pos_2 = new Vector3(item_pos[j][0], item_pos[j][1], item_pos[j][2]);
                if (item_pos[i][1] == item_pos[j][1] && Vector3.Distance(pos_1, pos_2) <= radius_items_apart_from_each_other)
                {
                    is_valid = false;
                }
            }
        }
        return is_valid;
    }

    bool IsItemsBothOnFirstAndSecondFloor()
    {
        int first_floor_count = 0;
        int second_floor_count = 0;
        for(int i = 0; i < item_pos.Count; i++)
        {
            if(item_pos[i][1] == first_floor_y)
            {
                first_floor_count++;
            }
            else
            {
                second_floor_count++;
            }
        }
        return first_floor_count >= 2 && second_floor_count >= 2;
    }

    bool IsItemNotNearDoor()
    {
        bool is_valid = true;
        for(int i = 0; i < item_pos.Count; i++)
        {
            for(int j = 0; j < door_blocks.Length; j++)
            {
                Vector3 pos = new Vector3(item_pos[i][0], item_pos[i][1], item_pos[i][2]);
                if(item_pos[i][1] == door_blocks[j].transform.position.y && Vector3.Distance(pos, door_blocks[j].transform.position) <= doorway_check_radius)
                {
                    is_valid = false;
                }
            }
        }
        return is_valid;
    }

    bool IsItemFarEnoughFromPlayer()
    {
        bool is_valid = true;
        for(int i = 0; i < item_pos.Count; i++)
        {
            Vector3 pos = new Vector3(item_pos[i][0], item_pos[i][1], item_pos[i][2]);
            if (item_pos[i][1] == player_starting_pos.y && Vector3.Distance(pos, player_starting_pos) <= radius_from_player)
            {
                is_valid = false;
            }
        }
        return is_valid;
    }

    bool CheckItemLocationConstraints()
    {
        bool areWeConsistent = IsItemsNotInsideWall() && IsItemsNotCloseToEachOther()
                            && IsItemsBothOnFirstAndSecondFloor()
                            && IsItemNotNearDoor()
                            && IsItemFarEnoughFromPlayer();
        return areWeConsistent;
    }

    //CSP contraint 1
    bool DoWeHaveTooManyInteriorWalls(List<TileType>[,] grid)
    {
        int[] number_of_assigned_elements = new int[] { 0, 0 };
        for (int w = 0; w < maze_width; w++)
            for (int l = 0; l < maze_length; l++)
            {
                if (w == 0 || l == 0 || w == maze_width - 1 || l == maze_length - 1)
                    continue;
                if (grid[w, l].Count == 1)
                    number_of_assigned_elements[(int)grid[w, l][0]]++;
            }

        if ((number_of_assigned_elements[(int)TileType.WALL] > (maze_width * maze_length) / 4))
            return true;
        else
            return false;
    }

    //CSP contraint 2
    // another type of constraint already implemented for you
    bool DoWeHaveTooFewWalls(List<TileType>[,] grid)
    {
        int[] number_of_potential_assignments = new int[] { 0, 0 };
        for (int w = 0; w < maze_width; w++)
            for (int l = 0; l < maze_length; l++)
            {
                if (w == 0 || l == 0 || w == maze_width - 1 || l == maze_length - 1)
                    continue;
                for (int i = 0; i < grid[w, l].Count; i++)
                    number_of_potential_assignments[(int)grid[w, l][i]]++;
            }

        if ((number_of_potential_assignments[(int)TileType.WALL] < (maze_width * maze_length) / 6))
            return true;
        else
            return false;
    }

    // must return true if there are three (or more) interior consecutive wall blocks either horizontally or vertically
    // by interior, we mean walls that do not belong to the perimeter of the grid
    // e.g., a grid configuration: "FLOOR - WALL - WALL - WALL - FLOOR" is not valid
    bool TooLongWall(List<TileType>[,] grid)
    {
        /*** implement the rest ! */
        for (int w = 1; w < maze_width - 1; w++)
        {
            for (int l = 1; l < maze_length - 1; l++)
            {
                if (grid[w, l][0] == TileType.WALL)
                {
                    if (w + 3 < maze_width - 1 && grid[w + 1, l][0] == TileType.WALL && grid[w + 2, l][0] == TileType.WALL && grid[w + 3, l][0] == TileType.WALL)
                    {
                        return true;
                    }
                    if (l + 3 < maze_length - 1 && grid[w, l + 1][0] == TileType.WALL && grid[w, l + 2][0] == TileType.WALL && grid[w, l + 3][0] == TileType.WALL)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // check if attempted assignment is consistent with the constraints or not
    bool CheckConsistency(List<TileType>[,] grid, int[] cell_pos, TileType t)
    {
        int w = cell_pos[0];
        int l = cell_pos[1];

        List<TileType> old_assignment = new List<TileType>();
        old_assignment.AddRange(grid[w, l]);
        grid[w, l] = new List<TileType> { t };

        // note that we negate the functions here i.e., check if we are consistent with the constraints we want
        bool areWeConsistent = !DoWeHaveTooManyInteriorWalls(grid) && !DoWeHaveTooFewWalls(grid);

        grid[w, l] = new List<TileType>();
        grid[w, l].AddRange(old_assignment);
        return areWeConsistent;
    }

    bool BackTrackingSearch(List<TileType>[,] grid, List<int[]> unassigned)
    {
        // if there are too many recursive function evaluations, then backtracking has become too slow (or constraints cannot be satisfied)
        // to provide a reasonable amount of time to start the level, we put a limit on the total number of recursive calls
        // if the number of calls exceed the limit, then it's better to try a different initialization
        if (function_calls++ > 100000)
            return false;

        // we are done!
        if (unassigned.Count == 0)
            return true;

        Shuffle<int[]>(ref unassigned);
        int[] unassigned_tile = unassigned[0];
        List<TileType> candidate_assignments = grid[unassigned_tile[0], unassigned_tile[1]];
        foreach (TileType t in candidate_assignments)
        {
            if (CheckConsistency(grid, unassigned_tile, t))
            {
                grid[unassigned_tile[0], unassigned_tile[1]] = new List<TileType> { t };
                unassigned.RemoveAt(0);
                bool result = BackTrackingSearch(grid, unassigned);
                if (result)
                {
                    return true;
                }
                grid[unassigned_tile[0], unassigned_tile[1]] = candidate_assignments;
                unassigned.Insert(0, unassigned_tile);
            }
        }
        return false;
    }

    void GetPathToItem(List<TileType>[,] solution, int entrance_x, int entrance_z)
    {
        bool contains_path = false;
        int[,] g_vals = new int[maze_width, maze_length];
        for (int i = 0; i < maze_width; i++)
        {
            for (int j = 0; j < maze_width; j++)
            {
                g_vals[i, j] = 5000000;
            }
        }
        int[,] f_vals = new int[maze_width, maze_length];
        for (int i = 0; i < maze_width; i++)
        {
            for (int j = 0; j < maze_width; j++)
            {
                f_vals[i, j] = 5000000;
            }
        }
        int[,,] came_from = new int[maze_width, maze_length, 2];
        g_vals[entrance_x, entrance_z] = 0;
        f_vals[entrance_x, entrance_z] = Mathf.Abs(entrance_x - maze_item_pos_x) + Mathf.Abs(entrance_z - maze_item_pos_z);
        int[] source = new int[2] { entrance_x, entrance_z };
        List<int[]> priority_queue = new List<int[]> { source };
        while (priority_queue.Count > 0)
        {
            int lowest_score = 5000000;
            int[] lowest_node = new int[2];
            int lowest_index = 0;
            for (int i = 0; i < priority_queue.Count; i++)
            {
                if (f_vals[priority_queue[i][0], priority_queue[i][1]] < lowest_score)
                {
                    if ((priority_queue[i][0] == maze_item_pos_x && priority_queue[i][1] == maze_item_pos_z) || (0 < priority_queue[i][0] && priority_queue[i][0] < maze_width && 0 < priority_queue[i][1] && priority_queue[i][1] < maze_length))
                    {
                        lowest_score = f_vals[priority_queue[i][0], priority_queue[i][1]];
                        lowest_node = priority_queue[i];
                        lowest_index = i;
                    }
                }
            }
            if (lowest_node[0] == maze_item_pos_x && lowest_node[1] == maze_item_pos_z)
            {
                if (g_vals[maze_item_pos_x, maze_item_pos_z] < 1000)
                {
                    contains_path = true;
                }
                break;
            }
            priority_queue.RemoveAt(lowest_index);
            for (int i = lowest_node[0] - 1; i < lowest_node[0] + 2; i++)
            {
                for (int j = lowest_node[1] - 1; j < lowest_node[1] + 2; j++)
                {
                    if (i == lowest_node[0] - 1 && j == lowest_node[1] - 1)
                    {
                        continue;
                    }
                    if (i == lowest_node[0] - 1 && j == lowest_node[1] + 1)
                    {
                        continue;
                    }
                    if (i == lowest_node[0] + 1 && j == lowest_node[1] - 1)
                    {
                        continue;
                    }
                    if (i == lowest_node[0] + 1 && j == lowest_node[1] + 1)
                    {
                        continue;
                    }
                    if ((i == maze_item_pos_x && j == maze_item_pos_z) || (i > 0 && i < maze_width && j > 0 && j < maze_length))
                    {
                        int edge_cost = 0;
                        if (solution[i, j][0] == TileType.WALL)
                        {
                            edge_cost = 1000;
                        }
                        else
                        {
                            edge_cost = 1;
                        }
                        int tentative_g_score = g_vals[lowest_node[0], lowest_node[1]] + edge_cost;
                        if (tentative_g_score < g_vals[i, j])
                        {
                            g_vals[i, j] = tentative_g_score;
                            f_vals[i, j] = tentative_g_score + Mathf.Abs(i - maze_item_pos_x) + Mathf.Abs(j - maze_item_pos_z);
                            came_from[i, j, 0] = lowest_node[0];
                            came_from[i, j, 1] = lowest_node[1];
                            bool contains = false;
                            for (int k = 0; k < priority_queue.Count; k++)
                            {
                                if (priority_queue[k][0] == i && priority_queue[k][1] == j)
                                {
                                    contains = true;
                                }
                            }
                            if (!contains)
                            {
                                int[] neighbor = new int[2] { i, j };
                                priority_queue.Add(neighbor);
                            }
                        }
                    }
                }
            }
        }
        if (!contains_path)
        {
            int[] node_path = new int[2];
            node_path[0] = came_from[maze_item_pos_x, maze_item_pos_z, 0];
            node_path[1] = came_from[maze_item_pos_x, maze_item_pos_z, 1];
            while (true)
            {
                if (solution[node_path[0], node_path[1]][0] == TileType.WALL && node_path[0] > 0 && node_path[0] < maze_width && node_path[1] > 0 && node_path[1] < maze_length)
                {
                    solution[node_path[0], node_path[1]][0] = TileType.FLOOR;
                }
                int node_path_x = node_path[0];
                int node_path_y = node_path[1];
                node_path[0] = came_from[node_path_x, node_path_y, 0];
                node_path[1] = came_from[node_path_x, node_path_y, 1];
                if (node_path[0] == 0 && node_path[1] == 0)
                {
                    break;
                }
            }
        }
        grid = solution;
    }

    // places the primitives/objects according to the grid assignents
    // you will need to edit this function (see below)
    void DrawDungeon(List<TileType>[,] solution)
    {

        // place character at random position (wr, lr) in terms of grid coordinates (integers)
        // make sure that this random position is a FLOOR tile (not wall, drug, or virus)
        int wr = 0;
        int lr = 0;

        if (maze_item_pos_x == -1 && maze_item_pos_z == -1)
        {
            while (true) // try until a valid position is sampled
            {
                wr = Random.Range(1, maze_width - 1);
                lr = Random.Range(1, maze_length - 1);

                if (solution[wr, lr][0] == TileType.FLOOR && (-1 * wr + maze_length) < lr)
                {
                    float x = (float)(wr * 2) + 33.0f;
                    float z = (float)(lr * 2) + 22.0f;
                    maze_item.transform.position = new Vector3(x, storey_height, z);
                    break;
                }
            }
            maze_item_pos_x = wr;
            maze_item_pos_z = lr;
        }
        else
        {
            wr = maze_item_pos_x;
            lr = maze_item_pos_z;
            float x = (float)wr + 33.0f;
            float z = (float)lr + 22.0f;
            maze_item.transform.position = new Vector3(x, storey_height, z);
        }

        GetPathToItem(solution, entrance_1_x, entrance_1_z);
        GetPathToItem(solution, entrance_2_x, entrance_2_z);

        // the rest of the code creates the scenery based on the grid state 
        // you don't need to modify this code (unless you want to replace the virus
        // or other prefabs with something else you like)
        for (int x = 0; x < maze_width; x++)
        {
            for (int z = 0; z < maze_length; z++)
            {
                //Debug.Log(w + " " + l + " " + h);
                if (grid[x, z][0] == TileType.WALL)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = "WALL";
                    float wall_x = (float)(x*2) + 33.0f;
                    float wall_z = (float)(z*2) + 22.0f;
                    cube.transform.position = new Vector3(wall_x, 0, wall_z);
                    cube.transform.localScale = new Vector3(2, storey_height, 2);
                    cube.GetComponent<Renderer>().material = maze_material;
                }
            }
        }
    }

    void PauseGame()
    {
        is_paused = true;
        Time.timeScale = 0;
    }

    void ResumeGame()
    {
        is_paused = false;
        Time.timeScale = 1;
    }

    void PauseButtonFunction()
    {
        if (!is_paused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    internal void UpdateCheckListPage(int new_page)
    {
        checklist_page = new_page;
    }

    internal void UpdateNotebookPage(int new_page)
    {
        notebook_page = new_page;
    }

}
