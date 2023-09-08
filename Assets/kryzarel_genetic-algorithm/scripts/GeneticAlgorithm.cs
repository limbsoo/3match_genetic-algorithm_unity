using System;
using System.Collections.Generic;

public class GeneticAlgorithm<T>
{
    public int generation = 1;
    public int populationSize = 10;
    public int elitism = 2;
    public float mutationRate = 0.01f;

    public int generationLimit = 100;
    public int repeat = 20;
    public int moveLimit = 200;

    public int targetMove = 30;
    public double targetStd = 15;
    public double targetFitness = 0.9;

    public List<DNA<T>> population;
	public List<DNA<T>> feasiblePopulation;
	public List<DNA<T>> infeasiblePopulation;

    private Random random;
    private int dnaSize;


    public double feasibleFitnessSum;
    public double infeasibleFitnessSum;

    public double feasibleMeanMoveSum;
    public double infeasibleBlockCntSum;

    public double curGenerationBestMean;
    public double curGenerationBestStd;
    public double curGenerationBestFitness;

    public double bestMeanMove;
    public double bestStd;
    public double bestFitness;

    public List<double> feasibleParent;
    public List<int> feasibleParentIdx;
    public List<double> infeasibleParent;
    public List<int> infeasibleParentIdx;
    public List<int> curBestMoves;
    public List<int> bestMoves;
    public List<List<int>> repeatMovements;
    public int repeatMovementsCnt;


    public List<List<int>> allMovements;

    public List<List<int>> obstructionRates;
    public List<List<int>> shorCutRates;

    public List<int> targetNeedCnt;
    public int possibleCnt;
    public bool isPossible;

    public List<int> possibleCountingList;

    public int[] possibleCounting;

    public int blockeCnt = 0;

    public GeneticAlgorithm(int dnaSize, Random random, Func<T> getRandomGene, Func<T[]> GetGenes)
	{
        population = new List<DNA<T>>(populationSize);
        feasiblePopulation = new List<DNA<T>>();
        infeasiblePopulation = new List<DNA<T>>();

		this.random = random;
		this.dnaSize = dnaSize;

        for (int i = 0; i < populationSize; i++)
        {
            population.Add(new DNA<T>(dnaSize, random, getRandomGene, GetGenes, shouldInitGenes: true));
        }
    }

	public void NewGeneration()
	{
        population.Clear();

        //feasibleParent = new List<double>();
        //feasibleParentIdx = new List<int>();
        //infeasibleParent = new List<double>();
        //infeasibleParentIdx = new List<int>();

        if (feasiblePopulation.Count != 0)
		{

            feasiblePopulation.Sort(CompareDNA);

            for (int i = 0; i < feasiblePopulation.Count; i++) feasibleMeanMoveSum += feasiblePopulation[i].mean;

            for (int i = 0; i < feasiblePopulation.Count; i++)
            {
                //if (i < elitism) population.Add(feasiblePopulation[i]);

                //else
                {
                    DNA<T> parent1 = ChooseParentInFeasible();
                    DNA<T> parent2 = ChooseParentInFeasible();
                    DNA<T> child = parent1.Crossover(parent2);
                    child.Mutate(mutationRate);
                    population.Add(child);
                }
            }

            feasibleMeanMoveSum = 0;
            feasibleFitnessSum = 0;
            feasiblePopulation.Clear();
        }

        if (infeasiblePopulation.Count != 0)
        {
            infeasiblePopulation.Sort(CompareDNA);

            for (int i = 0; i < infeasiblePopulation.Count; i++) infeasibleBlockCntSum += infeasiblePopulation[i].infeasibleCellCnt;

            for (int i = 0; i < infeasiblePopulation.Count; i++)
            {
                //if (i < elitism) population.Add(infeasiblePopulation[i]);

                //else
                {
                    DNA<T> parent1 = ChooseParentInInfeasible();
                    DNA<T> parent2 = ChooseParentInInfeasible();
                    DNA<T> child = parent1.Crossover(parent2);
                    child.Mutate(mutationRate);
                    population.Add(child);
                }
            }

            infeasibleBlockCntSum = 0;
            infeasibleFitnessSum = 0;
            infeasiblePopulation.Clear();
        }
        //generation++;
    }


    private int CompareDNAMean(DNA<T> a, DNA<T> b)
	{
        if(Math.Abs(targetMove - a.mean ) < Math.Abs(targetMove - b.mean)) return -1;
        else if (Math.Abs(targetMove - a.mean) > Math.Abs(targetMove - b.mean)) return 1;
        else return 0;
    }

    private int CompareInfeasibleCnt(DNA<T> a, DNA<T> b)
    {
        if (a.infeasibleCellCnt < b.infeasibleCellCnt) return -1;
        else if (a.infeasibleCellCnt > b.infeasibleCellCnt) return 1;
        else return 0;
    }

    private int CompareDNA(DNA<T> a, DNA<T> b)
    {
        //if (Math.Abs(a.fitness - b.fitness) <= 1e-9) return 0;

        //else if (a.fitness < b.fitness) return 1;

        //else return -1;


        if (a.fitness > b.fitness) return -1;
        else if (a.fitness < b.fitness) return 1;
        else return 0;
    }

    private DNA<T> ChooseParentInFeasible()
    {
        double randomNumber = random.NextDouble() * feasibleFitnessSum;

        for (int i = 0; i < feasiblePopulation.Count; i++)
        {
            if (randomNumber < feasiblePopulation[i].fitness)
            {
                //feasibleParent.Add(feasiblePopulation[i].fitness);
                //feasibleParentIdx.Add(i);
                return feasiblePopulation[i];
            }
           
            randomNumber -= feasiblePopulation[i].fitness;
        }

        //double randomNumber = random.NextDouble() * feasibleMeanMoveSum;

        //for (int i = 0; i < feasible_population.Count; i++)
        //{
        //    if (randomNumber < feasible_population[i].mean) return feasible_population[i];
        //    randomNumber -= feasible_population[i].mean;
        //}


        return null;
    }

    private DNA<T> ChooseParentInInfeasible()
    {
        //double randomNumber = random.NextDouble() * infeasibleFitnessSum;

        //for (int i = 0; i < infeasible_population.Count; i++)
        //{
        //    if (randomNumber < infeasible_population[i].fitness) return infeasible_population[i];
        //    randomNumber -= infeasible_population[i].fitness;
        //}

        double randomNumber = random.NextDouble() * infeasibleFitnessSum;

        for (int i = 0; i < infeasiblePopulation.Count; i++)
        {
            if (randomNumber < infeasiblePopulation[i].fitness)
            {
                //infeasibleParent.Add(infeasiblePopulation[i].fitness);
                //infeasibleParentIdx.Add(i);
                return infeasiblePopulation[i];
            }
            
            randomNumber -= infeasiblePopulation[i].fitness;
        }
        return null;
    }
}
