using System;
using System.Collections.Generic;

public class GeneticAlgorithm<T>
{
    public int populationSize = 10;
    public int generation = 0;
    public int elitism = 2;
    public float mutationRate = 0.01f;

    public int generation_limit = 20;
    public int repetition_limit = 5;
    public int move_limit = 100;


    public int target_move = 15;
    public double target_std = 0.5;
    public double target_fitness = 0.9;

    public List<DNA<T>> population;
	public List<DNA<T>> feasible_population;
	public List<DNA<T>> infeasible_population;

    private Random random;
    private int dnaSize;
    public double bestFitness;
    public double feasibleFitnessSum;
    public double infeasibleFitnessSum;

    public GeneticAlgorithm(int dnaSize, Random random, Func<T> getRandomGene)
	{
        population = new List<DNA<T>>(populationSize);
        feasible_population = new List<DNA<T>>();
        infeasible_population = new List<DNA<T>>();

		this.random = random;
		this.dnaSize = dnaSize;

        for (int i = 0; i < populationSize; i++)
        {
            population.Add(new DNA<T>(dnaSize, random, getRandomGene, shouldInitGenes: true));
        }
    }

	public void NewGeneration()
	{
        population.Clear();

		if (feasible_population.Count != 0)
		{
            feasible_population.Sort(CompareDNA);

            for (int i = 0; i < feasible_population.Count; i++)
            {
                if (i < elitism) population.Add(feasible_population[i]);

                else
                {
                    DNA<T> parent1 = ChooseParent_in_feasible();
                    DNA<T> parent2 = ChooseParent_in_feasible();
                    DNA<T> child = parent1.Crossover(parent2);
                    child.Mutate(mutationRate);
                    population.Add(child);
                }
            }
            feasibleFitnessSum = 0;
            feasible_population.Clear();
        }

        if (infeasible_population.Count != 0)
        {
            infeasible_population.Sort(CompareDNA);

            for (int i = 0; i < infeasible_population.Count; i++)
            {
                if (i < elitism) population.Add(infeasible_population[i]);

                else
                {
                    DNA<T> parent1 = ChooseParent_in_infeasible();
                    DNA<T> parent2 = ChooseParent_in_infeasible();
                    DNA<T> child = parent1.Crossover(parent2);
                    child.Mutate(mutationRate);
                    population.Add(child);
                }
            }
            infeasibleFitnessSum = 0;
            infeasible_population.Clear();
        }
        generation++;
    }


    private int CompareDNA(DNA<T> a, DNA<T> b)
	{
		if (a.fitness > b.fitness) return -1;
		else if (a.fitness < b.fitness) return 1;
		else return 0;
	}

    private DNA<T> ChooseParent_in_feasible()
    {
        double randomNumber = random.NextDouble() * feasibleFitnessSum;

        for (int i = 0; i < feasible_population.Count; i++)
        {
            if (randomNumber < feasible_population[i].fitness) return feasible_population[i];
            randomNumber -= feasible_population[i].fitness;
        }
        return null;
    }

    private DNA<T> ChooseParent_in_infeasible()
    {
        double randomNumber = random.NextDouble() * infeasibleFitnessSum;

        for (int i = 0; i < infeasible_population.Count; i++)
        {
            if (randomNumber < infeasible_population[i].fitness) return infeasible_population[i];
            randomNumber -= infeasible_population[i].fitness;
        }
        return null;
    }
}
