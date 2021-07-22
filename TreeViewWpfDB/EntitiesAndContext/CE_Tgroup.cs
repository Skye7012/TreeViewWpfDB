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
    [Table("TGROUP")]
    public class CE_Tgroup
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        public string GetText() => Name;
        public string GetName() => (Id + "|Group");
        public TreeNode ConvertToTreeNode()
        {
            return new TreeNode()
            {
                Text = GetText(),
                Name = GetName()
            };
        }
    }
}
