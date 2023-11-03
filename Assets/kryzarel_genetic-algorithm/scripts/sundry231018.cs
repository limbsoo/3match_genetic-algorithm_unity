

//20231018///////////////////////////////////////////////////////////////////////////////////////////////
//MatchGrid.cs-------------------------------------------//


//bool estimateMaxedOut(DNA<char> p, ref int cnt, int max)
//{
//    if(max <= cnt) p.maxedOut = true;
//    return p.maxedOut;
//}




//public int estimateTargetIsMatch(MatchGroup mg, int targetID)
//{
//    int result = 0;
//    List<int> mg_cell = mg.Cells[0].GetGridObjectsIDs();
//    if (mg_cell[0] == targetID) result = mg.Length;
//    return result;
//}

//public int estimateTargetIsUnderlay(MatchGroup mg, int targetID)
//{
//    int result = 0;
//    for (int i = 0; i < mg.Length; i++) if (mg.Cells[i].Underlay != null) result++;
//    return result;
//}
//public int estimateTargetIsOverlay(MatchGroup mg, int targetID)
//{
//    int result = 0;
//    for (int i = 0; i < mg.Length; i++) if (mg.Cells[i].Overlay != null) result++;
//    return result;
//}

//public bool estimateNeighborBlocked(GridCell g)
//{
//    if(g != null)
//    {
//        if (g.Blocked != null && g.Blocked.Destroyable) return true;
//    }
//    return false;
//}

//public int estimateTargetIsBlocked(MatchGroup mg, int targetID)
//{
//    int result = 0;
//    for (int i = 0; i < mg.Cells.Count; i++)
//    {
//        if (mg.Cells[i].Neighbors.Top != null)
//        {
//            if (estimateNeighborBlocked(mg.Cells[i].Neighbors.Top)) result++;
//        }

//        if (mg.Cells[i].Neighbors.Left != null)
//        {
//            if (estimateNeighborBlocked(mg.Cells[i].Neighbors.Left)) result++;
//        }

//        if (mg.Cells[i].Neighbors.Right != null)
//        {
//            if (estimateNeighborBlocked(mg.Cells[i].Neighbors.Right)) result++;
//        }

//        if (mg.Cells[i].Neighbors.Bottom != null)
//        {
//            if (estimateNeighborBlocked(mg.Cells[i].Neighbors.Bottom)) result++;
//        }
//    }
//    return result;
//}

//public int estimateIncludeTarget(MatchGroup mg)
//{
//    int includeTargetCnt = 0;

//    foreach (var item in m3h.curTargets)
//    {
//        if (!item.Value.Achieved)
//        {
//            switch (item.Value.ID)
//            {
//                case int n when (n >= 1000 && n <= 1006):
//                    includeTargetCnt = estimateTargetIsMatch(mg, n);
//                    break;
//                case 200001:
//                    includeTargetCnt = estimateTargetIsUnderlay(mg, 200001);
//                    break;
//                case 100004:
//                    includeTargetCnt = estimateTargetIsOverlay(mg, 100004);
//                    break;
//                case 101:
//                    includeTargetCnt = estimateTargetIsBlocked(mg, 101);
//                    break;
//            }
//        }

//    }
//    return includeTargetCnt;
//}



//public void fillState(DNA<char> p, SpawnController sC)
//{
//    Debug.Log("fill");
//    List<GridCell> gFreeCells = GetFreeCells(m3h.grid, true);
//    if (gFreeCells.Count > 0) m3h.createFillPath(m3h.grid);

//    int fill_cnt = 0;

//    while (gFreeCells.Count > 0)
//    {
//        //mh.fillGridByStep(gFreeCells, () => { });
//        m3h.new_fillGridByStep(gFreeCells);
//        gFreeCells = GetFreeCells(m3h.grid, true);
//        if (estimateMaxedOut(p, ref fill_cnt, 100)) break;
//    }

//    foreach (var item in m3h.curTargets)
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
//    Debug.Log("show");
//    foreach (var item in m3h.curTargets)
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
//        //mh.createMatchGroups1(2, true, mh.grid);
//        m3h.createMatchGroups(2, true, m3h.grid);

//        if (m3h.grid.mgList.Count == 0)
//        {
//            m3h.mixGrid(null, m3h.grid, trans);
//            mix_cnt++;
//        }
//        else break;
//    }

//    if (p.maxedOut) return;

//    if (m3h.grid.mgList.Count > 1)
//    {
//        m3h.isCounting = true;
//        m3h.count = 0;
//        List<int> predicCollect = new List<int>();

//        for (int i = 0; i < m3h.grid.mgList.Count; i++)
//        {
//            int predictCnt = 0;
//            predictCnt += estimateIncludeTarget(m3h.grid.mgList[i]);
//            predicCollect.Add(predictCnt);
//        }

//        int max = 0;
//        int maxIdx = 0;

//        for (int i = 0; i < predicCollect.Count; i++)
//        {
//            if (max < predicCollect[i])
//            {
//                max = predicCollect[i];
//                maxIdx = i;
//            }
//        }

//        if(maxIdx == 0)
//        {
//            int randNum = Random.Range(0, predicCollect.Count - 1);
//            m3h.grid.mgList[randNum].SwapEstimate();
//        }

//        else
//        m3h.grid.mgList[maxIdx].SwapEstimate();
//    }

//    //if (mh.grid.mgList.Count > 1)
//    //{
//    //    mh.isCounting = true;
//    //    mh.count = 0;
//    //    List<int> predicCollect = new List<int>();

//    //    GridCell[,] curCell = new GridCell[mh.grid.Columns.Count, mh.grid.Rows.Count];

//    //    for (int i = 0; i < mh.grid.Columns.Count; i++)
//    //    {
//    //        for (int j = 0; j < mh.grid.Rows.Count; j++)
//    //        {
//    //            curCell[i, j] = mh.grid.Cells[i * mh.grid.Rows.Count + j];
//    //        }
//    //    }

//    //    for (int i = 0; i < mh.grid.mgList.Count; i++)
//    //    {
//    //        int predictCnt = 0;
//    //        predictCnt += estimateIncludeTarget(mh.grid.mgList[i]);

//    //        for (int j = 0; j < mh.grid.mgList[i].Length; j++)
//    //        {
//    //            GridCell cur = curCell[mh.grid.mgList[i].Cells[j].Column, mh.grid.mgList[i].Cells[j].Row];

//    //            if (mh.grid.mgList[i].Cells[j].fillPathToSpawner == null) cur.isNotFill = true;

//    //            for (int k = 0; k < mh.grid.mgList[i].Cells[j].fillPathToSpawner.Count; k++)
//    //            {
//    //                curCell[mh.grid.mgList[i].Cells[j].Column, mh.grid.mgList[i].Cells[j].Row] = mh.grid.mgList[i].Cells[j].fillPathToSpawner[k];
//    //                cur = mh.grid.mgList[i].Cells[j].fillPathToSpawner[k];
//    //            }
//    //            cur.isNotFill = true;
//    //        }

//    //        mh.new_createMatchGroups(2, true, curCell);
//    //        predictCnt += mh.l_mgList.Count;

//    //        predicCollect.Add(predictCnt);
//    //    }

//    //    int max = 0;
//    //    int maxIdx = 0;

//    //    for (int i = 0; i < predicCollect.Count; i++)
//    //    {
//    //        if (max < predicCollect[i])
//    //        {
//    //            max = predicCollect[i];
//    //            maxIdx = i;
//    //        }
//    //    }
//    //    mh.grid.mgList[maxIdx].new_SwapEstimate();
//    //}

//    else m3h.grid.mgList[0].new_SwapEstimate();

//    p.swapCnt--;
//    p.curState = 2;
//}

//public void destroyAndCntMatch(MatchGroup mg, TargetData td)
//{
//    for (int i = 0; i < mg.Cells.Count; i++)
//    {
//        List<int> mgCells = mg.Cells[i].GetGridObjectsIDs();
//        if (mgCells[0] == td.ID)
//        {
//            if (!m3h.isCounting) td.IncCurrCount(1);
//            else m3h.count++;
//        }
//        mg.Cells[i].DestroyGridObjects();
//    }
//}
//public void destroyAndCntUnderlay(MatchGroup mg, TargetData td)
//{
//    for (int i = 0; i < mg.Cells.Count; i++)
//    {
//        List<int> mgCells = mg.Cells[i].GetGridObjectsIDs();
//        if (mgCells.Count > 1)
//        {
//            if (!m3h.isCounting) td.IncCurrCount(1);
//            else m3h.count++;
//        }
//        mg.Cells[i].DestroyGridObjects();
//    }
//}

//public void destroyAndCntOverlay(MatchGroup mg, TargetData td)
//{
//    for (int i = 0; i < mg.Cells.Count; i++)
//    {
//        List<int> mgCells = mg.Cells[i].GetGridObjectsIDs();

//        if (mgCells.Count > 1)
//        {
//            if (!m3h.isCounting)
//            {
//                mg.Cells[i].Overlay.hitCnt++;

//                if (mg.Cells[i].Overlay.hitCnt == 2)
//                {
//                    td.IncCurrCount(1);
//                    mg.Cells[i].DestroyGridObjects();
//                }
//            } 

//            else m3h.count++;
//        }
//        else mg.Cells[i].DestroyGridObjects();
//    }
//}

//public void blockedHit(GridCell g, TargetData td)
//{
//   if (g.Blocked.hitCnt == g.Blocked.Protection)
//    {
//        td.IncCurrCount(1);
//        g.DestroyGridObjects();
//    }
//}


//public int destroyAndCntBlocked(MatchGroup mg, TargetData td)
//{
//    int result = 0;
//    for (int i = 0; i < mg.Cells.Count; i++)
//    {
//        if (mg.Cells[i].Neighbors.Top != null)
//        {
//            if (estimateNeighborBlocked(mg.Cells[i].Neighbors.Top))
//            {
//                if (!m3h.isCounting)
//                {
//                    mg.Cells[i].Neighbors.Top.Blocked.hitCnt++;
//                    blockedHit(mg.Cells[i].Neighbors.Top, td);
//                }

//                else m3h.count++;
//            }
//        }

//        if (mg.Cells[i].Neighbors.Left != null)
//        {
//            if (estimateNeighborBlocked(mg.Cells[i].Neighbors.Left))
//            {
//                if (!m3h.isCounting)
//                {
//                    mg.Cells[i].Neighbors.Left.Blocked.hitCnt++;
//                    blockedHit(mg.Cells[i].Neighbors.Left, td);
//                }

//                else m3h.count++;
//            }
//        }

//        if (mg.Cells[i].Neighbors.Right != null)
//        {
//            if (estimateNeighborBlocked(mg.Cells[i].Neighbors.Right))
//            {
//                if (!m3h.isCounting)
//                {
//                    mg.Cells[i].Neighbors.Right.Blocked.hitCnt++;
//                    blockedHit(mg.Cells[i].Neighbors.Right, td);
//                }

//                else m3h.count++;
//            }
//        }

//        if (mg.Cells[i].Neighbors.Bottom != null)
//        {
//            if (estimateNeighborBlocked(mg.Cells[i].Neighbors.Bottom))
//            {
//                if (!m3h.isCounting)
//                {
//                    mg.Cells[i].Neighbors.Bottom.Blocked.hitCnt++;
//                    blockedHit(mg.Cells[i].Neighbors.Bottom, td);
//                }

//                else m3h.count++;
//            }
//        }


//    }

//    for (int i = 0; i < mg.Cells.Count; i++)
//    {
//        mg.Cells[i].DestroyGridObjects();
//    }

//    return result;
//}


//public void destroyAndCntBlocks(MatchGroup mg)
//{
//    foreach (var item in m3h.curTargets)
//    {
//        if (!item.Value.Achieved)
//        {
//            switch (item.Value.ID)
//            {
//                case int n when (n >= 1000 && n <= 1006):
//                    destroyAndCntMatch(mg, item.Value);
//                    break;
//                case 200001:
//                    destroyAndCntUnderlay(mg, item.Value);
//                    break;
//                case 100004:
//                    destroyAndCntOverlay(mg, item.Value);
//                    break;
//                case 101:
//                    destroyAndCntBlocked(mg, item.Value);
//                    break;
//            }
//        }
//    }
//}


//int collect_cnt = 0;
//public void collectState(DNA<char> p)
//{
//    Debug.Log("collect");


//    if (collect_cnt >= 100)
//    {
//        p.maxedOut = true;
//        return;
//    }

//    if (m3h.grid.GetFreeCells(true).Count > 0)
//    {
//        p.curState = 0;
//        return;
//    }

//    m3h.createMatchGroups(3, false, m3h.grid);

//    if (m3h.grid.mgList.Count == 0)
//    {
//        collect_cnt = 0;
//        p.curState = 1;
//    }

//    else
//    {
//        for (int i = 0; i < m3h.grid.mgList.Count; i++)
//        {
//            destroyAndCntBlocks(m3h.grid.mgList[i]);
//        }

//        p.allMove++;
//        p.curState = 0;
//    }

//    //collect_cnt++;
//}


//public void addBlockedTargetEstimateFeasible(DNA<char> p, SpawnController sC)
//{
//    p.newGenes = new char[Cells.Count];

//    List<int> newGenes = new List<int>();
//    for (int i = 0; i < Cells.Count; i++)
//    {
//        if (p.genes[i] == '1') newGenes.Add(1);
//        else newGenes.Add(0);
//    }

//    for (int i = 0; i < m3h.targetArray.Length; i++) newGenes[m3h.targetArray[i]] = 1;
//    for (int i = 0; i < Cells.Count; i++) p.newGenes[i] = (char)(newGenes[i] + '0');

//}


//public void initGrid(DNA<char> p, SpawnController sC)
//{
//    BlockedObject b = sC.GetObstacleObjectPrefab(LcSet, goSet);

//    for (int i = 0; i < m3h.grid.Cells.Count; i++)
//    {
//        if (p.genes[i] == '1') m3h.grid.Cells[i].SetObject1(b);

//        else
//        {
//            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
//            m3h.grid.Cells[i].SetObject1(m);
//        }
//    }
//}

//public void setTargetBlock(DNA<char> p, SpawnController sC, int targetID)
//{
//    //target: underlay (Grass)
//    if (targetID == 200001)
//    {
//        UnderlayObject u = sC.GetSelectUnderlayObjectPrefab(LcSet, goSet);
//        for (int i = 0; i < m3h.targetArray.Length; i++) m3h.grid.Cells[m3h.targetArray[i]].SetObject1(u);
//    }

//    //target: overlay (Lianna) 
//    else if (targetID == 100004)
//    {
//        OverlayObject o = sC.GetSelectOverlayObjectPrefab(LcSet, goSet);
//        for (int i = 0; i < m3h.targetArray.Length; i++) m3h.grid.Cells[m3h.targetArray[i]].SetObject1(o);
//    }

//    //target: blocked (Root)
//    else if (targetID == 101)
//    {
//        BlockedObject b = sC.GetSelectBreakableBlockedObjectPrefab(LcSet, goSet);

//        for (int i = 0; i < m3h.targetArray.Length; i++)
//        {
//            m3h.grid.Cells[m3h.targetArray[i]].DestroyGridObjects();
//            m3h.grid.Cells[m3h.targetArray[i]].SetObject1(b);
//        }
//    }
//}


//public void calculateFitness(DNA<char> p, SpawnController sC, Transform trans)
//{
//    if (p.isFeasible)
//    {
//        //MatchGroup matchGroup = new MatchGroup();

//        ////근데 블록정보를 다지웠었는데 그러면 초기화 필요있나
//        //for (int i = 0; i < m3h.grid.Cells.Count; i++)
//        //{
//        //    m3h.grid.Cells[i].cellMatchPotential = 0;
//        //}

//        //for (int i = 0; i < m3h.grid.Cells.Count; i++)
//        //{
//        //    matchGroup.countPossible(m3h.grid, i);
//        //}

//        ////ga.possibleCounting = new int[m3h.colSize * m3h.rowSize];

//        //for (int i = 0; i < m3h.grid.Cells.Count; i++)
//        //{
//        //    p.mapMatchPotential += m3h.grid.Cells[i].cellMatchPotential;
//        //    //ga.possibleCounting[m3h.grid.Cells[i].matchPotential]++;
//        //    p.mapMatchPotentialList.Add(m3h.grid.Cells[i].cellMatchPotential);

//        //    if (m3h.grid.Cells[i].DynamicObject == null) p.obstacleCnt++;
//        //}


//        //m3h.countObstacleData(p);

//        //if (m3h.isPlay) makeFeasibleFitness(p, sC, trans);

//        //else p.calculateFeasibleFitness();



//        p.calculateFeasibleFitness(m3h.wantDifficulty, m3h.difficultyTolerance);
//        //p.calculateFeasibleFitness();

//        ga.feasiblePopulation.Add(p);
//        ga.feasibleFitnessSum += p.fitness;
//    }

//    else
//    {
//        p.calculateInfeasibleFitness();
//        ga.infeasiblePopulation.Add(p);
//        ga.infeasibleFitnessSum += p.fitness;
//    }
//}


//public void makeFeasibleFitness(DNA<char> p, SpawnController sC, Transform trans)
//{
//    List<int> moveContainer = new List<int>();
//    List<int> nMoveContainer = new List<int>();
//    List<int> shortCutCntContainer = new List<int>();
//    List<int> possibleList = new List<int>();
//    List<int> spawnList = new List<int>();

//    setOnlyUnbreakableBlock(p, sC);

//    for (int repeatIdx = 0; repeatIdx < ga.repeat; repeatIdx++)
//    {
//        setOnlyMatchBlock(p, sC);
//        foreach (var item in m3h.curTargets) setTargetBlock(p, sC, item.Value.ID);
//        m3h.createFillPath(m3h.grid);
//        m3h.grid.RemoveMatches1();

//        if (ga.isPossible)
//        {
//            MatchGroup matchGroup = new MatchGroup();
//            for (int i = 0; i < m3h.grid.Cells.Count; i++) m3h.grid.Cells[i].cellMatchPotential = 0;
//            for (int i = 0; i < m3h.grid.Cells.Count; i++) matchGroup.countPossible(m3h.grid, i);
//            //ga.possibleCounting = new int[m3h.grid.Columns.Count * m3h.grid.Rows.Count];

//            for (int i = 0; i < m3h.grid.Cells.Count; i++)
//            {
//                //ga.possibleCnt += m3h.grid.Cells[i].cellMatchPotential;
//                //ga.possibleCounting[m3h.grid.Cells[i].matchPotential]++;
//                ga.possibleCountingList.Add(m3h.grid.Cells[i].cellMatchPotential);

//                if (m3h.grid.Cells[i].DynamicObject == null) p.obstacleCnt++;
//            }

//            ga.blockeCnt = p.obstacleCnt;
//            ga.isPossible = false;
//        }

//        collect_cnt = 0;
//        p.allMove = 0;
//        p.swapCnt = ga.moveLimit;
//        p.targetClear = false;
//        p.maxedOut = false;
//        p.curState = 0;
//        p.obstructionRate = 0;
//        p.shortCutCnt = 0;

//        foreach (var item in m3h.curTargets) item.Value.InitCurCount();

//        while (p.swapCnt > 0 && p.targetClear == false && p.maxedOut == false)
//        {
//            switch (p.curState)
//            {
//                case 0: fillState(p, sC);
//                    break;
//                case 1: showState(p, sC, trans);
//                    break;
//                case 2: collectState(p);
//                    break;
//            }

//            if (p.allMove > 10000) return;

//            m3h.isCounting = false;
//            Debug.Log("1roate");
//        }
//        Debug.Log("endPlay");

//        List<GridCell> gFreeCells = GetFreeCells(m3h.grid, true);

//        if (gFreeCells.Count > 0)
//        {
//            m3h.createFillPath(m3h.grid);
//            while (gFreeCells.Count > 0)
//            {
//                m3h.new_fillGridByStep(gFreeCells);
//                gFreeCells = GetFreeCells(m3h.grid, true);
//            }
//        }

//        shortCutCntContainer.Add(p.shortCutCnt);

//        if (p.maxedOut == true || p.targetClear == false)
//        {
//            moveContainer.Add(ga.moveLimit);
//            nMoveContainer.Add(ga.moveLimit);
//        }

//        else
//        {
//            moveContainer.Add(ga.moveLimit - p.swapCnt);
//            nMoveContainer.Add(p.allMove);
//        }

//    }

//    //p.calculateFeasibleFitness(moveContainer, (double)ga.targetMove, ga.targetStd);



//    ga.repeatMovements.Add(moveContainer);
//    ga.repeatMovementsCnt++;
//    ga.allMovements.Add(nMoveContainer);
//    ga.shorCutRates.Add(shortCutCntContainer);
//    if (ga.curGenerationBestFitness < p.fitness)
//    {
//        ga.curGenerationBestFitness = p.fitness;
//        ga.curGenerationBestMean = p.mean;
//        ga.curGenerationBestStd = p.stanardDeviation;
//        ga.curBestMoves = moveContainer;
//    }
//    if (ga.bestFitness < p.fitness)
//    {
//        ga.bestFitness = p.fitness;
//        ga.bestMeanMove = p.mean;
//        ga.bestStd = p.stanardDeviation;
//        ga.bestMoves = moveContainer;
//    }
//}


//public char[] getGenes()
//{
//    char[] genes = new char[Cells.Count];

//    string[] map = {

//                "0000101010000101000000110000110001110000000010010000000101001001101010000001110000000000001000000000000000100010000100001" ,
//                "0111010011110010100000001000100111000000001100000000111010000000011101000000110000000011100100001000000000100100001000000" ,
//                "0000000110000001000001000000001001100000111010000111111010000000110000100100111110000000000000010000000001111110000001111" ,
//                "1011110100000000010001110000101001000000010111100001100101010000000001000011101010000011100100000111000000000110110110001" ,
//                "0001000111010010100110001100101100001001011000000000000000101000011000010010100000000001000011011001100010000111101101011" ,
//                "0000100000110100100000111000100011110101000011000010001100101101000000110010101100000000000010100000111100000101111001011" ,
//                "1110010110111110011000111100001010110000011000101000010000011010100010011000000101100001100000010001101101000110011000000" ,
//                "0010010101100111101010101000001000011101010010001100000000100000000110000110000111001100110001000110000000001111010011000" ,
//                "0100001110001111001000101110100000000000011000101111100101010110001000100100110001100000100000110010010011001101100000001" ,
//                "1110100011111100010001100000111111111101001000110010010011101000000010001100010001000001001110010010110000010011101110110"
//            };

//    int[] difficult =
//    {
//               567 ,
//               546 ,
//               454 ,
//               400 ,
//               360 ,
//               302 ,
//               250 ,
//               229 ,
//               231 ,
//               155
//            };

//    for (int i = 0; i < map[m3h.csvCnt].Length; i++)
//    {
//        genes[i] = map[m3h.csvCnt][i];
//    }

//    m3h.wantDifficulty = difficult[m3h.csvCnt];





//    //char[] genes = new char[Cells.Count];

//    ////init gene
//    //List<int> cellList = new List<int>();
//    //for (int i = 0; i < Cells.Count; i++) cellList.Add(0);

//    ////obstacle만
//    //if (m3h.spawnObstacle)
//    //{
//    //    if(m3h.spawnObstacleRandomInCnt)
//    //    {
//    //        for (int i = 0; i < m3h.obstacleCnt; i++)
//    //        {
//    //            int randNum = Random.Range(0, Cells.Count - 1);
//    //            cellList[randNum] = 1;
//    //        }
//    //    }

//    //    else
//    //    {
//    //        for (int i = 0; i < m3h.obstacleArray.Length; i++) cellList[m3h.obstacleArray[i]] = 1;
//    //    }
//    //}


