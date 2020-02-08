using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ClientsController : Controller
    {
        private WebApplication1Context db = new WebApplication1Context();

        // GET: Clients
        public ActionResult Index()
        {
            ViewBag.CLientStateAction = db.ClientStateAction.ToList();

            return View(db.Clients.ToList());
        }



        // GET: Clients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClientId,Name,Mail,PhoneNumber")] Client client)
        {


            if (ModelState.IsValid)
            {
                db.Clients.Add(client);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(client);
        }

        // GET: Clients/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }

            ViewBag.States = db.ClientStates.ToList();
            ViewBag.Actions = db.StateActions.ToList();
            ViewBag.ActionStates = db.StateActionState.ToList();
            var clientState = from c in db.ClientStateAction
                              where c.ClientId == id
                              select c;

            ViewBag.CLientStateAction = clientState.ToList();
            List<int> InState = new List<int>();
            foreach (var ClientState in clientState.ToList())
            {
                StateActionState stateActionState = db.StateActionState.Find(ClientState.StateActionStateId);
                InState.Add(stateActionState.ClientStateId);
            }

            ViewBag.ActualState = InState.Max();

            return View(client);
        }

        public ActionResult AddAction(int StateActionStateId, int ClientId, string Message)
        {


            ClientStateAction clientStateAction = StateKey(ClientId, Message, StateActionStateId);
            if (clientStateAction == null)
            {
                clientStateAction = CreateClientStateAction(ClientId, Message, StateActionStateId);
            }

            db.ClientStateAction.Add(clientStateAction);
            db.SaveChanges();
            ViewBag.States = db.ClientStates.ToList();
            ViewBag.Actions = db.StateActions.ToList();
            ViewBag.ActionStates = db.StateActionState.ToList();
            var clientState = from c in db.ClientStateAction
                              where c.ClientId == ClientId
                              select c;
            ViewBag.CLientStateAction = clientState.ToList();
            List<int> InState = new List<int>();
            foreach (var ClientState in clientState.ToList())
            {
                StateActionState stateActionState = db.StateActionState.Find(ClientState.StateActionStateId);
                InState.Add(stateActionState.ClientStateId);
            }
            ViewBag.ActualState = InState.Max();

            return PartialView("StateView");
        }

        public ClientStateAction StateKey(int ClientId, string Message, int StateActionStateId)
        {
            ClientStateAction clientStateAction = null;

            StateActionState stateActionState = db.StateActionState.FirstOrDefault(s => s.StateActionStateId == StateActionStateId);
            StateAction action = db.StateActions.FirstOrDefault(x => x.StateActionId == stateActionState.StateActionId);

            StateActionState NewStateActionState;

            if (action.Name == "Rechazado")
            {
                NewStateActionState = db.StateActionState.FirstOrDefault(
                    s => s.ClientState.Name == "Rechazado" && s.StateAction.Name == "Rechazado");
                clientStateAction = CreateClientStateAction(ClientId, Message, NewStateActionState.StateActionStateId);
            }

            switch (stateActionState.ClientState.Name)
            {
                case "Lead":
                    if (action.Name == "Llamada")
                    {
                        NewStateActionState = db.StateActionState.FirstOrDefault(
                           s => s.ClientState.Name == "Contacto" && s.StateAction.Name == "llamada");
                        clientStateAction = CreateClientStateAction(ClientId, Message, NewStateActionState.StateActionStateId);
                    }
                    break;
                case "Contacto":
                    if (action.Name == "Cita")
                    {
                        NewStateActionState = db.StateActionState.FirstOrDefault(
                           s => s.ClientState.Name == "Visita" && s.StateAction.Name == "Cita");
                        clientStateAction = CreateClientStateAction(ClientId, Message, NewStateActionState.StateActionStateId);
                    }
                    break;
                case "Visita":
                    if (action.Name == "Venta realizada")
                    {
                        NewStateActionState = db.StateActionState.FirstOrDefault(
                           s => s.ClientState.Name == "Venta" && s.StateAction.Name == "Venta realizada");
                        clientStateAction = CreateClientStateAction(ClientId, Message, NewStateActionState.StateActionStateId);
                    }
                    break;
                case "Venta":
                    if (action.Name == "Cerrar")
                    {
                        NewStateActionState = db.StateActionState.FirstOrDefault(
                           s => s.ClientState.Name == "Vendido" && s.StateAction.Name == "Venta realizada");
                        clientStateAction = CreateClientStateAction(ClientId, Message, NewStateActionState.StateActionStateId);
                    }
                    break;
                case "Rechazado":
                    if (action.Name == "Recuperar cliente")
                    {
                        ClientStateAction clientStateActionToDelete = db.ClientStateAction.FirstOrDefault(
                            c => c.ClientId == ClientId && c.StateActionState.ClientState.Name == "Rechazado");
                        db.ClientStateAction.Remove(clientStateActionToDelete);
                        db.SaveChanges();
                        NewStateActionState = db.StateActionState.FirstOrDefault(
                           s => s.ClientState.Name == "Contacto" && s.StateAction.Name == "Informacion");
                        clientStateAction = CreateClientStateAction(ClientId, Message, NewStateActionState.StateActionStateId);
                    }
                    break;
                default:
                    clientStateAction = null;
                    break;
            }

            return clientStateAction;

        }

        public ClientStateAction CreateClientStateAction(int ClientId, string Message, int StateActionStateId)
        {
            ClientStateAction clientStateAction = new ClientStateAction();
            clientStateAction.ClientId = ClientId;
            clientStateAction.JoinAction = DateTime.Now;
            clientStateAction.Message = Message;
            clientStateAction.StateActionStateId = StateActionStateId;

            return clientStateAction;
        }

        // POST: Clients/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClientId,Name,Mail,PhoneNumber")] Client client)
        {
            if (ModelState.IsValid)
            {
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Client client = db.Clients.Find(id);
            db.Clients.Remove(client);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
