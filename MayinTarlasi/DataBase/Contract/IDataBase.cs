using MayinTarlasi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MayinTarlasi.DataBase.Contract
{
    public interface IDataBase
    {
        void DataInsert(Player player);
        DataTable DataLoad();
        void CreateTable(OleDbConnection connection);
    }
}
