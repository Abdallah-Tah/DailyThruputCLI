using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Net.Mail;

namespace DailyThruputCLI
{
    /* This program connects to the SQL Server database and executes a stored procedure usp_UpdateDailyThruput.
     * The stored procedure updates the daily throughput data in the database (ReportsWeb).
     *  If an error occurs during the execution of the stored procedure, an email is sent to the support team.
     *  Author: Abdallah Mohamed developed on 08/05/2024
     */

    class Program
    {
        static async Task Main(string[] args)
        {
            // Define connection string and stored procedure name
            string connectionString = "Server='';Database='';User Id=rundailythruput;Password='';";
            string storedProcedureName = "usp_UpdateDailyThruput";

            // Create SQL connection
            Console.WriteLine("Attempting to connect to database...");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(storedProcedureName, connection);
                command.CommandType = CommandType.StoredProcedure;

                // Adding a parameter to hold the return value
                SqlParameter returnValue = new SqlParameter();
                returnValue.Direction = ParameterDirection.ReturnValue;
                command.Parameters.Add(returnValue);

                try
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Connection successful. Executing stored procedure...");

                    await command.ExecuteNonQueryAsync();

                    // Getting the return value from the stored procedure
                    int result = (int)returnValue.Value;
                    Console.WriteLine($"Stored procedure executed successfully. Return Value: {result}");

                    // Additional message based on return value
                    if (result == 0)
                    {
                        Console.WriteLine("Operation completed without errors.");
                    }
                    else
                    {
                        Console.WriteLine("Operation completed with errors. Check the database for details.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
            Console.WriteLine("Process completed. Press any key to exit.");
            Console.ReadKey();  // Wait for user input before closing console window
        }

        static async Task SendEmailAsync(string subject, string body)
        {
            try
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress("bid.processing@avx.com");

                // Multiple recipients
                var emails = new List<string> { "abdallah.mohamed@kyocera-avx.com", "matt.pettine@kyocerax-avx.com" };
                foreach (var email in emails)
                {
                    message.To.Add(new MailAddress(email));
                }

                message.Subject = subject;
                message.Body = body;

                using (SmtpClient client = new SmtpClient("10.1.1.82"))
                {
                    client.Port = 25; 
                    client.EnableSsl = false; 
                    await client.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to send email: " + ex.Message);
            }
        }

    }
}
