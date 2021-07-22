using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Windows.Forms;

namespace TreeViewWpfDB
{
    public class CE_Context : DbContext
    {
        static CE_Context instance;
        public static CE_Context GetInstance(string connectionName) //паттерн Singleton
        {
            if (instance == null)
                instance = new CE_Context(connectionName);
            return instance;
        }
        public void Disconnect()
        {
            Database.Connection.Close();
        }
        public CE_Context(string connectionName) : base(connectionName) { }
        public virtual DbSet<CE_Tgroup> CE_Tgroup_Property { get; set; }
        public virtual DbSet<CE_Trelation> CE_Trelation_Property { get; set; }
        public virtual DbSet<CE_Tproperty> CE_Tproperty_Property { get; set; }
    }
}