//    ////if (m3h.spawnObstacle)
//    ////{
//    ////    if (m3h.setObstaclePosition)
//    ////    {
//    ////        for (int i = 0; i < m3h.obstacleArray.Length; i++) cellList[m3h.obstacleArray[i]] = 1;
//    ////    }

//    ////    else
//    ////    {
//    ////        if (m3h.obstacleCnt > 0)
//    ////        {
//    ////            for (int i = 0; i < m3h.obstacleCnt; i++)
//    ////            {
//    ////                int randNum = Random.Range(0, Cells.Count - 1);
//    ////                cellList[randNum] = 1;
//    ////            }
//    ////        }

//    ////        else
//    ////        {
//    ////            for (int i = 0; i < Cells.Count; i++) cellList.Add(Random.Range(0, 2));
//    ////        }
//    ////    }
//    ////}

//    ////if (m3h.spawnTarget)
//    ////{
//    ////    for (int i = 0; i < m3h.targetArray.Length; i++) cellList[m3h.targetArray[i]] = 0;
//    ////}

//    //// char 변환
//    //for (int i = 0; i < Cells.Count; i++) genes[i] = (char)(cellList[i] + '0');

//    return genes;
//}


//public void setBlockedFromNewGene(DNA<char> p, SpawnController sC)
//{
//    BlockedObject b = sC.GetObstacleObjectPrefab(LcSet, goSet);

//    for (int i = 0; i < m3h.grid.Cells.Count; i++)
//    {
//        if (p.newGenes[i] == '1') m3h.grid.Cells[i].SetObject1(b);

//        else
//        {
//            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
//            m3h.grid.Cells[i].SetObject1(m);
//        }
//    }
//}


//public void setObstacleFromGene(DNA<char> p, SpawnController sC)
//{
//    BlockedObject b = sC.GetObstacleObjectPrefab(LcSet, goSet);
//    BlockedObject b0 = sC.GetSelectBreakableBlockedObjectPrefab(LcSet, goSet);
//    OverlayObject o = sC.GetSelectOverlayObjectPrefab(LcSet, goSet);

//    b0.hitCnt = m3h.blockedObjectHitCnt;
//    o.hitCnt = m3h.overlayObjectHitCnt;

//    for (int i = 0; i < m3h.grid.Cells.Count; i++)
//    {
//        if (p.genes[i] == '1')
//        {
//            if (m3h.onlySpawnObstacleObject)
//            {
//                m3h.grid.Cells[i].SetObject1(b);
//                p.gl.Add(0);
//            }

//            else if (m3h.onlySpawnBlockedObject)
//            {
//                m3h.grid.Cells[i].SetObject1(b0);
//                p.gl.Add(1);
//            }

//            else if (m3h.onlySpawnOverlayObject)
//            {
//                MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
//                m3h.grid.Cells[i].SetObject1(m);
//                m3h.grid.Cells[i].SetObject1(o);
//                p.gl.Add(2);
//            }
//        }

//        else
//        {
//            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
//            m3h.grid.Cells[i].SetObject1(m);
//            p.gl.Add(3);
//        }

//    }



//    //BlockedObject b;
//    //OverlayObject o;

//    //if (m3h.isBreakableObstacle)
//    //{
//    //    b = sC.GetSelectBreakableBlockedObjectPrefab(LcSet, goSet);
//    //    b.hitCnt = m3h.obstacleHitCnt;

//    //    o = sC.GetSelectOverlayObjectPrefab(LcSet, goSet);
//    //    o.hitCnt = m3h.obstacleHitCnt;
//    //}

//    //else b = sC.GetObstacleObjectPrefab(LcSet, goSet);


//    //for (int i = 0; i < m3h.grid.Cells.Count; i++)
//    //{
//    //    if (p.genes[i] == '1') m3h.grid.Cells[i].SetObject1(b);

//    //    else
//    //    {
//    //        MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
//    //        m3h.grid.Cells[i].SetObject1(m);
//    //    }
//    //}
//}


//public void addObstacleFromTarget(SpawnController sC)
//{
//    BlockedObject b = sC.GetObstacleObjectPrefab(LcSet, goSet);

//    for (int i = 0; i < m3h.targetArray.Length; i++)
//    {
//        m3h.grid.Cells[m3h.targetArray[i]].SetObject1(b);
//    }
//}

//public void setOnlyUnbreakableBlock(DNA<char> p, SpawnController sC)
//{
//    BlockedObject b = sC.GetObstacleObjectPrefab(LcSet, goSet);

//    for (int i = 0; i < m3h.grid.Cells.Count; i++)
//    {
//        if (p.genes[i] == '1') m3h.grid.Cells[i].SetObject1(b);
//    }
//}

//public void setOnlyMatchBlock(DNA<char> p, SpawnController sC)
//{
//    for (int i = 0; i < m3h.grid.Cells.Count; i++)
//    {
//        if (p.genes[i] != '1')
//        {
//            m3h.grid.Cells[i].DestroyGridObjects();

//            MatchObject m = sC.GetRandomObjectPrefab(LcSet, goSet);
//            m3h.grid.Cells[i].SetObject1(m);

//        }
//    }
//}


//public void calculateFitnessThroughPlay(DNA<char> p, SpawnController sC, Transform trans, bool isObstacleTarget, CSVFileWriter cs)
//{
//    List<int> bestSwapCntContainer = new List<int>();
//    List<int> swapCntContainer = new List<int>();
//    List<int> matchCntContainer = new List<int>();

//    //initGrid();
//    //if(m3h.isBreakableObstacle) setOnlyUnbreakableBlock(p, sC);

//    for (int repeatIdx = 0; repeatIdx < m3h.limits.repeat; repeatIdx++)
//    {
//        //if (m3h.isBreakableObstacle) setOnlyMatchBlock(p, sC);
//        //else setObstacleFromGene(p, sC);

//        //setObstacleFromGene(p, sC);
//        initGrid();
//        setGridRepeat(ga.population[0], sC);

//        foreach (var item in m3h.curTargets)
//        {
//            //if (isObstacleTarget) setTargetBlock(p, sC, item.Value.ID);
//            item.Value.InitCurCount();
//        }

//        //m3h.createFillPath(m3h.grid);
//        m3h.new_createFillPath();
//        m3h.grid.RemoveMatches1();

//        p.swapCnt = 0;
//        p.matchCnt = 0;

//        m3h.plays = new PlayHelper();

//        while (m3h.plays.isClear == false && m3h.plays.isError == false && m3h.plays.playCnt < m3h.limits.find)
//        {
//            switch (m3h.plays.curState)
//            {
//                case 0:
//                    m3h.fillFreeCells();
//                    break;
//                case 1:
//                    m3h.swapCells(p, trans);
//                    break;
//                case 2:
//                    m3h.matchAndDestory(p);
//                    break;
//            }
//            m3h.plays.playCnt++;
//        }
//        Debug.Log("endPlay");

//        if (m3h.plays.isError == true || m3h.plays.isClear == false)
//        {
//            swapCntContainer.Add(999999);
//            matchCntContainer.Add(999999);
//            bestSwapCntContainer.Add(999999);
//        }

//        else
//        {
//            swapCntContainer.Add(p.swapCnt);
//            matchCntContainer.Add(p.matchCnt);
//            bestSwapCntContainer.Add(p.swapCnt);
//        }

//        Debug.Log("count++");
//    }
//    cs.swapContainer.Add(swapCntContainer);
//    cs.matchContainer.Add(matchCntContainer);
//    cs.bestSwapContainer.Add(bestSwapCntContainer);

//    ga.generation++;
//}


//void getMatch3Level(Transform trans)
//{
//    SpawnController sC = SpawnController.Instance;
//    CSVFileWriter cs = new CSVFileWriter(m3h);

//    Debug.Log("beforefitness");

//    bool isObstacleTarget = false;

//    foreach (var item in m3h.curTargets) //그냥 유전자는 obstacle과 빈셀로만 구성시켜야겠다
//    {
//        isObstacleTarget = estimateObstacleTarget(item.Value.ID);
//        if (isObstacleTarget) break;
//    }

//    if (m3h.isCheckSwapTest)
//    {
//        if (m3h.isUsingGenetic)
//        {
//            for (int i = 0; i < m3h.limits.geneticGeneration; i++)
//            {
//                for (int j = 0; j < ga.population.Count; j++)
//                {
//                    initGrid();
//                    setObstacleFromGene(ga.population[j], sC);
//                    estimateIsFeasible(ga.population[j]);

//                    if (ga.population[j].isFeasible)
//                    {
//                        m3h.countObstacle(ga.population[j]);
//                        //countPerObstacle(ga.population[j], sC);
//                        //m3h.countObstacleData(ga.population[j]);
//                    }

//                    calculateFitness(ga.population[j], sC, trans);

//                    if (ga.bestFitness < ga.population[j].fitness && ga.population[j].isFeasible) ga.bestFitness = ga.population[j].fitness;

//                    if (ga.bestFitness >= ga.targetFitness)
//                    {
//                        ga.population[0] = ga.population[j];
//                        break;
//                    }
//                }

//                if (ga.bestFitness >= ga.targetFitness) break;
//                else ga.newGeneration();
//            }

//            if (ga.bestFitness >= ga.targetFitness)
//            {
//                if (m3h.onlySpawnObstacleObject || m3h.onlySpawnBlockedObject || m3h.onlySpawnOverlayObject)
//                {
//                    countOneKindObstacle(ga.population[0], sC);
//                    m3h.cntMatchPottentials(ga.population[0]);
//                }

//                else countPerObstacle(ga.population[0], sC);

//                for (int i = 0; i < m3h.limits.generation; i++)
//                {
//                    calculateFitnessThroughPlay(ga.population[0], sC, trans, isObstacleTarget, cs);
//                }
//            }
//        }

//        //else
//        //{
//        //    m3h.findingFeasible = 0;

//        //    for (; m3h.findingFeasible < 300; m3h.findingFeasible++)
//        //    {
//        //        initGrid();
//        //        setObstacleFromGene(ga.population[0], sC);
//        //        estimateIsFeasible(ga.population[0]);

//        //        if (ga.population[0].isFeasible)
//        //        {
//        //            countPerObstacle(ga.population[0], sC);
//        //            m3h.countObstacleData(ga.population[0]);

//        //            for (int i = 0; i < 5; i++)
//        //            {
//        //                calculateFitnessThroughPlay(ga.population[0], sC, trans, isObstacleTarget, cs);
//        //                if (ga.bestFitness < ga.population[0].fitness) ga.bestFitness = ga.population[0].fitness;
//        //            }

//        //            break;
//        //        }

//        //        else ga.population[0].newInit();
//        //    }

//        //}




//        //initGrid();
//        //setObstacleFromGene(ga.population[0], sC);
//        ////if (isObstacleTarget) addObstacleFromTarget(sC);
//        //estimateIsFeasible(ga.population[0]);

//        //if (ga.population[0].isFeasible)
//        //{
//        //    countPerObstacle(ga.population[0], sC);

//        //    m3h.countObstacleData(ga.population[0]);

//        //    for (int i = 0; i < m3h.limits.generation; i++)
//        //    {
//        //        calculateFitnessThroughPlay(ga.population[0], sC, trans, isObstacleTarget, cs);
//        //        if (ga.bestFitness < ga.population[0].fitness) ga.bestFitness = ga.population[0].fitness;
//        //    }
//        //}

//        ///////////
//        ////else
//        ////{
//        ////    ga.population[0]

//        ////    m3h.obstacleCnt
//        ////}

//    }

//    else
//    {
//        //유전알고리즘을 통해 예측한 swap횟수에 따른 맵 생성
//        for (int i = 0; i < m3h.limits.generation; i++)
//        {
//            for (int j = 0; j < ga.population.Count; j++)
//            {
//                // destory all cells info
//                initGrid();
//                setObstacleFromGene(ga.population[j], sC);

//                ////target 위치를 어떻게 해야하지
//                //if (isObstacleTarget) addObstacleFromTarget(sC);

//                estimateIsFeasible(ga.population[j]);
//                calculateFitness(ga.population[j], sC, trans);

//                if (ga.bestFitness < ga.population[j].fitness) ga.bestFitness = ga.population[j].fitness;

//                if (ga.bestFitness >= ga.targetFitness)
//                {
//                    break;
//                }
//            }

//            //if (ga.infeasiblePopulation == null) cs.infeasiblePopulationCnt.Add(0);
//            //else cs.infeasiblePopulationCnt.Add(ga.infeasiblePopulation.Count());
//            //if (ga.feasiblePopulation == null) cs.feasiblePopulationCnt.Add(0);
//            //else cs.feasiblePopulationCnt.Add(ga.feasiblePopulation.Count());

//            if (ga.bestFitness >= ga.targetFitness) break;
//            else ga.newGeneration();
//        }
//    }

//    Debug.Log("ENDcalculate");
//    initGrid();
//    setGridRepeat(ga.population[0], sC);

//    if (ga.population[0].isFeasible)
//    {
//        cs.write(ga, m3h);
//        Debug.Log("writedCSV");

//        m3h.wantDifficulty -= 50;
//    }

//    else Debug.Log("isNotFeasible");

//    //setObstacleFromGene(ga.population[0], sC);


//}


////void getMatch3Level(Transform trans)
////{
////    SpawnController sC = SpawnController.Instance;
////    CSVFileWriter cs = new CSVFileWriter();

////    ga.bestMeanMove = 0;
////    ga.bestStd = 0;
////    ga.bestFitness = 0;
////    ga.repeatMovements = new List<List<int>>();
////    ga.obstructionRates = new List<List<int>>();
////    ga.shorCutRates = new List<List<int>>();
////    ga.targetNeedCnt = new List<int>();
////    ga.possibleCnt = 0;
////    ga.isPossible = true;
////    ga.allMovements = new List<List<int>>();
////    ga.possibleCountingList = new List<int>();
////    gh.cellCnts = new int[Cells.Count];
////    ga.isAddObstacle = false;
////    gh.feasibleIdx = 0;

////    int cnt = 0;

////    Debug.Log("beforefitness");

////    foreach (var item in gh.curTargets)
////    {
////        ga.isAddObstacle = isObstacleTarget(item.Value.ID);

////        if (ga.isAddObstacle)
////        {
////            for (int i = 0; i < ga.population.Count; i++) addBlockedTargetEstimateFeasible(ga.population[i], sC);
////        }
////    }


////    gh.isEnd = false;


////    for (int i = 0; i < ga.findFeasibleLimit; i++)
////    {
////        for (int j = 0; j < ga.population.Count; j++)
////        {
////            ga.population[j].blockedCnt = 0;

////            destroyAllGridObjects();

////            setBlockedFromGene(ga.population[j], sC);

////            //if (ga.isAddObstacle) setBlockedFromNewGene(ga.population[j], sC);
////            //else setBlockedFromGene(ga.population[j], sC);

////            estimateIsFeasible(ga.population[j]);





////            //if (ga.population[j].isFeasible)
////            //{
////            //    mh.feasibleIdx = j;
////            //    break;
////            //}

////            //if (ga.population[j].isFeasible)
////            //{
////            //    ga.feasiblePopulation.Add(ga.population[j]);
////            //}

////            //else ga.infeasiblePopulation.Add(ga.population[j]);

////            calculateFitness(ga.population[j], sC, trans);

////            if (gh.isEnd) break;
////        }

////        if(!gh.isEnd) ga.NewGeneration();

////        else break;

////        //if (!ga.population[mh.feasibleIdx].isFeasible)
////        //{
////        //    ga.NewGeneration();
////        //    cnt++;
////        //}

////        //else break;
////    }


////    //if (ga.population[mh.feasibleIdx].isFeasible)
////    //{
////    //    while (ga.generation <= ga.generationLimit)
////    //    {
////    //        ga.curGenerationBestMean = 0;
////    //        ga.curGenerationBestStd = 0;
////    //        ga.curGenerationBestFitness = 0;
////    //        ga.repeatMovementsCnt = 0;

////    //        Debug.Log("infitness");

////    //        destroyAllGridObjects();
////    //        makeFeasibleFitness(ga.population[mh.feasibleIdx], sC, trans);

////    //        cs.generation.Add(ga.generation);
////    //        if (ga.infeasiblePopulation == null) cs.infeasiblePopulationCnt.Add(0);
////    //        else cs.infeasiblePopulationCnt.Add(ga.infeasiblePopulation.Count());
////    //        if (ga.feasiblePopulation == null) cs.feasiblePopulationCnt.Add(0);
////    //        else cs.feasiblePopulationCnt.Add(ga.feasiblePopulation.Count());
////    //        cs.curGenerationBestMean.Add(ga.curGenerationBestMean);
////    //        cs.curGenerationBestStd.Add(ga.curGenerationBestStd);
////    //        cs.curGenerationBestFitness.Add(ga.curGenerationBestFitness);
////    //        cs.bestMeanMove.Add(ga.bestMeanMove);
////    //        cs.bestStd.Add(ga.bestStd);
////    //        cs.bestFitness.Add(ga.bestFitness);
////    //        cs.repeatMovementsCntContainer.Add(ga.repeatMovementsCnt);
////    //        if (ga.curBestMoves == null) cs.curBestMoves.Add(new List<int>() { -1 });
////    //        else cs.curBestMoves.Add(ga.curBestMoves);
////    //        if (ga.bestMoves == null) cs.bestMoves.Add(new List<int>() { -1 });
////    //        else cs.bestMoves.Add(ga.bestMoves);

////    //        ga.generation++;
////    //    }



////    initGrid(ga.population[mh.feasibleIdx], sC);
////    foreach (var item in mh.curTargets) setTargetBlock(ga.population[mh.feasibleIdx], sC, item.Value.ID);

////    //destroyAllGridObjects();
////}

////    List<int> cellPossible = new List<int>();
////    for (int i = 0; i < Cells.Count; i++) cellPossible.Add(gh.grid.Cells[i].possibleCnt);

////    cs.mixedList = new List<object>
////    {
////        cs.generation,
////        cs.infeasiblePopulationCnt,
////        cs.feasiblePopulationCnt,
////        cs.curGenerationBestMean,
////        cs.curGenerationBestStd,
////        cs.curGenerationBestFitness,
////        cs.bestMeanMove,
////        cs.bestStd,
////        cs.bestFitness,
////        cs.feasibleParent,
////        cs.feasibleParentIdx,
////        cs.infeasibleParent,
////        cs.infeasibleParentIdx,
////        cs.curBestMoves,
////        cs.bestMoves
////    };

////    foreach (var item in gh.curTargets)
////    {
////        ga.targetNeedCnt.Add(item.Value.ID);
////        ga.targetNeedCnt.Add(item.Value.NeedCount);

////    }


////        //for (int i = 0; i < 7; i++)
////        //{
////        //    MatchObject m = sC.GetPickMatchObject(LcSet, goSet, i);
////        //    int value = 0;
////        //    int id = 0;
////        //    foreach (var item in mh.curTargets)
////        //    {
////        //        if (m.ID == item.Value.ID)
////        //        {
////        //            value = item.Value.NeedCount;
////        //            break;
////        //        }
////        //    }

////        //    ga.targetNeedCnt.Add(value);
////        //}
////        cs.write(ga, Cells, gh.feasibleIdx, gh.csvCnt);

////    gh.csvCnt++;
////}


//internal void fillGrid(bool noMatches, MatchGrid g, Dictionary<int, TargetData> targets, Spawner spawnerPrefab, SpawnerStyle spawnerStyle, Transform GridContainer, Transform trans, LevelConstructSet IC)
//{
//    m3h = new Match3Helper(g, targets);
//    m3h.board.makeBoard(g, spawnerPrefab, spawnerStyle, GridContainer, trans, IC);
//    //g.mgList = new List<MatchGroup>();
//    m3h.obstacleCnt = 0;


//    for (; m3h.match3Cycle < m3h.match3CycleLimit;)
//    {
//        Debug.Log("startNewCycle");

//        //m3h.obstacleCnt = (int)(Random.Range((int)g.Cells.Count / 4, (int)g.Cells.Count / 2));
//        //m3h.obstacleCnt = (int)(Random.Range(1, (int)g.Cells.Count / 2));
//        //m3h.obstacleCnt = 20;
//        //m3h.obstacleCnt += (int)Random.Range(1, 3);

//        randomGa = new System.Random();
//        ga = new GeneticAlgorithm<char>(Cells.Count, randomGa, getRandomGene, getGenes, m3h); //유전알고리즘 호출
//        getMatch3Level(trans);

//        m3h.match3Cycle += 1;

//        if (m3h.csvCnt > 9) break;
//    }

//}


////GameBoard.cs-------------------------------------------------------------

//public void CollectMatchGroups1(MatchGrid g)
//{
//    ParallelTween pt = new ParallelTween();

//    if (g.mgList.Count == 0)
//    {
//        return;
//    }

//    GameBoard board = new GameBoard();

//    for (int i = 0; i < g.mgList.Count; i++)
//    {
//        if (g.mgList[i] != null)
//        {
//            MatchGroup m = g.mgList[i];

//            board.CollectHandler1(m);

//            //pt.Add1((callBack) =>
//            //{
//            //    board.CollectHandler1(m, callBack);
//            //    //Collect(m, callBack);
//            //});
//        }
//    }
//    //pt.Start1(() =>
//    //{
//    //});
//}

//public MatchGroupsHelper()
//{
//    mgList = new List<MatchGroup>();
//    this.grid = grid;

//}

//public void CancelTweens1(MatchGrid g)
//{
//    if (showSequence != null) { showSequence.Break(); showSequence = null; }
//    if (showEstimateSequence != null) { showEstimateSequence.Break(); showEstimateSequence = null; }
//    g.mgList.ForEach((mg) => { mg.CancelTween11(g); });
//}

//public class MatchGroup : CellsGroup

//public List<int> GetEst(List<MatchGroup> mg, int idx)
//{
//    List<int> list = new List<int>();


//    list.Add(mg[idx].est1.Column);
//    list.Add(mg[idx].est1.Row);
//    list.Add(mg[idx].est2.Column);
//    list.Add(mg[idx].est2.Row);

//    return list;
//}


//public void countPossible(MatchGrid grid, int idx)
//{
//    GridCell c0 = grid.Cells[idx];
//    int X0 = c0.Column;
//    int Y0 = c0.Row;

//    GridCell L = null;
//    GridCell T = null;

//    if (c0.DynamicObject != null)
//    {
//        //if (c0.Row < grid.Rows.Count && c0.Column + 1 < grid.Columns.Count) L = grid.Rows[c0.Row][c0.Column + 1];
//        //if (c0.Row < grid.Rows.Count && c0.Column + 2 < grid.Columns.Count) T = grid.Rows[c0.Row][c0.Column + 2];

//        //   1 X X
//        // 3 0 L T X
//        //   2 X X
//        L = grid[Y0, X0 + 1];
//        T = grid[Y0, X0 + 2];

//        if (L != null && L.IsDraggable() && T != null && T.IsDraggable())
//        {
//            int X1 = X0; int Y1 = Y0 - 1;
//            GridCell c1 = grid[Y1, X1];
//            if ((c1 != null) && c1.IsDraggable()) grid[Y0, X0].matchFromSwapPotential++;

//            int X2 = X0; int Y2 = Y0 + 1;
//            GridCell c2 = grid[Y2, X2];
//            if ((c2 != null) && c2.IsDraggable()) grid[Y0, X0].matchFromSwapPotential++;

//            int X3 = X0 - 1; int Y3 = Y0;
//            GridCell c3 = grid[Y3, X3];
//            if ((c3 != null) && c3.IsDraggable()) grid[Y0, X0].matchFromSwapPotential++;
//        }

//        //    X X 4
//        //  X L T 0 6
//        //    X X 5
//        L = grid[Y0, X0 - 1];
//        T = grid[Y0, X0 - 2];

//        if (L != null && L.IsDraggable() && T != null && T.IsDraggable())
//        {
//            int X4 = X0; int Y4 = Y0 - 1;
//            GridCell c4 = grid[Y4, X4];
//            if ((c4 != null) && c4.IsDraggable()) grid[Y0, X0].matchFromSwapPotential++;

