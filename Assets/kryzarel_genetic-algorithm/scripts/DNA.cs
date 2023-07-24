using System;
using System.Collections.Generic;

public class DNA<T>
{
	public T[] genes { get; private set; }
	private Random random;
	private Func<T> getRandomGene;
    public bool isFeasible = false;
    public int infeasible_cell_cnt = 0;
	public double fitness;

    public double average_move;
    public double sd;

    public int num_move;
    public int cur_state = 0;
    public bool targetClear = false;
    public bool maxedOut = false;

    public DNA(int size, Random random, Func<T> getRandomGene, bool shouldInitGenes = true)
	{
		genes = new T[size];
		this.random = random;
		this.getRandomGene = getRandomGene;


		if (shouldInitGenes)
		{
			for (int i = 0; i < genes.Length; i++) genes[i] = getRandomGene();
		}
	}

    public void Calculate_feasible_fitness(List<int> num_move_container, int target_move, double target_std)
    {
        int validCount = 0;
        int sum = 0;

        //- CalculateMean ------------------------/

        foreach(var c in num_move_container)
        {
            if(c != 0)
            {
                sum += c;
                validCount++;
            }
        }

        double mean = 0;

        if (sum != 0 || validCount != 0) mean = (double)sum / validCount;

        average_move = mean;

    //- CalculateStandardDeviation ------------------------/

    double sumOfSquaredDifferences = 0;

        foreach (var c in num_move_container)
        {
            if (c != 0) sumOfSquaredDifferences += Math.Pow(c - mean, 2);
        }

        double variance = 0;

        if (sumOfSquaredDifferences != 0 || validCount != 0) variance = sumOfSquaredDifferences / validCount;

        double standardDeviation = Math.Sqrt(variance);

        sd = standardDeviation;

        //- CalculateFitness ------------------------/

        double alpha = 0.5;
        fitness = alpha * Math.Exp(-Math.Abs(mean - target_move))
          + (1 - alpha) * Math.Exp(-Math.Abs(standardDeviation - target_std));
    }

    public void Calculate_infeasible_fitness()
    {
        fitness = Math.Exp(-Math.Abs(infeasible_cell_cnt));
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