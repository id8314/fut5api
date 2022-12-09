using System.ComponentModel.DataAnnotations;
using fut5.Data;

namespace fut5.Authentication
{
    public record UserModel
    {
        [Required]
        public string email { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
    }
    public interface IUserRepositoryService
    {
        Atleta ValidateCredentials(UserModel userModel);
    }
    public class UserRepositoryService : IUserRepositoryService
    {

        private readonly dbPathService _databasePath;
        public UserRepositoryService(dbPathService dps)
        {
            _databasePath = dps;
        }

        public Atleta ValidateCredentials(UserModel userModel)
        {
            Atleta atleta = new Atleta();

            AtletasDatabaseService atletas = new AtletasDatabaseService(_databasePath);
            var atletaList = atletas.GetAtleta(userModel.email).Result;
            
            foreach (var item in atletaList)
            {
                if (item.Pass == userModel.Password)
                {
                    atleta = item;
                }
            }

            return atleta;
        }

    }

}

