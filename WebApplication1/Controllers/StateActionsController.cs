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
    public class StateActionsController : Controller
    {
        private WebApplication1Context db = new WebApplication1Context();

        // GET: StateActions
        public ActionResult Index()
        {
            return View(db.StateActions.ToList());
        }

        // GET: StateActions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StateAction stateAction = db.StateActions.Find(id);
            if (stateAction == null)
            {
                return HttpNotFound();
            }
            return View(stateAction);
        }

        // GET: StateActions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StateActions/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StateActionsId,Name,WaitTime")] StateAction stateAction)
        {
            if (ModelState.IsValid)
            {
                db.StateActions.Add(stateAction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stateAction);
        }

        // GET: StateActions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StateAction stateAction = db.StateActions.Find(id);
            if (stateAction == null)
            {
                return HttpNotFound();
            }
            return View(stateAction);
        }

        // POST: StateActions/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StateActionsId,Name,WaitTime")] StateAction stateAction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stateAction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stateAction);
        }

        // GET: StateActions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StateAction stateAction = db.StateActions.Find(id);
            if (stateAction == null)
            {
                return HttpNotFound();
            }
            return View(stateAction);
        }

        // POST: StateActions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StateAction stateAction = db.StateActions.Find(id);
            db.StateActions.Remove(stateAction);
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
