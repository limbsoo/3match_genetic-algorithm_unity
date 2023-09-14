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


//0728
//public void Calculate_feasible_fitness(List<int> num_move_container, double target_move, double target_std)
//{



//    ////double standardDeviation = Math.Sqrt(varianceSum / (num_move_container.Count - 1));

//    //double alpha = 0.5;

//    //double aveageExp = Math.Exp(-Math.Abs(average));
//    //double standardDeviationExp = Math.Exp(-Math.Abs(standardDeviation));

//    //fitness = alpha * aveageExp + (1 - alpha) * standardDeviationExp;





//    //double standardDeviation = Math.Sqrt(varianceSum / (num_move_container.Count));
//    ////double standardDeviation = Math.Sqrt(varianceSum / (num_move_container.Count - 1));











//    //if (double.TryParse()



//    //int exponent = 100; // 지수 함수의 지수 값 (예: 100)
//    //BigDecimal result = BigDecimal.Pow(BigDecimal.Exp(BigDecimal.One), exponent);





//    //fitness = Math.Round(Math.Exp(-Math.Abs(average_move - target_move)), 14);

//    //Math.Round(fitness, 5);


//    //if (fitness < 0 || fitness > 1)
//    //{
//    //    fitness = double.MinValue;
//    //}




//    //double[] differenceData = new double[num_move_container.Count];

//    //for (int i = 0; i < num_move_container.Count; i++)
//    //{
//    //    differenceData[i] = -Math.Abs(num_move_container[i] - target_move);
//    //}

//    //double min = double.MaxValue;
//    //double max = double.MinValue;

//    //for (int i = 0; i < differenceData.Length; i++)
//    //{
//    //    if (differenceData[i] < min) min = differenceData[i];

//    //    if (differenceData[i] > max) max = differenceData[i];
//    //}


//    //double[] normalizedData = new double[differenceData.Length];

//    //for (int i = 0; i < differenceData.Length; i++)
//    //{
//    //    normalizedData[i] = (differenceData[i] - min) / max - min;
//    //}














//    //double[] differenceData = new double[num_move_container.Count];

//    //for (int i = 0; i < num_move_container.Count; i++)
//    //{
//    //    differenceData[i] = -Math.Abs(num_move_container[i] - target_move);
//    //}

//    //double max = differenceData[0];

//    //// 최대값 찾기
//    //foreach (double value in differenceData)
//    //{
//    //    if (value > max) max = value;
//    //}

//    //// 지수함수를 사용하여 정규화된 데이터 계산
//    //double[] normalizedData = new double[differenceData.Length];
//    //for (int i = 0; i < differenceData.Length; i++)
//    //{
//    //    if(max == 0) normalizedData[i] = 0;

//    //    else normalizedData[i] = Math.Exp(differenceData[i] / max);
//    //}

//    //double mean = 0.0;
//    //double sumSquaredDifferences = 0.0;

//    //// 평균 계산
//    //foreach (double value in normalizedData)
//    //{
//    //    mean += value;
//    //}
//    //mean /= normalizedData.Length;

//    //average_move = mean;

//    //// 각 데이터 값과 평균의 차이 제곱의 합 계산
//    //foreach (double value in normalizedData)
//    //{
//    //    double difference = value - mean;
//    //    sumSquaredDifferences += difference * difference;
//    //}

//    //// 표준편차 계산
//    //double standardDeviation = Math.Sqrt(sumSquaredDifferences / (normalizedData.Length - 1));
//    //sd = standardDeviation;

//    //double alpha = 0.5;
//    ////fitness = alpha * Math.Abs(mean) + (1 - alpha) * Math.Abs(standardDeviation - target_std);

//    //fitness = Math.Abs(mean);


























//    //double mean = 0.0;
//    //double sumSquaredDifferences = 0.0;

//    //foreach (double value in num_move_container) mean += value;

//    //mean /= num_move_container.Count;

//    //foreach (double value in num_move_container)
//    //{
//    //    double difference = value - mean;
//    //    sumSquaredDifferences += difference * difference;
//    //}

//    //double standardDeviation = Math.Sqrt(sumSquaredDifferences / (num_move_container.Count - 1));

//    //double absMove = -Math.Abs(mean - target_move);
//    //double absStandard = -Math.Abs(standardDeviation - target_std);

//    //double meanExp = 0;

//    //if (Math.Abs (absMove) > 9) meanExp = 0;
//    //else meanExp = Math.Exp(absMove);

//    //double example1 = Math.Exp(0);
//    //double example2 = Math.Exp(1);
//    //double example3 = Math.Exp(10);
//    //double example4 = Math.Exp(-1);
//    //double example5 = Math.Exp(-2);
//    //double example6 = Math.Exp(-5);
//    //double example7 = Math.Exp(-10);
//    //double example8 = Math.Exp(-15);
//    //double example9 = Math.Exp(-30);
//    //double example10 = Math.Exp(-9);

//    //double standardDeviationExp = Math.Exp(absStandard);

//    ////double meanExp = Math.Exp(-Math.Abs(mean - target_move));
//    ////double standardDeviationExp = Math.Exp(-Math.Abs(standardDeviation - target_std));

//    //double alpha = 0.5;
//    //fitness = alpha * meanExp + (1 - alpha) * standardDeviationExp;






//    //double sum = 0;

//    //foreach (var c in num_move_container) sum += c;

//    //double average = sum / num_move_container.Count;

//    //double variance = 0;
//    //double varianceSum = 0;

//    //foreach (var c in num_move_container)
//    //{
//    //    varianceSum += Math.Pow(c - average, 2);
//    //}

//    //double standardDeviation = Math.Sqrt(varianceSum / (num_move_container.Count));
//    ////double standardDeviation = Math.Sqrt(varianceSum / (num_move_container.Count - 1));

//    //double alpha = 0.5;

//    //double aveageExp = Math.Exp(-Math.Abs(average));
//    //double standardDeviationExp = Math.Exp(-Math.Abs(standardDeviation));

//    //fitness = alpha * aveageExp + (1 - alpha) * standardDeviationExp;



//    //foreach (var c in num_move_container)
//    //{
//    //    if (c != 0)
//    //    {
//    //        sum += c;
//    //        squaMean += Math.Pow(c, 2);
//    //        validCount++;
//    //    }
//    //}

//    //double mean = 0;
//    //double squaMean = 0;
//    //double variance = 0;

//    //if (sum != 0 || validCount != 0)
//    //{
//    //    mean = (double)sum / validCount;
//    //    squaMean = (double)squaMean / validCount;
//    //}

//    //average_move = mean;

//    //double standardDeviation = 0;

//    //double variance = squaMean - (mean * mean);

//    //standardDeviation = Math.Sqrt(variance);

//    //double alpha = 0.5;
//    //fitness = alpha * Math.Exp(-Math.Abs(mean - target_move))
//    //        + (1 - alpha) * Math.Exp(-Math.Abs(standardDeviation - target_std));








//    //int validCount = 0;
//    //int sum = 0;

//    ////- CalculateMean ------------------------/

//    //foreach(var c in num_move_container)
//    //{
//    //    if(c != 0)
//    //    {
//    //        sum += c;
//    //        validCount++;
//    //    }
//    //}

//    //double mean = 0;

//    //if (sum != 0 || validCount != 0) mean = (double)sum / validCount;

//    //average_move = mean;

//    ////- CalculateStandardDeviation ------------------------/

//    //float standardDeviation = 0;

//    //float squaMean = 


//    //    // float2 standardDeviation = sqrt(squaMean - mean * mean);//제곱의 평균 -평균의 제곱 : 표준편차 제곱 =>분산




//    ////double sumOfSquaredDifferences = 0;

//    ////foreach (var c in num_move_container)
//    ////{
//    ////    if (c != 0) sumOfSquaredDifferences += Math.Pow(c - mean, 2);
//    ////}

//    ////double variance = 0;




//    ////if (sumOfSquaredDifferences != 0 || validCount != 0) variance = sumOfSquaredDifferences / validCount;

//    ////double standardDeviation = Math.Sqrt(variance);

//    ////sd = standardDeviation;

//    ////- CalculateFitness ------------------------/

//    //double alpha = 0.5;
//    //fitness = alpha * Math.Exp(-Math.Abs(mean - target_move))
//    //    + (1 - alpha) * Math.Exp(-Math.Abs(standardDeviation - target_std));
//}



////664



////306
//i == 10 ||
//i == 17 || i == 18 || i == 19 ||
//i == 26

////289
//i == 9 || i == 10 || i == 13 || i == 14 ||
//i == 17 || i == 18 || i == 21 || i == 22 ||
//i == 25 || i == 26 || i == 30 ||
//i == 33 || i == 34 || i == 38

////553
//i == 6  ||
//i == 11 || i == 12 ||
//i == 17 || i == 18 || i == 19 || i == 20 ||
//i == 25 || i == 26 || i == 27 || i == 28 ||
//i == 33 || i == 34 || i == 35 || i == 36

////364
//i == 9 || i == 10 || i == 13 || i == 14 ||
//i == 17 || i == 18 ||
//i == 25 || i == 26 || i == 29 || i == 30


////346
//i == 9 || i == 10 || i == 13 || i == 14 ||
//i == 17 || i == 18 || i == 21 || i == 22 ||
//i == 25 || i == 26 || i == 29 || i == 30 

////256
//i == 9 || i == 10 || i == 13 || i == 14 ||
//i == 17 || i == 18 || i == 21 || i == 22 ||
//i == 25 || i == 26 || i == 29 || i == 30 ||
//i == 33 || i == 34 || i == 37 || i == 38

////624
//i == 9

////548
//i == 9 || i == 10 ||
//i == 17 || i == 18

////440
//i == 9 || i == 10 || i == 11 ||
//i == 17 || i == 18 || i == 19 ||
//i == 25 || i == 26 || i == 27

////300
//i == 9 || i == 10 || i == 11 || i == 12 ||
//i == 17 || i == 18 || i == 19 || i == 20 ||
//i == 25 || i == 26 || i == 27 || i == 28 ||
//i == 33 || i == 34 || i == 35 || i == 36

////488
//i == 10 ||
//i == 17 || i == 18 || i == 19 ||
//i == 26

////306
//i == 10 ||
//i == 17 || i == 18 || i == 19 ||
//i == 26 || i == 29 ||
//i == 36 || i == 37 || i == 38 ||
//i == 45

////289
//i == 9 || i == 10 || i == 13 || i == 14 ||
//i == 17 || i == 18 || i == 21 || i == 22 ||
//i == 25 || i == 26 || i == 30 ||
//i == 33 || i == 34 || i == 38

////256
//i == 9 || i == 10 || i == 13 || i == 14 ||
//i == 17 || i == 18 || i == 21 || i == 22 ||
//i == 25 || i == 26 || i == 29 || i == 30 ||
//i == 33 || i == 34 || i == 37 || i == 38


////200
//i == 10 ||
//i == 17 || i == 18 || i == 19 ||
//i == 26 || i == 29 ||
//i == 33 || i == 36 || i == 37 || i == 38 ||
//i == 40 || i == 41 || i == 42 || i == 45 ||
//i == 49

////185
//i == 9 || i == 10 ||
//i == 17 || i == 18 ||
//i == 25 || i == 26 || i == 29 || i == 30 ||
//i == 33 ||
//i == 43 || i == 44 ||
//i == 48 || i == 49 || i == 55 ||
//i == 56 || i == 57 || i == 58 || i == 61 || i == 62 || i == 63


////168
//i == 10 ||
//i == 17 || i == 18 || i == 19 ||
//i == 26 || i == 29 ||
//i == 33 || i == 36 || i == 37 || i == 38 ||
//i == 40 || i == 41 || i == 42 || i == 45 ||
//i == 49 || i == 55 ||
//i == 62 || i == 63

////113

//i == 10 || i == 14 || i == 15 ||
//i == 17 || i == 18 || i == 19 || i == 23 ||
//i == 26 || i == 29 ||
//i == 33 || i == 36 || i == 37 || i == 38 ||
//i == 40 || i == 41 || i == 42 || i == 45 ||
//i == 49 || i == 55 ||
//i == 62 || i == 63


////110
//i == 7 ||
//i == 10 || i == 14 || i == 15 ||
//i == 17 || i == 18 || i == 19 || i == 23 ||
//i == 26 || i == 29 ||
//i == 33 || i == 36 || i == 37 || i == 38 ||
//i == 40 || i == 41 || i == 42 || i == 45 ||
//i == 49 || i == 55 ||
//i == 62 || i == 63

////92
//i == 9 || i == 10 || 
//i == 17 || i == 18 || 
//i == 25 || i == 26 || i == 29 || i == 30 ||
//i == 33 || 
//i == 43 || i == 44 ||
//i == 48 || i == 49 || i == 55 ||
//i == 56 || i == 57 || i == 58 || i == 61 || i == 62 || i == 63

////70
//i == 9 || i == 10 || i == 13 || i == 14 ||
//i == 17 || i == 18 || i == 21 || i == 22 ||
//i == 25 || i == 26 || i == 29 || i == 30 ||
//i == 32 || i == 33 || i == 38 || i == 39 ||
//i == 40 || i == 41 || i == 43 || i == 44 || i == 46 || i == 47 ||
//i == 48 || i == 49 || i == 54 || i == 55 ||
//i == 56 || i == 57 || i == 58 || i == 59 || i == 60 || i == 61 || i == 62 || i == 63 




