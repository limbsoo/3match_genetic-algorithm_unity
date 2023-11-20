using Mkey;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class CSVFileWriter : MonoBehaviour
{

    public List<string[]> data;

    public List<int> targetNeedCnt;

    public List<int> infeasiblePopulationCnt;
    public List<int> feasiblePopulationCnt;

    public List<int> curGenerationBestMean;
    public List<int> curGenerationBestStd;
    public List<int> curGenerationBestFitness;

    public List<int> bestMeanMove;
    public List<int> bestStd;
    public List<int> bestFitness;

    public List<List<int>> bestSwapContainer;
    public List<List<int>> swapContainer;
    public List<List<int>> matchContainer;

    public CSVFileWriter(Match3Helper m3h)
    {
        data = new List<string[]>();
        targetNeedCnt = new List<int>();
        foreach (var item in m3h.curTargets)
        {
            targetNeedCnt.Add(item.Value.ID);
            targetNeedCnt.Add(item.Value.NeedCount);
        }

        infeasiblePopulationCnt = new List<int>();
        feasiblePopulationCnt = new List<int>();

        curGenerationBestMean = new List<int>();
        curGenerationBestStd = new List<int>();
        curGenerationBestFitness = new List<int>();

        bestMeanMove = new List<int>();
        bestStd = new List<int>();
        bestFitness = new List<int>();

        bestSwapContainer = new List<List<int>>();
        swapContainer = new List<List<int>>();
        matchContainer = new List<List<int>>();

    }

    public void write(GeneticAlgorithm<char> ga, Match3Helper m3h)
    {
        string[] tempData = new string[1000];
        string[] gaDatas = new string[1000];

        gaDatas[0] = "Limit";
        gaDatas[1] = "populationSize";
        gaDatas[2] = ga.populationSize.ToString();
        gaDatas[3] = "elitism";
        gaDatas[4] = ga.elitism.ToString();
        gaDatas[5] = "mutationRate";
        gaDatas[6] = ga.mutationRate.ToString();

        gaDatas[7] = "generationLimit";
        gaDatas[8] = m3h.limits.generation.ToString();
        gaDatas[9] = "moveLimit";
        gaDatas[10] = m3h.limits.move.ToString();
        gaDatas[11] = "repeat";
        gaDatas[12] = m3h.limits.repeat.ToString();

        gaDatas[13] = "INPUT";
        gaDatas[14] = "gridRowSize";
        gaDatas[15] = m3h.rowSize.ToString();
        gaDatas[16] = "gridColSize";
        gaDatas[17] = m3h.colSize.ToString();

        gaDatas[18] = "targetMove";
        gaDatas[19] = ga.targetMove.ToString();
        gaDatas[20] = "targetStd";
        gaDatas[21] = ga.targetStd.ToString();
        gaDatas[22] = "targetFitness";
        gaDatas[23] = ga.targetFitness.ToString();
        gaDatas[24] = "targetCnt";

        int csvIdx = 25;
        //¿œ¥‹ ≈µ
        for (int i = 0; i < targetNeedCnt.Count; i++)
        {
            gaDatas[csvIdx] = targetNeedCnt[i].ToString();
            csvIdx++;
        }

        //gaDatas[csvIdx] = "map"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].swapablePottential.map.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "notObstacle"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].swapablePottential.notObstacle.ToString(); csvIdx++;


        gaDatas[csvIdx] = "map"; csvIdx++;
        gaDatas[csvIdx] = ga.population[0].allPottential.map.ToString(); csvIdx++;

        gaDatas[csvIdx] = "obstacle"; csvIdx++;
        gaDatas[csvIdx] = ga.population[0].allPottential.obstacle.ToString(); csvIdx++;

        gaDatas[csvIdx] = "blocked1"; csvIdx++;
        gaDatas[csvIdx] = ga.population[0].allPottential.blocked1.ToString(); csvIdx++;

        gaDatas[csvIdx] = "blocked2"; csvIdx++;
        gaDatas[csvIdx] = ga.population[0].allPottential.blocked2.ToString(); csvIdx++;

        gaDatas[csvIdx] = "blocked3"; csvIdx++;
        gaDatas[csvIdx] = ga.population[0].allPottential.blocked3.ToString(); csvIdx++;

        gaDatas[csvIdx] = "blocked4"; csvIdx++;
        gaDatas[csvIdx] = ga.population[0].allPottential.blocked4.ToString(); csvIdx++;

        gaDatas[csvIdx] = "overlay1"; csvIdx++;
        gaDatas[csvIdx] = ga.population[0].allPottential.overlay1.ToString(); csvIdx++;

        gaDatas[csvIdx] = "overlay2"; csvIdx++;
        gaDatas[csvIdx] = ga.population[0].allPottential.overlay2.ToString(); csvIdx++;

        gaDatas[csvIdx] = "overlay3"; csvIdx++;
        gaDatas[csvIdx] = ga.population[0].allPottential.overlay3.ToString(); csvIdx++;

        gaDatas[csvIdx] = "overlay4"; csvIdx++;
        gaDatas[csvIdx] = ga.population[0].allPottential.overlay4.ToString(); csvIdx++;

        gaDatas[csvIdx] = "somethingWrong"; csvIdx++;
        gaDatas[csvIdx] = ga.population[0].allPottential.somethingWrong.ToString(); csvIdx++;





        //gaDatas[csvIdx] = "map"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].swapablePottential.map.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "obstacle"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].obstacle.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "blocked1"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].blocked1.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "blocked2"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].blocked2.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "blocked3"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].blocked3.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "overlay1"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].overlay1.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "overlay2"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].overlay2.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "overlay3"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].overlay3.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "somethingWrong"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].somethingWrong.ToString(); csvIdx++;



        //gaDatas[csvIdx] = "map"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].swapablePottential.map.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "obstacle"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].mapObstacle.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "blocked1"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].mapBlocked1.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "blocked2"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].mapBlocked2.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "blocked3"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].mapBlocked3.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "overlay1"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].mapOverlay1.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "overlay2"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].mapOverlay2.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "overlay3"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].mapOverlay3.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "somethingWrong"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].mapSomethingWrong.ToString(); csvIdx++;













        //gaDatas[csvIdx] = "onlyObstacle"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].pottential.onlyObstacle.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "onlyAllBlocked"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].pottential.onlyAllBlocked.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "onlyBlocked"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].pottential.onlyBlocked1.ToString(); csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].pottential.onlyBlocked2.ToString(); csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].pottential.onlyBlocked3.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "onlyAllOverlay"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].pottential.onlyAllOverlay.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "onlyOverlay"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].pottential.onlyOverlay1.ToString(); csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].pottential.onlyOverlay2.ToString(); csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].pottential.onlyOverlay3.ToString(); csvIdx++;


        //gaDatas[csvIdx] = "obstacleAndAllBlocked"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].pottential.obstacleAndAllBlocked.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "obstacleAndBlocked"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].pottential.obstacleAndBlocked1.ToString(); csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].pottential.obstacleAndBlocked2.ToString(); csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].pottential.obstacleAndBlocked3.ToString(); csvIdx++;


        //gaDatas[csvIdx] = "obstacleAndAllOverlay"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].pottential.obstacleAndAllOverlay.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "obstacleAndOverlay"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].pottential.obstacleAndOverlay1.ToString(); csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].pottential.obstacleAndOverlay2.ToString(); csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].pottential.obstacleAndOverlay3.ToString(); csvIdx++;


        gaDatas[csvIdx] = "numOfMatchBlock"; csvIdx++;
        gaDatas[csvIdx] = m3h.numOfMatchBlock.ToString(); csvIdx++;

        string s111 = "'";
        gaDatas[csvIdx] = "gridObjects"; csvIdx++;
        for (int i = 0; i < ga.population[0].gridObjects.Count; i++)
        {
            s111 += ga.population[0].gridObjects[i].ToString();
        }
        gaDatas[csvIdx] = s111; csvIdx++;

        //string s1112222 = "'";
        //gaDatas[csvIdx] = "objectProtection"; csvIdx++;
        //for (int i = 0; i < ga.population[0].objectProtection.Count; i++)
        //{
        //    s1112222 += ga.population[0].objectProtection[i].ToString();
        //}
        //gaDatas[csvIdx] = s1112222; csvIdx++;









        //gaDatas[csvIdx] = "obstacleCnt"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].obstacleCnt.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "possibleCnt"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].mapMatchPotential.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "notObstacleCnt"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].notObstacleCnt.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "obstaclePottentialCnt"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].obstaclePottentialCnt.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "blockedPottentialCnt"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].blockedPottentialCnt.ToString(); csvIdx++;


        //gaDatas[csvIdx] = "blockedPerPottential"; csvIdx++;

        //for (int i = 0;i < 3;i++)
        //{
        //    gaDatas[csvIdx] = ga.population[0].blocks[i].pottential.ToString(); csvIdx++;
        //}

        //gaDatas[csvIdx] = "overlayPottentialCnt"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].overlayPottentialCnt.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "overlayPerPottential"; csvIdx++;

        //for (int i = 3; i < 6; i++)
        //{
        //    gaDatas[csvIdx] = ga.population[0].blocks[i].pottential.ToString(); csvIdx++;
        //}


        //gaDatas[csvIdx] = "numOfMatchBlock"; csvIdx++;
        //gaDatas[csvIdx] = m3h.numOfMatchBlock.ToString(); csvIdx++;



        //string s111 = "'";
        //gaDatas[csvIdx] = "cellsID"; csvIdx++;
        //for (int i = 0; i < ga.population[0].cellsID.Count; i++)
        //{
        //    s111 += ga.population[0].cellsID[i].ToString();
        //}
        //gaDatas[csvIdx] = s111; csvIdx++;

        //string s1112222 = "'";
        //gaDatas[csvIdx] = "protections"; csvIdx++;
        //for (int i = 0; i < m3h.protections.Count; i++)
        //{
        //    s1112222 += m3h.protections[i].ToString();
        //}
        //gaDatas[csvIdx] = s1112222; csvIdx++;






        //gaDatas[csvIdx] = "blockedObjectHitCnt"; csvIdx++;
        //gaDatas[csvIdx] = m3h.blockedObjectHitCnt.ToString(); csvIdx++;
        ////gaDatas[csvIdx] = "blockedObjectHitCnt1"; csvIdx++;
        ////gaDatas[csvIdx] = m3h.blockedObjectHitCnt.ToString(); csvIdx++;
        ////gaDatas[csvIdx] = "blockedObjectHitCnt2"; csvIdx++;
        ////gaDatas[csvIdx] = m3h.blockedObjectHitCnt.ToString(); csvIdx++;
        //gaDatas[csvIdx] = "blockedPottentialCnt"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].blockedPottentialCnt.ToString(); csvIdx++;
        //gaDatas[csvIdx] = "breakableObstacle"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].breakableObstacle.ToString(); csvIdx++;


        //gaDatas[csvIdx] = "overlayObjectHitCnt"; csvIdx++;
        //gaDatas[csvIdx] = m3h.overlayObjectHitCnt.ToString(); csvIdx++;
        ////gaDatas[csvIdx] = "overlayObjectHitCnt1"; csvIdx++;
        ////gaDatas[csvIdx] = m3h.overlayObjectHitCnt.ToString(); csvIdx++;
        ////gaDatas[csvIdx] = "overlayObjectHitCnt2"; csvIdx++;
        ////gaDatas[csvIdx] = m3h.overlayObjectHitCnt.ToString(); csvIdx++;
        //gaDatas[csvIdx] = "overlayPottentialCnt"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].overlayPottentialCnt.ToString(); csvIdx++;
        //gaDatas[csvIdx] = "includeMatchObstacle"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].includeMatchObstacle.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "deduplicationMapMatchPottential"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].matchFromMap.ToString(); csvIdx++;
        //gaDatas[csvIdx] = "deduplicationNearBreakableObstacles"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].nearBreakableObstacles.ToString(); csvIdx++;
        //gaDatas[csvIdx] = "deduplicationIncludeMatchObstacles"; csvIdx++;
        //gaDatas[csvIdx] = ga.population[0].includeMatchObstacles.ToString(); csvIdx++;


        //gaDatas[csvIdx] = "numOfMatchBlock"; csvIdx++;
        //gaDatas[csvIdx] = m3h.numOfMatchBlock.ToString(); csvIdx++;

        //gaDatas[csvIdx] = "blockProtection"; csvIdx++;
        //gaDatas[csvIdx] = m3h.blockProtection.ToString(); csvIdx++;

        //p.cellsID.


        gaDatas[csvIdx] = "possibleCounting"; csvIdx++;
        for (int i = 0; i < ga.population[0].mapMatchPotentialList.Count; i++)
        {
            gaDatas[csvIdx] = ga.population[0].mapMatchPotentialList[i].ToString();
            csvIdx++;
        }

        string s = "'";

        for (int i = 0; i < m3h.gridSize; i++) s += ga.population[0].genes[i];

        gaDatas[csvIdx] = s;
        csvIdx++;



        int idx = 0;
        for (int i = 0; i < m3h.limits.generation; i++)
        {
            tempData = new string[1000];

            if (i < gaDatas.Length) tempData[0] = gaDatas[i];

            if (i < m3h.limits.generation)
            {
                tempData[1] = (i + 1).ToString();
                tempData[2] = 0.ToString();
                tempData[3] = 0.ToString();
                tempData[4] = 0.ToString();
                tempData[5] = 0.ToString();
                tempData[6] = 0.ToString();
                tempData[7] = 0.ToString();
                tempData[8] = 0.ToString();
                tempData[9] = 0.ToString();

                //for (int j = 0; j < bestSwapContainer[i].Count; j++)
                //{
                //    tempData[10] += bestSwapContainer[i][j].ToString();
                //    tempData[10] += ",";
                //}

                for (int j = 0; j < swapContainer[idx].Count; j++)
                {
                    tempData[10] += bestSwapContainer[i][j].ToString();
                    tempData[10] += ",";
                    tempData[11] += swapContainer[idx][j].ToString();
                    tempData[11] += ",";
                    tempData[12] += matchContainer[idx][j].ToString();
                    tempData[12] += ",";
                }
                idx++;
            }

            data.Add(tempData);
        }


        bool isFirst = true;
        while (idx < swapContainer.Count)
        {
            if (!isFirst)
            {
                if (idx < gaDatas.Length) tempData[0] = gaDatas[idx];
            }

            tempData = new string[1000];

            for (int j = 0; j < swapContainer[idx].Count; j++)
            {
                tempData[10 + ga.repeat + 1] += bestSwapContainer[idx][j].ToString();
                tempData[10 + ga.repeat + 1] += ",";

                tempData[11 + ga.repeat + 1] += swapContainer[idx][j].ToString();
                tempData[11 + ga.repeat + 1] += ",";
                tempData[12 + ga.repeat + 1] += matchContainer[idx][j].ToString();
                tempData[12 + ga.repeat + 1] += ",";
            }
            idx++;
            data.Add(tempData);

            isFirst = false;
        }

        if (data.Count < gaDatas.Length)
        {
            while (data.Count < gaDatas.Length)
            {
                tempData = new string[1000];
                tempData[0] = gaDatas[idx];
                idx++;
                data.Add(tempData);
            }
        }

        string[][] output = new string[data.Count][];

        for (int i = 0; i < output.Length; i++) output[i] = data[i];

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++) sb.AppendLine(string.Join(delimiter, output[index]));


        string sPath = "/CSV/";

        sPath += m3h.csvFolder.ToString();
        sPath += '/';
        sPath += (m3h.csvCnt % 10).ToString();
        sPath += ".csv";
        m3h.csvCnt++;
        String filePath = Application.dataPath + sPath;

        StreamWriter outStream = System.IO.File.CreateText(filePath);

        outStream.WriteLine(sb);
        outStream.Close();
    }
}






























