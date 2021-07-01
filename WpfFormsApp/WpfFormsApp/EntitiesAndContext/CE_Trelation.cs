using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Windows.Forms;

namespace WpfFormsApp
{
    [Table("TRELATION")]
    public class CE_Trelation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("id_parent", Order = 1)]
        public int Id_parent { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("id_child", Order = 2)]
        public int Id_child { get; set; }

    }
}