//if (
//    //i == 9 ||
//    //i == 17 ||
//    //i == 25 ||
//    //i == 33 ||
//    //i == 41 ||
//    //i == 49 ||
//    //i == 57 ||

//    i == 11 
//    //i == 19 ||
//    //i == 27 ||
//    //i == 35 ||
//    //i == 43 ||
//    //i == 51 ||
//    //i == 59


//    //i == 1 ||
//    //i == 9 ||
//    //i == 17 ||
//    //i == 25 ||
//    //i == 33 ||
//    //i == 41 ||
//    //i == 49 ||
//    //i == 57

//    //i == 2 ||
//    //i == 10 ||
//    //i == 18 ||
//    //i == 26 ||
//    //i == 34 ||
//    //i == 42 ||
//    //i == 50 ||
//    //i == 58

//    //i == 3 || i == 6 ||
//    //i == 10 || i == 11 || i == 14 ||
//    //i == 17 || i == 18 || i == 19 || i == 22 ||
//    //i == 25 || i == 26 || i == 30 ||
//    //i == 32 || i == 33 || i == 38 ||
//    //i == 45 ||
//    //i == 52 ||
//    //i == 59

//    )
//{
//    BlockedObject b = sC.GetRandomBlockedObjectPrefab(LcSet, goSet);
//    mh.grid.Cells[i].SetObject1(b);

//}



//public void showState(DNA<char> p, SpawnController sC, Transform trans)
//{

//    foreach (var item in mh.curTargets)
//    {
//        if (item.Value.Achieved) p.targetClear = true;
//        else
//        {
//            p.targetClear = false;
//            break;
//        }
//    }
//    if (p.targetClear) return;

//    int mix_cnt = 0;

//    while (!estimateMaxedOut(p, ref mix_cnt, 100))
//    {
//        //mh.cancelTweens(mh.grid);
//        mh.createMatchGroups1(2, true, mh.grid, p);

//        if (mh.grid.mgList.Count == 0)
//        {
//            mh.mixGrid(null, mh.grid, trans);
//            mix_cnt++;
//        }
//        else break;
//    }

//    if (p.maxedOut) return;

//    else
//    {
//        List<MatchGroup> matchedTarget = new List<MatchGroup>();

//        foreach (var mc in mh.grid.mgList)
//        {
//            foreach (var item in mh.curTargets)
//            {
//                List<int> mg_cell = mc.Cells[0].GetGridObjectsIDs();

//                if (mg_cell[0] == item.Key)
//                {
//                    matchedTarget.Add(mc);
//                    break;
//                }
//            }
//        }

//        if (matchedTarget.Count == 0) mh.grid.mgList[0].SwapEstimate();

//        else
//        {
//            //int target_num = 0;
//            //mh.grid.mgList[target_num].SwapEstimate();
//            //mgList에서 랜덤하게 해서 변종확률?

//            p.shortCutCnt++;

//            int number = Random.Range(0, matchedTarget.Count - 1);

//            for (int i = 0; i < mh.grid.mgList.Count; i++)
//            {
//                if (matchedTarget[number].Cells[0].DynamicObject == mh.grid.mgList[i].Cells[0].DynamicObject)
//                {
//                    mh.grid.mgList[i].SwapEstimate();
//                    break;
//                }
//            }

//        }



//        p.numMove--;
//        p.curState = 2;




//        //List<List<int>> targetsInCell = new List<List<int>>();

//        //foreach (var currentCellsID in mh.grid.Cells)
//        //{
//        //    List<int> res = currentCellsID.GetGridObjectsIDs();

//        //    //Debug.Log("currentCellsID ======Row" + currentCellsID.Row + "---------------------------Column" + currentCellsID.Column);

//        //    foreach (var item in mh.curTargets)
//        //    {
//        //        if (!item.Value.Achieved)
//        //        {
//        //            if (item.Value.ID >= 1000 && item.Value.ID <= 1006)
//        //            {
//        //                if (item.Value.ID == res[0]) targetsInCell.Add(new List<int> { currentCellsID.Row, currentCellsID.Column });
//        //            }
//        //        }
//        //    }
//        //}

//        //List<int> collectMatch = new List<int>();
//        ////셀판별추가
//        //for (int i = 0; i < mh.grid.mgList.Count; i++)
//        //{
//        //    bool isTargetBlock = false;

//        //    for (int j = 0; j < targetsInCell.Count; j++)
//        //    {
//        //        if (mh.grid.mgList[i].Cells[0].Row == targetsInCell[j][0] && mh.grid.mgList[i].Cells[0].Column == targetsInCell[j][1])
//        //        {
//        //            isTargetBlock = true;
//        //            break;
//        //        }

//        //        if (mh.grid.mgList[i].Cells[1].Row == targetsInCell[j][0] && mh.grid.mgList[i].Cells[1].Column == targetsInCell[j][1])
//        //        {
//        //            isTargetBlock = true;
//        //            break;
//        //        }
//        //    }

//        //    if (isTargetBlock) collectMatch.Add(i);
//        //}

//        //int target_num = 0;

//        //if (collectMatch.Count <= 0)
//        //{
//        //    mh.grid.mgList[0].SwapEstimate();
//        //    target_num = 0;
//        //}

//        //else
//        //{
//        //    int number = Random.Range(0, collectMatch.Count - 1);
//        //    mh.grid.mgList[collectMatch[number]].SwapEstimate();
//        //    target_num = collectMatch[number];
//        //}

//    }
//}










//230913
/////////////////////////////////////////////////////////////////////////////////////////////////////
///
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Dynamic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using Unity.Mathematics;
//using Unity.VisualScripting;
//using UnityEditor;
//using UnityEditor.VersionControl;
//using UnityEditorInternal.VersionControl;
//using UnityEngine;
//using UnityEngine.Rendering;
//using UnityEngine.UI;
//using UnityEngine.UIElements;
//using static Mkey.Match_Helper;
//using static UnityEditor.PlayerSettings;
//using static UnityEngine.GraphicsBuffer;
//using Random = UnityEngine.Random;


//namespace Mkey
//{
//    public class MatchGrid
//    {
//        private GameObjectsSet goSet;

//        public List<Column<GridCell>> Columns { get; private set; }
//        public List<GridCell> Cells { get; private set; }

//        /// <summary>
//        /// ///////////////////////////////////////////////////
//        /// </summary>
//        public List<GridCell> CellsID { get; private set; }
//        /// ///////////////////////////////////////////////////


//        public List<Row<GridCell>> Rows { get; private set; }
//        public Transform Parent { get; private set; }
//        private int sortingOrder;
//        private GameMode gMode;
//        private int vertSize;
//        private int horSize;
//        private GameObject prefab;
//        private float yStart; // Camera.main.orthographicSize - radius
//        private float yStep;
//        private float xStep;
//        private int yOffset;
//        private Vector2 cellSize;
//        private float cOffset;
//        public bool haveFillPath = false;
//        public Vector2 Step { get { return new Vector2(xStep, yStep); } }
//        public LevelConstructSet LcSet { get; private set; }

//        public MatchGrid(LevelConstructSet lcSet, GameObjectsSet goSet, Transform parent, int sortingOrder, GameMode gMode)
//        {
//            this.LcSet = lcSet;
//            this.Parent = parent;
//            this.gMode = gMode;
//            this.sortingOrder = sortingOrder;
//            Debug.Log("new grid " + lcSet.name);

//            vertSize = lcSet.VertSize;
//            horSize = lcSet.HorSize;
//            this.goSet = goSet;
//            prefab = goSet.gridCellEven;
//            cellSize = prefab.GetComponent<BoxCollider2D>().size;

//            float deltaX = lcSet.DistX;
//            float deltaY = lcSet.DistY;
//            float scale = lcSet.Scale;
//            parent.localScale = new Vector3(scale, scale, scale);

//            Cells = new List<GridCell>(vertSize * horSize);
//            Rows = new List<Row<GridCell>>(vertSize);

//            yOffset = 0;
//            xStep = (cellSize.x + deltaX);
//            yStep = (cellSize.y + deltaY);

//            cOffset = (1 - horSize) * xStep / 2.0f; // offset from center by x-axe
//            yStart = (vertSize - 1) * yStep / 2.0f;

//            //instantiate cells
//            for (int i = 0; i < vertSize; i++)
//            {
//                AddRow();
//            }

//            //readLevelConstructSet
//            SetObjectsData(lcSet, gMode);

//            //newLevelConstruct
//            //AssignObjectData(lcSet, gMode);


//            Debug.Log("create cells: " + Cells.Count);
//        }

//        public void Rebuild(GameObjectsSet mSet, GameMode gMode)
//        {
//            Debug.Log("rebuild ");

//            this.LcSet = LcSet;
//            vertSize = LcSet.VertSize;
//            horSize = LcSet.HorSize;

//            float deltaX = LcSet.DistX;
//            float deltaY = LcSet.DistY;
//            float scale = LcSet.Scale;
//            Parent.localScale = new Vector3(scale, scale, scale);

//            this.goSet = mSet;
//            Cells.ForEach((c) => { c.DestroyGridObjects(); });

//            List<GridCell> tempCells = Cells;
//            Cells = new List<GridCell>(vertSize * horSize + horSize);
//            Rows = new List<Row<GridCell>>(vertSize);

//            xStep = (cellSize.x + deltaX);
//            yStep = (cellSize.y + deltaY);

//            cOffset = (1 - horSize) * xStep / 2.0f; // offset from center by x-axe
//            yStart = (vertSize - 1) * yStep / 2.0f;

//            // create rows 
//            GridCell cell;
//            Row<GridCell> row;
//            int cellCounter = 0;
//            int ri = 0;
//            Sprite sRE = mSet.gridCellEven.GetComponent<SpriteRenderer>().sprite;
//            Sprite sRO = mSet.gridCellOdd.GetComponent<SpriteRenderer>().sprite;

//            for (int i = 0; i < vertSize; i++)
//            {
//                bool isEvenRow = (i % 2 == 0);
//                row = new Row<GridCell>(horSize);

//                for (int j = 0; j < row.Length; j++)
//                {
//                    bool isEvenColumn = (j % 2 == 0);
//                    Vector3 pos = new Vector3(j * xStep + cOffset, yStart - i * yStep, 0);

//                    if (tempCells != null && cellCounter < tempCells.Count)
//                    {
//                        cell = tempCells[cellCounter];
//                        cell.gameObject.SetActive(true);
//                        cell.transform.localPosition = pos;
//                        cellCounter++;
//                        SpriteRenderer sR = cell.GetComponent<SpriteRenderer>();
//                        if (sR)
//                        {
//                            sR.enabled = true;
//                            if (isEvenRow) sR.sprite = (!isEvenColumn) ? sRO : sRE;
//                            else sR.sprite = (isEvenColumn) ? sRO : sRE;
//                        }
//                    }
//                    else
//                    {
//                        if (isEvenRow)
//                            cell = UnityEngine.Object.Instantiate((!isEvenColumn) ? mSet.gridCellOdd : mSet.gridCellEven).GetComponent<GridCell>();
//                        else
//                            cell = UnityEngine.Object.Instantiate((isEvenColumn) ? mSet.gridCellOdd : mSet.gridCellEven).GetComponent<GridCell>();
//                        cell.transform.parent = Parent;
//                        cell.transform.localPosition = pos;
//                        cell.transform.localScale = Vector3.one;
//                    }


//                    Cells.Add(cell);
//                    row[j] = cell;
//                }
//                Rows.Add(row);
//                ri++;
//            }

//            // destroy not used cells
//            if (cellCounter < tempCells.Count)
//            {
//                for (int i = cellCounter; i < tempCells.Count; i++)
//                {
//                    UnityEngine.Object.Destroy(tempCells[i].gameObject);
//                }
//            }

//            // cache columns
//            Columns = new List<Column<GridCell>>(horSize);
//            Column<GridCell> column;
//            for (int c = 0; c < horSize; c++)
//            {
//                column = new Column<GridCell>(Rows.Count);
//                for (int r = 0; r < Rows.Count; r++)
//                {
//                    column[r] = Rows[r][c];
//                }
//                Columns.Add(column);
//            }

//            for (int r = 0; r < Rows.Count; r++)
//            {
//                for (int c = 0; c < horSize; c++)
//                {
//                    Rows[r][c].Init(r, c, Columns[c], Rows[r], gMode);
//                }
//            }
//            SetObjectsData(LcSet, gMode);

//            Debug.Log("rebuild cells: " + Cells.Count);
//        }

//        /// <summary>
//        /// set objects data from featured list to grid
//        /// </summary>
//        /// <param name="featCells"></param>
//        /// <param name="gMode"></param>
//        private void SetObjectsData(LevelConstructSet lcSet, GameMode gMode)
//        {
//            //MainLCSet에서 isDisabled 전달

//            if (lcSet.cells != null)
//                foreach (var c in lcSet.cells)
//                {
//                    if (c != null && c.gridObjects != null)
//                    {
//                        foreach (var o in c.gridObjects)
//                        {
//                            if (c.row < Rows.Count && c.column < Rows[c.row].Length) Rows[c.row][c.column].SetObject(o.id, o.hits);
//                        }
//                    }
//                }
//        }


