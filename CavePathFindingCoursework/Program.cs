using CavePathFindingCoursework.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CavePathFindingCoursework
{
	/// <summary>
	/// Class <c>Program</c> 
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
			//Create new savern path structure, passing in the filename that contains all the data.
			Data data = new Data("generated500-1");

			//data.printMatrix();

			//Run dijkstra algorithm passing in start node which in this case is the first node (0)
			data.dijkstra(0);
		}
	}
}