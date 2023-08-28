using Mkey;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

//using UnityEngine;

public class DNA<T>
{
	public T[] genes { get; private set; }
	private System.Random random;
	private Func<T> getRandomGene;
    public bool isFeasible = false;
    public int infeasibleCellCnt;
	public double fitness;

    public double mean;
    public double stanardDeviation;

    public int numMove;
    public int curState;
    public bool targetClear = false;
    public bool maxedOut = false;

    public int obstructionRate = 0;
    public int shortCutCnt = 0;

    public int allMove = 0;

    public DNA(int size, Random random, Func<T> getRandomGene, bool shouldInitGenes = true)
	{
		genes = new T[size];
		this.random = random;
		this.getRandomGene = getRandomGene;

		if (shouldInitGenes) for (int i = 0; i < genes.Length; i++) genes[i] = getRandomGene();
	}

    public void calculateFeasibleFitness(List<int> moveContainer, double targetMove, double targetStd)
    {
        //double max = 0;
        //int maxIdx = 0;
        //double min = 120;
        //int minIdx = 0;


        //double sum = 0;
        //for (int i = 0; i < moveContainer.Count; i++)
        //{
        //    sum += moveContainer[i];

        //    if(max < moveContainer[i])
        //    {
        //        maxIdx = i;
        //        max = moveContainer[i];
        //    }

        //    if(min > moveContainer[i])
        //    {
        //        minIdx = i;
        //        min = moveContainer[i];
        //    }
        //}

        //sum -= max;
        //sum -= min;

        //mean = sum / (moveContainer.Count - 2);

        //double squaredDifferencesSum = 0.0;
        //for (int i = 0; i < moveContainer.Count; i++)
        //{
        //    if (i == minIdx || i == maxIdx) continue;

        //    double diff = moveContainer[i] - mean;
        //    squaredDifferencesSum += diff * diff;
        //}
        //double variance = squaredDifferencesSum / (moveContainer.Count - 2);
        //stanardDeviation = Math.Sqrt(variance);

        //double normalMean = 1.0 / (1.0 + Math.Abs(mean - targetMove));
        //double normalStd = 1.0 / (1.0 + Math.Abs(stanardDeviation - targetStd));

        //double alpha = 0.5;
        //fitness = alpha * normalMean + (1 - alpha) * normalStd;






        double sum = 0;
        for (int i = 0; i < moveContainer.Count; i++) sum += moveContainer[i];
        mean = sum / moveContainer.Count;

        double squaredDifferencesSum = 0.0;
        for (int i = 0; i < moveContainer.Count; i++)
        {
            double diff = moveContainer[i] - mean;
            squaredDifferencesSum += diff * diff;
        }
        double variance = squaredDifferencesSum / moveContainer.Count;
        stanardDeviation = Math.Sqrt(variance);

        double normalMean = 1.0 / (1.0 + Math.Abs(mean - targetMove));
        double normalStd = 1.0 / (1.0 + Math.Abs(stanardDeviation - targetStd));

        double alpha = 0.5;
        fitness = alpha * normalMean + (1 - alpha) * normalStd;







        //double sum = 0;
        //for (int i = 0; i < moveContainer.Count; i++) sum += moveContainer[i];
        //mean = sum / moveContainer.Count;

        //double squaredDifferencesSum = 0.0;
        //for (int i = 0; i < moveContainer.Count; i++)
        //{
        //    double diff = moveContainer[i] - mean;
        //    squaredDifferencesSum += diff * diff;
        //}
        //double variance = squaredDifferencesSum / moveContainer.Count;
        //stanardDeviation = Math.Sqrt(variance);

        //// Z-Score Normalization 적용하여 평균값 계산
        //double sumNormalizedData = 0;
        //foreach (int moveCount in moveContainer)
        //{
        //    double normalizedValue = (moveCount - mean) / stanardDeviation;
        //    sumNormalizedData += normalizedValue;
        //}

        //// 파퓰레이션의 최종 피트니스 값을 평균으로 계산
        //fitness = sumNormalizedData / moveContainer.Count;








        //double sum = 0;
        //for (int i = 0; i < moveContainer.Count; i++) sum += moveContainer[i];
        //mean = sum / moveContainer.Count;

        //double squaredDifferencesSum = 0.0;
        //for (int i = 0; i < moveContainer.Count; i++)
        //{
        //    double diff = moveContainer[i] - mean;
        //    squaredDifferencesSum += diff * diff;
        //}
        //double variance = squaredDifferencesSum / moveContainer.Count;
        //stanardDeviation = Math.Sqrt(variance);

        //double normalMean = Math.Abs(mean - targetMove);

        //int maxValue = 100;
        //int minValue = 1;

        //normalMean = (double)(Math.Abs(mean - targetMove) - minValue) / (maxValue - minValue);

        //normalMean = Math.Max(0, Math.Min(1, normalMean));


        //fitness = normalMean;






        //double normalStd = Math.Exp(-Math.Abs(stanardDeviation - targetStd));

        //double alpha = 0.5;
        //fitness = alpha * expMean + (1 - alpha) * expStd;




        //double sum = 0;
        //for (int i = 0; i < moveContainer.Count; i++) sum += moveContainer[i];
        //mean = sum / moveContainer.Count;

        //double squaredDifferencesSum = 0.0;
        //for (int i = 0; i < moveContainer.Count; i++)
        //{
        //    double diff = moveContainer[i] - mean;
        //    squaredDifferencesSum += diff * diff;
        //}
        //double variance = squaredDifferencesSum / moveContainer.Count;
        //stanardDeviation = Math.Sqrt(variance);

        //double expMean = Math.Exp(-Math.Abs(mean - targetMove));
        //double expStd = Math.Exp(-Math.Abs(stanardDeviation - targetStd));

        //double alpha = 0.5;
        //fitness = alpha * expMean + (1 - alpha) * expStd;





















        //fitness = alpha * Math.Exp(-Math.Abs(mean - targetMove))
        //        + (1 - alpha) * Math.Exp(-Math.Abs(stanardDeviation - targetStd));





        //std = 1.0 / (1.0 + Math.Abs(standardDeviation - target_std));
        //double alpha = 0.5;
        //fitness = alpha * (1.0 / (1.0 + Math.Abs(mean - targetMove))) + (1- alpha) * (1.0 / (1.0 + Math.Abs(standardDeviation - targetStd)));









        //double expMean;

        //if (Math.Abs(mean - target_move) < 9)
        //{
        //    string result = string.Format("{0:0.########0}", Math.Exp(-Math.Abs(mean - target_move)));
        //    expMean = (Double.Parse(result));
        //}

        //fitness = 1.0 / (1.0 + Math.Abs(mean - target_move));

        ////else expMean = 0;

        //double squaredDifferencesSum = 0.0;
        //for (int i = 0; i < num_move_container.Count; i++)
        //{
        //    double diff = num_move_container[i] - mean;
        //    squaredDifferencesSum += diff * diff;
        //}

        //double variance = squaredDifferencesSum / num_move_container.Count;
        //double standardDeviation = Math.Sqrt(variance);

        //double expStd;

        //if (standardDeviation < 9)
        //{
        //    string result = string.Format("{0:0.########0}", Math.Exp(-Math.Abs(standardDeviation)));
        //    expStd = (Double.Parse(result));
        //}

        //else expStd = 0;

        //std = standardDeviation;


        //double alpha = 1;

        //fitness = alpha * expMean /*+ (1- alpha) * expStd*/;


    }

    public void calculateInfeasibleFitness()
    {
        fitness = 1.0 / (1.0 + Math.Abs(infeasibleCellCnt));

        //fitness = Math.Exp(-Math.Abs(infeasibleCellCnt));

        //if(infeasibleCellCnt < 9)
        //{
        //    fitness = Math.Exp(-Math.Abs(infeasibleCellCnt));
        //}

        //else fitness = 0;
    }


    public DNA<T> Crossover(DNA<T> otherParent)
	{
		DNA<T> child = new DNA<T>(genes.Length, random, getRandomGene, shouldInitGenes: false);

		for (int i = 0; i < genes.Length; i++)
		{
			child.genes[i] = random.NextDouble() < 0.5 ? genes[i] : otherParent.genes[i];
		}

		return child;
	}

	public void Mutate(float mutationRate)
	{
		for (int i = 0; i < genes.Length; i++)
		{
			if (random.NextDouble() < mutationRate) genes[i] = getRandomGene();
		}
	}
}