//        private void AssignObjectData(LevelConstructSet lcSet, GameMode gMode)
//        {
//            //int cellsCnt = 5;

//            Rows[0][1].SetObject(100, 0);
//            Rows[0][2].SetObject(500000, 0);
//            Rows[2][1].SetObject(1, 0);

//            //Rows[3][0].SetObject(100, 0);
//            //Rows[3][1].SetObject(100, 0);
//            //Rows[3][2].SetObject(100, 0);
//            //Rows[3][3].SetObject(100, 0);
//        }



//        /// <summary>
//        /// Add row to grid
//        /// </summary>
//        private void AddRow()
//        {
//            GridCell cell;
//            Row<GridCell> row = new Row<GridCell>(horSize);
//            bool isEvenRow = (Rows.Count % 2 == 0);
//            for (int j = 0; j < row.Length; j++)
//            {
//                bool isEvenColumn = (j % 2 == 0);
//                // pos를 고정시키면 셀 생성 X 
//                Vector3 pos = new Vector3(j * xStep + cOffset, yStart + yOffset * yStep, 0);


//                if (isEvenRow)
//                    cell = UnityEngine.Object.Instantiate((!isEvenColumn) ? goSet.gridCellOdd : goSet.gridCellEven).GetComponent<GridCell>();
//                else
//                    cell = UnityEngine.Object.Instantiate((isEvenColumn) ? goSet.gridCellOdd : goSet.gridCellEven).GetComponent<GridCell>();

//                cell.transform.parent = Parent;

//                cell.transform.localPosition = pos;
//                cell.transform.localScale = Vector3.one;
//                Cells.Add(cell);
//                row[j] = cell;
//            }

//            Rows.Add(row);

//            // cache columns
//            Columns = new List<Column<GridCell>>(horSize);
//            Column<GridCell> column;
//            for (int c = 0; c < horSize; c++)
//            {
//                column = new Column<GridCell>(Rows.Count);
//                for (int r = 0; r < Rows.Count; r++)
//                {
//                    column[r] = Rows[r][c];
//                }
//                Columns.Add(column);
//            }

//            //      Debug.Log("rows: " + Rows.Count +  " ;columns count: " + columns.Count);
//            for (int r = 0; r < Rows.Count; r++)
//            {
//                for (int c = 0; c < horSize; c++)
//                {
//                    Rows[r][c].Init(r, c, Columns[c], Rows[r], gMode);
//                }
//            }

//            yOffset--;
//        }

//        public GridCell this[int index0, int index1]
//        {
//            get { if (ok(index0, index1)) { return Rows[index0][index1]; } else { return null; } }
//            set { if (ok(index0, index1)) { Rows[index0][index1] = value; } else { } }
//        }

//        private bool ok(int index0, int index1)
//        {
//            return (index0 >= 0 && index0 < vertSize && index1 >= 0 && index1 < horSize);
//        }

//        /// <summary>
//        ///  return true if cells not simulate physics
//        /// </summary>
//        /// <returns></returns>
//        internal bool NoPhys()
//        {
//            foreach (GridCell c in Cells)
//            {
//                if (c.PhysStep) return false;
//            }
//            return true;
//        }

//        internal bool NoPhys1(MatchGrid g)
//        {
//            foreach (GridCell c in g.Cells)
//            {
//                if (c.PhysStep) return false;
//            }
//            return true;
//        }


//        #region  get data from grid
//        public MatchGroupsHelper GetMatches(int minMatch)
//        {
//            MatchGroupsHelper mgh = new MatchGroupsHelper(this);
//            mgh.CreateGroups(minMatch, false);
//            return mgh;
//        }

//        internal List<GridCell> GetEqualCells(GridCell gCell)
//        {
//            List<GridCell> gCells = new List<GridCell>();
//            for (int i = 0; i < Cells.Count; i++)
//            {
//                if (Cells[i].IsMatchObjectEquals(gCell))
//                {
//                    gCells.Add(Cells[i]);
//                }
//            }
//            return gCells;
//        }

//        internal List<GridCell> GetNeighCells(GridCell gCell, bool useDiagCells)
//        {
//            List<GridCell> nCells = new List<GridCell>();
//            int row = gCell.Row;
//            int column = gCell.Column;

//            GridCell c = this[row, column - 1]; if (c) nCells.Add(c); // left
//            c = this[row - 1, column]; if (c) nCells.Add(c); //  top
//            c = this[row, column + 1]; if (c) nCells.Add(c); // right
//            c = this[row + 1, column]; if (c) nCells.Add(c); // bot

//            if (useDiagCells)
//            {
//                c = this[row + 1, column - 1]; if (c) nCells.Add(c); // bot - left
//                c = this[row - 1, column - 1]; if (c) nCells.Add(c); // top - left
//                c = this[row - 1, column + 1]; if (c) nCells.Add(c); // top right
//                c = this[row + 1, column + 1]; if (c) nCells.Add(c); // bot- right
//            }
//            return nCells;
//        }



//        /// <summary>
//        /// Return not blocked, not disabled cells without dynamic object
//        /// </summary>
//        /// <returns></returns>
//        internal List<GridCell> GetFreeCells(MatchGrid g, bool withPath)
//        {
//            List<GridCell> gcL = new List<GridCell>();
//            for (int i = 0; i < g.Cells.Count; i++)
//            {
//                if (g.Cells[i].IsDynamicFree && !g.Cells[i].Blocked && !g.Cells[i].IsDisabled)
//                {
//                    //if (withPath && g.Cells[i].HaveFillPath() || !withPath)
//                    gcL.Add(g.Cells[i]);
//                }
//            }
//            return gcL;
//        }

//        /// <summary>
//        /// Return not blocked, not disabled cells without dynamic object, with fillPath or with and without
//        /// </summary>
//        /// <returns></returns>
//        internal List<GridCell> GetFreeCells(bool withPath)
//        {
//            List<GridCell> gcL = new List<GridCell>();
//            for (int i = 0; i < Cells.Count; i++)
//            {

//                if (Cells[i].IsDynamicFree && !Cells[i].Blocked && !Cells[i].IsDisabled)
//                {
//                    if (withPath && Cells[i].HaveFillPath() || !withPath)
//                        gcL.Add(Cells[i]);
//                }
//            }
//            return gcL;
//        }

//        /// <summary>
//        /// Return objects count on grid with selected ID
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        public int GetObjectsCountByID(int id)
//        {
//            int res = 0;
//            GridObject[] bds = Parent.GetComponentsInChildren<GridObject>();
//            foreach (var item in bds)
//            {
//                if (item.ID == id) res++;
//            }

//            return res;
//        }

//        /// <summary>
//        /// Return cells with object ID
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        public List<GridCell> GetAllByID(int id)
//        {
//            List<GridCell> res = new List<GridCell>();
//            foreach (var item in Cells)
//            {
//                if (item.HaveObjectWithID(id))
//                {
//                    res.Add(item);
//                }
//            }
//            return res;
//        }

//        public void CalcObjects()
//        {
//            GridObject[] bds = Parent.GetComponentsInChildren<GridObject>();
//            Debug.Log("Objects count: " + bds.Length);
//        }

//        /// <summary>
//        /// Get chess distance
//        /// </summary>
//        /// <returns></returns>
//        public static int GetChessDist(GridCell gc1, GridCell gc2)
//        {
//            return (Mathf.Abs(gc1.Row - gc2.Row) + Mathf.Abs(gc1.Column - gc2.Column));
//        }

//        /// <summary>
//        /// Get chess distance
//        /// </summary>
//        /// <returns></returns>
//        public static GridCell GetChessNear(GridCell gCell, IEnumerable<GridCell> area)
//        {
//            int dist = Int32.MaxValue;
//            GridCell nearItem = null;
//            if (gCell && area != null)
//            {
//                foreach (GridCell c in area)
//                {
//                    int dist2 = GetChessDist(c, gCell);
//                    if (dist2 < dist)
//                    {
//                        nearItem = c;
//                        dist = dist2;
//                    }
//                }
//            }
//            return nearItem;
//        }

//        /// <summary>
//        /// Return random match cell list
//        /// </summary>
//        /// <param name="count"></param>
//        /// <returns></returns>
//        public List<GridCell> GetRandomMatch(int count)
//        {
//            List<GridCell> temp = new List<GridCell>(Cells.Count);
//            List<GridCell> res = new List<GridCell>(count);

//            foreach (var item in Cells)
//            {
//                if (item.Match && !item.Overlay && !item.Underlay)
//                {
//                    temp.Add(item);
//                }
//            }
//            temp.Shuffle();
//            count = Mathf.Min(count, temp.Count);

//            for (int i = 0; i < count; i++)
//            {
//                res.Add(temp[i]);
//            }
//            return res;
//        }

//        public GridCell GetBomb()
//        {
//            foreach (var item in Cells)
//            {
//                if (item.HasBomb)
//                {
//                    if (item.Match && item.IsMatchable)
//                        return item;
//                    if (!item.Match)
//                        return item;
//                }
//            }
//            return null;
//        }

//        public List<GridCell> GetBottomDynCells()
//        {
//            // get all mather cells
//            List<GridCell> mathers = new List<GridCell>();
//            Cells.ForEach((c) =>
//            {
//                if (c.GravityMather && !mathers.Contains(c.GravityMather))
//                {
//                    mathers.Add(c.GravityMather);
//                }
//            });

//            List<GridCell> res = new List<GridCell>();
//            Cells.ForEach((c) =>
//            {
//                if (c.DynamicObject || (!c.Blocked && !c.IsDisabled))
//                {
//                    if (!mathers.Contains(c) && !res.Contains(c))
//                    {
//                        res.Add(c);
//                    }
//                }
//            });

//            return res;
//        }

//        public Row<GridCell> GetRow(int row)
//        {
//            return (row >= 0 && row < Rows.Count) ? Rows[row] : null;
//        }

//        public Column<GridCell> GetColumn(int col)
//        {
//            return (col >= 0 && col < Columns.Count) ? Columns[col] : null;
//        }

//        public CellsGroup GetWave(GridCell gCell, int radius)
//        {
//            radius = Mathf.Max(0, radius);
//            CellsGroup res = new CellsGroup();
//            int row1 = gCell.Row - radius;
//            int row2 = gCell.Row + radius;
//            int col1 = gCell.Column - radius;
//            int col2 = gCell.Column + radius;
//            Row<GridCell> topRow = GetRow(row1);
//            Row<GridCell> botRow = GetRow(row2);
//            Column<GridCell> leftCol = GetColumn(col1);
//            Column<GridCell> rightCol = GetColumn(col2);

//            if (topRow != null)
//            {
//                for (int i = col1; i <= col2; i++)
//                {
//                    if (ok(row1, i)) res.Add(topRow[i]);
//                }
//            }

//            if (rightCol != null)
//            {
//                for (int i = row1; i <= row2; i++)
//                {
//                    if (ok(i, col2)) res.Add(rightCol[i]);
//                }
//            }

//            if (botRow != null)
//            {
//                for (int i = col2; i >= col1; i--)
//                {
//                    if (ok(row2, i)) res.Add(botRow[i]);
//                }
//            }

//            if (leftCol != null)
//            {
//                for (int i = row2; i >= row1; i--)
//                {
//                    if (ok(i, col1)) res.Add(leftCol[i]);
//                }
//            }

//            return res;
//        }

//        public CellsGroup GetAroundArea(GridCell gCell, int radius)
//        {
//            radius = Mathf.Max(0, radius);
//            CellsGroup res = new CellsGroup();
//            if (radius > 0)
//                for (int i = 1; i <= radius; i++)
//                {
//                    res.AddRange(GetWave(gCell, i).Cells);
//                }
//            return res;
//        }

//        /// <summary>
//        /// Return gridcells group with id matched  around gCell
//        /// </summary>
//        /// <param name="gCell"></param>
//        /// <returns></returns>
//        public MatchGroup GetMatchIdArea(GridCell gCell)
//        {
//            MatchGroup res = new MatchGroup();
//            if (!gCell.Match || !gCell.IsMatchable) return res;

//            MatchGroup equalNeigh = new MatchGroup();
//            MatchGroup neighTemp;
//            int id = gCell.Match.ID;
//            res.Add(gCell);

//            equalNeigh.AddRange(gCell.Neighbors.GetMatchIdCells(id, true)); //equalNeigh.AddRange(gCell.EqualNeighBornCells());
//            while (equalNeigh.Length > 0)
//            {
//                res.AddRange(equalNeigh.Cells);
//                neighTemp = new MatchGroup();
//                foreach (var item in equalNeigh.Cells)
//                {
//                    neighTemp.AddRange(item.Neighbors.GetMatchIdCells(id, true)); // neighTemp.AddRange(item.EqualNeighBornCells());
//                }
//                equalNeigh = neighTemp;
//                equalNeigh.Remove(res.Cells);
//            }
//            return res;
//        }
//        #endregion  get data from grid

//        bool estimateMaxedOut(DNA<char> p, ref int cnt, int max)
//        {
//            if (max <= cnt) p.maxedOut = true;
//            return p.maxedOut;
//        }

