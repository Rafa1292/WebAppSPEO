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
    public class ClientStatesController : Controller
    {
        private WebApplication1Context db = new WebApplication1Context();

        // GET: ClientStates
        public ActionResult Index()
        {
            return View(db.ClientStates.ToList());
        }



        // GET: ClientStates/Create
        public ActionResult Create()
        {
            ViewBag.StateActions = new SelectList(db.StateActions.ToList(), "StateActionId", "Name");
            return View();
        }

        // POST: ClientStates/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClientStateId,Name")] ClientState clientState, int[] actions)
        {
            if (ModelState.IsValid)
            {

                db.ClientStates.Add(clientState);
                db.SaveChanges();
                StateActionState stateActionState = new StateActionState();
                List<StateActionState> stateActionStates = new List<StateActionState>();
                foreach (var action in actions)
                {
                    stateActionState.ClientStateId = clientState.ClientStateId;
                    stateActionState.StateActionId = action;
                    db.StateActionState.Add(stateActionState);
                    db.SaveChanges();

                }
                return RedirectToAction("Index");
            }

            return View(clientState);
        }

        // GET: ClientStates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientState clientState = db.ClientStates.Find(id);
            if (clientState == null)
            {
                return HttpNotFound();
            }
            return View(clientState);
        }

        // POST: ClientStates/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClientStateId,Name")] ClientState clientState)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientState).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clientState);
        }

        // GET: ClientStates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientState clientState = db.ClientStates.Find(id);
            if (clientState == null)
            {
                return HttpNotFound();
            }
            return View(clientState);
        }

        // POST: ClientStates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClientState clientState = db.ClientStates.Find(id);
            db.ClientStates.Remove(clientState);
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
