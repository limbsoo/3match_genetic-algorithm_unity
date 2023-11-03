using Mkey;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

//using UnityEngine;

public class Block
{
    public int kind;
    public int cnt;
    public int protection;
    public int pottential;

    public Block()
    {
        kind = 0;
        cnt = 0;
        protection = 0;
        pottential = 0;
    }

}
public class DNA<T>
{
	public T[] genes { get; private set; }
	private System.Random random;
    private Func<T[]> getGenes;
    private Func<T> getRandomGene;

    public bool isFeasible = false;
    public int infeasibleCellCnt;



    public int mapMatchPotential;
    public int includeMatchObstacle;
    public List<int> mapMatchPotentialList;

    public int swapCnt;
    public int matchCnt;
    public double fitness;

    public int obstacleCnt;

    public int breakableObstacle;
    public List<GridCell> breakableObstacleCellList;


    public int obstaclePottentialCnt;
    public int blockedPottentialCnt;
    public int overlayPottentialCnt;
    public int notObstacleCnt;

    public List<int> cellsID;
    public List<int> kindsOfObstacle;

    public int matchFromMap;
    public int nearBreakableObstacles;
    public int includeMatchObstacles;

    //public List<Block> blocks;

    public Block[] blocks;

    public DNA(int size, Random random, Func<T> getRandomGene, Func<T[]> getGenes, bool getSetGenes, bool shouldInitGenes = true)
	{
		genes = new T[size];
		this.random = random;
        this.getGenes = getGenes;
        this.getRandomGene = getRandomGene;

        if (shouldInitGenes)
        {
            if(getSetGenes) genes = getGenes();

            else
            {
                for (int i = 0; i < size; i++) genes[i] = getRandomGene();
            }

        }
        

        isFeasible = false;
        infeasibleCellCnt = 0;

        includeMatchObstacle = 0;
        mapMatchPotential = 0;
        mapMatchPotentialList = new List<int>();

        swapCnt = 0;
        matchCnt = 0;
        fitness = 0;

        obstacleCnt = 0;

        breakableObstacle = 0;
        breakableObstacleCellList = new List<GridCell>();

        obstaclePottentialCnt = 0;
        blockedPottentialCnt = 0;
        overlayPottentialCnt = 0;
        notObstacleCnt = 0;
        cellsID = new List<int>();
        kindsOfObstacle = new List<int>();



        matchFromMap = 0; //전체 원래 사이즈 그 맵 크기 뭐시기 나중에 측정해야할듯
        nearBreakableObstacles = 0;
        includeMatchObstacles = 0;

        blocks = new Block[6];

        //blocks = new List<Block>(6);

        for(int i = 1; i <= 3; i++)
        {
            blocks[i - 1] = new Block();
            blocks[i - 1].kind = 1;
            blocks[i - 1].protection = i;
        }

        for (int i = 1; i <= 3; i++)
        {
            blocks[i + 2] = new Block();
            blocks[i + 2].kind = 2;
            blocks[i + 2].protection = i;
        }
    }


    public void calculateFitennnsssssss(int correct)
    {
        fitness = Math.Abs(10 - correct);

        fitness = 1.0 / (1.0 + Math.Abs(fitness));
        

    }


    public void calculateFeasibleFitness(int wantDifficulty, int difficultyTolerance)
    {
        fitness = Math.Abs(mapMatchPotential - wantDifficulty);

        if (fitness > difficultyTolerance) fitness = 1.0 / (1.0 + Math.Abs(fitness));
        else fitness = 1;

    }
    public void calculateInfeasibleFitness()
    {
        fitness = 1.0 / (1.0 + Math.Abs(infeasibleCellCnt));
    }
    public DNA<T> Crossover(DNA<T> otherParent)
	{
		DNA<T> child = new DNA<T>(genes.Length, random, getRandomGene, getGenes, false, shouldInitGenes: false);

		for (int i = 0; i < genes.Length; i++)
		{

            if(random.NextDouble() < 0.5)
            {
                child.genes[i] = genes[i];
                child.cellsID.Add(cellsID[i]);
            }

            else
            {
                child.genes[i] = otherParent.genes[i];
                child.cellsID.Add(otherParent.cellsID[i]);
            }

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


