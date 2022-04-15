using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeCaseStudyAzureFunctions.Common
{
    public class AzureCosmoDBActivity
    {
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = "https://localhost:8081";
        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;

        // The name of the database and container we will create
        private string databaseId = "MyLearning";
        // private string containerId = "StudentsContainer";
        private string containerId = "StudentsLatest";

        public async Task InitiateConnection()
        {
            // Create a new instance of the Cosmos Client 
            //configuring Azure Cosmosdb sql api details
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            await CreateDatabaseAsync();
            await CreateContainerAsync();
        }

        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        }


        /// <summary>
        /// Create the container if it does not exist. 
        /// Specifiy "/StudentName" as the partition key since we're storing Student information, to ensure good distribution of requests and storage.
        /// </summary>
        /// <returns></returns>
        private async Task CreateContainerAsync()
        {
            // Create a new container
            //this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/StudentName");
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/RollNumber");
        }

        public async Task<ItemResponse<Student>> SaveNewStudentItem(Student objStudent)
        {
            ItemResponse<Student> studentResponse = null;
            try
            {
                //  studentResponse = await this.container.CreateItemAsync<Student>(objStudent, new PartitionKey(objStudent.Name));
                studentResponse = await this.container.CreateItemAsync<Student>(objStudent, new PartitionKey(objStudent.RollNumber));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
            return studentResponse;
        }

        public async Task<ItemResponse<Student>> ModifyStudentItem(Student objStudent)
        {
            ItemResponse<Student> studentResponse = null;
            try
            {
                /* Note : Partition Key value should not change */
                // studentResponse = await this.container.ReplaceItemAsync<Student>(objStudent, objStudent.StudentId, new PartitionKey(objStudent.Name));
                studentResponse = await this.container.ReplaceItemAsync<Student>(objStudent, objStudent.StudentId, new PartitionKey(objStudent.RollNumber));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
            return studentResponse;
        }


        public async Task<ItemResponse<Student>> GetStudentItem(string studentId, int partionKey)
        {
            ItemResponse<Student> studentResponse = null;
            try
            {
                studentResponse = await this.container.ReadItemAsync<Student>(studentId, new PartitionKey(partionKey));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
            return studentResponse;
        }

        public async Task<ItemResponse<Student>> DeleteStudentItem(string studentId, int partitionKey)
        {
            ItemResponse<Student> studentResponse = null;
            try
            {
                studentResponse = await this.container.ReadItemAsync<Student>(studentId, new PartitionKey(partitionKey));
                if (studentResponse != null)
                    studentResponse = await this.container.DeleteItemAsync<Student>(studentId, new PartitionKey(studentResponse.Resource.RollNumber));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
            return studentResponse;
        }


        public async Task<List<Student>> GetAllStudents()
        {
            var sqlQueryText = "SELECT * FROM c";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Student> queryResultSetIterator = this.container.GetItemQueryIterator<Student>(queryDefinition);

            List<Student> lststudents = new List<Student>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Student> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                lststudents = currentResultSet.Select(r => new Student()
                {
                    StudentId = r.StudentId,
                    Name = r.Name,
                    PhoneNumber = r.PhoneNumber,
                    Email = r.Email,
                    DateOfBirth = r.DateOfBirth,
                    Class = r.Class,
                    RollNumber = r.RollNumber
                }).ToList();
            }
            return lststudents;
        }

    }
}
