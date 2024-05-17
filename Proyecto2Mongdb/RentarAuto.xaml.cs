using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;
using System.Numerics;

namespace Proyecto2Mongdb;

public partial class RentarAuto : ContentPage
{
    public RentarAuto(string placa)
	{
        InitializeComponent();
        //CargarInformacionCarro(placa);
        
    }
    
    //private async void CargarInformacionCarro(string placa)
    //{
    //    try
    //    {
    //        // Cadena de conexión a tu instancia de MongoDB
    //        string connectionString = "mongodb://192.168.0.7:27017";

    //        // Nombre de la base de datos
    //        string databaseName = "RentaCarros";

    //        // Nombre de la colección
    //        string collectionName = "Carros";

    //        // Crear el cliente de MongoDB
    //        MongoClient client = new MongoClient(connectionString);

    //        // Obtener la base de datos
    //        IMongoDatabase database = client.GetDatabase(databaseName);

    //        // Obtener la colección
    //        IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(collectionName);

    //        // Crear un filtro para buscar el carro por la placa ingresada
    //        var filtro = Builders<BsonDocument>.Filter.Eq("Placas", placa);

    //        // Buscar el carro en la base de datos
    //        var carroEncontrado = await collection.Find(filtro).FirstOrDefaultAsync();

    //        // Verificar si se encontró un carro con la placa ingresada
    //        if (carroEncontrado != null)
    //        {
    //            // Rellenar los datos del carro en la interfaz de usuario
    //            placasEntry.Text = carroEncontrado["Placas"].AsString;
    //            precioEntry.Text = carroEncontrado["PrecioPorDia"].AsDouble.ToString();

    //            // Obtener la imagen del carro y mostrarla en la interfaz de usuario
    //            var imagenBytes = carroEncontrado["Imagen"].AsByteArray;
    //            carroImage.Source = ImageSource.FromStream(() => new MemoryStream(imagenBytes));
    //        }
    //        else
    //        {
    //            // Si no se encuentra ningún carro con la placa ingresada, mostrar un mensaje de error
    //            await DisplayAlert("Alerta", "No se encontró ningún carro con la placa ingresada", "Aceptar");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        await DisplayAlert("Error", $"Error al buscar el carro: {ex.Message}", "Aceptar");
    //    }
    //}
    private void btnRentar_Clicked(object sender, EventArgs e)
    {

    }

    private void btnRentar_Clicked_1(object sender, EventArgs e)
    {

    }
    

   

}