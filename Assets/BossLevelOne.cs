using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // Start is called before the first frame update
    void Start()
    {
        function_calls = 100000;
        //Dimensions of grid
        rows = 14;
        cols = 6;
        //For CSP calculation
        row_difference = 10;
        //Represents the platforms
        List<PlatformType>[,] grid = new List<PlatformType>[rows, cols];
        //For platforms with no assignment yet - for CSP
        List<int[]> unassigned = new List<int[]>();

        bool success = false;
        while(!success)
        {   
            //Randomly fill grid with assignments of either 0 or 1 - give 4/5 chance to 1 coming first
            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < cols; j++)
                {
                    if (Random.Range(1, 4) == 1)
                        grid[i, j] = new List<PlatformType> { PlatformType.STABLE, PlatformType.UNSTABLE };
                    else
                        grid[i, j] = new List<PlatformType> { PlatformType.UNSTABLE, PlatformType.STABLE };

                    unassigned.Add(new int[] { i, j });
                    // Console.WriteLine("[" + i + ", " + j + "]: " + grid[i, j][0] + "," + grid[i, j][1]);
                }
            }

            success = BackTrackingSearch(grid, unassigned);
            if (!success)
            {
                Debug.Log("Trying to generate solution again...");
                unassigned.Clear();
                grid = new List<PlatformType>[rows, cols];
                function_calls = 0;
            }
        }

        for(int i = 0; i < rows; i++)
        {
            string print = "";
            print += i + ":  ";
            for(int j = 0; j < cols; j++)
            {
                if (grid[i,j][0] == PlatformType.STABLE)
                    print += "S  ";
                else
                    print += "U  ";
            }
            Debug.Log(print);
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
        
    }
}
