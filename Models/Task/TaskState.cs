namespace bugtracker.Models
{
	public static class TaskState {
		public static string Pending { get { return "pending"; } }
		public static string InProgress { get { return "inProgress"; } }
		public static string Completed { get { return "completed"; } }
	}
}