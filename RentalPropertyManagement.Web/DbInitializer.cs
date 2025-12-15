using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;

public static class DbInitializer
{
    public static void Initialize(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            var commandText = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Messages')
                BEGIN
                    CREATE TABLE Messages (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        SenderId INT NOT NULL,
                        ReceiverId INT NOT NULL,
                        Content NVARCHAR(MAX) NOT NULL,
                        Timestamp DATETIME NOT NULL DEFAULT GETDATE(),
                        IsRead BIT NOT NULL DEFAULT 0,
                        CONSTRAINT FK_Messages_Sender FOREIGN KEY (SenderId) REFERENCES Users(Id),
                        CONSTRAINT FK_Messages_Receiver FOREIGN KEY (ReceiverId) REFERENCES Users(Id)
                    );
                END
            ";

            using (var command = new SqlCommand(commandText, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}
