using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace UnsecureApp.Controllers
{
    /// <summary>
    /// Crée des flux permettant de lire des fichiers.
    /// </summary>
    public interface IFileStreamFactory
    {
        /// <summary>
        /// Ouvre un fichier en lecture.
        /// </summary>
        /// <param name="path">Chemin du fichier à ouvrir.</param>
        /// <returns>Flux de lecture associé au fichier.</returns>
        Stream OpenRead(string path);
    }

    /// <summary>
    /// Crée des flux de lecture à partir du système de fichiers local.
    /// </summary>
    public sealed class FileStreamFactory : IFileStreamFactory
    {
        /// <inheritdoc />
        public Stream OpenRead(string path)
        {
            return File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }

    /// <summary>
    /// Fournit des opérations de démonstration pour l'accès aux fichiers,
    /// aux données SQL et la gestion des exceptions.
    /// </summary>
    public class MyController
    {
        private readonly IFileStreamFactory fileStreamFactory;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="MyController"/>.
        /// </summary>
        /// <param name="fileStreamFactory">
        /// Fabrique de flux utilisée pour lire les fichiers. Si elle est absente,
        /// une fabrique utilisant le système de fichiers local est créée.
        /// </param>
        public MyController(IFileStreamFactory? fileStreamFactory = null)
        {
            this.fileStreamFactory = fileStreamFactory ?? new FileStreamFactory();
        }

        /// <summary>
        /// Lit l'intégralité d'un fichier en UTF-8.
        /// </summary>
        /// <param name="userInput">Chemin du fichier à lire.</param>
        /// <returns>Contenu textuel du fichier.</returns>
        public string ReadFile(string userInput)
        {
            using Stream stream = fileStreamFactory.OpenRead(userInput);
            using StreamReader reader = new StreamReader(stream, Encoding.UTF8);

            return reader.ReadToEnd();
        }

        /// <summary>
        /// Recherche l'identifiant d'un produit à partir de son nom.
        /// </summary>
        /// <param name="productName">Nom du produit recherché.</param>
        /// <returns>Identifiant du produit trouvé.</returns>
        public int GetProduct(string productName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand()
                {
                    CommandText = "SELECT ProductId FROM Products WHERE ProductName = '" + productName + "'",
                    CommandType = CommandType.Text,
                };

                SqlDataReader reader = sqlCommand.ExecuteReader();
                return reader.GetInt32(0);
            }
        }

        /// <summary>
        /// Déclenche volontairement une exception de référence nulle et écrit
        /// ses détails dans la console.
        /// </summary>
        public void GetObject()
        {
            try
            {
                object? o = null;
                o!.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        
        }


        /// <summary>
        /// Chaîne de connexion utilisée pour accéder à la base de données.
        /// </summary>
        private string connectionString = "";
    }

    /// <summary>
    /// Représente un objet sans comportement défini.
    /// </summary>
    public class Toto
    {
    }

}