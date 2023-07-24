//using Mkey;
//using System.Collections.Generic;

//-MatchGrid.cs--------------------------------------------------------------------------------------//

//public void FillState_CurGrid(MatchGrid g, SpawnController sC)
//{
//    List<GridCell> gFreeCells = GetFreeCells(g, true);

//    int cnt = 0;
//    if (gFreeCells.Count > 0) cellContainer.CreateFillPath(g);




//    //int count = 0;
//    //for (int i = 0; i < g.Cells.Count; i++)
//    //{
//    //    if (g.Cells[i].fillPathToSpawner == null) count++;
//    //}

//    //Debug.Log("fillPathToSpawner == null : " + count);

//    //List<List<string>> b = new List<List<string>>();

//    //for (int i = 0; i < g.Cells.Count; i++)
//    //{
//    //    List<string> a = new List<string>();

//    //    int d = 0;

//    //    if (g.Cells[i].fillPathToSpawner == null)
//    //    {
//    //        d = 3;
//    //    }

//    //    for (int j = 0; j < g.Cells[i].fillPathToSpawner.Count; j++)
//    //    {
//    //        a.Add("Row:" + g.Cells[i].fillPathToSpawner[j].Row + "Column" + g.Cells[i].fillPathToSpawner[j].Column);
//    //    }

//    //    b.Add(a);
//    //}




//    //List<GameObject> cellList = new List<GameObject>();
//    //for (int index = 0; index < g.Cells.Count; index++) cellList.Add(g.Cells[index].DynamicObject);


//    while (gFreeCells.Count > 0)
//    {
//        cellContainer.FillGridByStep(gFreeCells, () => { });
//        gFreeCells = GetFreeCells(g, true);
//        cnt++;

//        if (cnt > 100)
//        {
//            notClear = true;
//            return;
//        }

//    }

//    //NoPhys1(g);
//    //while (!NoPhys1(g));
//    //if (noMatches) RemoveMatches();
//    cur_state = 2;
//}




//void GetMatch3Level(int populationSize, MatchGrid g, Dictionary<int, TargetData> targets, Spawner spawnerPrefab, SpawnerStyle spawnerStyle, Transform GridContainer, Transform trans, LevelConstructSet IC)
//{
//    SpawnController sC = SpawnController.Instance;

//    int repetition_limit = 1;
//    int generation_limit = 1;
//    int move_limit = 20;
//    g.mgList = new List<MatchGroup>();

//    for (int generation_idx = 0; generation_idx < generation_limit; generation_idx++)
//    {
//        //List<MatchObject> MatchObjectCollect = new List<MatchObject>(g.Cells.Count);
//        //for (int i = 0; i < g.Cells.Count; i++)
//        //{
//        //    MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
//        //    g.Cells[i].SetObject(m);
//        //    MatchObjectCollect.Add(m);
//        //}

//        gameBoard = new GameBoard();
//        gameBoard.test(g, spawnerPrefab, spawnerStyle, GridContainer, trans, IC);
//        //g.mgList = new List<MatchGroup>();

//        for (int population_idx = 0; population_idx < populationSize; population_idx++) // grid 개수
//        {
//            List<int> collect_Fitness = new List<int>();

//            for (int repetition_idx = 0; repetition_idx < repetition_limit; repetition_idx++) // 평균 계산 반복 횟수
//            {
//                for (int j = 0; j < Cells.Count; j++)
//                {
//                    MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
//                    g.Cells[j].SetObject(m);

//                    //g.Cells[j].SetObject(MatchObjectCollect[j]);
//                    if (ga.Population[population_idx].Genes[j] == '1')
//                    {
//                        BlockedObject b = sC.GetRandomBlockedObjectPrefab(LcSet, goSet);
//                        g.Cells[j].SetObject(b);
//                    }
//                }

//                cellContainer = new FitnessHelper();
//                move = move_limit;
//                move += 5;
//                cur_state = 0;
//                targetClear = false;
//                notClear = false;
//                //grid.mgList = new List<MatchGroup>();

//                foreach (var item in targets) item.Value.InitCurrCount();

//                ////fillpath 생성으로 
//                cellContainer.CreateFillPath(g);

//                while (move > 0 && targetClear == false && notClear == false)
//                {
//                    switch (cur_state)
//                    {
//                        case 0:
//                            FillState_CurGrid(g, sC);
//                            break;
//                        case 1:
//                            ShowState_CurGrid(g, targets, sC, trans);
//                            break;
//                        case 2:
//                            CollectState_CurGrid(g, targets);
//                            break;
//                    }
//                }

//                //List<GameObject> cellList = new List<GameObject>();
//                //for (int index = 0; index < g.Cells.Count; index++) cellList.Add(g.Cells[index].DynamicObject);

//                if (targetClear == true) collect_Fitness.Add(move - move_limit);
//                else collect_Fitness.Add(0);
//            }

//            int cnt = 0;
//            int sum = 0;

//            for (int idx2 = 0; idx2 < collect_Fitness.Count; idx2++)
//            {
//                if (collect_Fitness[idx2] != 0)
//                {
//                    sum += collect_Fitness[idx2];
//                    cnt++;
//                }
//            }

