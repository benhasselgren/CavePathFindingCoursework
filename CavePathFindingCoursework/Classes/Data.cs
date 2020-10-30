using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CavePathFindingCoursework.Classes
{
    class Data
    {
        //-------------------------------Instance Variables-------------------------------
        private string filename;
        private int numberOfCaverns;
        private List<Tuple<int, int>> coordinates =  new List<Tuple<int, int>>();
        private int[,] adjacencyMatrix;
		private static readonly int NO_PARENT = -1;

		//-------------------------------Constructor-------------------------------
		public Data(string filename)
        {
            this.filename = filename;
            readFile();
        }

        //-------------------------------Getters/Setters-------------------------------
        public string Filename { get => filename; set => filename = value; }
        public int NumberOfCaverns { get => numberOfCaverns; set => numberOfCaverns = value; }
        public List<Tuple<int, int>> Coordinates { get => coordinates; set => coordinates = value; }
        public int[,] AdjacencyMatrix { get => adjacencyMatrix; set => adjacencyMatrix = value; }


        //-------------------------------Methods-------------------------------
        private void readFile()
        {
            using (TextFieldParser parser = new TextFieldParser(@String.Format("../../../../{0}.cav", this.filename)))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    //Tracks the file position
                    int index = 0;

                    //Processing row
                    string[] fields = parser.ReadFields();

                    //First value in row equals number of caverns (n*2)
                    this.numberOfCaverns = Int32.Parse(fields[index]);

                    //Initialise adjacency matrix using width/height equal to number of caves
                    this.adjacencyMatrix = new int[this.numberOfCaverns, this.numberOfCaverns];

                    //Loop through all the coordinates
                    for(int i=1; i<=(this.numberOfCaverns); i++)
                    {
                        //Increase index by 1 and get x coord
                        index++;
                        int x = Int32.Parse(fields[index]);

                        //Increase index by 1 and get y coord
                        index++;
                        int y = Int32.Parse(fields[index]);

                        //Add new coordinate to list
                        this.coordinates.Add(new Tuple<int, int>(x,y));
                    }

                    //Loop through every connectivity row until end of file(n*2 iterations)
                    int outer;
                    //Increae index by 1 every outer loop iteration
                    index++;
                    for (outer = 0; outer<this.numberOfCaverns; outer++)
                    {      
                        for(int inner=0; inner<this.numberOfCaverns; inner++)
                        {
                            //Populate adjacency matrix
                            adjacencyMatrix[outer, inner] = Int32.Parse(fields[index]);
                            //Increae index by 1 every inner loop iteration
                            index++;
                        }
                    }

                    Console.Write("Finished reading file\n");
                }
            }
        }

        public void printMatrix()
        {
            int outer;
            Console.Write("{\n");
            for (outer = 0; outer < this.numberOfCaverns; outer++)
            {
                for (int inner = 0; inner < this.numberOfCaverns; inner++)
                {
                    //Populate adjacency matrix
                    Console.Write(String.Format("{0},", adjacencyMatrix[outer, inner].ToString()));
                }
                Console.Write("\n");
            }
            Console.Write("}");
        }

		// Function that implements Dijkstra's 
		// single source shortest path 
		// algorithm for a graph represented 
		// using adjacency matrix 
		// representation 
		private static void dijkstra(int[,] adjacencyMatrix, int startVertex, string[] names)
		{
			int nVertices = adjacencyMatrix.GetLength(0);

			// shortestDistances[i] will hold the 
			// shortest distance from src to i 
			int[] shortestDistances = new int[nVertices];

			// added[i] will true if vertex i is 
			// included / in shortest path tree 
			// or shortest distance from src to 
			// i is finalized 
			bool[] added = new bool[nVertices];

			// Initialize all distances as 
			// INFINITE and added[] as false 
			for (int vertexIndex = 0; vertexIndex < nVertices; vertexIndex++)
			{
				shortestDistances[vertexIndex] = int.MaxValue;
				added[vertexIndex] = false;
			}

			// Distance of source vertex from 
			// itself is always 0 
			shortestDistances[startVertex] = 0;

			// Parent array to store shortest 
			// path tree 
			int[] parents = new int[nVertices];

			// The starting vertex does not 
			// have a parent 
			parents[startVertex] = NO_PARENT;

			// Find shortest path for all 
			// vertices 
			for (int i = 1; i < nVertices; i++)
			{

				// Pick the minimum distance vertex 
				// from the set of vertices not yet 
				// processed. nearestVertex is 
				// always equal to startNode in 
				// first iteration. 
				int nearestVertex = -1;
				int shortestDistance = int.MaxValue;
				for (int vertexIndex = 0;
						vertexIndex < nVertices;
						vertexIndex++)
				{
					if (!added[vertexIndex] &&
						shortestDistances[vertexIndex] <
						shortestDistance)
					{
						nearestVertex = vertexIndex;
						shortestDistance = shortestDistances[vertexIndex];
					}
				}

				// Mark the picked vertex as 
				// processed 
				added[nearestVertex] = true;

				// Update dist value of the 
				// adjacent vertices of the 
				// picked vertex. 
				for (int vertexIndex = 0;
						vertexIndex < nVertices;
						vertexIndex++)
				{
					int edgeDistance = adjacencyMatrix[nearestVertex, vertexIndex];

					if (edgeDistance > 0 && ((shortestDistance + edgeDistance) < shortestDistances[vertexIndex]))
					{
						parents[vertexIndex] = nearestVertex;
						shortestDistances[vertexIndex] = shortestDistance + edgeDistance;
					}
				}
			}

			printSolution(startVertex, shortestDistances, parents, names);
		}

		// A utility function to print 
		// the constructed distances 
		// array and shortest paths 
		private static void printSolution(int startVertex, int[] distances, int[] parents, string[] names)
		{
			int nVertices = distances.Length;
			Console.Write("Vertex\t Distance\tPath");
			Console.Write("\n" + names[startVertex] + " -> ");
			Console.Write(names[nVertices - 1] + " \t\t ");
			Console.Write(distances[nVertices - 1] + "\t\t");
			printPath(nVertices - 1, parents, names);
		}

		// Function to print shortest path 
		// from source to currentVertex 
		// using parents array 
		private static void printPath(int currentVertex, int[] parents, string[] names)
		{

			// Base case : Source node has 
			// been processed 
			if (currentVertex == NO_PARENT)
			{
				return;
			}
			printPath(parents[currentVertex], parents, names);
			Console.Write(names[currentVertex] + " ");
		}
	}
}
