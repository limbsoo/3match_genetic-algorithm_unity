using Mkey;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class CSVFileWriter : MonoBehaviour
{
    public List<object> mixedList;

    public List<int> generation;
    public List<int> infeasiblePopulationCnt;
    public List<int> feasiblePopulationCnt;
    public List<double> curGenerationBestMean;
    public List<double> curGenerationBestStd;
    public List<double> curGenerationBestFitness;
    public List<double> bestMeanMove;
    public List<double> bestStd;
    public List<double> bestFitness;
    public List<List<double>> feasibleParent;
    public List<List<int>> feasibleParentIdx;
    public List<List<double>> infeasibleParent;
    public List<List<int>> infeasibleParentIdx;
    public List<List<int>> curBestMoves;
    public List<List<int>> bestMoves;
    public List<List<int>> repeatMovements;
    public List<int> repeatMovementsCntContainer;

    public List<string[]> data = new List<string[]>();

    public CSVFileWriter()
    {
        generation = new List<int>();
        infeasiblePopulationCnt = new List<int>();
        feasiblePopulationCnt = new List<int>();

        curGenerationBestMean = new List<double>();
        curGenerationBestStd = new List<double>();
        curGenerationBestFitness = new List<double>();

        bestMeanMove = new List<double>();
        bestStd = new List<double>();
        bestFitness = new List<double>();

        feasibleParent = new List<List<double>>();
        feasibleParentIdx = new List<List<int>>();
        infeasibleParent = new List<List<double>>();
        infeasibleParentIdx = new List<List<int>>();

        curBestMoves = new List<List<int>>();
        bestMoves = new List<List<int>>();
        repeatMovementsCntContainer = new List<int>();
    }

    public void write(GeneticAlgorithm<char> ga, List<GridCell> Cells)
    {
        string[] tempData = new string[100];

        //tempData[0] = string.Empty;
        //tempData[1] = "generation";
        //tempData[2] = "infeasiblePopulationCnt";
        //tempData[3] = "feasiblePopulationCnt";
        //tempData[4] = "curGenerationBestMean";
        //tempData[5] = "bestMeanMove";
        //tempData[6] = "curGenerationBestStd";
        //tempData[7] = "bestStd";
        //tempData[8] = "curGenerationBestFitness";
        //tempData[9] = "bestFitness";
        //tempData[10]= "feasibleParent";
        //tempData[11] = "feasibleParentIdx";
        //tempData[12] = "infeasibleParent";
        //tempData[13] = "infeasibleParentIdx";
        //tempData[14] = "curBestMoves";
        //tempData[15] = "bestMoves";
        //data.Add(tempData);

        string[] gaDatas = new string[34];

        gaDatas[0] = "Limit";
        gaDatas[1] = "populationSize";
        gaDatas[2] = ga.populationSize.ToString();
        gaDatas[3] = "elitism";
        gaDatas[4] = ga.elitism.ToString();
        gaDatas[5] = "mutationRate";
        gaDatas[6] = ga.mutationRate.ToString();
        gaDatas[7] = "generationLimit";
        gaDatas[8] = ga.generationLimit.ToString();
        gaDatas[9] = "moveLimit";
        gaDatas[10] = ga.moveLimit.ToString();
        gaDatas[11] = "repeat";
        gaDatas[12] = ga.repeat.ToString();
        gaDatas[13] = "INPUT";
        gaDatas[14] = "gridRowSize";
        gaDatas[15] = Cells[0].GRow.Length.ToString();
        gaDatas[16] = "gridColSize";
        gaDatas[17] = Cells[0].GColumn.Length.ToString();
        gaDatas[18] = "targetMove";
        gaDatas[19] = ga.targetMove.ToString();
        gaDatas[20] = "targetStd";
        gaDatas[21] = ga.targetStd.ToString();
        gaDatas[22] = "targetFitness";
        gaDatas[23] = ga.targetFitness.ToString();


        gaDatas[24] = "targetCnt";
        gaDatas[25] = ga.targetNeedCnt[0].ToString();
        gaDatas[26] = ga.targetNeedCnt[1].ToString();
        gaDatas[27] = ga.targetNeedCnt[2].ToString();
        gaDatas[28] = ga.targetNeedCnt[3].ToString();
        gaDatas[29] = ga.targetNeedCnt[4].ToString();
        gaDatas[30] = ga.targetNeedCnt[5].ToString();
        gaDatas[31] = ga.targetNeedCnt[6].ToString();
        gaDatas[32] = "possibleCnt";
        gaDatas[33] = ga.possibleCnt.ToString();


        int idx = 0;
        //int excelLength = Math.Max(generation.Count, gaDatas.Length);



        for (int i = 0; i < generation.Count; i++)
        {
            //tempData = new string[mixedList.Count + 1];
            tempData = new string[13];

            if (i < gaDatas.Length) tempData[0] = gaDatas[i];

            if (i < generation.Count)
            {
                tempData[1] = generation[i].ToString();
                tempData[2] = infeasiblePopulationCnt[i].ToString();
                tempData[3] = feasiblePopulationCnt[i].ToString();

                tempData[4] = curGenerationBestMean[i].ToString();
                tempData[5] = curGenerationBestStd[i].ToString();
                tempData[6] = curGenerationBestFitness[i].ToString();

                tempData[7] = bestMeanMove[i].ToString();
                tempData[8] = bestStd[i].ToString();
                tempData[9] = bestFitness[i].ToString();

                for (int j = 0; j < bestMoves[i].Count; j++)
                {
                    tempData[10] += bestMoves[i][j].ToString();
                    tempData[10] += ",";
                }

                //if(ga.repeat < 20)
                //{
                //    int iidx = 0;
                //    while(ga.repeat + iidx < 20)
                //    {
                //        tempData[10] += ",";
                //        iidx++;
                //    }
                    
                //}

                for (int j = 0; j < ga.repeatMovements[idx].Count; j++)
                {
                    tempData[11] += ga.repeatMovements[idx][j].ToString();
                    tempData[11] += ",";
                    //tempData[12] += ga.shorCutRates[idx][j].ToString();
                    //tempData[12] += ",";
                    tempData[12] += ga.allMovements[idx][j].ToString();
                    tempData[12] += ",";


                }

                idx++;

                //tempData[12] += 1.ToString();
                //tempData[12] += ",";
            }

            data.Add(tempData);
        }

        tempData = new string[50];
        //data.Add(tempData);

        if(idx < ga.repeatMovements.Count)
        {
            for (int j = 0; j < ga.repeatMovements[idx].Count; j++)
            {
                tempData[10 + ga.repeat + 1] += ga.repeatMovements[idx][j].ToString();
                tempData[10 + ga.repeat + 1] += ",";
            }

            for (int j = 0; j < ga.repeatMovements[idx].Count; j++)
            {
                //tempData[11 + ga.repeat + 1] += ga.shorCutRates[idx][j].ToString();
                //tempData[11 + ga.repeat + 1] += ",";

                tempData[11 + ga.repeat + 1] += ga.allMovements[idx][j].ToString();
                tempData[11 + ga.repeat + 1] += ",";
            }



            idx++;

            data.Add(tempData);
        }

        while (idx < ga.repeatMovements.Count)
        {
            if (idx < gaDatas.Length) tempData[0] = gaDatas[idx];

            tempData = new string[50];

            for (int j = 0; j < ga.repeatMovements[idx].Count; j++)
            {
                tempData[10 + ga.repeat + 1] += ga.repeatMovements[idx][j].ToString();
                tempData[10 + ga.repeat + 1] += ",";

                //tempData[11 + ga.repeat + 1] += ga.shorCutRates[idx][j].ToString();
                //tempData[11 + ga.repeat + 1] += ",";

                tempData[11 + ga.repeat + 1] += ga.allMovements[idx][j].ToString();
                tempData[11 + ga.repeat + 1] += ",";
            }
            idx++;

            data.Add(tempData);
        }

        if (data.Count < gaDatas.Length)
        {
            while(data.Count < gaDatas.Length)
            {
                tempData = new string[13];
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

        String filePath = Application.dataPath + "/CSV/geneticAlgorithm.csv";
        StreamWriter outStream = System.IO.File.CreateText(filePath);

        outStream.WriteLine(sb);
        outStream.Close();
    }

    //public void write(GeneticAlgorithm<char> ga, List<GridCell> Cells)
    //{
    //    string[] tempData = new string[mixedList.Count + 1];

    //    //tempData[0] = string.Empty;
    //    //tempData[1] = "generation";
    //    //tempData[2] = "infeasiblePopulationCnt";
    //    //tempData[3] = "feasiblePopulationCnt";
    //    //tempData[4] = "curGenerationBestMean";
    //    //tempData[5] = "bestMeanMove";
    //    //tempData[6] = "curGenerationBestStd";
    //    //tempData[7] = "bestStd";
    //    //tempData[8] = "curGenerationBestFitness";
    //    //tempData[9] = "bestFitness";
    //    //tempData[10]= "feasibleParent";
    //    //tempData[11] = "feasibleParentIdx";
    //    //tempData[12] = "infeasibleParent";
    //    //tempData[13] = "infeasibleParentIdx";
    //    //tempData[14] = "curBestMoves";
    //    //tempData[15] = "bestMoves";
    //    //data.Add(tempData);

    //    string[] gaDatas = new string[29];

    //    gaDatas[0] = "Limit";
    //    gaDatas[1] = "populationSize";
    //    gaDatas[2] = ga.populationSize.ToString();
    //    gaDatas[3] = "elitism";
    //    gaDatas[4] = ga.elitism.ToString();
    //    gaDatas[5] = "mutationRate";
    //    gaDatas[6] = ga.mutationRate.ToString();
    //    gaDatas[7] = "generationLimit";
    //    gaDatas[8] = ga.generationLimit.ToString();
    //    gaDatas[9] = "moveLimit";
    //    gaDatas[10] = ga.moveLimit.ToString();
    //    gaDatas[11] = "repeat";
    //    gaDatas[12] = ga.repeat.ToString();
    //    gaDatas[13] = "INPUT";
    //    gaDatas[14] = "gridRowSize";
    //    gaDatas[15] = Cells[0].GRow.Length.ToString();
    //    gaDatas[16] = "gridColSize";
    //    gaDatas[17] = Cells[0].GColumn.Length.ToString();
    //    gaDatas[18] = "targetMove";
    //    gaDatas[19] = ga.targetMove.ToString();
    //    gaDatas[20] = "targetStd";
    //    gaDatas[21] = ga.targetStd.ToString();
    //    gaDatas[22] = "targetFitness";
    //    gaDatas[23] = ga.targetFitness.ToString();
    //    gaDatas[24] = "targetBlocks";
    //    gaDatas[25] = "name";
    //    gaDatas[26] = "Apple";
    //    gaDatas[27] = "needCnt";
    //    gaDatas[28] = 15.ToString();

    //    int check = 0;
    //    int idx = 0;
    //    int idx2 = 0;

    //    for (int i = 0; i < generation.Count; i++)
    //    {
    //        tempData = new string[mixedList.Count + 1];

    //        if (i < gaDatas.Length)
    //        {
    //            tempData[0] = gaDatas[i];
    //            check++;
    //        } 
    //        else tempData[0] = string.Empty;

    //        tempData[1] = generation[i].ToString();
    //        tempData[2] = infeasiblePopulationCnt[i].ToString();
    //        tempData[3] = feasiblePopulationCnt[i].ToString();

    //        tempData[4] = curGenerationBestMean[i].ToString();
    //        tempData[5] = curGenerationBestStd[i].ToString();
    //        tempData[6] = curGenerationBestFitness[i].ToString();

    //        tempData[7] = bestMeanMove[i].ToString();
    //        tempData[8] = bestStd[i].ToString();
    //        tempData[9] = bestFitness[i].ToString();

    //        //tempData[4] = curGenerationBestMean[i].ToString();
    //        //tempData[5] = bestMeanMove[i].ToString();
    //        //tempData[6] = curGenerationBestStd[i].ToString();
    //        //tempData[7] = bestStd[i].ToString();
    //        //tempData[8] = curGenerationBestFitness[i].ToString();
    //        //tempData[9] = bestFitness[i].ToString();

    //        tempData[10] = string.Empty;
    //        tempData[11] = string.Empty;
    //        tempData[12] = string.Empty;
    //        tempData[13] = string.Empty;


    //        //for (int j = 0; j < feasibleParent[i].Count; j += 2)
    //        //{
    //        //    tempData[10] += "(";
    //        //    tempData[10] += feasibleParent[i][j].ToString();
    //        //    tempData[10] += ". ";
    //        //    tempData[10] += feasibleParent[i][j + 1].ToString();
    //        //    tempData[10] += ")  ";
    //        //}

    //        //for (int j = 0; j < feasibleParentIdx[i].Count; j += 2)
    //        //{
    //        //    tempData[11] += "(";
    //        //    tempData[11] += feasibleParentIdx[i][j].ToString();
    //        //    tempData[11] += ". ";
    //        //    tempData[11] += feasibleParentIdx[i][j + 1].ToString();
    //        //    tempData[11] += ")  ";
    //        //}

    //        //for (int j = 0; j < infeasibleParent[i].Count; j += 2)
    //        //{
    //        //    tempData[12] += "(";
    //        //    tempData[12] += infeasibleParent[i][j].ToString();
    //        //    tempData[12] += ". ";
    //        //    tempData[12] += infeasibleParent[i][j + 1].ToString();
    //        //    tempData[12] += ")  ";
    //        //}

    //        //for (int j = 0; j < infeasibleParentIdx[i].Count; j += 2)
    //        //{
    //        //    tempData[13] += "(";
    //        //    tempData[13] += infeasibleParentIdx[i][j].ToString();
    //        //    tempData[13] += ". ";
    //        //    tempData[13] += infeasibleParentIdx[i][j + 1].ToString();
    //        //    tempData[13] += ")  ";
    //        //}

    //        //for (int j = 0; j < curBestMoves[i].Count; j++)
    //        //{
    //        //    tempData[14] += curBestMoves[i][j].ToString();
    //        //    tempData[14] += ",";
    //        //}


    //        //for (int j = 0; j < bestMoves[i].Count; j++)
    //        //{
    //        //    tempData[15] += bestMoves[i][j].ToString();
    //        //    tempData[15] += ",";
    //        //}

    //        for (int j = 0; j < bestMoves[i].Count; j++)
    //        {
    //            tempData[14] += bestMoves[i][j].ToString();
    //            tempData[14] += ",";
    //        }

    //        for (int j = 0; j < ga.repeatMovements[idx].Count; j++)
    //        {
    //            tempData[15] += ga.repeatMovements[idx][j].ToString();
    //            tempData[15] += ",";
    //        }
    //        idx++;

    //        data.Add(tempData);
    //    }


    //    foreach (var rm in repeatMovementsCntContainer)
    //    {
    //        for (int j = 0; j < rm; j++)
    //        {
    //            tempData = new string[mixedList.Count + 1];

    //            for (int k = 0; k < ga.repeatMovements[idx].Count; k++)
    //            {
    //                tempData[14] += ga.repeatMovements[idx][k].ToString();
    //                tempData[14] += ",";
    //            }

    //            tempData[14] += ",";

    //            for (int k = 0; k < ga.repeatMovements[idx].Count; k++)
    //            {
    //                tempData[14] += ga.obstructionRates[idx][k].ToString();
    //                tempData[14] += ",";
    //            }

    //            idx++;

    //            data.Add(tempData);
    //        }
    //        //data.Add(tempData1);
    //    }




    //    if (check < 29)
    //    {
    //        for (int i = check; i < 29; i++)
    //        {
    //            tempData = new string[1];
    //            tempData[0] = gaDatas[i];
    //            data.Add(tempData);
    //        }

    //    }




    //    //string[] tempData1 = new string[1];
    //    ////tempData1[0] = string.Empty;
    //    //data.Add(tempData1);
    //    //data.Add(tempData1);

    //    //int idx = 0;
    //    //foreach (var rm in repeatMovementsCntContainer)
    //    //{
    //    //    for (int j = 0; j < rm; j++)
    //    //    {
    //    //        tempData = new string[mixedList.Count +1];

    //    //        for (int k = 0; k < ga.repeatMovements[idx].Count; k++)
    //    //        {
    //    //            tempData[14] += ga.repeatMovements[idx][k].ToString();
    //    //            tempData[14] += ",";
    //    //        }

    //    //        tempData[14] += ",";

    //    //        for (int k = 0; k < ga.repeatMovements[idx].Count; k++)
    //    //        {
    //    //            tempData[14] += ga.obstructionRates[idx][k].ToString();
    //    //            tempData[14] += ",";
    //    //        }

    //    //        idx++;

    //    //        data.Add(tempData);
    //    //    }
    //    //    //data.Add(tempData1);
    //    //}



    //    string[][] output = new string[data.Count][];

    //    for (int i = 0; i < output.Length; i++)
    //    {
    //        output[i] = data[i];
    //    }

    //    int length = output.GetLength(0);
    //    string delimiter = ",";

    //    StringBuilder sb = new StringBuilder();

    //    for (int index = 0; index < length; index++)
    //    {
    //        sb.AppendLine(string.Join(delimiter, output[index]));
    //    }

    //    String filePath = Application.dataPath + "/CSV/geneticAlgorithm.csv";
    //    StreamWriter outStream = System.IO.File.CreateText(filePath);

    //    outStream.WriteLine(sb);
    //    outStream.Close();
    //}


    //public void write1(GeneticAlgorithm<char> ga)
    //{
    //    int idx = 0;
    //    data = new List<string[]>();

    //    for (int i = 0; i < repeatMovementsCntContainer.Count; i++)
    //    {
    //        for(int j = 0; j < repeatMovementsCntContainer[i];j++)
    //        {
    //            string[] tempData = new string[ga.repeat];

    //            for (int k = 0; k < ga.repeat; k++)
    //            {
    //                tempData[k] += ga.repeatMovements[idx][k].ToString();
    //                //tempData[k] += ",";
    //            }

    //            data.Add(tempData);
    //            idx++;
    //        }

    //        string[] tempData1 = new string[1];
    //        tempData1[0] = string.Empty;
    //        data.Add(tempData1);
    //    }




    //        //for (int i = 0; i < ga.repeatMovements.Count; i++)
    //        //{
    //        //    string[] tempData = new string[ga.repeat];

    //        //    for (int k = 0; k < ga.repeat; k++)
    //        //    {
    //        //        tempData[k] = ga.repeatMovements[i][k].ToString();
    //        //        tempData[k] += ",";
    //        //    }

    //        //    data.Add(tempData);
    //        //    cnt++;

    //        //    if (cnt >= feasiblePopulationCnt[idx])
    //        //    {
    //        //        cnt = 0;
    //        //        idx++;

    //        //        tempData = new string[ga.populationSize];
    //        //        tempData[0] = string.Empty;
    //        //        data.Add(tempData);
    //        //    }
    //        //}


    //    string[][] output = new string[data.Count][];

    //    for (int i = 0; i < output.Length; i++)
    //    {
    //        output[i] = data[i];
    //    }

    //    int length = output.GetLength(0);
    //    string delimiter = ",";

    //    StringBuilder sb = new StringBuilder();

    //    for (int index = 0; index < length; index++)
    //    {
    //        sb.AppendLine(string.Join(delimiter, output[index]));
    //    }

    //    String filePath = Application.dataPath + "/CSV/repeatMovement.csv";
    //    StreamWriter outStream = System.IO.File.CreateText(filePath);

    //    outStream.WriteLine(sb);
    //    outStream.Close();
    //}

}



//public class CSVFileWriter : MonoBehaviour
//{
//    public List<string[]> data = new List<string[]>();

//    public void write(List<int> generation, List<int> infeasiblePopulationCnt, List<int> feasiblePopulationCnt,
//                      List<double> curGenerationBestMean, List<double> curGenerationBestStd, List<double> curGenerationBestFitness, 
//                      List<double> bestMeanMove, List<double> bestStd, List<double> bestFitness,
//                      List<List<double>> feasibleParent, List<List<int>> feasibleParentIdx, List<List<double>> infeasibleParent, List<List<int>> infeasibleParentIdx,
//        List<List<int>> curBestMoves, List<List<int>> bestMoves)
//    {
//        string[] tempData = new string[15];

//        //tempData[0] = "generation";
//        //tempData[1] = "infeasiblePopulationCnt";
//        //tempData[2] = "feasiblePopulationCnt";
//        //tempData[3] = "curGenerationBestMean";
//        //tempData[4] = "curGenerationBestStd";
//        //tempData[5] = "curGenerationBestFitness";
//        //tempData[6] = "bestMeanMove";
//        //tempData[7] = "bestStd";
//        //tempData[8] = "bestFitness";  

//        tempData[0] = "generation";
//        tempData[1] = "infeasiblePopulationCnt";
//        tempData[2] = "feasiblePopulationCnt";
//        tempData[3] = "curGenerationBestMean";
//        tempData[4] = "bestMeanMove";
//        tempData[5] = "curGenerationBestStd";
//        tempData[6] = "bestStd";
//        tempData[7] = "curGenerationBestFitness";
//        tempData[8] = "bestFitness";

//        tempData[9] = "feasibleParent";
//        tempData[10] = "feasibleParentIdx";
//        tempData[11] = "infeasibleParent";
//        tempData[12] = "infeasibleParentIdx";

//        tempData[13] = "curBestMoves";
//        tempData[14] = "bestMoves";

//        data.Add(tempData);

//        for (int i = 0; i < generation.Count; i++)
//        {
//            tempData = new string[15];
//            tempData[0] = generation[i].ToString();
//            tempData[1] = infeasiblePopulationCnt[i].ToString();
//            tempData[2] = feasiblePopulationCnt[i].ToString();
//            tempData[3] = curGenerationBestMean[i].ToString();
//            tempData[4] = bestMeanMove[i].ToString();
//            tempData[5] = curGenerationBestStd[i].ToString();
//            tempData[6] = bestStd[i].ToString();
//            tempData[7] = curGenerationBestFitness[i].ToString();
//            tempData[8] = bestFitness[i].ToString();

//            for(int j=0; j< feasibleParent[i].Count; j += 2)
//            {
//                tempData[9] += "(";
//                tempData[9] += feasibleParent[i][j].ToString();
//                tempData[9] += ". ";
//                tempData[9] += feasibleParent[i][j+1].ToString();
//                tempData[9] += ")  ";
//            }

//            for (int j = 0; j < feasibleParentIdx[i].Count; j += 2)
//            {
//                tempData[10] += "(";
//                tempData[10] += feasibleParentIdx[i][j].ToString();
//                tempData[10] += ". ";
//                tempData[10] += feasibleParentIdx[i][j + 1].ToString();
//                tempData[10] += ")  ";
//            }


//            for (int j = 0; j < infeasibleParent[i].Count; j += 2)
//            {
//                tempData[11] += "(";
//                tempData[11] += infeasibleParent[i][j].ToString();
//                tempData[11] += ". ";
//                tempData[11] += infeasibleParent[i][j + 1].ToString();
//                tempData[11] += ")  ";
//            }


//            for (int j = 0; j < infeasibleParentIdx[i].Count; j += 2)
//            {
//                tempData[12] += "(";
//                tempData[12] += infeasibleParentIdx[i][j].ToString();
//                tempData[12] += ". ";
//                tempData[12] += infeasibleParentIdx[i][j + 1].ToString();
//                tempData[12] += ")  ";
//            }

//            for (int j = 0; j < curBestMoves[i].Count; j++)
//            {
//                tempData[13] += curBestMoves[i][j].ToString();
//                tempData[13] += ",";
//            }
//            for (int j = 0; j < bestMoves[i].Count; j++)
//            {
//                tempData[14] += bestMoves[i][j].ToString();
//                tempData[14] += ",";
//            }


//            //tempData = new string[9];
//            //tempData[0] = generation[i].ToString();
//            //tempData[1] = infeasiblePopulationCnt[i].ToString();
//            //tempData[2] = feasiblePopulationCnt[i].ToString();
//            //tempData[3] = curGenerationBestMean[i].ToString();
//            //tempData[4] = curGenerationBestStd[i].ToString();
//            //tempData[5] = curGenerationBestFitness[i].ToString();
//            //tempData[6] = bestMeanMove[i].ToString();
//            //tempData[7] = bestStd[i].ToString();
//            //tempData[8] = bestFitness[i].ToString();

//            data.Add(tempData);
//        }

//        string[][] output = new string[data.Count][];

//        for (int i = 0; i < output.Length; i++)
//        {
//            output[i] = data[i];
//        }

//        int length = output.GetLength(0);
//        string delimiter = ",";

//        StringBuilder sb = new StringBuilder();

//        for (int index = 0; index < length; index++)
//        {
//            sb.AppendLine(string.Join(delimiter, output[index]));
//        }

//        //string filePath = getPath();
//        //StreamWriter outStream = new StreamWriter(filePath);

//        String filePath = Application.dataPath + "/CSV/geneticAlgorithm.csv";
//        StreamWriter outStream = System.IO.File.CreateText(filePath);

//        outStream.WriteLine(sb);
//        outStream.Close();
//    }

//    //public string getPath()
//    //{
//    //    return Application.dataPath + "/CSV/“+”/Student Data.csv";
//    //}


//    //public StreamWriter outStream;
//    //public String filePath;

//    //public CSVFileWriter()
//    //{
//    //    this.filePath = Application.dataPath + "/CSV/Student Data.csv";
//    //    this.outStream = new StreamWriter(filePath);
//    //}

//}


















//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Runtime.InteropServices;
//using Excel = Microsoft.Office.Interop.Excel;

//namespace ExportExcel
//{
//    class Dog
//    {
//        public string name;     // 개 이름
//        public string breeds;   // 개 종류
//        public string sex;      // 개 성별
//    }

//    class Program
//    {
//        static Excel.Application excelApp = null;
//        static Excel.Workbook workBook = null;
//        static Excel.Worksheet workSheet = null;

//        static void Main(string[] args)
//        {
//            try
//            {
//                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);  // 바탕화면 경로
//                string path = Path.Combine(desktopPath, "Excel.xlsx");                              // 엑셀 파일 저장 경로

//                excelApp = new Excel.Application();                             // 엑셀 어플리케이션 생성
//                workBook = excelApp.Workbooks.Add();                            // 워크북 추가
//                workSheet = workBook.Worksheets.get_Item(1) as Excel.Worksheet; // 엑셀 첫번째 워크시트 가져오기

//                workSheet.Cells[1, 1] = "이름";
//                workSheet.Cells[1, 2] = "종류";
//                workSheet.Cells[1, 3] = "성별";

//                // 엑셀에 저장할 개 데이터
//                List<Dog> dogs = new List<Dog>();

//                dogs.Add(new Dog() { name = "백구", breeds = "진돗개", sex = "여" });
//                dogs.Add(new Dog() { name = "곰이", breeds = "시바", sex = "남" });
//                dogs.Add(new Dog() { name = "두부", breeds = "포메라니안", sex = "여" });
//                dogs.Add(new Dog() { name = "뭉치", breeds = "말티즈", sex = "남" });

//                for (int i = 0; i < dogs.Count; i++)
//                {
//                    Dog dog = dogs[i];

//                    // 셀에 데이터 입력
//                    workSheet.Cells[2 + i, 1] = dog.name;
//                    workSheet.Cells[2 + i, 2] = dog.breeds;
//                    workSheet.Cells[2 + i, 3] = dog.sex;
//                }

//                workSheet.Columns.AutoFit();                                    // 열 너비 자동 맞춤
//                workBook.SaveAs(path, Excel.XlFileFormat.xlWorkbookDefault);    // 엑셀 파일 저장
//                workBook.Close(true);
//                excelApp.Quit();
//            }
//            finally
//            {
//                ReleaseObject(workSheet);
//                ReleaseObject(workBook);
//                ReleaseObject(excelApp);
//            }
//        }

//        /// <summary>
//        /// 액셀 객체 해제 메소드
//        /// </summary>
//        /// <param name="obj"></param>
//        static void ReleaseObject(object obj)
//        {
//            try
//            {
//                if (obj != null)
//                {
//                    Marshal.ReleaseComObject(obj);  // 액셀 객체 해제
//                    obj = null;
//                }
//            }
//            catch (Exception ex)
//            {
//                obj = null;
//                throw ex;
//            }
//            finally
//            {
//                GC.Collect();   // 가비지 수집
//            }
//        }
//    }
//}