//        public void fillState(DNA<char> p, SpawnController sC)
//        {
//            List<GridCell> gFreeCells = GetFreeCells(mh.grid, true);
//            if (gFreeCells.Count > 0) mh.createFillPath(mh.grid);

//            int fill_cnt = 0;

//            while (gFreeCells.Count > 0)
//            {
//                mh.fillGridByStep(gFreeCells, () => { });
//                gFreeCells = GetFreeCells(mh.grid, true);

//                if (estimateMaxedOut(p, ref fill_cnt, 100)) break;
//            }


//            //for (int i = 0; i < Cells.Count; i++)
//            //{
//            //    List<int> res = mh.grid.Cells[i].GetGridObjectsIDs();

//            //    if (mh.CellsContainer[i] != res[0])
//            //    {
//            //        mh.CellsContainer[i] = res[0];
//            //        mh.cellCnts[i]++;
//            //    }
//            //}




//            foreach (var item in mh.curTargets)
//            {
//                if (item.Value.Achieved) p.targetClear = true;
//                else
//                {
//                    p.targetClear = false;
//                    break;
//                }
//            }
//            if (p.targetClear) return;

//            p.curState = 2;

//            //NoPhys1(g);
//            //while (!NoPhys1(g));
//            //if (noMatches) RemoveMatches();
//        }

//        public void showState(DNA<char> p, SpawnController sC, Transform trans)
//        {

//            foreach (var item in mh.curTargets)
//            {
//                if (item.Value.Achieved) p.targetClear = true;
//                else
//                {
//                    p.targetClear = false;
//                    break;
//                }
//            }
//            if (p.targetClear) return;

//            int mix_cnt = 0;

//            while (!estimateMaxedOut(p, ref mix_cnt, 100))
//            {
//                //mh.cancelTweens(mh.grid);
//                mh.createMatchGroups1(2, true, mh.grid, p);

//                if (mh.grid.mgList.Count == 0)
//                {
//                    mh.mixGrid(null, mh.grid, trans);
//                    mix_cnt++;
//                }
//                else break;
//            }

//            if (p.maxedOut) return;

//            else
//            {
//                List<int> targetMatched = new List<int>();

//                //List<MatchGroup> matchedTarget = new List<MatchGroup>();

//                for (int i = 0; i < mh.grid.mgList.Count; i++)
//                {
//                    foreach (var item in mh.curTargets)
//                    {
//                        //target:matchBlock
//                        if (item.Value.ID >= 1000 && item.Value.ID <= 1006)
//                        {
//                            List<int> mg_cell = mh.grid.mgList[i].Cells[0].GetGridObjectsIDs();

//                            if (mg_cell[0] == item.Value.ID)
//                            {
//                                targetMatched.Add(i);
//                                break;
//                            }
//                        }

//                        //target:underlay
//                        else if (item.Value.ID >= 200000 && item.Value.ID <= 200001)
//                        {
//                            for (int j = 0; j < mh.grid.mgList[i].Length; j++)
//                            {
//                                if (mh.grid.mgList[i].Cells[j].Underlay != null)
//                                {
//                                    targetMatched.Add(i);
//                                    break;
//                                }
//                            }
//                        }

//                    }
//                }

//                if (targetMatched.Count == 0) mh.grid.mgList[0].SwapEstimate();

//                else
//                {
//                    p.shortCutCnt++;
//                    int number = Random.Range(0, targetMatched.Count - 1);
//                    mh.grid.mgList[targetMatched[number]].SwapEstimate();
//                }

//                p.numMove--;
//                p.curState = 2;
//            }
//        }

//        int collect_cnt = 0;
//        public void collectState(DNA<char> p)
//        {
//            if (collect_cnt >= 100)
//            {
//                p.maxedOut = true;
//                return;
//            }

//            if (mh.grid.GetFreeCells(true).Count > 0)
//            {
//                p.curState = 0;
//                return;
//            }

//            //mh.collectFalling(mh.grid);
//            //mh.cancelTweens(mh.grid);
//            mh.createMatchGroups(3, false, mh.grid);

//            if (mh.grid.mgList.Count == 0)
//            {
//                collect_cnt = 0;
//                p.curState = 1;
//            }

//            else
//            {
//                for (int i = 0; i < mh.grid.mgList.Count; i++)
//                {
//                    foreach (var item in mh.curTargets)
//                    {
//                        //target:matchBlock
//                        if (item.Value.ID >= 1000 && item.Value.ID <= 1006)
//                        {
//                            List<int> mg_cell = mh.grid.mgList[i].Cells[0].GetGridObjectsIDs();

//                            if (item.Value.ID == mg_cell[0])
//                            {
//                                item.Value.IncCurrCount(mh.grid.mgList[i].Cells.Count);
//                                break;
//                            }
//                        }

//                        //target:underlay
//                        else if (item.Value.ID >= 200000 && item.Value.ID <= 200001)
//                        {
//                            int cnt = 0;

//                            for (int j = 0; j < mh.grid.mgList[i].Length; j++)
//                            {
//                                if (mh.grid.mgList[i].Cells[j].Underlay != null)
//                                {
//                                    cnt++;
//                                }
//                            }

//                            item.Value.IncCurrCount(cnt);
//                        }
//                    }
//                }

//                for (int i = 0; i < mh.grid.mgList.Count; i++)
//                {
//                    if (mh.grid.mgList[i] != null)
//                    {
//                        foreach (GridCell c in mh.grid.mgList[i].Cells) c.DestroyGridObjects();
//                    }
//                }

//                p.allMove++;
//                p.curState = 0;


//                //    foreach (var mc in mh.grid.mgList)
//                //{
//                //    foreach (var item in mh.curTargets)
//                //    {
//                //        //target:matchBlock
//                //        if (item.Value.ID >= 1000 && item.Value.ID <= 1006)
//                //        {
//                //            List<int> mg_cell = mc.Cells[0].GetGridObjectsIDs();

//                //            if (item.Value.ID == mg_cell[0])
//                //            {
//                //                item.Value.IncCurrCount(mc.Cells.Count);
//                //                break;
//                //            }
//                //        }

//                //        //target:underlay
//                //        else if (item.Value.ID >= 200000 && item.Value.ID <= 200001)
//                //        {
//                //            int cnt = 0;

//                //            for (int i = 0; i < mc.Length; i++)
//                //            {
//                //                if (mc.Cells[i].Underlay != null) cnt++;
//                //            }

//                //            item.Value.IncCurrCount(cnt);
//                //        }

//                //    }
//                //}



//                //for (int i = 0; i < mh.grid.mgList.Count; i++)
//                //{
//                //    List<int> res = mh.grid.mgList[i].Cells[0].GetGridObjectsIDs();
//                //    foreach (var item in mh.curTargets)
//                //    {
//                //        if (item.Value.ID == res[0]) item.Value.IncCurrCount(mh.grid.mgList[i].Cells.Count);

//                //    }

//                //}

//                ////////
//            }

//            collect_cnt++;
//        }


//        public List<int> randomObsatcles()
//        {
//            List<int> ob = new List<int>();

//            for (int i = 0; i < 5; i++)
//            {
//                int number = Random.Range(0, mh.grid.Cells.Count + 1);
//                ob.Add(number);
//            }
//            return ob;
//        }

//        public char[] GetGenes()
//        {
//            char[] ti = new char[Cells.Count];

//            List<int> newGenes = new List<int>();

//            for (int i = 0; i < Cells.Count; i++) newGenes.Add(0);

//            for (int i = 0; i < 7; i++)
//            {
//                int number = Random.Range(0, Cells.Count);


//                if (
//                   number == 9 || number == 10 || number == 11 ||
//                   number == 16 || number == 17 || number == 18 ||
//                   number == 23 || number == 24 || number == 25
//                   )
//                {

//                }

//                else newGenes[number] = 1;
//            }

//            for (int i = 0; i < Cells.Count; i++) ti[i] = (char)(newGenes[i] + '0');

//            return ti;
//        }


//        public char GetRandomGene()
//        {
//            int number = Random.Range(0, 2);
//            return (char)(number + '0');
//        }

//        public void initalizeGrid1(SpawnController sC)
//        {
//            for (int i = 0; i < mh.grid.Cells.Count; i++)
//            {
//                mh.grid.Cells[i].DestroyGridObjects();

//                MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
//                mh.grid.Cells[i].SetObject1(m);

//            }
//        }

//        public void initObstacle(DNA<char> p, SpawnController sC)
//        {
//            for (int i = 0; i < mh.grid.Cells.Count; i++)
//            {
//                mh.grid.Cells[i].DestroyGridObjects();

//                if (p.genes[i] == '1')
//                {
//                    BlockedObject b = sC.GetRandomBlockedObjectPrefab(LcSet, goSet);
//                    mh.grid.Cells[i].SetObject1(b);
//                }

//                else
//                {
//                    MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
//                    mh.grid.Cells[i].SetObject1(m);
//                }
//            }
//        }


//        public void initGrid(DNA<char> p, SpawnController sC)
//        {
//            for (int i = 0; i < mh.grid.Cells.Count; i++)
//            {
//                mh.grid.Cells[i].DestroyGridObjects();

//                if (p.genes[i] == '1')
//                {
//                    BlockedObject b = sC.GetPickedObstacleObjectPrefab(LcSet, goSet);
//                    mh.grid.Cells[i].SetObject1(b);
//                }

//                else
//                {
//                    MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
//                    mh.grid.Cells[i].SetObject1(m);
//                }
//            }


//            foreach (var item in mh.curTargets)
//            {
//                //target: underlay (Grass)
//                if (item.Value.ID == 200001)
//                {
//                    if (i == 9)
//                    {
//                        UnderlayObject u = sC.GetSelectUnderlayObjectPrefab(LcSet, goSet);
//                        mh.grid.Cells[i].SetObject1(u);
//                    }
//                }

//                //target: overlay (Lianna) 
//                else if (item.Value.ID == 100001)
//                {

//                }

//                //target: blocked (Root) ?
//                else if (item.Value.ID == 123123)
//                {

//                }

//            }

//            //foreach (var item in mh.curTargets)
//            //{
//            //    //target:matchBlock
//            //    if (item.Value.ID >= 1000 && item.Value.ID <= 1006)
//            //    {
//            //        List<int> mg_cell = mh.grid.mgList[i].Cells[0].GetGridObjectsIDs();

//            //        if (item.Value.ID == mg_cell[0])
//            //        {
//            //            item.Value.IncCurrCount(mh.grid.mgList[i].Cells.Count);
//            //            break;
//            //        }
//            //    }

//            //    //target:underlay
//            //    else if (item.Value.ID >= 200000 && item.Value.ID <= 200001)
//            //    {
//            //        int cnt = 0;

//            //        for (int j = 0; j < mh.grid.mgList[i].Length; j++)
//            //        {
//            //            if (mh.grid.mgList[i].Cells[j].Underlay != null)
//            //            {
//            //                cnt++;
//            //            }
//            //        }

//            //        item.Value.IncCurrCount(cnt);
//            //    }
//            //}



//            //if(
//            //   i == 9  || i == 10 || i == 11 || 
//            //   i == 16 || i == 17 || i == 18 ||
//            //   i == 23 || i == 24 || i == 25
//            //   )
//            //{
//            //    UnderlayObject u = sC.GetSelectUnderlayObjectPrefab(LcSet, goSet);
//            //    mh.grid.Cells[i].SetObject1(u);
//            //}



//            mh.grid.RemoveMatches1();
//        }


//        public void initalizeGrid(DNA<char> p, SpawnController sC)
//        {
//            for (int i = 0; i < mh.grid.Cells.Count; i++)
//            {
//                mh.grid.Cells[i].DestroyGridObjects();

//                MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
//                mh.grid.Cells[i].SetObject1(m);


//                //if (


//                //    //i == 24 || i == 25 || i == 26 || i == 27 || i == 28 || i == 29 || i == 30 || i == 31 ||
//                //    //i == 32 || i == 33 || i == 34 || i == 35 || i == 36 || i == 37 || i == 38 || i == 39 ||
//                //    //i == 40 || i == 41 || i == 42 || i == 43 || i == 44 || i == 45 || i == 46 || i == 47 ||
//                //    //i == 48 || i == 49 || i == 50 || i == 51 || i == 52 || i == 53 || i == 54 || i == 55 ||
//                //    //i == 56 || i == 57 || i == 58 || i == 59 || i == 60 || i == 61 || i == 62 || i == 63

//                //    //i == 1 ||
//                //    //i == 9 ||
//                //    //i == 17 ||
//                //    //i == 25 ||
//                //    //i == 33 ||
//                //    //i == 41 ||
//                //    //i == 49 ||
//                //    //i == 57 

//                //    //||

//                //    //i == 2 ||
//                //    //i == 10 ||
//                //    //i == 18 ||
//                //    //i == 26 ||
//                //    //i == 34 ||
//                //    i == 42 ||
//                //    //i == 50 ||
//                //    //i == 58

//                //    //i == 1 ||
//                //    //i == 9 ||
//                //    //i == 17 ||
//                //    //i == 25 ||
//                //    //i == 33 ||
//                //    //i == 41 ||
//                //    i == 49 ||
//                //    i == 57 ||

//                //    //i == 3 ||
//                //    //i == 11 ||
//                //    i == 19 ||
//                //    i == 27 ||
//                //    i == 35 ||
//                //    //i == 43 ||
//                //    i == 51 ||
//                //    i == 59

