using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CavePathFindingCoursework.Classes
{
	/// <summary>
	/// Class <c>Data</c> 
	/// Class that contains the data structure of a cave system
	/// Includes methods that find the quickes path through a cave system. Includes utility methods that help with debugging.
	/// </summary>
	class Data
    {
        //-------------------------------Instance Variables-------------------------------
        private string filename;
        private int numberOfCaverns;
        private List<Tuple<int, int>> coordinates =  new List<Tuple<int, int>>();
        private double[,] adjacencyMatrix;
		private static readonly int NO_PARENT = -1;
		private string quickestPath;

		//-------------------------------Constructors-------------------------------
		public Data(string filename)
        {
            this.filename = filename;
            readFile();
        }

        //-------------------------------Getters/Setters-------------------------------
        public string Filename { get => filename; set => filename = value; }
        public int NumberOfCaverns { get => numberOfCaverns; set => numberOfCaverns = value; }
        public List<Tuple<int, int>> Coordinates { get => coordinates; set => coordinates = value; }
        public double[,] AdjacencyMatrix { get => adjacencyMatrix; }

		//-------------------------------Methods-------------------------------
		/// <summary>
		/// Method <c>readFile</c> 
		/// Reads in a .cav file and creates an adjacency matrix. Replaces 1's in matrix with euclidian distances between the two coordinates.
		/// </summary>
		private void readFile()
        {
			string filepath = String.Format("../../../../{0}.cav", this.filename);
			if (File.Exists(filepath))
			{

				using (StreamReader sr = new StreamReader(@filepath))
				{
					while (sr.Peek() >= 0)
					{
						//Tracks the file position
						int index = 0;

						//First value in row equals number of caverns (n*2)
						this.numberOfCaverns = Int32.Parse(textSeperator(sr));

						//Initialise adjacency matrix using width/height equal to number of caves
						this.adjacencyMatrix = new double[this.numberOfCaverns, this.numberOfCaverns];
						
						//Loop through all the coordinates
						for (int i = 1; i <= (this.numberOfCaverns); i++)
						{
							//Increase index by 1 and get x coord
							index++;
							int x = Int32.Parse(textSeperator(sr));

							//Increase index by 1 and get y coord
							index++;
							int y = Int32.Parse(textSeperator(sr));

							//Add new coordinate to list
							this.coordinates.Add(new Tuple<int, int>(x, y));
						}

						//Loop through every connectivity row until end of file(n*2 iterations)
						int outer;
						//Increae index by 1 
						index++;
						for (outer = 0; outer < this.numberOfCaverns; outer++)
						{
							for (int inner = 0; inner < this.numberOfCaverns; inner++)
							{
								//If value is equal to 1 then replace 1 with euclidian distance between coordinates
								if ((textSeperator(sr)).Equals("1"))
								{
									adjacencyMatrix[inner, outer] = Math.Sqrt((Math.Pow((coordinates[outer].Item1 - coordinates[inner].Item1), 2) + Math.Pow((coordinates[outer].Item2 - coordinates[inner].Item2), 2)));
								}
								else
								{
									//Populate adjacency matrix with 0
									adjacencyMatrix[inner, outer] = 0;
								}
								//Increase index by 1 every inner loop iteration
								index++;
							}
						}
					}
				}
			}
            else
            {
				//Throw no file exists given exception
				throw new Exception(String.Format("{0}.cav does not exist. Try another filename", this.filename));
			}
		}

		/// <summary>
		/// Method <c>writeFile</c> 
		/// Writes the quickest path through a cavern system to a .csn file
		/// </summary>
		public void writeFile()
        {
			string filepath = String.Format("../../../../{0}.csn", this.filename);
			using (StreamWriter writer = new StreamWriter(new FileStream(filepath,FileMode.Create, FileAccess.Write)))
			{
				writer.WriteLine(this.quickestPath.Trim());
			}
		}

		/// <summary>
		/// Method <c>dijkstra</c> 
		/// Implements Dijkstra's algorithm for a graph represented using adjacency matrix representation
		/// </summary>
		public void dijkstra(int startCave)
		{
			int nCaves = this.adjacencyMatrix.GetLength(0);

			//ShortestDistances[i] will hold the shortest distance from source to i 
			double[] shortestDistances = new double[nCaves];

			//Added[i] will true if cave i is included / in shortest path tree or shortest distance from source to i is finalized 
			bool[] added = new bool[nCaves];

			//Initialize all distances as INFINITE and added[] as false 
			for (int caveIndex = 0; caveIndex < nCaves; caveIndex++)
			{
				shortestDistances[caveIndex] = double.MaxValue;
				added[caveIndex] = false;
			}

			//Distance of source cave from itself is always 0 
			shortestDistances[startCave] = 0;

			//Parent array to store shortest path tree 
			int[] parents = new int[nCaves];

			//The starting cave does not have a parent 
			parents[startCave] = NO_PARENT;

			// Find shortest path for all caves
			for (int i = 1; i < nCaves; i++)
			{

				//Pick the minimum distance cave from the set of caves not yet processed. nearestCave is always equal to startNode in first iteration. 
				int nearestCave = -1;
				double shortestDistance = int.MaxValue;
				for (int caveIndex = 0; caveIndex < nCaves; caveIndex++)
				{
					if (!added[caveIndex] && shortestDistances[caveIndex] < shortestDistance)
					{
						nearestCave = caveIndex;
						shortestDistance = shortestDistances[caveIndex];
					}
				}

				//If nothing found then all paths found and break out of loop
				if (nearestCave == -1)
				{
					break;
				}

				//Mark the picked cave as processed 
				added[nearestCave] = true;

				//Update distance value of the adjacent caves of the picked cave. 
				for (int caveIndex = 0; caveIndex < nCaves; caveIndex++)
				{
					double edgeDistance = this.adjacencyMatrix[nearestCave, caveIndex];

					if (edgeDistance > 0 && ((shortestDistance + edgeDistance) < shortestDistances[caveIndex]))
					{
						parents[caveIndex] = nearestCave;
						shortestDistances[caveIndex] = shortestDistance + edgeDistance;
					}
				}
			}

			//Store quickest path as string
			returnPath(nCaves - 1, parents);
			//Print the quickest path to console
			printQuickestPath(startCave, shortestDistances, parents);
		}

		//-------------------------------Utility Methods-------------------------------

		/// <summary>
		/// Utility Method <c>textSeperator</c> 
		/// Takes a streamreader and reads the next character until a comma is reached.
		/// It then returns the string
		/// </summary>
		private string textSeperator(StreamReader sr)
		{
			//Set string to empty
			string text ="";
				
			//Read next character until a comma or end of file is reached 
			while (!((char)sr.Peek()).Equals(',') && sr.Peek() > -1)
			{
				text = String.Format("{0}{1}", text, ((char)sr.Read()));
			}
				
			//If it's not end of file then it's a comma and it needs to be read
			if(!(sr.Peek() ==-1))
            {
				//Read stream reader to skip comma
				sr.Read();
			}

			return text;
		}

		/// <summary>
		/// Utility Method <c>endOfFile</c> 
		/// Checks if it's an end of file
		/// </summary>
		private bool endOfFile(StreamReader sr)
		{
			//If it's end of file then update tuple item2 to return true
			if (sr.Peek() == -1)
			{
				//If end of file then return true
				return true;
			}
            else
            {
				//if not end of file, return false
				return false;
			}
		}

		/// <summary>
		/// Utility Method <c>printMatrix</c> 
		/// Prints the adjacency matrix to console in a readable format. (Good for smaller matrices)
		/// </summary>
		public void printMatrix()
		{
			int outer;
			Console.Write("{\n");
			for (outer = 0; outer < this.numberOfCaverns; outer++)
			{
				for (int inner = 0; inner < this.numberOfCaverns; inner++)
				{
					//Populate adjacency matrix
					Console.Write(String.Format("{0},", adjacencyMatrix[inner, outer].ToString()));
				}
				Console.Write("\n");
			}
			Console.Write("}");
		}

		/// <summary>
		/// Utility Method <c>printSolution</c> 
		/// Prints the shortest path and other information to console
		/// </summary>
		private void printQuickestPath(int startCave, double[] distances, int[] parents)
		{
			int nVertices = distances.Length;
			Console.Write("Cave\t\t Distance\t\tPath");
			Console.Write("\n" + (startCave + 1) + " -> ");
			Console.Write(nVertices + " \t ");
			Console.Write(Math.Round(distances[nVertices - 1],2) + "\t\t\t" + this.quickestPath.Trim());
		}

		/// <summary>
		/// Utility Method <c>returnPath</c> 
		/// Returns the quickest path from source to target in a format which can be read to file. 
		/// </summary>
		private void returnPath(int currentCave, int[] parents)
		{
			//Base-case: current cave equals source 
			if (currentCave == NO_PARENT)
			{
				return;
			}

			//Recursive call
			returnPath(parents[currentCave], parents);

			//String containing path
			this.quickestPath = String.Format("{0} {1}", this.quickestPath, (currentCave+1));
		}
	}
}