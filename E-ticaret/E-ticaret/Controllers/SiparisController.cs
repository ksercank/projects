using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using E_ticaret.Models;
using Microsoft.AspNet.Identity;

namespace E_ticaret.Controllers
{
    public class SiparisController : Controller
    {
        private ETICARETEntities db = new ETICARETEntities();

        // GET: Siparis
        public ActionResult Index()
        {
            var siparis = db.Siparis.Include(s => s.AspNetUsers);
            return View(siparis.ToList());
        }

        public ActionResult SiparisTamamla()
        {
            return RedirectToAction("Index");
        }

        public ActionResult Tamamlandi()
        {
            string userID = User.Identity.GetUserId();
            Siparis siparis = new Siparis()
            {
                Ad = Request.Form.Get("Ad"),
                Soyad = Request.Form.Get("Soyad"),
                Adres = Request.Form.Get("Adres"),
                Tarih = DateTime.Now,
                TCKimlik = Request.Form.Get("TCKimlik"),
                Telefon = Request.Form.Get("Telefon"),
                RefKulID = User.Identity.GetUserId()
            };

            List<Sepet> sepettekiurunler=db.Sepets.Where(x=>x.RefKulID==userID).ToList();

            foreach(Sepet item in sepettekiurunler)
            {
                SiparisDetay detay = new SiparisDetay();
                detay.RefUrunID = item.RefUrunID;
                detay.Adet = item.Adet;
                detay.ToplamTutar = item.Toplam;

                siparis.SiparisDetay.Add(detay);
                db.Sepets.Remove(item);
            }

            db.Siparis.Add(siparis);
            db.SaveChanges();
            return View();
        }

        // GET: Siparis/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Siparis siparis = db.Siparis.Find(id);
            if (siparis == null)
            {
                return HttpNotFound();
            }
            return View(siparis);
        }

        // GET: Siparis/Create
        public ActionResult Create()
        {
            ViewBag.RefKulID = new SelectList(db.AspNetUsers, "Id", "Email");
            return View();
        }

        // POST: Siparis/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SiparisID,RefKulID,Ad,Soyad,Adres,Telefon,TC_Kimlik,Tarih")] Siparis siparis)
        {
            if (ModelState.IsValid)
            {
                db.Siparis.Add(siparis);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RefKulID = new SelectList(db.AspNetUsers, "Id", "Email", siparis.RefKulID);
            return View(siparis);
        }

        // GET: Siparis/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Siparis siparis = db.Siparis.Find(id);
            if (siparis == null)
            {
                return HttpNotFound();
            }
            ViewBag.RefKulID = new SelectList(db.AspNetUsers, "Id", "Email", siparis.RefKulID);
            return View(siparis);
        }

        // POST: Siparis/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SiparisID,RefKulID,Ad,Soyad,Adres,Telefon,TC_Kimlik,Tarih")] Siparis siparis)
        {
            if (ModelState.IsValid)
            {
                db.Entry(siparis).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RefKulID = new SelectList(db.AspNetUsers, "Id", "Email", siparis.RefKulID);
            return View(siparis);
        }

        // GET: Siparis/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Siparis siparis = db.Siparis.Find(id);
            if (siparis == null)
            {
                return HttpNotFound();
            }
            return View(siparis);
        }

        // POST: Siparis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Siparis siparis = db.Siparis.Find(id);
            db.Siparis.Remove(siparis);
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