//                //    ////193
//                //    //i == 3 || i == 5 ||
//                //    //i == 10 || i == 11 || i == 13 ||
//                //    //i == 17 || i == 18 || i == 19 || i == 21 ||
//                //    //i == 25 || i == 26 || i == 29 ||
//                //    //i == 32 || i == 33 || i == 37 || i == 38 ||
//                //    //i == 45 || i == 47

//                //    ////163
//                //    //i == 3 || i == 6 ||
//                //    //i == 10 || i == 11 || i == 14 ||
//                //    //i == 17 || i == 18 || i == 19 || i == 22 ||
//                //    //i == 25 || i == 26 || i == 30 ||
//                //    //i == 32 || i == 33 || i == 38 ||
//                //    //i == 45 ||
//                //    //i == 52 ||
//                //    //i == 59

//                //    )
//                //{
//                //    BlockedObject b = sC.GetRandomBlockedObjectPrefab(LcSet, goSet);
//                //    mh.grid.Cells[i].SetObject1(b);
//                //}



//                //if (p.genes[i] == '1')
//                //{
//                //    BlockedObject b = sC.GetRandomBlockedObjectPrefab(LcSet, goSet);
//                //    mh.grid.Cells[i].SetObject1(b);
//                //}
//            }

//            //public List<GridCell> GetAllByID(int id)
//            //{
//            //    List<GridCell> res = new List<GridCell>();
//            //    foreach (var item in Cells)
//            //    {
//            //        if (item.HaveObjectWithID(id))
//            //        {
//            //            res.Add(item);
//            //        }
//            //    }
//            //    return res;




//            //List<MatchGroup> matchedTarget = new List<MatchGroup>();

//            //foreach (var mc in mh.grid.mgList)
//            //{
//            //    foreach (var item in mh.curTargets)
//            //    {
//            //        List<int> mg_cell = mc.Cells[0].GetGridObjectsIDs();

//            //        if (mg_cell[0] == item.Key)
//            //        {
//            //            matchedTarget.Add(mc);
//            //            break;
//            //        }
//            //    }
//            //}



//            //List<int> list = new List<int>();
//            //List<int> s_list = new List<int>();

//            //foreach (var item in Cells)
//            //{
//            //    s_list = item.GetGridObjectsIDs();
//            //    list.Add(s_list[0]);
//            //}

//            mh.grid.RemoveMatches1();

//            // List<int> lis1t = new List<int>();
//            // List<int> s_list1 = new List<int>();


//            // foreach (var item in Cells)
//            // {
//            //     s_list1 = item.GetGridObjectsIDs();
//            //     lis1t.Add(s_list1[0]);
//            // }


//            //List<int> res = Cells[0].GetGridObjectsIDs();


//            //for (int i = 0; i < mh.grid.Cells.Count; i++)
//            //{
//            //    if(mh.grid.Cells[i].HaveObjectWithID(0))
//            //    {

//            //    }
//            //}



//            //for (int i = 0; i < 7; i++)
//            //{
//            //    MatchObject m = sC.GetPickMatchObject(LcSet, goSet, i);

//            //    int value = 0;

//            //    foreach (var item in mh.curTargets)
//            //    {
//            //        if (m.ID == item.Value.ID)
//            //        {
//            //            value = item.Value.NeedCount;
//            //            break;
//            //        }
//            //    }

//            //    ga.targetNeedCnt.Add(value);
//            //}


//        }

//        public void initGridWithOutBlockedBlock(DNA<char> p, SpawnController sC)
//        {
//            for (int i = 0; i < mh.grid.Cells.Count; i++)
//            {
//                if (mh.grid.Cells[i].Blocked == null)
//                {
//                    //mh.grid.Cells[i].DestroyGridObjects();
//                    MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
//                    mh.grid.Cells[i].SetObject1(m);
//                }


//                //if (p.genes[i] != '1')
//                //{
//                //    //mh.grid.Cells[i].DestroyGridObjects();
//                //    MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
//                //    mh.grid.Cells[i].SetObject1(m);
//                //}
//            }

//            mh.grid.RemoveMatches1();
//        }

//        public bool EstimateFeasible(MatchGrid grid, DNA<char> p)
//        {
//            int cnt = 0;
//            bool isFeasible = mh.isConnected(grid, cnt);
//            return false;
//        }
//        public void estimateIsFeasible(DNA<char> p)
//        {
//            mh.createFillPath(mh.grid);
//            p.infeasibleCellCnt = 0;

//            for (int i = mh.grid.Columns.Count(); i < mh.grid.Cells.Count; i++)
//            {
//                if (mh.grid.Cells[i].Blocked == null && mh.grid.Cells[i].fillPathToSpawner == null)
//                {
//                    if (mh.grid.Cells[i].Neighbors.Top.Blocked != null) p.infeasibleCellCnt++;
//                }
//            }

//            if (p.infeasibleCellCnt == 0) p.isFeasible = true;
//            else p.isFeasible = false;
//        }


//        public void estimateFeasible(DNA<char> p)
//        {
//            p.isFeasible = true;
//            //mh.createFillPath(mh.grid);
//            //p.infeasibleCellCnt = 0;
//            //for (int i = mh.grid.Columns.Count(); i < mh.grid.Cells.Count; i++)
//            //{
//            //    if (mh.grid.Cells[i].Blocked == null && mh.grid.Cells[i].fillPathToSpawner == null)
//            //    {
//            //        if (mh.grid.Cells[i].Neighbors.Top.Blocked != null) p.infeasibleCellCnt++;
//            //    }
//            //}
//            //if (p.infeasibleCellCnt == 0) p.isFeasible = true;
//            //else p.isFeasible = false;
//        }

//        public void calculateFitness(DNA<char> p, SpawnController sC, Transform trans)
//        {
//            if (p.isFeasible)
//            {
//                makeFeasibleFitness(p, sC, trans);
//                ga.feasiblePopulation.Add(p);
//                ga.feasibleFitnessSum += p.fitness;

//            }

//            else
//            {
//                p.calculateInfeasibleFitness();
//                ga.infeasiblePopulation.Add(p);
//                ga.infeasibleFitnessSum += p.fitness;
//            }
//        }

//        public bool estimateFloationgPoint(double a, double b)
//        {
//            //double epsilon = 1e-9;
//            if (areEqual(a, b) || a < b) return true;
//            return false;

//        }

//        public bool areEqual(double a, double b, double tolerance = 1e-9)
//        {
//            return Math.Abs(a - b) <= tolerance;
//        }


//        public void makeFeasibleFitness(DNA<char> p, SpawnController sC, Transform trans)
//        {
//            List<int> moveContainer = new List<int>();

//            List<int> nMoveContainer = new List<int>();

//            List<int> shortCutCntContainer = new List<int>();

//            List<int> possibleList = new List<int>();

//            List<int> spawnList = new List<int>();

//            for (int repeatIdx = 0; repeatIdx < ga.repeat; repeatIdx++)
//            {
//                //initalizeGrid(p, sC);
//                //mh.createAvailableMatchGroup(mh.grid);
//                //mh.CellsContainer = new List<int>();
//                //for (int i=0;i<Cells.Count;i++) 
//                //{
//                //    List<int> res = mh.grid.Cells[i].GetGridObjectsIDs();
//                //    mh.CellsContainer.Add(res[0]);
//                //}


//                initGrid(p, sC);
//                //initGridWithOutBlockedBlock(p, sC);


//                //////////////////////////////////////////////////////////////

//                if (ga.isPossible)
//                {
//                    MatchGroup matchGroup = new MatchGroup();
//                    for (int i = 0; i < mh.grid.Cells.Count; i++) mh.grid.Cells[i].possibleCnt = 0;
//                    for (int i = 0; i < mh.grid.Cells.Count; i++) matchGroup.countPossible(mh.grid, i);
//                    ga.possibleCounting = new int[mh.grid.Columns.Count * mh.grid.Rows.Count];

//                    for (int i = 0; i < mh.grid.Cells.Count; i++)
//                    {
//                        ga.possibleCnt += mh.grid.Cells[i].possibleCnt;
//                        ga.possibleCounting[mh.grid.Cells[i].possibleCnt]++;
//                        ga.possibleCountingList.Add(mh.grid.Cells[i].possibleCnt);

//                        if (mh.grid.Cells[i].DynamicObject == null) p.blockedCnt++;
//                    }

//                    ga.blockeCnt = p.blockedCnt;
//                    ga.isPossible = false;

//                    //for (int i = 0; i < mh.grid.Cells.Count; i++)
//                    //{
//                    //    if (mh.grid.Cells[i].fillPathToSpawner != null)
//                    //    {
//                    //        if(mh.grid.Cells[i].fillPathToSpawner.Count == 0) spawnList.Add(mh.grid.Cells[i].Column);
//                    //        else
//                    //        {
//                    //            int a = mh.grid.Cells[i].fillPathToSpawner[mh.grid.Cells[i].fillPathToSpawner.Count - 1].Column;
//                    //            spawnList.Add(a);
//                    //        }
//                    //    }
//                    //    else spawnList.Add(9);
//                    //}
//                    //mh.isConnected(mh.grid, 0);
//                }


//                p.allMove = 0;
//                p.numMove = ga.moveLimit;
//                p.targetClear = false;
//                p.maxedOut = false;
//                p.curState = 0;
//                p.obstructionRate = 0;
//                p.shortCutCnt = 0;
//                //int tryCnt = 0;

//                foreach (var item in mh.curTargets) item.Value.InitCurCount();

//                while (p.numMove > 0 && p.targetClear == false && p.maxedOut == false)
//                {
//                    switch (p.curState)
//                    {
//                        case 0:
//                            fillState(p, sC);
//                            break;
//                        case 1:
//                            showState(p, sC, trans);
//                            break;
//                        case 2:
//                            collectState(p);
//                            break;
//                    }
//                    //if (tryCnt > 5000) break;
//                    //tryCnt++;
//                }


//                List<GridCell> gFreeCells = GetFreeCells(mh.grid, true);

//                if (gFreeCells.Count > 0)
//                {
//                    mh.createFillPath(mh.grid);
//                    while (gFreeCells.Count > 0)
//                    {
//                        mh.fillGridByStep(gFreeCells, () => { });
//                        gFreeCells = GetFreeCells(mh.grid, true);
//                    }
//                }

//                //for (int i = 0; i < Cells.Count; i++)
//                //{
//                //    List<int> res = mh.grid.Cells[i].GetGridObjectsIDs();

//                //    if (mh.CellsContainer[i] != res[0])
//                //    {
//                //        mh.CellsContainer[i] = res[0];
//                //        mh.cellCnts[i]++;
//                //    }
//                //}

//                shortCutCntContainer.Add(p.shortCutCnt);
//                //shortCutCntContainer.Add(p.obstructionRate);

//                if (p.maxedOut == true || p.targetClear == false)
//                {
//                    moveContainer.Add(ga.moveLimit);
//                    nMoveContainer.Add(ga.moveLimit);

//                    //continue;
//                }

//                //////// 교체함
//                else
//                {
//                    moveContainer.Add(ga.moveLimit - p.numMove);
//                    nMoveContainer.Add(p.allMove);
//                }

//                //mh.CancelTweens(mh.grid);
//                //else num_move_container.Add(0);
//            }

//            p.calculateFeasibleFitness(moveContainer, (double)ga.targetMove, ga.targetStd);

//            ga.repeatMovements.Add(moveContainer);
//            ga.repeatMovementsCnt++;
//            ga.allMovements.Add(nMoveContainer);
//            ga.shorCutRates.Add(shortCutCntContainer);

//            if (ga.curGenerationBestFitness < p.fitness)
//            {
//                ga.curGenerationBestFitness = p.fitness;
//                ga.curGenerationBestMean = p.mean;
//                ga.curGenerationBestStd = p.stanardDeviation;
//                ga.curBestMoves = moveContainer;
//            }

//            if (ga.bestFitness < p.fitness)
//            {
//                ga.bestFitness = p.fitness;
//                ga.bestMeanMove = p.mean;
//                ga.bestStd = p.stanardDeviation;
//                ga.bestMoves = moveContainer;
//            }


//            //if(estimateFloationgPoint(ga.curGenerationBestFitness, p.fitness))
//            //{
//            //    ga.curGenerationBestFitness = p.fitness;
//            //    ga.curGenerationBestMean = p.mean;
//            //    ga.curGenerationBestStd = p.stanardDeviation;
//            //}
//            //if (estimateFloationgPoint(ga.bestFitness, p.fitness))
//            //{
//            //    ga.bestFitness = p.fitness;
//            //    ga.bestMeanMove = p.mean;
//            //    ga.bestStd = p.stanardDeviation;
//            //}
//            //if(alpha * (1.0 / (1.0 + Math.Abs(mean - target_move))) + (1 - alpha) * (1.0 / (1.0 + Math.Abs(standardDeviation - target_std));
//            //if (Math.Abs(ga.targetMove - ga.curBestMeanMove) > Math.Abs(ga.targetMove - p.mean))
//            //{
//            //    ga.curBestMeanMove = p.mean;
//            //}
//        }

