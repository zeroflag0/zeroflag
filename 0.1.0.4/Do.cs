using System;

namespace zeroflag.Threading
{
	public static class Do
	{
		/// <summary>
		/// Create an empty task for configuration via []
		/// </summary>
		public static Task Run
		{
			get
			{
				return new Task();
			}
		}

		public static For For
		{
			get
			{
				return new For();
			}
		}
		//public static Task Run(params Task[] tasks)
		//{
		//    return Run(0, tasks).Run();
		//}
		//static Task Run(int index, params Task[] tasks)
		//{
		//    return tasks[index].Then(Run(index + 1, tasks));
		//}

		//public static Task Run(params Task.RunHandle[] tasks)
		//{
		//    return Run(0, tasks).Run();
		//}

		//static Task Run(int index, params Task.RunHandle[] tasks)
		//{
		//    return new Task(tasks[index]).Then(Run(index + 1, tasks));
		//}


		//public static Task For(For task)
		//{
		//    return task.Run();
		//}
	}
}
