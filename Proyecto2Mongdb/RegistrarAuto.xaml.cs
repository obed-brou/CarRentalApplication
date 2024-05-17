using System;
using MongoDB.Driver;

using MongoDB.Bson;
using Microsoft.Maui.Controls;

namespace Proyecto2Mongdb;




public partial class RegistrarAuto : ContentPage
{
    byte[] imagenBytes;
    public RegistrarAuto()
	{
		InitializeComponent();
	}

    async void btnAgregarCarro_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Validar que se haya seleccionado una imagen
            if (carroImage.Source == null)
            {
                await DisplayAlert("Alerta", "Debes seleccionar una imagen del carro", "Aceptar");
                return;
            }

            // Convertir la imagen a un arreglo de bytes
            if (imagenBytes == null)
            {
                await DisplayAlert("Alerta", "No se ha cargado la imagen del carro", "Aceptar");
                return;
            }

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

            // Crear un documento BSON con los datos del carro
            var carro = new BsonDocument
            {
                { "Placas", placasEntry.Text },
                { "Marca", marcaEntry.Text },
                { "Modelo", modeloEntry.Text },
                { "A�o", Convert.ToInt32(a�oEntry.Text) },
                { "PrecioPorDia", Convert.ToDouble(precioEntry.Text) },
                { "Imagen", new BsonBinaryData(imagenBytes) },// Almacena la imagen como un arreglo de bytes
                { "Ocupado", true } // Marcar como ocupado (true) al rentar el carro
            };

            // Insertar el documento en la colecci�n
            await collection.InsertOneAsync(carro);
            var listaCarros = new ListaCarros();
            await listaCarros.CargarCarrosAsync();
            await DisplayAlert("�xito", "Carro agregado correctamente", "Aceptar");
            var listaCarrosPage = new ListaCarros();
            Renta renta = new Renta();
            renta.CargarCarros();
            
            

