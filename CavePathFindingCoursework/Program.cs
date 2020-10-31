using CavePathFindingCoursework.Classes;
using System;

namespace CavePathFindingCoursework
{
	/// <summary>
	/// Class <c>CavePathFindingCoursework</c> 
	/// Class that contains main method (driver code) to execute the program
	/// Reads in a .cav file and returns the  quickes path from the start cavern to the last cavern
	/// </summary>
	class Program
	{
		//-------------------------------Methods-------------------------------
		/// <summary>
		/// Method <c>Main</c> 
		/// Main method that executes code.
		/// </summary>
		public static void Main(String[] args)
		{
			//If there is an input then length will be >0
			int isInput = args.Length;

			try
			{
				if (isInput >= 1)
				{
					//Create new savern path structure, passing in the filename that contains all the data.
					Data data = new Data(args[0]);

					//data.printMatrix();

					//Run dijkstra algorithm passing in start node which in this case is the first node (0)
					data.dijkstra(0);

					//Write quickest path to file
					data.writeFile();
				}
				else
				{
					//Throw no filename given exception
					throw new Exception("No filename given. Please add file name.");
				}
			}
            catch(Exception ex)
			{
				Console.Write(String.Format("ERROR: {0}",ex.Message));
			}
		}
	}
}