using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using WEDEPX_DB.Models;

namespace WEDEPX_DB.Dao
{
    public class WedepxDbHelper : DaoDb
    {
        private readonly WEDEPXEntities _db;
        public WedepxDbHelper()
        {
            _db = GetConnection();

        }
        public List<bd_emp> Get()
        {
            return _db.bd_emp.ToList();
        }
        public void Save(bd_emp emp)
        {
            try
            {

                var maxEmp =  _db.bd_emp.Max(x => x.EMP_CODE);
                emp.EMP_CODE = maxEmp + 1;

                _db.bd_emp.Add(emp);
                _db.SaveChanges();

            }
            catch (Exception ex)
            {

            }
        }
        public void Update(bd_emp emp)
        {

            try
            {
                _db.bd_emp.AddOrUpdate(emp);
                _db.SaveChanges();

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public bd_emp Delete(int EMP_CODE)
        {
            var del = new bd_emp();
            try
            {
                var emp = Convert.ToInt32(EMP_CODE);
                del = _db.bd_emp.Where(x => x.EMP_CODE == emp).FirstOrDefault();

                _db.bd_emp.Remove(del);
                _db.SaveChanges();
            }
            catch (Exception ex) { throw ex; }
            return del;

        }

    }
}