            // Cierra la p�gina actual
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al agregar el carro: {ex.Message}", "Aceptar");
        }
    }
    


    async void btnCargarImagen_Clicked(object sender, EventArgs e)
    {
        try
        {
            var resultado = await MediaPicker.PickPhotoAsync();
            if (resultado != null)
            {
                var stream = await resultado.OpenReadAsync();
                imagenBytes = new byte[stream.Length];
                await stream.ReadAsync(imagenBytes, 0, (int)stream.Length);
                carroImage.Source = ImageSource.FromStream(() => new MemoryStream(imagenBytes));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar la imagen: {ex.Message}", "Aceptar");
        }
    }

    async void placasEntry_Completed(object sender, EventArgs e)
    {
        try
        {
            // Validar que se haya ingresado una placa
            if (string.IsNullOrWhiteSpace(placasEntry.Text))
            {
                await DisplayAlert("Alerta", "Debes ingresar una placa", "Aceptar");
                return;
            }

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

            // Crear un filtro para buscar el carro por la placa ingresada
            var filtro = Builders<BsonDocument>.Filter.Eq("Placas", placasEntry.Text);

            // Buscar el carro en la base de datos
            var carroEncontrado = await collection.Find(filtro).FirstOrDefaultAsync();

            // Verificar si se encontr� un carro con la placa ingresada
            if (carroEncontrado != null)
            {
                // Rellenar los datos del carro en la interfaz de usuario
                marcaEntry.Text = carroEncontrado["Marca"].AsString;
                modeloEntry.Text = carroEncontrado["Modelo"].AsString;
                a�oEntry.Text = carroEncontrado["A�o"].AsInt32.ToString();
                precioEntry.Text = carroEncontrado["PrecioPorDia"].AsDouble.ToString();

                // Obtener la imagen del carro y mostrarla en la interfaz de usuario
                var imagenBytes = carroEncontrado["Imagen"].AsByteArray;
                carroImage.Source = ImageSource.FromStream(() => new MemoryStream(imagenBytes));
            }
            else
            {
                // Si no se encuentra ning�n carro con la placa ingresada, mostrar un mensaje de error
                await DisplayAlert("Alerta", "No se encontr� ning�n carro con la placa ingresada", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al buscar el carro: {ex.Message}", "Aceptar");
        }
    }
    private async Task<Stream> ObtenerNuevaImagenAsync()
    {
        try
        {
            // Solicitar permiso al usuario para acceder a la galer�a de im�genes
            var status = await Permissions.RequestAsync<Permissions.Photos>();

            if (status != PermissionStatus.Granted)
            {
                // Permiso denegado, mostrar un mensaje de error
                await DisplayAlert("Error", "No se concedi� permiso para acceder a la galer�a de im�genes", "Aceptar");
                return null;
            }

            // Abrir la galer�a de im�genes del dispositivo y permitir al usuario seleccionar una imagen
            var imagen = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Seleccionar imagen"
            });

            if (imagen == null)
            {
                // El usuario cancel� la selecci�n, no se seleccion� ninguna imagen
                return null;
            }

            // Abrir un flujo de lectura para la imagen seleccionada
            var stream = await imagen.OpenReadAsync();
            return stream;
        }
        catch (Exception ex)
        {
            // Manejar cualquier error que pueda ocurrir al acceder a la galer�a de im�genes
            await DisplayAlert("Error", $"Error al acceder a la galer�a de im�genes: {ex.Message}", "Aceptar");
            return null;
        }
    }

    private async void btnActualizarCarro_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Obtener la placa del carro a actualizar
            string placa = placasEntry.Text;

            // Validar que se haya ingresado una placa
            if (string.IsNullOrEmpty(placa))
            {
                await DisplayAlert("Alerta", "Debes ingresar la placa del carro a actualizar", "Aceptar");
                return;
            }

            // Obtener los nuevos valores ingresados por el usuario
            string nuevaMarca = marcaEntry.Text;
            string nuevoModelo = modeloEntry.Text;
            int nuevoAnio = Convert.ToInt32(a�oEntry.Text);
            double nuevoPrecioPorDia = Convert.ToDouble(precioEntry.Text);

            // Obtener la nueva imagen del carro
            byte[] nuevaImagenBytes = null;
            Stream nuevaImagenStream = await ObtenerNuevaImagenAsync(); // M�todo para obtener la nueva imagen, deber�s implementarlo
            if (nuevaImagenStream != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await nuevaImagenStream.CopyToAsync(memoryStream);
                    nuevaImagenBytes = memoryStream.ToArray();
                }
            }

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

            // Crear un filtro para encontrar el carro con la placa especificada
            var filtro = Builders<BsonDocument>.Filter.Eq("Placas", placa);

            // Crear un objeto de actualizaci�n para cambiar los campos del carro
            var actualizacionBuilder = Builders<BsonDocument>.Update
                .Set("Marca", nuevaMarca)
                .Set("Modelo", nuevoModelo)
                .Set("A�o", nuevoAnio)
                .Set("PrecioPorDia", nuevoPrecioPorDia);

            // Si se ha cargado una nueva imagen, tambi�n la actualizamos
            if (nuevaImagenBytes != null)
            {
                actualizacionBuilder = actualizacionBuilder.Set("Imagen", new BsonBinaryData(nuevaImagenBytes));
            }

            // Realizar la actualizaci�n en la base de datos
            var resultado = await collection.UpdateOneAsync(filtro, actualizacionBuilder);

            // Verificar si se encontr� y actualiz� el carro
            if (resultado.ModifiedCount > 0)
            {
                await DisplayAlert("�xito", "Carro actualizado correctamente", "Aceptar");
            }
            else
            {
                await DisplayAlert("Alerta", "No se encontr� ning�n carro con la placa especificada", "Aceptar");
            }

            // Actualizar la lista de carros
            var listaCarros = new ListaCarros();
            await listaCarros.CargarCarrosAsync();
            Renta renta = new Renta();
            renta.CargarCarros();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al actualizar el carro: {ex.Message}", "Aceptar");
        }
    }
    }