using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Examenn.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Examenn.Firebase;

namespace Examenn.Controllers
{
    public class TransaccionController : Controller
    {
        private readonly FirestoreDb _firestoreDb;

        public TransaccionController()
        {
            _firestoreDb = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId);
        }

        // Redirigir de Index a Transaccion
        public async Task<IActionResult> Index()
        {
            var user = GetSessionInfo();
            if (user == null)
            {
                return RedirectToAction("Index", "Error");
            }

            var transaccionesRef = _firestoreDb.Collection("Transacciones").WhereEqualTo("User", user.uuid);
            var snapshot = await transaccionesRef.GetSnapshotAsync();
            var transacciones = new List<dynamic>();

            foreach (var doc in snapshot.Documents)
            {
                transacciones.Add(doc.ConvertTo<dynamic>());
            }

            // Redirigir a la acción 'Transaccion'
            return RedirectToAction("Transaccion");
        }

        public IActionResult Transaccion()
        {
            // Aquí puedes devolver una vista o manejar alguna lógica si es necesario
            return View();
        }

        private UserModel GetSessionInfo()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
            {
                return JsonConvert.DeserializeObject<UserModel>(HttpContext.Session.GetString("userSession"));
            }
            return null;
        }

        // Vista de Crear una nueva Transacción
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(string descripcion, decimal monto)
        {
            var user = GetSessionInfo();
            if (user == null)
            {
                return RedirectToAction("Index", "Error");
            }

            var transaccionRef = _firestoreDb.Collection("Transacciones").Document();
            var data = new
            {
                Descripcion = descripcion,
                Monto = monto,
                Fecha = DateTime.UtcNow,
                UserId = user.uuid
            };
            await transaccionRef.SetAsync(data);

            return RedirectToAction("Index");
        }

        // Vista de Editar Transacción
        public async Task<IActionResult> Editar(string id)
        {
            var user = GetSessionInfo();
            if (user == null)
            {
                return RedirectToAction("Index", "Error");
            }

            var doc = await _firestoreDb.Collection("Transacciones").Document(id).GetSnapshotAsync();
            if (doc.Exists)
            {
                return View(doc.ConvertTo<dynamic>());
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Editar(string id, string descripcion, decimal monto)
        {
            var user = GetSessionInfo();
            if (user == null)
            {
                return RedirectToAction("Index", "Error");
            }

            var transaccionRef = _firestoreDb.Collection("Transacciones").Document(id);
            var updates = new Dictionary<string, object>
            {
                { "Descripcion", descripcion },
                { "Monto", monto }
            };
            await transaccionRef.UpdateAsync(updates);

            return RedirectToAction("Index");
        }

        // Eliminar Transacción
        public async Task<IActionResult> Eliminar(string id)
        {
            var user = GetSessionInfo();
            if (user == null)
            {
                return RedirectToAction("Index", "Error");
            }

            var transaccionRef = _firestoreDb.Collection("Transacciones").Document(id);
            await transaccionRef.DeleteAsync();

            return RedirectToAction("Index");
        }

        // Vista de Registro de Usuario
        public IActionResult Registro()
        {
            return View();
        }

        // Método para registrar a un nuevo usuario
        [HttpPost]
        public async Task<IActionResult> Registro(string nombre, string correo, string password)
        {
            // Verifica si ya existe un usuario con el mismo correo
            var usuariosRef = _firestoreDb.Collection("Usuarios").WhereEqualTo("Correo", correo);
            var snapshot = await usuariosRef.GetSnapshotAsync();

            if (snapshot.Count > 0)
            {
                // Usuario ya existe, redirige a error o muestra un mensaje
                return RedirectToAction("Index", "Error");
            }

            // Crear un nuevo usuario en Firestore
            var userRef = _firestoreDb.Collection("Usuarios").Document();
            var userData = new
            {
                Nombre = nombre,
                Correo = correo,
                Password = password, // Considera encriptar la contraseña antes de guardarla
                FechaRegistro = DateTime.UtcNow
            };

            await userRef.SetAsync(userData);

            // Guarda información de sesión en caso de que sea necesario
            var userSession = new UserModel
            {
                uuid = userRef.Id,
                name = nombre,
                email = correo
            };
            HttpContext.Session.SetString("userSession", JsonConvert.SerializeObject(userSession));

            // Redirigir a la vista principal después del registro
            return RedirectToAction("Index", "Home");
        }
    }
}