//            if (cnt == 0 || sum == 0) ga.Population[population_idx].SetFitness(0);
//            else ga.Population[population_idx].SetFitness(sum / cnt);

//            Debug.Log("Fitness =========" + ga.Population[population_idx].Fitness + "------------------------------------");

//        }

//        ga.NewGeneration();
//    }
//}

//public void ShowStateGrid(MatchGrid g, Dictionary<int, TargetData> targets, SpawnController sC, Transform trans)
//{
//List<int> find = new List<int>();
//find = g.mgList[target_num].GetEst(g.mgList, target_num);

//GridCell tmp_Cell = new GridCell();
//tmp_Cell = g.Cells[g.mgList[target_num].Cells[0].Column + g.mgList[target_num].Cells[0].Row * Rows.Count];

//tmp_Cell = g.Cells[find[0] + find[1] * Rows.Count];
//int tmp_row = g.Cells[find[0] + find[1] * Rows.Count].Row;
//int tmp_col = g.Cells[find[0] + find[1] * Rows.Count].Column;

//g.Cells[find[0] + find[1] * Rows.Count] = g.Cells[find[2] + find[3] * Rows.Count];
//g.Cells[find[0] + find[1] * Rows.Count].Row = g.Cells[find[2] + find[3] * Rows.Count].Row;
//g.Cells[find[2] + find[3] * Rows.Count] = tmp_Cell;

//}

//public void FillStateGrid(MatchGrid g, SpawnController sC)
//{
//    //List<GridCell> gFreeCells = g.GetFreeCells(); // Debug.Log("fill free count: " + gFreeCells.Count + " to collapse" );

//    if (cellContainer.freeCellContainer.Count <= 0)
//    {
//        cur_state = 2;
//    }

//    else
//    {
//        ////bool filled = false;
//        ////cellContainer.CreateFillPath(g);
//        ////while (cellContainer.freeCellContainer.Count > 0)
//        ////{
//        ////cellContainer.FillGridByStep(cellContainer.freeCellContainer, () => { });
//        ////cellContainer.freeCellContainer = g.GetFreeCells();
//        ////}

//        //for (int i = 0; i < cellContainer.freeCellContainer.Count;i++) 
//        //{
//        //    //cellContainer.freeCellContainer.Add(g.Cells[cellContainer.l_mgList[i].Cells[j].Row * Rows.Count() + cellContainer.l_mgList[i].Cells[j].Column]);
//        //    MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
//        //    Cells[cellContainer.freeCellContainer[i].Column + cellContainer.freeCellContainer[i].Row * Rows.Count()].SetObject(m);

//        //    //Cells[j].SetObject(m);

//        //}

//        cellContainer.CreateFillPath(g);
//        cellContainer.FillGridByStep(cellContainer.freeCellContainer, () => { });
//        cellContainer.freeCellContainer.RemoveAll(x => x.IsMatchable);

//        cur_state = 2;
//    }



//    //if (noMatches)
//    //{
//    //    RemoveMatches();
//    //}

//}

//int mix_limit = 10;
//int mix_cnt = 0;

//public void ShowStateGrid(MatchGrid g, Dictionary<int, TargetData> targets, SpawnController sC)
//{
//    cellContainer.l_mgList = new List<MatchGroup>();

//    while (mix_cnt <= mix_limit)
//    {
//        cellContainer.CancelTweens();
//        cellContainer.CreateMatchGroups(2, true, g);

//        if (cellContainer.l_mgList.Count == 0)
//        {
//            cellContainer.MixGrid(g);
//            mix_cnt++;
//        }

//        else break;
//    }

//    if (mix_cnt >= mix_limit)
//    {
//        mix_cnt = 0;
//        notClear = true;
//        return;
//    }

//    else
//    {
//        List<List<int>> targetsInCell = new List<List<int>>();

//        foreach (var currentCellsID in g.Cells)
//        {
//            List<int> res = currentCellsID.GetGridObjectsIDs();

//            foreach (var item in targets)
//            {
//                if (!item.Value.Achieved)
//                {
//                    if (item.Value.ID >= 1000 && item.Value.ID <= 1006)
//                    {
//                        if (item.Value.ID == res[0])
//                        {
//                            targetsInCell.Add(new List<int> { currentCellsID.Row, currentCellsID.Column });
//                        }
//                    }
//                }
//            }
//        }

//        //cellContainer.TargetSwap(targetsInCell);

//        List<int> collectMatch = new List<int>();

//        for (int i = 0; i < cellContainer.l_mgList.Count; i++)
//        {
//            bool isInsert = false;

//            for (int j = 0; j < targetsInCell.Count; j++)
//            {
//                if (cellContainer.l_mgList[i].Cells[0].Row == targetsInCell[j][0] && cellContainer.l_mgList[i].Cells[0].Column == targetsInCell[j][1])
//                {
//                    isInsert = true;
//                    break;
//                }

