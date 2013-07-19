
namespace JobTest
{
	using System;
	using System.Web;
	using System.Web.UI;
	using DelayedJob;
	using System.Net;
	using System.Net.Mail;
	using System.Net.Security;
	using System.Security.Cryptography.X509Certificates;

	/* For setup and information on delayed_job.net please see the wiki:
	 * https://github.com/fritzcoder/delayed_job.net/wiki
	 */


	public partial class Default : System.Web.UI.Page
	{
		
		public void button1Clicked (object sender, EventArgs args)
		{
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

			EmailJob me = new EmailJob ();
			DelayedJob.Job.Enqueue (me);
			button1.Text = "Email queued";

			//now go to your configured worker.exe directory and run
		}
	}

	//Send an email through gmail
	public class EmailJob : DelayedJob.IJob
	{
		public string fromAddress = "@gmail.com";
		public string toAddress = "@gmail.com";
		public const string fromPassword = "password";
		public const string subject = "delayed_job.net test";
		public string body = "A test from delayed_job.net";

		public void perform(){
			var smtp = new SmtpClient
			{
				Host = "smtp.gmail.com",
				Port = 587,
				EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(fromAddress, fromPassword)
			};
			using (var message = new MailMessage(fromAddress, toAddress)
			       {
				Subject = subject,
				Body = body
			})
			{

				ServicePointManager.ServerCertificateValidationCallback = 
					delegate(object s, X509Certificate certificate, X509Chain chain, 
					         SslPolicyErrors sslPolicyErrors) 
				{ return true; };
				smtp.Send(message);
			}

			System.Threading.Thread.Sleep (1000 * 10);
		}
	}

}

