using System;
using Nancy;
using DelayedJob;

namespace Nancy_spam_check
{
	public class SpamModule : NancyModule
	{
		public SpamModule()
		{

			Get ["/"] = parameters => {
				/* Change this to your database */
				//SQLite3
				string connectionString = 
				"URI=file:delayed_job.db";

				//MySQL
				//string connectionString = 
				//	"Data Source=172.16.24.131;Database=delayed_job_test;User ID=;Password=";

				//Microsoft SQL Server
				//string connectionString = 
				//	"Server=172.16.24.141;Database=delayed_job_test;User ID=;Password=";

				/* pick your database */
				//Job.Repository = new DelayedJob.RepositoryMsSQL (connectionString);
				//Job.Repository = new DelayedJob.RepositoryMySQL (connectionstring);
				Job.Repository = new DelayedJob.RepositoryMonoSQLite (connectionString);

				CheckForSpam spam = new CheckForSpam ();
				//Because spam.Text is private it will be stored in the delayed_job database when serialized. 
				spam.text = "Buy this dolo sit amet, consectetur adipiscing elit. Nullam non enim et felis hendrerit " +
					"vestibulum. Vivamus semper aliquam ornare. In id urna est. Aenean pellentesque lacus elit, " +
					"sed bibendum lacus egestas ac. Proin ac sodales tellus. Donec rutrum quis sapien vel porta. " +
					"Fusce posuere dolor du, sed pretium ipsum rutrum id. Vestibulum, porttitor vel adipiscing " +
					"in, venenatis et velit.";

				DelayedJob.Job.Enqueue (spam);

				return "Potential spam, queuieng spam check on text: <br /><i><b>" + spam.text + 
					"</b></i>";

				//now go to your configured worker.exe directory and run
			};
		}
	}

	public class CheckForSpam : DelayedJob.IJob
	{
		//We want this information in the database. 
		//Make sure all information that needs to be persisted in public for serialization
		public string text;

		public void perform(){
			//real word do some run long running algorithm 
			if (text.ToLower ().Contains ("buy this")) {
				Console.WriteLine ("Found some spam!");
			}
		}
	}

}