//                if (cellContainer.l_mgList[i].Cells[1].Row == targetsInCell[j][0] && cellContainer.l_mgList[i].Cells[1].Column == targetsInCell[j][1])
//                {
//                    isInsert = true;
//                    break;
//                }
//            }

//            if (isInsert) collectMatch.Add(i);
//        }

//        int target_num = 0;

//        if (collectMatch.Count <= 0)
//        {
//            cellContainer.l_mgList[0].SwapEstimate();
//            target_num = 0;
//        } 

//        else
//        {
//            int number = Random.Range(0, collectMatch.Count - 1);
//            cellContainer.l_mgList[collectMatch[number]].SwapEstimate();
//            target_num = collectMatch[number];
//        }

//        List<int> find = new List<int>();
//        find = cellContainer.l_mgList[target_num].GetEst();

//        GridCell tmp_Cell = new GridCell();

//        tmp_Cell = Cells[cellContainer.l_mgList[target_num].Cells[0].Column + cellContainer.l_mgList[target_num].Cells[0].Row * Rows.Count];

//        tmp_Cell = Cells[find[0] + find[1] * Rows.Count];
//        Cells[find[0] + find[1] * Rows.Count] = Cells[find[2] + find[3] * Rows.Count];
//        Cells[find[2] + find[3] * Rows.Count] = tmp_Cell;

//        //Cells[cellContainer.freeCellContainer[i].Column + cellContainer.freeCellContainer[i].Row * Rows.Count()].SetObject(m);

//        //for (int i=0;i< cellContainer.l_mgList.Count; i++)
//        //{
//        //    //Cells[cellContainer.l_mgList[target_num].Cells[0].Column + cellContainer.l_mgList[target_num].Cells[0].Row * Rows.Count] = 

//        //    //MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
//        //    //Cells[i].SetObject(m);
//        //}


//        move--;
//        cur_state = 2;
//    }

//}

//int collect_limit = 100;
//int collect_cnt = 0;

//public void CollectStateGrid(MatchGrid g)
//{

//    if (collect_cnt >= collect_limit)
//    {
//        collect_cnt = 0;
//        notClear = true;
//        return;
//    }

//    if (cellContainer.freeCellContainer.Count > 0)
//    {
//        cur_state = 0;
//        return;
//    }

//    //if (g.GetFreeCells(true).Count > 0)
//    //{
//    //    cur_state = 0;
//    //    return;
//    //}


//    cellContainer.l_mgList = new List<MatchGroup>();
//    //cellContainer.CollectFalling(g);
//    cellContainer.CancelTweens();
//    cellContainer.CreateMatchGroups(3, false, g);
//    g.mgList = cellContainer.l_mgList;

//    cellContainer.CollectFalling(g);

//    //if(GetFreeCells(g).Count > 0)
//    //{
//    //    collect_cnt = 0;
//    //    cur_state = 0;
//    //}

//    //매치블록 지우기 얘내를 프리셀로 만들어야됨
//    cellContainer.freeCellContainer = new List<GridCell>();

//    for (int i = 0; i < cellContainer.l_mgList.Count; i++)
//    {

//        for(int j = 0; j < cellContainer.l_mgList[i].Length; j++)
//        {
//            cellContainer.freeCellContainer.Add(g.Cells[cellContainer.l_mgList[i].Cells[j].Row * Rows.Count() + cellContainer.l_mgList[i].Cells[j].Column]);
//        }

//        //4개 일땐?
//    }

//    if (cellContainer.freeCellContainer.Count > 0)
//    {
//        collect_cnt = 0;
//        cur_state = 0;
//    }

//    //int collected = 0;
//    //List<GridCell> gcL = new List<GridCell>();
//    //for (int i = 0; i < Cells.Count; i++)
//    //{

//    //    //Dictionary <string,int> a = new Dictionary<string,int>();
//    //    //a = Cells[2].DynamicObject;

//    //    //int a = Cells[2].DynamicObject.base

//    //    if (Cells[i].IsDynamicFree && !Cells[i].Blocked && !Cells[i].IsDisabled)
//    //    {
//    //        if (withPath && Cells[i].HaveFillPath() || !withPath)
//    //            gcL.Add(Cells[i]);
//    //    }
//    //}
//    //return gcL;


//    else cur_state = 1;
//}



//유전알고리즘
//////////////////////////////////////////////////////////////////


//System.Random random_ga;
//GeneticAlgorithm<char> ga;
//string targetString = "000001111100000111110000011111000001111100000111110000011111";
//string validCharacters = "000001111100000111110000011111000001111100000111110000011111";
//int populationSize = 5;
//float mutationRate = 5;
//int elitism = 5;
//int targetInt = 5;

//int numCharsPerText = 15000;
//string targetText;
//string bestText;
//string bestFitnessText;
//string numGenerationsText;
//Transform populationTextParent;
//string textPrefab;

//string gridsizeString;
//int howManyCorrect = 5;


//private char GetRandomCharacter()
//{
//    int i = random_ga.Next(validCharacters.Length);
//    return validCharacters[i];
//}

//private float FitnessFunction(int index)
//{
//    float score = 0;
//    DNA<char> dna = ga.Population[index];