//            int X5 = X0; int Y5 = Y0 + 1;
//            GridCell c5 = grid[Y5, X5];
//            if ((c5 != null) && c5.IsDraggable()) grid[Y0, X0].matchFromSwapPotential++;

//            int X6 = X0 + 1; int Y6 = Y0;
//            GridCell c6 = grid[Y6, X6];
//            if ((c6 != null) && c6.IsDraggable()) grid[Y0, X0].matchFromSwapPotential++;
//        }

//        //    X 7 X
//        //  X L 0 T X
//        //    X 8 X
//        L = grid[Y0, X0 - 1];
//        T = grid[Y0, X0 + 1];

//        if (L != null && L.IsDraggable() && T != null && T.IsDraggable())
//        {
//            int X7 = L.Column + 1; int Y7 = L.Row - 1;
//            GridCell c7 = grid[Y7, X7];
//            if (c7 != null && c7.IsDraggable()) grid[Y0, X0].matchFromSwapPotential++;

//            int X8 = L.Column + 1; int Y8 = L.Row + 1;
//            GridCell c8 = grid[Y8, X8];
//            if (c8 != null && c8.IsDraggable()) grid[Y0, X0].matchFromSwapPotential++;
//        }

//        //     X 
//        //   X T X 
//        //   X L X
//        //   1 0 2
//        //     3
//        L = grid[Y0 - 1, X0];
//        T = grid[Y0 - 2, X0];

//        if (L != null && L.IsDraggable() && T != null && T.IsDraggable())
//        {
//            int X1 = X0 - 1; int Y1 = Y0;
//            GridCell c1 = grid[Y1, X1];
//            if ((c1 != null) && c1.IsDraggable()) grid[Y0, X0].matchFromSwapPotential++;

//            int X2 = X0 + 1; int Y2 = Y0;
//            GridCell c2 = grid[Y2, X2];
//            if ((c2 != null) && c2.IsDraggable()) grid[Y0, X0].matchFromSwapPotential++;

//            int X3 = X0; int Y3 = Y0 + 1;
//            GridCell c3 = grid[Y3, X3];
//            if ((c3 != null) && c3.IsDraggable()) grid[Y0, X0].matchFromSwapPotential++;
//        }

//        //     6
//        //   4 0 5
//        //   X T X 
//        //   X L X
//        //     X 
//        L = grid[Y0 + 2, X0];
//        T = grid[Y0 + 1, X0];

//        if (L != null && L.IsDraggable() && T != null && T.IsDraggable())
//        {
//            int X4 = T.Column - 1; int Y4 = T.Row - 1;
//            GridCell c4 = grid[Y4, X4];
//            if ((c4 != null) && c4.IsDraggable()) grid[Y0, X0].matchFromSwapPotential++;

//            int X5 = T.Column + 1; int Y5 = T.Row - 1;
//            GridCell c5 = grid[Y5, X5];
//            if ((c5 != null) && c5.IsDraggable()) grid[Y0, X0].matchFromSwapPotential++;

//            int X6 = T.Column; int Y6 = T.Row - 2;
//            GridCell c6 = grid[Y6, X6];
//            if ((c6 != null) && c6.IsDraggable()) grid[Y0, X0].matchFromSwapPotential++;
//        }

//        //      X
//        //    X T X
//        //    7 0 8 
//        //    X L X
//        //      X
//        L = grid[Y0 + 1, X0];
//        T = grid[Y0 - 1, X0];

//        if (L != null && L.IsDraggable() && T != null && T.IsDraggable())
//        {
//            int X7 = X0 - 1; int Y7 = Y0;
//            GridCell c7 = grid[Y7, X7];
//            if (c7 != null && c7.IsDraggable()) grid[Y0, X0].matchFromSwapPotential++;

//            int X8 = X0 + 1; int Y8 = Y0;
//            GridCell c8 = grid[Y8, X8];
//            if (c8 != null && c8.IsDraggable()) grid[Y0, X0].matchFromSwapPotential++;
//        }
//    }
//}






////public bool IsEstimateMatch1(int matchCount, bool horizontal, MatchGrid grid, DNA<char> p)
////{
////    if (Length != matchCount) return false;
////    if (horizontal)
////    {
////        GridCell L = GetLowermostX();
////        GridCell T = GetTopmostX();

////        // 3 estimate positions for l - cell (astrics)
////        //   1 X X
////        // 3 0 L T X
////        //   2 X X
////        int X0 = L.Column - 1; int Y0 = L.Row;
////        GridCell c0 = grid[Y0, X0];

////        if ((c0 != null) && c0.IsDraggable() && ((T.Column - L.Column) == 1))
////        {
////            int X1 = X0; int Y1 = Y0 - 1;
////            GridCell c1 = grid[Y1, X1];
////            if ((c1 != null) && c1.IsMatchObjectEquals(L) && c1.IsDraggable())
////            {
////                Add(c1);
////                est1 = c0;
////                est2 = c1;
////                return true;
////            }

////            if ((c1 != null) && c1.DynamicObject == null) p.obstructionRate++;


////            int X2 = X0; int Y2 = Y0 + 1;
////            GridCell c2 = grid[Y2, X2];
////            if ((c2 != null) && c2.IsMatchObjectEquals(L) && c2.IsDraggable())
////            {
////                Add(c2);
////                est1 = c0;
////                est2 = c2;
////                return true;
////            }

////            if ((c2 != null) && c2.DynamicObject == null) p.obstructionRate++;

////            int X3 = X0 - 1; int Y3 = Y0;
////            GridCell c3 = grid[Y3, X3];
////            if ((c3 != null) && c3.IsMatchObjectEquals(L) && c3.IsDraggable())
////            {
////                Add(c3);
////                est1 = c0;
////                est2 = c3;
////                return true;
////            }

////            if ((c3 != null) && c3.DynamicObject == null) p.obstructionRate++;
////        }

////        if ((c0 != null) && c0.DynamicObject == null) p.obstructionRate++;

////        // 3 estimate positions for T - cell (astrics)
////        //    X X 4
////        //  X L T 0 6
////        //    X X 5
////        X0 = T.Column + 1; Y0 = T.Row;
////        c0 = grid[Y0, X0];
////        if ((c0 != null) && c0.IsDraggable() && ((T.Column - L.Column) == 1))
////        {
////            int X4 = X0; int Y4 = Y0 - 1;
////            GridCell c4 = grid[Y4, X4];
////            if ((c4 != null) && c4.IsMatchObjectEquals(T) && c4.IsDraggable())
////            {
////                Add(c4);
////                est1 = c0;
////                est2 = c4;
////                return true;
////            }

////            if ((c4 != null) && c4.DynamicObject == null) p.obstructionRate++;

////            int X5 = X0; int Y5 = Y0 + 1;
////            GridCell c5 = grid[Y5, X5];
////            if ((c5 != null) && c5.IsMatchObjectEquals(T) && c5.IsDraggable())
////            {
////                Add(c5);
////                est1 = c0;
////                est2 = c5;
////                return true;
////            }

////            if ((c5 != null) && c5.DynamicObject == null) p.obstructionRate++;

////            int X6 = X0 + 1; int Y6 = Y0;
////            GridCell c6 = grid[Y6, X6];
////            if ((c6 != null) && c6.IsMatchObjectEquals(T) && c6.IsDraggable())
////            {
////                Add(c6);
////                est1 = c0;
////                est2 = c6;
////                return true;
////            }

////            if ((c6 != null) && c6.DynamicObject == null) p.obstructionRate++;
////        }

////        if ((c0 != null) && c0.DynamicObject == null) p.obstructionRate++;

////        // 2 estimate positions for L0T - horizontal
////        //    X 7 X
////        //  X L 0 T X
////        //    X 8 X
////        X0 = L.Column + 1; Y0 = L.Row;
////        c0 = grid[Y0, X0];
////        if ((c0 != null) && c0.IsDraggable() && ((T.Column - L.Column) == 2))
////        {
////            int X7 = L.Column + 1; int Y7 = L.Row - 1;
////            GridCell c7 = grid[Y7, X7];
////            if (c7 != null && c7.IsMatchObjectEquals(L) && c7.IsDraggable())
////            {
////                Add(c7);
////                est1 = c0;
////                est2 = c7;
////                return true;
////            }

////            if ((c7 != null) && c7.DynamicObject == null) p.obstructionRate++;

////            int X8 = L.Column + 1; int Y8 = L.Row + 1;
////            GridCell c8 = grid[Y8, X8];
////            if (c8 != null && c8.IsMatchObjectEquals(L) && c8.IsDraggable())
////            {
////                Add(c8);
////                est1 = c0;
////                est2 = c8;
////                return true;
////            }

////            if ((c8 != null) && c8.DynamicObject == null) p.obstructionRate++;
////        }

////        if ((c0 != null) && c0.DynamicObject == null) p.obstructionRate++;
////    }
////    else
////    {
////        GridCell L = GetLowermostY();
////        GridCell T = GetTopmostY();
////        // 3 estimate positions for L - cell 
////        //     
////        //     X 
////        //   X T X 
////        //   X L X
////        //   1 0 2
////        //     3
////        int X0 = L.Column; int Y0 = L.Row + 1;
////        GridCell c0 = grid[Y0, X0];
////        if ((c0 != null) && c0.IsDraggable() && ((T.Row - L.Row) == -1))
////        {
////            int X1 = X0 - 1; int Y1 = Y0;
////            GridCell c1 = grid[Y1, X1];
////            if ((c1 != null) && c1.IsMatchObjectEquals(L) && c1.IsDraggable())
////            {
////                Add(c1);
////                est1 = c0;
////                est2 = c1;
////                return true;
////            }

////            if ((c1 != null) && c1.DynamicObject == null) p.obstructionRate++;

////            int X2 = X0 + 1; int Y2 = Y0;
////            GridCell c2 = grid[Y2, X2];
////            if ((c2 != null) && c2.IsMatchObjectEquals(L) && c2.IsDraggable())
////            {
////                Add(c2);
////                est1 = c0;
////                est2 = c2;
////                return true;
////            }

////            if ((c2 != null) && c2.DynamicObject == null) p.obstructionRate++;

////            int X3 = X0; int Y3 = Y0 + 1;
////            GridCell c3 = grid[Y3, X3];
////            if ((c3 != null) && c3.IsMatchObjectEquals(L) && c3.IsDraggable())
////            {
////                Add(c3);
////                est1 = c0;
////                est2 = c3;
////                return true;
////            }

////            if ((c3 != null) && c3.DynamicObject == null) p.obstructionRate++;
////        }

////        if ((c0 != null) && c0.DynamicObject == null) p.obstructionRate++;

////        // 3 estimate positions for T - cell
////        //     6
////        //   4 0 5
////        //   X T X 
////        //   X L X
////        //     X 
////        X0 = L.Column; Y0 = T.Row - 1;
////        c0 = grid[Y0, X0];
////        //   Debug.Log("c0: " + c0 + " : " + c0.IsDraggable() +" : " + ((T.Row - L.Row)));
////        if ((c0 != null) && c0.IsDraggable() && ((T.Row - L.Row) == -1))
////        {
////            int X4 = T.Column - 1; int Y4 = T.Row - 1;
////            GridCell c4 = grid[Y4, X4];
////            if ((c4 != null) && c4.IsMatchObjectEquals(L) && c4.IsDraggable())
////            {
////                Add(c4);
////                est1 = c0;
////                est2 = c4;
////                return true;
////            }

////            if ((c4 != null) && c4.DynamicObject == null) p.obstructionRate++;

////            int X5 = T.Column + 1; int Y5 = T.Row - 1;
////            GridCell c5 = grid[Y5, X5];
////            if ((c5 != null) && c5.IsMatchObjectEquals(L) && c5.IsDraggable())
////            {
////                Add(c5);
////                est1 = c0;
////                est2 = c5;
////                return true;
////            }

////            if ((c5 != null) && c5.DynamicObject == null) p.obstructionRate++;

////            int X6 = T.Column; int Y6 = T.Row - 2;
////            GridCell c6 = grid[Y6, X6];
////            if ((c6 != null) && c6.IsMatchObjectEquals(L) && c6.IsDraggable())
////            {
////                Add(c6);
////                est1 = c0;
////                est2 = c6;
////                return true;
////            }

////            if ((c6 != null) && c6.DynamicObject == null) p.obstructionRate++;
////        }

////        if ((c0 != null) && c0.DynamicObject == null) p.obstructionRate++;

////        // 2 estimate positions for T0L - vertical
////        //      X
////        //    X T X
////        //    7 0 8 
////        //    X L X
////        //      X
////        X0 = L.Column; Y0 = L.Row - 1;
////        c0 = grid[Y0, X0];
////        if ((c0 != null) && c0.IsDraggable() && ((T.Row - L.Row) == -2))
////        {
////            int X7 = X0 - 1; int Y7 = Y0;
////            GridCell c7 = grid[Y7, X7];
////            if ((c7 != null) && c7.IsMatchObjectEquals(L) && c7.IsDraggable())
////            {
////                Add(c7);
////                est1 = c0;
////                est2 = c7;
////                return true;
////            }

////            if ((c7 != null) && c7.DynamicObject == null) p.obstructionRate++;

////            int X8 = X0 + 1; int Y8 = Y0;
////            GridCell c8 = grid[Y8, X8];
////            if ((c8 != null) && c8.IsMatchObjectEquals(L) && c8.IsDraggable())
////            {
////                Add(c8);
////                est1 = c0;
////                est2 = c8;
////                return true;
////            }

////            if ((c8 != null) && c8.DynamicObject == null) p.obstructionRate++;
////        }

////        if ((c0 != null) && c0.DynamicObject == null) p.obstructionRate++;
////    }
////    return false;
////}


//public bool new_IsEstimateMatch(int matchCount, bool horizontal, GridCell[,] curCell)
//{
//    if (Length != matchCount) return false;
//    if (horizontal)
//    {
//        GridCell L = GetLowermostX();
//        GridCell T = GetTopmostX();
//        GridCell c0;
//        // 3 estimate positions for l - cell (astrics)
//        //   1 X X
//        // 3 0 L T X
//        //   2 X X
//        int X0 = L.Column - 1; int Y0 = L.Row;



//        if (isOverBoard(X0, Y0, curCell))
//        {
//            c0 = curCell[Y0, X0];

//            if ((c0 != null) && c0.IsDraggable() && ((T.Column - L.Column) == 1) && !c0.isNotFill)
//            {
//                int X1 = X0; int Y1 = Y0 - 1;
//                GridCell c1 = curCell[Y1, X1];
//                if ((c1 != null) && c1.IsMatchObjectEquals(L) && c1.IsDraggable() && !c1.isNotFill)
//                {
//                    Add(c1);
//                    est1 = c0;
//                    est2 = c1;
//                    return true;
//                }

//                int X2 = X0; int Y2 = Y0 + 1;
//                GridCell c2 = curCell[Y2, X2];
//                if ((c2 != null) && c2.IsMatchObjectEquals(L) && c2.IsDraggable() && !c2.isNotFill)
//                {
//                    Add(c2);
//                    est1 = c0;
//                    est2 = c2;
//                    return true;
//                }

//                int X3 = X0 - 1; int Y3 = Y0;
//                GridCell c3 = curCell[Y3, X3];
//                if ((c3 != null) && c3.IsMatchObjectEquals(L) && c3.IsDraggable() && !c3.isNotFill)
//                {
//                    Add(c3);
//                    est1 = c0;
//                    est2 = c3;
//                    return true;
//                }
//            }
//        }


//        // 3 estimate positions for T - cell (astrics)
//        //    X X 4
//        //  X L T 0 6
//        //    X X 5
//        X0 = T.Column + 1; Y0 = T.Row;

//        if (isOverBoard(X0, Y0, curCell))
//        {
//            c0 = curCell[Y0, X0];

//            if ((c0 != null) && c0.IsDraggable() && ((T.Column - L.Column) == 1))
//            {
//                int X4 = X0; int Y4 = Y0 - 1;
//                GridCell c4 = curCell[Y4, X4];
//                if ((c4 != null) && c4.IsMatchObjectEquals(T) && c4.IsDraggable() && !c4.isNotFill)
//                {
//                    Add(c4);
//                    est1 = c0;
//                    est2 = c4;
//                    return true;
//                }

//                int X5 = X0; int Y5 = Y0 + 1;
//                GridCell c5 = curCell[Y5, X5];
//                if ((c5 != null) && c5.IsMatchObjectEquals(T) && c5.IsDraggable() && !c5.isNotFill)
//                {
//                    Add(c5);
//                    est1 = c0;
//                    est2 = c5;
//                    return true;
//                }

//                int X6 = X0 + 1; int Y6 = Y0;
//                GridCell c6 = curCell[Y6, X6];
//                if ((c6 != null) && c6.IsMatchObjectEquals(T) && c6.IsDraggable() && !c6.isNotFill)
//                {
//                    Add(c6);
//                    est1 = c0;
//                    est2 = c6;
//                    return true;
//                }
//            }
//        }



//        // 2 estimate positions for L0T - horizontal
//        //    X 7 X
//        //  X L 0 T X
//        //    X 8 X
//        X0 = L.Column + 1; Y0 = L.Row;
//        if (isOverBoard(X0, Y0, curCell))
//        {

//            c0 = curCell[Y0, X0];
//            if ((c0 != null) && c0.IsDraggable() && ((T.Column - L.Column) == 2) && !c0.isNotFill)
//            {
//                int X7 = L.Column + 1; int Y7 = L.Row - 1;
//                GridCell c7 = curCell[Y7, X7];
//                if (c7 != null && c7.IsMatchObjectEquals(L) && c7.IsDraggable() && !c7.isNotFill)
//                {
//                    Add(c7);
//                    est1 = c0;
//                    est2 = c7;
//                    return true;
//                }

//                int X8 = L.Column + 1; int Y8 = L.Row + 1;
//                GridCell c8 = curCell[Y8, X8];
//                if (c8 != null && c8.IsMatchObjectEquals(L) && c8.IsDraggable() && !c8.isNotFill)
//                {
//                    Add(c8);
//                    est1 = c0;
//                    est2 = c8;
//                    return true;
//                }
//            }
//        }


//    }
//    else
//    {
//        GridCell L = GetLowermostY();
//        GridCell T = GetTopmostY();
//        // 3 estimate positions for L - cell 
//        //     
//        //     X 
//        //   X T X 
//        //   X L X
//        //   1 0 2
//        //     3

//        GridCell c0;
//        int X0 = L.Column; int Y0 = L.Row + 1;

//        if (isOverBoard(X0, Y0, curCell))
//        {

//            c0 = curCell[Y0, X0];
//            if ((c0 != null) && c0.IsDraggable() && ((T.Row - L.Row) == -1) && !c0.isNotFill)
//            {
//                int X1 = X0 - 1; int Y1 = Y0;
//                GridCell c1 = curCell[Y1, X1];
//                if ((c1 != null) && c1.IsMatchObjectEquals(L) && c1.IsDraggable() && !c1.isNotFill)
//                {
//                    Add(c1);
//                    est1 = c0;
//                    est2 = c1;
//                    return true;
//                }

//                int X2 = X0 + 1; int Y2 = Y0;
//                GridCell c2 = curCell[Y2, X2];
//                if ((c2 != null) && c2.IsMatchObjectEquals(L) && c2.IsDraggable() && !c2.isNotFill)
//                {
//                    Add(c2);
//                    est1 = c0;
//                    est2 = c2;
//                    return true;
//                }

//                int X3 = X0; int Y3 = Y0 + 1;
//                GridCell c3 = curCell[Y3, X3];
//                if ((c3 != null) && c3.IsMatchObjectEquals(L) && c3.IsDraggable() && !c3.isNotFill)
//                {
//                    Add(c3);
//                    est1 = c0;
//                    est2 = c3;
//                    return true;
//                }
//            }

//        }

//        // 3 estimate positions for T - cell
//        //     6
//        //   4 0 5
//        //   X T X 
//        //   X L X
//        //     X 
//        X0 = L.Column; Y0 = T.Row - 1;
//        if (isOverBoard(X0, Y0, curCell))
//        {

//            c0 = curCell[Y0, X0];
//            //   Debug.Log("c0: " + c0 + " : " + c0.IsDraggable() +" : " + ((T.Row - L.Row)));
//            if ((c0 != null) && c0.IsDraggable() && ((T.Row - L.Row) == -1))
//            {
//                int X4 = T.Column - 1; int Y4 = T.Row - 1;
//                GridCell c4 = curCell[Y4, X4];
//                if ((c4 != null) && c4.IsMatchObjectEquals(L) && c4.IsDraggable() && !c4.isNotFill)
//                {
//                    Add(c4);
//                    est1 = c0;
//                    est2 = c4;
//                    return true;
//                }

//                int X5 = T.Column + 1; int Y5 = T.Row - 1;
//                GridCell c5 = curCell[Y5, X5];
//                if ((c5 != null) && c5.IsMatchObjectEquals(L) && c5.IsDraggable() && !c5.isNotFill)
//                {
//                    Add(c5);
//                    est1 = c0;
//                    est2 = c5;
//                    return true;
//                }

//                int X6 = T.Column; int Y6 = T.Row - 2;
//                GridCell c6 = curCell[Y6, X6];
//                if ((c6 != null) && c6.IsMatchObjectEquals(L) && c6.IsDraggable() && !c6.isNotFill)
//                {
//                    Add(c6);
//                    est1 = c0;
//                    est2 = c6;
//                    return true;
//                }
//            }
//        }



//        // 2 estimate positions for T0L - vertical
//        //      X
//        //    X T X
//        //    7 0 8 
//        //    X L X
//        //      X
//        X0 = L.Column; Y0 = L.Row - 1;
//        if (isOverBoard(X0, Y0, curCell))
//        {

//            c0 = curCell[Y0, X0];
//            if ((c0 != null) && c0.IsDraggable() && ((T.Row - L.Row) == -2))
//            {
//                int X7 = X0 - 1; int Y7 = Y0;
//                GridCell c7 = curCell[Y7, X7];
//                if ((c7 != null) && c7.IsMatchObjectEquals(L) && c7.IsDraggable() && !c7.isNotFill)
//                {
//                    Add(c7);
//                    est1 = c0;
//                    est2 = c7;
//                    return true;
//                }

//                int X8 = X0 + 1; int Y8 = Y0;
//                GridCell c8 = curCell[Y8, X8];
//                if ((c8 != null) && c8.IsMatchObjectEquals(L) && c8.IsDraggable() && !c8.isNotFill)
//                {
//                    Add(c8);
//                    est1 = c0;
//                    est2 = c8;
//                    return true;
//                }
//            }
//        }
//    }



//    return false;
//}

//    public class CellArray<T> : GenInd<T> where T : GridCell



//public List<MatchGroup> isContinuousRow()
//{
//    List<MatchGroup> mgList = new List<MatchGroup>();
//    MatchGroup mg = new MatchGroup();
//    int minMatches = 2;
//    mg.Add(cells[0]);

//    for (int i = 1; i < cells.Length; i++)
//    {
//        int prev = mg.Length - 1;

//        if (cells[i].IsMatchable && cells[prev].IsMatchable)
//        {
//            mg.Add(cells[i]);

//            if (mg.Length >= minMatches)
//            {
//                mgList.Add(mg);
//                mg = new MatchGroup();
//            }
//        }

//        //else
//        //{
//        //    if (mg.Length >= minMatches) mgList.Add(mg);
//        //    mg = new MatchGroup();
//        //    mg.Add(cells[i]);
//        //}
//    }

//    mg = new MatchGroup();

//    for (int i = 2; i < cells.Length; i++)
//    {
//        mg.Add(cells[i - 2]);

//        if (cells[i].DynamicObject != null && cells[0].DynamicObject != null && cells[i - 1].DynamicObject != null)
//        {
//            mg.Add(cells[i]);
//            mgList.Add(mg);
//        }

