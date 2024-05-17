namespace Proyecto2Mongdb;
using MongoDB.Bson;
using MongoDB.Driver;

public partial class Renta : ContentPage
{
	public Renta()
	{
		InitializeComponent();
        CargarCarros();
	}
    public class Carro
    {
        public bool Ocupado { get; set; }
        public ImageSource Imagen { get; set; }
        public string Placas { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Año { get; set; }
        public double PrecioPorDia { get; set; }
    }
    private void btnRentar_Clicked(object sender, EventArgs e)
    {
        RentarCarro();
    }
    public void CargarCarros()
    {
        try
        {
            // Cadena de conexión a tu instancia de MongoDB
            string connectionString = "mongodb://192.168.137.38:27017";

            // Nombre de la base de datos
            string databaseName = "RentaCarros";

            // Nombre de la colección
            string collectionName = "Carros";

            // Crear el cliente de MongoDB
            MongoClient client = new MongoClient(connectionString);

            // Obtener la base de datos
            IMongoDatabase database = client.GetDatabase(databaseName);

            // Obtener la colección
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(collectionName);

            // Definir el filtro para obtener solo los carros desocupados
            var filtro = Builders<BsonDocument>.Filter.Eq("Ocupado", true);

            // Obtener todos los carros de la colección que están desocupados
            var carros = collection.Find(filtro).ToList();

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
                    Año = doc["Año"].AsInt32,
                    PrecioPorDia = doc["PrecioPorDia"].AsDouble

                });
            }

            // Asignar la lista de carros al ListView
            carrosListView.ItemsSource = listaCarros;
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Error al cargar los carros: {ex.Message}", "Aceptar");
        }
    }


    private void carrosListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            // Verificar si se seleccionó un elemento
            if (e.SelectedItem == null)
                return;

            // Obtener el carro seleccionado
            var carroSeleccionado = (Carro)e.SelectedItem;

            // Rellenar los campos PlacasEntry y PrecioEntry con la información del carro seleccionado
            placasEntry.Text = carroSeleccionado.Placas;
            precioEntry.Text = carroSeleccionado.PrecioPorDia.ToString();

            // Deseleccionar el elemento de la ListView
            carrosListView.SelectedItem = null;
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Error al seleccionar el carro: {ex.Message}", "Aceptar");
        }
    }
    private void CalcularTotalPagar()
    {
        try
        {
            // Obtener la fecha de salida y de entrega
            DateTime fechaSalida = fechaSalidaPicker.Date;
            DateTime fechaEntrega = fechaEntregaPicker.Date;

            // Calcular la diferencia de días entre la fecha de salida y de entrega
            int diasTotales = (int)(fechaEntrega - fechaSalida).TotalDays;

            // Obtener el precio por día
            double precioPorDia = Convert.ToDouble(precioEntry.Text);

            // Calcular el total a pagar
            double totalPagar = diasTotales * precioPorDia;

            // Mostrar el total a pagar en el entry correspondiente
            totalPagarEntry.Text = totalPagar.ToString();
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Error al calcular el total a pagar: {ex.Message}", "Aceptar");
        }
    }

    private void fechaEntregaPicker_DateSelected(object sender, DateChangedEventArgs e)
    {

        CalcularTotalPagar();
    }

    private void fechaSalidaPicker_DateSelected(object sender, DateChangedEventArgs e)
    {

        CalcularTotalPagar();

    }
    private async void RentarCarro()
    {
        try
        {
            // Validar que los campos no estén vacíos
            if (string.IsNullOrWhiteSpace(placasEntry.Text) ||
                string.IsNullOrWhiteSpace(nombreClienteEntry.Text) ||
                string.IsNullOrWhiteSpace(telefonoEntry.Text) ||
                string.IsNullOrWhiteSpace(totalPagarEntry.Text))
            {
                await DisplayAlert("Alerta", "Por favor completa todos los campos", "Aceptar");
                return;
            }

            // Validar que las fechas de salida y entrega sean válidas
            if (fechaSalidaPicker.Date >= fechaEntregaPicker.Date)
            {
                await DisplayAlert("Alerta", "La fecha de entrega debe ser posterior a la fecha de salida", "Aceptar");
                return;
            }

            // Obtener los datos ingresados por el usuario
            string placa = placasEntry.Text;
            string nombreCliente = nombreClienteEntry.Text;
            string telefono = telefonoEntry.Text;
            DateTime fechaSalida = fechaSalidaPicker.Date;
            DateTime fechaEntrega = fechaEntregaPicker.Date;
            double totalPagar = Convert.ToDouble(totalPagarEntry.Text);

            // Cadena de conexión a tu instancia de MongoDB
            string connectionString = "mongodb://192.168.137.38:27017";

            // Nombre de la base de datos
            string databaseName = "RentaCarros";

            // Nombre de la colección
            string collectionName = "Rentas";

            // Crear el cliente de MongoDB
            MongoClient client = new MongoClient(connectionString);

            // Obtener la base de datos
            IMongoDatabase database = client.GetDatabase(databaseName);

            // Obtener la colección
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(collectionName);

            // Crear un documento BSON con los datos de la renta
            var renta = new BsonDocument
        {
            { "Placa", placa },
            { "NombreCliente", nombreCliente },
            { "Telefono", telefono },
            { "FechaSalida", fechaSalida },
            { "FechaEntrega", fechaEntrega },
            { "TotalPagar", totalPagar },
            { "Ocupado", true } // Marcar como ocupado (true) al rentar el carro
        };

            // Insertar el documento en la colección
            await collection.InsertOneAsync(renta);

            // Mostrar mensaje de éxito
            await DisplayAlert("Éxito", "Carro rentado correctamente", "Aceptar");

            // Recargar la lista de carros
            CargarCarros();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al rentar el carro: {ex.Message}", "Aceptar");
        }
    }

}