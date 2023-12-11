using ChatDemoAPI.Models;
using ChatDemoAPI.Services.Interfaces;
using OpenAI_API.Embedding;
using OpenAI_API;
using OpenAI_API.Models;
using Microsoft.EntityFrameworkCore;
using ChatDemoAPI.Models.ViewModels;
using ChatDemoAPI.Models.RequestModels;
using HtmlAgilityPack;


namespace ChatDemoAPI.Services
{
    // <summary>
    // Represents a service that provides functionality related to chat operations.
    // Implements the IChatService interface.
    // </summary>
    public class ChatService : IChatService
    {
        public IServiceProvider Services { get; }
        private readonly ChatDemoAPIContext dbcontext;
        private ILogger<ChatService> _logger;
        private readonly IConfiguration _conf;

        // <summary>
        // Constructs a new instance of the ChatService class.
        //
        // Parameters:
        // - conf: The IConfiguration instance used to access configuration settings.
        // - services: The IServiceProvider instance used for dependency injection.
        // - dbcontext: The ChatDemoAPIContext instance used for database operations.
        // - logger: The ILogger instance used for logging.
        // </summary>
        public ChatService(IConfiguration conf, IServiceProvider services, ChatDemoAPIContext dbcontext, ILogger<ChatService> logger)
        {
            Services = services;
            this.dbcontext = dbcontext;
            _logger = logger;
            _conf = conf;
        }

        // <summary>
        // Adds a chat robot to the database.
        //
        // Parameters:
        // - chatRobotRequest: The ChatRobotRequest instance containing the details of the chat robot to be added.
        //
        // Returns:
        // - A boolean value indicating whether the chat robot was successfully added or not.
        // </summary>
        public async Task<bool> AddChatRobot(ChatRobotRequest chatRobotRequest)
        {
            var result = await AddChatRobot(Guid.NewGuid(), chatRobotRequest.ChatRobotName, chatRobotRequest.ChatRobotDescription);
            return result;
        }

        // <summary>
        // Adds a user to the database.
        //
        // Parameters:
        // - userDescription: The description of the user to be added.
        //
        // Returns:
        // - A boolean value indicating whether the user was successfully added or not.
        // </summary>
        public async Task<bool> AddUser(string userDescription)
        {
            User user = new User();
            user.UserId = Guid.NewGuid();
            user.UserDescription = userDescription;
            using var transaction = await dbcontext.Database.BeginTransactionAsync();
            try
            {
                var result = await dbcontext.Users.AddAsync(user);
                await dbcontext.SaveChangesAsync();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError("Error:" + ex.Message);
                return false;
            };
        }

        // <summary>
        // Adds a chat robot description to the database.
        //
        // Parameters:
        // - chatRobotDescriptionRequest: The ChatRobotDescriptionRequest instance containing the details of the chat robot description to be added.
        //
        // Returns:
        // - A boolean value indicating whether the chat robot description was successfully added or not.
        // </summary>
        public async Task<bool> AddChatRobotDescription(ChatRobotDescriptionRequest chatRobotDescriptionRequest)
        {
            ChatRobotDescription chatRobotDescription = new ChatRobotDescription();
            chatRobotDescription.Description = chatRobotDescriptionRequest.Description;
            chatRobotDescription.IsDefault = chatRobotDescriptionRequest.IsDefault;
            chatRobotDescription.ChatRobotDescriptionId = Guid.NewGuid();
            chatRobotDescription.ChatRobotId = chatRobotDescriptionRequest.ChatRobotId;
            using var transaction = await dbcontext.Database.BeginTransactionAsync();
            try
            {
                var result = await dbcontext.ChatRobotsDescription.AddAsync(chatRobotDescription);
                await dbcontext.SaveChangesAsync();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError("Error:" + ex.Message);
                return false;
            }

        }

        // <summary>
        // Uploads a PDF file and extracts the text chunks from it.
        //
        // Parameters:
        // - file: The IFormFile instance representing the PDF file to be uploaded.
        //
        // Returns:
        // - A list of TextChunk instances representing the extracted text chunks from the PDF file.
        // </summary>
        public async Task<List<TextChunk>> UploadPdf(IFormFile file)
        {
            string OpenAI_Key = _conf["OpenAI:OpenAI_Key"];
            List<TextChunk> result = new List<TextChunk>();
            PdfReader pdfTextReader = new PdfReader();
            result = await pdfTextReader.UploadPdf(file, 600, OpenAI_Key);
            return result;
        }