//        mg = new MatchGroup();
//    }
//    return mgList;
//}

//public List<MatchGroup> isContinuousColumn()
//{
//    List<MatchGroup> mgList = new List<MatchGroup>();
//    MatchGroup mg = new MatchGroup();
//    int minMatches = 2;
//    mg.Add(cells[0]);

//    for (int i = 1; i < cells.Length; i++)
//    {
//        int prev = mg.Length - 1;

//        if (cells[i].DynamicObject != null && cells[prev].DynamicObject != null)
//        {
//            mg.Add(cells[i]);

//            if (i == cells.Length - 1 && mg.Length >= minMatches)
//            {
//                mgList.Add(mg);
//                mg = new MatchGroup();
//            }
//        }

//        else
//        {
//            if (mg.Length >= minMatches) mgList.Add(mg);
//            mg = new MatchGroup();
//            mg.Add(cells[i]);
//        }
//    }

//    mg = new MatchGroup();

//    for (int i = 2; i < cells.Length; i++)
//    {
//        mg.Add(cells[i - 2]);

//        if (cells[i].DynamicObject != null && cells[0].DynamicObject != null && cells[i - 1].DynamicObject != null)
//        {
//            mg.Add(cells[i]);
//            mgList.Add(mg);
//        }

//        mg = new MatchGroup();
//    }
//    return mgList;
//}


//public List<MatchGroup> GetMatches2(int minMatches, bool X0X)
//{
//    List<MatchGroup> mgList = new List<MatchGroup>();
//    MatchGroup mg = new MatchGroup();
//    mg.Add(cells[0]);
//    for (int i = 1; i < cells.Length; i++)
//    {
//        int prev = mg.Length - 1;
//        if (cells[i].IsMatchable && cells[i].IsMatchObjectEquals(mg.Cells[prev]) && mg.Cells[prev].IsMatchable)
//        {
//            mg.Add(cells[i]);
//            if (i == cells.Length - 1 && mg.Length >= minMatches)
//            {
//                mgList.Add(mg);
//                mg = new MatchGroup();
//            }
//        }
//        else // start new match group
//        {
//            if (mg.Length >= minMatches)
//            {
//                mgList.Add(mg);
//            }
//            mg = new MatchGroup();
//            mg.Add(cells[i]);
//        }
//    }

//    if (X0X) // [i-2, i-1, i]
//    {
//        mg = new MatchGroup();

//        //for (int i = 2; i < cells.Length; i++)
//        //{
//        //    mg.Add(cells[i - 2]);
//        //    if (cells[i].IsMatchable && cells[i].IsMatchObjectEquals(mg.Cells[0]) && !cells[i - 1].IsMatchObjectEquals(mg.Cells[0])
//        //        && mg.Cells[0].IsMatchable && cells[i - 1].IsDraggable())
//        //    {
//        //        mg.Add(cells[i]);
//        //        mgList.Add(mg);
//        //    }
//        //    mg = new MatchGroup();
//        //}

//        for (int i = 2; i < cells.Length; i++)
//        {
//            mg.Add(cells[i - 2]);
//            if (cells[i].IsMatchable && cells[i].IsMatchObjectEquals(mg.Cells[0]) && !cells[i - 1].IsMatchObjectEquals(mg.Cells[0]) && mg.Cells[0].IsMatchable)
//            {
//                if (cells[i - 1].IsDraggable())
//                {
//                    mg.haveBlocked = false;
//                    mg.Add(cells[i]);
//                    mgList.Add(mg);
//                }

//                else
//                {
//                    mg.haveBlocked = true;
//                    mg.Add(cells[i]);
//                    mgList.Add(mg);
//                }
//            }
//            mg = new MatchGroup();
//        }

//    } // end X0X
//    return mgList;
//}


//public List<MatchGroup> GetMatches1(int minMatches, bool X0X)
//{
//    List<MatchGroup> mgList = new List<MatchGroup>();
//    MatchGroup mg = new MatchGroup();
//    mg.Add(cells[0]);
//    for (int i = 1; i < cells.Length; i++)
//    {
//        int prev = mg.Length - 1;
//        if (cells[i].IsMatchable && cells[i].IsMatchObjectEquals(mg.Cells[prev]) && mg.Cells[prev].IsMatchable)
//        {
//            mg.Add(cells[i]);
//            if (i == cells.Length - 1 && mg.Length >= minMatches)
//            {
//                mgList.Add(mg);
//                mg = new MatchGroup();
//            }
//        }
//        else // start new match group
//        {
//            if (mg.Length >= minMatches)
//            {
//                mgList.Add(mg);
//            }
//            mg = new MatchGroup();
//            mg.Add(cells[i]);
//        }
//    }

//    if (X0X) // [i-2, i-1, i]
//    {
//        mg = new MatchGroup();

//        //for (int i = 2; i < cells.Length; i++)
//        //{
//        //    mg.Add(cells[i - 2]);
//        //    if (cells[i].IsMatchable && cells[i].IsMatchObjectEquals(mg.Cells[0]) && !cells[i - 1].IsMatchObjectEquals(mg.Cells[0])
//        //        && mg.Cells[0].IsMatchable && cells[i - 1].IsDraggable())
//        //    {
//        //        mg.Add(cells[i]);
//        //        mgList.Add(mg);
//        //    }
//        //    mg = new MatchGroup();
//        //}

//        for (int i = 2; i < cells.Length; i++)
//        {
//            mg.Add(cells[i - 2]);
//            if (cells[i].IsMatchable && cells[i].IsMatchObjectEquals(mg.Cells[0]) && !cells[i - 1].IsMatchObjectEquals(mg.Cells[0]) && mg.Cells[0].IsMatchable)
//            {
//                if (cells[i - 1].IsDraggable())
//                {
//                    mg.haveBlocked = false;
//                    mg.Add(cells[i]);
//                    mgList.Add(mg);
//                }

//                else
//                {
//                    mg.haveBlocked = true;
//                    mg.Add(cells[i]);
//                    mgList.Add(mg);
//                }
//            }
//            mg = new MatchGroup();
//        }

//    } // end X0X
//    return mgList;
//}
//public List<MatchGroup> new_GetMatches(int minMatches, bool X0X, GridCell[,] curCell)
//{
//    List<MatchGroup> mgList = new List<MatchGroup>();
//    MatchGroup mg = new MatchGroup();

//    //mg.Add(cells[0]);
//    mg.Add(curCell[cells[0].Column, cells[0].Row]);

//    for (int i = 1; i < cells.Length; i++)
//    {
//        int prev = mg.Length - 1;

//        if (curCell[cells[i].Column, cells[i].Row].IsMatchable && !curCell[cells[i].Column, cells[i].Row].isNotFill
//            && curCell[cells[i].Column, cells[i].Row].IsMatchObjectEquals(mg.Cells[prev])
//            && !mg.Cells[prev].IsMatchable && !mg.Cells[prev].isNotFill)
//        {
//            mg.Add(curCell[cells[i].Column, cells[i].Row]);
//            if (i == cells.Length - 1 && mg.Length >= minMatches)
//            {
//                mgList.Add(mg);
//                mg = new MatchGroup();
//            }
//        }
//        else // start new match group
//        {
//            if (mg.Length >= minMatches) mgList.Add(mg);

//            mg = new MatchGroup();
//            mg.Add(curCell[cells[i].Column, cells[i].Row]);
//        }
//    }

//    if (X0X) // [i-2, i-1, i]
//    {
//        mg = new MatchGroup();

//        for (int i = 2; i < cells.Length; i++)
//        {
//            mg.Add(curCell[cells[i - 2].Column, cells[i - 2].Row]);

//            if (curCell[cells[i].Column, cells[i].Row].IsMatchable && !curCell[cells[i].Column, cells[i].Row].isNotFill
//                && curCell[cells[i].Column, cells[i].Row].IsMatchObjectEquals(mg.Cells[0])
//                && !curCell[cells[i - 1].Column, cells[i - 1].Row].IsMatchObjectEquals(mg.Cells[0])
//                && mg.Cells[0].IsMatchable && !mg.Cells[0].isNotFill
//                && curCell[cells[i - 1].Column, cells[i - 1].Row].IsDraggable())
//            {
//                mg.Add(curCell[cells[i].Column, cells[i].Row]);
//                mgList.Add(mg);
//            }
//            mg = new MatchGroup();
//        }
//    } // end X0X
//    return mgList;
//}


//public List<MatchGroup> getMatchableGroup(int minMatches)
//{
//    List<MatchGroup> mgList = new List<MatchGroup>();
//    MatchGroup mg = new MatchGroup();
//    //mg.Add(cells[0]);

//    for (int i = 2; i < cells.Length; i++)
//    {
//        int first = i - 2;
//        int second = i - 1;

//        if (cells[first].IsMatchable && cells[second].IsMatchable && cells[i].IsMatchable)
//        {
//            mg.Add(cells[first]);
//            mg.Add(cells[second]);
//            mg.Add(cells[i]);
//            mgList.Add(mg);
//            mg = new MatchGroup();
//        }


//        //int prev = mg.Length - 1;

//        //if (mg.Length >= minMatches)
//        //{
//        //    mgList.Add(mg);
//        //    mg = new MatchGroup();
//        //    mg.Add(cells[i]);
//        //    prev = mg.Length - 1;
//        //}

//        //else if (cells[i].IsMatchable && mg.Cells[prev].IsMatchable)
//        //{
//        //    mg.Add(cells[i]);
//        //}

//        //else
//        //{
//        //    if (mg.Length >= minMatches)
//        //    {
//        //        mgList.Add(mg);
//        //    }
//        //    mg = new MatchGroup();
//        //    mg.Add(cells[i]);
//        //}

//    }

//    return mgList;
//}


//public override string ToString()
//{
//    string s = "";
//    for (int i = 0; i < cells.Length; i++)
//    {
//        s += cells[i].ToString();
//    }
//    return s;
//}



//public List<MatchGroup> GetMatches1(int minMatches, bool X0X, MatchGrid grid)
//{
//    List<MatchGroup> mgList = new List<MatchGroup>();
//    MatchGroup mg = new MatchGroup();
//    mg.Add(cells[0]);
//    for (int i = 1; i < grid.Cells.Count; i++)
//    {
//        int prev = mg.Length - 1;
//        if (grid.Cells[i].IsMatchable && grid.Cells[i].IsMatchObjectEquals(mg.Cells[prev]) && mg.Cells[prev].IsMatchable)
//        {
//            mg.Add(grid.Cells[i]);
//            if (i == grid.Cells.Count - 1 && mg.Length >= minMatches)
//            {
//                mgList.Add(mg);
//                mg = new MatchGroup();
//            }
//        }
//        else // start new match group
//        {
//            if (mg.Length >= minMatches)
//            {
//                mgList.Add(mg);
//            }
//            mg = new MatchGroup();
//            mg.Add(grid.Cells[i]);
//        }
//    }

//    if (X0X) // [i-2, i-1, i]
//    {
//        mg = new MatchGroup();

//        for (int i = 2; i < grid.Cells.Count; i++)
//        {
//            mg.Add(grid.Cells[i - 2]);
//            if (grid.Cells[i].IsMatchable && grid.Cells[i].IsMatchObjectEquals(mg.Cells[0]) && !grid.Cells[i - 1].IsMatchObjectEquals(mg.Cells[0])
//                && mg.Cells[0].IsMatchable && grid.Cells[i - 1].IsDraggable())
//            {
//                mg.Add(grid.Cells[i]);
//                mgList.Add(mg);
//            }
//            mg = new MatchGroup();
//        }
//    } // end X0X
//    return mgList;
//}

//Match3Helper.cs---------------------------------------------------------------------------------------------/

/////
//public class Match3Helper
//{
//    public MatchGrid grid;
//    public Dictionary<int, TargetData> curTargets;
//    public GameBoard board;
//    public Limit limits;
//    public PlayHelper plays;

//    public int match3Cycle;
//    public bool isCheckSwapTest;
//    public bool spawnObstacle;
//    public bool spawnObstacleRandomInCnt;
//    public int obstacleCnt;
//    public int[] obstacleArray;
//    public int[] targetArray;

//    //public int obstacleHitCnt;
//    public bool isBreakableObstacle;

//    public int gridSize;
//    public int rowSize;
//    public int colSize;
//    public int csvCnt;
//    public int match3CycleLimit;

//    public int blockedObjectHitCnt;
//    public int overlayObjectHitCnt;

//    public bool onlySpawnBlockedObject;
//    public bool onlySpawnOverlayObject;
//    public bool onlySpawnObstacleObject;



//    public int findingFeasible;



//    public bool isUsingGenetic;

//    public int wantDifficulty;
//    public int difficultyTolerance;

//    //public bool isPlay;
//    //public bool spawnTarget;
//    //public bool setObstaclePosition;
//    //public List<MatchGroup> l_mgList;
//    //public List<int> CellsContainer;
//    //public int[] cellCnts;
//    //public int feasibleIdx;
//    //public bool isCounting = false;
//    //public int count=0;
//    //public int csvCnt = 0;
//    //public bool isEnd = false;



//    public Match3Helper(MatchGrid g, Dictionary<int, TargetData> targets)
//    {
//        board = new GameBoard();
//        limits = new Limit();

//        csvCnt = 0;
//        match3Cycle = 0;
//        match3CycleLimit = 50;

//        // swap 수 측정을 위한 테스트 인지
//        isCheckSwapTest = true;
//        if (isCheckSwapTest)
//        {
//            limits.generation = 1;
//            //limits.generation = 100;
//            limits.geneticGeneration = 500;
//            limits.move = 200;
//            limits.repeat = 20;
//            limits.find = 2000;
//            limits.mix = 200;
//        }

//        isUsingGenetic = true;
//        spawnObstacle = true;  // 장애물 생성할건지
//        spawnObstacleRandomInCnt = true;  // 장애물을 일정 개수 랜덤위치하게 할건지
//        obstacleArray = new int[] { 3, 5, 6, 8, 9, 14, 16, 18, 20 };  // 장애물을 위치와 수를 정할건지
//        targetArray = new int[] { 3, 5, 6, 8, 9, 14, 16, 18, 20 };

//        blockedObjectHitCnt = 2;
//        overlayObjectHitCnt = 11111;  //getMatchableGroup은 IsMatchable만 확인하므로 ovelay스폰시 확인 필요


//        //전체 원래 사이즈 그 맵 크기 뭐시기 나중에 측정해야할듯
//        onlySpawnObstacleObject = false;
//        onlySpawnBlockedObject = true;
//        onlySpawnOverlayObject = false;


//        wantDifficulty = 600;
//        difficultyTolerance = 50;


//        grid = g;
//        curTargets = targets;
//        gridSize = g.Cells.Count;
//        rowSize = g.Rows.Count;
//        colSize = g.Columns.Count;
//        obstacleCnt = 0;


//        //setObstacleFromGene에서 장애물의 히트 수 정하기
//        //isBreakableObstacle = true;
//        //spawnTarget = false;
//        //isPlay = false;
//        //findingFeasible = 0;


//    }


//public void setTargetArray()
//{
//    //set target flower point
//    int middle_x = rowSize / 2;
//    int middle_y = colSize / 2;
//    int middle_top_x = middle_x;
//    int middle_top_y = 1;
//    int middle_bottom_x = middle_x;
//    int middle_bottom_y = colSize - 2;
//    int middle_left_x = 1;
//    int middle_left_y = middle_y;
//    int middle_right_x = rowSize - 2;
//    int middle_right_y = middle_y;

//    targetArray = new int[9];
//    targetArray[0] = (colSize * 1) + 1;
//    targetArray[1] = (colSize * 1) + rowSize - 2;
//    targetArray[2] = (colSize * (colSize - 2)) + 1;
//    targetArray[3] = (colSize * (colSize - 2)) + rowSize - 2;
//    targetArray[4] = (colSize * (middle_y)) + middle_y;
//    targetArray[5] = (colSize * (middle_top_y)) + middle_top_x;
//    targetArray[6] = (colSize * (middle_bottom_y)) + middle_bottom_x;
//    targetArray[7] = (colSize * (middle_left_y)) + middle_left_x;
//    targetArray[8] = (colSize * (middle_right_y)) + middle_right_x;
//}

////-- matchAndDestory ------------------------------------------------------------------------//

//public void matchAndDestory(DNA<char> p)
//{
//    if (estimateMax(plays.findMatchCnt, limits.find)) return;

//    if (grid.GetFreeCells(true).Count > 0)
//    {
//        plays.curState = 0;
//        return;
//    }

//    createMatchGroups(3, false, grid);

//    if (grid.mgList.Count == 0)
//    {
//        plays.findMatchCnt = 0;
//        plays.curState = 1;
//    }

//    else
//    {
//        //for (int i = 0; i < grid.mgList.Count; i++) destroyAndCntBlocks(grid.mgList[i]);
//        //p.matchCnt++;
//        //plays.curState = 0;

//        for (int i = 0; i < grid.mgList.Count; i++)
//        {
//            foreach (var item in curTargets)
//            {
//                if (!item.Value.Achieved)
//                {
//                    for (int j = 0; j < grid.mgList[i].Cells.Count; j++)
//                    {
//                        List<int> mgCells = grid.mgList[i].Cells[j].GetGridObjectsIDs();
//                        if (mgCells[0] == item.Value.ID) item.Value.IncCurrCount(1);

//                        destoryNeigborObstacle(grid.mgList[i].Cells[j].Neighbors.Top);
//                        destoryNeigborObstacle(grid.mgList[i].Cells[j].Neighbors.Left);
//                        destoryNeigborObstacle(grid.mgList[i].Cells[j].Neighbors.Right);
//                        destoryNeigborObstacle(grid.mgList[i].Cells[j].Neighbors.Bottom);

//                        grid.mgList[i].Cells[j].DestroyGridObjects();
//                    }
//                }
//            }
//        }

//        p.matchCnt++;
//        plays.curState = 0;

//        //for (int i = 0; i < grid.Cells.Count; i++)
//        //{

//        //}


//        //for (int i = 0; i < grid.mgList.Count; i++)
//        //{
//        //    for (int j = 0; j < grid.mgList[i].Cells.Count; j++)
//        //    {
//        //        if(grid.mgList[i].Cells[j].Neighbors.Top != null)
//        //        {
//        //            if(grid.mgList[i].Cells[j].Neighbors.Top.IsMatchable == false)
//        //            {
//        //                GridObject g = grid.mgList[i].Cells[j].Neighbors.Top.GetGridObjects();
//        //            }
//        //        }


//        //        grid.mgList[i].Cells[j].DestroyGridObjects();

//        //    }
//        //}

//    }

//    //for(int i= 0;i<)
//}

////-- matchAndDestory --////////////////////////////////////////////////////////////////////////

//public void destroyAndCntBlocks(MatchGroup mg)
//{
//    foreach (var item in curTargets)
//    {
//        if (!item.Value.Achieved)
//        {
//            switch (item.Value.ID)
//            {
//                case int n when (n >= 1000 && n <= 1006):
//                    destroyAndCntMatch(mg, item.Value);
//                    break;
//                case 200001:
//                    destroyAndCntUnderlay(mg, item.Value);
//                    break;
//                case 100004:
//                    destroyAndCntOverlay(mg, item.Value);
//                    break;
//                case 101:
//                    destroyAndCntBlocked(mg, item.Value);
//                    break;
//            }
//        }
//    }
//}

//public void destroyAndCntMatch(MatchGroup mg, TargetData td)
//{
//    for (int i = 0; i < mg.Cells.Count; i++)
//    {
//        List<int> mgCells = mg.Cells[i].GetGridObjectsIDs();

//        if (mgCells[0] == td.ID) td.IncCurrCount(1);

//        mg.Cells[i].DestroyGridObjects();
//    }
//}
//public void destroyAndCntUnderlay(MatchGroup mg, TargetData td)
//{
//    for (int i = 0; i < mg.Cells.Count; i++)
//    {
//        List<int> mgCells = mg.Cells[i].GetGridObjectsIDs();

//        if (mgCells.Count > 1) td.IncCurrCount(1);

//        mg.Cells[i].DestroyGridObjects();
//    }
//}

//public void destroyAndCntOverlay(MatchGroup mg, TargetData td)
//{
//    for (int i = 0; i < mg.Cells.Count; i++)
//    {
//        List<int> mgCells = mg.Cells[i].GetGridObjectsIDs();

//        if (mgCells.Count > 1)
//        {
//            mg.Cells[i].Overlay.hitCnt++;

//            if (mg.Cells[i].Overlay.hitCnt == 2)
//            {
//                td.IncCurrCount(1);
//                mg.Cells[i].DestroyGridObjects();
//            }

//        }
//        else mg.Cells[i].DestroyGridObjects();
//    }
//}


//public void blockedHit(GridCell g, TargetData td)
//{
//    if (g.Blocked.hitCnt == g.Blocked.Protection)
//    {
//        td.IncCurrCount(1);
//        g.DestroyGridObjects();
//    }
//}

//public int destroyAndCntBlocked(MatchGroup mg, TargetData td)
//{
//    int result = 0;
//    for (int i = 0; i < mg.Cells.Count; i++)
//    {
//        if (mg.Cells[i].Neighbors.Top != null)
//        {
//            if (estimateNeighborBlocked(mg.Cells[i].Neighbors.Top))
//            {
//                mg.Cells[i].Neighbors.Top.Blocked.hitCnt++;
//                blockedHit(mg.Cells[i].Neighbors.Top, td);
//            }
//        }

//        if (mg.Cells[i].Neighbors.Left != null)
//        {
//            if (estimateNeighborBlocked(mg.Cells[i].Neighbors.Left))
//            {
//                mg.Cells[i].Neighbors.Left.Blocked.hitCnt++;
//                blockedHit(mg.Cells[i].Neighbors.Left, td);
//            }
//        }

//        if (mg.Cells[i].Neighbors.Right != null)
//        {
//            if (estimateNeighborBlocked(mg.Cells[i].Neighbors.Right))
//            {
//                mg.Cells[i].Neighbors.Right.Blocked.hitCnt++;
//                blockedHit(mg.Cells[i].Neighbors.Right, td);
//            }
//        }

//        if (mg.Cells[i].Neighbors.Bottom != null)
//        {
//            if (estimateNeighborBlocked(mg.Cells[i].Neighbors.Bottom))
//            {
//                mg.Cells[i].Neighbors.Bottom.Blocked.hitCnt++;
//                blockedHit(mg.Cells[i].Neighbors.Bottom, td);
//            }
//        }
//    }

//    for (int i = 0; i < mg.Cells.Count; i++) mg.Cells[i].DestroyGridObjects();

//    return result;
//}


//////////////////////////////////////////////////////////////////////////////////////////////


//public void cntMatchPottentials(DNA<char> p)
//{
//    int minMatches = 3;
//    grid.mgList = new List<MatchGroup>();

//    grid.Rows.ForEach((br) =>
//    {
//        List<MatchGroup> mgList_t = br.getMatchableGroup(minMatches);

//        for (int i = 0; i < mgList_t.Count; i++)
//        {
//            grid.mgList.Add(mgList_t[i]);
//        }

//        //grid.mgList.Add(mgList_t);

//        //if (mgList_t != null && mgList_t.Count > 0)
//        //{
//        //    addRange(mgList_t, grid);
//        //}
//    });

//    grid.Columns.ForEach((bc) =>
//    {
//        List<MatchGroup> mgList_t = bc.getMatchableGroup(minMatches);

//        for (int i = 0; i < mgList_t.Count; i++)
//        {
//            grid.mgList.Add(mgList_t[i]);
//        }

//        //if (mgList_t != null && mgList_t.Count > 0)
//        //{
//        //    addRange(mgList_t, grid);
//        //}
//    });

//    p.matchFromMap = grid.mgList.Count;

