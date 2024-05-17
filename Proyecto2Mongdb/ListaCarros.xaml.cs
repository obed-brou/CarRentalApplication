using MongoDB.Bson;
using MongoDB.Driver;

namespace Proyecto2Mongdb;

public partial class ListaCarros : ContentPage
{
	public ListaCarros()
	{
		InitializeComponent();
        CargarCarrosAsync();
    }
    public class Carro
    {
        public ImageSource Imagen { get; set; }
        public string Placas {  get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int A�o { get; set; }
        public double PrecioPorDia { get; set; }
    }
    public async Task CargarCarrosAsync()
    {
        try
        {
            // Cadena de conexi�n a tu instancia de MongoDB
            string connectionString = "mongodb://192.168.137.38:27017";

            // Nombre de la base de datos
            string databaseName = "RentaCarros";

            // Nombre de la colecci�n
            string collectionName = "Carros";

            // Crear el cliente de MongoDB
            MongoClient client = new MongoClient(connectionString);

            // Obtener la base de datos
            IMongoDatabase database = client.GetDatabase(databaseName);

            // Obtener la colecci�n
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(collectionName);

            // Obtener todos los carros de la colecci�n
            var carros = await collection.Find(new BsonDocument()).ToListAsync();

            // Convertir los documentos BSON a una lista de objetos Carro
            var listaCarros = new List<Carro>();
            foreach (var doc in carros)
            {
                listaCarros.Add(new Carro
                {
                    Imagen = ImageSource.FromStream(() => new MemoryStream(doc["Imagen"].AsByteArray)),
                    Placas = doc["Placas"].AsString,
                    Marca = doc["Marca"].AsString,
                    Modelo = doc["Modelo"].AsString,
                    A�o = doc["A�o"].AsInt32,
                    PrecioPorDia = doc["PrecioPorDia"].AsDouble
                });
            }

            // Asignar la lista de carros al ListView
            carrosListView.ItemsSource = listaCarros;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar los carros: {ex.Message}", "Aceptar");
        }
    }
    
    
    

    async void btnActualizar_Clicked(object sender, EventArgs e)
    {
        await CargarCarrosAsync();
    }

    async void btnRentar_Clicked(object sender, EventArgs e)
    {
        //try
        //{
        //    // Verificar si se ha seleccionado un carro en la lista
        //    if (carrosListView.SelectedItem == null)
        //    {
        //        await DisplayAlert("Alerta", "Debes seleccionar un carro de la lista", "Aceptar");
        //        return;
        //    }

        //    // Obtener el carro seleccionado
        //    var carroSeleccionado = (Carro)carrosListView.SelectedItem;

        //    // Crear una instancia de la p�gina RentarAuto y pasar la placa del carro seleccionado
        //    Renta rentarAutoPage = new Renta(carroSeleccionado.Placas);


        //    // Mostrar la p�gina RentarAuto
        //    await Navigation.PushAsync(rentarAutoPage);
        //}
        //catch (Exception ex)
        //{
        //    await DisplayAlert("Error", $"Error al rentar el auto: {ex.ToString()}", "Aceptar");
        //}

    }
}