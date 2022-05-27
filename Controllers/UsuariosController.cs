using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RpgMvc.Models;
using System.Collections.Generic;



namespace RpgMvc.Controllers

{
    public class UsuariosController : Controller
    {
        public string uriBase = "http://localhost:5000/Usuario/"; 
    
    [HttpGet]
    public ActionResult Index()
    {
        return View("CadastrarUsuario");
    }
    
    [HttpPost]
        public async Task<ActionResult> RegistrarAsync(UsuarioViewModel u)
        {
            try
            {
               HttpClient httpClient = new HttpClient();
               string uriComplementar = "Registrar";

               var content = new StringContent(JsonConvert.SerializeObject(u));
               content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
               HttpResponseMessage response = await httpClient.PostAsync(uriBase + uriComplementar, content);

               string serialized = await response.Content.ReadAsStringAsync();

               if (response.StatusCode == System.Net.HttpStatusCode.OK)
               {
                   TempData["Mensagem"]=
                    string.Format("Usuario {0} Registrado com sucesso! Faça o login para acessar.", u.Username);
                    return View("AutenticarUsuario"); 
               }
               else
               {
                   throw new System.Exception(serialized);
               }
            }
            catch (System.Exception ex)
            {
               TempData["MensagemErro"] = ex.Message + " " + ex.InnerException;
               return RedirectToAction("Index");
            }
        }

    [HttpGet]

    public ActionResult IndexLogin()
    {
        return View("AutenticarUsuario");
    }    

    [HttpPost]

    public async Task<ActionResult> AutenticarAsync(UsuarioViewModel u)
    {
        try
        {
            HttpClient httpClient = new HttpClient();
            string uriComplementar = "Autenticar";

            var content = new StringContent(JsonConvert.SerializeObject(u));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PostAsync(uriBase + uriComplementar, content);

            string serialized = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HttpContext.Session.SetString("SessionTokenUsuario", serialized);
                TempData["Mensagem"] = string.Format("Bem-vindo {0}!!!", u.Username);
                return RedirectToAction("Index", "Personagens");
            }
            else
            {
                throw new System.Exception(serialized);
            }
        }
        catch (System.Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
            return IndexLogin();
        }
    }


    [HttpGet]

    public async Task<ActionResult> IndexAsync()
    {
        try
        {
            string uriComplementar = "GetAll";
            HttpClient httpClient = new HttpClient();
            string token = HttpContext.Session.GetString("SessionTokenUsuario");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await httpClient.GetAsync(uriBase + uriComplementar);
            string serialized = await response.Content.ReadAsStringAsync();
            
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<PersonagemViewModel> listaPersonagens = await Task.Run(() =>
                 JsonConvert.DeserializeObject<List<PersonagemViewModel>>(serialized));

                 return View(listaPersonagens);
            }
            else
                throw new System.Exception(serialized);
        }
        catch (System.Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
            return RedirectToAction("Index");
        }
    }


    [HttpPost]

    public async Task<ActionResult> CreateAsync(PersonagemViewModel p)
    {
        try
        {
            HttpClient httpClient = new HttpClient();
            string token = HttpContext.Session.GetString("SessionTokenUsuario");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonConvert.SerializeObject(p));
            content.Headers.ContentType = new  MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PostAsync(uriBase + content);
            string serialized = await response.Content.ReadAsStringAsync();
            
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                TempData["Mensagem"] = string.Format("Personagem {0}, Id {1} salvo com sucesso!", p.Nome, serialized);
                return RedirectToAction("Index");
            }
            else
                throw new System.Exception(serialized);
        }
        catch (System.Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
            return RedirectToAction("Create");
        }
    }



    
    }

}
