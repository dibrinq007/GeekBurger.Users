using GeekBurger.Users.Contract;
using GeekBurger.Users.Repository;
using GeekBurger.Users.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GeekBurger.Users.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]

    public class UsersController : Controller
    {
        private IUsersRepository _usersRepository;
        public static IConfiguration _configuration;
        public static FaceServiceClient _faceServiceClient;
        public static Guid FaceListId;

        public UsersController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;      
            _configuration = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json")
                  .Build();
        }


        [HttpPost("foodRestriction")]
        public async void PostFoodRestriction([FromBody] FoodRestriction foodRestriction)
        {
            var userRetrieved = new UserRetrieved()
            {
                AreRestrictionsSet = true,
                UserId = foodRestriction.UserId
            };

            var serviceBusService = new ServiceBusService(_configuration);
            serviceBusService.CreateTopic();
            await serviceBusService.SendMessagesAsync(userRetrieved);
        }

        [HttpPost("user")]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {           
            var serviceBusService = new ServiceBusService(_configuration);
            var result = serviceBusService.CreateTopic();

            try
            {
                var resultUsersRepository = _usersRepository.GetUsers();
                if (resultUsersRepository.Exists(x => x.Face == user.Face))
                {

                    var userRetrieved = new UserRetrieved()
                    {
                        AreRestrictionsSet = true,
                        UserId = user.UserId
                    };

                    result = await serviceBusService.SendMessagesAsync(userRetrieved);                    
                }
                else
                {                   

                    _faceServiceClient = new FaceServiceClient(_configuration["FaceAPIKey"], _configuration["FaceEndPoint"]);

                    FaceListId = Guid.Parse(_configuration["FaceListId"]);
                    var templateImage = @"Faces\{0}.jpg";                   

                    var containsAnyFaceOnList = UpsertFaceListAndCheckIfContainsFaceAsync().Result;
                    MemoryStream imageStream = new MemoryStream(Convert.FromBase64String(templateImage));

                    var face = DetectFaceAsync(imageStream).Result;
                    if (face != null)
                    {
                        Guid? persistedId = null;
                        if (containsAnyFaceOnList)
                            persistedId = FindSimilarAsync(face.FaceId, FaceListId).Result;

                        if (persistedId == null)
                            persistedId = AddFaceAsync(FaceListId, imageStream).Result;

                        var userRetrieved = new UserRetrieved()
                        {
                            AreRestrictionsSet = true,
                            UserId = (Guid)persistedId
                        };                                              

                        result = await serviceBusService.SendMessagesAsync(userRetrieved); 
                        
                    }                                    
                }

                if (result == false)
                {
                    return NotFound();
                }

                return StatusCode(200);
            }
            catch (Exception e)
            {
                return BadRequest("Ocorreu um erro durante o reconhecimento facial!");
            }
        }


        #region Api Face Detection
        private static async Task<bool> UpsertFaceListAndCheckIfContainsFaceAsync()
        {
            var faceListId = FaceListId.ToString();
            var faceLists = await _faceServiceClient.ListFaceListsAsync();
            var faceList = faceLists.FirstOrDefault(_ => _.FaceListId == FaceListId.ToString());

            if (faceList == null)
            {
                await _faceServiceClient.CreateFaceListAsync(faceListId, "GeekBurgerFaces", null);
                return false;
            }

            var faceListJustCreated = await _faceServiceClient.GetFaceListAsync(faceListId);

            return faceListJustCreated.PersistedFaces.Any();
        }

        private static async Task<Guid?> FindSimilarAsync(Guid faceId, Guid faceListId)
        {
            var similarFaces = await _faceServiceClient.FindSimilarAsync(faceId, faceListId.ToString());

            var similarFace = similarFaces.FirstOrDefault(_ => _.Confidence > 0.5);

            return similarFace?.PersistedFaceId;
        }

        private static async Task<Face> DetectFaceAsync(Stream imageStream)
        {
            try
            {
                var faces = await _faceServiceClient.DetectAsync(imageStream);
                return faces.FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static async Task<Guid?> AddFaceAsync(Guid faceListId, Stream imageStream)
        {
            try
            {
                AddPersistedFaceResult faceResult;
                faceResult = await _faceServiceClient.AddFaceToFaceListAsync(faceListId.ToString(), imageStream);
                return faceResult.PersistedFaceId;
            }
            catch (Exception e)
            {
                Console.WriteLine("Face não inclusa na Lista!");
                return null;
            }
        }
        #endregion

    }
}