        // <summary>
        // Saves the content of web pages to the database for a given chat robot.
        //
        // Parameters:
        // - chatRobotId: The ID of the chat robot.
        // - url: The URL of the web page to be saved.
        // - deep: The depth of recursive crawling for sub-links on the web page. Default is 1.
        //
        // Returns:
        // - A boolean value indicating whether the web pages were successfully saved to the database or not.
        // </summary>
        public async Task<bool> SaveWebpageToDb(Guid chatRobotId, string url, int deep = 1)
        {

            int i = 1;
            HashSet<string> crawledUrls = new();
            var webPageList = await GetLinksRecursive(url, deep, crawledUrls);
            var noDupesWebPageList = webPageList.Distinct().ToList();
            string OpenAI_Key = _conf["OpenAI:OpenAI_Key"];
            foreach (var webPage in noDupesWebPageList)
            {
                try

                {
                    _logger.LogInformation("####### In process:" + i.ToString() + "/" + noDupesWebPageList.Count.ToString() + " ########");
                    i++;
                    PdfReader webPageReader = new PdfReader();
                    var result = await webPageReader.HandleWebpage(webPage, deep, 600, OpenAI_Key);
                    if (result.Count > 0)
                    {
                        await SavePdfToChatRobot(result, chatRobotId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("ERROR in SaveWebpageToDb:" + ex.Message.ToString());
                    continue;
                }
            }
            return true;
        }

        // <summary>
        // Checks if a URL exists.
        //
        // Parameters:
        // - url: The URL to check.
        //
        // Returns:
        // - A boolean value indicating whether the URL exists or not.
        // </summary>
        private static async Task<bool> DoesUrlExists(String url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Do only Head request to avoid downloading the full file.
                    var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));

                    if (response.IsSuccessStatusCode)
                    {
                        // URL is available if we have a SuccessStatusCode.
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        // <summary>
        // Recursively retrieves the links from a web page.
        //
        // Parameters:
        // - url: The URL of the web page.
        // - depth: The depth of recursive crawling for sub-links on the web page.
        // - crawledUrls: A HashSet containing the URLs that have already been crawled.
        //
        // Returns:
        // - A list of strings representing the links found on the web page.
        // </summary>
        private async Task<List<string>> GetLinksRecursive(string url, int depth, HashSet<string> crawledUrls)
        {

            url = fixDocumentURL(url);

            if (depth <= 0 || crawledUrls.Contains(url))
                return new List<string>();

            crawledUrls.Add(url);

            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = new HtmlDocument();
            var doesUrlExists = await DoesUrlExists(url);
            if (doesUrlExists == true) { document = web.Load(url); }

            var links = GetDocumentLinks(document, url);


            var subLinks = new List<string>();
            if (depth > 1)
            {
                foreach (string link in links)
                {
                    var subDepth = depth - 1;
                    var subCrawledUrls = new HashSet<string>(crawledUrls);
                    var subSubLinks = await GetLinksRecursive(link, subDepth, subCrawledUrls);
                    subLinks.AddRange(subSubLinks);
                }
            }
            string plainText = document.DocumentNode.InnerText;
            links.AddRange(subLinks);
            return links;
        }

        // <summary>
        // Fixes the URL of a document by removing the trailing slash if present.
        //
        // Parameters:
        // - url: The URL to fix.
        //
        // Returns:
        // - The fixed URL.
        // </summary>
        private string fixDocumentURL(string url)
        {
            if (url.EndsWith("/"))
            {
                url = url.Remove(url.Length - 1);
            }
            return url;
        }

        // <summary>
        // Retrieves the links from an HTML document.
        //
        // Parameters:
        // - htmlDocument: The HtmlDocument instance representing the HTML document.
        // - documentUrl: The URL of the document.
        //
        // Returns:
        // - A list of strings representing the links found in the HTML document.
        // </summary>
        private List<string> GetDocumentLinks(HtmlDocument htmlDocument, string documentUrl)
        {

            var nodeLinks = htmlDocument.DocumentNode.SelectNodes("//a[@href]");
            if (nodeLinks == null) return new List<string>();

            var links = new List<string>();

            foreach (HtmlNode link in nodeLinks)
            {
                string href = link.GetAttributeValue("href", "");

                var isRelative = !href.StartsWith("http") && !href.StartsWith("www.");

                if (isRelative)
                {
                    href = documentUrl + href;
                }


                if (string.IsNullOrEmpty(href))
                {
                    break;
                }


                if (!href.Contains("#"))
                {
                    links.Add(href);
                }

            }


            return links;
        }

        // <summary>
        // Retrieves the name of a chat robot based on its ID.
        //
        // Parameters:
        // - chatRobotId: The ID of the chat robot.
        //
        // Returns:
        // - A string representing the name of the chat robot.
        // </summary>
        private async Task<string> GetChatRobotName(Guid chatRobotId)
        {
            var result = await dbcontext.ChatRobots.Where(x => x.ChatRobotId == chatRobotId).Select(y => new { y.ChatRobotName }).FirstOrDefaultAsync();
            return result.ToString();
        }

        // <summary>
        // Retrieves the document detail based on its ID.
        //
        // Parameters:
        // - documentDetailId: The ID of the document detail.
        //
        // Returns:
        // - The DocumentDetail instance representing the document detail.
        // </summary>
        private async Task<DocumentDetail> GetDocumentDetail(Guid documentDetailId)
        {
            var result = await dbcontext.DocumentDetail.Where(x => x.DocumentDetailId == documentDetailId).FirstOrDefaultAsync();
            return result;
        }

        // <summary>
        // Retrieves the document based on its ID.
        //
        // Parameters:
        // - documentId: The ID of the document.
        //
        // Returns:
        // - The Document instance representing the document.
        // </summary>
        private async Task<Models.Document> GetDocument(Guid documentId)
        {
            var result = await dbcontext.Document.Where(x => x.DocumentId == documentId).FirstOrDefaultAsync();
            return result;
        }


        // <summary>
        // Retrieves a list of documents with similar content to a given question for a specific chat robot.
        //
        // Parameters:
        // - chatRobotId: The ID of the chat robot.
        // - question: The question for which similar content is to be retrieved.
        // - count: The number of similar content documents to retrieve. Default is 10.
        //
        // Returns:
        // - A list of SimilarContentDocumentResult instances representing the similar content documents.
        // </summary>
        public async Task<List<SimilarContentDocumentResult>> GetSimilarContent(Guid chatRobotId, string question, int count = 10)
        {
            var similarContentDocumentResult = new List<SimilarContentDocumentResult>();
            try
            {
                var chatRobotName = await GetChatRobotName(chatRobotId);

                var questionVector = await CreateVectors(question);
                var questionVectorData = new List<double>();
                foreach (var vector in questionVector)
                {
                    questionVectorData.Add(vector);
                }
                double[] answerData = questionVectorData.ToArray();
                var result = await GetChatRobotVectorData(chatRobotId);
                var chunkVectordata = result
                    .GroupBy(x => x.DocumentDetaiId)
                    .Select(group => new
                    {
                        DocumentDetaiId = group.Key,
                        vectors = group
                        .Select(x => x.VectorValue).ToArray()
                    });
                if (chunkVectordata.Count() > 0)
                {

                    foreach (var item in chunkVectordata)
                    {
                        var value = CalculateSimilarity(answerData, item.vectors);
                        var resultDocumentDetail = await GetDocumentDetail(item.DocumentDetaiId);
                        var resultDocument = await GetDocument(resultDocumentDetail.DocumentId);
                        var similarContent = new SimilarContentDocumentResult
                        {
                            cosine_distance = value,
                            DocumentContent = resultDocumentDetail.DocumentContent,
                            DocumentSequence = resultDocumentDetail.DocumentSequence,
                            DocumentName = resultDocument.DocumentName,
                            ChatRobotName = chatRobotName,
                            DocumentDetailId = resultDocumentDetail.DocumentDetailId,
                        };
                        similarContentDocumentResult.Add(similarContent);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GetSimilarContent:" + ex.Message);
                return null;
            }
            List<SimilarContentDocumentResult> objListOrder = similarContentDocumentResult.OrderByDescending(order => order.cosine_distance).ToList();
            var topResult = objListOrder.Take(count).ToList();
            return topResult;
        }

        // <summary>
        // Retrieves the vector data for a chat robot.
        //
        // Parameters:
        // - chatRobotId: The ID of the chat robot.
        //
        // Returns:
        // - A list of VectorDataResponse instances representing the vector data for the chat robot.
        // </summary>
        public async Task<List<VectorDataResponse>> GetChatRobotVectorData(Guid chatRobotId)
        {
            var chatRobotVectorData = new List<VectorDataResponse>();
            try
            {
                var result = await (from cr in dbcontext.ChatRobots
                                    join d in dbcontext.Document on cr.ChatRobotId equals d.ChatRobotId
                                    join dr in dbcontext.DocumentDetail on d.DocumentId equals dr.DocumentId
                                    join dvd in dbcontext.DocumentVectorData on dr.DocumentDetailId equals dvd.DocumentDetailId
                                    where cr.ChatRobotId == chatRobotId
                                    select new VectorDataResponse
                                    {
                                        ChatRobotId = chatRobotId,
                                        DocumentDetaiId = dr.DocumentDetailId,
                                        DocumentId = dr.DocumentId,
                                        DocumentContent = dr.DocumentContent,
                                        VectorValue = dvd.VectorValue
                                    }).ToListAsync();
                chatRobotVectorData.AddRange(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetChatRobotVectorData:" + ex.Message);

            }
            return chatRobotVectorData;
        }

        // <summary>
        // Retrieves a list of chat robots from the database.
        //
        // Returns:
        // - A list of ChatRobot instances representing the chat robots.
        // </summary>
        public async Task<List<ChatRobot>> GetChatRobots()
        {
            var chatRobotResult = await dbcontext.ChatRobots.ToListAsync();
            //.Include(x => x.ChatRobotDescriptions).ToListAsync();
            return chatRobotResult;
        }

        // <summary>
        // Retrieves a list of users from the database.
        //
        // Returns:
        // - A list of User instances representing the users.
        // </summary>
        public async Task<List<User>> GetUsers()
        {
            var usersResult = await dbcontext.Users.ToListAsync();
            //.Include(x => x.ChatRobotDescriptions).ToListAsync();
            return usersResult;
        }

        // <summary>
        // Retrieves a list of chat robots based on their ID.
        //
        // Parameters:
        // - chatRobotId: The ID of the chat robot.
        //
        // Returns:
        // - A list of ChatRobot instances representing the chat robots.
        // </summary>
        public async Task<List<ChatRobot>> GetChatRobotById(Guid chatRobotId)
        {
            var chatRobotResult = await dbcontext.ChatRobots.Where(a => a.ChatRobotId == chatRobotId).ToListAsync();
            // .Include(a => a.Document).Where(a => a.ChatRobotId == chatRobotId).ToListAsync();   

            return chatRobotResult;
        }

        // <summary>
        // Calculates the similarity between two embeddings using cosine similarity.
        //
        // Parameters:
        // - embedding1: The first embedding.
        // - embedding2: The second embedding.
        //
        // Returns:
        // - A double value representing the cosine similarity between the two embeddings.
        // </summary>
        public double CalculateSimilarity(double[] embedding1, double[] embedding2)
        {
            if (embedding1.Length != embedding2.Length)
            {
                return 0;
            }

            double dotProduct = 0.0;
            double magnitude1 = 0.0;
            double magnitude2 = 0.0;

            for (int i = 0; i < embedding1.Length; i++)
            {
                dotProduct += embedding1[i] * embedding2[i];
                magnitude1 += Math.Pow(embedding1[i], 2);
                magnitude2 += Math.Pow(embedding2[i], 2);
            }

            magnitude1 = Math.Sqrt(magnitude1);
            magnitude2 = Math.Sqrt(magnitude2);

            if (magnitude1 == 0.0 || magnitude2 == 0.0)
            {
                throw new ArgumentException("embedding must not have zero magnitude.");
            }

            double cosineSimilarity = dotProduct / (magnitude1 * magnitude2);

            return cosineSimilarity;

            // Uncomment this if you need a cosine distance instead of similarity
            //double cosineDistance = 1 - cosineSimilarity;

            //return cosineDistance;
        }

        // <summary>
        // Creates embeddings for a given text using the OpenAI API.
        //
        // Parameters:
        // - text: The text for which embeddings are to be created.
        //
        // Returns:
        // - A float array representing the embeddings for the given text.
        // </summary>
        public async Task<float[]> CreateVectors(string text)
        {
            try
            {
                string OpenAI_Key = _conf["OpenAI:OpenAI_Key"];
                OpenAIAPI api = new OpenAIAPI(new APIAuthentication(OpenAI_Key));

                var result = await api.Embeddings.CreateEmbeddingAsync(
                    new EmbeddingRequest(Model.AdaTextEmbedding, text));

                return result.Data[0].Embedding;
            }
            catch (Exception ex)
            {
                _logger.LogError("Create vectors:" + ex.Message);
                float[] val = new float[1];
                return val;
            }
        }

        // <summary>
        // Adds a chat robot to the database.
        //
        // Parameters:
        // - chatRobotId: The ID of the chat robot.
        // - chatRobotName: The name of the chat robot.
        // - chatRobotDescription: The description of the chat robot.
        //
        // Returns:
        // - A boolean value indicating whether the chat robot was successfully added or not.
        // </summary>
        public async Task<bool> AddChatRobot(Guid chatRobotId, string chatRobotName, string chatRobotDescription)
        {
            ChatRobot chatRobot = new ChatRobot
            {
                ChatRobotId = chatRobotId,
                ChatRobotName = chatRobotName,
                ChatRobotDescription = chatRobotDescription

            };

            using var transaction = await dbcontext.Database.BeginTransactionAsync();

            try
            {
                var res = await dbcontext.ChatRobots.AddAsync(chatRobot);
                await dbcontext.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Failed to add chat robot.");
                return false;
            }

        }

        // <summary>
        // Adds a document to the database.
        //
        // Parameters:
        // - documentId: The ID of the document.
        // - chatRobotId: The ID of the chat robot.
        // - documentName: The name of the document.
        //
        // Returns:
        // - A boolean value indicating whether the document was successfully added or not.
        // </summary>
        public async Task<bool> AddDocument(Guid documentId, Guid chatRobotId, string documentName)
        {
            Models.Document document = new Models.Document
            {
                DocumentId = documentId,
                ChatRobotId = chatRobotId,
                DocumentName = documentName
            };

            using var transaction = await dbcontext.Database.BeginTransactionAsync();

            try
            {
                var res = await dbcontext.Document.AddAsync(document);
                await dbcontext.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Failed to add document.");
                return false;
            }

        }

        // <summary>
        // Adds a document detail to the database.
        //
        // Parameters:
        // - documentId: The ID of the document.
        // - data: The list of TextChunk instances representing the document details.
        //
        // Returns:
        // - A boolean value indicating whether the document detail was successfully added or not.
        // </summary>
        public async Task<bool> AddDocumentDetail(Guid documentId, List<TextChunk> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                Guid documentDetailId = Guid.NewGuid();
                DocumentDetail documentDetail = new DocumentDetail
                {
                    DocumentId = documentId,
                    DocumentDetailId = documentDetailId,
                    DocumentSequence = i,
                    DocumentContent = data[i].Text,
                    DocumentType = 0
                };
                var res = await dbcontext.DocumentDetail.AddAsync(documentDetail);
                await AddDocumentVectorData(documentDetailId, data[i]);
            }
            using var transaction = await dbcontext.Database.BeginTransactionAsync();

            try
            {

                await dbcontext.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Failed to add document.");
                return false;
            }

        }

        // <summary>
        // Adds document vector data to the database.
        //
        // Parameters:
        // - documentDetailId: The ID of the document detail.
        // - data: The TextChunk instance representing the document vector data.
        //
        // Returns:
        // - A boolean value indicating whether the document vector data was successfully added or not.
        // </summary>
        public async Task<bool> AddDocumentVectorData(Guid documentDetailId, TextChunk data)
        {
            for (int i = 0; i < data.TextVectors.Length; i++)
            {

                DocumentVectorData documentVectorData = new DocumentVectorData
                {
                    DocumentDetailId = documentDetailId,
                    VectorValue = data.TextVectors[i]
                };
                var res = await dbcontext.DocumentVectorData.AddAsync(documentVectorData);
            }
            using var transaction = await dbcontext.Database.BeginTransactionAsync();

            try
            {

                await dbcontext.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Failed to add document.");
                return false;
            }

        }

        // <summary>
        // Saves data to the database.
        //
        // Parameters:
        // - data: The list of TextChunk instances representing the data to be saved.
        // - chatRobotName: The name of the chat robot.
        // - chatRobotDescription: The description of the chat robot.
        //
        // Returns:
        // - A Task representing the asynchronous operation.
        // </summary>
        public async Task SaveDataToDb(List<TextChunk> data, string chatRobotName, string chatRobotDescription)
        {
            Guid chatRobotId = Guid.NewGuid();
            Guid documentId = Guid.NewGuid();

            var resultAddChatRobot = await AddChatRobot(chatRobotId, chatRobotName, chatRobotDescription);
            if (resultAddChatRobot == true)
            {
                var resultAddDocumnet = await AddDocument(documentId, chatRobotId, data[0].SourceFile);
                if (resultAddDocumnet == true)
                {
                    var result = await AddDocumentDetail(documentId, data);
                }
            }
        }

        // <summary>
        // Saves PDF data to the chat robot in the database.
        //
        // Parameters:
        // - data: The list of TextChunk instances representing the PDF data.
        // - chatRobotId: The ID of the chat robot.
        //
        // Returns:
        // - A Task representing the asynchronous operation.
        // </summary>
        public async Task SavePdfToChatRobot(List<TextChunk> data, Guid chatRobotId)
        {
            Guid documentId = Guid.NewGuid();
            var resultAddDocumnet = await AddDocument(documentId, chatRobotId, data[0].SourceFile);
            if (resultAddDocumnet == true)
            {
                var result = await AddDocumentDetail(documentId, data);
            }
        }
    }
}