//        void getMatch3Level(Transform trans)
//        {
//            SpawnController sC = SpawnController.Instance;
//            CSVFileWriter cs = new CSVFileWriter();

//            ga.bestMeanMove = 0;
//            ga.bestStd = 0;
//            ga.bestFitness = 0;
//            ga.repeatMovements = new List<List<int>>();
//            ga.obstructionRates = new List<List<int>>();
//            ga.shorCutRates = new List<List<int>>();
//            ga.targetNeedCnt = new List<int>();
//            ga.possibleCnt = 0;
//            ga.isPossible = true;
//            ga.allMovements = new List<List<int>>();
//            ga.possibleCountingList = new List<int>();


//            mh.cellCnts = new int[Cells.Count];
//            int cnt = 0;
//            int feasibleIdx = 0;

//            for (int i = 0; i < ga.findFeasibleLimit; i++)
//            {
//                for (int j = 0; j < ga.population.Count; j++)
//                {
//                    ga.population[j].blockedCnt = 0;

//                    initObstacle(ga.population[j], sC);
//                    estimateIsFeasible(ga.population[j]);

//                    if (ga.population[j].isFeasible)
//                    {
//                        feasibleIdx = j;
//                        break;
//                    }

//                    calculateFitness(ga.population[j], sC, trans);
//                }

//                if (!ga.population[feasibleIdx].isFeasible)
//                {
//                    ga.NewGeneration();
//                    cnt++;
//                }
//                else break;
//            }


//            if (ga.population[feasibleIdx].isFeasible)
//            {
//                while (ga.generation <= ga.generationLimit)
//                {
//                    ga.curGenerationBestMean = 0;
//                    ga.curGenerationBestStd = 0;
//                    ga.curGenerationBestFitness = 0;
//                    ga.repeatMovementsCnt = 0;


//                    makeFeasibleFitness(ga.population[feasibleIdx], sC, trans);

//                    cs.generation.Add(ga.generation);

//                    if (ga.infeasiblePopulation == null) cs.infeasiblePopulationCnt.Add(0);
//                    else cs.infeasiblePopulationCnt.Add(ga.infeasiblePopulation.Count());

//                    if (ga.feasiblePopulation == null) cs.feasiblePopulationCnt.Add(0);
//                    else cs.feasiblePopulationCnt.Add(ga.feasiblePopulation.Count());

//                    cs.curGenerationBestMean.Add(ga.curGenerationBestMean);
//                    cs.curGenerationBestStd.Add(ga.curGenerationBestStd);
//                    cs.curGenerationBestFitness.Add(ga.curGenerationBestFitness);

//                    cs.bestMeanMove.Add(ga.bestMeanMove);
//                    cs.bestStd.Add(ga.bestStd);
//                    cs.bestFitness.Add(ga.bestFitness);

//                    cs.repeatMovementsCntContainer.Add(ga.repeatMovementsCnt);

//                    if (ga.curBestMoves == null) cs.curBestMoves.Add(new List<int>() { -1 });
//                    else cs.curBestMoves.Add(ga.curBestMoves);

//                    if (ga.bestMoves == null) cs.bestMoves.Add(new List<int>() { -1 });
//                    else cs.bestMoves.Add(ga.bestMoves);


//                    //if (ga.bestFitness >= ga.targetFitness) break;

//                    ga.generation++;
//                }

//                initGrid(ga.population[feasibleIdx], sC);
//            }



//            List<int> cellPossible = new List<int>();

//            for (int i = 0; i < Cells.Count; i++)
//            {
//                cellPossible.Add(mh.grid.Cells[i].possibleCnt);
//            }


//            //while (!ga.population[0].isFeasible)
//            //{
//            //    foreach (DNA<char> p in ga.population)
//            //    {
//            //        initalizeGridOnlyCheckFeasible(p, sC);
//            //        estimateIsFeasible(p);
//            //    }
//            //}


//            //while (ga.generation <= ga.generationLimit)
//            //{
//            //    ga.curGenerationBestMean = 0;
//            //    ga.curGenerationBestStd = 0;
//            //    ga.curGenerationBestFitness = 0;
//            //    ga.repeatMovementsCnt = 0;

//            //    foreach (DNA<char> p in ga.population)
//            //    {
//            //        if (p.fitness != 0)
//            //        {
//            //            if (p.isFeasible) ga.feasiblePopulation.Add(p);
//            //            else ga.infeasiblePopulation.Add(p);
//            //        }

//            //        else
//            //        {
//            //            initalizeGrid(p, sC);
//            //            estimateFeasible(p);
//            //            calculateFitness(p, sC, trans);
//            //        }
//            //    }

//            //    cs.generation.Add(ga.generation);

//            //    if (ga.infeasiblePopulation == null) cs.infeasiblePopulationCnt.Add(0);
//            //    else cs.infeasiblePopulationCnt.Add(ga.infeasiblePopulation.Count());

//            //    if (ga.feasiblePopulation == null) cs.feasiblePopulationCnt.Add(0);
//            //    else cs.feasiblePopulationCnt.Add(ga.feasiblePopulation.Count());

//            //    cs.curGenerationBestMean.Add(ga.curGenerationBestMean);
//            //    cs.curGenerationBestStd.Add(ga.curGenerationBestStd);
//            //    cs.curGenerationBestFitness.Add(ga.curGenerationBestFitness);

//            //    cs.bestMeanMove.Add(ga.bestMeanMove);
//            //    cs.bestStd.Add(ga.bestStd);
//            //    cs.bestFitness.Add(ga.bestFitness);

//            //    cs.repeatMovementsCntContainer.Add(ga.repeatMovementsCnt);

//            //    if (ga.curBestMoves == null) cs.curBestMoves.Add(new List<int>() { -1 });
//            //    else cs.curBestMoves.Add(ga.curBestMoves);

//            //    if (ga.bestMoves == null) cs.bestMoves.Add(new List<int>() { -1 });
//            //    else cs.bestMoves.Add(ga.bestMoves);


//            //    if (ga.bestFitness >= ga.targetFitness) break;

//            //    ga.NewGeneration();
//            //}

//            cs.mixedList = new List<object>
//            {
//                cs.generation,
//                cs.infeasiblePopulationCnt,
//                cs.feasiblePopulationCnt,
//                cs.curGenerationBestMean,
//                cs.curGenerationBestStd,
//                cs.curGenerationBestFitness,
//                cs.bestMeanMove,
//                cs.bestStd,
//                cs.bestFitness,
//                cs.feasibleParent,
//                cs.feasibleParentIdx,
//                cs.infeasibleParent,
//                cs.infeasibleParentIdx,
//                cs.curBestMoves,
//                cs.bestMoves
//            };


//            for (int i = 0; i < 7; i++)
//            {
//                MatchObject m = sC.GetPickMatchObject(LcSet, goSet, i);

//                int value = 0;

//                foreach (var item in mh.curTargets)
//                {
//                    if (m.ID == item.Value.ID)
//                    {
//                        value = item.Value.NeedCount;
//                        break;
//                    }
//                }

//                ga.targetNeedCnt.Add(value);
//            }


//            cs.write(ga, Cells);
//            //initalizeGrid(ga.population[0], sC);
//        }

//        //void getMatch3Level(Transform trans)
//        //{
//        //    SpawnController sC = SpawnController.Instance;
//        //    CSVFileWriter cs = new CSVFileWriter();

//        //    ga.bestMeanMove = 0;
//        //    ga.bestStd = 0;
//        //    ga.bestFitness = 0;
//        //    ga.repeatMovements = new List<List<int>>();
//        //    ga.obstructionRates = new List<List<int>>();
//        //    ga.shorCutRates = new List<List<int>>();
//        //    ga.targetNeedCnt = new List<int>();
//        //    ga.possibleCnt = 0;
//        //    ga.isPossible = true;
//        //    ga.allMovements = new List<List<int>>();
//        //    ga.possibleCountingList = new List<int>();

//        //    //initalizeGrid1(sC);

//        //    //MatchGroup matchGroup = new MatchGroup();
//        //    ////for (int i = 0; i < mh.grid.Cells.Count; i++) mh.grid.Cells[i].possibleCnt = 0;
//        //    //for (int i = 0; i < mh.grid.Cells.Count; i++)
//        //    //{
//        //    //    matchGroup.countPossible(mh.grid, i);
//        //    //}

//        //    //int a = 0;

//        //    //for (int i = 0; i < mh.grid.Cells.Count; i++)
//        //    //{
//        //    //    a += mh.grid.Cells[i].possibleCnt;
//        //    //}

//        //    //List<int> pc = new List<int>();

//        //    //for (int i = 0; i < mh.grid.Cells.Count; i++)
//        //    //{
//        //    //    pc.Add(mh.grid.Cells[i].possibleCnt);
//        //    //}

//        //    while (ga.bestFitness < ga.targetFitness && ga.generation <= ga.generationLimit)
//        //    {
//        //        ga.curGenerationBestMean = 0;
//        //        ga.curGenerationBestStd = 0;
//        //        ga.curGenerationBestFitness = 0;
//        //        ga.repeatMovementsCnt = 0;

//        //        foreach (DNA<char> p in ga.population)
//        //        {
//        //            if (p.fitness != 0)
//        //            {
//        //                if (p.isFeasible) ga.feasiblePopulation.Add(p);
//        //                else ga.infeasiblePopulation.Add(p);
//        //            }

//        //            else
//        //            {
//        //                initalizeGrid(p, sC);
//        //                estimateFeasible(p);
//        //                calculateFitness(p, sC, trans);
//        //            }
//        //        }

//        //        //double bestAverageMove = 0;
//        //        //double beststandardDeviation = 0;

//        //        ////for (int i = 0; i < ga.feasible_population.Count; i++)
//        //        ////{
//        //        ////    if(bestAverageMove < ga.feasible_population[i].average_move) bestAverageMove = ga.feasible_population[i].average_move;
//        //        ////    if (beststandardDeviation < ga.feasible_population[i].sd) beststandardDeviation = ga.feasible_population[i].sd;
//        //        ////}


//        //        cs.generation.Add(ga.generation);

//        //        if(ga.infeasiblePopulation == null) cs.infeasiblePopulationCnt.Add(0);
//        //        else cs.infeasiblePopulationCnt.Add(ga.infeasiblePopulation.Count());

//        //        if (ga.feasiblePopulation == null) cs.feasiblePopulationCnt.Add(0);
//        //        else cs.feasiblePopulationCnt.Add(ga.feasiblePopulation.Count());

//        //        //cs.infeasiblePopulationCnt.Add(ga.infeasiblePopulation.Count());
//        //        //cs.feasiblePopulationCnt.Add(ga.feasiblePopulation.Count());

//        //        cs.curGenerationBestMean.Add(ga.curGenerationBestMean);
//        //        cs.curGenerationBestStd.Add(ga.curGenerationBestStd);
//        //        cs.curGenerationBestFitness.Add(ga.curGenerationBestFitness);

//        //        cs.bestMeanMove.Add(ga.bestMeanMove);
//        //        cs.bestStd.Add(ga.bestStd);
//        //        cs.bestFitness.Add(ga.bestFitness);

//        //        cs.repeatMovementsCntContainer.Add(ga.repeatMovementsCnt);

//        //        if (ga.curBestMoves == null) cs.curBestMoves.Add(new List<int>() { -1 });
//        //        else cs.curBestMoves.Add(ga.curBestMoves);

//        //        if (ga.bestMoves == null) cs.bestMoves.Add(new List<int>() { -1 });
//        //        else cs.bestMoves.Add(ga.bestMoves);


//        //        if (ga.bestFitness >= ga.targetFitness) break;
//        //        ga.NewGeneration();


//        //        //List<int> mg_cell = GetGridObjectsIDs();


//        //        //foreach (var item in mh.curTargets) item.Value.InitCurCount();





//        //        //if (ga.feasibleParent == null)
//        //        //{
//        //        //    cs.feasibleParent.Add(new List<double>() { -1, -1 });
//        //        //    cs.feasibleParentIdx.Add(new List<int>() { -1, -1 });
//        //        //}

//        //        //else
//        //        //{
//        //        //    cs.feasibleParent.Add(new List<double>(ga.feasibleParent));
//        //        //    cs.feasibleParentIdx.Add(new List<int>(ga.feasibleParentIdx));
//        //        //}


//        //        //if (ga.infeasibleParent == null)
//        //        //{
//        //        //    cs.infeasibleParent.Add(new List<double>() { -1, -1 });
//        //        //    cs.infeasibleParentIdx.Add(new List<int>() { -1, -1 });
//        //        //}

//        //        //else
//        //        //{
//        //        //    cs.infeasibleParent.Add(new List<double>(ga.infeasibleParent));
//        //        //    cs.infeasibleParentIdx.Add(new List<int>(ga.infeasibleParentIdx));
//        //        //}

//        //    }

//        //    cs.mixedList = new List<object>
//        //    {
//        //        cs.generation,
//        //        cs.infeasiblePopulationCnt,
//        //        cs.feasiblePopulationCnt,
//        //        cs.curGenerationBestMean,
//        //        cs.curGenerationBestStd,
//        //        cs.curGenerationBestFitness,
//        //        cs.bestMeanMove,
//        //        cs.bestStd,
//        //        cs.bestFitness,
//        //        cs.feasibleParent,
//        //        cs.feasibleParentIdx,
//        //        cs.infeasibleParent,
//        //        cs.infeasibleParentIdx,
//        //        cs.curBestMoves,
//        //        cs.bestMoves
//        //    };