//    for (int i = 0; i < grid.mgList.Count; i++)
//    {
//        if (haveNeigborBreakableObstacle(grid.mgList[i].Cells[0]))
//        {
//            p.nearBreakableObstacles++;
//            continue;
//        }

//        if (haveNeigborBreakableObstacle(grid.mgList[i].Cells[1]))
//        {
//            p.nearBreakableObstacles++;
//            continue;
//        }

//        if (haveNeigborBreakableObstacle(grid.mgList[i].Cells[2]))
//        {
//            p.nearBreakableObstacles++;
//        }
//    }

//    for (int i = 0; i < grid.mgList.Count; i++)
//    {
//        if (grid.mgList[i].Cells[0].Overlay != null)
//        {
//            p.includeMatchObstacles++;
//            continue;
//        }

//        if (haveNeigborBreakableObstacle(grid.mgList[i].Cells[1]))
//        {
//            p.includeMatchObstacles++;
//            continue;
//        }

//        if (haveNeigborBreakableObstacle(grid.mgList[i].Cells[2]))
//        {
//            p.includeMatchObstacles++;
//        }
//    }
//}


//public void cntObstaclePottential(DNA<char> p)
//{
//    MatchGroup mg = new MatchGroup();
//    for (int i = 0; i < grid.Cells.Count; i++) grid.Cells[i].cellMatchPotential = 0;
//    for (int i = 0; i < grid.Cells.Count; i++) mg.cntMatchPottential(grid, i);

//    for (int i = 0; i < grid.Cells.Count; i++)
//    {
//        p.obstaclePottentialCnt += grid.Cells[i].cellMatchPotential;
//    }
//}

//public void cntBlockedPottential(DNA<char> p)
//{
//    MatchGroup mg = new MatchGroup();
//    for (int i = 0; i < grid.Cells.Count; i++) grid.Cells[i].cellMatchPotential = 0;
//    for (int i = 0; i < grid.Cells.Count; i++) mg.cntMatchPottential(grid, i);

//    for (int i = 0; i < grid.Cells.Count; i++)
//    {
//        p.blockedPottentialCnt += grid.Cells[i].cellMatchPotential;
//    }
//}

//public void cntOverlayPottential(DNA<char> p)
//{
//    MatchGroup mg = new MatchGroup();
//    for (int i = 0; i < grid.Cells.Count; i++) grid.Cells[i].cellMatchPotential = 0;
//    for (int i = 0; i < grid.Cells.Count; i++) mg.cntMatchPottential(grid, i);

//    for (int i = 0; i < grid.Cells.Count; i++)
//    {
//        p.overlayPottentialCnt += grid.Cells[i].cellMatchPotential;
//    }
//}


//public void countObstacleData(DNA<char> p)
//{
//    MatchGroup mg = new MatchGroup();

//    for (int i = 0; i < grid.Cells.Count; i++) grid.Cells[i].matchFromSwapPotential = 0;
//    for (int i = 0; i < grid.Cells.Count; i++) mg.cntMatchPottential(grid, i);


//    for (int i = 0; i < grid.Cells.Count; i++)
//    {
//        p.mapMatchPotential += grid.Cells[i].matchFromSwapPotential;
//        //ga.possibleCounting[m3h.grid.Cells[i].matchPotential]++;
//        p.mapMatchPotentialList.Add(grid.Cells[i].matchFromSwapPotential);

//        p.breakableObstacle += grid.Cells[i].nearBreakableObstacle;

//        if (grid.Cells[i].DynamicObject == null) p.obstacleCnt++;
//    }

//}




////public void countObstacleData(DNA<char> p)
////{
////    MatchGroup mg = new MatchGroup();

////    //근데 블록정보를 다지웠었는데 그러면 초기화 필요있나
////    for (int i = 0; i < grid.Cells.Count; i++) grid.Cells[i].cellMatchPotential = 0;
////    //for (int i = 0; i < grid.Cells.Count; i++) mg.countPossible(grid, i);


////    for (int i = 0; i < grid.Cells.Count; i++) mg.cntMatchPottential(grid, i);

////    //if (isBreakableObstacle)
////    //{
////    //    for (int i = 0; i < grid.Cells.Count; i++)
////    //    {
////    //        if (grid.Cells[i].Blocked != null)
////    //        {
////    //            if (grid.Cells[i].Blocked.Destroyable)
////    //            {
////    //                addObstacleDestroyableCells(p, grid.Cells[i].Neighbors.Top);
////    //                addObstacleDestroyableCells(p, grid.Cells[i].Neighbors.Left);
////    //                addObstacleDestroyableCells(p, grid.Cells[i].Neighbors.Right);
////    //                addObstacleDestroyableCells(p, grid.Cells[i].Neighbors.Bottom);
////    //            }
////    //        } 
////    //    }
////    //}

////    //for (int i = 0; i < p.breakableObstacleCellList.Count; i++)
////    //{
////    //    p.breakableObstacle += grid.Cells[p.breakableObstacleCellList[i].Row * rowSize + p.breakableObstacleCellList[i].Column].cellMatchPotential;
////    //}



////    //ga.possibleCounting = new int[m3h.colSize * m3h.rowSize];

////    for (int i = 0; i < grid.Cells.Count; i++)
////    {
////        p.mapMatchPotential += grid.Cells[i].cellMatchPotential;
////        //ga.possibleCounting[m3h.grid.Cells[i].matchPotential]++;
////        p.mapMatchPotentialList.Add(grid.Cells[i].cellMatchPotential);

////        p.breakableObstacle += grid.Cells[i].nearBreakableObstacle;

////        if (grid.Cells[i].DynamicObject == null) p.obstacleCnt++;

////        //if (grid.Cells[i].Blocked != null)
////        //{
////        //    if(grid.Cells[i].Blocked.Destroyable) p.blockedPottentialCnt += grid.Cells[i].cellMatchPotential;
////        //    else p.obstaclePottentialCnt += grid.Cells[i].cellMatchPotential;
////        //} 

////        //else if (grid.Cells[i].Overlay != null) p.overlayPottentialCnt += grid.Cells[i].cellMatchPotential;

////    }

////}





//public void addObstacleDestroyableCells(DNA<char> p, GridCell gc)
//{
//    if (gc != null)
//    {
//        if (gc.IsDraggable())
//        {
//            if (p.breakableObstacleCellList.Count == 0) p.breakableObstacleCellList.Add(gc);

//            else
//            {
//                for (int i = 0; i < p.breakableObstacleCellList.Count; i++)
//                {
//                    if (p.breakableObstacleCellList[i].Column == gc.Column &&
//                        p.breakableObstacleCellList[i].Row == gc.Row) return;
//                }
//                p.breakableObstacleCellList.Add(gc);
//            }
//        }
//    }
//}



////public FitnessHelper(List<GridCell> cells, List<int> collectID)
////{
////    fixGrid = new List<CellInfo>(cells.Count);
////    l_mgList = new List<MatchGroup>();

////    for (int i = 0; i < 30; i++) fixGrid.Add(new CellInfo(cells[i].Row, cells[i].Column, collectID[i]));
////}
////public class CellInfo
////{
////    public int row;
////    public int col;
////    public int objectID;

////    public CellInfo(int c_row, int c_col, int c_ID)
////    {
////        row = c_row;
////        col = c_col;
////        objectID = c_ID;
////    }
////}

////GetFreeCells


////public void createMatchGroups1(int minMatches, bool estimate, MatchGrid grid, DNA<char> p)
////{
////    //l_mgList = new List<MatchGroup>();
////    grid.mgList = new List<MatchGroup>();
////    if (!estimate)
////    {
////        grid.Rows.ForEach((br) =>
////        {
////            List<MatchGroup> mgList_t = br.GetMatches(minMatches, false);
////            if (mgList_t != null && mgList_t.Count > 0)
////            {
////                addRange(mgList_t, grid);
////            }
////        });

////        grid.Columns.ForEach((bc) =>
////        {
////            List<MatchGroup> mgList_t = bc.GetMatches(minMatches, false);
////            if (mgList_t != null && mgList_t.Count > 0)
////            {
////                addRange(mgList_t, grid);
////            }
////        });
////    }
////    else
////    {
////        List<MatchGroup> mgList_t = new List<MatchGroup>();
////        grid.Rows.ForEach((gr) =>
////        {
////            mgList_t.AddRange(gr.GetMatches1(minMatches, true));
////        });
////        mgList_t.ForEach((mg) => { if (mg.IsEstimateMatch1(mg.Length, true, grid, p)) { addEstimate(mg, grid); } });

////        mgList_t = new List<MatchGroup>();
////        grid.Columns.ForEach((gc) =>
////        {
////            mgList_t.AddRange(gc.GetMatches1(minMatches, true));
////        });
////        mgList_t.ForEach((mg) => { if (mg.IsEstimateMatch1(mg.Length, false, grid, p)) { addEstimate(mg, grid); } });
////    }
////}

////public void new_createMatchGroups(int minMatches, bool estimate, GridCell[,] curCell)
////{
////    //mgList = new List<MatchGroup>();

////    if (!estimate)
////    {
////        //grid.Rows.ForEach((br) =>
////        //{
////        //    List<MatchGroup> mgList_t = br.new_GetMatches(minMatches, false, curCell);
////        //    row_idx++;
////        //    if (mgList_t != null && mgList_t.Count > 0)
////        //    {
////        //        addRange(mgList_t, grid);
////        //    }
////        //});

////        //grid.Columns.ForEach((bc) =>
////        //{
////        //    List<MatchGroup> mgList_t = bc.new_GetMatches(minMatches, false, curCell);
////        //    col_idx++;
////        //    if (mgList_t != null && mgList_t.Count > 0)
////        //    {
////        //        addRange(mgList_t, grid);
////        //    }
////        //});
////    }
////    else
////    {
////        List<MatchGroup> mgList_t = new List<MatchGroup>();
////        grid.Rows.ForEach((gr) =>
////        {
////            mgList_t.AddRange(gr.new_GetMatches(minMatches, true, curCell));

////        });

////        mgList_t.ForEach((mg) => 
////        {
////            if (mg.new_IsEstimateMatch(mg.Length, true, curCell))
////            {
////                new_addEstimate(mg, l_mgList);
////            }
////        });

////        mgList_t = new List<MatchGroup>();
////        grid.Columns.ForEach((gc) =>
////        {
////            mgList_t.AddRange(gc.new_GetMatches(minMatches, true, curCell));
////        });

////        mgList_t.ForEach((mg) => 
////        {
////            if (mg.new_IsEstimateMatch(mg.Length, false, curCell))
////            {
////                new_addEstimate(mg, l_mgList);
////            }
////        });
////    }
////}



//public void createFillPath(MatchGrid g)
//{
//    if (!g.haveFillPath)
//    {
//        Debug.Log("mh Make gravity fill path");
//        Map map = new Map(g);
//        PathFinder pF = new PathFinder();

//        g.Cells.ForEach((c) =>
//        {
//            if (!c.Blocked && !c.IsDisabled && !c.MovementBlocked)
//            {
//                int length = int.MaxValue;
//                List<GridCell> path = null;
//                g.Columns.ForEach((col) =>
//                {
//                    if (col.Spawn)
//                    {
//                        if (col.Spawn.gridCell != c)
//                        {
//                            pF.CreatePath(map, c.pfCell, col.Spawn.gridCell.pfCell);
//                            if (pF.FullPath != null && pF.PathLenght < length) { path = pF.GCPath(); length = pF.PathLenght; }
//                        }
//                        else
//                        {
//                            length = 0;
//                            path = new List<GridCell>();
//                        }
//                    }
//                });
//                c.fillPathToSpawner = path;
//            }
//        });
//    }
//    else
//    {
//        Debug.Log("Have predefined fill path");
//        PBoard pBoard = g.LcSet.GetBoard(g);
//        g.Cells.ForEach((c) =>
//        {
//            if (!c.Blocked && !c.IsDisabled && !c.MovementBlocked && !c.spawner)
//            {
//                //   Debug.Log("path for " + c);
//                GridCell next = c;
//                List<GridCell> path = new List<GridCell>();
//                GridCell mather = null;
//                GridCell neigh = null;
//                bool end = false;
//                DirMather dir = DirMather.None;
//                bool clampDir = false;
//                while (!end)
//                {
//                    dir = (!clampDir) ? pBoard[next.Row, next.Column] : dir;
//                    NeighBors nS = next.Neighbors;
//                    //     Debug.Log(dir);
//                    switch (dir)
//                    {
//                        case DirMather.None:
//                            neigh = null;
//                            break;
//                        case DirMather.Top:
//                            neigh = nS.Top;
//                            break;
//                        case DirMather.Right:
//                            neigh = nS.Right;
//                            break;
//                        case DirMather.Bottom:
//                            neigh = nS.Bottom;
//                            break;
//                        case DirMather.Left:
//                            neigh = nS.Left;
//                            break;
//                    }

//                    if (neigh && neigh.spawner)
//                    {
//                        //  Debug.Log("spawner neigh " + neigh);
//                        path.Add(neigh);
//                        if (mather) mather = neigh;
//                        end = true;
//                    }
//                    else if (!neigh)
//                    {
//                        //  Debug.Log("none neigh ");
//                        end = true;
//                        path = null;
//                    }
//                    else if (neigh)
//                    {
//                        if (!neigh.Blocked && !neigh.IsDisabled && !neigh.MovementBlocked)
//                        {
//                            if (path.Contains(neigh)) // corrupted path
//                            {
//                                // Debug.Log("corruptred neigh " + neigh);
//                                end = true;
//                                path = null;
//                            }
//                            else
//                            {
//                                clampDir = false;
//                                path.Add(neigh);
//                                next = neigh;
//                                // Debug.Log("add " + neigh);
//                                clampDir = pBoard[next.Row, next.Column] == DirMather.None; // предусмотреть отсутствие направление у ячейки (save pevious dir)
//                            }
//                        }
//                        else if (neigh.IsDisabled) // passage cell
//                        {
//                            next = neigh;
//                            clampDir = true;
//                            //  Debug.Log("disabled " + neigh);
//                        }
//                        else
//                        {
//                            //  Debug.Log("another block " + neigh);
//                            end = true;
//                            path = null;
//                        }
//                    }
//                }
//                c.fillPathToSpawner = path;
//            }
//        });
//    }
//}

//public void getParent(List<GridCell> freeCells)
//{

//}



//using Mkey;
/////
//using System.Collections.Generic;
/////
//public void fillGridByStep(List<GridCell> freeCells, Action completeCallBack)
//{
//    if (freeCells.Count == 0)
//    {
//        //completeCallBack?.Invoke();
//        return;
//    }

//    foreach (GridCell gc in freeCells)
//    {
//        gc.fillGrab1(completeCallBack);
//    }


//    //ParallelTween tp = new ParallelTween();
//    //foreach (GridCell gc in freeCells)
//    //{
//    //    tp.Add((callback) =>
//    //    {
//    //        gc.FillGrab1(callback);
//    //    });
//    //}
//    //tp.Start1(() =>
//    //{
//    //    //completeCallBack?.Invoke();
//    //});
//}

//public void collectFalling(MatchGrid grid)
//{
//    //   Debug.Log("collect falling " + GetFalling().Count);
//    ParallelTween pt = new ParallelTween();
//    foreach (var item in getFalling(grid))
//    {
//        pt.Add((callBack) =>
//        {
//            item.Collect(0, false, true, callBack);
//        });
//    }
//    pt.Start1();
//}


//public List<FallingObject> getFalling(MatchGrid grid)
//{
//    List<GridCell> botCell = grid.GetBottomDynCells();
//    List<FallingObject> res = new List<FallingObject>();
//    foreach (var item in botCell)
//    {
//        if (item)
//        {
//            FallingObject f = item.Falling;
//            if (f)
//            {
//                res.Add(f);
//            }
//        }
//    }
//    return res;
//}

//public void collectMatchGroups(MatchGrid grid)
//{
//    ParallelTween pt = new ParallelTween();

//    if (grid.mgList.Count == 0) return;


//    for (int i = 0; i < grid.mgList.Count; i++)
//    {
//        if (grid.mgList[i] != null)
//        {
//            MatchGroup m = grid.mgList[i];
//            pt.Add((callBack) =>
//            {
//                //Collect(m, callBack);
//            });
//        }
//    }
//    pt.Start1(() =>
//    {

//    });
//}



//public bool[,] isVisited;
//public int[,] connected;
//public int[] possibleArea;

//public bool isConnected(MatchGrid grid, int cnt)
//{
//    GridCell[,] curCell = new GridCell[grid.Columns.Count, grid.Rows.Count];

//    for (int i = 0; i < grid.Columns.Count; i++)
//    {
//        for (int j = 0; j < grid.Rows.Count; j++)
//        {
//            curCell[i, j] = grid.Cells[i * grid.Rows.Count + j];
//        }
//    }

//    isVisited = new bool[grid.Columns.Count, grid.Rows.Count];
//    connected = new int[grid.Columns.Count, grid.Rows.Count];

//    //for (int i = 0; i < grid.Columns.Count; i++)
//    //{
//    //    for (int j = 0; j < grid.Rows.Count; j++)
//    //    {
//    //        isVisited[i,j] = true;

//    //        if(curCell[i, j].DynamicObject)
//    //        {
//    //            bool isReachSpawn = false;
//    //            visitConnectCell(curCell, i, j, isReachSpawn);
//    //            break;
//    //        }


//    //        //if (!isVisited[i, j] && curCell[i, j].DynamicObject)
//    //        //{
//    //        //    bool isReachSpawn = false;
//    //        //    visitConnectCell(curCell, i, j, isReachSpawn);
//    //        //    if (!isReachSpawn) return false;
//    //        //}
//    //    }

//    //    break;
//    //}

//    for (int row = 0; row < grid.Rows.Count; row++)
//    {
//        for (int col = 0; col < grid.Columns.Count; col++)
//        {
//            if(!curCell[col, row].DynamicObject)
//            {
//                isVisited[col, row] = true;
//                connected[col, row] = grid.Columns.Count + 1;
//            }
//        }
//    }

//    int area = 1;

//    for (int row = 0; row < grid.Rows.Count; row++)
//    {
//        int col = 0;

//        if (!isVisited[col, row])
//        {
//            isVisited[col, row] = true;
//            bool isReachSpawn = false;
//            visitConnectCell(curCell, col, row, isReachSpawn);

//            for (int i = 0; i < grid.Columns.Count; i++)
//            {
//                for (int j = 0; j < grid.Rows.Count; j++)
//                {
//                    if (isVisited[i,j] && connected[i, j] == 0) connected[i, j] = area;
//                }
//            }

//            area++;
//        }
//    }

//    possibleArea = new int[grid.Columns.Count + 2];

//    for (int i = 0; i < grid.Columns.Count; i++)
//    {
//        for (int j = 0; j < grid.Rows.Count; j++)
//        {
//            possibleArea[connected[i, j]] += grid.Cells[i * grid.Rows.Count + j].possibleCnt;
//        }
//    }


//    //int[,] possibleCnts = new int[grid.Columns.Count, grid.Rows.Count];
//    //int[] cnts = new int[16];

//    //for (int i = 0; i < grid.Columns.Count; i++)
//    //{
//    //    for (int j = 0; j < grid.Rows.Count; j++)
//    //    {
//    //        possibleCnts[i, j] = grid.Cells[i * grid.Rows.Count + j].possibleCnt;
//    //        cnts[grid.Cells[i * grid.Rows.Count + j].possibleCnt]++;
//    //    }
//    //}



//    return true;
//}

//void visitConnectCell(GridCell[,] curCell, int col, int row, bool isReachSpawn)
//{
//    if (canVisit(curCell, col, row - 1)) // Top
//    {
//        if (row == 0) isReachSpawn = true;
//        isVisited[col,row - 1] = true;
//        visitConnectCell(curCell, col, row - 1, isReachSpawn);
//    }

//    if (canVisit(curCell, col, row + 1)) // Bottom
//    {
//        if (row == 0) isReachSpawn = true;
//        isVisited[col, row + 1] = true;
//        visitConnectCell(curCell, col, row + 1, isReachSpawn);
//    }

//    if (canVisit(curCell, col - 1, row)) // Left
//    {
//        if (row == 0) isReachSpawn = true;
//        isVisited[col - 1,row] = true;
//        visitConnectCell(curCell, col - 1, row, isReachSpawn);
//    }

//    if (canVisit(curCell, col + 1, row)) // Right
//    {
//        if (row == 0) isReachSpawn = true;
//        isVisited[col + 1,row] = true;
//        visitConnectCell(curCell, col + 1, row, isReachSpawn);
//    }

//    //if (canVisit(curCell, col - 1, row - 1)) // TopLeft
//    //{
//    //    if (row == 0) isReachSpawn = true;
//    //    isVisited[col - 1,row - 1] = true;
//    //    visitConnectCell(curCell, col - 1, row - 1, isReachSpawn);
//    //}

//    //if (canVisit(curCell, col - 1, row + 1)) // BottomLeft
//    //{
//    //    if (row == 0) isReachSpawn = true;
//    //    isVisited[col - 1, row + 1] = true;
//    //    visitConnectCell(curCell, col - 1, row + 1, isReachSpawn);
//    //}

//    //if (canVisit(curCell, col + 1, row - 1)) // TopRight
//    //{
//    //    if (row == 0) isReachSpawn = true;
//    //    isVisited[col + 1, row - 1] = true;
//    //    visitConnectCell(curCell, col + 1, row - 1, isReachSpawn);
//    //}

//    //if (canVisit(curCell, col + 1, row + 1)) // BottomRight
//    //{
//    //    if (row == 0) isReachSpawn = true;
//    //    isVisited[col + 1, row + 1] = true;
//    //    visitConnectCell(curCell, col + 1, row + 1, isReachSpawn);
//    //}
//}

//bool canVisit(GridCell[,] curCell, int col, int row)
//{
//    if( col >= 0 && row >= 0 && col < grid.Columns.Count && row < grid.Rows.Count)
//    {
//        if(!isVisited[col, row] && curCell[col, row].DynamicObject) return true;
//        else return false;

//        //if (col >= 0 && row >= 0 && col <= grid.Columns.Count && row <= grid.Rows.Count)
//        //{
//        //    if (curCell[col, row].DynamicObject) return true;
//        //        else return false;
//        //}
//    }


//    //if (col >= 0 && col < curCell[col,row].GColumn.Length && row >= 0 && row < curCell[col, row].GRow.Length)
//    //{
//    //    if (curCell[col, row].DynamicObject) return true;
//    //    else return false;
//    //}

//    else return false;
//}


//public void createAvailableMatchGroup(MatchGrid grid)
//{
//    for (int i = 0; i < grid.Cells.Count; i++) grid.Cells[i].matchFromSwapPotential = 0;

//    //grid.mgList = new List<MatchGroup>();

//    //List<MatchGroup> mgList_t = new List<MatchGroup>();
//    //grid.Rows.ForEach((gr) =>
//    //{
//    //    mgList_t.AddRange(gr.isContinuousRow());
//    //});
//    //mgList_t.ForEach((mg) => { mg.countPossible(mg.Length, true, grid);});

//    //mgList_t = new List<MatchGroup>();
//    //grid.Columns.ForEach((gc) =>
//    //{
//    //    mgList_t.AddRange(gc.isContinuousColumn());
//    //});

//    //mgList_t.ForEach((mg) => { mg.countPossible(mg.Length, false, grid); });

//    List<int> pc = new List<int>();

//    for (int i = 0; i < grid.Cells.Count; i++)
//    {
//        pc.Add(grid.Cells[i].matchFromSwapPotential);
//    }


//    int a = 0;

//}





////public GridCell GetLowermostX(MatchGrid grid)
////{
////    if (grid.Cells.Count == 0) return null;
////    GridCell l = grid.Cells[0];
////    for (int i = 0; i < grid.Cells.Count; i++) if (grid.Cells[i].Column < l.Column) l = grid.Cells[i];
////    return l;
////}

