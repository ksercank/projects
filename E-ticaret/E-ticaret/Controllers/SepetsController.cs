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
using Microsoft.AspNet.Identity.EntityFramework;

namespace E_ticaret.Controllers
{
    public class SepetsController : Controller
    {
        private ETICARETEntities ctx = new ETICARETEntities();

        // GET: Sepets
        public ActionResult Index()
        {
            var sepets = ctx.Sepets.Include(s => s.AspNetUser).Include(s => s.Urunler);
            return View(sepets.ToList());
        }

        public ActionResult SepeteEkle(int? adet, int id)
        {
            string UserID = User.Identity.GetUserId();
            Urunler urun = ctx.Urunlers.Find(id);
            Sepet sepettekiurunler = ctx.Sepets.FirstOrDefault(x => x.RefUrunID == id && x.RefKulID == UserID);
            if(sepettekiurunler==null)
            {
                Sepet yeniurun = new Sepet();
                yeniurun.RefKulID = UserID;
                yeniurun.RefUrunID = id;
                yeniurun.Toplam = (adet??1) * urun.UrunFiyati;
                yeniurun.Adet = adet??1;
                ctx.Sepets.Add(yeniurun);
            }
            else
            {
                sepettekiurunler.Adet = sepettekiurunler.Adet + (adet??1);
                sepettekiurunler.Toplam = sepettekiurunler.Adet * urun.UrunFiyati;
            }
            ctx.SaveChanges();
            return RedirectToAction("Index");
        }

        public Boolean isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Admin")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public void menuAyar()
        {
            if (isAdminUser())
            {
                ViewBag.display = "block";
            }
            else
            {
                ViewBag.display = "None";
            }
        }
        public ActionResult SepetGuncelle(int id, int? adet)
        {
            Sepet sepet = ctx.Sepets.Find(id);
            Urunler urun = ctx.Urunlers.Find(sepet.RefUrunID);

            sepet.Adet = adet ?? 1;
            sepet.Toplam = sepet.Adet * urun.UrunFiyati;
            ctx.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Sepets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sepet sepet = ctx.Sepets.Find(id);
            if (sepet == null)
            {
                return HttpNotFound();
            }
            return View(sepet);
        }

        // GET: Sepets/Create
        public ActionResult Create()
        {
            ViewBag.RefKulID = new SelectList(ctx.AspNetUsers, "Id", "Email");
            ViewBag.RefUrunID = new SelectList(ctx.Urunlers, "UrunID", "UrunAdi");
            return View();
        }

        // POST: Sepets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SepetID,RefKulID,RefUrunID,Adet,Toplam")] Sepet sepet)
        {
            if (ModelState.IsValid)
            {
                ctx.Sepets.Add(sepet);
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RefKulID = new SelectList(ctx.AspNetUsers, "Id", "Email", sepet.RefKulID);
            ViewBag.RefUrunID = new SelectList(ctx.Urunlers, "UrunID", "UrunAdi", sepet.RefUrunID);
            return View(sepet);
        }

        // GET: Sepets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sepet sepet = ctx.Sepets.Find(id);
            if (sepet == null)
            {
                return HttpNotFound();
            }
            ViewBag.RefKulID = new SelectList(ctx.AspNetUsers, "Id", "Email", sepet.RefKulID);
            ViewBag.RefUrunID = new SelectList(ctx.Urunlers, "UrunID", "UrunAdi", sepet.RefUrunID);
            return View(sepet);
        }

        // POST: Sepets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SepetID,RefKulID,RefUrunID,Adet,Toplam")] Sepet sepet)
        {
            if (ModelState.IsValid)
            {
                ctx.Entry(sepet).State = EntityState.Modified;
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RefKulID = new SelectList(ctx.AspNetUsers, "Id", "Email", sepet.RefKulID);
            ViewBag.RefUrunID = new SelectList(ctx.Urunlers, "UrunID", "UrunAdi", sepet.RefUrunID);
            return View(sepet);
        }

        // GET: Sepets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sepet sepet = ctx.Sepets.Find(id);
            if (sepet == null)
            {
                return HttpNotFound();
            }
            ctx.Sepets.Remove(sepet);
            ctx.SaveChanges();
            return RedirectToAction("Index");
            //return View(sepet);
        }

        // POST: Sepets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sepet sepet = ctx.Sepets.Find(id);
            ctx.Sepets.Remove(sepet);
            ctx.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ctx.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
