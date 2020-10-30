using CavePathFindingCoursework.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CavePathFindingCoursework
{
    class Program
	{

		private static readonly int NO_PARENT = -1;

		// Function that implements Dijkstra's 
		// single source shortest path 
		// algorithm for a graph represented 
		// using adjacency matrix 
		// representation 
		private static void dijkstra(double[,] adjacencyMatrix, int startVertex, List<Tuple<int, int>> coordinates)
		{
			int nVertices = adjacencyMatrix.GetLength(0);

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
				double shortestDistance = int.MaxValue;
				for (int vertexIndex = 0;vertexIndex < nVertices; vertexIndex++)
				{
					if (!added[vertexIndex] && shortestDistances[vertexIndex] < shortestDistance)
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
					double edgeDistance = adjacencyMatrix[nearestVertex, vertexIndex];

					if (edgeDistance > 0 && ((shortestDistance + edgeDistance) < shortestDistances[vertexIndex]))
					{
						parents[vertexIndex] = nearestVertex;
						shortestDistances[vertexIndex] = shortestDistance + edgeDistance;
					}
				}
			}

			printSolution(startVertex, shortestDistances, parents, coordinates);
		}

		// A utility function to print 
		// the constructed distances 
		// array and shortest paths 
		private static void printSolution(int startVertex, double[] distances, int[] parents, List<Tuple<int, int>> coordinates)
		{
			int nVertices = distances.Length;
			Console.Write("Vertex\t Distance\tPath");
					Console.Write("\n" + coordinates[startVertex] + " -> ");
					Console.Write(coordinates[nVertices - 1] + " \t\t ");
					Console.Write(distances[nVertices - 1] + "\t\t");
					printPath(nVertices - 1, parents, coordinates);
		}

		// Function to print shortest path 
		// from source to currentVertex 
		// using parents array 
		private static void printPath(int currentVertex, int[] parents, List<Tuple<int, int>> coordinates)
		{

			// Base case : Source node has 
			// been processed 
			if (currentVertex == NO_PARENT)
			{
				return;
			}
			printPath(parents[currentVertex], parents, coordinates);
			Console.Write(coordinates[currentVertex] + " ");
		}

		// Driver Code 
		public static void Main(String[] args)
		{
			Data data = new Data("generated100-1");

			//data.printMatrix();
			
			/*
			int[,] adjacencyMatrix = { { 0, 4, 0, 0, 0, 0, 0, 8, 0 },
									{ 4, 0, 8, 0, 0, 0, 0, 11, 0 },
									{ 0, 8, 0, 7, 0, 4, 0, 0, 2 },
									{ 0, 0, 7, 0, 9, 14, 0, 0, 0 },
									{ 0, 0, 0, 9, 0, 10, 0, 0, 0 },
									{ 0, 0, 4, 0, 10, 0, 2, 0, 0 },
									{ 0, 0, 0, 14, 0, 2, 0, 1, 6 },
									{ 8, 11, 0, 0, 0, 0, 1, 0, 7 },
									{ 0, 0, 2, 0, 0, 0, 6, 7, 0 } };

			List<Tuple<int, int>> coordinates = new List<Tuple<int, int>>();

			coordinates.Add(Tuple.Create(3,2));
			coordinates.Add(Tuple.Create(4,5));
			coordinates.Add(Tuple.Create(7,6));
			coordinates.Add(Tuple.Create(2,9));
			coordinates.Add(Tuple.Create(9,4));
			coordinates.Add(Tuple.Create(3,4));
			coordinates.Add(Tuple.Create(6,2));
			coordinates.Add(Tuple.Create(1,2));
			coordinates.Add(Tuple.Create(1,8));
			*/

			dijkstra(data.AdjacencyMatrix, 0, data.Coordinates);
		}
	}
}