////public GridCell GetTopmostX(MatchGrid grid)
////{
////    if (grid.Cells.Count == 0) return null;
////    GridCell t = grid.Cells[0];
////    for (int i = 0; i < grid.Cells.Count; i++) if (grid.Cells[i].Column > t.Column) t = grid.Cells[i];
////    return t;
////}

////public GridCell GetLowermostY(MatchGrid grid)
////{
////    if (grid.Cells.Count == 0) return null;
////    GridCell l = grid.Cells[0];
////    for (int i = 0; i < grid.Cells.Count; i++) if (grid.Cells[i].Row > l.Row) l = grid.Cells[i];
////    return l;
////}

////public GridCell GetTopmostY(MatchGrid grid)
////{
////    if (grid.Cells.Count == 0) return null;
////    GridCell t = grid.Cells[0];
////    for (int i = 0; i < grid.Cells.Count; i++) if (grid.Cells[i].Row < t.Row) t = grid.Cells[i];
////    return t;
////}


//public class CellPosition
//{
//    public List<int> cellPosition;

//    public List<int> setCellPostion(int cnt, bool isObstacle, int gridSize)
//    {
//        cellPosition = new List<int>();

//        for (int i = 0; i < gridSize; i++) cellPosition.Add(0);

//        if (isObstacle)
//        {
//            for (int i = 0; i < cnt; i++)
//            {
//                int randNum = Random.Range(0, gridSize - 1);
//                cellPosition[randNum] = 1;
//            }
//        }

//        else
//        {

//        }

//        return cellPosition;
//    }

//    public void setCellPostion(int[] array)
//    {

//    }
//}

//GeneticAlgorithm.cs--------------------------------------------------------------------/

//public class GeneticAlgorithm<T>
//{
//    //public int targetMove = 30;
//    //public double targetStd = 15;


//    //public class CountResult
//    //{
//    //    public List<int> infeasiblePopulation;
//    //    public List<int> feasiblePopulation;

//    //    public List<int> curGenerationBestMean;
//    //    public List<int> curGenerationBestStd;
//    //    public List<int> curGenerationBestFitness;

//    //    public List<int> bestMeanMove;
//    //    public List<int> bestStd;
//    //    public List<int> bestFitness;
//    //    public List<int> bestMoves;

//    //    public List<int> repeatSwap;
//    //    public List<int> repeatMatch;
//    //}




//    //public int generation = 1;
//    //public int populationSize = 20;
//    //public int elitism = 2;
//    //public float mutationRate = 0.01f;
//    //public int generationLimit = 2;
//    //public int findFeasibleLimit = 200;
//    //public int repeat = 1;
//    //public int moveLimit = 200;
//    //public int targetMove = 30;
//    //public double targetStd = 15;
//    //public double targetFitness = 0.9;
//    public int geneticGeneration;

//    public int generation;

//    public int populationSize;
//    public int elitism;
//    public float mutationRate;
//    public int generationLimit;
//    public int findFeasibleLimit;
//    public int repeat;
//    public int moveLimit;

//    public int targetMove;
//    public double targetStd;
//    public double targetFitness;

//    public List<DNA<T>> population;
//    public List<DNA<T>> feasiblePopulation;
//    public List<DNA<T>> infeasiblePopulation;

//    private Random random;
//    private int dnaSize;

//    public double feasibleFitnessSum;
//    public double infeasibleFitnessSum;

//    public double curGenerationBestMean;
//    public double curGenerationBestStd;
//    public double curGenerationBestFitness;

//    public double bestMean;
//    public double bestStd;
//    public double bestFitness;
//    public int bestFitnessIdx;

//    //public double feasibleMeanMoveSum;
//    //public double infeasibleBlockCntSum;

//    //public double curGenerationBestMean;
//    //public double curGenerationBestStd;
//    //public double curGenerationBestFitness;

//    //public double bestMeanMove;
//    //public double bestStd;
//    //public double bestFitness;

//    //public List<double> feasibleParent;
//    //public List<int> feasibleParentIdx;
//    //public List<double> infeasibleParent;
//    //public List<int> infeasibleParentIdx;
//    //public List<int> curBestMoves;
//    //public List<int> bestMoves;
//    //public List<List<int>> repeatMovements;
//    //public int repeatMovementsCnt;


//    //public List<List<int>> allMovements;
//    //public List<List<int>> obstructionRates;
//    //public List<List<int>> shorCutRates;

//    //public List<int> targetNeedCnt;
//    ////public int possibleCnt;
//    //public bool isPossible;

//    //public List<int> possibleCountingList;

//    ////public int[] possibleCounting;

//    //public int blockeCnt = 0;
//    ////public int[] targetArray = { 1, 2, 3 };
//    ////public int[] geneArray = { 7, 8, 9 };

//    //public List<DNA<T>> addObstaclePopulation;
//    //public bool isAddObstacle;

//    public GeneticAlgorithm(int cellSize, Random random, Func<T> getRandomGene, Func<T[]> getGenes, Match3Helper m3h)
//    {
//        geneticGeneration = 1;
//        generation = 1;

//        feasibleFitnessSum = 0;
//        infeasibleFitnessSum = 0;

//        //if (m3h.isCheckSwapTest)
//        //{
//        //    populationSize = 1;
//        //    elitism = 0;
//        //    mutationRate = 0;
//        //    targetMove = 0;
//        //    targetStd = 0;
//        //    targetFitness = 0;
//        //}

//        //else
//        //{
//        //    populationSize = 20;
//        //    elitism = 2;
//        //    mutationRate = 0.01f;
//        //    targetMove = 0;
//        //    targetStd = 0;
//        //    targetFitness = 0.9;
//        //}


//        if (m3h.isUsingGenetic)
//        {
//            populationSize = 20;
//            elitism = 2;
//            mutationRate = 0.01f;
//            targetMove = 0;
//            targetStd = 0;
//            targetFitness = 1;
//        }

//        population = new List<DNA<T>>(populationSize);
//        feasiblePopulation = new List<DNA<T>>();
//        infeasiblePopulation = new List<DNA<T>>();
//        this.random = random;
//        dnaSize = cellSize;

//        bestFitness = 0;
//        bestFitnessIdx = 0;


//        for (int i = 0; i < populationSize; i++)
//        {
//            population.Add(new DNA<T>(dnaSize, random, getRandomGene, getGenes, shouldInitGenes: true));
//        }


//        //infeasiblePopulationCnt = new List<int>();
//        //feasiblePopulationCnt = new List<int>();

//        //bestMeanMove = 0;
//        //bestStd = 0;

//        //repeatMovements = new List<List<int>>();
//        //obstructionRates = new List<List<int>>();
//        //shorCutRates = new List<List<int>>();
//        //targetNeedCnt = new List<int>();
//        ////possibleCnt = 0;
//        //isPossible = true;
//        //allMovements = new List<List<int>>();
//        ////possibleCountingList = new List<int>();
//        //isAddObstacle = false;


//    }

//    public void newInitOnePopulation()
//    {
//        population[0].newInit();
//    }




//    public void newGeneration()
//    {
//        population.Clear();

//        if (feasiblePopulation.Count != 0)
//        {
//            feasiblePopulation.Sort(CompareDNA);

//            //for (int i = 0; i < feasiblePopulation.Count; i++) feasibleMeanMoveSum += feasiblePopulation[i].mean;

//            for (int i = 0; i < feasiblePopulation.Count; i++)
//            {
//                if (i < elitism) population.Add(feasiblePopulation[i]);

//                else
//                {
//                    DNA<T> parent1 = ChooseParentInFeasible();
//                    DNA<T> parent2 = ChooseParentInFeasible();
//                    DNA<T> child = parent1.Crossover(parent2);
//                    child.Mutate(mutationRate);
//                    population.Add(child);
//                }
//            }

//            //feasibleMeanMoveSum = 0;
//            feasibleFitnessSum = 0;
//            feasiblePopulation.Clear();
//        }

//        if (infeasiblePopulation.Count != 0)
//        {
//            infeasiblePopulation.Sort(CompareDNA);

//            //for (int i = 0; i < infeasiblePopulation.Count; i++) infeasibleBlockCntSum += infeasiblePopulation[i].infeasibleCellCnt;

//            for (int i = 0; i < infeasiblePopulation.Count; i++)
//            {
//                if (i < elitism) population.Add(infeasiblePopulation[i]);

//                else
//                {
//                    DNA<T> parent1 = ChooseParentInInfeasible();
//                    DNA<T> parent2 = ChooseParentInInfeasible();
//                    DNA<T> child = parent1.Crossover(parent2);
//                    child.Mutate(mutationRate);
//                    population.Add(child);
//                }
//            }

//            //infeasibleBlockCntSum = 0;
//            infeasibleFitnessSum = 0;
//            infeasiblePopulation.Clear();
//        }
//        geneticGeneration++;
//    }

//    //public void newGeneration()
//    //{
//    //       population.Clear();

//    //       //feasibleParent = new List<double>();
//    //       //feasibleParentIdx = new List<int>();
//    //       //infeasibleParent = new List<double>();
//    //       //infeasibleParentIdx = new List<int>();

//    //       if (feasiblePopulation.Count != 0)
//    //	{
//    //           feasiblePopulation.Sort(CompareDNA);

//    //           for (int i = 0; i < feasiblePopulation.Count; i++) feasibleMeanMoveSum += feasiblePopulation[i].mean;

//    //           for (int i = 0; i < feasiblePopulation.Count; i++)
//    //           {
//    //               if (i < elitism) population.Add(feasiblePopulation[i]);

//    //               else
//    //               {
//    //                   DNA<T> parent1 = ChooseParentInFeasible();
//    //                   DNA<T> parent2 = ChooseParentInFeasible();
//    //                   DNA<T> child = parent1.Crossover(parent2);
//    //                   child.Mutate(mutationRate);
//    //                   population.Add(child);
//    //               }
//    //           }

//    //           feasibleMeanMoveSum = 0;
//    //           feasibleFitnessSum = 0;
//    //           feasiblePopulation.Clear();
//    //       }

//    //       if (infeasiblePopulation.Count != 0)
//    //       {
//    //           infeasiblePopulation.Sort(CompareDNA);

//    //           for (int i = 0; i < infeasiblePopulation.Count; i++) infeasibleBlockCntSum += infeasiblePopulation[i].infeasibleCellCnt;

//    //           for (int i = 0; i < infeasiblePopulation.Count; i++)
//    //           {
//    //               if (i < elitism) population.Add(infeasiblePopulation[i]);

//    //               else
//    //               {
//    //                   DNA<T> parent1 = ChooseParentInInfeasible();
//    //                   DNA<T> parent2 = ChooseParentInInfeasible();
//    //                   DNA<T> child = parent1.Crossover(parent2);
//    //                   child.Mutate(mutationRate);
//    //                   population.Add(child);
//    //               }
//    //           }

//    //           infeasibleBlockCntSum = 0;
//    //           infeasibleFitnessSum = 0;
//    //           infeasiblePopulation.Clear();
//    //       }
//    //       generation++;
//    //   }


//    //   private int CompareDNAMean(DNA<T> a, DNA<T> b)
//    //{
//    //       if(Math.Abs(targetMove - a.mean ) < Math.Abs(targetMove - b.mean)) return -1;
//    //       else if (Math.Abs(targetMove - a.mean) > Math.Abs(targetMove - b.mean)) return 1;
//    //       else return 0;
//    //   }

//    private int CompareInfeasibleCnt(DNA<T> a, DNA<T> b)
//    {
//        if (a.infeasibleCellCnt < b.infeasibleCellCnt) return -1;
//        else if (a.infeasibleCellCnt > b.infeasibleCellCnt) return 1;
//        else return 0;
//    }

//    private int CompareDNA(DNA<T> a, DNA<T> b)
//    {
//        //if (Math.Abs(a.fitness - b.fitness) <= 1e-9) return 0;

//        //else if (a.fitness < b.fitness) return 1;

//        //else return -1;


//        if (a.fitness > b.fitness) return -1;
//        else if (a.fitness < b.fitness) return 1;
//        else return 0;
//    }

//    private DNA<T> ChooseParentInFeasible()
//    {
//        double randomNumber = random.NextDouble() * feasibleFitnessSum;

//        for (int i = 0; i < feasiblePopulation.Count; i++)
//        {
//            if (randomNumber < feasiblePopulation[i].fitness)
//            {
//                //feasibleParent.Add(feasiblePopulation[i].fitness);
//                //feasibleParentIdx.Add(i);
//                return feasiblePopulation[i];
//            }

//            randomNumber -= feasiblePopulation[i].fitness;
//        }

//        //double randomNumber = random.NextDouble() * feasibleMeanMoveSum;

//        //for (int i = 0; i < feasible_population.Count; i++)
//        //{
//        //    if (randomNumber < feasible_population[i].mean) return feasible_population[i];
//        //    randomNumber -= feasible_population[i].mean;
//        //}


//        return null;
//    }

//    private DNA<T> ChooseParentInInfeasible()
//    {
//        //double randomNumber = random.NextDouble() * infeasibleFitnessSum;

//        //for (int i = 0; i < infeasible_population.Count; i++)
//        //{
//        //    if (randomNumber < infeasible_population[i].fitness) return infeasible_population[i];
//        //    randomNumber -= infeasible_population[i].fitness;
//        //}

//        double randomNumber = random.NextDouble() * infeasibleFitnessSum;

//        for (int i = 0; i < infeasiblePopulation.Count; i++)
//        {
//            if (randomNumber < infeasiblePopulation[i].fitness)
//            {
//                //infeasibleParent.Add(infeasiblePopulation[i].fitness);
//                //infeasibleParentIdx.Add(i);
//                return infeasiblePopulation[i];
//            }

//            randomNumber -= infeasiblePopulation[i].fitness;
//        }
//        return null;
//    }




//}


//DNA.cs-------------------------------------------/

//public class DNA<T>
//{
//    public T[] genes { get; private set; }
//    private System.Random random;
//    private Func<T> getRandomGene;
//    private Func<T[]> getGenes;

//    public bool isFeasible = false;
//    public int infeasibleCellCnt;

//    public int mapMatchPotential;
//    public int includeMatchObstacle;
//    public List<int> mapMatchPotentialList;

//    public int swapCnt;
//    public double swapStd;

//    public int matchCnt;
//    public double matchStd;
//    public double fitness;

//    public int obstacleCnt;

//    public int breakableObstacle;
//    public List<GridCell> breakableObstacleCellList;


//    public int obstaclePottentialCnt;
//    public int blockedPottentialCnt;
//    public int overlayPottentialCnt;
//    public int notObstacleCnt;

//    public List<int> gl;

//    //public double mean;
//    //public double stanardDeviation;


//    //public int curState;
//    //public bool targetClear = false;
//    //public bool maxedOut = false;

//    //public int obstructionRate = 0;
//    //public int shortCutCnt = 0;

//    //public int allMove = 0;

//    //
//    //public int obstacleLimit = 5;

//    //public T[] newGenes;


//    public int matchFromMap;
//    public int nearBreakableObstacles;
//    public int includeMatchObstacles;

//    public DNA(int size, Random random, Func<T> getRandomGene, Func<T[]> getGenes, bool shouldInitGenes = true)
//    {
//        genes = new T[size];
//        this.random = random;
//        this.getGenes = getGenes;
//        this.getRandomGene = getRandomGene;

//        genes = getGenes();

//        //if(shouldInitGenes) for (int i = 0; i < size; i++) genes[i] = getRandomGene();




//        isFeasible = false;
//        infeasibleCellCnt = 0;

//        includeMatchObstacle = 0;
//        mapMatchPotential = 0;
//        mapMatchPotentialList = new List<int>();

//        swapCnt = 0;
//        matchCnt = 0;
//        fitness = 0;

//        obstacleCnt = 0;

//        //mapMatchPotential = 0;
//        //mapMatchPotentialList = new List<int>();

//        breakableObstacle = 0;
//        breakableObstacleCellList = new List<GridCell>();

//        obstaclePottentialCnt = 0;
//        blockedPottentialCnt = 0;
//        overlayPottentialCnt = 0;
//        notObstacleCnt = 0;
//        gl = new List<int>();




//        matchFromMap = 0;
//        nearBreakableObstacles = 0;
//        includeMatchObstacles = 0;



//    }

//    public void calculateFeasibleFitness(int wantDifficulty, int difficultyTolerance)
//    {
//        fitness = Math.Abs(mapMatchPotential - wantDifficulty);

//        if (fitness > difficultyTolerance) fitness = 1.0 / (1.0 + Math.Abs(fitness));

//        else fitness = 1;

//    }


//    public void calculateFeasibleFitness()
//    {
//        fitness = 26.0202 + ((-0.0059) * mapMatchPotential);

//        if (fitness - 18 <= 1)
//        {
//            //initGrid(p, sC);
//            //m3h.isEnd = true;
//        }

//        fitness = 1.0 / (1.0 + Math.Abs(fitness - 18));
//    }

//    //public void calculateFeasibleFitness(List<int> moveContainer, double targetMove, double targetStd)
//    //{



//    //    ////double max = 0;
//    //    ////int maxIdx = 0;
//    //    ////double min = 120;
//    //    ////int minIdx = 0;


//    //    ////double sum = 0;
//    //    ////for (int i = 0; i < moveContainer.Count; i++)
//    //    ////{
//    //    ////    sum += moveContainer[i];

//    //    ////    if(max < moveContainer[i])
//    //    ////    {
//    //    ////        maxIdx = i;
//    //    ////        max = moveContainer[i];
//    //    ////    }

//    //    ////    if(min > moveContainer[i])
//    //    ////    {
//    //    ////        minIdx = i;
//    //    ////        min = moveContainer[i];
//    //    ////    }
//    //    ////}

//    //    ////sum -= max;
//    //    ////sum -= min;

//    //    ////mean = sum / (moveContainer.Count - 2);

//    //    ////double squaredDifferencesSum = 0.0;
//    //    ////for (int i = 0; i < moveContainer.Count; i++)
//    //    ////{
//    //    ////    if (i == minIdx || i == maxIdx) continue;

//    //    ////    double diff = moveContainer[i] - mean;
//    //    ////    squaredDifferencesSum += diff * diff;
//    //    ////}
//    //    ////double variance = squaredDifferencesSum / (moveContainer.Count - 2);
//    //    ////stanardDeviation = Math.Sqrt(variance);

//    //    ////double normalMean = 1.0 / (1.0 + Math.Abs(mean - targetMove));
//    //    ////double normalStd = 1.0 / (1.0 + Math.Abs(stanardDeviation - targetStd));

//    //    ////double alpha = 0.5;
//    //    ////fitness = alpha * normalMean + (1 - alpha) * normalStd;






//    //    //double sum = 0;
//    //    //for (int i = 0; i < moveContainer.Count; i++) sum += moveContainer[i];
//    //    //mean = sum / moveContainer.Count;

//    //    //double squaredDifferencesSum = 0.0;
//    //    //for (int i = 0; i < moveContainer.Count; i++)
//    //    //{
//    //    //    double diff = moveContainer[i] - mean;
//    //    //    squaredDifferencesSum += diff * diff;
//    //    //}
//    //    //double variance = squaredDifferencesSum / moveContainer.Count;
//    //    //stanardDeviation = Math.Sqrt(variance);

//    //    //double normalMean = 1.0 / (1.0 + Math.Abs(mean - targetMove));
//    //    //double normalStd = 1.0 / (1.0 + Math.Abs(stanardDeviation - targetStd));

//    //    //double alpha = 0.5;
//    //    //fitness = alpha * normalMean + (1 - alpha) * normalStd;







//    //    ////double sum = 0;
//    //    ////for (int i = 0; i < moveContainer.Count; i++) sum += moveContainer[i];
//    //    ////mean = sum / moveContainer.Count;

//    //    ////double squaredDifferencesSum = 0.0;
//    //    ////for (int i = 0; i < moveContainer.Count; i++)
//    //    ////{
//    //    ////    double diff = moveContainer[i] - mean;
//    //    ////    squaredDifferencesSum += diff * diff;
//    //    ////}
//    //    ////double variance = squaredDifferencesSum / moveContainer.Count;
//    //    ////stanardDeviation = Math.Sqrt(variance);

//    //    ////// Z-Score Normalization 적용하여 평균값 계산
//    //    ////double sumNormalizedData = 0;
//    //    ////foreach (int moveCount in moveContainer)
//    //    ////{
//    //    ////    double normalizedValue = (moveCount - mean) / stanardDeviation;
//    //    ////    sumNormalizedData += normalizedValue;
//    //    ////}

//    //    ////// 파퓰레이션의 최종 피트니스 값을 평균으로 계산
//    //    ////fitness = sumNormalizedData / moveContainer.Count;








//    //    ////double sum = 0;
//    //    ////for (int i = 0; i < moveContainer.Count; i++) sum += moveContainer[i];
//    //    ////mean = sum / moveContainer.Count;

//    //    ////double squaredDifferencesSum = 0.0;
//    //    ////for (int i = 0; i < moveContainer.Count; i++)
//    //    ////{
//    //    ////    double diff = moveContainer[i] - mean;
//    //    ////    squaredDifferencesSum += diff * diff;
//    //    ////}
//    //    ////double variance = squaredDifferencesSum / moveContainer.Count;
//    //    ////stanardDeviation = Math.Sqrt(variance);

//    //    ////double normalMean = Math.Abs(mean - targetMove);

//    //    ////int maxValue = 100;
//    //    ////int minValue = 1;

//    //    ////normalMean = (double)(Math.Abs(mean - targetMove) - minValue) / (maxValue - minValue);

//    //    ////normalMean = Math.Max(0, Math.Min(1, normalMean));


//    //    ////fitness = normalMean;






//    //    ////double normalStd = Math.Exp(-Math.Abs(stanardDeviation - targetStd));

//    //    ////double alpha = 0.5;
//    //    ////fitness = alpha * expMean + (1 - alpha) * expStd;




//    //    ////double sum = 0;
//    //    ////for (int i = 0; i < moveContainer.Count; i++) sum += moveContainer[i];
//    //    ////mean = sum / moveContainer.Count;

//    //    ////double squaredDifferencesSum = 0.0;
//    //    ////for (int i = 0; i < moveContainer.Count; i++)
//    //    ////{
//    //    ////    double diff = moveContainer[i] - mean;
//    //    ////    squaredDifferencesSum += diff * diff;
//    //    ////}
//    //    ////double variance = squaredDifferencesSum / moveContainer.Count;
//    //    ////stanardDeviation = Math.Sqrt(variance);

//    //    ////double expMean = Math.Exp(-Math.Abs(mean - targetMove));
//    //    ////double expStd = Math.Exp(-Math.Abs(stanardDeviation - targetStd));

//    //    ////double alpha = 0.5;
//    //    ////fitness = alpha * expMean + (1 - alpha) * expStd;





















//    //    ////fitness = alpha * Math.Exp(-Math.Abs(mean - targetMove))
//    //    ////        + (1 - alpha) * Math.Exp(-Math.Abs(stanardDeviation - targetStd));





//    //    ////std = 1.0 / (1.0 + Math.Abs(standardDeviation - target_std));
//    //    ////double alpha = 0.5;
//    //    ////fitness = alpha * (1.0 / (1.0 + Math.Abs(mean - targetMove))) + (1- alpha) * (1.0 / (1.0 + Math.Abs(standardDeviation - targetStd)));









//    //    ////double expMean;

//    //    ////if (Math.Abs(mean - target_move) < 9)
//    //    ////{
//    //    ////    string result = string.Format("{0:0.########0}", Math.Exp(-Math.Abs(mean - target_move)));
//    //    ////    expMean = (Double.Parse(result));
//    //    ////}

//    //    ////fitness = 1.0 / (1.0 + Math.Abs(mean - target_move));

//    //    //////else expMean = 0;