//    //for (int i = 0; i < dna.Genes.Length; i++)
//    //{
//    //    if (dna.Genes[i] == targetString[i])
//    //    {
//    //        score += 1;
//    //    }
//    //}

//    //score /= targetString.Length;

//    //score = (Mathf.Pow(2, score) - 1) / (2 - 1);


//    for (int i = 0; i < dna.Genes.Length; i++)
//    {
//        if (dna.Genes[i] == '1')
//        {
//            score += 1;
//        }
//    }

//    //score = score / howManyCorrect;


//    return score;
//}




//internal void FillGrid(bool noMatches, MatchGrid g, Dictionary<int, TargetData> targets, Spawner spawnerPrefab, SpawnerStyle spawnerStyle, Transform GridContainer, Transform trans, LevelConstructSet IC)
//{
//float4 searchBoundary(float2 uv)
//{
//    const int constant = 3;

//    float2 mean = g_motionVectorMipMap.SampleLevel(g_samLinear, uv, MIPMAP_LEVEL).xy;
//    float2 squaMean = g_motionVectorMipMap.SampleLevel(g_samLinear, uv, MIPMAP_LEVEL).zw;

//    float2 standardDeviation = sqrt(squaMean - mean * mean);
//    float2 Max = mean + standardDeviation * constant;
//    float2 Min = mean - standardDeviation * constant;

//    float2 leftTopCorner = max(uv - Max, float2(0.0f, 0.0f));
//    float2 rightBottomCorner = min(uv - Min, float2(1.0f, 1.0f));

//    return float4(leftTopCorner, rightBottomCorner);
//}

//float constant = 3;
//float mean;
//float squareMean;
//float standardDevitaion = (float)Math.Sqrt(squareMean - mean * mean);


//평균 이동횟수가 이동한계횟수에 가깝고 표준편차가 원하는 이동한계 표준편차에 가까워야한다.

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////while (1 - Math.Abs(ga.BestFitness) > 0.1f && generation_cnt < generation_limit)
//{
//    List<MatchObject> MatchObjectCollect = new List<MatchObject>(Cells.Count);
//    for (int j = 0; j < Cells.Count; j++)
//    {
//        MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
//        Cells[j].SetObject(m);
//        MatchObjectCollect.Add(m);
//    }

//    for (int i = 0; i < populationSize; i++)
//    {
//        List<int> collect_Fitness = new List<int>();

//        for (int j = 0; j < repetition_limit; j++)
//        {
//            //for (int k = 0; k < Cells.Count; k++)
//            //{
//            //    Cells[k].SetObject(MatchObjectCollect[k]);

//            //    //if (ga.Population[i].Genes[k] == '1')
//            //    //{
//            //    //    BlockedObject m = sC.GetRandomBlockedObjectPrefab(LcSet, goSet);
//            //    //    Cells[k].SetObject(m);
//            //    //}
//            //}

//            cellContainer = new FitnessHelper();
//            move = move_limit;
//            move += 5;
//            cur_state = 0;
//            targetClear = false;

//            while (move > 0 || targetClear == false)
//            {
//                switch (cur_state)
//                {
//                    case 0:
//                        FillStateGrid(g);
//                        break;
//                    case 1:
//                        ShowStateGrid(g, targets);
//                        break;
//                    case 2:
//                        CollectStateGrid(g);
//                        break;
//                }
//            }

//            if (targetClear == true)
//            {
//                //move 가공 필요
//                collect_Fitness.Add(move);
//            }
//        }

//        ////fitness 입력
//        //ga.Population[i].Fitness = 1;
//    }

//    //ga.NewGeneration();
//    //generation_cnt++;
//    //break;
//}









//while (1 - Math.Abs(ga.BestFitness) > 0.1f)
//{
//    for (int i = 0; i < populationSize; i++)
//    {
//        move = 25;
//        cur_state = 0;
//        ////gene으로  cell 채우기
//        //List<int> collectID = new List<int>();
//        for (int j = 0; j < Cells.Count; j++)
//        {
//            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
//            Cells[j].SetObject(m);
//            //collectID.Add(m.ID);
//        }

//        for (int j = 0; j < populationSize; j++)
//        {
//            if (ga.Population[i].Genes[j] == '1')
//            {
//                BlockedObject m = sC.GetRandomBlockedObjectPrefab(LcSet, goSet);
//                Cells[j].SetObject(m);
//            }

//        }

//        //cellContainer = new FitnessHelper(Cells, collectID);
//        cellContainer = new FitnessHelper();

//        while (move > 0)
//        {
//            if (cur_state == 0) FillStateGrid(g);
//            else if (cur_state == 1) ShowStateGrid(g, targets);
//            else if (cur_state == 2) CollectStateGrid(g);
//        }
//    }

//    //if (cnt > 100)
//    //{
//    //    Debug.Log("Can't find BestFitness : " + ga.BestFitness);
//    //    break;
//    //}

//    ga.NewGeneration();

//    //UpdateText(ga.BestGenes, ga.BestFitness, ga.Generation, ga.Population.Count, (j) => ga.Population[j].Genes);
//    //cnt++;
//}

