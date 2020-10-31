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

				using (TextFieldParser parser = new TextFieldParser(@filepath))
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
						this.adjacencyMatrix = new double[this.numberOfCaverns, this.numberOfCaverns];

						//Loop through all the coordinates
						for (int i = 1; i <= (this.numberOfCaverns); i++)
						{
							//Increase index by 1 and get x coord
							index++;
							int x = Int32.Parse(fields[index]);

							//Increase index by 1 and get y coord
							index++;
							int y = Int32.Parse(fields[index]);

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
								if (fields[index].Equals("1"))
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
		public void dijkstra(int startVertex)
		{
			int nVertices = this.adjacencyMatrix.GetLength(0);

			// shortestDistances[i] will hold the 
			// shortest distance from src to i 
			double[] shortestDistances = new double[nVertices];

			// added[i] will true if vertex i is 
			// included / in shortest path tree 
			// or shortest distance from src to 
			// i is finalized 
			bool[] added = new bool[nVertices];

			// Initialize all distances as 
			// INFINITE and added[] as false 
			for (int vertexIndex = 0; vertexIndex < nVertices; vertexIndex++)
			{
				shortestDistances[vertexIndex] = double.MaxValue;
				added[vertexIndex] = false;
			}

			// Distance of source vertex from 
			// itself   is always 0 
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
				double shortestDistance = int.MaxValue;
				for (int vertexIndex = 0; vertexIndex < nVertices; vertexIndex++)
				{
					if (!added[vertexIndex] && shortestDistances[vertexIndex] < shortestDistance)
					{
						nearestVertex = vertexIndex;
						shortestDistance = shortestDistances[vertexIndex];
					}
				}

				//If nothing found then all paths found and break out of loop
				if (nearestVertex == -1)
				{
					break;
				}

				// Mark the picked vertex as 
				// processed 
				added[nearestVertex] = true;

				// Update dist value of the 
				// adjacent vertices of the 
				// picked vertex. 
				for (int vertexIndex = 0; vertexIndex < nVertices; vertexIndex++)
				{
					double edgeDistance = this.adjacencyMatrix[nearestVertex, vertexIndex];

					if (edgeDistance > 0 && ((shortestDistance + edgeDistance) < shortestDistances[vertexIndex]))
					{
						parents[vertexIndex] = nearestVertex;
						shortestDistances[vertexIndex] = shortestDistance + edgeDistance;
					}
				}
			}

			//Store quickest path as string
			returnPath(nVertices - 1, parents);
			//Print the quickest path to console
			printQuickestPath(startVertex, shortestDistances, parents);
		}

		//-------------------------------Utility Methods-------------------------------

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
		private void printQuickestPath(int startVertex, double[] distances, int[] parents)
		{
			int nVertices = distances.Length;
			Console.Write("Vertex\t\t Distance\t\tPath");
			Console.Write("\n" + (startVertex + 1) + " -> ");
			Console.Write(nVertices + " \t ");
			Console.Write(Math.Round(distances[nVertices - 1],2) + "\t\t\t" + this.quickestPath.Trim());
		}

		/// <summary>
		/// Utility Method <c>returnPath</c> 
		/// Returns the quickest path from source to target in a format which can be read to file. 
		/// </summary>
		private void returnPath(int currentVertex, int[] parents)
		{
			//Base-case: current cave equals source 
			if (currentVertex == NO_PARENT)
			{
				return;
			}

			//Recursive call
			returnPath(parents[currentVertex], parents);

			//string containing path
			this.quickestPath = String.Format("{0} {1}", this.quickestPath, (currentVertex+1));
		}
	}
}