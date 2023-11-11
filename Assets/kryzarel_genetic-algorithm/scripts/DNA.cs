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

public class Pottential
{
    public int map;
    public int notObstacle;

    public int onlyObstacle;

    public int onlyAllBlocked;
    public int onlyBlocked1;
    public int onlyBlocked2;
    public int onlyBlocked3;

    public int onlyAllOverlay;
    public int onlyOverlay1;
    public int onlyOverlay2;
    public int onlyOverlay3;

    public int obstacleAndAllBlocked;
    public int obstacleAndBlocked1;
    public int obstacleAndBlocked2;
    public int obstacleAndBlocked3;

    public int obstacleAndAllOverlay;
    public int obstacleAndOverlay1;
    public int obstacleAndOverlay2;
    public int obstacleAndOverlay3;


    public Pottential()
    {
        map = 0;
        notObstacle = 0;
        onlyObstacle = 0;

        onlyAllBlocked = 0;
        onlyBlocked1 = 0;
        onlyBlocked2 = 0;
        onlyBlocked3 = 0;

        onlyAllOverlay = 0;
        onlyOverlay1 = 0;
        onlyOverlay2 = 0;
        onlyOverlay3 = 0;

        obstacleAndAllBlocked = 0;
        obstacleAndBlocked1 = 0;
        obstacleAndBlocked2 = 0;
        obstacleAndBlocked3 = 0;

        obstacleAndAllOverlay = 0;
        obstacleAndOverlay1 = 0;
        obstacleAndOverlay2 = 0;
        obstacleAndOverlay3 = 0;
    }
}

public class AllPottentials
{


    public double map;
    public double notObstacle;

    public double obstacle;
    public double blocked1;
    public double blocked2;
    public double blocked3;
    public double blocked4;
    public double overlay1;
    public double overlay2;
    public double overlay3;
    public double overlay4;
    public double somethingWrong;



    public AllPottentials()
    {
        map = 0;
        notObstacle = 0;

        obstacle = 0;
        blocked1 = 0;
        blocked2 = 0;
        blocked3 = 0;
        blocked4 = 0;
        overlay1 = 0;
        overlay2 = 0;
        overlay3 = 0;
        overlay4 = 0;
        somethingWrong = 0;

    }
}


//public class SwapablePottential
//{
//    public int map;
//    public int notObstacle;

//    public int onlyObstacle;

//    public int onlyAllBlocked;
//    public int onlyBlocked1;
//    public int onlyBlocked2;
//    public int onlyBlocked3;

//    public int onlyAllOverlay;
//    public int onlyOverlay1;
//    public int onlyOverlay2;
//    public int onlyOverlay3;

//    public int obstacleAndAllBlocked;
//    public int obstacleAndBlocked1;
//    public int obstacleAndBlocked2;
//    public int obstacleAndBlocked3;

//    public int obstacleAndAllOverlay;
//    public int obstacleAndOverlay1;
//    public int obstacleAndOverlay2;
//    public int obstacleAndOverlay3;


//    public SwapablePottential()
//    {
//        map = 0;
//        notObstacle = 0;

//        onlyObstacle = 0;

//        onlyAllBlocked = 0;
//        onlyBlocked1 = 0;
//        onlyBlocked2 = 0;
//        onlyBlocked3 = 0;

//        onlyAllOverlay = 0;
//        onlyOverlay1 = 0;
//        onlyOverlay2 = 0;
//        onlyOverlay3 = 0;

//        obstacleAndAllBlocked = 0;
//        obstacleAndBlocked1 = 0;
//        obstacleAndBlocked2 = 0;
//        obstacleAndBlocked3 = 0;

//        obstacleAndAllOverlay = 0;
//        obstacleAndOverlay1 = 0;
//        obstacleAndOverlay2 = 0;
//        obstacleAndOverlay3 = 0;
//    }

//}






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



    //public List<int> cellsID;
    //public List<int> kindsOfObstacle;

    public int matchFromMap;
    public int nearBreakableObstacles;
    public int includeMatchObstacles;

    //public List<Block> blocks;

    public Block[] blocks;

    //public SwapablePottential swapablePottential;
    public AllPottentials allPottential;
    public Pottential swapablePottential;

    public List<int> gridObjects;
    public List<int> objectProtection;



    //public int obstacle;
    //public int blocked1;
    //public int blocked2;
    //public int blocked3;
    //public int overlay1;
    //public int overlay2;
    //public int overlay3;
    //public int somethingWrong;


    //public int mapObstacle;
    //public int mapBlocked1;
    //public int mapBlocked2;
    //public int mapBlocked3;
    //public int mapOverlay1;
    //public int mapOverlay2;
    //public int mapOverlay3;
    //public int mapSomethingWrong;






    public DNA(int size, Random random, Func<T> getRandomGene, Func<T[]> getGenes, bool getSetGenes, bool shouldInitGenes = true)
	{
        gridObjects = new List<int>();
        objectProtection = new List<int>();


        genes = new T[size];
		this.random = random;
        this.getGenes = getGenes;
        this.getRandomGene = getRandomGene;

        if (shouldInitGenes)
        {
            //for (int i = 0; i < size; i++) genes[i] = getRandomGene();

            if (getSetGenes) genes = getGenes();

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
        //cellsID = new List<int>();
        //kindsOfObstacle = new List<int>();



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




        //obstacle = 0;
        //blocked1 = 0;
        //blocked2 = 0;
        //blocked3 = 0;
        //overlay1 = 0;
        //overlay2 = 0;
        //overlay3 = 0;
        //somethingWrong = 0;



        //mapObstacle = 0;
        //mapBlocked1 = 0;
        //mapBlocked2 = 0;
        //mapBlocked3 = 0;
        //mapOverlay1 = 0;
        //mapOverlay2 = 0;
        //mapOverlay3 = 0;
        //mapSomethingWrong = 0;

    }


    public void calculateFitennnsssssss(int correct)
    {
        fitness = Math.Abs(10 - correct);

        fitness = 1.0 / (1.0 + Math.Abs(fitness));
        

    }


    public void calculateFeasibleFitness(int wantDifficulty, int difficultyTolerance)
    {
        fitness = Math.Abs(allPottential.map - (double)wantDifficulty);
        //fitness = Math.Abs(swapablePottential.map - wantDifficulty);

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
                //child.cellsID.Add(cellsID[i]);
            }

            else
            {
                child.genes[i] = otherParent.genes[i];
                //child.cellsID.Add(otherParent.cellsID[i]);
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