//        //    for (int i = 0; i < 7; i++)
//        //    {
//        //        MatchObject m = sC.GetPickMatchObject(LcSet, goSet, i);

//        //        int value = 0;

//        //        foreach (var item in mh.curTargets)
//        //        {
//        //            if (m.ID == item.Value.ID)
//        //            {
//        //                value = item.Value.NeedCount;
//        //                break;
//        //            }
//        //        }

//        //        ga.targetNeedCnt.Add(value);
//        //    }


//        //    cs.write(ga, Cells);
//        //    //cs.write1(ga);

//        //    initalizeGrid(ga.population[0], sC);
//        //}

//        public List<MatchGroup> mgList;
//        public Match_Helper mh;
//        System.Random randomGa;
//        GeneticAlgorithm<char> ga;

//        internal void fillGrid(bool noMatches, MatchGrid g, Dictionary<int, TargetData> targets, Spawner spawnerPrefab, SpawnerStyle spawnerStyle, Transform GridContainer, Transform trans, LevelConstructSet IC)
//        {
//            randomGa = new System.Random();
//            ga = new GeneticAlgorithm<char>(Cells.Count, randomGa, GetRandomGene, GetGenes); //유전알고리즘 호출

//            mh = new Match_Helper();
//            mh.board = new GameBoard();
//            mh.board.makeBoard(g, spawnerPrefab, spawnerStyle, GridContainer, trans, IC);
//            g.mgList = new List<MatchGroup>();
//            mh.grid = g;
//            mh.curTargets = targets;


//            //List<double> doubles = new List<double>();
//            //for (double i = 0; i < 10; i += 0.01)
//            //{
//            //    string result = string.Format("{0:0.########0}", Math.Exp(-Math.Abs(i)));

//            //    doubles.Add((Double.Parse(result)));
//            //}

//            //List<double> doubles = new List<double>();
//            //for (double i = 0; i < 100; i += 0.01)
//            //{
//            //    doubles.Add(1.0 / (1.0 + Math.Abs(i)));
//            //}

//            getMatch3Level(trans);
//        }


//        #region fill grid
//        /// <summary>
//        /// Fill grid with random regular objects, preserve existing dynamic objects (match, click bomb, falling)
//        /// </summary>
//        /// <param name="noMatches"></param>
//        /// <param name="goSet"></param>
//        /// 
//        internal void FillGrid(bool noMatches)
//        {
//            SpawnController sC = SpawnController.Instance;
//            Debug.Log("fill grid, remove matches: " + noMatches);
//            for (int i = 0; i < Cells.Count; i++)
//            {
//                if (!Cells[i].Blocked && !Cells[i].IsDisabled && !Cells[i].DynamicObject)
//                {
//                    MatchObject m = sC.GetMainRandomObjectPrefab(LcSet, goSet);
//                    Cells[i].SetObject(m);
//                }
//            }
//            if (noMatches) RemoveMatches();
//        }
//        internal void RemoveMatches()
//        {
//            SpawnController sC = SpawnController.Instance;
//            int minMatch = 3;
//            GridCell[] gc_row = new GridCell[minMatch];
//            GridCell[] gc_col = new GridCell[minMatch];
//            System.Func<GridCell[], bool> isEqual = (gcl) =>
//            {
//                if (gcl == null || gcl.Length == 0) return false;
//                foreach (var item in gcl)
//                    if (!item || !item.Match) return false;

//                int id = gcl[0].Match.ID;

//                foreach (var item in gcl)
//                    if (item.Match.ID != id) return false;
//                return true;
//            };
//            List<GridObject> mod_list;
//            for (int i = 0; i < vertSize; i++)
//            {
//                for (int j = 0; j < horSize; j++)
//                {
//                    if (Rows[i][j].Blocked || Rows[i][j].IsDisabled) continue;
//                    for (int m = 0; m < minMatch; m++)
//                    {
//                        gc_row[m] = this[i, j - m];
//                        gc_col[m] = this[i - m, j];
//                    }
//                    mod_list = new List<GridObject>();
//                    bool rowHasMatches = false;
//                    bool colHasMatches = false;

//                    if (isEqual(gc_row)) rowHasMatches = true;
//                    if (isEqual(gc_col)) colHasMatches = true;

//                    if (rowHasMatches || colHasMatches)
//                    {
//                        if (gc_col[1] && gc_col[1].Match) mod_list.Add(sC.GetMainObjectPrefab(goSet, gc_col[1].Match.ID));
//                        if (gc_row[1] && gc_row[1].Match) mod_list.Add(sC.GetMainObjectPrefab(goSet, gc_row[1].Match.ID));
//                    }
//                    if (mod_list.Count > 0) Rows[i][j].GetComponent<GridCell>().SetObject((sC.GetMainRandomObjectPrefab(LcSet, goSet, mod_list)));
//                }
//            }
//#if UNITY_EDITOR
//            // double test
//            for (int i = 0; i < vertSize; i++)
//            {
//                for (int j = 0; j < horSize; j++)
//                {
//                    if (Rows[i][j].Blocked || Rows[i][j].IsDisabled) continue;
//                    for (int m = 0; m < minMatch; m++)
//                    {
//                        gc_row[m] = this[i, j - m];
//                        gc_col[m] = this[i - m, j];
//                    }
//                    mod_list = new List<GridObject>();
//                    bool rowHasMatches = false;
//                    bool colHasMatches = false;

//                    if (isEqual(gc_row)) rowHasMatches = true;
//                    if (isEqual(gc_col)) colHasMatches = true;

//                    if (rowHasMatches || colHasMatches)
//                    {
//                        if (gc_col[1] && gc_col[1].Match) mod_list.Add(sC.GetMainObjectPrefab(goSet, gc_col[1].Match.ID));
//                        if (gc_row[1] && gc_row[1].Match) mod_list.Add(sC.GetMainObjectPrefab(goSet, gc_row[1].Match.ID));
//                        Debug.Log("----------------------------Found matches--------------------------------------");
//                    }
//                    if (mod_list.Count > 0) Rows[i][j].GetComponent<GridCell>().SetObject((sC.GetMainRandomObjectPrefab(LcSet, goSet, mod_list)));
//                }
//            }
//#endif
//        }
//        #endregion fill grid


//        internal void RemoveMatches1()
//        {
//            SpawnController sC = SpawnController.Instance;
//            int minMatch = 3;
//            GridCell[] gc_row = new GridCell[minMatch];
//            GridCell[] gc_col = new GridCell[minMatch];
//            System.Func<GridCell[], bool> isEqual = (gcl) =>
//            {
//                if (gcl == null || gcl.Length == 0) return false;
//                foreach (var item in gcl)
//                    if (!item || !item.Match) return false;

//                int id = gcl[0].Match.ID;

//                foreach (var item in gcl)
//                    if (item.Match.ID != id) return false;
//                return true;
//            };
//            List<GridObject> mod_list;
//            for (int i = 0; i < vertSize; i++)
//            {
//                for (int j = 0; j < horSize; j++)
//                {
//                    if (Rows[i][j].Blocked || Rows[i][j].IsDisabled) continue;
//                    for (int m = 0; m < minMatch; m++)
//                    {
//                        gc_row[m] = this[i, j - m];
//                        gc_col[m] = this[i - m, j];
//                    }
//                    mod_list = new List<GridObject>();
//                    bool rowHasMatches = false;
//                    bool colHasMatches = false;

//                    if (isEqual(gc_row)) rowHasMatches = true;
//                    if (isEqual(gc_col)) colHasMatches = true;

//                    if (rowHasMatches || colHasMatches)
//                    {
//                        if (gc_col[1] && gc_col[1].Match) mod_list.Add(sC.GetMainObjectPrefab(goSet, gc_col[1].Match.ID));
//                        if (gc_row[1] && gc_row[1].Match) mod_list.Add(sC.GetMainObjectPrefab(goSet, gc_row[1].Match.ID));
//                    }
//                    if (mod_list.Count > 0)
//                    {
//                        Rows[i][j].GetComponent<GridCell>().SetObject((sC.GetMainRandomObjectPrefab(LcSet, goSet, mod_list)));
//                    }

//                }
//            }
//#if UNITY_EDITOR
//            // double test
//            for (int i = 0; i < vertSize; i++)
//            {
//                for (int j = 0; j < horSize; j++)
//                {
//                    if (Rows[i][j].Blocked || Rows[i][j].IsDisabled) continue;
//                    for (int m = 0; m < minMatch; m++)
//                    {
//                        gc_row[m] = this[i, j - m];
//                        gc_col[m] = this[i - m, j];
//                    }
//                    mod_list = new List<GridObject>();
//                    bool rowHasMatches = false;
//                    bool colHasMatches = false;

//                    if (isEqual(gc_row)) rowHasMatches = true;
//                    if (isEqual(gc_col)) colHasMatches = true;

//                    if (rowHasMatches || colHasMatches)
//                    {
//                        if (gc_col[1] && gc_col[1].Match) mod_list.Add(sC.GetMainObjectPrefab(goSet, gc_col[1].Match.ID));
//                        if (gc_row[1] && gc_row[1].Match) mod_list.Add(sC.GetMainObjectPrefab(goSet, gc_row[1].Match.ID));
//                        Debug.Log("----------------------------Found matches--------------------------------------");
//                    }
//                    if (mod_list.Count > 0)
//                    {
//                        Rows[i][j].GetComponent<GridCell>().SetObject((sC.GetMainRandomObjectPrefab(LcSet, goSet, mod_list)));
//                    }


//                }
//            }
//#endif
//        }
//    }
//}




//using Mkey;
/////
//using System.Collections.Generic;
/////
//public void fillState(DNA<char> p, SpawnController sC)
//{
//    List<GridCell> gFreeCells = GetFreeCells(mh.grid, true);
//    if (gFreeCells.Count > 0) mh.createFillPath(mh.grid);

//    int fill_cnt = 0;

//    while (gFreeCells.Count > 0)
//    {
//        //mh.fillGridByStep(gFreeCells, () => { });
//        mh.new_fillGridByStep(gFreeCells);
//        gFreeCells = GetFreeCells(mh.grid, true);
//        if (estimateMaxedOut(p, ref fill_cnt, 100)) break;
//    }

//    foreach (var item in mh.curTargets)
//    {
//        if (item.Value.Achieved) p.targetClear = true;

//        else
//        {
//            p.targetClear = false;
//            break;
//        }
//    }
//    if (p.targetClear) return;

//    p.curState = 2;
//}

//public void showState(DNA<char> p, SpawnController sC, Transform trans)
//{
//    foreach (var item in mh.curTargets)
//    {
//        if (item.Value.Achieved) p.targetClear = true;
//        else
//        {
//            p.targetClear = false;
//            break;
//        }
//    }
//    if (p.targetClear) return;

//    int mix_cnt = 0;

//    while (!estimateMaxedOut(p, ref mix_cnt, 100))
//    {
//        mh.createMatchGroups1(2, true, mh.grid, p);

//        if (mh.grid.mgList.Count == 0)
//        {
//            mh.mixGrid(null, mh.grid, trans);
//            mix_cnt++;
//        }
//        else break;
//    }


//    if (p.maxedOut) return;

//    else
//    {
//        //List<int> targetMatched = new List<int>();

//        //for (int i = 0; i < mh.grid.mgList.Count; i++)
//        //{
//        //    foreach (var item in mh.curTargets)
//        //    {
//        //        //target: matchBlock
//        //        if (item.Value.ID >= 1000 && item.Value.ID <= 1006)
//        //        {
//        //            List<int> mg_cell = mh.grid.mgList[i].Cells[0].GetGridObjectsIDs();

//        //            if (mg_cell[0] == item.Value.ID)
//        //            {
//        //                targetMatched.Add(i);
//        //                break;
//        //            }
//        //        }

//        //        //target:underlay (Grass)
//        //        else if (item.Value.ID == 200001)
//        //        {
//        //            for (int j = 0; j < mh.grid.mgList[i].Length; j++)
//        //            {
//        //                if (mh.grid.mgList[i].Cells[j].Underlay != null)
//        //                {
//        //                    targetMatched.Add(i);
//        //                    break;
//        //                }
//        //            }
//        //        }

//        //        //target: overlay (lianna) 
//        //        else if (item.Value.ID == 100004)
//        //        {
//        //            for (int j = 0; j < mh.grid.mgList[i].Length; j++)
//        //            {
//        //                if (mh.grid.mgList[i].Cells[j].Overlay != null)
//        //                {
//        //                    targetMatched.Add(i);
//        //                    break;
//        //                }
//        //            }
//        //        }

//        //        //target: blocked (Root)
//        //        else if (item.Value.ID == 101)
//        //        {
//        //            //int blockedCnt = 0;

//        //            for (int j = 0; j < mh.grid.mgList[i].Cells.Count; j++)
//        //            {
//        //                GridCell g = mh.grid.mgList[i].Cells[j];

//        //                if (g.Neighbors.Top != null)
//        //                {
//        //                    if (g.Neighbors.Top.Blocked != null && g.Neighbors.Top.Blocked.Destroyable)
//        //                    {
//        //                        targetMatched.Add(i);
//        //                        break;
//        //                    }
//        //                }

