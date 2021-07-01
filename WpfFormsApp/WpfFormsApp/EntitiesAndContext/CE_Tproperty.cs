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
    [Table("TPROPERTY")]
    public class CE_Tproperty
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("value")]
        public string Value { get; set; }
        [Column("group_id")]
        public int Group_id { get; set; }
        public string GetText() => Name;
        public string GetName() => (Id + "|Property");
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