//    //    ////double squaredDifferencesSum = 0.0;
//    //    ////for (int i = 0; i < num_move_container.Count; i++)
//    //    ////{
//    //    ////    double diff = num_move_container[i] - mean;
//    //    ////    squaredDifferencesSum += diff * diff;
//    //    ////}

//    //    ////double variance = squaredDifferencesSum / num_move_container.Count;
//    //    ////double standardDeviation = Math.Sqrt(variance);

//    //    ////double expStd;

//    //    ////if (standardDeviation < 9)
//    //    ////{
//    //    ////    string result = string.Format("{0:0.########0}", Math.Exp(-Math.Abs(standardDeviation)));
//    //    ////    expStd = (Double.Parse(result));
//    //    ////}

//    //    ////else expStd = 0;

//    //    ////std = standardDeviation;


//    //    ////double alpha = 1;

//    //    ////fitness = alpha * expMean /*+ (1- alpha) * expStd*/;


//    //}

//    public void calculateInfeasibleFitness()
//    {
//        fitness = 1.0 / (1.0 + Math.Abs(infeasibleCellCnt));

//        //fitness = Math.Exp(-Math.Abs(infeasibleCellCnt));

//        //if(infeasibleCellCnt < 9)
//        //{
//        //    fitness = Math.Exp(-Math.Abs(infeasibleCellCnt));
//        //}

//        //else fitness = 0;
//    }


//    public DNA<T> Crossover(DNA<T> otherParent)
//    {
//        DNA<T> child = new DNA<T>(genes.Length, random, getRandomGene, getGenes, shouldInitGenes: false);

//        for (int i = 0; i < genes.Length; i++)
//        {
//            child.genes[i] = random.NextDouble() < 0.5 ? genes[i] : otherParent.genes[i];
//        }

//        return child;
//    }

//    public void Mutate(float mutationRate)
//    {
//        for (int i = 0; i < genes.Length; i++)
//        {
//            if (random.NextDouble() < mutationRate) genes[i] = getRandomGene();
//        }
//    }

//    //public DNA<T> newCrossover(DNA<T> otherParent)
//    //{
//    //    DNA<T> child = new DNA<T>(genes.Length, random, getRandomGene, getGenes, shouldInitGenes: false);

//    //    for (int i = 0; i < genes.Length; i++)
//    //    {
//    //        child.genes[i] = random.NextDouble() < 0.5 ? newGenes[i] : otherParent.newGenes[i];
//    //    }

//    //    return child;
//    //}



//    public DNA<T> newInit()
//    {
//        DNA<T> child = new DNA<T>(genes.Length, random, getRandomGene, getGenes, shouldInitGenes: false);
//        return child;
//    }
//}


//CSVFielWriter.cs---------------------------------------/

//public class CSVFileWriter : MonoBehaviour
//{
//    //public List<object> mixedList;

//    //public List<int> generation;
//    //public List<int> infeasiblePopulationCnt;
//    //public List<int> feasiblePopulationCnt;

//    //public List<double> curGenerationBestMean;
//    //public List<double> curGenerationBestStd;
//    //public List<double> curGenerationBestFitness;

//    //public List<double> bestMeanMove;
//    //public List<double> bestStd;
//    //public List<double> bestFitness;


//    //public List<List<double>> feasibleParent;
//    //public List<List<int>> feasibleParentIdx;
//    //public List<List<double>> infeasibleParent;
//    //public List<List<int>> infeasibleParentIdx;
//    //public List<List<int>> curBestMoves;
//    //public List<List<int>> bestMoves;
//    //public List<List<int>> repeatMovements;
//    //public List<int> repeatMovementsCntContainer;

//    public List<string[]> data;

//    public List<int> targetNeedCnt;

//    public List<int> infeasiblePopulationCnt;
//    public List<int> feasiblePopulationCnt;

//    public List<int> curGenerationBestMean;
//    public List<int> curGenerationBestStd;
//    public List<int> curGenerationBestFitness;

//    public List<int> bestMeanMove;
//    public List<int> bestStd;
//    public List<int> bestFitness;

//    public List<List<int>> bestSwapContainer;
//    public List<List<int>> swapContainer;
//    public List<List<int>> matchContainer;

//    //public List<int> curBestMoves;
//    //public List<int> bestMoves;

//    public CSVFileWriter(Match3Helper m3h)
//    {
//        data = new List<string[]>();
//        targetNeedCnt = new List<int>();
//        foreach (var item in m3h.curTargets)
//        {
//            targetNeedCnt.Add(item.Value.ID);
//            targetNeedCnt.Add(item.Value.NeedCount);
//        }

//        infeasiblePopulationCnt = new List<int>();
//        feasiblePopulationCnt = new List<int>();

//        curGenerationBestMean = new List<int>();
//        curGenerationBestStd = new List<int>();
//        curGenerationBestFitness = new List<int>();

//        bestMeanMove = new List<int>();
//        bestStd = new List<int>();
//        bestFitness = new List<int>();

//        bestSwapContainer = new List<List<int>>();
//        swapContainer = new List<List<int>>();
//        matchContainer = new List<List<int>>();




//        //generation = new List<int>();
//        //infeasiblePopulationCnt = new List<int>();
//        //feasiblePopulationCnt = new List<int>();

//        //curGenerationBestMean = new List<double>();
//        //curGenerationBestStd = new List<double>();
//        //curGenerationBestFitness = new List<double>();

//        //bestMeanMove = new List<double>();
//        //bestStd = new List<double>();
//        //bestFitness = new List<double>();

//        //feasibleParent = new List<List<double>>();
//        //feasibleParentIdx = new List<List<int>>();
//        //infeasibleParent = new List<List<double>>();
//        //infeasibleParentIdx = new List<List<int>>();

//        //curBestMoves = new List<List<int>>();
//        //bestMoves = new List<List<int>>();
//        //repeatMovementsCntContainer = new List<int>();
//    }

//    public void write(GeneticAlgorithm<char> ga, Match3Helper m3h)
//    {
//        string[] tempData = new string[1000];
//        string[] gaDatas = new string[1000];

//        gaDatas[0] = "Limit";
//        gaDatas[1] = "populationSize";
//        gaDatas[2] = ga.populationSize.ToString();
//        gaDatas[3] = "elitism";
//        gaDatas[4] = ga.elitism.ToString();
//        gaDatas[5] = "mutationRate";
//        gaDatas[6] = ga.mutationRate.ToString();

//        gaDatas[7] = "generationLimit";
//        gaDatas[8] = m3h.limits.generation.ToString();
//        gaDatas[9] = "moveLimit";
//        gaDatas[10] = m3h.limits.move.ToString();
//        gaDatas[11] = "repeat";
//        gaDatas[12] = m3h.limits.repeat.ToString();

//        gaDatas[13] = "INPUT";
//        gaDatas[14] = "gridRowSize";
//        gaDatas[15] = m3h.rowSize.ToString();
//        gaDatas[16] = "gridColSize";
//        gaDatas[17] = m3h.colSize.ToString();

//        gaDatas[18] = "targetMove";
//        gaDatas[19] = ga.targetMove.ToString();
//        gaDatas[20] = "targetStd";
//        gaDatas[21] = ga.targetStd.ToString();
//        gaDatas[22] = "targetFitness";
//        gaDatas[23] = ga.targetFitness.ToString();
//        gaDatas[24] = "targetCnt";

//        int csvIdx = 25;
//        //일단 킵
//        for (int i = 0; i < targetNeedCnt.Count; i++)
//        {
//            gaDatas[csvIdx] = targetNeedCnt[i].ToString();
//            csvIdx++;
//        }


//        gaDatas[csvIdx] = "obstacleCnt"; csvIdx++;
//        gaDatas[csvIdx] = ga.population[0].obstacleCnt.ToString(); csvIdx++;

//        gaDatas[csvIdx] = "possibleCnt"; csvIdx++;
//        gaDatas[csvIdx] = ga.population[0].mapMatchPotential.ToString(); csvIdx++;

//        gaDatas[csvIdx] = "notObstacleCnt"; csvIdx++;
//        gaDatas[csvIdx] = ga.population[0].notObstacleCnt.ToString(); csvIdx++;

//        gaDatas[csvIdx] = "obstaclePottentialCnt"; csvIdx++;
//        gaDatas[csvIdx] = ga.population[0].obstaclePottentialCnt.ToString(); csvIdx++;

//        gaDatas[csvIdx] = "blockedObjectHitCnt"; csvIdx++;
//        gaDatas[csvIdx] = m3h.blockedObjectHitCnt.ToString(); csvIdx++;
//        //gaDatas[csvIdx] = "blockedObjectHitCnt1"; csvIdx++;
//        //gaDatas[csvIdx] = m3h.blockedObjectHitCnt.ToString(); csvIdx++;
//        //gaDatas[csvIdx] = "blockedObjectHitCnt2"; csvIdx++;
//        //gaDatas[csvIdx] = m3h.blockedObjectHitCnt.ToString(); csvIdx++;
//        gaDatas[csvIdx] = "blockedPottentialCnt"; csvIdx++;
//        gaDatas[csvIdx] = ga.population[0].blockedPottentialCnt.ToString(); csvIdx++;
//        gaDatas[csvIdx] = "breakableObstacle"; csvIdx++;
//        gaDatas[csvIdx] = ga.population[0].breakableObstacle.ToString(); csvIdx++;


//        gaDatas[csvIdx] = "overlayObjectHitCnt"; csvIdx++;
//        gaDatas[csvIdx] = m3h.overlayObjectHitCnt.ToString(); csvIdx++;
//        //gaDatas[csvIdx] = "overlayObjectHitCnt1"; csvIdx++;
//        //gaDatas[csvIdx] = m3h.overlayObjectHitCnt.ToString(); csvIdx++;
//        //gaDatas[csvIdx] = "overlayObjectHitCnt2"; csvIdx++;
//        //gaDatas[csvIdx] = m3h.overlayObjectHitCnt.ToString(); csvIdx++;
//        gaDatas[csvIdx] = "overlayPottentialCnt"; csvIdx++;
//        gaDatas[csvIdx] = ga.population[0].overlayPottentialCnt.ToString(); csvIdx++;
//        gaDatas[csvIdx] = "includeMatchObstacle"; csvIdx++;
//        gaDatas[csvIdx] = ga.population[0].includeMatchObstacle.ToString(); csvIdx++;

//        gaDatas[csvIdx] = "deduplicationMapMatchPottential"; csvIdx++;
//        gaDatas[csvIdx] = ga.population[0].matchFromMap.ToString(); csvIdx++;
//        gaDatas[csvIdx] = "deduplicationNearBreakableObstacles"; csvIdx++;
//        gaDatas[csvIdx] = ga.population[0].nearBreakableObstacles.ToString(); csvIdx++;
//        gaDatas[csvIdx] = "deduplicationIncludeMatchObstacles"; csvIdx++;
//        gaDatas[csvIdx] = ga.population[0].includeMatchObstacles.ToString(); csvIdx++;


//        //gaDatas[csvIdx] = "breakableObstacle";
//        //csvIdx++;
//        //gaDatas[csvIdx] = ga.population[0].breakableObstacle.ToString();
//        //csvIdx++;

//        gaDatas[csvIdx] = "possibleCounting"; csvIdx++;
//        for (int i = 0; i < ga.population[0].mapMatchPotentialList.Count; i++)
//        {
//            gaDatas[csvIdx] = ga.population[0].mapMatchPotentialList[i].ToString();
//            csvIdx++;
//        }

//        string s = "'";

//        for (int i = 0; i < m3h.gridSize; i++) s += ga.population[0].genes[i];

//        gaDatas[csvIdx] = s;
//        csvIdx++;



//        int idx = 0;
//        for (int i = 0; i < m3h.limits.generation; i++)
//        {
//            tempData = new string[1000];

//            if (i < gaDatas.Length) tempData[0] = gaDatas[i];

//            if (i < m3h.limits.generation)
//            {
//                tempData[1] = (i + 1).ToString();
//                //tempData[2] = infeasiblePopulationCnt[i].ToString();
//                //tempData[3] = feasiblePopulationCnt[i].ToString();

//                //tempData[4] = curGenerationBestMean[i].ToString();
//                //tempData[5] = curGenerationBestStd[i].ToString();
//                //tempData[6] = curGenerationBestFitness[i].ToString();

//                //tempData[7] = bestMeanMove[i].ToString();
//                //tempData[8] = bestStd[i].ToString();
//                //tempData[9] = bestFitness[i].ToString();

//                tempData[2] = 0.ToString();
//                tempData[3] = 0.ToString();
//                tempData[4] = 0.ToString();
//                tempData[5] = 0.ToString();
//                tempData[6] = 0.ToString();
//                tempData[7] = 0.ToString();
//                tempData[8] = 0.ToString();
//                tempData[9] = 0.ToString();

//                for (int j = 0; j < bestSwapContainer[i].Count; j++)
//                {
//                    tempData[10] += bestSwapContainer[i][j].ToString();
//                    tempData[10] += ",";
//                }

//                for (int j = 0; j < swapContainer[idx].Count; j++)
//                {
//                    tempData[11] += swapContainer[idx][j].ToString();
//                    tempData[11] += ",";
//                    tempData[12] += matchContainer[idx][j].ToString();
//                    tempData[12] += ",";
//                }
//                idx++;
//            }

//            data.Add(tempData);
//        }

//        //tempData = new string[60];

//        //if (idx < ga.repeatMovements.Count)
//        //{
//        //    for (int j = 0; j < ga.repeatMovements[idx].Count; j++)
//        //    {
//        //        tempData[10 + ga.repeat + 1] += ga.repeatMovements[idx][j].ToString();
//        //        tempData[10 + ga.repeat + 1] += ",";
//        //        tempData[11 + ga.repeat + 1] += ga.allMovements[idx][j].ToString();
//        //        tempData[11 + ga.repeat + 1] += ",";
//        //    }
//        //    idx++;
//        //    data.Add(tempData);
//        //}

//        bool isFirst = true;
//        while (idx < swapContainer.Count)
//        {
//            if (!isFirst)
//            {
//                if (idx < gaDatas.Length) tempData[0] = gaDatas[idx];
//            }

//            tempData = new string[1000];

//            for (int j = 0; j < swapContainer[idx].Count; j++)
//            {
//                tempData[10 + ga.repeat + 1] += swapContainer[idx][j].ToString();
//                tempData[10 + ga.repeat + 1] += ",";
//                tempData[11 + ga.repeat + 1] += matchContainer[idx][j].ToString();
//                tempData[11 + ga.repeat + 1] += ",";
//            }
//            idx++;
//            data.Add(tempData);

//            isFirst = false;
//        }

//        if (data.Count < gaDatas.Length)
//        {
//            while (data.Count < gaDatas.Length)
//            {
//                tempData = new string[1000];
//                tempData[0] = gaDatas[idx];
//                idx++;
//                data.Add(tempData);
//            }
//        }

//        string[][] output = new string[data.Count][];

//        for (int i = 0; i < output.Length; i++) output[i] = data[i];

//        int length = output.GetLength(0);
//        string delimiter = ",";

//        StringBuilder sb = new StringBuilder();

//        for (int index = 0; index < length; index++) sb.AppendLine(string.Join(delimiter, output[index]));

//        string sPath = "/CSV/";
//        sPath += m3h.csvCnt.ToString();
//        sPath += ".csv";
//        m3h.csvCnt++;
//        String filePath = Application.dataPath + sPath;

//        StreamWriter outStream = System.IO.File.CreateText(filePath);

//        outStream.WriteLine(sb);
//        outStream.Close();
//    }









//    //public class CSVFileWriter : MonoBehaviour
//    //{
//    //    public List<object> mixedList;

//    //    public List<int> generation;
//    //    public List<int> infeasiblePopulationCnt;
//    //    public List<int> feasiblePopulationCnt;
//    //    public List<double> curGenerationBestMean;
//    //    public List<double> curGenerationBestStd;
//    //    public List<double> curGenerationBestFitness;
//    //    public List<double> bestMeanMove;
//    //    public List<double> bestStd;
//    //    public List<double> bestFitness;
//    //    public List<List<double>> feasibleParent;
//    //    public List<List<int>> feasibleParentIdx;
//    //    public List<List<double>> infeasibleParent;
//    //    public List<List<int>> infeasibleParentIdx;
//    //    public List<List<int>> curBestMoves;
//    //    public List<List<int>> bestMoves;
//    //    public List<List<int>> repeatMovements;
//    //    public List<int> repeatMovementsCntContainer;

//    //    public List<string[]> data = new List<string[]>();

//    //    public CSVFileWriter()
//    //    {
//    //        generation = new List<int>();
//    //        infeasiblePopulationCnt = new List<int>();
//    //        feasiblePopulationCnt = new List<int>();

//    //        curGenerationBestMean = new List<double>();
//    //        curGenerationBestStd = new List<double>();
//    //        curGenerationBestFitness = new List<double>();

//    //        bestMeanMove = new List<double>();
//    //        bestStd = new List<double>();
//    //        bestFitness = new List<double>();

//    //        feasibleParent = new List<List<double>>();
//    //        feasibleParentIdx = new List<List<int>>();
//    //        infeasibleParent = new List<List<double>>();
//    //        infeasibleParentIdx = new List<List<int>>();

//    //        curBestMoves = new List<List<int>>();
//    //        bestMoves = new List<List<int>>();
//    //        repeatMovementsCntContainer = new List<int>();
//    //    }

//    //    public void write(GeneticAlgorithm<char> ga, List<GridCell> Cells,int feasibleIdx,int csvCnt)
//    //    {
//    //        string[] tempData = new string[200];

//    //        //tempData[0] = string.Empty;
//    //        //tempData[1] = "generation";
//    //        //tempData[2] = "infeasiblePopulationCnt";
//    //        //tempData[3] = "feasiblePopulationCnt";
//    //        //tempData[4] = "curGenerationBestMean";
//    //        //tempData[5] = "bestMeanMove";
//    //        //tempData[6] = "curGenerationBestStd";
//    //        //tempData[7] = "bestStd";
//    //        //tempData[8] = "curGenerationBestFitness";
//    //        //tempData[9] = "bestFitness";
//    //        //tempData[10]= "feasibleParent";
//    //        //tempData[11] = "feasibleParentIdx";
//    //        //tempData[12] = "infeasibleParent";
//    //        //tempData[13] = "infeasibleParentIdx";
//    //        //tempData[14] = "curBestMoves";
//    //        //tempData[15] = "bestMoves";
//    //        //data.Add(tempData);

//    //        string[] gaDatas = new string[200];

//    //        gaDatas[0] = "Limit";
//    //        gaDatas[1] = "populationSize";
//    //        gaDatas[2] = ga.populationSize.ToString();
//    //        gaDatas[3] = "elitism";
//    //        gaDatas[4] = ga.elitism.ToString();
//    //        gaDatas[5] = "mutationRate";
//    //        gaDatas[6] = ga.mutationRate.ToString();
//    //        gaDatas[7] = "generationLimit";
//    //        gaDatas[8] = ga.generationLimit.ToString();
//    //        gaDatas[9] = "moveLimit";
//    //        gaDatas[10] = ga.moveLimit.ToString();
//    //        gaDatas[11] = "repeat";
//    //        gaDatas[12] = ga.repeat.ToString();
//    //        gaDatas[13] = "INPUT";
//    //        gaDatas[14] = "gridRowSize";
//    //        gaDatas[15] = Cells[0].GRow.Length.ToString();
//    //        gaDatas[16] = "gridColSize";
//    //        gaDatas[17] = Cells[0].GColumn.Length.ToString();
//    //        gaDatas[18] = "targetMove";
//    //        gaDatas[19] = ga.targetMove.ToString();
//    //        gaDatas[20] = "targetStd";
//    //        gaDatas[21] = ga.targetStd.ToString();
//    //        gaDatas[22] = "targetFitness";
//    //        gaDatas[23] = ga.targetFitness.ToString();


//    //        gaDatas[24] = "targetCnt";

//    //        int index2 = 25;

//    //        for(int i= 0;i < ga.targetNeedCnt.Count; i++)
//    //        {
//    //            gaDatas[index2] = ga.targetNeedCnt[i].ToString();
//    //            index2++;
//    //        }
//    //        //gaDatas[25] = ga.targetNeedCnt[0].ToString();
//    //        //gaDatas[26] = ga.targetNeedCnt[1].ToString();
//    //        //gaDatas[27] = ga.targetNeedCnt[2].ToString();
//    //        //gaDatas[28] = ga.targetNeedCnt[3].ToString();
//    //        //gaDatas[29] = ga.targetNeedCnt[4].ToString();
//    //        //gaDatas[30] = ga.targetNeedCnt[5].ToString();
//    //        //gaDatas[31] = ga.targetNeedCnt[6].ToString();


//    //        gaDatas[32] = "possibleCnt";
//    //        gaDatas[33] = ga.possibleCnt.ToString();



//    //        gaDatas[34] = "blockedCnt";
//    //        gaDatas[35] = ga.blockeCnt.ToString();


//    //        gaDatas[36] = "possibleCounting";

//    //        //index2 = 35;

//    //        //for (int i = 0; i < 17; i++)
//    //        //{
//    //        //    gaDatas[index2] = ga.possibleCounting[i].ToString();
//    //        //    index2++;
//    //        //}

//    //        index2 = 37;

//    //        for (int i = 0; i < ga.possibleCountingList.Count; i++)
//    //        {
//    //            gaDatas[index2] = ga.possibleCountingList[i].ToString();
//    //            index2++;
//    //        }


//    //        string s = "'";

//    //        for(int i = 0;i< Cells.Count;i++)
//    //        {
//    //           s += ga.population[feasibleIdx].genes[i];
//    //        }

//    //        gaDatas[index2] = s;


//    //        int idx = 0;
//    //        //int excelLength = Math.Max(generation.Count, gaDatas.Length);



//    //        for (int i = 0; i < generation.Count; i++)
//    //        {
//    //            //tempData = new string[mixedList.Count + 1];
//    //            tempData = new string[13];

//    //            if (i < gaDatas.Length) tempData[0] = gaDatas[i];

//    //            if (i < generation.Count)
//    //            {
//    //                tempData[1] = generation[i].ToString();
//    //                tempData[2] = infeasiblePopulationCnt[i].ToString();
//    //                tempData[3] = feasiblePopulationCnt[i].ToString();

//    //                tempData[4] = curGenerationBestMean[i].ToString();
//    //                tempData[5] = curGenerationBestStd[i].ToString();
//    //                tempData[6] = curGenerationBestFitness[i].ToString();

//    //                tempData[7] = bestMeanMove[i].ToString();
//    //                tempData[8] = bestStd[i].ToString();
//    //                tempData[9] = bestFitness[i].ToString();

//    //                for (int j = 0; j < bestMoves[i].Count; j++)
//    //                {
//    //                    tempData[10] += bestMoves[i][j].ToString();
//    //                    tempData[10] += ",";
//    //                }

//    //                //if(ga.repeat < 20)
//    //                //{
//    //                //    int iidx = 0;
//    //                //    while(ga.repeat + iidx < 20)
//    //                //    {
//    //                //        tempData[10] += ",";
//    //                //        iidx++;
//    //                //    }

//    //                //}

//    //                for (int j = 0; j < ga.repeatMovements[idx].Count; j++)
//    //                {
//    //                    tempData[11] += ga.repeatMovements[idx][j].ToString();
//    //                    tempData[11] += ",";
//    //                    //tempData[12] += ga.shorCutRates[idx][j].ToString();
//    //                    //tempData[12] += ",";
//    //                    tempData[12] += ga.allMovements[idx][j].ToString();
//    //                    tempData[12] += ",";


//    //                }

//    //                idx++;

//    //                //tempData[12] += 1.ToString();
//    //                //tempData[12] += ",";
//    //            }

//    //            data.Add(tempData);
//    //        }

//    //        tempData = new string[60];
//    //        //data.Add(tempData);

