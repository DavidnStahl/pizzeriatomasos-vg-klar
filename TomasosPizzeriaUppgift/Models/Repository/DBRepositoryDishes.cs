using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosPizzeriaUppgift.Interface;
using TomasosPizzeriaUppgift.ViewModels;
using System.Data.SqlClient;

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;


namespace TomasosPizzeriaUppgift.Models.Repository
{
    public class DBRepositoryDishes : IRepositoryDishes
    {
        private readonly TomasosContext _context = new TomasosContext();
        public bool AddIngrediens(Produkt produkt)
        {
            using (TomasosContext db = new TomasosContext())
            {
                db.Produkt.FirstOrDefault(r => r.ProduktNamn == produkt.ProduktNamn);
                var ingrediens = db.Produkt.FirstOrDefault(r => r.ProduktNamn == produkt.ProduktNamn);
                if (ingrediens != null) { return false; }
                db.Produkt.Add(produkt).Context.SaveChanges();
                return true;
            }
       
        }
        public MenuPage CheckMatrattsValidation(MenuPage model)
        {
            using (TomasosContext db = new TomasosContext())
            {
                var matratt = db.Matratt.FirstOrDefault(r => r.MatrattNamn == model.NewDish.Matratt.MatrattNamn);
                if (matratt != null) { model.MatrattsnamnTaken = true; model.NewDish.MatrattnamnTaken = true; }
                return model;
            }
                
        }
        public void SaveIngrediensesToDish(Matratt dish)
        {
            using (TomasosContext db = new TomasosContext())
            {
                _context.Matratt.Update(dish).Context.SaveChanges();
            }              
        }
        public Matratt GetDishByName(string name)
        {
            using (TomasosContext db = new TomasosContext())
            {
                return db.Matratt.FirstOrDefault(r => r.MatrattNamn == name);
            }

        }

       public Matratt GetDishById(int id)
        {
            using (TomasosContext db = new TomasosContext())
            {
                return db.Matratt.Include(r => r.MatrattProdukt).FirstOrDefault(r => r.MatrattId == id);
            }
        }

        public List<Produkt> GetIngrdiensInMatratt(int id)
        {
            using (TomasosContext db = new TomasosContext())
            {
                return db.MatrattProdukt.Include(r => r.Produkt)
                                          .Where(r => r.MatrattId == id)
                                          .Select(r => r.Produkt).ToList();
            }
        }

        public void RemoveIngrediens(int id)
        {
            using (TomasosContext db = new TomasosContext())
            {
                var ingrediensemodel = db.MatrattProdukt.Where(r => r.ProduktId == id);
                var model = db.Produkt.FirstOrDefault(r => r.ProduktId == id);
                db.MatrattProdukt.RemoveRange(ingrediensemodel);
                db.Produkt.Remove(model);
                db.SaveChanges();
            }
                 
        }
        public void Delete(int id)
        {
            using (TomasosContext db = new TomasosContext())
            {
                var dish = db.Matratt.Include(r => r.MatrattProdukt)
                                       .Include(r => r.BestallningMatratt)
                                       .FirstOrDefault(r => r.MatrattId == id);

                db.MatrattProdukt.RemoveRange(dish.MatrattProdukt.ToList());
                db.BestallningMatratt.RemoveRange(dish.BestallningMatratt.ToList());
                db.Matratt.Remove(dish);
                db.SaveChanges();
            }
                

            
        }
        public void Create(Matratt dish)
        {
            using (TomasosContext db = new TomasosContext())
            {
                db.Matratt.Add(dish).Context.SaveChanges();
            }
                
        }
        public void DeleteMatrattProduktList(List<MatrattProdukt> model)
        {
            using (TomasosContext db = new TomasosContext())
            {
                db.MatrattProdukt.RemoveRange(model);
                db.SaveChanges();
            }             
        }
        public void UpdateIngrediensesInDish(Matratt model)
        {
            using (TomasosContext db = new TomasosContext())
            {
                db.Matratt.Update(model).Context.SaveChanges();
            }
        }
        public List<MatrattProdukt> GetOldIngredienses(int id)
        {
            using (TomasosContext db = new TomasosContext())
            {
                return db.MatrattProdukt.Where(r => r.MatrattId == id).ToList();
            }
            
        }
        public void Update(Matratt model)
        {
            using (TomasosContext db = new TomasosContext())
            {
                db.Matratt.Update(model);
                db.MatrattProdukt.AddRange(model.MatrattProdukt.ToList());             
                db.SaveChanges();
            }

        }
        public void UpdateDishIngredienses(List<MatrattProdukt> matrattprodukt)
        {
            using (TomasosContext db = new TomasosContext())
            {
                db.MatrattProdukt.AddRange(matrattprodukt);
                db.SaveChanges();
            }
            
        }

        public List<MatrattProdukt> GetMatrattProduktByMatrattID(int id)
        {
            using (TomasosContext db = new TomasosContext())
            {
                var model = db.MatrattProdukt.Where(r => r.MatrattId == id).ToList();
                return model;
            }
        }

        public void UpdateOnlymatratt(Matratt dish)
        {
            using (TomasosContext db = new TomasosContext())
            {
                db.Matratt.Update(dish);
                db.SaveChanges();
            }
                
        }
    }
}
