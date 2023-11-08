using Mkey;
using System;
using System.Collections.Generic;

public class GeneticAlgorithm<T>
{
    public int geneticGeneration;
    public int generation;

    public int populationSize;
    public int elitism;
    public float mutationRate;
    public int targetMove;
    public double targetStd;
    public double targetFitness;

    public List<DNA<T>> population;
    public List<DNA<T>> feasiblePopulation;
    public List<DNA<T>> infeasiblePopulation;

    private Random random;
    private int dnaSize;

    public double bestFitness;
    public double feasibleFitnessSum;
    public double infeasibleFitnessSum;

    public int repeat = 1; // 왜 있는거지

    public int bestPottential;

    public GeneticAlgorithm(int cellSize, Random random, Func<T> getRandomGene, Func<T[]> getGenes, Match3Helper m3h)
    {
        geneticGeneration = 1;
        generation = 1;

        populationSize = 20;
        elitism = 2;
        mutationRate = 0.01f;
        targetMove = 0;
        targetStd = 0;
        targetFitness = 1;

        population = new List<DNA<T>>(populationSize);
        feasiblePopulation = new List<DNA<T>>();
        infeasiblePopulation = new List<DNA<T>>();

        this.random = random;
        dnaSize = cellSize;

        bestFitness = 0;
        feasibleFitnessSum = 0;
        infeasibleFitnessSum = 0;

        bestPottential = 0;

        for (int i = 0; i < populationSize; i++)
        {
            population.Add(new DNA<T>(dnaSize, random, getRandomGene, getGenes, m3h.getSetGenes, shouldInitGenes: true));
        }
    }

    public void newNEWEWEWEWEGeneration()
    {
        population.Clear();

        if (feasiblePopulation.Count != 0)
        {
            feasiblePopulation.Sort(CompareDNA);

            for (int i = 0; i < feasiblePopulation.Count; i++)
            {
                if (i < elitism) population.Add(feasiblePopulation[i]);

                else
                {
                    DNA<T> parent1 = ChooseParentInFeasible();
                    DNA<T> parent2 = ChooseParentInFeasible();
                    DNA<T> child = parent1.Crossover(parent2);
                    //child.Mutate(mutationRate);
                    population.Add(child);
                }
            }

            feasibleFitnessSum = 0;
            feasiblePopulation.Clear();
        }

        if (infeasiblePopulation.Count != 0)
        {
            infeasiblePopulation.Sort(CompareDNA);

            for (int i = 0; i < infeasiblePopulation.Count; i++)
            {
                if (i < elitism) population.Add(infeasiblePopulation[i]);

                else
                {
                    DNA<T> parent1 = ChooseParentInInfeasible();
                    DNA<T> parent2 = ChooseParentInInfeasible();
                    DNA<T> child = parent1.Crossover(parent2);
                    //child.Mutate(mutationRate);
                    population.Add(child);
                }
            }

            infeasibleFitnessSum = 0;
            infeasiblePopulation.Clear();
        }
        geneticGeneration++;
    }


    public void newGeneration()
    {
        population.Clear();

        if (feasiblePopulation.Count != 0)
        {
            feasiblePopulation.Sort(CompareDNA);

            for (int i = 0; i < feasiblePopulation.Count; i++)
            {
                if (i < elitism) population.Add(feasiblePopulation[i]);

                else
                {
                    DNA<T> parent1 = ChooseParentInFeasible();
                    DNA<T> parent2 = ChooseParentInFeasible();
                    DNA<T> child = parent1.Crossover(parent2);
                    child.Mutate(mutationRate);
                    population.Add(child);
                }
            }

            feasibleFitnessSum = 0;
            feasiblePopulation.Clear();
        }

        if (infeasiblePopulation.Count != 0)
        {
            infeasiblePopulation.Sort(CompareDNA);

            for (int i = 0; i < infeasiblePopulation.Count; i++)
            {
                if (i < elitism) population.Add(infeasiblePopulation[i]);

                else
                {
                    DNA<T> parent1 = ChooseParentInInfeasible();
                    DNA<T> parent2 = ChooseParentInInfeasible();
                    DNA<T> child = parent1.Crossover(parent2);
                    child.Mutate(mutationRate);
                    population.Add(child);
                }
            }

            infeasibleFitnessSum = 0;
            infeasiblePopulation.Clear();
        }
        geneticGeneration++;
    }


    private int CompareDNA(DNA<T> a, DNA<T> b)
    {
        if (a.fitness > b.fitness) return -1;
        else if (a.fitness < b.fitness) return 1;
        else return 0;
    }

    private DNA<T> ChooseParentInFeasible()
    {
        double randomNumber = random.NextDouble() * feasibleFitnessSum;

        for (int i = 0; i < feasiblePopulation.Count; i++)
        {
            if (randomNumber < feasiblePopulation[i].fitness) return feasiblePopulation[i];
            randomNumber -= feasiblePopulation[i].fitness;
        }
        return null;
    }

    private DNA<T> ChooseParentInInfeasible()
    {
        double randomNumber = random.NextDouble() * infeasibleFitnessSum;

        for (int i = 0; i < infeasiblePopulation.Count; i++)
        {
            if (randomNumber < infeasiblePopulation[i].fitness) return infeasiblePopulation[i];
            randomNumber -= infeasiblePopulation[i].fitness;
        }
        return null;
    }




}