//    //        if(idx < ga.repeatMovements.Count)
//    //        {
//    //            for (int j = 0; j < ga.repeatMovements[idx].Count; j++)
//    //            {
//    //                tempData[10 + ga.repeat + 1] += ga.repeatMovements[idx][j].ToString();
//    //                tempData[10 + ga.repeat + 1] += ",";
//    //            }

//    //            for (int j = 0; j < ga.repeatMovements[idx].Count; j++)
//    //            {
//    //                //tempData[11 + ga.repeat + 1] += ga.shorCutRates[idx][j].ToString();
//    //                //tempData[11 + ga.repeat + 1] += ",";

//    //                tempData[11 + ga.repeat + 1] += ga.allMovements[idx][j].ToString();
//    //                tempData[11 + ga.repeat + 1] += ",";
//    //            }



//    //            idx++;

//    //            data.Add(tempData);
//    //        }

//    //        while (idx < ga.repeatMovements.Count)
//    //        {
//    //            if (idx < gaDatas.Length) tempData[0] = gaDatas[idx];

//    //            tempData = new string[60];

//    //            for (int j = 0; j < ga.repeatMovements[idx].Count; j++)
//    //            {
//    //                tempData[10 + ga.repeat + 1] += ga.repeatMovements[idx][j].ToString();
//    //                tempData[10 + ga.repeat + 1] += ",";

//    //                //tempData[11 + ga.repeat + 1] += ga.shorCutRates[idx][j].ToString();
//    //                //tempData[11 + ga.repeat + 1] += ",";

//    //                tempData[11 + ga.repeat + 1] += ga.allMovements[idx][j].ToString();
//    //                tempData[11 + ga.repeat + 1] += ",";
//    //            }
//    //            idx++;

//    //            data.Add(tempData);
//    //        }

//    //        if (data.Count < gaDatas.Length)
//    //        {
//    //            while(data.Count < gaDatas.Length)
//    //            {
//    //                tempData = new string[13];
//    //                tempData[0] = gaDatas[idx];
//    //                idx++;
//    //                data.Add(tempData);
//    //            }
//    //        }



//    //        string[][] output = new string[data.Count][];

//    //        for (int i = 0; i < output.Length; i++) output[i] = data[i];

//    //        int length = output.GetLength(0);
//    //        string delimiter = ",";

//    //        StringBuilder sb = new StringBuilder();

//    //        for (int index = 0; index < length; index++) sb.AppendLine(string.Join(delimiter, output[index]));

//    //        //String filePath = Application.dataPath + "/CSV/geneticAlgorithm.csv";


//    //        string sPath = "/CSV/";
//    //        sPath += csvCnt.ToString();
//    //        sPath += ".csv";
//    //        String filePath = Application.dataPath + sPath;

//    //        StreamWriter outStream = System.IO.File.CreateText(filePath);

//    //        outStream.WriteLine(sb);
//    //        outStream.Close();
//    //    }








































//    //public void write(GeneticAlgorithm<char> ga, List<GridCell> Cells)
//    //{
//    //    string[] tempData = new string[mixedList.Count + 1];

//    //    //tempData[0] = string.Empty;
//    //    //tempData[1] = "generation";
//    //    //tempData[2] = "infeasiblePopulationCnt";
//    //    //tempData[3] = "feasiblePopulationCnt";
//    //    //tempData[4] = "curGenerationBestMean";
//    //    //tempData[5] = "bestMeanMove";
//    //    //tempData[6] = "curGenerationBestStd";
//    //    //tempData[7] = "bestStd";
//    //    //tempData[8] = "curGenerationBestFitness";
//    //    //tempData[9] = "bestFitness";
//    //    //tempData[10]= "feasibleParent";
//    //    //tempData[11] = "feasibleParentIdx";
//    //    //tempData[12] = "infeasibleParent";
//    //    //tempData[13] = "infeasibleParentIdx";
//    //    //tempData[14] = "curBestMoves";
//    //    //tempData[15] = "bestMoves";
//    //    //data.Add(tempData);

//    //    string[] gaDatas = new string[29];

//    //    gaDatas[0] = "Limit";
//    //    gaDatas[1] = "populationSize";
//    //    gaDatas[2] = ga.populationSize.ToString();
//    //    gaDatas[3] = "elitism";
//    //    gaDatas[4] = ga.elitism.ToString();
//    //    gaDatas[5] = "mutationRate";
//    //    gaDatas[6] = ga.mutationRate.ToString();
//    //    gaDatas[7] = "generationLimit";
//    //    gaDatas[8] = ga.generationLimit.ToString();
//    //    gaDatas[9] = "moveLimit";
//    //    gaDatas[10] = ga.moveLimit.ToString();
//    //    gaDatas[11] = "repeat";
//    //    gaDatas[12] = ga.repeat.ToString();
//    //    gaDatas[13] = "INPUT";
//    //    gaDatas[14] = "gridRowSize";
//    //    gaDatas[15] = Cells[0].GRow.Length.ToString();
//    //    gaDatas[16] = "gridColSize";
//    //    gaDatas[17] = Cells[0].GColumn.Length.ToString();
//    //    gaDatas[18] = "targetMove";
//    //    gaDatas[19] = ga.targetMove.ToString();
//    //    gaDatas[20] = "targetStd";
//    //    gaDatas[21] = ga.targetStd.ToString();
//    //    gaDatas[22] = "targetFitness";
//    //    gaDatas[23] = ga.targetFitness.ToString();
//    //    gaDatas[24] = "targetBlocks";
//    //    gaDatas[25] = "name";
//    //    gaDatas[26] = "Apple";
//    //    gaDatas[27] = "needCnt";
//    //    gaDatas[28] = 15.ToString();

//    //    int check = 0;
//    //    int idx = 0;
//    //    int idx2 = 0;

//    //    for (int i = 0; i < generation.Count; i++)
//    //    {
//    //        tempData = new string[mixedList.Count + 1];

//    //        if (i < gaDatas.Length)
//    //        {
//    //            tempData[0] = gaDatas[i];
//    //            check++;
//    //        } 
//    //        else tempData[0] = string.Empty;

//    //        tempData[1] = generation[i].ToString();
//    //        tempData[2] = infeasiblePopulationCnt[i].ToString();
//    //        tempData[3] = feasiblePopulationCnt[i].ToString();

//    //        tempData[4] = curGenerationBestMean[i].ToString();
//    //        tempData[5] = curGenerationBestStd[i].ToString();
//    //        tempData[6] = curGenerationBestFitness[i].ToString();

//    //        tempData[7] = bestMeanMove[i].ToString();
//    //        tempData[8] = bestStd[i].ToString();
//    //        tempData[9] = bestFitness[i].ToString();

//    //        //tempData[4] = curGenerationBestMean[i].ToString();
//    //        //tempData[5] = bestMeanMove[i].ToString();
//    //        //tempData[6] = curGenerationBestStd[i].ToString();
//    //        //tempData[7] = bestStd[i].ToString();
//    //        //tempData[8] = curGenerationBestFitness[i].ToString();
//    //        //tempData[9] = bestFitness[i].ToString();

//    //        tempData[10] = string.Empty;
//    //        tempData[11] = string.Empty;
//    //        tempData[12] = string.Empty;
//    //        tempData[13] = string.Empty;


//    //        //for (int j = 0; j < feasibleParent[i].Count; j += 2)
//    //        //{
//    //        //    tempData[10] += "(";
//    //        //    tempData[10] += feasibleParent[i][j].ToString();
//    //        //    tempData[10] += ". ";
//    //        //    tempData[10] += feasibleParent[i][j + 1].ToString();
//    //        //    tempData[10] += ")  ";
//    //        //}

//    //        //for (int j = 0; j < feasibleParentIdx[i].Count; j += 2)
//    //        //{
//    //        //    tempData[11] += "(";
//    //        //    tempData[11] += feasibleParentIdx[i][j].ToString();
//    //        //    tempData[11] += ". ";
//    //        //    tempData[11] += feasibleParentIdx[i][j + 1].ToString();
//    //        //    tempData[11] += ")  ";
//    //        //}

//    //        //for (int j = 0; j < infeasibleParent[i].Count; j += 2)
//    //        //{
//    //        //    tempData[12] += "(";
//    //        //    tempData[12] += infeasibleParent[i][j].ToString();
//    //        //    tempData[12] += ". ";
//    //        //    tempData[12] += infeasibleParent[i][j + 1].ToString();
//    //        //    tempData[12] += ")  ";
//    //        //}

//    //        //for (int j = 0; j < infeasibleParentIdx[i].Count; j += 2)
//    //        //{
//    //        //    tempData[13] += "(";
//    //        //    tempData[13] += infeasibleParentIdx[i][j].ToString();
//    //        //    tempData[13] += ". ";
//    //        //    tempData[13] += infeasibleParentIdx[i][j + 1].ToString();
//    //        //    tempData[13] += ")  ";
//    //        //}

//    //        //for (int j = 0; j < curBestMoves[i].Count; j++)
//    //        //{
//    //        //    tempData[14] += curBestMoves[i][j].ToString();
//    //        //    tempData[14] += ",";
//    //        //}


//    //        //for (int j = 0; j < bestMoves[i].Count; j++)
//    //        //{
//    //        //    tempData[15] += bestMoves[i][j].ToString();
//    //        //    tempData[15] += ",";
//    //        //}

//    //        for (int j = 0; j < bestMoves[i].Count; j++)
//    //        {
//    //            tempData[14] += bestMoves[i][j].ToString();
//    //            tempData[14] += ",";
//    //        }

//    //        for (int j = 0; j < ga.repeatMovements[idx].Count; j++)
//    //        {
//    //            tempData[15] += ga.repeatMovements[idx][j].ToString();
//    //            tempData[15] += ",";
//    //        }
//    //        idx++;

//    //        data.Add(tempData);
//    //    }


//    //    foreach (var rm in repeatMovementsCntContainer)
//    //    {
//    //        for (int j = 0; j < rm; j++)
//    //        {
//    //            tempData = new string[mixedList.Count + 1];

//    //            for (int k = 0; k < ga.repeatMovements[idx].Count; k++)
//    //            {
//    //                tempData[14] += ga.repeatMovements[idx][k].ToString();
//    //                tempData[14] += ",";
//    //            }

//    //            tempData[14] += ",";

//    //            for (int k = 0; k < ga.repeatMovements[idx].Count; k++)
//    //            {
//    //                tempData[14] += ga.obstructionRates[idx][k].ToString();
//    //                tempData[14] += ",";
//    //            }

//    //            idx++;

//    //            data.Add(tempData);
//    //        }
//    //        //data.Add(tempData1);
//    //    }




//    //    if (check < 29)
//    //    {
//    //        for (int i = check; i < 29; i++)
//    //        {
//    //            tempData = new string[1];
//    //            tempData[0] = gaDatas[i];
//    //            data.Add(tempData);
//    //        }

//    //    }




//    //    //string[] tempData1 = new string[1];
//    //    ////tempData1[0] = string.Empty;
//    //    //data.Add(tempData1);
//    //    //data.Add(tempData1);

//    //    //int idx = 0;
//    //    //foreach (var rm in repeatMovementsCntContainer)
//    //    //{
//    //    //    for (int j = 0; j < rm; j++)
//    //    //    {
//    //    //        tempData = new string[mixedList.Count +1];

//    //    //        for (int k = 0; k < ga.repeatMovements[idx].Count; k++)
//    //    //        {
//    //    //            tempData[14] += ga.repeatMovements[idx][k].ToString();
//    //    //            tempData[14] += ",";
//    //    //        }

//    //    //        tempData[14] += ",";

//    //    //        for (int k = 0; k < ga.repeatMovements[idx].Count; k++)
//    //    //        {
//    //    //            tempData[14] += ga.obstructionRates[idx][k].ToString();
//    //    //            tempData[14] += ",";
//    //    //        }

//    //    //        idx++;

//    //    //        data.Add(tempData);
//    //    //    }
//    //    //    //data.Add(tempData1);
//    //    //}



//    //    string[][] output = new string[data.Count][];

//    //    for (int i = 0; i < output.Length; i++)
//    //    {
//    //        output[i] = data[i];
//    //    }

//    //    int length = output.GetLength(0);
//    //    string delimiter = ",";

//    //    StringBuilder sb = new StringBuilder();

//    //    for (int index = 0; index < length; index++)
//    //    {
//    //        sb.AppendLine(string.Join(delimiter, output[index]));
//    //    }

//    //    String filePath = Application.dataPath + "/CSV/geneticAlgorithm.csv";
//    //    StreamWriter outStream = System.IO.File.CreateText(filePath);

//    //    outStream.WriteLine(sb);
//    //    outStream.Close();
//    //}


//    //public void write1(GeneticAlgorithm<char> ga)
//    //{
//    //    int idx = 0;
//    //    data = new List<string[]>();

//    //    for (int i = 0; i < repeatMovementsCntContainer.Count; i++)
//    //    {
//    //        for(int j = 0; j < repeatMovementsCntContainer[i];j++)
//    //        {
//    //            string[] tempData = new string[ga.repeat];

//    //            for (int k = 0; k < ga.repeat; k++)
//    //            {
//    //                tempData[k] += ga.repeatMovements[idx][k].ToString();
//    //                //tempData[k] += ",";
//    //            }

//    //            data.Add(tempData);
//    //            idx++;
//    //        }

//    //        string[] tempData1 = new string[1];
//    //        tempData1[0] = string.Empty;
//    //        data.Add(tempData1);
//    //    }




//    //        //for (int i = 0; i < ga.repeatMovements.Count; i++)
//    //        //{
//    //        //    string[] tempData = new string[ga.repeat];

//    //        //    for (int k = 0; k < ga.repeat; k++)
//    //        //    {
//    //        //        tempData[k] = ga.repeatMovements[i][k].ToString();
//    //        //        tempData[k] += ",";
//    //        //    }

//    //        //    data.Add(tempData);
//    //        //    cnt++;

//    //        //    if (cnt >= feasiblePopulationCnt[idx])
//    //        //    {
//    //        //        cnt = 0;
//    //        //        idx++;

//    //        //        tempData = new string[ga.populationSize];
//    //        //        tempData[0] = string.Empty;
//    //        //        data.Add(tempData);
//    //        //    }
//    //        //}


//    //    string[][] output = new string[data.Count][];

//    //    for (int i = 0; i < output.Length; i++)
//    //    {
//    //        output[i] = data[i];
//    //    }

//    //    int length = output.GetLength(0);
//    //    string delimiter = ",";

//    //    StringBuilder sb = new StringBuilder();

//    //    for (int index = 0; index < length; index++)
//    //    {
//    //        sb.AppendLine(string.Join(delimiter, output[index]));
//    //    }

//    //    String filePath = Application.dataPath + "/CSV/repeatMovement.csv";
//    //    StreamWriter outStream = System.IO.File.CreateText(filePath);

//    //    outStream.WriteLine(sb);
//    //    outStream.Close();
//    //}

//}



////public class CSVFileWriter : MonoBehaviour
////{
////    public List<string[]> data = new List<string[]>();

////    public void write(List<int> generation, List<int> infeasiblePopulationCnt, List<int> feasiblePopulationCnt,
////                      List<double> curGenerationBestMean, List<double> curGenerationBestStd, List<double> curGenerationBestFitness, 
////                      List<double> bestMeanMove, List<double> bestStd, List<double> bestFitness,
////                      List<List<double>> feasibleParent, List<List<int>> feasibleParentIdx, List<List<double>> infeasibleParent, List<List<int>> infeasibleParentIdx,
////        List<List<int>> curBestMoves, List<List<int>> bestMoves)
////    {
////        string[] tempData = new string[15];

////        //tempData[0] = "generation";
////        //tempData[1] = "infeasiblePopulationCnt";
////        //tempData[2] = "feasiblePopulationCnt";
////        //tempData[3] = "curGenerationBestMean";
////        //tempData[4] = "curGenerationBestStd";
////        //tempData[5] = "curGenerationBestFitness";
////        //tempData[6] = "bestMeanMove";
////        //tempData[7] = "bestStd";
////        //tempData[8] = "bestFitness";  

////        tempData[0] = "generation";
////        tempData[1] = "infeasiblePopulationCnt";
////        tempData[2] = "feasiblePopulationCnt";
////        tempData[3] = "curGenerationBestMean";
////        tempData[4] = "bestMeanMove";
////        tempData[5] = "curGenerationBestStd";
////        tempData[6] = "bestStd";
////        tempData[7] = "curGenerationBestFitness";
////        tempData[8] = "bestFitness";

////        tempData[9] = "feasibleParent";
////        tempData[10] = "feasibleParentIdx";
////        tempData[11] = "infeasibleParent";
////        tempData[12] = "infeasibleParentIdx";

////        tempData[13] = "curBestMoves";
////        tempData[14] = "bestMoves";

////        data.Add(tempData);

////        for (int i = 0; i < generation.Count; i++)
////        {
////            tempData = new string[15];
////            tempData[0] = generation[i].ToString();
////            tempData[1] = infeasiblePopulationCnt[i].ToString();
////            tempData[2] = feasiblePopulationCnt[i].ToString();
////            tempData[3] = curGenerationBestMean[i].ToString();
////            tempData[4] = bestMeanMove[i].ToString();
////            tempData[5] = curGenerationBestStd[i].ToString();
////            tempData[6] = bestStd[i].ToString();
////            tempData[7] = curGenerationBestFitness[i].ToString();
////            tempData[8] = bestFitness[i].ToString();

////            for(int j=0; j< feasibleParent[i].Count; j += 2)
////            {
////                tempData[9] += "(";
////                tempData[9] += feasibleParent[i][j].ToString();
////                tempData[9] += ". ";
////                tempData[9] += feasibleParent[i][j+1].ToString();
////                tempData[9] += ")  ";
////            }

////            for (int j = 0; j < feasibleParentIdx[i].Count; j += 2)
////            {
////                tempData[10] += "(";
////                tempData[10] += feasibleParentIdx[i][j].ToString();
////                tempData[10] += ". ";
////                tempData[10] += feasibleParentIdx[i][j + 1].ToString();
////                tempData[10] += ")  ";
////            }


////            for (int j = 0; j < infeasibleParent[i].Count; j += 2)
////            {
////                tempData[11] += "(";
////                tempData[11] += infeasibleParent[i][j].ToString();
////                tempData[11] += ". ";
////                tempData[11] += infeasibleParent[i][j + 1].ToString();
////                tempData[11] += ")  ";
////            }


////            for (int j = 0; j < infeasibleParentIdx[i].Count; j += 2)
////            {
////                tempData[12] += "(";
////                tempData[12] += infeasibleParentIdx[i][j].ToString();
////                tempData[12] += ". ";
////                tempData[12] += infeasibleParentIdx[i][j + 1].ToString();
////                tempData[12] += ")  ";
////            }

////            for (int j = 0; j < curBestMoves[i].Count; j++)
////            {
////                tempData[13] += curBestMoves[i][j].ToString();
////                tempData[13] += ",";
////            }
////            for (int j = 0; j < bestMoves[i].Count; j++)
////            {
////                tempData[14] += bestMoves[i][j].ToString();
////                tempData[14] += ",";
////            }


////            //tempData = new string[9];
////            //tempData[0] = generation[i].ToString();
////            //tempData[1] = infeasiblePopulationCnt[i].ToString();
////            //tempData[2] = feasiblePopulationCnt[i].ToString();
////            //tempData[3] = curGenerationBestMean[i].ToString();
////            //tempData[4] = curGenerationBestStd[i].ToString();
////            //tempData[5] = curGenerationBestFitness[i].ToString();
////            //tempData[6] = bestMeanMove[i].ToString();
////            //tempData[7] = bestStd[i].ToString();
////            //tempData[8] = bestFitness[i].ToString();

////            data.Add(tempData);
////        }

////        string[][] output = new string[data.Count][];

////        for (int i = 0; i < output.Length; i++)
////        {
////            output[i] = data[i];
////        }

////        int length = output.GetLength(0);
////        string delimiter = ",";

////        StringBuilder sb = new StringBuilder();

////        for (int index = 0; index < length; index++)
////        {
////            sb.AppendLine(string.Join(delimiter, output[index]));
////        }

////        //string filePath = getPath();
////        //StreamWriter outStream = new StreamWriter(filePath);

////        String filePath = Application.dataPath + "/CSV/geneticAlgorithm.csv";
////        StreamWriter outStream = System.IO.File.CreateText(filePath);

////        outStream.WriteLine(sb);
////        outStream.Close();
////    }

////    //public string getPath()
////    //{
////    //    return Application.dataPath + "/CSV/“+”/Student Data.csv";
////    //}


////    //public StreamWriter outStream;
////    //public String filePath;

////    //public CSVFileWriter()
////    //{
////    //    this.filePath = Application.dataPath + "/CSV/Student Data.csv";
////    //    this.outStream = new StreamWriter(filePath);
////    //}

////}


















////using System;
////using System.Collections.Generic;
////using System.IO;
////using System.Runtime.InteropServices;
////using Excel = Microsoft.Office.Interop.Excel;

////namespace ExportExcel
////{
////    class Dog
////    {
////        public string name;     // 개 이름
////        public string breeds;   // 개 종류
////        public string sex;      // 개 성별
////    }

////    class Program
////    {
////        static Excel.Application excelApp = null;
////        static Excel.Workbook workBook = null;
////        static Excel.Worksheet workSheet = null;

////        static void Main(string[] args)
////        {
////            try
////            {
////                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);  // 바탕화면 경로
////                string path = Path.Combine(desktopPath, "Excel.xlsx");                              // 엑셀 파일 저장 경로

////                excelApp = new Excel.Application();                             // 엑셀 어플리케이션 생성
////                workBook = excelApp.Workbooks.Add();                            // 워크북 추가
////                workSheet = workBook.Worksheets.get_Item(1) as Excel.Worksheet; // 엑셀 첫번째 워크시트 가져오기

////                workSheet.Cells[1, 1] = "이름";
////                workSheet.Cells[1, 2] = "종류";
////                workSheet.Cells[1, 3] = "성별";

////                // 엑셀에 저장할 개 데이터
////                List<Dog> dogs = new List<Dog>();

////                dogs.Add(new Dog() { name = "백구", breeds = "진돗개", sex = "여" });
////                dogs.Add(new Dog() { name = "곰이", breeds = "시바", sex = "남" });
////                dogs.Add(new Dog() { name = "두부", breeds = "포메라니안", sex = "여" });
////                dogs.Add(new Dog() { name = "뭉치", breeds = "말티즈", sex = "남" });

////                for (int i = 0; i < dogs.Count; i++)
////                {
////                    Dog dog = dogs[i];

////                    // 셀에 데이터 입력
////                    workSheet.Cells[2 + i, 1] = dog.name;
////                    workSheet.Cells[2 + i, 2] = dog.breeds;
////                    workSheet.Cells[2 + i, 3] = dog.sex;
////                }

////                workSheet.Columns.AutoFit();                                    // 열 너비 자동 맞춤
////                workBook.SaveAs(path, Excel.XlFileFormat.xlWorkbookDefault);    // 엑셀 파일 저장
////                workBook.Close(true);
////                excelApp.Quit();
////            }
////            finally
////            {
////                ReleaseObject(workSheet);
////                ReleaseObject(workBook);
////                ReleaseObject(excelApp);
////            }
////        }

////        /// <summary>
////        /// 액셀 객체 해제 메소드
////        /// </summary>
////        /// <param name="obj"></param>
////        static void ReleaseObject(object obj)
////        {
////            try
////            {
////                if (obj != null)
////                {
////                    Marshal.ReleaseComObject(obj);  // 액셀 객체 해제
////                    obj = null;
////                }
////            }
////            catch (Exception ex)
////            {
////                obj = null;
////                throw ex;
////            }
////            finally
////            {
////                GC.Collect();   // 가비지 수집
////            }
////        }
////    }
////}