////if (cnt < 100) Debug.Log("find BestFitness : " + ga.BestFitness);




///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////







////FillState-------------------------------------------------------------------------------------//

//List<GridCell> gFreeCells = g.GetFreeCells(true); // Debug.Log("fill free count: " + gFreeCells.Count + " to collapse" );
//bool filled = false;
//if (gFreeCells.Count > 0)
//{
//    cellContainer.CreateFillPath(g);
//}

//while (gFreeCells.Count > 0)
//{
//    cellContainer.FillGridByStep(gFreeCells, () => { });
//    gFreeCells = g.GetFreeCells(true);
//    filled = true;
//}

////ShowEstimateState-------------------------------------------------------------------------------------//

//////getset을 써야하나
////Dictionary<int, TargetData> copied_targets = new Dictionary<int, TargetData>();
////board.Copy_target(copied_targets);

//cellContainer.CancelTweens();
//cellContainer.CreateMatchGroups(2, true, g);

////MixGrid

//List<List<int>> targetsInCell = new List<List<int>>();

//foreach (var currentCellsID in g.Cells)
//{
//    List<int> res = currentCellsID.GetGridObjectsIDs();

//    foreach (var item in targets)
//    {
//        if (!item.Value.Achieved)
//        {
//            if (item.Value.ID >= 1000 && item.Value.ID <= 1006)
//            {
//                if (item.Value.ID == res[0])
//                {
//                    targetsInCell.Add(new List<int> { currentCellsID.Row, currentCellsID.Column });
//                }
//            }
//        }
//    }
//}

////cellContainer.TargetSwap(targetsInCell);


////CollectState-------------------------------------------------------------------------------------//
//int collected = 0;

//if (g.GetFreeCells(true).Count > 0)
//{
//    return;
//}

//cellContainer.CollectFalling(g);
//cellContainer.CancelTweens();
//cellContainer.CreateMatchGroups(3, false, g);
//if(cellContainer.l_mgList.Count==0)
//{

//}


//if (CollectGroups.Length == 0) // no matches
//{
//    if (g.GetFreeCells(true).Count > 0)
//        MbState = MatchBoardState.Fill;
//    else
//    {
//        MbState = MatchBoardState.ShowEstimate;
//    }
//}

//else
//{
//    BeforeCollectBoardEvent?.Invoke(this);
//    MatchScore = scoreController.GetScoreForMatches(CollectGroups.Length);
//    CollectGroups.CollectMatchGroups(() => { GreatingMessage(); MbState = MatchBoardState.Fill; MatchScore = scoreController.GetScoreForMatches(0); });
//}







////first grid fill 시 3match 줄여줌
//if (noMatches)
//{
//    RemoveMatches();
//}












//CreateGroups11(2, true);

////if (EstimateGroups.Length > 0)
//{
//    //EstimateGroups.SwapEstimate();

//    List<List<int>> targetsInCell = new List<List<int>>();

//    foreach (var currentCellsID in CurrentGrid.Cells)
//    {
//        List<int> res = currentCellsID.GetGridObjectsIDs();

//        foreach (var item in CurTargets)
//        {
//            if (!item.Value.Achieved)
//            {
//                if (item.Value.ID >= 1000 && item.Value.ID <= 1006)
//                {
//                    if (item.Value.ID == res[0])
//                    {
//                        targetsInCell.Add(new List<int> { currentCellsID.Row, currentCellsID.Column });
//                    }
//                }
//            }
//        }
//    }


//    EstimateGroups.FOA_TargetSwap(targetsInCell);



//}




//CollectState();
//FillState();










//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////










//for (int i = 0; i < Cells.Count; i++)
//{
//    MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
//    Cells[i].SetObject(m);
//}


//for (int i = 0; i < gridsizeString.Length; i++)
//{
//    if (ga.BestGenes[i] == '1')
//    {
//        BlockedObject m = sC.GetRandomBlockedObjectPrefab(LcSet, goSet);
//        Cells[i].SetObject(m);
//    }

//}








//for (int i = 0; i < 3; i++)
//{
//    OverlayObject m = sC.GetRandomOverlayObjectPrefab(LcSet, goSet);

//    //m.BlockMovement = true;

//    Cells[i].SetObject(m);
//}

//// 됨 target count 체크 필요
//for (int i = 0; i < 3; i++)
//{
//    UnderlayObject m = sC.GetRandomUnderlayObjectPrefab(LcSet, goSet);

//    Cells[i].SetObject(m);
//}

//// 됨 target count 체크 필요
//for (int i = 0; i < 3; i++)
//{
//    FallingObject m = sC.GetRandomUgetFallingObjectPrefab(LcSet, goSet);

//    Cells[i].SetObject(m);
//}

//// 됨 target count 체크 필요
//for (int i = 0; i < 3; i++)
//{
//    HiddenObject m = sC.GetRandomHiddenObjectPrefab(LcSet, goSet);

//    Cells[i].SetObject(m);
//}