//        //                if (g.Neighbors.Left != null)
//        //                {
//        //                    if (g.Neighbors.Left.Blocked != null && g.Neighbors.Left.Blocked.Destroyable)
//        //                    {
//        //                        targetMatched.Add(i);
//        //                        break;
//        //                    }
//        //                }

//        //                if (g.Neighbors.Right != null)
//        //                {
//        //                    if (g.Neighbors.Right.Blocked != null && g.Neighbors.Right.Blocked.Destroyable)
//        //                    {
//        //                        targetMatched.Add(i);
//        //                        break;
//        //                    }
//        //                }

//        //                if (g.Neighbors.Bottom != null)
//        //                {
//        //                    if (g.Neighbors.Bottom.Blocked != null && g.Neighbors.Bottom.Blocked.Destroyable)
//        //                    {
//        //                        targetMatched.Add(i);
//        //                        break;
//        //                    }
//        //                }

//        //            }

//        //        }

//        //    }
//        //}

//        ////if (targetMatched.Count > 1)
//        ////{
//        ////    for (int i = 0; i < targetMatched.Count; i++)
//        ////    {
//        ////        for (int j = 0; j < Cells.Count; j++)
//        ////        {
//        ////            List<int> mgCell = mh.grid.Cells[j].GetGridObjectsIDs();
//        ////            for (int k = 0; k < mgCell.Count; k++) mh.tmpGrid.Cells[j].SetObject(mgCell[k]);
//        ////        }

//        ////        mh.createMatchGroups1(2, true, mh.tmpGrid, p);
//        ////        mh.tmpGrid.mgList[i].SwapEstimate();

//        ////        mh.createMatchGroups(3, false, mh.tmpGrid);

//        ////        for (int j = 0; j < mh.tmpGrid.mgList.Count; j++)
//        ////        {
//        ////            int cnt = 0;

//        ////            foreach (var item in mh.curTargets)
//        ////            {
//        ////                //target: matchBlock
//        ////                if (item.Value.ID >= 1000 && item.Value.ID <= 1006)
//        ////                {
//        ////                    for (int k = 0; k < mh.tmpGrid.mgList[j].Cells.Count; k++)
//        ////                    {
//        ////                        List<int> mgCell = mh.tmpGrid.mgList[j].Cells[k].GetGridObjectsIDs();
//        ////                        if (mgCell[0] == item.Value.ID) cnt++;
//        ////                        mh.tmpGrid.mgList[j].Cells[k].DestroyGridObjects();
//        ////                    }
//        ////                }

//        ////                //target:underlay
//        ////                else if (item.Value.ID == 200001)
//        ////                {
//        ////                    for (int k = 0; k < mh.tmpGrid.mgList[j].Cells.Count; k++)
//        ////                    {
//        ////                        List<int> mgCell = mh.tmpGrid.mgList[j].Cells[k].GetGridObjectsIDs();
//        ////                        if (mgCell.Count > 1) cnt++;
//        ////                        mh.tmpGrid.mgList[i].Cells[j].DestroyGridObjects();
//        ////                    }
//        ////                }

//        ////                //target: overlay (lianna) 
//        ////                else if (item.Value.ID == 100004)
//        ////                {
//        ////                    for (int k = 0; k < mh.tmpGrid.mgList[j].Cells.Count; k++)
//        ////                    {
//        ////                        List<int> mgCell = mh.tmpGrid.mgList[j].Cells[k].GetGridObjectsIDs();
//        ////                        if (mgCell.Count > 1)
//        ////                        {
//        ////                            if (mh.tmpGrid.mgList[i].Cells[j].Overlay.hitCnt == 1)
//        ////                            {
//        ////                                cnt++;
//        ////                                mh.tmpGrid.mgList[i].Cells[j].DestroyGridObjects();
//        ////                            }

//        ////                        }

//        ////                        else mh.tmpGrid.mgList[i].Cells[j].DestroyGridObjects();
//        ////                    }

//        ////                }

//        ////                //target: blocked (Root)
//        ////                else if (item.Value.ID == 101)
//        ////                {
//        ////                    for (int k = 0; k < mh.tmpGrid.mgList[j].Cells.Count; k++)
//        ////                    {
//        ////                        GridCell g = mh.tmpGrid.mgList[i].Cells[j];

//        ////                        if (g.Neighbors.Top != null)
//        ////                        {
//        ////                            if (g.Neighbors.Top.Blocked != null && g.Neighbors.Top.Blocked.Destroyable)
//        ////                            {
//        ////                                if (g.Neighbors.Top.Blocked.hitCnt == g.Neighbors.Top.Blocked.Protection - 1)
//        ////                                {
//        ////                                    cnt++;
//        ////                                    g.Neighbors.Top.DestroyGridObjects();
//        ////                                }
//        ////                            }
//        ////                        }

//        ////                        if (g.Neighbors.Left != null)
//        ////                        {
//        ////                            if (g.Neighbors.Left.Blocked != null && g.Neighbors.Left.Blocked.Destroyable)
//        ////                            {
//        ////                                if (g.Neighbors.Left.Blocked.hitCnt == g.Neighbors.Left.Blocked.Protection - 1)
//        ////                                {
//        ////                                    cnt++;
//        ////                                    g.Neighbors.Left.DestroyGridObjects();
//        ////                                }
//        ////                            }
//        ////                        }

//        ////                        if (g.Neighbors.Right != null)
//        ////                        {
//        ////                            if (g.Neighbors.Right.Blocked != null && g.Neighbors.Right.Blocked.Destroyable)
//        ////                            {
//        ////                                if (g.Neighbors.Right.Blocked.hitCnt == g.Neighbors.Right.Blocked.Protection - 1)
//        ////                                {
//        ////                                    cnt++;
//        ////                                    g.Neighbors.Right.DestroyGridObjects();
//        ////                                }
//        ////                            }
//        ////                        }

//        ////                        if (g.Neighbors.Bottom != null)
//        ////                        {
//        ////                            if (g.Neighbors.Bottom.Blocked != null && g.Neighbors.Bottom.Blocked.Destroyable)
//        ////                            {
//        ////                                if (g.Neighbors.Bottom.Blocked.hitCnt == g.Neighbors.Bottom.Blocked.Protection - 1)
//        ////                                {
//        ////                                    cnt++;
//        ////                                    g.Neighbors.Bottom.DestroyGridObjects();
//        ////                                }
//        ////                            }
//        ////                        }

//        ////                        mh.tmpGrid.mgList[i].Cells[j].DestroyGridObjects();
//        ////                    }

//        ////                }
//        ////            }
//        ////        }

//        ////        List<GridCell> gFreeCells = GetFreeCells(mh.tmpGrid, true);
//        ////        if (gFreeCells.Count > 0) mh.createFillPath(mh.tmpGrid);

//        ////        int fill_cnt = 0;

//        ////        while (gFreeCells.Count > 0)
//        ////        {
//        ////            //mh.fillGridByStep(gFreeCells, () => { });
//        ////            mh.new_fillGridByStep(gFreeCells);
//        ////            gFreeCells = GetFreeCells(mh.tmpGrid, true);
//        ////        }

//        ////        mh.createMatchGroups1(2, true, mh.tmpGrid, p);

//        ////    }



//        ////}















//        //if (targetMatched.Count == 0) mh.grid.mgList[0].SwapEstimate();

//        //else
//        //{
//        //    p.shortCutCnt++;
//        //    int number = Random.Range(0, targetMatched.Count - 1);
//        //    mh.grid.mgList[targetMatched[number]].SwapEstimate();
//        //}

//        p.numMove--;
//        p.curState = 2;

//    }

//}

//int collect_cnt = 0;
//public void collectState(DNA<char> p)
//{
//    if (collect_cnt >= 100)
//    {
//        p.maxedOut = true;
//        return;
//    }

//    if (mh.grid.GetFreeCells(true).Count > 0)
//    {
//        p.curState = 0;
//        return;
//    }

//    mh.createMatchGroups(3, false, mh.grid);

//    if (mh.grid.mgList.Count == 0)
//    {
//        collect_cnt = 0;
//        p.curState = 1;
//    }

//    else
//    {
//        //destory
//        for (int i = 0; i < mh.grid.mgList.Count; i++)
//        {
//            foreach (var item in mh.curTargets)
//            {
//                //target: matchBlock
//                if (item.Value.ID >= 1000 && item.Value.ID <= 1006)
//                {
//                    for (int j = 0; j < mh.grid.mgList[i].Cells.Count; j++)
//                    {
//                        List<int> mgCell = mh.grid.mgList[i].Cells[j].GetGridObjectsIDs();

//                        if (mgCell[0] == item.Value.ID)
//                        {
//                            item.Value.IncCurrCount(1);
//                        }

//                        mh.grid.mgList[i].Cells[j].DestroyGridObjects();
//                    }
//                }


//                //target:underlay
//                else if (item.Value.ID == 200001)
//                {
//                    for (int j = 0; j < mh.grid.mgList[i].Cells.Count; j++)
//                    {
//                        List<int> mgCell = mh.grid.mgList[i].Cells[j].GetGridObjectsIDs();
//                        if (mgCell.Count > 1)
//                        {
//                            item.Value.IncCurrCount(1);
//                            //mh.grid.mgList[i].Cells[j].RemoveObject(mgCell[1]);
//                        }

//                        mh.grid.mgList[i].Cells[j].DestroyGridObjects();
//                    }
//                }

//                //target: overlay (lianna) 
//                else if (item.Value.ID == 100004)
//                {
//                    for (int j = 0; j < mh.grid.mgList[i].Cells.Count; j++)
//                    {
//                        List<int> mgCell = mh.grid.mgList[i].Cells[j].GetGridObjectsIDs();
//                        if (mgCell.Count > 1)
//                        {
//                            mh.grid.mgList[i].Cells[j].Overlay.hitCnt++;

//                            if (mh.grid.mgList[i].Cells[j].Overlay.hitCnt == 2)
//                            {
//                                item.Value.IncCurrCount(1);
//                                mh.grid.mgList[i].Cells[j].DestroyGridObjects();
//                                //mh.grid.mgList[i].Cells[j].RemoveObject(mgCell[1]);
//                            }
//                        }

//                        else mh.grid.mgList[i].Cells[j].DestroyGridObjects();
//                    }

//                }

//                //target: blocked (Root)
//                else if (item.Value.ID == 101)
//                {
//                    for (int j = 0; j < mh.grid.mgList[i].Cells.Count; j++)
//                    {
//                        GridCell g = mh.grid.mgList[i].Cells[j];

//                        if (g.Neighbors.Top != null)
//                        {
//                            if (g.Neighbors.Top.Blocked != null && g.Neighbors.Top.Blocked.Destroyable)
//                            {
//                                g.Neighbors.Top.Blocked.hitCnt++;

//                                if (g.Neighbors.Top.Blocked.hitCnt == g.Neighbors.Top.Blocked.Protection)
//                                {
//                                    item.Value.IncCurrCount(1);
//                                    g.Neighbors.Top.DestroyGridObjects();
//                                }
//                            }
//                        }

//                        if (g.Neighbors.Left != null)
//                        {
//                            if (g.Neighbors.Left.Blocked != null && g.Neighbors.Left.Blocked.Destroyable)
//                            {
//                                g.Neighbors.Left.Blocked.hitCnt++;

//                                if (g.Neighbors.Left.Blocked.hitCnt == g.Neighbors.Left.Blocked.Protection)
//                                {
//                                    item.Value.IncCurrCount(1);
//                                    g.Neighbors.Left.DestroyGridObjects();
//                                }
//                            }
//                        }

//                        if (g.Neighbors.Right != null)
//                        {
//                            if (g.Neighbors.Right.Blocked != null && g.Neighbors.Right.Blocked.Destroyable)
//                            {
//                                g.Neighbors.Right.Blocked.hitCnt++;

//                                if (g.Neighbors.Right.Blocked.hitCnt == g.Neighbors.Right.Blocked.Protection)
//                                {
//                                    item.Value.IncCurrCount(1);
//                                    g.Neighbors.Right.DestroyGridObjects();
//                                }
//                            }
//                        }

//                        if (g.Neighbors.Bottom != null)
//                        {
//                            if (g.Neighbors.Bottom.Blocked != null && g.Neighbors.Bottom.Blocked.Destroyable)
//                            {
//                                g.Neighbors.Bottom.Blocked.hitCnt++;

//                                if (g.Neighbors.Bottom.Blocked.hitCnt == g.Neighbors.Bottom.Blocked.Protection)
//                                {
//                                    item.Value.IncCurrCount(1);
//                                    g.Neighbors.Bottom.DestroyGridObjects();
//                                }
//                            }
//                        }

//                        mh.grid.mgList[i].Cells[j].DestroyGridObjects();
//                    }



//                }

//            }
//        }

//        //for (int i = 0; i < mh.grid.mgList.Count; i++)
//        //{
//        //    if (mh.grid.mgList[i] != null)
//        //    {
//        //        foreach (GridCell c in mh.grid.mgList[i].Cells) c.DestroyGridObjects();
//        //    }
//        //}

//        p.allMove++;
//        p.curState = 0;
//    }

//    collect_cnt++;
//}
