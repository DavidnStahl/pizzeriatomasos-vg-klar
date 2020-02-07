using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosPizzeriaUppgift.Interface;
using TomasosPizzeriaUppgift.ViewModels;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace TomasosPizzeriaUppgift.Models.Repository
{
    public class DBRepositoryCustomers : IRepositoryCustomers
    {

        private readonly TomasosContext _context = new TomasosContext();
       
        public void Create(Kund customer)
        {
            using (TomasosContext db = new TomasosContext())
            {
                db.Kund.Add(customer).Context.SaveChanges();
            }

        }
        public void Update(Kund customer)
        {
            using (TomasosContext db = new TomasosContext())
            {
               db.Kund.Update(customer).Context.SaveChanges();
            }
        }
        public Kund GetByUsername(string userName)
        {
            using (TomasosContext db = new TomasosContext())
            {
                return db.Kund.FirstOrDefault(r => r.AnvandarNamn == userName);
            }
               

        }
        public List<Kund> GetAll()
        {
            using(TomasosContext db = new TomasosContext())
            {
                return db.Kund.ToList();
            }

        }
        public void Delete(Kund c)
        {
            using (TomasosContext db = new TomasosContext())
            {
                var order = db.Bestallning.Where(r => r.KundId == c.KundId);
                var bestallningmatratt = db.BestallningMatratt.Where(r => r.Bestallning.KundId == c.KundId);

                db.BestallningMatratt.RemoveRange(bestallningmatratt);
                db.Bestallning.RemoveRange(order);
                db.Kund.Remove(c);
                db.SaveChanges();
            }
        }
    }
}