////getPickedObjectBlock
//for (int i = 0; i < Cells.Count; i++)
//{
//    if (!Cells[i].Blocked && !Cells[i].IsDisabled && !Cells[i].DynamicObject)
//    {
//        MatchObject m = sC.GetPickedObjectBlock(LcSet, goSet);
//        Cells[i].SetObject(m);
//    }

//}




////fill control test
//MatchObject m = sC.GetMainRandomObjectPrefab(LcSet, goSet);
//for (int i = 0; i < Cells.Count; i++)
//{
//    Cells[i].SetObject(m);
//}

//}



////DNA.cs------------------------------------------------------------------------------------------------------------------------//
//using System;

//public class DNA<T>
//{
//    public T[] Genes { get; private set; }
//    public float Fitness { get; private set; }

//    private Random random;
//    private Func<T> getRandomGene;
//    private Func<int, float> fitnessFunction;

//    public DNA(int size, Random random, Func<T> getRandomGene, Func<int, float> fitnessFunction, bool shouldInitGenes = true)
//    {
//        Genes = new T[size];
//        this.random = random;
//        this.getRandomGene = getRandomGene;
//        this.fitnessFunction = fitnessFunction;


//        if (shouldInitGenes)
//        {
//            for (int i = 0; i < Genes.Length; i++)
//            {
//                Genes[i] = getRandomGene();
//            }
//        }
//    }

//    public void SetFitness(int fit)
//    {
//        Fitness = fit;
//    }

//    //public float CalculateFitness(int index)
//    //{
//    //	Fitness = fitnessFunction(index);
//    //	return Fitness;
//    //}

//    public DNA<T> Crossover(DNA<T> otherParent)
//    {
//        DNA<T> child = new DNA<T>(Genes.Length, random, getRandomGene, fitnessFunction, shouldInitGenes: false);

//        for (int i = 0; i < Genes.Length; i++)
//        {
//            child.Genes[i] = random.NextDouble() < 0.5 ? Genes[i] : otherParent.Genes[i];
//        }

//        return child;
//    }

//    public void Mutate(float mutationRate)
//    {
//        for (int i = 0; i < Genes.Length; i++)
//        {
//            if (random.NextDouble() < mutationRate)
//            {
//                Genes[i] = getRandomGene();
//            }
//        }
//    }
//}

////GeneticAlgorithm.cs------------------------------------------------------------------------------------------------------------------------//

//public class GeneticAlgorithm<T>
//{
//    public List<DNA<T>> Population { get; private set; }
//    public int Generation { get; private set; }
//    public float BestFitness { get; private set; }
//    public T[] BestGenes { get; private set; }

//    public int Elitism;
//    public float MutationRate;

//    private List<DNA<T>> newPopulation;
//    private Random random;
//    private float fitnessSum;
//    private int dnaSize;
//    private Func<T> getRandomGene;
//    private Func<int, float> fitnessFunction;

//    public GeneticAlgorithm(int populationSize, int dnaSize, Random random, Func<T> getRandomGene, Func<int, float> fitnessFunction,
//        int elitism, float mutationRate = 0.01f)
//    {
//        Generation = 1;
//        Elitism = elitism;
//        MutationRate = mutationRate;
//        Population = new List<DNA<T>>(populationSize);
//        newPopulation = new List<DNA<T>>(populationSize);
//        this.random = random;
//        this.dnaSize = dnaSize;
//        this.getRandomGene = getRandomGene;
//        this.fitnessFunction = fitnessFunction;

//        BestGenes = new T[dnaSize];

//        for (int i = 0; i < populationSize; i++)
//        {
//            Population.Add(new DNA<T>(dnaSize, random, getRandomGene, fitnessFunction, shouldInitGenes: true));
//        }
//    }

//    public void NewGeneration(int numNewDNA = 0, bool crossoverNewDNA = false)
//    {
//        int finalCount = Population.Count + numNewDNA;

//        if (finalCount <= 0)
//        {
//            return;
//        }

//        if (Population.Count > 0)
//        {
//            CalculateFitness();
//            Population.Sort(CompareDNA);
//        }
//        newPopulation.Clear();

//        for (int i = 0; i < Population.Count; i++)
//        {
//            if (i < Elitism && i < Population.Count)
//            {
//                newPopulation.Add(Population[i]);
//            }
//            else if (i < Population.Count || crossoverNewDNA)
//            {
//                DNA<T> parent1 = ChooseParent();
//                DNA<T> parent2 = ChooseParent();

//                DNA<T> child = parent1.Crossover(parent2);

//                child.Mutate(MutationRate);

//                newPopulation.Add(child);
//            }
//            else
//            {
//                newPopulation.Add(new DNA<T>(dnaSize, random, getRandomGene, fitnessFunction, shouldInitGenes: true));
//            }
//        }

//        List<DNA<T>> tmpList = Population;
//        Population = newPopulation;
//        newPopulation = tmpList;

//        Generation++;
//    }

//    private int CompareDNA(DNA<T> a, DNA<T> b)
//    {
//        if (a.Fitness > b.Fitness)
//        {
//            return -1;
//        }
//        else if (a.Fitness < b.Fitness)
//        {
//            return 1;
//        }
//        else
//        {
//            return 0;
//        }
//    }

//    private void CalculateFitness()
//    {
//        fitnessSum = 0;
//        DNA<T> best = Population[0];

//        for (int i = 0; i < Population.Count; i++)
//        {
//            //fitnessSum += Population[i].CalculateFitness(i);

//            fitnessSum += Population[i].Fitness;

//            //if (Population[i].Fitness가 1에 가까울수록 best하게 
//            if (Population[i].Fitness > best.Fitness)
//            {
//                best = Population[i];
//            }
//        }

//        BestFitness = best.Fitness;
//        best.Genes.CopyTo(BestGenes, 0);
//    }

//    private DNA<T> ChooseParent()
//    {
//        double randomNumber = random.NextDouble() * fitnessSum;

//        for (int i = 0; i < Population.Count; i++)
//        {
//            if (randomNumber < Population[i].Fitness)
//            {
//                return Population[i];
//            }

//            randomNumber -= Population[i].Fitness;
//        }

//        return null;
//    }
//}



//230720 GetMatch3Level

//public GameBoard board;
//public MatchGrid cur_grid;
//public List<MatchGroup> mgList;
//int cur_state;
//bool targetClear = false;
//bool notClear = false;
//public FitnessHelper cellContainer { get; private set; }

//int num_move;
//int tryCnt;
////List<GameObject> cellList = new List<GameObject>();
////for (int index = 0; index < g.Cells.Count; index++) cellList.Add(g.Cells[index].DynamicObject);

//void GetMatch3Level(int populationSize, MatchGrid g, Dictionary<int, TargetData> targets, Spawner spawnerPrefab, SpawnerStyle spawnerStyle, Transform GridContainer, Transform trans, LevelConstructSet IC, MatchGrid tmp_g)
//{
//    SpawnController sC = SpawnController.Instance;

//    int repetition_limit = 2;
//    int generation_limit = 2;
//    int move_limit = 20;

//    board = new GameBoard();
//    board.MakeBoard(g, spawnerPrefab, spawnerStyle, GridContainer, trans, IC);

//    for (int generation_idx = 0; generation_idx < generation_limit; generation_idx++)
//    {
//        //--Make grid------------------------------------------------------------------//

//        //List<GridCell> tmp_cells = new List<GridCell>();
//        //tmp_cells = g.Cells;

//        for (int population_idx = 0; population_idx < populationSize; population_idx++)
//        {
//            //g.Cells = tmp_cells;
//            //g = tmp_g;

//            g.mgList = new List<MatchGroup>();

//            //List<GameObject> cellList = new List<GameObject>();
//            //for (int index = 0; index < g.Cells.Count; index++)
//            //{
//            //    cellList.Add(g.Cells[index].Blocked);
//            //}

//            for (int i = 0; i < g.Cells.Count; i++)
//            {
//                g.Cells[i].DestroyGridObjects();

//                MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
//                g.Cells[i].SetObject(m);

//                if (ga.population[population_idx].genes[i] == '1')
//                {
//                    BlockedObject b = sC.GetRandomBlockedObjectPrefab(LcSet, goSet);
//                    g.Cells[i].SetObject(b);
//                }
//            }

//            //List<GameObject> cellList = new List<GameObject>();
//            //for (int index = 0; index < g.Cells.Count; index++) cellList.Add(g.Cells[index].DynamicObject);

//            //--Estimate_feasible------------------------------------------------------------------//
//            cellContainer = new FitnessHelper();
//            cellContainer.CreateFillPath(g);
//            int infeasible_cell_cnt = 0;

//            //장애물 블록인데 패스가 있는경우, 불가능하면 null fillPathToSpawner

//            for (int i = g.Columns.Count(); i < g.Cells.Count; i++)
//            {
//                if (g.Cells[i].Blocked == null && g.Cells[i].fillPathToSpawner == null)
//                {
//                    if (g.Cells[g.Cells[i].Column + (g.Cells[i].Row - 1) * Rows.Count].Blocked != null)
//                    {
//                        infeasible_cell_cnt++;
//                    }
//                }

//            }

//            //List<int> infeasibleList = new List<int>();

//            //for (int i = 0; i < g.Columns.Count(); i++)
//            //{
//            //    if (g.Cells[i].fillPathToSpawner != null)
//            //    {
//            //        if (g.Cells[i].Blocked == null) infeasibleList.Add(g.Cells[i].fillPathToSpawner.Count());
//            //        else infeasibleList.Add(0);
//            //    }

//            //    else infeasibleList.Add(-1);
//            //}

//            //for (int i = g.Columns.Count(); i < g.Cells.Count; i++)
//            //{
//            //    if (g.Cells[i].fillPathToSpawner != null)
//            //    {
//            //        if (g.Cells[i].Blocked == null) infeasibleList.Add(g.Cells[i].fillPathToSpawner.Count());
//            //        else infeasibleList.Add(0);
//            //    }

//            //    else infeasibleList.Add(-1);
//            //}

//            //int a = 0;

//            //CheckCount(ref a);

//            //List<int> infeasibleList = new List<int>();

//            //for (int i = 0; i < g.Columns.Count(); i++)
//            //{
//            //    if (g.Cells[i].fillPathToSpawner == null) infeasibleList.Add(-1);
//            //    else if (g.Cells[i].Blocked == null) infeasibleList.Add(9);
//            //    else infeasibleList.Add(g.Cells[i].fillPathToSpawner.Count());
//            //}

//            //for (int i = g.Columns.Count(); i < g.Cells.Count; i++)
//            //{
//            //    if(g.Cells[i].fillPathToSpawner == null && g.Cells[i].Blocked != null)
//            //    {
//            //        infeasible_cell_cnt++;
//            //        infeasibleList.Add(-1);
//            //    }

//            //    else if (g.Cells[i].Blocked == null) infeasibleList.Add(9);

//            //    else infeasibleList.Add(g.Cells[i].fillPathToSpawner.Count());

//            //}

//            //fillpath가 있고 


//            //List<int> blockList = new List<int>();
//            //for (int index = 0; index < g.Cells.Count; index++)
//            //{
//            //    if (g.Cells[index].Blocked != null)
//            //    {
//            //        blockList.Add(g.Cells[index].Blocked.ID);
//            //    }

//            //    else blockList.Add(0);
//            //}

//            //infeasible_cell_cnt = 0;

//            //--Infeasible Population------------------------------------------------------------------//
//            if (infeasible_cell_cnt > 0)
//            {
//                ga.population[population_idx].isInfeasible = true;
//                ga.population[population_idx].SetFitness(infeasible_cell_cnt);
//            }

//            //--Feasible Population------------------------------------------------------------------//
//            else
//            {
//                tryCnt = 0;

//                List<int> fitness_container = new List<int>();

//                for (int repetition_idx = 0; repetition_idx < repetition_limit; repetition_idx++)
//                {
//                    cur_grid = g;
//                    num_move = move_limit;
//                    num_move += 5;
//                    cur_state = 0;
//                    targetClear = false;
//                    notClear = false;

//                    foreach (var item in targets) item.Value.InitCurCount();

//                    while (num_move > 0 && targetClear == false && notClear == false)
//                    {
//                        switch (cur_state)
//                        {
//                            case 0:
//                                FillState_CurGrid(cur_grid, sC);
//                                break;
//                            case 1:
//                                ShowState_CurGrid(cur_grid, targets, sC, trans);
//                                break;
//                            case 2:
//                                CollectState_CurGrid(cur_grid, targets);
//                                break;
//                        }

//                        if (tryCnt > 100) break;
//                        tryCnt++;
//                    }

//                    if (targetClear == true) fitness_container.Add(num_move);
//                    //if (targetClear == true) fitness_container.Add(move - move_limit);
//                    else fitness_container.Add(0);
//                }

//                ga.population[population_idx].CalculateFitness(fitness_container, 15, 0.5);

//                //int fitness_cnt = 0;
//                //int fitness_sum = 0;

//                //for (int i = 0; i < fitness_container.Count; i++)
//                //{
//                //    if (fitness_container[i] != 0)
//                //    {
//                //        fitness_sum += fitness_container[i];
//                //        fitness_cnt++;
//                //    }
//                //}

//                //if (fitness_cnt == 0 || fitness_sum == 0) ga.population[population_idx].SetFitness(0);
//                //else ga.population[population_idx].SetFitness(fitness_sum / fitness_cnt);

//                ////Debug.Log("Fitness =========" + ga.Population[population_idx].Fitness + "------------------------------------");
//            }
//        }
//        ga.NewGeneration();

//        double threshold = 0.1;

//        if (ga.bestFitness > 1 - threshold)
//        {
//            break;
//        }
//    }
//}

//public void CheckCount(ref int a)
//{
//    a++;
//}



//public class GeneticAlgorithm<T>
//{
//	public List<DNA<T>> population;
//	public List<DNA<T>> feasible_population;
//	public List<DNA<T>> infeasible_population;

//	public int generation;
//    public int elitism;
//    public float mutationRate;
//    private Random random;
//    private int dnaSize;
//    public double bestFitness;

//    public double feasibleFitnessSum;
//    public double infeasibleFitnessSum;


//    public int generation_limit = 10;
//    public int repetition_limit = 5;
//    public int move_limit = 20;

//    public double target_fitness = 0.9;
//    public int target_move = 15;
//    public double target_std = 0.5;

//    public GeneticAlgorithm(int populationSize, int dnaSize, Random random, Func<T> getRandomGene,
//		int elitism, float mutationRate = 0.01f)
//	{
//        population = new List<DNA<T>>(populationSize);
//        feasible_population = new List<DNA<T>>();
//        infeasible_population = new List<DNA<T>>();
//        generation = 1;
//		this.elitism = elitism;
//		this.mutationRate = mutationRate;
//		this.random = random;
//		this.dnaSize = dnaSize;

//        for (int i = 0; i < populationSize; i++)
//        {
//            population.Add(new DNA<T>(dnaSize, random, getRandomGene, shouldInitGenes: true));
//        }
//    }